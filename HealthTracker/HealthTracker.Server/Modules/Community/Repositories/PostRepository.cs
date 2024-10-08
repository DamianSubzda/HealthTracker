﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using HealthTracker.Server.Core.Exceptions;
using HealthTracker.Server.Core.Exceptions.Community;
using HealthTracker.Server.Core.Models;
using HealthTracker.Server.Infrastructure.Data;
using HealthTracker.Server.Infrastructure.Services;
using HealthTracker.Server.Modules.Community.DTOs;
using HealthTracker.Server.Modules.Community.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthTracker.Server.Modules.Community.Repositories
{
    public interface IPostRepository
    {
        Task<PostDTO> CreatePost(CreatePostDTO postDTO);
        Task<PostDTO> GetPost(int postId);
        Task DeletePost(int postId);
        Task<List<PostDTO>> GetPosts(int userId, int pageNumber, int pageSize);
        Task<List<PostDTO>> GetUserPosts(int userId, int pageNumber, int pageSize);
        Task<CommentDTO> CreateComment(int? parentCommentId, CreateCommentDTO commentDTO);
        Task<CommentDTO> GetComment(int commentId);
        Task<CommentFromPostDTO> GetCommentsByPostId(int postId, int pageNr, int pageSize);
        Task<List<CommentDTO>> GetCommentsByParentCommentId(int postId, int parentCommentId);
        Task DeleteComment(int commentId);
        Task DeleteCommentsFromPost(int postId);
        Task DeleteUserComments(int userId);
        Task<LikeDTO> CreateLike(LikeDTO likeDTO);
        Task<LikeDTO> GetLike(int userId, int postId);
        Task<List<LikeDTO>> GetLikesFromPost(int postId);
        Task DeleteLike(int userId, int postId);
    }
    public class PostRepository : IPostRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;

        public PostRepository(ApplicationDbContext context, IMapper mapper, IFileService fileService)
        {
            _context = context;
            _mapper = mapper;
            _fileService = fileService;
        }

        public async Task<PostDTO> CreatePost(CreatePostDTO createPostDTO)
        {
            var user = await _context.User.FirstOrDefaultAsync(line => line.Id == createPostDTO.UserId);
            if (user == null)
            {
                throw new UserNotFoundException();
            }

            var post = _mapper.Map<Post>(createPostDTO);

            if (createPostDTO.ImageFile != null)
            {
                post.ImageURL = _fileService.SaveFile(createPostDTO.ImageFile, "Modules\\Community\\Assets\\PostAttachments");
            }

            await _context.Post.AddAsync(post);
            await _context.SaveChangesAsync();

            var postDTO = _mapper.Map<PostDTO>(post);

            postDTO.UserFirstName = user.FirstName;
            postDTO.UserLastName = user.LastName;

            return postDTO;
        }

        public async Task<PostDTO> GetPost(int postId)
        {
            var post = await _context.Post
                .Include(p => p.User)
                .Include(p => p.Likes)
                .FirstOrDefaultAsync(p => p.Id == postId) ?? throw new PostNotFoundException();

            var postDto = _mapper.Map<PostDTO>(post);
            postDto.AmountOfComments = await _context.Comment.CountAsync(c => c.PostId == postId);

            return postDto;
        }

        public async Task DeletePost(int postId)
        {
            var post = await _context.Post.FindAsync(postId);

            if (post == null)
            {
                throw new PostNotFoundException();
            }

            var likes = await _context.Like.Where(line => line.PostId == postId).ToListAsync();
            var comments = await _context.Comment.Where(line => line.PostId == postId).ToListAsync();

            if (post.ImageURL != null)
            {
                _fileService.DeleteFile(post.ImageURL);
            }

            _context.Like.RemoveRange(likes);
            _context.Comment.RemoveRange(comments);
            _context.Post.Remove(post);

            await _context.SaveChangesAsync();
        }

        public async Task<List<PostDTO>> GetPosts(int userId, int pageNumber, int pageSize)
        {
            if (!await _context.User.AnyAsync(u => u.Id == userId))
            {
                throw new UserNotFoundException();
            }

            var friendIds = await _context.Friendship
                .Where(f => (f.UserId == userId || f.FriendId == userId) && f.Status == Status.Accepted)
                .Select(f => f.UserId == userId ? f.FriendId : f.UserId)
                .Distinct()
                .ToListAsync();

            var posts = await _context.Post
                .Where(p => friendIds.Contains(p.UserId))
                .OrderByDescending(p => p.DateOfCreate)
                .Include(p => p.User)
                .Include(p => p.Comments)
                .Include(p => p.Likes)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .Select(p => new PostDTO
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    UserFirstName = p.User.FirstName,
                    UserLastName = p.User.LastName,
                    Content = p.Content,
                    DateOfCreate = p.DateOfCreate,
                    ImageURL = p.ImageURL,
                    AmountOfComments = p.Comments.Count(line => line.ParentCommentId == null),
                    Likes = p.Likes.Select(l => _mapper.Map<LikeDTO>(l)).ToList()
                })
                .ToListAsync();

            if (posts.Count == 0)
            {
                throw new NullPageException();
            }

            return posts;
        }

        public async Task<List<PostDTO>> GetUserPosts(int userId, int pageNumber, int pageSize)
        {
            if (!await _context.User.AnyAsync(u => u.Id == userId))
            {
                throw new UserNotFoundException();
            }

            var posts = await _context.Post
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.DateOfCreate)
                .Include(p => p.User)
                .Include(p => p.Comments)
                .Include(p => p.Likes)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .Select(p => new PostDTO
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    UserFirstName = p.User.FirstName,
                    UserLastName = p.User.LastName,
                    Content = p.Content,
                    DateOfCreate = p.DateOfCreate,
                    ImageURL = p.ImageURL,
                    AmountOfComments = p.Comments.Count(line => line.ParentCommentId == null),
                    Likes = p.Likes.Select(l => _mapper.Map<LikeDTO>(l)).ToList()
                })
                .ToListAsync();

            if (posts.Count == 0)
            {
                throw new NullPageException();
            }

            return posts;
        }

        public async Task<CommentDTO> CreateComment(int? parentCommentId, CreateCommentDTO createCommentDTO)
        {
            if (parentCommentId.HasValue)
            {
                var parentComment = await GetComment(parentCommentId.Value);
                if (parentComment == null)
                {
                    throw new CommentNotFoundException();
                }
            }

            var user = await _context.User.FirstOrDefaultAsync(line => line.Id == createCommentDTO.UserId);

            if (user == null)
            {
                throw new UserNotFoundException();
            }

            if (!await _context.Post.AnyAsync(line => line.Id == createCommentDTO.PostId))
            {
                throw new PostNotFoundException();
            }

            var comment = _mapper.Map<Comment>(createCommentDTO);
            comment.ParentCommentId = parentCommentId;

            await _context.Comment.AddAsync(comment);
            await _context.SaveChangesAsync();

            var commentDTO = _mapper.Map<CommentDTO>(comment);

            commentDTO.UserFirstName = user.FirstName;
            commentDTO.UserLastName = user.LastName;
            commentDTO.AmountOfChildComments = 0;

            return commentDTO;
        }

        public async Task<CommentDTO> GetComment(int commentId)
        {
            var commentDTO = await _context.Comment
                .Where(line => line.Id == commentId)
                .ProjectTo<CommentDTO>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync() ?? throw new CommentNotFoundException();

            commentDTO.AmountOfChildComments = await _context.Comment
                    .CountAsync(c => c.ParentCommentId == commentDTO.Id);

            return commentDTO;
        }

        public async Task<CommentFromPostDTO> GetCommentsByPostId(int postId, int pageNr, int pageSize)
        {
            if (!await _context.Post.AnyAsync(line => line.Id == postId))
            {
                throw new PostNotFoundException();
            }

            var commentsDTO = await _context.Comment
                .Where(comment => comment.PostId == postId && comment.ParentCommentId == null)
                .OrderByDescending(comment => comment.DateOfCreate)
                .Skip((pageNr - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<CommentDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();

            if (commentsDTO.Count == 0)
            {
                throw new NullPageException();
            }

            var childCommentCounts = await _context.Comment
                .Where(c => c.ParentCommentId.HasValue && commentsDTO.Select(dto => dto.Id).Contains(c.ParentCommentId.Value))
                .GroupBy(c => c.ParentCommentId.Value)
                .Select(g => new { ParentCommentId = g.Key, Count = g.Count() })
                .ToListAsync();

            foreach (var comment in commentsDTO)
            {
                comment.AmountOfChildComments = childCommentCounts.FirstOrDefault(c => c.ParentCommentId == comment.Id)?.Count ?? 0;
            }

            var totalCommentsCount = await _context.Comment
                .CountAsync(comment => comment.PostId == postId && comment.ParentCommentId == null);

            var totalCommentsLeft = totalCommentsCount - pageNr * pageSize > 0 ? totalCommentsCount - pageNr * pageSize : 0;

            return new CommentFromPostDTO()
            {
                Comments = commentsDTO,
                PageNr = pageNr,
                PageSize = pageSize,
                PostId = postId,
                TotalCommentsLeft = totalCommentsLeft
            };
        }

        public async Task<List<CommentDTO>> GetCommentsByParentCommentId(int postId, int parentCommentId)
        {
            if (!await _context.Post.AnyAsync(post => post.Id == postId))
            {
                throw new PostNotFoundException();
            }

            if (!await _context.Comment.AnyAsync(comment => comment.Id == parentCommentId))
            {
                throw new CommentNotFoundException();
            }

            var commentsDTO = await _context.Comment
                .Where(comment => comment.PostId == postId && comment.ParentCommentId == parentCommentId)
                .OrderByDescending(comment => comment.DateOfCreate)
                .ProjectTo<CommentDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();

            var childCommentCounts = await _context.Comment
                .Where(c => c.ParentCommentId.HasValue && commentsDTO.Select(dto => dto.Id).Contains(c.ParentCommentId.Value))
                .GroupBy(c => c.ParentCommentId.Value)
                .Select(g => new { ParentCommentId = g.Key, Count = g.Count() })
                .ToListAsync();

            foreach (var comment in commentsDTO)
            {
                comment.AmountOfChildComments = childCommentCounts.FirstOrDefault(c => c.ParentCommentId == comment.Id)?.Count ?? 0;
            }

            return commentsDTO;
        }


        public async Task DeleteComment(int commentId)
        {
            var comment = await _context.Comment.FindAsync(commentId);
            if (comment != null)
            {
                await DeleteChildComments(comment.Id);
                _context.Comment.Remove(comment);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new CommentNotFoundException();
            }
        }

        private async Task DeleteChildComments(int parentId)
        {
            var childComments = await _context.Comment.Where(c => c.ParentCommentId == parentId).ToListAsync();
            foreach (var child in childComments)
            {
                await DeleteChildComments(child.Id);
            }
            _context.Comment.RemoveRange(childComments);
        }

        public async Task DeleteCommentsFromPost(int postId)
        {
            if (!await _context.Post.AnyAsync(line => line.Id == postId))
            {
                throw new PostNotFoundException();
            }
            var comments = await _context.Comment
                .Where(line => line.PostId == postId)
                .ToListAsync();

            _context.RemoveRange(comments);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserComments(int userId)
        {
            if (!await _context.User.AnyAsync(line => line.Id == userId))
            {
                throw new UserNotFoundException();
            }

            var comments = await _context.Comment
                .Where(line => line.UserId == userId)
                .ToListAsync();

            _context.RemoveRange(comments);
            await _context.SaveChangesAsync();
        }

        public async Task<LikeDTO> CreateLike(LikeDTO likeDTO)
        {
            if (await _context.Like.AnyAsync(p => p.UserId == likeDTO.UserId && p.PostId == likeDTO.PostId))
            {
                throw new LikeAlreadyExistsException();
            }

            if (!await _context.User.AnyAsync(line => line.Id == likeDTO.UserId))
            {
                throw new UserNotFoundException();
            }

            if (!await _context.Post.AnyAsync(line => line.Id == likeDTO.PostId))
            {
                throw new PostNotFoundException();
            }
            var like = _mapper.Map<Like>(likeDTO);

            await _context.Like.AddAsync(like);
            await _context.SaveChangesAsync();

            return likeDTO;
        }

        public async Task<LikeDTO> GetLike(int userId, int postId)
        {
            var like = await _context.Like
                 .FirstOrDefaultAsync(p => p.UserId == userId && p.PostId == postId);

            var likeDto = _mapper.Map<LikeDTO>(like);

            return likeDto ?? throw new LikeNotFoundException();
        }

        public async Task<List<LikeDTO>> GetLikesFromPost(int postId)
        {
            var likes = await _context.Like
                .Where(like => like.PostId == postId)
                .ProjectTo<LikeDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();
            return likes;
        }

        public async Task DeleteLike(int userId, int postId)
        {
            var like = await _context.Like
                .FirstOrDefaultAsync(p => p.UserId == userId && p.PostId == postId);
            if (like != null)
            {
                _context.Like.Remove(like);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new LikeNotFoundException();
            }
        }

    }
}

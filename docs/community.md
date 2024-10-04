# Community Documentation

## ✨ Features

TODO

## 🖼️ Page view
TODO
![Image 1](images/)

## 🌐 API Reference

### FriendshipController

#### Create a Friendship Request:

```http
POST /api/users/friends
```
| Parameter | Type     | Location | Description                |
| :-------- | :------- |:---------| :------------------------- |
| `UserId` | `int` | Body |**Required**. The ID of the user sending the request |
| `FriendId` | `int` | Body |**Required**. The ID of the friend receiving the request |

#### Get a Friendship:

```http
GET /api/users/friends/{friendshipId}
```
| Parameter | Type     | Location | Description                |
| :-------- | :------- |:---------| :------------------------- |
| `friendshipId` | `int` | Path |**Required**. The ID of the friendship |

#### Get a Friendship by Users id:

```http
GET /api/users/{userId}/friends/{friendId}
```

| Parameter | Type     | Location | Description                |
| :-------- | :------- |:---------| :------------------------- |
| `userId` | `int` | Path |**Required**. The ID of the user in friendship |
| `friendId` | `int` | Path |**Required**. The ID of the friend in friendship |

#### Get a list of friends for user:

```http
GET /api/users/{userId}/friends
```

| Parameter | Type     | Location | Description                |
| :-------- | :------- |:---------| :------------------------- |
| `userId` | `int` | Path |**Required**. The ID of the user    |

#### Get a list of frienships requests for user:

```http
GET /api/users/{userId}/friends/requests
```

| Parameter | Type     | Location | Description                |
| :-------- | :------- |:---------| :------------------------- |
| `userId` | `int` | Path |**Required**. The ID of the user    |

#### Accept a frienship request:

```http
GET /api/users/{userId}/friends/{friendId}/accept
```

| Parameter | Type     | Location | Description                |
| :-------- | :------- |:---------| :------------------------- |
| `userId` | `int` | Path |**Required**. The ID of the user    |
| `friendId` | `int` | Path |**Required**. The ID of the friend|

#### Decline a frienship request:

```http
GET /api/users/{userId}/friends/{friendId}/decline
```

| Parameter | Type     | Location | Description                |
| :-------- | :------- |:---------| :------------------------- |
| `userId` | `int` | Path |**Required**. The ID of the user    |
| `friendId` | `int` | Path |**Required**. The ID of the friend|

#### Delete a friendship:

```http
DELETE /api/users/{userId}/friends/{friendId}
```

| Parameter | Type     | Location | Description                |
| :-------- | :------- |:---------| :------------------------- |
| `userId` | `int` | Path |**Required**. The ID of the user    |
| `friendId` | `int` | Path |**Required**. The ID of the friend|

### ChatController

#### Create a message:

```http
POST /api/users/messages
```

| Parameter | Type     | Location | Description                |
| :-------- | :------- |:---------| :------------------------- |
| `UserIdFrom` | `int` | Body |**Required**. The ID of the user creating (sending) message|
| `UserIdTo` | `int` | Body |**Required**. The ID of the user receiving message|
| `Text` | `string` | Body |**Required**. The text of message |

#### Get a message:

```http
GET api/users/messages/{messageId}
```

| Parameter | Type     | Location | Description                |
| :-------- | :------- |:---------| :------------------------- |
| `messageId` | `int` | Path |**Required**. The ID of the message|

#### Get a list of messages:

```http
GET api/users/messages/{userFrom}/{userTo}
```

| Parameter | Type     | Location | Description                |
| :-------- | :------- |:---------| :------------------------- |
| `userFrom` | `int` | Path |**Required**. The ID of the user creating (sending) message|
| `userTo` | `int` | Path |**Required**. The ID of the user receiving message|
| `pageNumber` | `int` | Query |**Required**. The number of returned page|
| `pageSize` | `int` | Query |**Optional**. The size of returned page|

#### Get a number Of New Messages for user:

```http
GET api/users/messages/{userFrom}/{userTo}/new
```

| Parameter | Type     | Location | Description                |
| :-------- | :------- |:---------| :------------------------- |
| `userFrom` | `int` | Path |**Required**. The ID of the user creating (sending) message|
| `userTo` | `int` | Path |**Required**. The ID of the user receiving message|

#### Update Messages To Readed status:

```http
PUT api/users/messages/{userFrom}/{userTo}
```

| Parameter | Type     | Location | Description                |
| :-------- | :------- |:---------| :------------------------- |
| `userFrom` | `int` | Path |**Required**. The ID of the user sending message|
| `userTo` | `int` | Path |**Required**. The ID of the user receiving message|

### PostController

#### Create post:

```http
POST api/users/posts
```

| Parameter | Type     | Location | Description                |
| :-------- | :------- |:---------| :------------------------- |
| `UserId` | `int` | Body |**Required**. The ID of the user creating post|
| `Content` | `int` | Body |**Required**. The post text content|
| `ImageFile` | `IFormFile?` | Body |**Optional**. The image file in post attachment|

#### Get the post:

```http
GET api/users/posts/{postId}
```

| Parameter | Type     | Location | Description                |
| :-------- | :------- |:---------| :------------------------- |
| `postId` | `int` | Body |**Required**. The ID of the post|

#### Get the list of the posts on users wall:

```http
GET api/users/{userId}/wall/posts
```

| Parameter | Type     | Location | Description                |
| :-------- | :------- |:---------| :------------------------- |
| `userId` | `int` | Path |**Required**. The ID of the user |
| `pageNumber` | `int` | Query |**Required**. The page number of posts|
| `pageSize` | `int` | Query |**Optional**. The page size|


#### Get the list of users` posts:

```http
GET api/users/{userId}/posts
```

| Parameter | Type     | Location | Description                |
| :-------- | :------- |:---------| :------------------------- |
| `userId` | `int` | Path |**Required**. The ID of the user |
| `pageNumber` | `int` | Query |**Required**. The page number of posts|
| `pageSize` | `int` | Query |**Optional**. The page size|

#### Delete post:

```http
DELETE api/users/posts/{postId}
```

| Parameter | Type     | Location | Description                |
| :-------- | :------- |:---------| :------------------------- |
| `postId` | `int` | Body |**Required**. The ID of the post|

#### Create like (like post):

```http
POST api/users/posts/likes
```

| Parameter | Type     | Location | Description                |
| :-------- | :------- |:---------| :------------------------- |
| `UserId` | `int` | Body |**Required**. The ID of the user    |
| `PostId` | `int` | Body |**Required**. The ID of the post    |

#### Get like:

```http
GET api/users/{userId}/posts/{postId}/likes
```

| Parameter | Type     | Location | Description                |
| :-------- | :------- |:---------| :------------------------- |
| `userId` | `int` | Path |**Required**. The ID of the user    |
| `postId` | `int` | Path |**Required**. The ID of the post    |

#### Get likes from post:

```http
GET api/users/posts/{postId}/likes
```

| Parameter | Type     | Location | Description                |
| :-------- | :------- |:---------| :------------------------- |
| `postId` | `int` | Path |**Required**. The ID of the post    |

#### Delete like:

```http
DELETE api/users/{userId}/posts/{postId}/likes
```

| Parameter | Type     | Location | Description                |
| :-------- | :------- |:---------| :------------------------- |
| `userId` | `int` | Path |**Required**. The ID of the user    |
| `postId` | `int` | Path |**Required**. The ID of the post    |

### CommentController
TODO



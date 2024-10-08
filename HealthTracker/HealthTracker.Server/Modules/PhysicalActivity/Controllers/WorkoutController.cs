﻿using HealthTracker.Server.Core.DTOs;
using HealthTracker.Server.Core.Exceptions.PhysicalActivity;
using HealthTracker.Server.Modules.PhysicalActivity.DTOs;
using HealthTracker.Server.Modules.PhysicalActivity.Models;
using HealthTracker.Server.Modules.PhysicalActivity.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthTracker.Server.Modules.PhysicalActivity.Controllers
{
    [Route("api")]
    [ApiController]
    [Authorize]
    public class WorkoutController : ControllerBase
    {
        private readonly IWorkoutRepository _workoutRepository;
        private readonly ILogger<WorkoutController> _logger;
        public WorkoutController(IWorkoutRepository workoutRepository, ILogger<WorkoutController> logger) 
        {
            _workoutRepository = workoutRepository;
            _logger = logger;
        }

        [HttpPost("users/workouts")]
        public async Task<ActionResult> CreateExercise([FromBody] CreateWorkoutDTO createWorkoutDTO)
        {
            try
            {
                var result = await _workoutRepository.CreateWorkout(createWorkoutDTO);
                return CreatedAtAction(nameof(GetWorkout), new { id = result.Id }, result);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error occurred during the create exercise process for {DTO}.", createWorkoutDTO);
                return BadRequest(ex.InnerException?.Message ?? "Database error.");
            }
            catch (ExerciseTypeNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during the create exercise process for {DTO}.", createWorkoutDTO);
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpPost("users/workouts/exercises")]
        public async Task<ActionResult> AddExerciseToWorkout([FromQuery] int workoutId, [FromQuery] int exerciseId)
        {
            try
            {
                await _workoutRepository.AddExerciseToWorkout(workoutId, exerciseId);
                return Ok();
            }
            catch (Exception ex) when (ex is ExerciseNotFoundException || ex is WorkoutNotFoundException || ex is ExerciseAlreadyExistsInWorkout)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during the add exercise to workout process for workout: {workoutId} and exercise: {exerciseId}.", workoutId, exerciseId);
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet("users/workouts/{id}")]
        public async Task<ActionResult<WorkoutDTO>> GetWorkout(int id)
        {
            try
            {
                var result = await _workoutRepository.GetWorkout(id);
                return Ok(result);
            }
            catch (ExerciseNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during the get workout process for {WorkoutId}.", id);
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpDelete("users/workouts/{id}")]
        public async Task<ActionResult> DeleteWorkout(int id)
        {
            try
            {
                await _workoutRepository.DeleteWorkout(id);
                return Ok();
            }
            catch (WorkoutNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during the delete workout process for {WorkoutId}.", id);
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}

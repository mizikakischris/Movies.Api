﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Movie.Api.Dtos;
using Movie.Api.Models;
using Movie.Api.Repository.IRepository;

namespace Movie.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieModelRepository _repo;
        private readonly IMapper _mapper;

        public MoviesController(IMovieModelRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }


        /// <summary>
        /// Get list of movies.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetMovies()
        {
            var moviesList = _repo.GetMovies();
            var movieDtos = new List<MovieDto>();
            foreach (var movie in moviesList)
            {
                movieDtos.Add(_mapper.Map<MovieDto>(movie));
            }
            return Ok(movieDtos);
        }

     
        [HttpGet("{movieId:int}", Name = "GetMovie")]
        public IActionResult GetMovie(int movieId)
        {
            var movie = _repo.GetMovieModel(movieId);
            if (movie == null)
            {
                return NotFound();
            }

            var movieDto = _mapper.Map<MovieDto>(movie);
            return Ok(movieDto);
        }

        [HttpPost]
        public IActionResult CreateMovie([FromBody] MovieDto movieDto)
        {
            if (movieDto == null)
            {
                return BadRequest(ModelState);
            }
            if (_repo.MovieModelExists(movieDto.Name))
            {
                ModelState.AddModelError("", "Movie Exists!");
                return StatusCode(404, ModelState);
            }
            var movieObj = _mapper.Map<MovieModel>(movieDto);
            if (!_repo.CreateMovieModel(movieObj))
            {
                ModelState.AddModelError("", $"Something went wrong when saving the record {movieObj.Name}");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("GetMovie", new { movieId = movieObj.Id }, movieObj);
        }

        [HttpPatch("{movieId:int}", Name = "UpdateMovie")]
        public IActionResult UpdateMovie(int movieId, [FromBody] MovieDto movieDto)
        {
            if (movieDto == null || movieId != movieDto.Id)
            {
                return BadRequest(ModelState);
            }

            var movieObj = _mapper.Map<MovieModel>(movieDto);
            if (!_repo.UpdateMovieModel(movieObj))
            {
                ModelState.AddModelError("", $"Something went wrong when updating the record {movieObj.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();

        }


        [HttpDelete("{movieId:int}", Name = "DeleteMovie")]
        public IActionResult DeleteMovie(int movieId)
        {
            if (!_repo.MovieModelExists(movieId))
            {
                return NotFound();
            }

            var movieObj = _repo.GetMovieModel(movieId);
            if (!_repo.DeleteMovieModel(movieObj))
            {
                ModelState.AddModelError("", $"Something went wrong when deleting the record {movieObj.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();

        }
    }
}
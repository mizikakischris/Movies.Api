﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Movie.Types.Dtos;
using Movie.Api.Exceptions;
using Movie.Types.Models;
using Movie.Interfaces;
using Movie.Types.Responses;
using AutoMapper;
using System.Linq;

namespace Movie.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _service;
        private readonly IMapper _mapper;
        public MoviesController(IMovieService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }


        /// <summary>
        /// Get list of movies.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<MovieDto>))]
        public ActionResult<Response<MovieDto>> GetMovies()
        {
            var moviesList = _service.GetMovies();

            Response<MovieDto> response = new Response<MovieDto>
            {
                Payload = new Payload<MovieDto>
                {
                    PayloadObjects = moviesList
                }
            };
            return Ok(response);
        }

        /// <summary>
        /// Get individual movie
        /// </summary>
        /// <param name="movieId"> The Id of the movie </param>
        /// <returns></returns>
        [HttpGet("{movieId:int}", Name = "GetMovie")]
        [ProducesResponseType(200, Type = typeof(MovieDto))]
        [ProducesResponseType(404)]
        public ActionResult<Response<MovieDto>> GetMovie(int movieId)
        {
            try
            {

                var movie = _service.GetMovieModel(movieId);
                if (movie == null)
                {
                    throw new ErrorDetails
                    {
                        Description = $"Not found item with Id {movieId}",
                        StatusCode = StatusCodes.Status404NotFound,
                    };

                }

                Response<MovieDto> response = new Response<MovieDto>
                {

                    Payload = new Payload<MovieDto> { PayloadObject = movie }
                };
                return Ok(response);


            }
            catch (ErrorDetails ex)
            {
                Response<MovieDto> response = new Response<MovieDto>
                {
                    Payload = null,
                    Exception = ex
                };
                return response;
            }

        }

        [HttpGet("[action]/{actorId:int}")]
        [ProducesResponseType(200, Type = typeof(MovieDto))]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public ActionResult<Response<MovieDto>> GetMoviesByActor(int actorId)
        {
            try
            {
                var movieModels = _service.GetMoviesByActor(actorId);
                if (movieModels == null)
                {
                    throw new ErrorDetails
                    {
                        Description = $"Movies Not found for Id {actorId}",
                        StatusCode = StatusCodes.Status404NotFound,
                    };
                }
                var movieDtos = new List<MovieDto>();
                foreach (var movie in movieModels)
                {
                    movieDtos.Add(_mapper.Map<MovieDto>(movie));
                }
                Response<MovieDto> response = new Response<MovieDto>
                {
                    Payload = new Payload<MovieDto>
                    {
                        PayloadObjects = movieDtos
                    }
                };
                return Ok(response);
            }
            catch (ErrorDetails ex)
            {

                Response<MovieDto> response = new Response<MovieDto>
                {
                    Payload = null,
                    Exception = ex
                };
                return response;
            }


        }

        /// <summary>
        /// Create movie
        /// </summary>
        /// <param name="movieDto"> The Dto movie </param>
        /// <returns></returns>
        //api/movies?actorId=1&actorId=2&catId=1&catId=2
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(MovieDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<Response<MovieDto>> CreateMovie([FromBody] MovieDto movieDto, List<int> actorIds)
        {
            try
            {

                ValidateMovie(actorIds, movieDto);

                var movieModel = _service.CreateMovieModel(movieDto, actorIds);
                Response<MovieDto> response = new Response<MovieDto>
                {

                    Payload = new Payload<MovieDto>
                    {
                        PayloadObject = new MovieDto
                        {
                            BoxOffice = movieModel.BoxOffice,
                            Id = movieModel.Id,
                            Name = movieModel.Title,
                            Picture = movieModel.Picture,
                            ReleaseDate = movieModel.ReleaseDate.Value
                        }
                    }
                };
                return CreatedAtRoute("GetMovie", new { movieId = movieModel.Id }, response);
            }
            catch (ErrorDetails ex)
            {
                Response<MovieDto> resp = new Response<MovieDto>
                {
                    Payload = null,
                    Exception = ex
                };
                return resp;
            }
        }

        /// <summary>
        /// Update individual movie
        /// </summary>
        /// <param name="movieId"> The Id of the movie </param>
        /// <param name="movieDto"> The Dto movie </param>
        /// <returns></returns>
        [HttpPatch("{movieId:int}", Name = "UpdateMovie")]
        [ProducesResponseType(204)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateMovie(int movieId, [FromBody] MovieDto movieDto)
        {
            if (movieDto == null || movieId != movieDto.Id)
            {
                return BadRequest(ModelState);
            }

            var movieObj = _mapper.Map<MovieModel>(movieDto);
            if (!_service.UpdateMovieModel(movieObj))
            {
                ModelState.AddModelError("", $"Something went wrong when updating the record {movieObj.Title}");
                return StatusCode(500, ModelState);
            }

            return NoContent();

        }

        /// <summary>
        /// Delete individual movie
        /// </summary>
        /// <param name="movieId"> The Id of the movie </param>
        /// <returns></returns>
        [HttpDelete("{movieId:int}", Name = "DeleteMovie")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteMovie(int movieId)
        {
            if (!_service.MovieModelExists(movieId))
            {
                return NotFound();
            }

            var movieObj = _service.GetTheMovieModel(movieId);
            if (!_service.DeleteMovieModel(movieObj))
            {
                ModelState.AddModelError("", $"Something went wrong when deleting the record {movieObj.Title}");
                return StatusCode(500, ModelState);
            }

            return NoContent();

        }

        private StatusCodeResult ValidateMovie(List<int> actorsId,  MovieDto movie)
        {
            if (movie == null || actorsId.Count() <= 0 )
            {
                ModelState.AddModelError("", "Missing movie or actor");
                return BadRequest();
            }


            if (_service.MovieModelExists(movie.Name))
            {
                throw new ErrorDetails
                {
                    StatusCode = 404,
                    Description = "Movie Exists..!"
                };
            }
            return NoContent();
        }
    }
}

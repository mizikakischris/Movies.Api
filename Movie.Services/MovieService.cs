﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Movie.Api.Exceptions;
using Movie.Interfaces;
using Movie.Types.Dtos;
using Movie.Types.Models;
using System;
using System.Collections.Generic;

namespace Movie.Services
{
    public class MovieService : IMovieService
    {

        private readonly IMovieRepositoryService _repo;
        private readonly IMapper _mapper;
        public MovieService(IMovieRepositoryService repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        public MovieModel CreateMovieModel(MovieDto movieDto, List<int> actorIds)
        {

          
            var movieObj = _mapper.Map<MovieModel>(movieDto);
            if (!_repo.CreateMovieModel(movieObj, actorIds))
            {
                throw new Exception( $"Something went wrong when saving the record {movieObj.Title}");
               
            }
            return movieObj;
        }

        public bool DeleteMovieModel(MovieModel movie)
        {
            return _repo.DeleteMovieModel(movie);
        }

        public MovieDto GetMovieModel(int movieId)
        {
            var movie =  _repo.GetMovieModel(movieId);
            if (movie == null)
            {
                
                throw new ErrorDetails
                {
                    
                    Description = $"Not found item with Id {movieId}",
                    StatusCode = StatusCodes.Status404NotFound,
                };

            }
            var actors = GetActorsByMovie(movie.Id);
            var actorDtos = new List<ActorsByMovieDto>();
            foreach (var actor in actors)
            {
                actorDtos.Add(_mapper.Map<ActorsByMovieDto>(actor));
            }

            var movieDto = _mapper.Map<MovieDto>(movie);
            movieDto.Actors = actorDtos;

            return movieDto;
        }

        public MovieModel GetTheMovieModel(int movieId)
        {
            var movie = _repo.GetMovieModel(movieId);

            return movie;
        }

        public List<MovieDto> GetMovies()
        {
            var moviesList =  _repo.GetMovies();

            //With movie Id I get the actors when searching the movieActors (intermediate table)
            Dictionary<int, List<ActorsByMovieDto>> dict = new Dictionary<int, List<ActorsByMovieDto>>();
            foreach (var movie in moviesList)
            {
                var actors = GetActorsByMovie(movie.Id);
                var actorDtos = new List<ActorsByMovieDto>();
                foreach (var actor in actors)
                {
                    actorDtos.Add(_mapper.Map<ActorsByMovieDto>(actor));
                }
                dict.Add(movie.Id, actorDtos);
            }
         

            var movieDtos = new List<MovieDto>();

            foreach (var movie in moviesList)
            {
                foreach (var kvp in dict)
                {
                    if (movie.Id == kvp.Key )
                    {
                        movieDtos.Add(_mapper.Map<MovieDto>(movie));
                    }
                }
            }

            foreach (var movieDto in movieDtos)
            {
                foreach (var kvp in dict)
                {
                    if (movieDto.Id == kvp.Key)
                    {
                        movieDto.Actors = kvp.Value;
                    }
                }
                 
            }

            return movieDtos;
        }


        public bool MovieModelExists(string name)
        {
            return _repo.MovieModelExists(name);
        }

        public bool MovieModelExists(int id)
        {
            return _repo.MovieModelExists(id);
        }

        public bool Save()
        {
            return _repo.Save();
        }

        public bool UpdateMovieModel(MovieModel movie)
        {
            return _repo.UpdateMovieModel(movie);
        }

        public List<MoviesByActorDto> GetMoviesByActor(int actorId)
        {
            var moviesList = _repo.GetMoviesByActor(actorId);
            var movieDtos = new List<MoviesByActorDto>();
            foreach (var movie in moviesList)
            {
                movieDtos.Add(_mapper.Map<MoviesByActorDto>(movie));
            }

            return movieDtos;
        }

        public List<Actor> GetActorsByMovie(int movieId)
        {
            var actors = _repo.GetActorsByMovie(movieId);
          
            return actors;
        }
    }

    
}

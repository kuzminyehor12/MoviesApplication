using Microsoft.AspNetCore.Mvc;
using Movies.Application.Abstractions;
using Movies.Application.Models;

namespace Movies.API.Controllers
{
    [Route("api/movies")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MoviesController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [HttpGet]
        public async Task<ActionResult<MovieExtendedModel>> GetMovieExtended([FromQuery] int movieId, CancellationToken cancellationToken = default)
        {
            var movie = await _movieService.GetMovieExtendedAsync(movieId, cancellationToken);
            return Ok(movie);
        }
    }
}
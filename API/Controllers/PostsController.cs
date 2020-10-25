using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Posts;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class PostsController : ControllerBase
    {
        private readonly IMediator mediator;

        public PostsController(IMediator mediator) => this.mediator = mediator;

        /// <summary>
        /// GET api/posts
        /// </summary>
        /// <returns>A list of posts</returns>
        [HttpGet]
        public async Task<ActionResult<List<Post>>> List()
        {
            return await this.mediator.Send(new List.Query());
        }

        /// <summary>
        /// Get api/posts/[id]
        /// </summary>
        /// <param name="id">Post id</param>
        /// <returns>A single post</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetById(Guid id)
        {
            // return await this.mediator.Send(new List.GetById{Id = id});
            return await this.mediator.Send(new List.GetById{Id = id});;
        }

        /// <summary>
        /// POST api/post
        /// </summary>
        /// <param name="request">JSON request containing post fields</param>
        /// <returns>A new post</returns>
        [HttpPost]
        public async Task<ActionResult<Post>> CreatePost([FromBody]Post p)
        {
            // return await this.mediator.Send(new List.CreatePost(data));
            return await this.mediator.Send(new List.CreatePost{Id = p.Id, Title = p.Title, Body = p.Body, Date = DateTime.Now});
        }

        /// <summary>
        /// PUT api/put
        /// </summary>
        /// <param name="request">JSON request containing one or more updated post fields</param>
        /// <returns>An update post</returns>
        [HttpPut]
        public async Task<ActionResult<Post>> UpdatePost([FromBody]Post p)
        {
            return await this.mediator.Send(new List.UpdatePost{Id = p.Id, Title = p.Title, Body = p.Body, Date = p.Date});
        }
    }
}
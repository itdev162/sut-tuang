using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Posts
{
    public class List
    {
        public class Query : IRequest<List<Post>> { }

        public class GetById : IRequest<Post> {
            public Guid Id { get; set;}
        }

        public class CreatePost : IRequest<Post> {
            public Guid Id {get;set;}
            public string Title {get;set;}
            public string Body {get;set;}
            public DateTime Date {get;set;}
            // private DateTime Date = DateTime.Now;
        }

        public class UpdatePost : IRequest<Post> {
            public Guid Id {get;set;}
            public string Title {get;set;}
            public string Body {get;set;}
            public DateTime Date {get;set;}
        }

        public class Handler : 
            IRequestHandler<Query, List<Post>>, 
            IRequestHandler<GetById, Post>,
            IRequestHandler<CreatePost, Post>,
            IRequestHandler<UpdatePost, Post>
        {
            private readonly DataContext context;

            public Handler(DataContext context) => this.context = context;
            
            public async Task<List<Post>> Handle(Query request, CancellationToken cancellationToken)
            {
                return await this.context.Posts.ToListAsync();
            }

            public async Task<Post> Handle(GetById request, CancellationToken cancellationToken)
            {
                // return this.context.Posts.FirstOrDefaultAsync(p => p.Id == request.Id);
                // return Task.FromResult(new Post{Title = "Test", Body = "Test"});
                return await this.context.Posts.FindAsync(request.Id).AsTask();
            }

            public async Task<Post> Handle(CreatePost request, CancellationToken cancellationToken)
            {
                var post = new Post {
                    Id = request.Id,
                    Title = request.Title,
                    Body = request.Body,
                    Date = request.Date
                };

                await context.Posts.AddAsync(post);

                try
                {
                    await context.SaveChangesAsync();
                    return await Task.FromResult(post);
                }
                catch (Exception ex)
                {
                    return await Task.FromResult(new Post { });
                }

            }

            public async Task<Post> Handle(UpdatePost request, CancellationToken cancellationToken)
            {
                try
                {
                    var result = this.context.Posts.FirstOrDefaultAsync(p => p.Id == request.Id);

                    var post = result.Result;

                    post.Title = request.Title != null ? request.Title : post.Title;
                    post.Body = request.Body != null ? request.Body : post.Body;
                    post.Date = request.Date != null ? request.Date : post.Date;

                    await context.SaveChangesAsync();

                    return await result;
                }
                catch (Exception ex)
                {
                    return await Task.FromResult(new Post { });
                }
            } 
            
        }
    }
}
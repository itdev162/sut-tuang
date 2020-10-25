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
            
            public Task<List<Post>> Handle(Query request, CancellationToken cancellationToken)
            {
                return this.context.Posts.ToListAsync();
            }

            public Task<Post> Handle(GetById request, CancellationToken cancellationToken)
            {
                // return this.context.Posts.FirstOrDefaultAsync(p => p.Id == request.Id);
                // return Task.FromResult(new Post{Title = "Test", Body = "Test"});
                return this.context.Posts.FindAsync(request.Id).AsTask();
            }

            public Task<Post> Handle(CreatePost request, CancellationToken cancellationToken)
            {
                var post = new Post {
                    Id = request.Id,
                    Title = request.Title,
                    Body = request.Body,
                    Date = request.Date
                };

                context.Posts.AddAsync(post);

                try
                {
                    context.SaveChangesAsync();
                    return Task.FromResult(post);
                }
                catch (Exception ex)
                {
                    return Task.FromResult(new Post{});
                }

            }

            public Task<Post> Handle(UpdatePost request, CancellationToken cancellationToken)
            {
                try
                {
                    var result = this.context.Posts.FirstOrDefaultAsync(p => p.Id == request.Id);

                    var post = result.Result;

                    post.Title = request.Title != null ? request.Title : post.Title;
                    post.Body = request.Body != null ? request.Body : post.Body;
                    post.Date = request.Date != null ? request.Date : post.Date;

                    context.SaveChangesAsync();

                    return result;
                }
                catch (Exception ex)
                {
                    return Task.FromResult(new Post{});
                }
            } 
            
        }
    }
}
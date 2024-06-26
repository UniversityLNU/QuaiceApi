﻿
using EduRateApi.Dtos.FundraisingDTO;
using EduRateApi.Dtos.PostsDTO;
using EduRateApi.Interfaces;
using EduRateApi.Models;
using FireSharp;
using FireSharp.Config;
using FireSharp.Response;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace EduRateApi.Implementation
{
    public class PostService : IPostService
    {
        private readonly IFirebaseConnectingService _firebaseConnectingService;
        public PostService( IFirebaseConnectingService firebaseConnectingService)
        {
                _firebaseConnectingService = firebaseConnectingService;
        }
        public async Task<AllPostsResponse> GetAllPosts()
        {
            try
            {
                using (var client = _firebaseConnectingService.GetFirebaseClient())
                {
                    if (client != null)
                    {
                        var response = await client.GetAsync("Posts");
                        if (response.Body != "null")
                        {
                            var posts = response.ResultAs<Dictionary<string, Posts>>();
                            var postList = posts.Values.ToList();
                            return new AllPostsResponse(postList: postList, message: "OK", statusCode: 200);
                        }
                        else
                        {
                            return new AllPostsResponse(postList: new List<Posts>(), message: "Non objects", statusCode: 200);
                        }
                    }
                    else
                    {
                        return new AllPostsResponse(postList: new List<Posts>(), message: "", statusCode: 400);
                    }
                }
            }
            catch (Exception ex)
            {
                return new AllPostsResponse(postList: new List<Posts>(), message: "", statusCode: 500);
            }
        }

        public async Task<PostResponse> GetPostById(string postId)
        {
            try
            {
                using (var client = _firebaseConnectingService.GetFirebaseClient())
                {
                    if (client != null)
                    {
                        var response = await client.GetAsync($"Posts/{postId}");
                        if (response.Body != "null") 
                        {
                            var post = response.ResultAs<Posts>();
                            var postDto = new Posts
                            {
                                userId = post.userId,
                                creatorFullName = post.creatorFullName,
                                description = post.description,
                                dateOfCreation = post.dateOfCreation,
                                fundraisingId = post.fundraisingId,
                                attachedPhotos = post.attachedPhotos
                            };
                            return new PostResponse(postDto, "Post retrieved successfully", 200);
                        }
                        else
                        {
                            return new PostResponse(new Posts(), "Post not found", 404);
                        }
                    }
                    else
                    {
                        return new PostResponse(new Posts(), "Firebase connection failed", 400);
                    }
                }
            }
            catch (Exception ex)
            {
                return new PostResponse(new Posts(), $"An error occurred while retrieving post: {ex.Message}", 500);
            }
        }

        public async Task<CreatePostResponseDto> UploadUserPost(Posts post)
        {
            try
            {
                using (var client = _firebaseConnectingService.GetFirebaseClient())
                {
                    if (client != null)
                    {
                        var response = await client.GetAsync($"Users/{post.userId}");
                        var user = response.ResultAs<User>();
                        user = Calculate(user, new DateTime(post.dateOfCreation));

                        if (user.dailyCount != 2)
                        {
                            user.dailyCount += 1;
                        }

                        var strickGap = new DateTime(post.dateOfCreation) - user.lastDonateTime;

                        if (strickGap.Days > 1)
                        {
                            user.strickCount = 0;
                        }
                        else if (user.lastDonateTime.Day != new DateTime(post.dateOfCreation).Day)
                        {
                            user.strickCount += 1;
                        }

                        user.lastDonateTime = new DateTime(post.dateOfCreation);
                        var updateUser = await client.UpdateAsync($"Users/{user.userId}", user);

                        var newPostId = Guid.NewGuid().ToString();
                        var setUserPost = await client.SetAsync($"Posts/{newPostId}/", new Posts
                        {
                            userId = post.userId,
                            creatorFullName = post.creatorFullName,
                            description = post.description,
                            dateOfCreation = post.dateOfCreation,
                            fundraisingId = post.fundraisingId,
                            attachedPhotos = post.attachedPhotos,
                        });
                        if (setUserPost.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            return new CreatePostResponseDto("Post uploaded successfully", 200, newPostId, post.attachedPhotos);
                        }
                        else
                        {
                            return new CreatePostResponseDto(message: "Failed to upload fundraising", statusCode: 400, photoLinks:null ,  postId: null);
                        }
                    }
                    else
                    {
                        return new CreatePostResponseDto("Firebase connection failed", 400, null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while uploading post to Firebase: " + ex.Message);
                return new CreatePostResponseDto($"An error occurred: {ex.Message}", 500, null, null);
            }
        }

        private User Calculate(User user, DateTime dateOfCreation)
        {
            const double dailyMult = 0.05;
            const double strickMult = 0.01;

            user.numberOfDonatsCoins += (user.dailyCount * dailyMult + 1) * (user.strickCount * strickMult + 1) * 2;

            return user;
        }
    }
}

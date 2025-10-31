using Microsoft.EntityFrameworkCore;
using JasonAlmond_DiscussionBoard.Models;
using JasonAlmond.DiscussionBoard.Repos;

namespace JasonAlmond_DiscussionBoard.Services
{
    public class PostService
    {
        private readonly IRepo<Post> _repo;
        private readonly IConfiguration _config;
        private readonly ILogger<PostService> _log;

        private Post _post;
        private List<Post> _posts;

        public PostService(IRepo<Post> repo, IConfiguration config, ILogger<PostService> log)
        {
            _repo = repo;
            _config = config;
            _log = log;

            _post = new Post();
            _posts = new List<Post>();
        }

        public Post Get(int id)
        {
            try
            {
                _post = _repo.Search(x => x.Id == id && !x.IsDeleted)
                    .Include(x => x.DiscussionThread)
                    .Include(x => x.SubPosts)
                    .FirstOrDefault() ?? new Post();
            }
            catch (Exception ex)
            {
                _log.LogError("Error getting Post with {Id}. " + ex.Message, id);
            }
            return _post;
        }

        public List<Post> GetAll()
        {
            try
            {
                _posts = _repo.Search(x => !x.IsDeleted)
                    .Include(x => x.DiscussionThread)
                    .Include(x => x.SubPosts)
                    .ToList();
            }
            catch (Exception ex)
            {
                _log.LogError("Error getting all Posts: " + ex.Message);
            }
            return _posts;
        }

        public List<Post> GetForUser(string applicationUserId)
        {
            try
            {
                _posts = _repo.Search(x => x.ApplicationUserId == applicationUserId && !x.IsDeleted)
                    .Include(x => x.DiscussionThread)
                    .Include(x => x.SubPosts)
                    .ToList();
            }
            catch (Exception ex)
            {
                _log.LogError("Error getting Posts for user {UserId}. " + ex.Message, applicationUserId);
            }
            return _posts;
        }

        public List<Post> GetRecycleBin()
        {
            try
            {
                _posts = _repo.Search(x => x.IsDeleted)
                    .Include(x => x.DiscussionThread)
                    .Include(x => x.SubPosts)
                    .ToList();
            }
            catch (Exception ex)
            {
                _log.LogError("Error getting recycle bin Posts: " + ex.Message);
            }
            return _posts;
        }

        public void Add(Post post)
        {
            try
            {
                _repo.Add(post);
                _repo.SaveChanges();
            }
            catch (Exception ex)
            {
                _log.LogError("Error adding Post: " + ex.Message);
            }
        }

        public void Update(Post post)
        {
            try
            {
                _repo.Update(post);
                _repo.SaveChanges();
            }
            catch (Exception ex)
            {
                _log.LogError("Error updating Post: " + ex.Message);
            }
        }

        public void Remove(Post post)
        {
            try
            {
                _repo.Delete(post);
                _repo.SaveChanges();
            }
            catch (Exception ex)
            {
                _log.LogError("Error removing Post: " + ex.Message);
            }
        }

        public void Delete(Post post)
        {
            try
            {
                post.IsDeleted = true;
                _repo.Update(post);
                _repo.SaveChanges();
            }
            catch (Exception ex)
            {
                _log.LogError("Error soft-deleting Post: " + ex.Message);
            }
        }

        public void UnDelete(Post post)
        {
            try
            {
                post.IsDeleted = false;
                _repo.Update(post);
                _repo.SaveChanges();
            }
            catch (Exception ex)
            {
                _log.LogError("Error restoring Post: " + ex.Message);
            }
        }
        
        public List<Post> GetForThread(int threadId)// Return hierarchical posts and subposts for a discussion thread
        {
            List<Post> posts = new List<Post>();
            try
            {
                posts = _repo.Search(x => x.DiscussionThreadId == threadId && x.ParentPostId == null && !x.IsDeleted)
                    .Include(x => x.DiscussionThread)
                    .Include(x => x.ApplicationUser)
                    .ToList();

                foreach (Post p in posts)
                {
                    p.SubPosts = LoadHierarchy(p);
                }
            }
            catch (Exception ex)
            {
                _log.LogError("Error getting hierarchical posts for thread {ThreadId}: " + ex.Message, threadId);
            }
            return posts;
        }
        
        private List<Post> LoadHierarchy(Post post)// Recursively loads subposts of the given post
        {
            List<Post> subposts = new List<Post>();
            try
            {
                subposts = _repo.Search(x => !x.IsDeleted && x.ParentPostId == post.Id)
                    .Include(x => x.ParentPost)
                    .ToList();

                //Recursively load hierarchy
                foreach(Post p in subposts)
                {
                    p.SubPosts = LoadHierarchy(p);
                }

            }
            catch (Exception ex)
            {
                _log.LogError("Error loading subposts for post {PostId}: " + ex.Message, post.Id);
            }
            return subposts;
        }
    }
}
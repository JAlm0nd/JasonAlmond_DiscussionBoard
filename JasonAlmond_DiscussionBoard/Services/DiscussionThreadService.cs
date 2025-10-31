using Microsoft.EntityFrameworkCore;
using JasonAlmond_DiscussionBoard.Models;
using JasonAlmond.DiscussionBoard.Repos;

namespace JasonAlmond_DiscussionBoard.Services
{
    public class DiscussionThreadService
    {
        private readonly IRepo<DiscussionThread> _repo;
        private readonly PostService _postService;
        private readonly IConfiguration _config;
        private readonly ILogger<DiscussionThread> _log;

        private DiscussionThread dt;
        private List<DiscussionThread> threads;

        public DiscussionThreadService(IRepo<DiscussionThread> repo, PostService postService, IConfiguration config, ILogger<DiscussionThread> log)
        {
            _repo = repo;
            _config = config;
            _log = log;
            dt = new DiscussionThread();
            threads = new List<DiscussionThread>();
        }

        public DiscussionThread Get(int Id)
        {
            try
            {
                dt = _repo.Search(x => x.Id == Id && !x.IsDeleted)
                    .Include(x => x.ApplicationUser)
                    .Include(x => x.Posts)
                    .FirstOrDefault() ?? new DiscussionThread();
                dt.Posts = _postService.GetForThread(Id);
            }
            catch (Exception ex)
            {
                _log.LogError("Error getting Discussion Thread with {Id}. " + ex.Message, Id);
            }
            return dt;
        }

        public int CountPosts(int ThreadId)
        {
            try
            {
                return _postService.GetAll().Count(p => p.DiscussionThreadId == ThreadId && !p.IsDeleted);
            }
            catch (Exception ex)
            {
                _log.LogError("Error counting posts for ThreadId {ThreadId}: " + ex.Message, ThreadId);
                return 0;
            }
        }
        
        public List<DiscussionThread> GetAll()
        {
            try
            {
                threads = _repo.Search(x => !x.IsDeleted)
                    .Include(x => x.ApplicationUser)
                    .Include(x => x.Posts)
                    .ToList();
            }
            catch (Exception ex)
            {
                _log.LogError("Error getting all Discussion Threads: " + ex.Message);
            }
            return threads;
        }

        public List<DiscussionThread> GetForUser(string ApplicationUserId)
        {
            try
            {
                threads = _repo.Search(x => x.ApplicationUserId == ApplicationUserId && !x.IsDeleted)
                    .Include(x => x.ApplicationUser)
                    .Include(x => x.Posts)
                    .ToList();
            }
            catch (Exception ex)
            {
                _log.LogError("Error getting Discussion Threads for user {UserId}. " + ex.Message, ApplicationUserId);
            }
            return threads;
        }

        public List<DiscussionThread> GetRecycleBin()
        {
            try
            {
                threads = _repo.Search(x => x.IsDeleted)
                    .Include(x => x.ApplicationUser)
                    .Include(x => x.Posts)
                    .ToList();
            }
            catch (Exception ex)
            {
                _log.LogError("Error getting recycle bin Discussion Threads: " + ex.Message);
            }
            return threads;
        }

        public void Add(DiscussionThread thread)
        {
            try
            {
                _repo.Add(thread);
                _repo.SaveChanges();
            }
            catch (Exception ex)
            {
                _log.LogError("Error adding Discussion Thread: " + ex.Message);
            }
        }

        public void Update(DiscussionThread thread)
        {
            try
            {
                _repo.Update(thread);
                _repo.SaveChanges();
            }
            catch (Exception ex)
            {
                _log.LogError("Error updating Discussion Thread: " + ex.Message);
            }
        }

        // Hard delete
        public void Remove(DiscussionThread thread)
        {
            try
            {
                _repo.Delete(thread);
                _repo.SaveChanges();
            }
            catch (Exception ex)
            {
                _log.LogError("Error removing Discussion Thread: " + ex.Message);
            }
        }

        // Soft delete
        public void Delete(DiscussionThread thread)
        {
            try
            {
                thread.IsDeleted = true;
                _repo.Update(thread);
                _repo.SaveChanges();
            }
            catch (Exception ex)
            {
                _log.LogError("Error soft-deleting Discussion Thread: " + ex.Message);
            }
        }

        // Undo soft delete
        public void UnDelete(DiscussionThread thread)
        {
            try
            {
                thread.IsDeleted = false;
                _repo.Update(thread);
                _repo.SaveChanges();
            }
            catch (Exception ex)
            {
                _log.LogError("Error restoring Discussion Thread: " + ex.Message);
            }
        }
    }
}
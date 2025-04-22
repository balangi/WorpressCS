using System;
using System.Collections.Generic;
using System.Linq;

public class CapabilityService
{
    private readonly ApplicationDbContext _context;

    public CapabilityService(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<string> MapMetaCap(string cap, int userId, params object[] args)
    {
        var caps = new List<string>();

        switch (cap)
        {
            case "remove_user":
                if (args.Length > 0 && userId == (int)args[0] && !_context.Users.Any(u => u.Id == userId && u.IsSuperAdmin))
                {
                    caps.Add("do_not_allow");
                }
                else
                {
                    caps.Add("remove_users");
                }
                break;

            case "promote_user":
            case "add_users":
                caps.Add("promote_users");
                break;

            case "edit_user":
            case "edit_users":
                if (cap == "edit_user" && args.Length > 0 && userId == (int)args[0])
                {
                    break;
                }

                if (_context.Users.Any(u => u.Id == userId && !u.IsSuperAdmin) &&
                    (args.Length == 0 || _context.Users.Any(u => u.Id == (int)args[0] && u.IsSuperAdmin)))
                {
                    caps.Add("do_not_allow");
                }
                else
                {
                    caps.Add("edit_users");
                }
                break;

            case "delete_post":
            case "delete_page":
                if (args.Length == 0)
                {
                    caps.Add("do_not_allow");
                    break;
                }

                var postId = (int)args[0];
                var post = _context.Posts.FirstOrDefault(p => p.Id == postId);

                if (post == null || post.PostType == "revision")
                {
                    caps.Add("do_not_allow");
                    break;
                }

                if (post.AuthorId == userId)
                {
                    if (new[] { "publish", "future" }.Contains(post.Status))
                    {
                        caps.Add("delete_published_posts");
                    }
                    else if (post.Status == "trash")
                    {
                        var trashStatus = _context.PostMeta.FirstOrDefault(pm => pm.PostId == postId)?.MetaValue;
                        if (new[] { "publish", "future" }.Contains(trashStatus))
                        {
                            caps.Add("delete_published_posts");
                        }
                        else
                        {
                            caps.Add("delete_posts");
                        }
                    }
                    else
                    {
                        caps.Add("delete_posts");
                    }
                }
                else
                {
                    caps.Add("delete_others_posts");
                    if (new[] { "publish", "future" }.Contains(post.Status))
                    {
                        caps.Add("delete_published_posts");
                    }
                    else if (post.Status == "private")
                    {
                        caps.Add("delete_private_posts");
                    }
                }
                break;

            default:
                caps.Add(cap);
                break;
        }

        return caps;
    }

    public bool CurrentUserCan(string capability, params object[] args)
    {
        var currentUser = _context.Users.FirstOrDefault(u => u.Id == GetCurrentUserId());
        return UserCan(currentUser, capability, args);
    }

    public bool UserCan(User user, string capability, params object[] args)
    {
        if (user == null)
        {
            return false;
        }

        var requiredCaps = MapMetaCap(capability, user.Id, args);
        return requiredCaps.All(cap => user.Roles.SelectMany(r => r.Capabilities).Any(c => c.Name == cap));
    }

    private int GetCurrentUserId()
    {
        // Implement logic to get the current user ID (e.g., from HttpContext)
        return 1; // Placeholder
    }
}
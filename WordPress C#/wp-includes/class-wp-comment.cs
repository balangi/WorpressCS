// <summary>
    public class CommentService
    {
        private readonly ApplicationDbContext _context;

        public CommentService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves a comment by ID.
        /// </summary>
        public Comment? GetCommentById(int id)
        {
            return _context.Comments
                .Include(c => c.Children)
                .Include(c => c.Post)
                .FirstOrDefault(c => c.CommentId == id);
        }

        /// <summary>
        /// Retrieves children of a comment.
        /// </summary>
        public IEnumerable<Comment> GetChildren(int parentId, string format = "tree")
        {
            var children = _context.Comments
                .Where(c => c.ParentId == parentId)
                .ToList();

            if (format == "flat")
            {
                var flatList = new List<Comment>();
                foreach (var child in children)
                {
                    flatList.Add(child);
                    flatList.AddRange(GetChildren(child.CommentId, format));
                }
                return flatList;
            }

            return children;
        }

        /// <summary>
        /// Adds a child comment to a parent comment.
        /// </summary>
        public void AddChild(Comment parent, Comment child)
        {
            parent.Children.Add(child);
            _context.SaveChanges();
        }

        /// <summary>
        /// Checks if a property is set.
        /// </summary>
        public bool IsPropertySet(Comment comment, string propertyName)
        {
            if (propertyName == nameof(comment.Post))
            {
                return comment.Post != null;
            }

            return false;
        }
    }
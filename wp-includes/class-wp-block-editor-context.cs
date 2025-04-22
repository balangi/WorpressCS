using System;
using WordPress.Core.Data;
using WordPress.Core.Models;

namespace WordPress.Core.BlockEditor
{
    public class BlockEditorContextManager
    {
        /// <summary>
        /// String that identifies the block editor being rendered.
        /// </summary>
        public string Name { get; private set; } = "core/edit-post";

        /// <summary>
        /// The post being edited by the block editor. Optional.
        /// </summary>
        public Post Post { get; private set; }

        private readonly AppDbContext _dbContext;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="settings">The list of optional settings to expose in a given context.</param>
        public BlockEditorContextManager(AppDbContext dbContext, Dictionary<string, object> settings = null)
        {
            _dbContext = dbContext;

            if (settings != null)
            {
                if (settings.ContainsKey("name") && settings["name"] is string name)
                {
                    Name = name;
                }

                if (settings.ContainsKey("post") && settings["post"] is Post post)
                {
                    Post = post;
                }
            }
        }

        /// <summary>
        /// Saves the block editor context to the database.
        /// </summary>
        public void Save()
        {
            var context = new BlockEditorContext
            {
                Name = Name,
                PostId = Post?.Id
            };

            _dbContext.BlockEditorContexts.Add(context);
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// Loads a block editor context from the database.
        /// </summary>
        /// <param name="id">The ID of the block editor context.</param>
        /// <returns>The loaded block editor context.</returns>
        public static BlockEditorContextManager Load(int id, AppDbContext dbContext)
        {
            var context = dbContext.BlockEditorContexts
                .Include(c => c.Post)
                .FirstOrDefault(c => c.Id == id);

            if (context == null)
            {
                throw new InvalidOperationException("Block editor context not found.");
            }

            return new BlockEditorContextManager(dbContext)
            {
                Name = context.Name,
                Post = context.Post
            };
        }
    }
}
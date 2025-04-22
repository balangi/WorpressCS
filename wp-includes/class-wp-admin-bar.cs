using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using WordPress.Core.Data;
using WordPress.Core.Models;

namespace WordPress.Core.Toolbar
{
    public class WPAdminBar
    {
        private readonly AppDbContext _dbContext;
        private readonly Dictionary<string, ToolbarNode> _nodes = new Dictionary<string, ToolbarNode>();
        private bool _bound = false;

        public WPAdminBar(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Initializes the admin bar.
        /// </summary>
        public void Initialize()
        {
            LoadNodesFromDatabase();
            AddDefaultMenus();
            Render();
        }

        /// <summary>
        /// Loads nodes from the database.
        /// </summary>
        private void LoadNodesFromDatabase()
        {
            var nodes = _dbContext.ToolbarNodes
                .Include(n => n.Children)
                .ToList();

            foreach (var node in nodes)
            {
                _nodes[node.Id] = node;
            }
        }

        /// <summary>
        /// Adds default menus to the admin bar.
        /// </summary>
        private void AddDefaultMenus()
        {
            AddNode(new ToolbarNode
            {
                Id = "my-account",
                Title = "My Account",
                Type = "item",
                Href = "/account"
            });

            AddNode(new ToolbarNode
            {
                Id = "site-menu",
                Title = "Site Menu",
                Type = "group",
                Children = new List<ToolbarNode>
                {
                    new ToolbarNode
                    {
                        Id = "dashboard",
                        Title = "Dashboard",
                        Type = "item",
                        Href = "/dashboard"
                    }
                }
            });
        }

        /// <summary>
        /// Renders the admin bar.
        /// </summary>
        private void Render()
        {
            var root = GetNode("root");
            if (root == null)
            {
                root = new ToolbarNode { Id = "root", Type = "container" };
                _nodes["root"] = root;
            }

            var output = new StringBuilder();
            output.AppendLine("<div id=\"wpadminbar\">");
            RenderGroup(root, output);
            output.AppendLine("</div>");

            Console.WriteLine(output.ToString());
        }

        /// <summary>
        /// Renders a group of nodes.
        /// </summary>
        private void RenderGroup(ToolbarNode node, StringBuilder output)
        {
            if (node.Type != "container" && node.Type != "group")
                return;

            output.AppendLine($"<ul id=\"wp-admin-bar-{node.Id}\">");
            foreach (var child in node.Children)
            {
                RenderItem(child, output);
            }
            output.AppendLine("</ul>");
        }

        /// <summary>
        /// Renders an individual node.
        /// </summary>
        private void RenderItem(ToolbarNode node, StringBuilder output)
        {
            if (node.Type == "container" || node.Type == "group")
            {
                RenderGroup(node, output);
                return;
            }

            var classes = node.Meta.ContainsKey("class") ? $" class=\"{node.Meta["class"]}\"" : "";
            var href = !string.IsNullOrEmpty(node.Href) ? $" href=\"{node.Href}\"" : "";

            output.AppendLine($"<li id=\"wp-admin-bar-{node.Id}\"{classes}>");
            output.AppendLine($"<a{href}>{node.Title}</a>");
            output.AppendLine("</li>");
        }

        /// <summary>
        /// Adds a node to the admin bar.
        /// </summary>
        public void AddNode(ToolbarNode node)
        {
            if (_nodes.ContainsKey(node.Id))
            {
                _nodes[node.Id] = node;
            }
            else
            {
                _nodes.Add(node.Id, node);
            }

            SaveNodeToDatabase(node);
        }

        /// <summary>
        /// Removes a node from the admin bar.
        /// </summary>
        public void RemoveNode(string id)
        {
            if (_nodes.ContainsKey(id))
            {
                _nodes.Remove(id);
            }

            RemoveNodeFromDatabase(id);
        }

        /// <summary>
        /// Gets a node by its ID.
        /// </summary>
        public ToolbarNode GetNode(string id)
        {
            return _nodes.ContainsKey(id) ? _nodes[id] : null;
        }

        /// <summary>
        /// Saves a node to the database.
        /// </summary>
        private void SaveNodeToDatabase(ToolbarNode node)
        {
            var existingNode = _dbContext.ToolbarNodes.FirstOrDefault(n => n.Id == node.Id);
            if (existingNode != null)
            {
                _dbContext.Entry(existingNode).CurrentValues.SetValues(node);
            }
            else
            {
                _dbContext.ToolbarNodes.Add(node);
            }
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// Removes a node from the database.
        /// </summary>
        private void RemoveNodeFromDatabase(string id)
        {
            var node = _dbContext.ToolbarNodes.FirstOrDefault(n => n.Id == id);
            if (node != null)
            {
                _dbContext.ToolbarNodes.Remove(node);
                _dbContext.SaveChanges();
            }
        }
    }
}
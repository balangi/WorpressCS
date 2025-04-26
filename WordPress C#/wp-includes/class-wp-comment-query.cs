 /// <summary>

            if (!string.IsNullOrEmpty(parameters.AuthorEmail))
                query = query.Where(c => c.AuthorEmail == parameters.AuthorEmail);

            if (parameters.Status != "all")
                query = query.Where(c => c.Approved == parameters.Status);

            if (parameters.ParentId.HasValue)
                query = query.Where(c => c.ParentId == parameters.ParentId.Value);

            // Apply ordering
            switch (parameters.OrderBy.ToLower())
            {
                case "comment_date":
                    query = parameters.Order.ToLower() == "asc"
                        ? query.OrderBy(c => c.CommentDate)
                        : query.OrderByDescending(c => c.CommentDate);
                    break;

                case "comment_date_gmt":
                    query = parameters.Order.ToLower() == "asc"
                        ? query.OrderBy(c => c.CommentDateGmt)
                        : query.OrderByDescending(c => c.CommentDateGmt);
                    break;

                default:
                    query = query.OrderByDescending(c => c.CommentDateGmt);
                    break;
            }

            // Include children if requested
            if (parameters.IncludeChildren)
                query = query.Include(c => c.Children);

            return query;
        }

        /// <summary>
        /// Retrieves paginated results for a comment query.
        /// </summary>
        public PaginatedResult<Comment> GetPaginatedComments(CommentQueryParameters parameters)
        {
            var query = QueryComments(parameters);

            // Pagination
            var totalItems = query.Count();
            var items = query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToList();

            return new PaginatedResult<Comment>
            {
                Items = items,
                TotalItems = totalItems,
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize
            };
        }
    }
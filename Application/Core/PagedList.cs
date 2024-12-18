using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Core
{
    /// <summary>
    /// General Class the Helps with Pagination
    /// </summary>
    /// <typeparam name="T">Any Type</typeparam>
    public class PagedList<T> : List<T>
    {
        public PagedList(IEnumerable<T> items ,int count, int pageNumber, int pageSize)
        {
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            PageSize = pageSize;
            TotalCount = count;

            // Adding the item to the current List
            AddRange(items);
        }

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        /// <summary>
        /// Factory Method that creates a new object of the class
        /// </summary>
        /// <param name="source" type="List<T>"></param>
        /// <param name="pageNumber" type="int"></param>
        /// <param name="pageSize" type="int"></param>
        /// <returns>New Object of PagesList class</returns>
        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            // We have 2 Queries for the database in this implementation of the paging
                // First One is the query to get the count of the list before we execute the query
                // Second One is the execution of the query we got after we skipped and took the number of elements requried by the user

            // Get the count of the list
            var count = await source.CountAsync();

            // Get paged items
            // We Skip the number of elements with respect to the page size
            // After skipping n elements, we take number of elements after n which is the page size
            var items = await source.Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(); // Execure the query

            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}

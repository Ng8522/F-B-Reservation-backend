using System;
using System.Collections.Generic;
using System.Linq;

namespace FnbReservationAPI.src.utils
{
    public class Pagination<T>
    {
        public int PageNumber { get; private set; }
        public int PageSize { get; private set; }
        public int TotalPages { get; private set; }
        public int TotalCount { get; private set; }
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
        public List<T> Items { get; private set; }

        public Pagination(List<T> items, int count, int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = count;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            Items = items;
        }

        public static Pagination<T> Create(IEnumerable<T> source, int pageNumber, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return new Pagination<T>(items, count, pageNumber, pageSize);
        }

        public object ToResponse()
        {
            return new
            {
                success = true,
                message = "Data retrieved successfully.",
                data = new
                {
                    items = Items,
                    pageNumber = PageNumber,
                    pageSize = PageSize,
                    totalPages = TotalPages,
                    totalCount = TotalCount,
                    hasPreviousPage = HasPreviousPage,
                    hasNextPage = HasNextPage
                }
            };
        }
    }
} 
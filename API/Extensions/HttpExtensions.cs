using System.Text.Json;

namespace API.Extensions
{
    public static class HttpExtensions
    {
        public static void AddPaginationHeader(this HttpResponse response, int currentPage, 
            int itemsPerPage, int totalItems, int totalPages)
        {
            var paginationHeader = new
            {
                currentPage,
                itemsPerPage,
                totalItems,
                totalPages,
            };

            // Add the Pagination Header
            response.Headers.Append("Pagination", JsonSerializer.Serialize(paginationHeader));

            // Expose the header to be visible by browsers
            response.Headers.Append("Access-Control-Expose-Headers", "Pagination");
        } 
    }
}

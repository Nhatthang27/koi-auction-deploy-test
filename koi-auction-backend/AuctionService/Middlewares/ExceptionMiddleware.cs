using System.Net;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            var (statusCode, message) = ex switch
            {
                // Vi phạm UNIQUE KEY trong cơ sở dữ liệu
                DbUpdateException dbEx when dbEx.InnerException is SqlException sqlEx && sqlEx.Number == 2627
                    => ((int)HttpStatusCode.Conflict, "A record with the same unique key already exists."),

                // Lỗi vi phạm ràng buộc khóa ngoại (Foreign Key constraint)
                DbUpdateException dbEx when dbEx.InnerException is SqlException sqlEx && sqlEx.Number == 547
                    => ((int)HttpStatusCode.BadRequest, "The operation failed due to a foreign key constraint violation."),

                // Lỗi không tìm thấy tài nguyên (thường gặp khi ID không tồn tại)
                KeyNotFoundException => ((int)HttpStatusCode.NotFound, ex.Message),

                // Lỗi dữ liệu không hợp lệ (Validation Errors)
                ArgumentException => ((int)HttpStatusCode.BadRequest, ex.Message),

                // Lỗi không cho phép (ví dụ như truy cập tài nguyên không được phép)
                UnauthorizedAccessException => ((int)HttpStatusCode.Unauthorized, "You are not authorized to access this resource."),

                // Lỗi khi tài nguyên bị xung đột trạng thái (Concurrency Conflict)
                DbUpdateConcurrencyException => ((int)HttpStatusCode.Conflict, "The resource was modified by another process. Please reload and try again."),

                // Các lỗi hệ thống hoặc chưa xác định
                _ => ((int)HttpStatusCode.InternalServerError, "An unexpected error occurred. Please try again later.")
            };

            context.Response.StatusCode = statusCode;

            var errorResponse = new
            {
                StatusCode = statusCode,
                Message = message,
                Details = ex.Message // Bạn có thể tùy chỉnh hiển thị chi tiết này hoặc loại bỏ cho môi trường sản xuất
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        }
    }

}
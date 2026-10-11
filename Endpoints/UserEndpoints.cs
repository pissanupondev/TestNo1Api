using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapPost("/api/users", async (CreateUserDto dto, IUserService userService) =>
        {
            var result = await userService.CreateUserAsync(dto);

            if (!result.Success)
            {
                // กรณี Validation ไม่ผ่าน (มี Error รายฟิลด์)
                if (result.Errors != null)
                {
                    return Results.BadRequest(new
                    {
                        success = false,
                        message = result.Message,
                        errors = result.Errors
                    });
                }
                
                return Results.BadRequest(new
                {
                    success = false,
                    message = result.Message
                });
            }

            // กรณีบันทึกข้อมูลสำเร็จ
            return Results.Ok(new
            {
                success = true,
                message = result.Message,
                data = result.Data
            });
        });
    }
}
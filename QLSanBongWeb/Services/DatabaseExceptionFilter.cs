using System.Data.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
namespace QLSanBongWeb.Services;

public sealed class DatabaseExceptionFilter(ILogger<DatabaseExceptionFilter> logger) : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is not DbException && context.Exception is not DbUpdateException) return;
        logger.LogError(context.Exception, "Lỗi thao tác CSDL tại {Path}", context.HttpContext.Request.Path);
        if (context.HttpContext.Request.Headers.Accept.ToString().Contains("application/json"))
            context.Result = new JsonResult(new { success = false, message = "Chưa hoàn tất thao tác với CSDL. Kiểm tra SQL Server và thử lại." }) { StatusCode = 503 };
        else
            context.Result = new ViewResult { ViewName = "~/Views/Shared/DatabaseError.cshtml", StatusCode = 503 };
        context.ExceptionHandled = true;
    }
}

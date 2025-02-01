using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OrderService.Model;
using OrderService.Model.Exceptions;

namespace OrderService.Api.Filter;

	public class GlobalExceptionFilter : IExceptionFilter
	{
		public void OnException(ExceptionContext context)
		{
			ErrorModel errorModel;

			switch (context.Exception)
			{
				case NotFoundException notFoundException:
					errorModel = new ErrorModel
					{
						StatusCode = StatusCodes.Status404NotFound,
						Message = notFoundException.Message
					};
					context.Result = new JsonResult(errorModel)
					{
						StatusCode = StatusCodes.Status404NotFound
					};
					break;
				default:
					errorModel = new ErrorModel
					{
						StatusCode = StatusCodes.Status500InternalServerError,
						Message = context.Exception.Message,
						Details = context.Exception.StackTrace
					};
					context.Result = new JsonResult(errorModel)
					{
						StatusCode = StatusCodes.Status500InternalServerError
					};
					break;
			}
		}
	}

using System;
using System.Collections.Generic;
using System.Linq;
using Hermes.DataObjects;
using Hermes.Services.Helpers;
using Hermes.Services.Helpers.Logging;
using Nancy;
using Nancy.ModelBinding;

namespace Hermes.WebAPI.WebAPI.Modules
{
    public abstract class CommonModule : NancyModule
    {
        protected CommonModule(string modulePath) : base(modulePath)
        {
            OnError += (ctx, ex) =>
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;

                return Response.AsJson(new HermesApiExceptionDto(ex.Message, ex.StackTrace), HttpStatusCode.BadRequest);
            };
        }

        protected dynamic CallWithStatus(string callName, Action<AppLogger> call)
        {
            AppLogger logger = LoggerHelper.GetLogger(LoggerHelper.HermesWebApi, callName);

            try
            {
                call(logger);
                return HttpStatusCode.OK;
            }
            catch (Exception e)
            {
                logger.ErrorWithException(e, "Unable to perform action");
                return Negotiate
                    .WithModel(new { Message = e.Message })
                    .WithStatusCode(HttpStatusCode.BadRequest);
            }
        }

        protected dynamic CallWithResponse<T>(string callName, Func<AppLogger, T> call)
        {
            AppLogger logger = LoggerHelper.GetLogger(LoggerHelper.HermesWebApi, callName);

            try
            {
                return Response.AsJson(call(logger));
            }
            catch (Exception e)
            {
                logger.ErrorWithException(e, "Unable to perform action");
                return Negotiate
                    .WithModel(new { Message = e.Message })
                    .WithStatusCode(HttpStatusCode.BadRequest);
            }
        }

        protected dynamic PostRequest<TInput>(string callName, Func<AppLogger, TInput, dynamic> callback)
        {
            TInput input;
            if (!SafeBindAndValidate(out input))
                return Negotiate
                    .WithModel(new { Message = "Invalid body" })
                    .WithStatusCode(HttpStatusCode.BadRequest);

            if (!ModelValidationResult.IsValid)
            {
                return Negotiate
                    .WithModel(ModelValidationResult)
                    .WithStatusCode(HttpStatusCode.BadRequest);
            }

            AppLogger logger = LoggerHelper.GetLogger(LoggerHelper.HermesWebApi, callName);

            try
            {
                return FormatterExtensions.AsJson(Response, callback(logger, input));
            }
            catch (Exception e)
            {
                logger.ErrorWithException(e, "Unable to perform action");
                return Negotiate
                    .WithModel(new { Message = e.Message })
                    .WithStatusCode(HttpStatusCode.BadRequest);
            }
        }

        protected bool SafeBindAndValidate<T>(out T result)
        {
            try
            {
                result = this.BindAndValidate<T>();
                return true;
            }
            catch (Exception e)
            {
                result = default(T);
                return false;
            }
        }

        protected static List<T> GetList<T>(string parameter)
        {
            return (from dynamic o in parameter.Split(',') select (T)Convert.ChangeType(o, typeof(T))).ToList();
        }
    }
}

using Core.Domain.Notificacoes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Swashbuckle.AspNetCore.Annotations;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json;

namespace Core.WebApi.Controller
{
    [ExcludeFromCodeCoverage]
    [ApiController]
    [Produces("application/json")]
    [SwaggerResponse((int)HttpStatusCode.OK, "OK", null)]
    [SwaggerResponse((int)HttpStatusCode.Created, "Created", null)]
    [SwaggerResponse((int)HttpStatusCode.NoContent, "No Content", null)]
    [SwaggerResponse((int)HttpStatusCode.BadRequest, "Bad Request", null)]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized, "Unauthorized", null)]
    [SwaggerResponse((int)HttpStatusCode.Forbidden, "Forbidden", null)]
    [SwaggerResponse((int)HttpStatusCode.NotFound, "Not Found", null)]
    [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Internal Server Error", null)]
    [SwaggerResponse((int)HttpStatusCode.ServiceUnavailable, "Service Unavailable", null)]
    public abstract class MainController(INotificador notificador) : ControllerBase
    {
        protected IActionResult CustomResponseGet(object? data) =>
            data is not null
                ? Ok(SuccessResponse(data))
                : NotFound(NoSuccessResponse(data));

        protected IActionResult CustomResponseGet(string result) =>
            result.Equals("[]") ? NotFound(NoSuccessResponse()) : Ok(SuccessResponse(result));

        protected IActionResult CustomResponsePost(string url, object data, bool result) =>
            OperacaoValida() || result
                ? Created(url, SuccessResponse(data))
                : BadRequest(NoSuccessResponse(data));

        protected IActionResult CustomResponsePutPatch(object data, bool result) =>
            OperacaoValida() || result
                ? Ok(SuccessResponse(data))
                : BadRequest(NoSuccessResponse(data));

        protected IActionResult CustomResponseDelete(Guid id, bool result) =>
            result
                ? Ok(SuccessResponse(id))
                : BadRequest(NoSuccessResponse(id));

        protected IActionResult ErrorBadRequestPutId()
        {
            var errors = new List<string> { "O ID informado não é o mesmo que o informado na request." };
            return BadRequest(new BaseApiResponse { Success = false, Errors = errors });
        }

        protected IActionResult ErrorBadRequestModelState(ModelStateDictionary modelState)
        {
            var errors = modelState.Values.SelectMany(v => v.Errors)
                                          .Select(e => e.ErrorMessage)
                                          .ToList();

            return BadRequest(new BaseApiResponse
            {
                Success = false,
                Errors = errors
            });
        }

        private bool OperacaoValida() =>
            !notificador.TemNotificacao();

        private static BaseApiResponse SuccessResponse(object data) =>
            new() { Success = true, Data = data };

        private static BaseApiResponse SuccessResponse(string data) =>
            new()
            {
                Success = true,
                Data = JsonDocument.Parse(data).RootElement
            };

        private static BaseApiResponse SuccessResponse(Guid id) =>
            new() { Success = true, Data = id };

        private BaseApiResponse NoSuccessResponse(Guid id) =>
            new()
            {
                Success = false,
                Data = id,
                Errors = notificador.ObterNotificacoes().Select(n => n.Mensagem).ToList()
            };

        private BaseApiResponse NoSuccessResponse(object? data) =>
            new()
            {
                Success = false,
                Data = data,
                Errors = notificador.ObterNotificacoes().Select(n => n.Mensagem).ToList()
            };

        private static BaseApiResponse NoSuccessResponse() =>
            new()
            {
                Success = true,
                Data = "[]"
            };
    }
}
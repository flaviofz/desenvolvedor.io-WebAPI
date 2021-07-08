using System;
using System.Linq;
using DevIO.Business.Intefaces;
using DevIO.Business.Notificacoes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DevIO.Api.Controllers
{
    [ApiController]
    public abstract class MainController : ControllerBase
    {
        private readonly INotificador _notificador;
        protected readonly IUser _appUser;

        protected Guid UsuarioId { get; set; }
        protected bool UsuarioAautenticado { get; set; }

        protected MainController(
            INotificador notificador,
            IUser appUser)
        {
            _notificador = notificador;
            _appUser = appUser;

            if(_appUser.IsAuthenticated())
            {
                UsuarioId = _appUser.GetUserId();
                UsuarioAautenticado = true;
            }
        }

        protected bool OperacaoValida()
        {
            return !_notificador.TemNotificacao();
        }

        protected ActionResult CustomResponse(object result = null)
        {
            if (OperacaoValida())
            {
                return Ok(new 
                {
                    success = true,
                    data = result
                });
            }

            return BadRequest(new 
            {
                success = false,
                erros = _notificador.ObterNotificacoes().Select(n => n.Mensagem)
            });
        }

        protected ActionResult CustomResponse(ModelStateDictionary modelState)
        {
            if (!ModelState.IsValid) NotificarErroModelInvalida(modelState);
            return CustomResponse();
        }

        protected void NotificarErroModelInvalida(ModelStateDictionary modelState)
        {
            var erros = modelState.Values.SelectMany(e => e.Errors);

            foreach (var erro in erros)
            {
                var mensagemErro = erro.Exception == null ? erro.ErrorMessage : erro.Exception.Message;
                NotificarErro(mensagemErro);
            }
        }

        protected void NotificarErro(string mensagemErro)
        {
            _notificador.Handle(new Notificacao(mensagemErro));
        }
    }
}
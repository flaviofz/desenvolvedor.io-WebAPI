using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DevIO.Api.ViewModels;
using DevIO.Business.Intefaces;
using Microsoft.AspNetCore.Mvc;

namespace DevIO.Api.Controllers
{
    [Route("api/[controller]")]
    public  class FornecedoresController : MainController
    {
        private readonly IFornecedorRepository _fornecedorRespository;
        private readonly IMapper _mapper;

        public FornecedoresController(
            IFornecedorRepository fornecedorRespository, 
            IMapper mapper
        )
        {
            _fornecedorRespository = fornecedorRespository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FornecedorViewModel>>> ObterTodos()
        {
            var fornecedores = _mapper.Map<IEnumerable<FornecedorViewModel>>(await _fornecedorRespository.ObterTodos());

            return Ok(fornecedores);
        }
    }
}
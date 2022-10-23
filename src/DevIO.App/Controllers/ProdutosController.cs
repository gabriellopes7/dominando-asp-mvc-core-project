using AppMvcBasica.Models;
using AutoMapper;
using DevIO.App.Extensions;
using DevIO.App.ViewModels;
using DevIO.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevIO.App.Controllers
{
    [Authorize]
    public class ProdutosController : BaseController
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IFornecedorRepository _fornecedorRepository;
        private readonly IProdutoService _produtoService;

        private readonly IMapper _mapper;


        public ProdutosController(IProdutoRepository produtoRepository,
            IFornecedorRepository fornecedorRepository,
            IMapper mapper,
            IProdutoService produtoService,
            INotificador notificador) : base(notificador)
        {
            _produtoRepository = produtoRepository;
            _fornecedorRepository = fornecedorRepository;

            _mapper = mapper;
            _produtoService = produtoService;
        }

        // GET: Produtos
        [Route("lista-de-produtos")]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {

            return View(_mapper.Map<IEnumerable<ProdutoViewModel>>(await _produtoRepository.ObterProdutosFornecedores()));
        }

        // GET: Produtos/Details/5
        [Route("dados-do-produto/{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> Details(Guid id)
        {
            var produtoViewModel = await ObterProduto(id);

            if (produtoViewModel == null)
            {
                return NotFound();
            }

            return View(produtoViewModel);
        }

        // GET: Produtos/Create
        [Route("novo-produto")]
        [ClaimsAuthorize("Produto", "Adicionar")]
        public async Task<IActionResult> Create()
        {
            var produtoViewModel = await PopularFornecedores(new ProdutoViewModel());

            return View(produtoViewModel);
        }

        // POST: Produtos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("novo-produto")]
        [ClaimsAuthorize("Produto", "Adicionar")]
        public async Task<IActionResult> Create(ProdutoViewModel produtoViewModel)
        {
            produtoViewModel = await PopularFornecedores(produtoViewModel);

            if (!ModelState.IsValid)
                return View(produtoViewModel);

            var imgPrefixo = Guid.NewGuid() + "_";

            //Adicionando imagem do produto

            if (!await UploadArquivo(produtoViewModel.ImagemUpload, imgPrefixo))
                return View(produtoViewModel);

            produtoViewModel.Imagem = imgPrefixo + produtoViewModel.ImagemUpload.FileName;

            var produto = _mapper.Map<Produto>(produtoViewModel);

            await _produtoService.Adicionar(produto);

            if (!OperacaoValida())
                return View(produtoViewModel);

            return RedirectToAction("Index");

        }



        // GET: Produtos/Edit/5
        [Route("editar-produto/{id:guid}")]
        [ClaimsAuthorize("Produto", "Editar")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var produtoViewModel = await ObterProduto(id);

            //Nao precisa popular o fornecedor pq uma vez que a gente cadastra, nao precisa trocar o fornecedor
            if (produtoViewModel == null)
            {
                return NotFound();
            }




            return View(produtoViewModel);
        }

        // POST: Produtos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("editar-produto/{id:guid}")]
        [ClaimsAuthorize("Produto", "Editar")]
        public async Task<IActionResult> Edit(Guid id, ProdutoViewModel produtoViewModel)
        {
            if (id != produtoViewModel.Id)
                return NotFound();


            var produtoAtualizacao = await ObterProduto(id);

            produtoViewModel.Fornecedor = produtoAtualizacao.Fornecedor;

            produtoViewModel.Imagem = produtoAtualizacao.Imagem;


            if (!ModelState.IsValid)
                return View(produtoViewModel);


            //Se teve alteração da imagem
            if (produtoViewModel.ImagemUpload != null)
            {
                var imgPrefixo = Guid.NewGuid() + "_";
                if (!await UploadArquivo(produtoViewModel.ImagemUpload, imgPrefixo))
                    return View(produtoViewModel);

                produtoAtualizacao.Imagem = imgPrefixo + produtoViewModel.ImagemUpload.FileName;
            }

            //utilizamos esses produtoAtualizacao para evitar a tentativa de atualização do fornecedor
            produtoAtualizacao.Nome = produtoViewModel.Nome;
            produtoAtualizacao.Descricao = produtoViewModel.Descricao;
            produtoAtualizacao.Valor = produtoViewModel.Valor;
            produtoAtualizacao.Ativo = produtoViewModel.Ativo;


            var produto = _mapper.Map<Produto>(produtoAtualizacao);

            await _produtoService.Atualizar(produto);

            if (OperacaoValida())
                return View(produtoViewModel);

            return RedirectToAction("Index");
        }

        // GET: Produtos/Delete/5
        [Route("excluir-produto/{id:guid}")]
        [ClaimsAuthorize("Produto", "Excluir")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var produtoViewModel = await ObterProduto(id);

            if (produtoViewModel == null)
            {
                return NotFound();
            }


            return View(produtoViewModel);
        }

        // POST: Produtos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("excluir-produto/{id:guid}")]
        [ClaimsAuthorize("Produto", "Excluir")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var produto = await ObterProduto(id);

            if (produto == null)
            {
                return NotFound();
            }

            await _produtoService.Remover(id);

            if (!OperacaoValida())
                return View(produto);

            TempData["Sucesso"] = "Produto excluído com sucesso.";

            return RedirectToAction("Index");
        }



        private async Task<ProdutoViewModel> ObterProduto(Guid id)
        {
            var produto = _mapper.Map<ProdutoViewModel>(await _produtoRepository.ObterProdutoFornecedor(id));

            produto.Fornecedores = _mapper.Map<IEnumerable<FornecedorViewModel>>(await _fornecedorRepository.ObterTodos());

            return produto;
        }

        //Para cada produto que eu passo ele popula os fornecedores para mim
        private async Task<ProdutoViewModel> PopularFornecedores(ProdutoViewModel produto)
        {
            produto.Fornecedores = _mapper.Map<IEnumerable<FornecedorViewModel>>(await _fornecedorRepository.ObterTodos());
            return produto;
        }

        private async Task<bool> UploadArquivo(IFormFile arquivo, string imgPrefixo)
        {
            if (arquivo.Length <= 0)
                return false;

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagens", imgPrefixo + arquivo.FileName);

            if (System.IO.File.Exists(path))
            {
                ModelState.AddModelError(string.Empty, "Já existe um arquivo com este nome!");
                return false;
            }

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await arquivo.CopyToAsync(stream); //Método que está gravando o arquivo no disco, copiando para o disco
            }

            return true;

        }


    }
}

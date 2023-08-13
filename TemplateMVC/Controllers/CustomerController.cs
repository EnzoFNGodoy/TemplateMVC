using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TemplateMVC.Data;
using TemplateMVC.Models;

namespace TemplateMVC.Controllers;

// Configuração padrão para o Controller
[ApiController]
// Especifica a rota padrão
[Route("customers")]
public sealed class CustomerController : ControllerBase
{
    // Injeção de Dependência
    private readonly TemplateMVCContext _context;

    public CustomerController(TemplateMVCContext context)
    {
        _context = context;
    }
    //

    [HttpGet]
    public IActionResult GetAllCustomers()
    {
        // Recuperando a lista de Customers
        // Utilize o AsNoTracking para não haver problemas com consulta 
        // e edição de um mesmo registro
        var response = _context.Customers.AsNoTracking().ToList();

        // Se houver Customer: Limpar as senhas para o retorno
        if (response.Any())
            response.ForEach(customer => customer.Password = string.Empty);

        // Retornando código 200 com a lista de Customers
        return Ok(response);
    }

    [HttpGet("{id}")]
    public IActionResult GetCustomerById(Guid id)
    {
        try
        {
            // Recuperando / Verificando o Customer com o ID especificado
            // Utilize o AsNoTracking para não haver problemas com consulta 
            // e edição de um mesmo registro
            var response = _context.Customers.AsNoTracking().FirstOrDefault(customer => customer.Id == id);

            // Caso não exista um Customer com o ID especificado: Retornar 404
            if (response is null)
                return NotFound();

            // Se existir, limpar a senha
            response.Password = string.Empty;

            // Retornando código 200 e o Customer com o ID especificado
            return Ok(response);
        }
        catch (Exception ex)
        {
            // Caso haja erro, retornar 400 mais a mensagem de erro
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public IActionResult PostCustomer([FromBody] Customer customer)
    {
        try
        {
            // Validando o Customer
            if (!ModelState.IsValid)
                return BadRequest("Cliente inválido.");

            // Verificando se existe algum Customer com o email passado pelo corpo
            // Utilize o AsNoTracking para não haver problemas com consulta 
            // e edição de um mesmo registro
            var customerExists = _context.Customers.AsNoTracking().FirstOrDefault(c => c.Email == customer.Email);
            // Caso exista: Retorna 400 mais uma mensagem de erro
            if (customerExists is not null)
                return BadRequest("Um cliente com esse email já existe.");

            // Registra o Customer
            _context.Customers.Add(customer);
            // Salva as alterações (Persiste no BD)
            _context.SaveChanges();

            // Se der tudo certo, retorna um 201 e recupera o objeto do Customer criado
            // para ser retornado
            return CreatedAtAction(nameof(GetCustomerById), new { customer.Id }, customer);
        }
        catch (Exception ex)
        {
            // Caso haja erro, retornar 400 mais a mensagem de erro
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public IActionResult PutCustomer(Guid id, [FromBody] Customer customer)
    {
        try
        {
            // Recuperando / Verificando o Customer com o ID especificado
            // Utilize o AsNoTracking para não haver problemas com consulta 
            // e edição de um mesmo registro
            var customerExists = _context.Customers.AsNoTracking().FirstOrDefault(customer => customer.Id == id);

            // Caso não exista um Customer com o ID especificado: Retornar 404
            if (customerExists is null)
                return NotFound();

            // Verificando se existe algum Customer com o email passado pelo corpo 
            // e com o ID diferente do atual
            // Utilize o AsNoTracking para não haver problemas com consulta 
            // e edição de um mesmo registro
            customerExists = _context.Customers.AsNoTracking().FirstOrDefault(c => c.Id != id 
            && c.Email == customer.Email);
            // Caso exista: Retorna 400 mais uma mensagem de erro
            if (customerExists is not null)
                return BadRequest("Um cliente com esse email já existe.");

            // Colocando o ID correto no atual objeto passado
            customer.Id = id;

            // Atualiza o Customer
            _context.Customers.Update(customer);
            // Salva as alterações (Persiste no BD)
            _context.SaveChanges();

            // Retornando código 200 e o Customer editado
            return Ok(customer);
        }
        catch (Exception ex)
        {
            // Caso haja erro, retornar 400 mais a mensagem de erro
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteCustomer(Guid id)
    {
        try
        {
            // Recuperando / Verificando o Customer com o ID especificado
            // Utilize o AsNoTracking para não haver problemas com consulta 
            // e edição de um mesmo registro
            var customerExists = _context.Customers.AsNoTracking().FirstOrDefault(customer => customer.Id == id);

            // Caso não exista um Customer com o ID especificado: Retornar 404
            if (customerExists is null)
                return NotFound();

            // Deleta o Customer
            _context.Customers.Remove(customerExists);
            // Salva as alterações (Persiste no BD)
            _context.SaveChanges();

            // Retornando código 200 e uma mensagem de sucesso
            return Ok("Cliente deletado com sucesso.");
        }
        catch (Exception ex)
        {
            // Caso haja erro, retornar 400 mais a mensagem de erro
            return BadRequest(ex.Message);
        }
    }
}
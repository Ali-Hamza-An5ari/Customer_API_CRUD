using Dapper;
using dapperCRUD.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace dapperCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IConfiguration _config;
        public CustomerController(IConfiguration config)
        {
            _config = config;
        }
        private SqlConnection GetConnection()
        {
            return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        }


        private static async Task<IEnumerable<Customer>> SelectAllCustomers(SqlConnection connection)
        {
            return await connection.QueryAsync<Customer>("Select * from customers");
        }

        [HttpGet("/GetAll")]

        public async Task<ActionResult<List<Customer>>> GetAllCustomers()
        {
            using var connection = GetConnection();
            IEnumerable<Customer> customers = await SelectAllCustomers(connection);
            return Ok(customers);
        }


        [HttpGet("/Get/{customerId}")]
        public async Task<ActionResult<Customer>> GetCustomer(int customerId)
        {
            using var connection = GetConnection();
            var customer = await connection.QueryFirstOrDefaultAsync<Customer>("SELECT * FROM customers WHERE Id=@Id",
                new { Id = customerId });

            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }


        [HttpPost("/Create")]
        public async Task<ActionResult<List<Customer>>> CreateCustomer(Customer customer)
        {
            using var connection = GetConnection();
            await connection.ExecuteAsync("INSERT INTO customers(Name,Email, DateOfBirth) VALUES (@Name, @Email, @DateOfBirth)",
                customer);
            return Ok(await SelectAllCustomers(connection));
        }

        [HttpPut("/UpdateCustomer")]
        public async Task<ActionResult<List<Customer>>> UpdateCustomer(Customer customer)
        {
            using var connection = GetConnection();
            var searchedCust = await connection.QueryFirstOrDefaultAsync<Customer>("SELECT * FROM customers WHERE Id=@Id",
                new { Id = customer.Id });

            if (searchedCust == null)
            {
                // If the customer does not exist, return a 404 Not Found response
                return NotFound();
            }

            await connection.ExecuteAsync("UPDATE customers SET Name=@Name, Email=@Email WHERE id=@Id",
                customer);
            return Ok(await SelectAllCustomers(connection));
        }


        [HttpDelete("/Delete/{customerId}")]
        public async Task<ActionResult<List<Customer>>> DeleteCustomer(int customerId)
        {
            using var connection = GetConnection();

            // Check if the customer exists
            var customer = await connection.QueryFirstOrDefaultAsync<Customer>("SELECT * FROM customers WHERE Id=@Id",
                new { Id = customerId });

            if (customer == null)
            {
                // If the customer does not exist, return a 404 Not Found response
                return NotFound();
            }

            // If the customer exists, delete it
            await connection.ExecuteAsync("DELETE FROM customers WHERE id=@Id", new { Id = customerId });

            return Ok(await SelectAllCustomers(connection));
        }


        //[HttpDelete("{customerId}")]
        //public async Task<ActionResult<List<Customer>>> DeleteCustomer(int customerId)
        //{
        //    using var connection = GetConnection();
        //    await connection.ExecuteAsync("DELETE FROM customers  WHERE id=@Id",
        //        new {Id = customerId});



        //    return Ok(await SelectAllCustomers(connection));
        //}
    }
}

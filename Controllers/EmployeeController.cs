using Microsoft.AspNetCore.Mvc;  
using Microsoft.AspNetCore.SignalR;  
using Microsoft.EntityFrameworkCore;
using System;  
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SignalR.Model;
using SignalR.Data;
using SignalR.Interface;

namespace SignalR.Controllers  
{  
    [Route("api/[controller]")]  
    [ApiController]  
    public class EmployeesController : ControllerBase  
    {  
        private readonly MyDbContext _context;  
        private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;  
  
        public EmployeesController(MyDbContext context, IHubContext<BroadcastHub, IHubClient> hubContext)  
        {  
            _context = context;  
            _hubContext = hubContext;  
        }  
  
        // GET: api/Employees  
        [HttpGet]  
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployee()  
        {  
            return await _context.Employees.ToListAsync();  
        }  
  
        // GET: api/Employees/5  
        [HttpGet("{id}")]  
        public async Task<ActionResult<Employee>> GetEmployee(string id)  
        {  
            var employee = await _context.Employees.FindAsync(id);  
  
            if (employee == null)  
            {  
                return NotFound();  
            }  
  
            return employee;  
        }  
        
        [HttpPut("{id}")]  
        public async Task<IActionResult> PutEmployee(string id, Employee employee)  
        {  
            if (id != employee.Id)  
            {  
                return BadRequest();  
            }  
  
            _context.Entry(employee).State = EntityState.Modified;  
  
            Notification notification = new Notification()  
            {  
                EmployeeName = employee.Name,  
                TranType = "Edit"  
            };  
            _context.Notifications.Add(notification);  
  
            try  
            {  
                await _context.SaveChangesAsync();  
                await _hubContext.Clients.All.BroadcastMessage();  
            }  
            catch (DbUpdateConcurrencyException)  
            {  
                if (!EmployeesExists(id))  
                {  
                    return NotFound();  
                }  
                else  
                {  
                    throw;  
                }  
            }  
  
            return NoContent();  
        }  
        
        [HttpPost]  
        public async Task<ActionResult<Employee>> PostEmployee(Employee employee)  
        {  
            employee.Id = Guid.NewGuid().ToString();  
            _context.Employees.Add(employee);  
  
            Notification notification = new Notification()  
            {  
                EmployeeName = employee.Name,  
                TranType = "Add"  
            };  
            _context.Notifications.Add(notification);  
  
            try  
            {  
                await _context.SaveChangesAsync();  
                await _hubContext.Clients.All.BroadcastMessage();  
            }  
            catch (DbUpdateException)  
            {  
                if (EmployeesExists(employee.Id))  
                {  
                    return Conflict();  
                }  
                else  
                {  
                    throw;  
                }  
            }  
  
            return CreatedAtAction("GetEmployee", new { id = employee.Id }, employee);  
        }  
  
        // DELETE: api/Employees/5  
        [HttpDelete("{id}")]  
        public async Task<IActionResult> DeleteEmployee(string id)  
        {  
            var employee = await _context.Employees.FindAsync(id);  
            if (employee == null)  
            {  
                return NotFound();  
            }  
  
            Notification notification = new Notification()  
            {  
                EmployeeName = employee.Name,  
                TranType = "Delete"  
            };  
  
            _context.Employees.Remove(employee);  
            _context.Notifications.Add(notification);  
  
            await _context.SaveChangesAsync();  
            await _hubContext.Clients.All.BroadcastMessage();  
  
            return NoContent();  
        }  
  
        private bool EmployeesExists(string id)  
        {  
            return _context.Employees.Any(e => e.Id == id);  
        }  
    }  
}
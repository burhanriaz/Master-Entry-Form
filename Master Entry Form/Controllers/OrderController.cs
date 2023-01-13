using Master_Entry_Form.Data;
using Master_Entry_Form.Models;
using Master_Entry_Form.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Master_Entry_Form.Controllers
{

    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext dbContext)
        {
            _context = dbContext;
        }

        [HttpGet]
        [Route("/Index")]
        public async Task<ActionResult<IEnumerable<Order>>> Index()
        {
            var orders = await _context.Orders.ToListAsync();
            return Ok(orders);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<OrderVM>> GetOrder(int id)
        {
            var order = await _context.Orders.Include(o => o.OrderDetails)
                .SingleOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            var orderViewModel = new OrderVM
            {
                OrderNo = order.OrderNo,
                Date = order.Date,
                OrderDetails = order.OrderDetails.Select(od => new OrderDetail
                {
                    OrderId = od.OrderId,
                    Total = od.Total,
                    Rate = od.Rate,
                    Quentity = od.Quentity,
                    itemName = od.itemName,
                }).ToList()
            };

            return Ok(orderViewModel);
        }


        [HttpGet]
        [Route("/Create")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [Route("/Create")]
        public IActionResult Create(OrderVM orderViewModel)
        {
            bool status = false;
            var order = new Order
            {
                OrderNo = orderViewModel.OrderNo,
                Date = orderViewModel.Date,

                OrderDetails = orderViewModel.OrderDetails.Select(od => new OrderDetail
                {
                    OrderId = od.OrderId,
                    Total = od.Total,
                    Rate = od.Rate,
                    Quentity = od.Quentity,
                    itemName = od.itemName,
                }).ToList()

            };
            _context.Orders.Add(order);
            _context.SaveChanges();

            orderViewModel.Id = order.Id;
            if (orderViewModel.Id != null)
                status = true;

            // return CreatedAtAction("GetOrder", new { id = order.Id }, orderViewModel);
            else
            {
                status = false;
            }
            return Ok(StatusCodes.Status200OK);
        }


        [HttpPost("{id}")]
        public async Task<IActionResult> Update(OrderVM orderViewModel)
        {
            var order = await _context.Orders.Include(o => o.OrderDetails).SingleOrDefaultAsync(o => o.Id == orderViewModel.Id);

            if (order is null)
            {
                return NotFound();
            }
            order.OrderNo = orderViewModel.OrderNo;
            order.Date = orderViewModel.Date;

            // Update existing OrderDetails
            var existingOrderDetails = order.OrderDetails.ToList();
            for (int i = 0; i < existingOrderDetails.Count; i++)
            {
                var existingOrderDetail = existingOrderDetails[i];
                var updatedOrderDetail = orderViewModel.OrderDetails.FirstOrDefault(od => od.OrderDetaiId == existingOrderDetail.OrderDetaiId);
                if (updatedOrderDetail != null)
                {
                    existingOrderDetail.Quentity = updatedOrderDetail.Quentity;
                    existingOrderDetail.Rate = updatedOrderDetail.Rate;
                    existingOrderDetail.Total = updatedOrderDetail.Total;
                    existingOrderDetail.itemName = updatedOrderDetail.itemName;

                }
            }

            // Add new OrderDetails
            var newOrderDetails = orderViewModel.OrderDetails.Where(od => od.OrderDetaiId == 0).ToList();
            for (int i = 0; i < newOrderDetails.Count; i++)
            {
                var newOrderDetail = newOrderDetails[i];
                order.OrderDetails.Add(new OrderDetail
                {
                    Quentity = newOrderDetail.Quentity,
                    Rate = newOrderDetail.Rate,
                    Total = newOrderDetail.Total,
                    itemName = newOrderDetail.itemName
                });
            }

            // Remove deleted OrderDetails
            var deletedOrderDetails = existingOrderDetails.Where(od => !orderViewModel.OrderDetails.Any(odvm => odvm.OrderDetaiId == od.OrderDetaiId)).ToList();
            for (int i = 0; i < deletedOrderDetails.Count(); i++)
            {
                var deletedOrderDetail = deletedOrderDetails[i];
                order.OrderDetails.Remove(deletedOrderDetail);
            }

            await _context.SaveChangesAsync();
            return Ok(true);
        }

        public IActionResult Delete(int id)
        {
            var order = _context.Orders.SingleOrDefault(o => o.Id == id);

            if (order is null)
            {
                return NotFound();
            }
            _context.Orders.Remove(order);
            _context.SaveChanges();
            return Ok(true);
        }
    }
}
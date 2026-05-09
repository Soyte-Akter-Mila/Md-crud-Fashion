using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MD_Crud_Fashion.Data;

namespace MD_Crud_Fashion.ViewComponents
{
    public class ActiveOrdersViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public ActiveOrdersViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var activeCount = await _context.OrderEntries
                .Include(o => o.Customer)
                .CountAsync(o => o.Customer.UrgentDelivery == true);

            return View(activeCount); 
        }


    }
}
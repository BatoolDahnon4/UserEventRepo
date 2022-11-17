using MedcorSL.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eventRegistration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuestController : Controller
    {
        private IGuest _IGuest;
        private readonly GContext _context;
        private readonly IEmailService emailService;

        public GuestController(GContext dbContext, IEmailService emailService)
        {
            _context = dbContext;
            this.emailService = emailService;
        }

        [HttpGet]
        [Route("getGuestes")]
        public async Task<ActionResult<List<Guest>>> GetGuestes()
        {
            var guest = await _context.Guest.Select(v => new Guest()
            {
                Id = v.Id,
                Name = v.Name,
                Position = v.Position,
                Email = v.Email,
                CompanyName = v.CompanyName,
                PhoneNumber = v.PhoneNumber
            }).ToListAsync();
            return Ok(guest);
        }

        [HttpGet]
        [Route("getGuestById")]
        public async Task<ActionResult<Guest>> GetGuest(int Id)
        {
            var guest = await _context.Guest.Where(e => e.Id == Id).FirstOrDefaultAsync();
            if (guest == null)
                return BadRequest("not found");
            return Ok(guest);
        }

        [HttpPost]
        [Route("addGuest")]
        public async Task<ActionResult<Guest>> AddGuest(Guest guest)
        {
            var oldEmail= await _context.Guest.AnyAsync(c => c.Email == guest.Email);
            if (oldEmail)
            {
                return BadRequest("Use another email please!");
            }


            await _context.Guest.AddAsync(guest);
             _context.SaveChanges();

            await emailService.SendRegistrationEmailAsync(guest);
            return Ok("");
        }

    }
}

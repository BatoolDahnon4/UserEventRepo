using MedcorSL.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
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

        public string lblCount;

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
                PhoneNumber = v.PhoneNumber,
                Source = v.Source,
                Okay = v.Okay,
            }).ToListAsync();
            return Ok(guest);
        }

        [HttpGet]
        [Route("getGuestByGuid/{id}")]
        public async Task<ActionResult<Guest>> GetGuest(Guid id)
        {
            var guest = await _context.Guest.FindAsync(id);

            if (guest == null)
                return BadRequest("not found");

            guest.IsAttended = true;
            await _context.SaveChangesAsync();

            return Ok(guest);
        }

        [HttpPost]
        [Route("addGuest")]
        public async Task<ActionResult<Guest>> AddGuest(Guest guest)
        {
            var oldEmail = await _context.Guest.AnyAsync(c => c.Email == guest.Email);
            if (oldEmail)
            {
                return BadRequest("Use another email please!");
            }
            Random generator = new Random();
            var count = generator.Next(100000, 1000000);




            //var count = await _context.Guest.CountAsync();



            await _context.Guest.AddAsync(guest);
            _context.SaveChanges();
            if (guest.Source == "gaza" || guest.Source == "wb")
            {
                await emailService.SendRegistrationEmailAsync(guest, count + 1, guest.Id);
            }
            else if (guest.Source == "conferencegaza" || guest.Source == "conferencewb")
            {
                await emailService.SendRegistrationEmailAsync(guest.Id, guest);
            }
            return Ok("");

        }


    }
}

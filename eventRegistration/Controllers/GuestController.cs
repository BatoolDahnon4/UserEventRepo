using MedcorSL.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace eventRegistration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuestController : Controller
    {
        private IGuest _IGuest;
        private readonly GContext _context;
        private readonly IEmailService emailService;
        public List<Guest> guests= new List<Guest>();
        public string lblCount;

        public GuestController(GContext dbContext, IEmailService emailService)
        {
            _context = dbContext;
            this.emailService = emailService;
        }


        [HttpGet]
        [Route("getConfernceGuestes")]
        
        public async Task<ActionResult<List<Guest>>> getConfernceGuestes()
        {


            var guest = await _context.Guest.Where(e => e.Source == "conferencegaza"|| e.Source == "conferencewb").ToListAsync();

            if (guest == null)
            {
                return BadRequest("not found");
            }
            return Ok(guest);
        }
        [HttpGet]
        [Route("getGalaGuestes")]

        public async Task<ActionResult<List<Guest>>> getGalaGuestes()
        {


            var guest = await _context.Guest.Where(e => e.Source == "gaza" || e.Source == "wb").ToListAsync();

            if (guest == null)
            {
                return BadRequest("not found");
            }
            return Ok(guest);
        }

        [HttpGet]
        [Route("getGuestByGuid/{id}")]
        public async Task<ActionResult<Guest>> GetGuest(Guid id)
        {
            var guest = await _context.Guest.FindAsync(id);

            if (guest == null)
                return BadRequest("not found");

            //guest.IsAttended = true;
            //await _context.SaveChangesAsync();

            return Ok(guest);
        }

        [HttpPost]
        [Route("addGuest")]
        public async Task<ActionResult<Guest>> AddGuest(Guest guest)
        {
            //var oldEmail = await _context.Guest.AnyAsync(c => c.Email == guest.Email);
            //if (oldEmail)
            //{
            //    return BadRequest("Use another email please!");
            //}
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

        [HttpPut]
        [Route("updateGuest")]
        public async Task<ActionResult<Guest>> updateGuest(Guest gst)
        {
            var guest = _context.Guest.Find(gst.Id);
            if (guest == null)
            {
                return NotFound("Not found");
            }

            guest.IsAttended = gst.IsAttended;
            guest.Table = gst.Table;
            guest.CompanyName = gst.CompanyName;
            guest.PhoneNumber = gst.PhoneNumber;
            guest.Email = gst.Email;
            guest.Source = gst.Source;
            guest.Position= gst.Position;
            guest.Name = gst.Name;
            guest.Okay = gst.Okay;

            await _context.SaveChangesAsync();
            return Ok(guest);
        }
        

        [ClaimRequirementFilter]
        [HttpGet]
        public IActionResult GetResource()
        {
            return Ok();
        }
        

    }
}

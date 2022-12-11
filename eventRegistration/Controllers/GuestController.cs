using eventRegistration.Jobs;
using Hangfire;
using MedcorSL.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
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
        private readonly EmailConfig _emailConfig;
        private readonly IEmailService emailService;
        public List<Guest> guests= new List<Guest>();
        public string lblCount;

        public GuestController(GContext dbContext, IEmailService emailService, EmailConfig emailConfig)
        {
            _context = dbContext;
            this.emailService = emailService;
            _emailConfig = emailConfig;
        }

        [HttpPost]
        [Route("sendInvitationQRtoGuests")]

        public async Task<ActionResult> sendInvitationQRtoGuests(List<Guest> guests)
        {
            var guestIds = guests.Select(g => g.Id).ToList();
            var validGuests = _context.Guest.Where(guest => guestIds.Contains(guest.Id)).Where(guest => guest.Source.Equals("wb")).ToList();

            validGuests.ForEach(g =>
            {
                BackgroundJob.Enqueue(() => EmailJob.SendInvitationQR(g, _emailConfig));
            });
            return Accepted();
        }

        [HttpGet]
        [Route("getConfernceGuestes")]
        
        public async Task<ActionResult<List<Guest>>> getConfernceGuestes([FromQuery] string source)
        {
            var validSources = new List<string>() { "conferencegaza", "conferencewb" };
            if (!validSources.Contains(source.ToLower()))
            {
                return BadRequest("Invalid Source");
            }

            var guest = await _context.Guest.Where(e => e.Source == source.ToLower()).OrderBy(e => e.CompanyName).ToListAsync();

            if (guest == null)
            {
                return NoContent();
            }
            return Ok(guest);
        }
        [HttpGet]
        [Route("getGalaGuestes")]

        public async Task<ActionResult<List<Guest>>> getGalaGuestes([FromQuery]string source)
        {
            var validSources = new List<string>() { "wb","gaza"};
            if (!validSources.Contains(source.ToLower()))
            {
                return BadRequest("Invalid Source");
            }
            var guest = await _context.Guest.Where(e => e.Source == source.ToLower()).OrderBy(e => e.CompanyName).ToListAsync();

            if (guest == null)
            {
                return NoContent();
            }
            return Ok(guest);
        }

        [HttpGet]
        [Route("getGuestByGuid/{id}")]
        public async Task<ActionResult<Guest>> GetGuest(Guid id)
        {
            var guest = await _context.Guest.FindAsync(id);
            
            if (guest == null)
            {
                return NotFound();
            }
                
            guest.IsAttended = true;
            await _context.SaveChangesAsync();

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

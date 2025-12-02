using Microsoft.AspNetCore.Mvc;
using HotelListingAPI.Data;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HotelListingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelsController : ControllerBase
    {
        private static List<Hotel> _hotels = new List<Hotel>() { 
            new Hotel{Id = 1, Name="Plaza", Address="Main Street 204", Rating=10.0 },
            new Hotel{Id = 2, Name="Remington", Address="Second Street 11", Rating=7.3 },
            new Hotel{Id = 3, Name="Ocean View", Address="Cinq Boulevard 53", Rating=5.0 },
            new Hotel{Id = 4, Name="Excelsior", Address="German Allee 99", Rating=9.0 }
        };

        // GET: api/<HotelsController>
        [HttpGet]
        public ActionResult<IEnumerable<Hotel>> Get()
        {

            return Ok(_hotels);
        }

        // GET api/<HotelsController>/5
        [HttpGet("{id}")]
        public ActionResult<Hotel> Get(int id)
        {
            var hotel = _hotels.FirstOrDefault(h => h.Id == id );
            if (hotel == null)
            {
                return NotFound(); 
            }
            return Ok(hotel);
        }

        // POST api/<HotelsController>
        [HttpPost]
        public ActionResult<Hotel> Post([FromBody] Hotel value)
        {
            if(_hotels.Any(h => h.Id == value.Id))
            {
                return BadRequest("Hotel with this Id already exists");
            }
            _hotels.Add(value);
            return CreatedAtAction(nameof(Post), new { id = value.Id }, value);
        }

        // PUT api/<HotelsController>/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] Hotel value)
        {
            var hotel = _hotels.FirstOrDefault(h => h.Id == id);
            if (hotel == null)
            {
                return NotFound($"Hotel with Id {value.Id} does not exist");
            }
            if (value.Id != id)
            {
                return BadRequest($"Hotel with Id {value.Id} does not match request Id. Updating key fields not allowed.");
            }
            hotel.Name = value.Name;
            hotel.Address = value.Address;
            hotel.Rating = value.Rating;
            return NoContent();
        }

        // DELETE api/<HotelsController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var hotel = _hotels.FirstOrDefault(h => h.Id == id);
            if (hotel == null)
            {
                return NotFound($"Hotel with Id {id} does not exist");
            }
            _hotels.Remove(hotel);
            return NoContent();
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PWEB_Proiect.DTOs;

namespace PWEB_Proiect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : Controller
    {
        private readonly AppDbContext _context;

        public RoomController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "admin")]
        [HttpPost("create_room")]
        public async Task<IActionResult> CreateRoom([FromBody] RoomCreateRequest roomDto)
        {
            if (!ModelState.IsValid)
            {
                /*return BadRequest(ModelState);*/
                return Ok(new ErrorMessageDTO() { Error = "Invalid data" });
            }

            var existingRoom = await _context.Rooms
                .AnyAsync(r => r.Building == roomDto.Building && r.Floor == roomDto.Floor && r.Capacity == roomDto.Capacity);

            if (existingRoom)
            {
                return Ok(new ErrorMessageDTO() { Error = "A room with the same building, floor and capacity already exists." });
            }

            var room = new Room
            {
                Capacity = roomDto.Capacity,
                Building = roomDto.Building,
                Floor = roomDto.Floor,
                MonthlyCost = roomDto.MonthlyCost,
                NrRoom = roomDto.NrRoom
            };

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Room successfully created", id = room.Id });
        }

        [HttpGet("get_rooms")]
        public async Task<IActionResult> GetRooms()
        {
            var rooms = await _context.Rooms
                .OrderBy(r => r.Building).ThenBy(r => r.Floor).ThenBy(r => r.NrRoom)
                .GroupBy(r => r.Building)
                .Select(g => new { Name = g.Key, Rooms = g.Select(r=> new
                { // deocamdata nu avem nevoie de toate proprietatile
                    r.Capacity,
                    r.Building,
                    r.Floor,
                    r.MonthlyCost,
                    r.NrRoom
                }).ToList() })
                .ToListAsync();

            return Ok(rooms);
        }

        /*[HttpGet("get_buildings")]
        public async Task<IActionResult> GetBuildings()
        {
            var buildings = await _context.Rooms
                .Select(r => r.Building)
                .Distinct()
                .ToListAsync();

            return Ok(buildings);
        }*/



    }
}

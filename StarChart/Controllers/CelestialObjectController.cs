﻿using System.Linq;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }        

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects.Find(id);

            if(celestialObject == null)
            {
                return NotFound();
            }

            celestialObject.Satellites = _context.CelestialObjects.Where(o => o.OrbitedObjectId == id).ToList();

            return Ok(celestialObject);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = _context.CelestialObjects.Where(o => o.Name == name).ToList();

            if (celestialObjects.Count < 1)
            {
                return NotFound();
            }

            foreach (var item in celestialObjects)
            {
                item.Satellites = _context.CelestialObjects.Where(o => o.OrbitedObjectId == item.Id).ToList();
            }

            return Ok(celestialObjects);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects;

            foreach (var item in celestialObjects)
            {
                item.Satellites = _context.CelestialObjects.Where(o => o.OrbitedObjectId == item.Id).ToList();
            }

            return Ok(celestialObjects);
        }

        [HttpPost]
        public IActionResult Create([FromBody]CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();

            return CreatedAtRoute("GetById", new { Id = celestialObject.Id }, celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObject)
        {
            var celestialObjectToUpdate = _context.CelestialObjects.Find(id);

            if (celestialObjectToUpdate == null)
            {
                return NotFound();
            }

            celestialObjectToUpdate.Name = celestialObject.Name;
            celestialObjectToUpdate.OrbitalPeriod = celestialObject.OrbitalPeriod;
            celestialObjectToUpdate.OrbitedObjectId = celestialObject.OrbitedObjectId;

            _context.CelestialObjects.Update(celestialObjectToUpdate);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var celestialObjectToUpdate = _context.CelestialObjects.Find(id);

            if (celestialObjectToUpdate == null)
            {
                return NotFound();
            }

            celestialObjectToUpdate.Name = name;

            _context.CelestialObjects.Update(celestialObjectToUpdate);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var celestialObjectsToDelete = _context.CelestialObjects.Where(o => o.Id == id || o.OrbitedObjectId == id).ToList();

            if (celestialObjectsToDelete.Count < 1)
            {
                return NotFound();
            }

            _context.CelestialObjects.RemoveRange(celestialObjectsToDelete);
            _context.SaveChanges();

            return NoContent();
        }
    }
}

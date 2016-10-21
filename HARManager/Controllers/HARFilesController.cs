using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using HARManager.Models;

namespace HARManager.Controllers
{
    public class HARFilesController : ApiController
    {
        private HARManagerEntities db = new HARManagerEntities();

        // GET: api/HARFiles
        public IQueryable<HARFile> GetHARFiles()
        {
            return db.HARFiles;
        }

        // GET: api/HARFiles/5
        [ResponseType(typeof(HARFile))]
        public IHttpActionResult GetHARFile(int id)
        {
            HARFile hARFile = db.HARFiles.Find(id);
            if (hARFile == null)
            {
                return NotFound();
            }

            return Ok(hARFile);
        }

        // PUT: api/HARFiles/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutHARFile(int id, HARFile hARFile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != hARFile.id)
            {
                return BadRequest();
            }

            db.Entry(hARFile).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HARFileExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/HARFiles
        [ResponseType(typeof(HARFile))]
        public IHttpActionResult PostHARFile(HARFile hARFile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.HARFiles.Add(hARFile);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = hARFile.id }, hARFile);
        }

        // DELETE: api/HARFiles/5
        [ResponseType(typeof(HARFile))]
        public IHttpActionResult DeleteHARFile(int id)
        {
            HARFile hARFile = db.HARFiles.Find(id);
            if (hARFile == null)
            {
                return NotFound();
            }

            db.HARFiles.Remove(hARFile);
            db.SaveChanges();

            return Ok(hARFile);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool HARFileExists(int id)
        {
            return db.HARFiles.Count(e => e.id == id) > 0;
        }
    }
}
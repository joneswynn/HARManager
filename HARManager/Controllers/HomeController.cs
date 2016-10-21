using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HARManager.Models;
using System.Web.Script.Serialization;
using System.IO;
using Newtonsoft.Json;

namespace HARManager.Controllers
{
    public class HomeController : Controller
    {
        private HARManagerEntities db = new HARManagerEntities();
        private HARFilesController hfc = new HARFilesController();

        // GET: Home
        public ActionResult Index()
        {
            if (Session["Error"] != null) {
                ViewBag.Error = "<br /><br /><font color='red'>" + Session["Error"] + "</font><br/>";
                Session["Error"] = null;
            }

            List<HARFile> hrl = new List<HARFile>();
            hrl = hfc.GetHARFiles().ToList();

            //return View(await db.HARFiles.ToListAsync());
            return View(hrl);
        }

        // GET: Home/Details/5
        public ActionResult Details(int id)
        {
            string jsonResponse = hfc.GetHARFile(id).ToString();

            JavaScriptSerializer jss = new JavaScriptSerializer();
            HARFile harFile = jss.Deserialize<HARFile>(jsonResponse);

            if (harFile == null)
            {
                return HttpNotFound();
            }
            return View(harFile);
        }

        // GET: Home/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Home/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,method,URL")] HARFile hARFile)
        {
            //if (ModelState.IsValid)
            //{
            //    db.HARFiles.Add(hARFile);
            //    await db.SaveChangesAsync();
            //    return RedirectToAction("Index");
            //}

            hfc.PostHARFile(hARFile);


            return View(hARFile);
        }

        // GET: Home/Edit/5
        public ActionResult Edit(int id)
        {
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            //HARFile hARFile = await db.HARFiles.FindAsync(id);
            //if (hARFile == null)
            //{
            //    return HttpNotFound();
            //}
            
            var harFile = hfc.GetHARFile(id);
            HARFile harFileMain = ((System.Web.Http.Results.OkNegotiatedContentResult<HARManager.Models.HARFile>)harFile).Content;

            //hfc.PutHARFile(id, harFile);
            return View(harFileMain);
        }

        // POST: Home/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,method,URL")] HARFile hARFile)
        {
            //if (ModelState.IsValid)
            //{
            //    db.Entry(hARFile).State = EntityState.Modified;
            //    await db.SaveChangesAsync();
            //    return RedirectToAction("Index");
            //}


            hfc.PutHARFile(hARFile.id, hARFile);

            return View(hARFile);
        }

        // GET: Home/Delete/5
        public ActionResult Delete(int id)
        {
            //HARFile hARFile = await db.HARFiles.FindAsync(id);

            var harFile = hfc.GetHARFile(id);
            HARFile harFileMain = ((System.Web.Http.Results.OkNegotiatedContentResult<HARManager.Models.HARFile>)harFile).Content;


            return View(harFileMain);
        }

        // POST: Home/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //HARFile hARFile = await db.HARFiles.FindAsync(id);
            //db.HARFiles.Remove(hARFile);
            //await db.SaveChangesAsync();

            var harFile = hfc.GetHARFile(id);
            HARFile harFileMain = ((System.Web.Http.Results.OkNegotiatedContentResult<HARManager.Models.HARFile>)harFile).Content;


            hfc.DeleteHARFile(id);

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // This action handles the form POST and the upload
        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file)
        {
            // Verify that the user selected a file
            if (file != null && file.ContentLength > 0)
            {
                string result = new StreamReader(file.InputStream).ReadToEnd();

                HARFile harFile = new HARFile();

                dynamic obj = JsonConvert.DeserializeObject<dynamic>(result);

                //Validate it's 1.2
                if (obj.log.version.ToString() == "1.2") { 

                harFile.method = obj.log.entries[0].request.method.ToString();
                harFile.URL = obj.log.entries[0].request.url.ToString();

                hfc.PostHARFile(harFile);
                } else {
                    Session["Error"] = "HAR File does not match version 1.2.  Please choose a HAR File with a version of 1.2.";
                }

            }
            // redirect back to the index action to show the form once again
            return RedirectToAction("Index");
        }

    }
}

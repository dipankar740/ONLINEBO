using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ONLINEBO.Controllers
{
    public class EKYCController : Controller
    {
        // GET: EKYC
        public ActionResult Index()
        {
            return View();
        }

        // GET: EKYC/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: EKYC/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EKYC/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: EKYC/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: EKYC/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: EKYC/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: EKYC/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}

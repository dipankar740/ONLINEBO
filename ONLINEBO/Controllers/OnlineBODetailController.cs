using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ONLINEBO.Models;

namespace ONLINEBO.Controllers
{
    public class OnlineBODetailController : Controller
    {
        //
        // GET: /OnlineBODetail/
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /OnlineBODetail/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /OnlineBODetail/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /OnlineBODetail/Create
        [HttpPost]
        public ActionResult Create(OnlineBODetailModel bomodel)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    OnlineBoDetailDBHandle DB = new OnlineBoDetailDBHandle();

                    if(DB.AddOnlineBoDetail(bomodel))
                    {
                        ViewBag.Message = "BO Details Added Successfully";
                        ModelState.Clear();
                    }
                    else
                        ViewBag.Message = "BO Details Added Failed!!!";
                }

                return View();
                //return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /OnlineBODetail/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /OnlineBODetail/Edit/5
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

        //
        // GET: /OnlineBODetail/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /OnlineBODetail/Delete/5
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

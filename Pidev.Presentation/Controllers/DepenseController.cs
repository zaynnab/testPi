using Pidev.data;
using Presentation.Models;
using Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.text.html.simpleparser;
using System.Net.Mail;

namespace Pidev.Presentation.Controllers
{
    public class DepenseController : Controller
    {
        IMissionService pp = null;
        IExpensesService expService = null;

        public DepenseController()
        {
            expService = new ExpensesService();
            pp = new MissionService();
        }

        // GET: Expense
        public ActionResult Index(string searchString)
        {
            var maliste = new List<exp>();
            IEnumerable<expenses> expDomain = expService.GetAll();
            if (!String.IsNullOrEmpty(searchString))
            {
                expDomain = expService.GetexpByType(searchString);
            }

            foreach (var dep in expDomain)
            {

                maliste.Add(new exp
                {
                    id_Exp = dep.id_Exp,
                    DateExpense = dep.DateExpense,
                    NatureDepense = (NatureVM)dep.NatureDepense,
                    nbNuitete = dep.nbNuitete,
                    moyTransport = (MoyeneVM)dep.moyTransport,
                    distance = dep.distance,
                    Justificatif = dep.Justificatif,
                    MontantTotal = dep.MontantTotal,
                    commentaire = dep.commentaire,
                    mm_id_mission = dep.mm_id_mission,
                    nbVue = dep.nbVue
                    //Frexp_id_frais = dep.Frexp_id_frais
                }) ;
            }
            return View(maliste);
        }


        [HttpPost]
        [ValidateInput(false)]
        public FileResult Export(string GridHtml)
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                StringReader sr = new StringReader(GridHtml);
                Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 100f, 0f);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();
                XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                pdfDoc.Close();
                return File(stream.ToArray(), "application/pdf", "Grid.pdf");
            }
        }













        // GET: Expense/Details/5
        public ActionResult Details(int id)
        {
            var s = expService.GetById(id);
            s.nbVue = s.nbVue + 1;
            return View(s);
          
        }


        // GET: Expense/Create
        public ActionResult Create()
        {
            var missions = pp.GetMany().ToList();

            ViewBag.listmiss = new SelectList(missions, "libelle", "libelle");
            ViewBag.listmiss = missions;
            return View();
        }

        // POST: Expense/Create
        [HttpPost]
        public ActionResult Create(exp dep, HttpPostedFileBase file, int abc)
        {
            var missions = pp.GetMany().ToList();

            ViewBag.listmiss = new SelectList(missions, "libelle", "libelle");
            ViewBag.listmiss = missions;
            if (!ModelState.IsValid || file == null || file.ContentLength == 0)
            {
                RedirectToAction("Create");
            }
            var fileName = "";

            if (file.ContentLength > 0)
            {
                fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath("/Content/Upload/"), file.FileName);
                file.SaveAs(path);
            }
            expenses d = new expenses()
            {
                DateExpense = dep.DateExpense,
                NatureDepense = (Nature)dep.NatureDepense,
                nbNuitete = dep.nbNuitete,
                moyTransport = (Moyene)dep.moyTransport,
                distance = dep.distance,
                Justificatif = file.FileName,
                MontantTotal = dep.MontantTotal,
                commentaire = dep.commentaire,
                mm_id_mission = abc

            };
            d.Justificatif = file.FileName;
            d.mm_id_mission = abc;
            expService.Add(d);
            expService.Commit();
            // expService.Dispose();   //  @Html.DropDownList("m_id_mission", (SelectList)ViewData["listmiss




          

            return RedirectToAction("Index");


        }


        public ActionResult Edit(int id)
        {

            // GET: Expense/Edit/5
            if (id == null)

                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            expenses dep = expService.GetById(id);
            expenses p1 = new expenses()
            {
                DateExpense = dep.DateExpense,
                NatureDepense = (Nature)dep.NatureDepense,
                nbNuitete = dep.nbNuitete,
                moyTransport = (Moyene)dep.moyTransport,
                distance = dep.distance,
                Justificatif = dep.Justificatif,
                MontantTotal = dep.MontantTotal,
                commentaire = dep.commentaire,
                mm_id_mission = dep.mm_id_mission

            };

            if (dep == null)
                return HttpNotFound();
            expService.Update(dep);
            expService.Commit();
            return View(p1);
        }

        // POST: Comment/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, exp dep)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (id == null)
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    expenses p = expService.GetById(id);

                    dep = new exp()
                    {
                        DateExpense = dep.DateExpense,
                        NatureDepense = (NatureVM)dep.NatureDepense,
                        nbNuitete = dep.nbNuitete,
                        moyTransport = (MoyeneVM)dep.moyTransport,
                        distance = dep.distance,
                        // Justificatif = file.FileName,
                        MontantTotal = dep.MontantTotal,
                        commentaire = dep.commentaire,
                        mm_id_mission = dep.mm_id_mission

                    };


                    if (p == null)
                        return HttpNotFound();

                    Console.WriteLine("updaaaaaaaaaaaate");
                    expService.Update(p);
                    expService.Commit();
                    // Service.Dispose();
                    return RedirectToAction("Index");
                    // return RedirectToAction("Index");
                }
                // TODO: Add delete logic here
                return View(dep);

            }
            catch
            {
                return View();
            }
        }


        // GET: Expense/Delete/5
        public ActionResult Delete(int id)
        {

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            expenses dep = expService.GetById(id);
            exp p = new exp()
            {
                DateExpense = dep.DateExpense,
                NatureDepense = (NatureVM)dep.NatureDepense,
                nbNuitete = dep.nbNuitete,
                moyTransport = (MoyeneVM)dep.moyTransport,
                distance = dep.distance,
                Justificatif = dep.Justificatif,
                MontantTotal = dep.MontantTotal,
                commentaire = dep.commentaire,
                mm_id_mission = dep.mm_id_mission

            };

            if (dep == null)
                return HttpNotFound();
            expService.Delete(dep);
            expService.Commit();
            return View(p);
        }



        // POST: Expense/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, exp evt)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    if (id == null)
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    expenses dep = expService.GetById(id);
                    evt = new exp()
                    {
                        DateExpense = dep.DateExpense,
                        NatureDepense = (NatureVM)dep.NatureDepense,
                        nbNuitete = dep.nbNuitete,
                        moyTransport = (MoyeneVM)dep.moyTransport,
                        distance = dep.distance,
                        Justificatif = dep.Justificatif,
                        MontantTotal = dep.MontantTotal,
                        commentaire = dep.commentaire,
                        mm_id_mission = dep.mm_id_mission

                    };
                    if (dep == null)
                        return HttpNotFound();
                    Console.WriteLine("Deleted");
                    expService.Delete(dep);
                    expService.Commit();
                    // Service.Dispose();
                    return RedirectToAction("Index");
                }
                // TODO: Add delete logic here
                return View(evt);

            }
            catch
            {
                return View();
            }
        }
    }
}

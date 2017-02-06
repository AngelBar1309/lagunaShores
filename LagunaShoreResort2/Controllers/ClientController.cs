using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LagunaShoreResort2.Models;
using Microsoft.VisualBasic;

namespace LagunaShoreResort2.Controllers
{
    [Authorize]
    public class ClientController : Controller 
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Client
        
        public ActionResult Index(string buscadorCliente)
        {
            //IEnumerable<Client> clients;
            if (!String.IsNullOrEmpty(buscadorCliente))
            {
                //    clients = db.Clients.Where(client =>  client.firstName == buscadorCliente);

                var result = from c in db.Clients
                             where
                                 c.legalName.Contains(buscadorCliente) || 
                                 c.secondLegalName.Contains(buscadorCliente) 
                             select c;
                return View(result.ToList());
            }
            else
            {
                return View(db.Clients.ToList());
            }
            //return result.ToList();
            //else 
            //    clients = db.Clients.Include(a => a.)
            
        }

        // GET: Client/Details/5
        [Authorize(Roles = AccountRolesNames.ADMINISTRATOR+","+AccountRolesNames.CONTRACT_MANAGER+","+
            AccountRolesNames.PAYMENTS_RESERVATIONS + "," + AccountRolesNames.CEO + "," + AccountRolesNames.VLO + "," + AccountRolesNames.OWNER_SERVICES)]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = db.Clients.Find(id);
            if (client == null)
            { 
                return HttpNotFound();
            }
            //SalesContract salesContract = db.SalesContracts.Find(id);
            //if (salesContract == null)
            //{
            //    return HttpNotFound();
            //}
            //var tuple = new Tuple<Client, SalesContract>(client, new SalesContract());

            return View(client);
        }

        // GET: Client/Create
        [Authorize(Roles = AccountRolesNames.ADMINISTRATOR+","+AccountRolesNames.CONTRACT_OFFICER)]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Client/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, fo
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //Esto irve para    que se modifiquen solo estos campos [Bind(Include = "clientID,firstName,middleName,lastName,legalName,emailAddress,primaryPhoneNumber,primaryResidenceAddress,secondFirstName,secondMiddleName,secondLastName,secondLegalName,secondEmailAddress,secondPhoneNumber,city,state,zipCode,contractNumber,verifiedByAdmin")]
        [Authorize(Roles = AccountRolesNames.ADMINISTRATOR + "," + AccountRolesNames.CONTRACT_OFFICER)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( Client client)
        {
            client.legalName = client.firstName + " " + client.middleName + " " + client.lastName;
            client.secondLegalName = client.secondFirstName + " " + client.secondMiddleName + " " + client.secondLastName;
            try
            {
                DateTime dateNow = DateTime.Now;
                int ClientID = int.Parse(db.Clients.OrderByDescending(p => p.clientID).Select(r => r.clientID).First().ToString());
                string contractType = Request["contractType"];

                //client.contractNumber = contractType + ClientID + dateNow.ToString("dd/MM/yyyy").Replace("/", "");
            }
            catch { }
            if (ModelState.IsValid)
            {
                db.Clients.Add(client);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(client);
        }
        // GET: Client/Edit/5
        [Authorize(Roles = AccountRolesNames.ADMINISTRATOR + "," + AccountRolesNames.CONTRACT_OFFICER+","
            +AccountRolesNames.CONTRACT_MANAGER)]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = db.Clients.Find(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }

        // POST: Client/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[Authorize(Roles = "Administrador")] 
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AccountRolesNames.ADMINISTRATOR + "," + AccountRolesNames.CONTRACT_OFFICER + ","
            + AccountRolesNames.CONTRACT_MANAGER)]
        public ActionResult Edit( Client client)
        {
            client.legalName = client.firstName + " " + client.middleName + " " + client.lastName;
            client.secondLegalName = client.secondFirstName + " " + client.secondMiddleName + " " + client.secondLastName;
            if (ModelState.IsValid)
            {
                db.Entry(client).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(client);
        }

        //[Authorize(Roles = "Administrador")] 
        // GET: Client/Delete/5
        [Authorize(Roles = AccountRolesNames.ADMINISTRATOR)]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = db.Clients.Find(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }

        // POST: Client/Delete/5
        [Authorize(Roles = AccountRolesNames.ADMINISTRATOR)] 
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Client client = db.Clients.Find(id);
            db.Clients.Remove(client);
            db.SaveChanges();
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
    }
}

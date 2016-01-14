using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using sfdb.Models;
using Microsoft.WindowsAzure.MobileServices;

namespace sfdb.Controllers
{
    public class ShopItemsController : Controller
    {
        private sfdbContext db = new sfdbContext();

        // GET: ShopItems
        public async Task<ActionResult> Index()
        {
            //return View(await db.ShopItems.ToListAsync());
            var itemlist = await MobileService.GetTable<ShopItems>().ToListAsync();
            return View(itemlist);
        }

        //private ContactContext db = new ContactContext();
        private static MobileServiceClient MobileService = new MobileServiceClient(
            "https://superfashiondb.azure-mobile.net/",
            "nPtBYuImfnLpFPGnVXmuULvTFdkqNb31"
        );

        // GET: ShopItems/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShopItems shopItem = await db.ShopItems.FindAsync(id);
            if (shopItem == null)
            {
                return HttpNotFound();
            }
            return View(shopItem);
        }

        // GET: ShopItems/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ShopItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id,name,description,price,url,imgUrl,shopName,imageUrl,gender,color,type,filename")] ShopItems shopItem)
        {
            if (ModelState.IsValid)
            {
                db.ShopItems.Add(shopItem);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(shopItem);
        }

        // GET: ShopItems/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShopItems shopItem = await db.ShopItems.FindAsync(id);
            if (shopItem == null)
            {
                return HttpNotFound();
            }
            return View(shopItem);
        }

        // POST: ShopItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,name,description,price,url,imgUrl,shopName,imageUrl,gender,color,type,filename")] ShopItems shopItem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(shopItem).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(shopItem);
        }

        // GET: ShopItems/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShopItems shopItem = await db.ShopItems.FindAsync(id);
            if (shopItem == null)
            {
                return HttpNotFound();
            }
            return View(shopItem);
        }

        // POST: ShopItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ShopItems shopItem = await db.ShopItems.FindAsync(id);
            db.ShopItems.Remove(shopItem);
            await db.SaveChangesAsync();
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

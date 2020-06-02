using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoList.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Security.Claims;

namespace ToDoList.Controllers
{
  [Authorize]
  public class CategoriesController : Controller
  {
    private readonly ToDoListContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public CategoriesController(UserManager<ApplicationUser> userManager, ToDoListContext db)
    {
      _userManager = userManager;
      _db = db;
    }

    public async Task<ActionResult> Index()
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      var userCategories = _db.Categories.Where(entry => entry.User.Id == currentUser.Id);
      return View(userCategories);
    }

    public ActionResult Create()
    {
      return View();
    }

    [HttpPost]
    public async Task<ActionResult> Create(Category category)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      category.User = currentUser;
      _db.Categories.Add(category);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult Details(int id)
    {
      var thisCategory = _db.Categories
        .Include(category => category.Items)
        .ThenInclude(join => join.Item)
        .FirstOrDefault(category => category.CategoryId == id);
      return View(thisCategory);
    }

    public ActionResult Edit(int id)
    {
      var thisCategory = _db.Categories.FirstOrDefault(categories => categories.CategoryId == id);
      return View(thisCategory);
    }

    [HttpPost]
    public ActionResult Edit(Category category)
    {
      _db.Entry(category).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    [HttpGet]
    public ActionResult Delete(int id)
    {
      var thisCategory = _db.Categories.FirstOrDefault(categories => categories.CategoryId == id);
      return View(thisCategory);
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
      var thisCategory = _db.Categories.FirstOrDefault(categories => categories.CategoryId == id);
      _db.Categories.Remove(thisCategory);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
  }
}
using ImageShareHw.Web.Models;
using ImageShareHW.Data;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace ImageShareHw.Web.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=Homework; Integrated Security=true;";
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Upload(IFormFile imageFile, string password)
        {

            var fileName = $"{Guid.NewGuid()}-{imageFile.FileName}";
            var fullImagePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", fileName);
            using FileStream fs = new FileStream(fullImagePath, FileMode.Create);
            imageFile.CopyTo(fs);
            ImageRepository repo = new ImageRepository(_connectionString);

            int id = repo.AddImage(new()
            {
                Img = fileName,
                Password = password
            });

            return View(new ImageViewModel
            {
                Id = id,
                Password = password

            });

        }


        
        public IActionResult ViewImage(string password, int id)
        {
            var viewed = HttpContext.Session.Get<List<int>>("Ids");
            if (viewed == null)
            {
                viewed = new List<int>();
            }
            ImageRepository repo = new(_connectionString);
            var current = repo.GetImageById(id);
            var vm = new ViewImageViewModel();
            if (password == null)
            {
                vm.Image = new() { Id = id };
                return View(vm);
            }
            if (password == current.Password)
            {
                viewed.Add(id);
                HttpContext.Session.Set("Ids", viewed);
            }
            if (password != current.Password)
            {
                vm.InvalidMessage = "Incorrect Password Entered!!!";
            }
            if (viewed.Contains(id))
            {
                vm.Image = current;
                vm.Unlocked = true;
                repo.UpdateImageViewCount(id, current.Views + 1);
            }
            else
            {
                vm.Image = new() { Id = id };
            }

            return View(vm);
        }


    }
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            string value = session.GetString(key);

            return value == null ? default(T) :
                JsonSerializer.Deserialize<T>(value);
        }
    }
}
using HRProject_NTier.CORE.Entities;
using HRProject_NTier.CORE.ViewModels;
using HRProject_NTier.DATAACCESS.Context;
using HRProject_NTier.DATAACCESS.Repositories.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRProject_NTier.WEBUI.Areas.PersonnelArea.Controllers
{
    [Area("PersonnelArea")]
    public class HomeController : Controller
    {
        private IPersonnelRepository _personnelRepository;
        private ProjectContext _context;
        public HomeController(IPersonnelRepository personnelRepository,ProjectContext context)
        {
            _personnelRepository = personnelRepository;
            _context = context;
        }

        public IActionResult Index(Personnel personnel,int id)
        {
            id = Convert.ToInt32(HttpContext.Session.GetInt32("id"));
            personnel = _personnelRepository.Personnels.FirstOrDefault(x=>x.ID == id);
            return View(personnel);
        }

      
        public IActionResult GetCalendarEvents()
        {
            var events = _context.Calenders.ToList();
            return new JsonResult(events);
        }


    }
}

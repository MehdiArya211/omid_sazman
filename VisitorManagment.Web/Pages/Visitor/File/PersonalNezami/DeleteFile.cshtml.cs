using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.Services.Interfaces;

namespace VisitorManagment.Web.Pages.Visitor.File.PersonalNezami
{
    [Authorize]
    public class DeleteFileModel : PageModel
    {
        private readonly IFileService _fileService;
        public DeleteFileModel(IFileService fileService)
        {
            _fileService = fileService;
        }
        [BindProperty]
        public DeleteFactPersonalViewModel deleteFactPersonalViewModel { get; set; }
        public void OnGet(int id)
        {
            deleteFactPersonalViewModel = _fileService.GetFileInformation(id);
        }
        public IActionResult OnPost(int Id)
        {
            _fileService.DeleteFile(Id);

            return RedirectToPage("/Visitor/File/PersonalNezami/ListFile");
        }
    }
}

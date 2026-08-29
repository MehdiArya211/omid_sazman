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
            if (Id <= 0)
            {
                TempData["OperationTitle"] = "خطا در حذف";
                TempData["OperationMessage"] = "شناسه درخواست ملاقات معتبر نیست.";
                TempData["OperationIcon"] = "error";
                return RedirectToPage("/Visitor/File/PersonalNezami/ListFile");
            }

            _fileService.DeleteFile(Id);
            TempData["OperationTitle"] = "حذف موفق";
            TempData["OperationMessage"] = "درخواست ملاقات با موفقیت حذف شد.";
            TempData["OperationIcon"] = "success";
            return RedirectToPage("/Visitor/File/PersonalNezami/ListFile");
        }
    }
}

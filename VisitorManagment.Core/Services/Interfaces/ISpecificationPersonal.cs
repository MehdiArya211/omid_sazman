using System;
using System.Collections.Generic;
using System.Text;
using VisitorManagment.Core.DTOs;

namespace VisitorManagment.Core.Services.Interfaces
{
  public  interface ISpecificationPersonal
    {
        FactSpecificationPersonalViewModel GetPersonalByPersonalNo(int fileId);
        ListFileViewModel GetFile(int FileId);
        List<TashvighatInfoViewModel> GetTashvighatByPersonalNo(int fileId);
        List<TanbihatInfoViewModel> GetTanbihatByPersonalNo(int fileId);
    }
}

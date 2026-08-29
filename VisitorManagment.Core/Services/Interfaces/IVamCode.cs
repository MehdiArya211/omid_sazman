using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisitorManagment.DataLayer.Entities.VisitorManagment;

namespace VisitorManagment.Core.Services
{
    public interface IVamCodeService
    {
        VamCode GetVamCodeWithVamId(int vamId);
    }
}

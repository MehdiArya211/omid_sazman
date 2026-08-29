using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisitorManagment.DataLayer.Context;
using VisitorManagment.DataLayer.Entities.VisitorManagment;

namespace VisitorManagment.Core.Services
{
    public class VamCodeService : IVamCodeService
    {
        private readonly VisitorManagmentContext _context;
        public VamCodeService(VisitorManagmentContext context)
        {
            _context = context;
        }
        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public VamCode GetVamCodeWithVamId(int vamId)
        {
            var vamCode = _context.VamCodes.Where(x => x.Id == vamId).FirstOrDefault();

            return vamCode;
        }
    }
}

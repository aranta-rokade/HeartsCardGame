using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hearts.DAL
{
    public class AudirTrailDAL
    {
        public void Add(AuditTrail audit)
        {
            using (var db = new HeartsEntities())
            {
                db.AuditTrails.Add(audit);
                db.SaveChanges();
            }
        }
    }
}

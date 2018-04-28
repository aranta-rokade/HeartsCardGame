using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hearts.ViewModel
{
    public class AuditTrailModel
    {
        public int AuditTrailId { get; set; }
        public string UserId { get; set; }
        public string SessionId { get; set; }
        public string IPAddress { get; set; }
        public string PageAccessed { get; set; }
        public DateTime? LoggedInAt { get; set; }
        public DateTime? LoggedInOut { get; set; }
        public string LoginStatus { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
    }
}

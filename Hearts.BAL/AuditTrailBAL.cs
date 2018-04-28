using Hearts.DAL;
using Hearts.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace Hearts.BAL
{
    public class AuditTrailBAL
    {
        public void AddAuditTrail(AuditTrailModel auditModel) {
            AudirTrailDAL a_dal = new AudirTrailDAL();
            AuditTrail audit = new AuditTrail
            {
                ActionName = auditModel.ActionName,
                ControllerName = auditModel.ControllerName,
                IPAddress = auditModel.IPAddress,
                LoggedInAt = auditModel.LoggedInAt,
                LoggedInOut = auditModel.LoggedInOut,
                LoginStatus = auditModel.LoginStatus,
                PageAccessed = auditModel.PageAccessed,
                SessionId = auditModel.SessionId,
            };

            if (auditModel.UserId != null)
                audit.UserId = Convert.ToInt32(new Hashing().Decrypt(auditModel.UserId));
            a_dal.Add(audit);
        }
    }
}

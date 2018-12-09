using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace PD.Models
{
    public class AuditRecord
    {
        public enum eAuditRecordType { Info, Warning, Error }

        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string Message { get; set; }
        public eAuditRecordType AuditType { get; set; }

        public AuditRecord()
        {

        }
        public AuditRecord(eAuditRecordType type)
        {
            AuditType = type;
        }

        public bool TrackChange(string userId, Object oldObject, Object newObject)
        {
            Type t = newObject.GetType();
            PropertyInfo[] props = t.GetProperties();
            List<string> changes = new List<string>();
            
            foreach (PropertyInfo prp in props)
            {
                Object oldVal = prp.GetValue(oldObject);
                Object newVal = prp.GetValue(newObject);

                if (oldVal != newVal)
                    changes.Add(string.Format("{0}: {1} => {2}", prp.Name, oldVal, newVal));
            }
            Message += string.Join("<br />", changes);
            return changes.Count > 0;
        }

    }
}

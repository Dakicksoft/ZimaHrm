using System;

namespace ZimaHrm.Core.DataModel.Base
{
    public class BaseModel
    {
        public Guid Id { get; set; }
        public string CreatedBy { get; set; }

        public DateTime CreatedUtc { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime LastModifiedUtc { get; set; }
        public bool IsDelete { get; set; }
    }
}

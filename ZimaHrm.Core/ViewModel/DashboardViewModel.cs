using ZimaHrm.Core.DataModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ZimaHrm.Core.ViewModel
{
    public class DashboardViewModel
    {
        public DashboardViewModel()
        {
            Notices = new List<NoticeModel>();
            Holidays = new List<HolidayModel>();
        }
        public int TotalEmployee { get; set; }
        public int TotalDept { get; set; }
        public int PresentCountToday { get; set; }
        public int AbsenceCountToday { get; set; }

        public IList<NoticeModel> Notices { get; set; }
        public IList<HolidayModel> Holidays { get; set; }

    }
}

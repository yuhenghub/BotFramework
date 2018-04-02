using DirectLineSDK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectLineSDK.Models
{
    /// <summary>
    /// 信息设置
    /// </summary>
    public class ActivitiesSet
    {
        public Activities[] activities { get; set; }

        public string watermark { get; set; }
    }
}

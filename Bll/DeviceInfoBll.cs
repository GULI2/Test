using DAL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bll
{
    public partial class DeviceInfoBll : BaseBll<T_Device_IP_Information>//继承父类
    {
        public override BaseDal<T_Device_IP_Information> GetDal()
        {
            return new DeviceInfoDAL();
        }

       
    }
}

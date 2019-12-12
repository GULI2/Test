using DAL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bll
{
    public partial class HistoryInfoBll:BaseBll<T_Device_IP_History_Information>
    {
        //子类中重写父类方法得到对象
        public override BaseDal<T_Device_IP_History_Information> GetDal()
        {
            return new HistoryInfoDAL();
        }
    }
}

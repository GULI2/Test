using Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public partial class DeviceInfoDAL:BaseDal<T_Device_IP_Information>//继承于BaseDal,类型变化使用泛型。完成了代码的重用
    {
        //父类BaseDal里调用的两个抽象方法
        //父类中不知道具体的属性列是神马，只有到了子类中才知道。所以使用抽象方法。
        public override Expression<Func<T_Device_IP_Information, string>> GetKey()
        {
            return u => u.device_coding;
        }
        public override Expression<Func<T_Device_IP_Information, DateTime?>> GetPingTime()
        {
            return u => u.ping_time;
        }
        public override Expression<Func<T_Device_IP_Information, bool>> GetByCodeKey(string code)
        {
            return u => u.device_coding == code;
        }

        public override Expression<Func<T_Device_IP_Information, bool>> GetByCodeKey2(string code)
        {
            if(!String.IsNullOrEmpty(code))
            {
                return u => u.device_coding.Contains(code);
            }
            else
            {
                return u => 1 == 1;
            }
        }

        public override Expression<Func<T_Device_IP_Information, bool>> GetByPingTag(string pingTag)
        {
            if (String.IsNullOrEmpty(pingTag)||pingTag.Equals("all"))
            {
                return u => 1 == 1;
            }
            else
            {
                return u => u.ping_tag == pingTag;
            }
        }

        public override Expression<Func<T_Device_IP_Information, bool>> GetByIP(string IP)
        {
            if (!String.IsNullOrEmpty(IP))
            {
                return u => u.ip.Contains(IP);
            }
            else
            {
                return u => 1 == 1;
            }

        }
    }
}

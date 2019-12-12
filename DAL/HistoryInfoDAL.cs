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
    //public partial class 在其他地方也可以定义相同的类 
    //public class 在其他地方不可以定义相同的类
    public partial class HistoryInfoDAL:BaseDal<T_Device_IP_History_Information>
    {
        //要扩展或修改继承的方法、属性、索引器或事件的抽象实现或虚实现，必须使用 override 修饰符。
        public override Expression<Func<T_Device_IP_History_Information, string>> GetKey()
        {
            return u => u.device_coding;
        }

        public override Expression<Func<T_Device_IP_History_Information, DateTime?>> GetPingTime()
        {
            return u => u.ping_time;
        }

        

        public override Expression<Func<T_Device_IP_History_Information, bool>> GetByCodeKey(string code)
        {
            return u => u.device_coding == code;
        }

        public override Expression<Func<T_Device_IP_History_Information, bool>> GetByCodeKey2(string code)
        {
            return u => u.device_coding.Contains(code);
        }

        public override Expression<Func<T_Device_IP_History_Information, bool>> GetByPingTag(string pingTag)
        {
            return u => u.ping_tag == pingTag;
        }

        public override Expression<Func<T_Device_IP_History_Information, bool>> GetByIP(string IP)
        {
            return u => u.ping_tag.Contains(IP);
        }
 
    }
}

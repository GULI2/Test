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
    public abstract partial class BaseDal<T>
        where T:class//约束
    {
        //查询需要上下文对象  MyContent是appConfig中与数据库的连接
        DbContext dbContext = new MyContext();

        //查询返回一个集合。分页需要当前页和页大小。  查询多条
        public IQueryable<T> GetList(int pageSize, int pageIndex)
        {
            return dbContext.Set<T>()
                //不同的类 ，属性不一定是id。通过抽象方法GetKey()获取
                 //.OrderBy(u => u.id)
                 .OrderBy(GetKey())
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize);
        }

        //验证数据是否已存在
        public bool CheckDeviceExists(string code)
        {
            bool exists = false;
            List<T_Device_IP_Information> list1 = dbContext.Set<T_Device_IP_Information>().Where(u => u.device_coding == code).ToList();
            if (list1.Count > 0)
            {
                exists = true;
            }
            return exists;
        }

        //根据条件查询单条
        public T GetByDeviceCode(string code)
        {
            return dbContext.Set<T>()
                //where后面值只能是true或false.即条件要么被满足，要么不被满足
                //.Where(u => u.device_coding == code)
                .Where(GetByCodeKey(code))
                .FirstOrDefault();
        }

        //根据主机名查询历史记录
        public IQueryable<T> GetHistoryByDeviceCode(string code)
        {
            return dbContext.Set<T>()
                //where后面值只能是true或false.即条件要么被满足，要么不被满足
                //.Where(u => u.device_coding == code)
                .Where(GetByCodeKey(code)).OrderByDescending(GetPingTime());
        }

        public IQueryable<T> GetByCode(string code, string pingTag, string IP)
        {
            return dbContext.Set<T>()
                //where后面值只能是true或false.即条件要么被满足，要么不被满足
                //.Where(u => u.device_coding == code)
                .OrderBy(GetKey())
                .Where(GetByCodeKey2(code))
                .Where(GetByPingTag(pingTag))
                .Where(GetByIP(IP));
        }

        //新增
        public int Add(T DevInfo)
        {
            dbContext.Set<T>().Add(DevInfo);
            return dbContext.SaveChanges();
        }

        //修改
        public int Edit(T DevInfo)
        {
            dbContext.Set<T>().Attach(DevInfo);
            dbContext.Entry(DevInfo).State = EntityState.Modified;
            return dbContext.SaveChanges();
        }

        //ping主机IP时修改ping_tag和ping_time
        public int EditPing(T DevInfo)
        {
            dbContext.Set<T>().Attach(DevInfo);
            dbContext.Entry(DevInfo).State = EntityState.Modified;
            //dbContext.Entry(DevInfo).Property("device_coding").IsModified = false;
            dbContext.Entry(DevInfo).Property("device_name").IsModified = false;
            dbContext.Entry(DevInfo).Property("ip").IsModified = false;
            dbContext.Entry(DevInfo).Property("id").IsModified = false;
            return dbContext.SaveChanges();
        }

        //删除
        public int Remove(string code)
        {
            var DevInfo = GetByDeviceCode(code);//根据主机名获取对象
            dbContext.Set<T>().Remove(DevInfo);//删除对象
            return dbContext.SaveChanges();
        }

        //返回类的属性
        public abstract Expression<Func<T, string>> GetKey();

        public abstract Expression<Func<T, bool>> GetByCodeKey(string code);
        public abstract Expression<Func<T, bool>> GetByCodeKey2(string code);
        public abstract Expression<Func<T, bool>> GetByPingTag(string pingTag);
        public abstract Expression<Func<T, bool>> GetByIP(string IP);
        public abstract Expression<Func<T, DateTime?>> GetPingTime();

        public int getCount()
        {
            return dbContext.Set<T>().Count();//集合行数
        }
    }
}

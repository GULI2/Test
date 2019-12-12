using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bll
{
    public abstract partial class BaseBll<T>
        where T:class//这里的class表示T是个引用类型
    {
        private BaseDal<T> dal;
        public abstract BaseDal<T> GetDal();//抽象方法获取子类对象。具体在子类中获取。
        //构造方法
        public BaseBll()
        {
            dal = GetDal();
        }

        public IQueryable<T> GetList(int pageSize, int pageIndex)
        {
            return dal.GetList(pageSize, pageIndex);
        }

        public T GetByDeviceCode(string code)
        {
            return dal.GetByDeviceCode(code);
        }

        public IQueryable<T> GetByCode(string code, string pingTag, string IP)
        {
            return dal.GetByCode(code, pingTag,IP);
        }

        public int Add(T DevInfo)
        {
            return dal.Add(DevInfo);
        }

        //修改
        public int Edit(T DevInfo)
        {
            return dal.Edit(DevInfo);
        }

        //ping主机IP时修改ping_tag和ping_time
        public int EditPing(T DevInfo)
        {
            return dal.EditPing(DevInfo);
        }

        //删除
        public int Remove(string code)
        {
            return dal.Remove(code);
        }

        public int GetCount()
        {
            return dal.getCount();
        }

        public bool CheckDeviceExists(string code)
        {
            return dal.CheckDeviceExists(code);
        }

        public IQueryable<T> GetHistoryByDeviceCode(string code)
        {
            return dal.GetHistoryByDeviceCode(code);
        }

    }
}

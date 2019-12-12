using Bll;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Timers;
using System.Web;
//表现层（UI）、业务逻辑层（BLL）、数据访问层（DAL）再加上实体类库（Model）
namespace UI.Controllers
{
    public class DeviceInfoController : Controller
    {
        // GET: DeviceInfo
        DeviceInfoBll deviceInfoBll = new DeviceInfoBll();
        HistoryInfoBll historyBll = new HistoryInfoBll();
        public ActionResult layuiTest()
        {
            return View();
        }
 
        #region 查询
        public ActionResult Index(string sortOrder)
        {
            
                ViewBag.ipSort = (!String.IsNullOrEmpty(sortOrder)&& sortOrder.Equals("ip") )? "ip_desc" : "ip";
                ViewBag.CodeSort =(!String.IsNullOrEmpty(sortOrder) && sortOrder.Equals("device_coding")) ? "device_coding_desc" : "device_coding";
                ViewBag.pingTagSort = (!String.IsNullOrEmpty(sortOrder) && sortOrder.Equals("ping_tag")) ? "ping_tag_desc" : "ping_tag";


            //action向view传值的方法ViewData
            var device = deviceInfoBll.GetList(100, 1).OrderByDescending(u=>u.id);
            switch (sortOrder)
            {
                case "ip_desc":
                    device = device.OrderByDescending(s => s.ip);
                    break;
                case "ip":
                    device = device.OrderBy(s => s.ip);
                    break;
                case "device_coding_desc":
                    device = device.OrderByDescending(s => s.device_coding);
                    break;
                case "device_coding":
                    device = device.OrderBy(s => s.device_coding);
                    break;
                case "ping_tag_desc":
                    device = device.OrderByDescending(s => s.ping_tag);
                    break;
                case "ping_tag":
                    device = device.OrderBy(s => s.ping_tag);
                    break;
                default:
                    device = device.OrderBy(s => s.id);
                    break;
            }
            ViewData.Model = device;
            //ViewData.Model = deviceInfoBll.GetList(100, 1).OrderByDescending(u => u.id);
            return View();
        }

        public ActionResult Search(string code,string pingTag,string IP)
        {
            ViewData.Model = deviceInfoBll.GetByCode(code, pingTag,IP);
            return View("Index");
        }

        //异步分批取数
        public ActionResult LoadList(int pageSize, int pageIndex)
        {
            var list = deviceInfoBll.GetList(pageSize, pageIndex)
                .Select(u => new {
                    deviceCode = u.device_coding,
                    ip = u.ip,
                    deviceName = u.device_name,
                    pingTag = u.ping_tag,
                    pingTime = u.ping_time
                }
                )
                .ToList();
            int rowCount = deviceInfoBll.GetCount();//获取总行数
            int pageCount = Convert.ToInt32(Math.Ceiling(rowCount * 1.0 / pageSize));//总页数
            StringBuilder pager = new StringBuilder();
            if (pageIndex == 1)
            {
                pager.Append("首页 上一页");
            }
            else
            {
                pager.Append("<a href = 'javascript:GoPage(1)'>首页</a>&nbsp;" +
                    "<a href='javascript:GoPage(" + (pageIndex - 1) + ")'>上一页</a>");
            }
            if (pageCount == pageIndex)
            {
                pager.Append("下一页 尾页");
            }
            else
            {
                pager.Append("&nbsp;<a href='javascript:GoPage(" + (pageIndex + 1) + ")'>下一页</a>&nbsp;" +
                    "<a href = 'javascript:GoPage(" + pageCount + ")'>尾页</a>");
            }

            var temp = new
            {
                list = list,
                pager = pager.ToString()
            };//将list 和分页字符串一起传过去
            return Json(temp, JsonRequestBehavior.AllowGet);//返回json 对象
        }

        //查询历史明细
        public ActionResult SearchHistory(string deviceCode)
        {
            ViewData.Model = historyBll.GetHistoryByDeviceCode(deviceCode);
            return View();
        }
        #endregion

        #region 新增
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(T_Device_IP_Information deviceInfo)
        {
            //if (!CheckIp(deviceInfo.ip))
            //{
            //    return Content("<script>alert('存在非法IP,请输入正确的IP格式！');location.href='"+Url.Action("Add","DeviceInfo")+"';</script>");
            //}
            if (CheckDeviceExists(deviceInfo.ip))
            {
                return Content("<script>alert('已存在该主机名！');location.href='" + Url.Action("Add", "DeviceInfo") + "';</script>");
            }

            deviceInfoBll.Add(deviceInfo);
            //Controller的Redirect(string url)方法:创建一个重定向到指定的 URL 的 RedirectResult 对象。
            //创建一个请求Action的链接最好的方法是使用Url.Action,匹配你所定义的路由的过程
            return Redirect(Url.Action("Index"));
        }
        #endregion

        #region 修改
        public ActionResult Edit(string deviceCode)
        {     
            ViewData.Model = deviceInfoBll.GetByDeviceCode(deviceCode);//根据deviceCode获取到要修改的对象
            return View();
        }

        [HttpPost]
        public ActionResult Edit2(T_Device_IP_Information deviceInfo)
        {
            try
            {
                //if (!CheckIp(deviceInfo.ip))
                //{
                //    return Content("<script>alert('存在非法IP,请输入正确的IP格式！');location.href='" + Url.Action("Edit", "DeviceInfo") + "';</script>");
                //}
                deviceInfoBll.Edit(deviceInfo);
                //Controller的Redirect(string url)方法:创建一个重定向到指定的 URL 的 RedirectResult 对象。
                //创建一个请求Action的链接最好的方法是使用Url.Action,匹配你所定义的路由的过程
                return Redirect(Url.Action("Index"));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 删除
        public ActionResult Remove(string deviceCode)
        {
            deviceInfoBll.Remove(deviceCode);
            historyBll.Remove(deviceCode);
            return Redirect(Url.Action("Index"));
        }
        #endregion

        #region 循环ping主机的Ip  
        public ActionResult pingIP()
        {
            Ping ping = new Ping();
            List<T_Device_IP_Information> list = deviceInfoBll.GetList(100, 1).OrderByDescending(u => u.id).ToList();
            if (HttpRuntime.Cache["test"]!=null)
            {

            };
            foreach (var item in list)
            {
                int len = item.ip.Trim().Length;
                if (IPStatus.Success == ping.Send(item.ip.Trim()).Status)
                {
                    item.ping_tag = "T";
                }
                else
                {
                    item.ping_tag = "F";
                }
                item.ping_time = DateTime.Now;
                deviceInfoBll.EditPing(item);
                //添加历史记录
                T_Device_IP_History_Information dh =new T_Device_IP_History_Information()
                {
                    device_coding=item.device_coding,
                    ip = item.ip,
                    device_name = item.device_name,
                    ping_tag = item.ping_tag,
                    ping_time =Convert.ToDateTime(item.ping_time) 
                };
               
                historyBll.Add(dh);
            }
            return Redirect(Url.Action("Index"));
        }
        #endregion

        #region 数据验证
        //检查数据是否已存在
        public bool CheckDeviceExists(string code)
        {
            return deviceInfoBll.CheckDeviceExists(code);
        }

        //检查IP格式
        public bool CheckIp(string ip)
        {
            Regex rx = new Regex(@"((?:(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d)))\.){3}(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d))))");
            if (!rx.IsMatch(ip))
            {
                return false;
            }
            return true;
        }
        #endregion
    }
}
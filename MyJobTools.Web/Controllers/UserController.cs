using MyJobTools.Bll;
using MyJobTools.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyJobTools.Web.EF;
using MyJobTools.Library.Extension;

namespace MyJobTools.Web.Controllers
{
    public class UserController : Controller
    {
        public ActionResult List(UserQueryModel queryModel, PageQueryModel pageModel)
        {

            //var list = new List<UserModel>();
            //list.Add(new UserModel { UserCode = "111", UserName = "1111" });
            //list.Add(new UserModel { UserCode = "222", UserName = "2222" });
            //list.Add(new UserModel { UserCode = "333", UserName = "3333" });
            //list.Add(new UserModel { UserCode = "444", UserName = "4444" });
            //list.Add(new UserModel { UserCode = "555", UserName = "5555" });
            //list.Add(new UserModel { UserCode = "66", UserName = "6666" });
            //UserBll dao = new UserBll();
            //var resultlist = dao.GetPage(queryModel, pageModel);
            //if (resultlist == null || resultlist.Count == 0)
            //{
            //    foreach (var item in list)
            //    {
            //        dao.Insert(item);
            //    }
            //    resultlist = list;
            //}
            MyDBContext content = new MyDBContext();
            List<User> resultlist;
            if (queryModel.UserCode.IsNullOrWhiteSpace())
            {
                //resultlist= content.Users.Where(m => m.UserCode)
            }
            resultlist = content.Users.Where(m => queryModel.UserCode.IsNullOrWhiteSpace() || m.UserCode.Contains(queryModel.UserCode))
                .Where(m => queryModel.UserName_Like.IsNullOrWhiteSpace() || m.UserName.Contains(queryModel.UserName_Like))
                .Skip(pageModel.GetSkip()).Take(pageModel.limit ?? 20)
                .ToList();
            ViewBag.list = resultlist;

            return View();
        }
    }
}

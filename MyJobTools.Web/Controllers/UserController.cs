using MyJobTools.Dao;
using MyJobTools.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyJobTools.Web.Controllers
{
    public class UserController : Controller
    {
        public ActionResult List(UserQueryModel queryModel, PageQueryModel pageModel)
        {

            var list = new List<UserModel>();
            list.Add(new UserModel { UserCode = "111", UserName = "1111" });
            list.Add(new UserModel { UserCode = "222", UserName = "2222" });
            list.Add(new UserModel { UserCode = "333", UserName = "3333" });
            list.Add(new UserModel { UserCode = "444", UserName = "4444" });
            list.Add(new UserModel { UserCode = "555", UserName = "5555" });
            list.Add(new UserModel { UserCode = "66", UserName = "6666" });
            UserDao dao = new UserDao();
            var resultlist = dao.GetPage(queryModel, pageModel);
            if (resultlist == null || resultlist.Count == 0)
            {
                foreach (var item in list)
                {
                    dao.Insert(item);
                }
                resultlist = list;
            }
            ViewBag.list = resultlist;

            return View();
        }
    }
}

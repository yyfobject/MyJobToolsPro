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
            
            List<User> resultlist;
            if (queryModel.UserCode.IsNullOrWhiteSpace())
            {
                //resultlist= content.Users.Where(m => m.UserCode)
            }
            //resultlist = content.Users.Where(m => queryModel.UserCode.IsNullOrWhiteSpace() || m.UserCode.Contains(queryModel.UserCode))
            //    .Where(m => queryModel.UserName_Like.IsNullOrWhiteSpace() || m.UserName.Contains(queryModel.UserName_Like))
            //    .Skip(pageModel.GetSkip()).Take(pageModel.Limit ?? 20)
            //    .ToList();
            //Expression<List<User >,bool> whereFilter = (List<User> list) => { list.Where(m => m.UserCode.Contains(queryModel.UserCode)).ToList(); };
            using (MyDBContext content = new MyDBContext())
            {
                //resultlist = content.Users.GetPageQuery(
                //    (User item) => item.UserCode.Contains(queryModel.UserCode), //where
                //    (User item) => item.Id, //order by
                //    true, pageModel
                //    ).ToList();


                resultlist = content.Users.Where(m => (queryModel.UserCode == null || m.UserCode.Contains(queryModel.UserCode)) && (queryModel.UserName_Like == null || m.UserName.Contains(queryModel.UserName_Like))).OrderBy(m => m.UserName).Skip(pageModel.GetSkip()).Take(pageModel.GetLimit()).ToList();
            }
            ViewBag.list = resultlist;

            return View();
        }

        public ActionResult GetList(UserQueryModel queryModel, PageQueryModel pageModel)
        {

            UserBll uBll = new UserBll();
            var resultlist = uBll.GetPage(queryModel, pageModel);

            return Content(resultlist.ToJson());
        }
    }
}

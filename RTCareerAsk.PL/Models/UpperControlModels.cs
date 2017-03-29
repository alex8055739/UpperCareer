using System.Collections.Generic;
using System.Web.Mvc.Ajax;

namespace RTCareerAsk.PL.Models
{
    public class TabModel
    {
        public TabModel()
        {
            TabItems = new Dictionary<string, ActionLinkParams>();
            HtmlAttrs = new Dictionary<string, string>();
        }

        public Dictionary<string, ActionLinkParams> TabItems { get; set; }

        public string ActiveItem { get; set; }

        public Dictionary<string, string> HtmlAttrs { get; set; }

        public AjaxOptions AjaxOptns { get; set; }
    }

    public class ActionLinkParams
    {
        public string ActionName { get; set; }

        public string ControllerName { get; set; }

        public object RouteValues { get; set; }
    }
}
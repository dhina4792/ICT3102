using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StowagePlanAnalytics_ITP_2016.CustomFunction
{
    /************************************************************************************
     * Description: Web Optimization #5 - Configure Entity Tags (ETags)                 *
     *                                                                                  *
     ************************************************************************************/
    public class RemoveETTags : IHttpModule
    {
        private static readonly List<string> _headersToRemove = new List<string> { "X-AspNet-Version", "X-AspNetMvc-Version", "Etag", "Server", };

        public void Init(HttpApplication context)
        {
            context.EndRequest += new EventHandler(context_EndRequest);
        }

        private void context_EndRequest(object sender, EventArgs e)
        {
            var context = HttpContext.Current;
            _headersToRemove.ForEach(h => context.Response.Headers.Remove(h));
        }

        public void Dispose() { }
    }
}
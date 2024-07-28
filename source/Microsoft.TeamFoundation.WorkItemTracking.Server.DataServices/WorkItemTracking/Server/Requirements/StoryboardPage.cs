// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Requirements.StoryboardPage
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 929F0284-16B2-4277-9F4A-B615689A77D1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.dll

using Microsoft.TeamFoundation.Server.Core;
using System;
using System.Globalization;
using System.Web;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Requirements
{
  public class StoryboardPage : TeamFoundationPage
  {
    protected void Page_Load(object sender, EventArgs e)
    {
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      bool flag = false;
      string str1 = this.Request.Params["artifactMoniker"];
      Uri result;
      if (str1 != null && Uri.TryCreate(str1, UriKind.Absolute, out result))
      {
        if (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps)
        {
          this.Response.Write(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ResourceStrings.Get("StoryboardPage_RedirectingTo"), (object) result.AbsoluteUri));
          this.Response.Redirect(result.AbsoluteUri);
          flag = true;
        }
        else if (result.IsUnc)
        {
          string str2 = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "<a href='{0}'>{1}</a><br/><br/>", (object) HttpUtility.HtmlAttributeEncode(result.AbsoluteUri), (object) HttpUtility.HtmlEncode(str1));
          this.Response.Write(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ResourceStrings.Get("StoryboardPage_ClickHereToOpenLink"), (object) str2));
          flag = true;
        }
      }
      if (flag)
        return;
      this.Response.StatusCode = 400;
    }
  }
}

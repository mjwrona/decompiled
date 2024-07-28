// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.CustomRedirectResult
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using System;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public class CustomRedirectResult : ActionResult
  {
    private string m_redirectLocation;
    private HttpStatusCode m_statusCode;

    public CustomRedirectResult(string redirectAddress, HttpStatusCode statusCode)
    {
      this.m_redirectLocation = redirectAddress != null ? redirectAddress : throw new ArgumentNullException(nameof (redirectAddress));
      this.m_statusCode = statusCode;
    }

    public override void ExecuteResult(ControllerContext context)
    {
      context.HttpContext.Response.StatusCode = (int) this.m_statusCode;
      string str = this.m_redirectLocation;
      if (str.Length > 0 && str[0] == '~')
        str = VirtualPathUtility.ToAbsolute(str);
      this.DoRedirect(context, str);
      context.HttpContext.Response.End();
    }

    protected virtual void DoRedirect(ControllerContext context, string redirectLocation) => context.HttpContext.Response.RedirectLocation = redirectLocation;
  }
}

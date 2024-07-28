// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.StaticContentRewriteModule
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Configuration;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class StaticContentRewriteModule : IHttpModule
  {
    public void Init(HttpApplication context) => context.BeginRequest += new EventHandler(this.BeginRequest);

    public void Dispose()
    {
    }

    private void BeginRequest(object sender, EventArgs e)
    {
      HttpContext context = ((HttpApplication) sender).Context;
      if (context.Items.Contains((object) HttpContextConstants.ArrRequestRouted) || !(context.Items[(object) HttpContextConstants.StaticContentRewritePath] is string path))
        return;
      context.RewritePath(path);
      string appSetting = ConfigurationManager.AppSettings["AccessControlAllowOriginOverride"];
      if (appSetting == null)
        return;
      context.Response.Headers["Access-Control-Allow-Origin"] = appSetting;
    }
  }
}

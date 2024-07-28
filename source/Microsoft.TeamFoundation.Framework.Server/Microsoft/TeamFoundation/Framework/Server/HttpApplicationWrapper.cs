// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HttpApplicationWrapper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class HttpApplicationWrapper : IHttpApplication
  {
    private readonly HttpApplication _application;
    private HttpContextBase httpContextBase;

    public HttpApplicationWrapper(HttpApplication application) => this._application = application;

    public HttpContextBase Context
    {
      get
      {
        if (this.httpContextBase == null)
          this.httpContextBase = (HttpContextBase) new HttpContextWrapper(this._application.Context);
        return this.httpContextBase;
      }
    }

    public HttpRequest Request => this._application.Request;

    public HttpResponse Response => this._application.Response;

    public void AddExceptionHeadersToList(List<KeyValuePair<string, string>> headers, Exception ex)
    {
      if (!(this._application is VisualStudioServicesApplication application))
        return;
      application.AddExceptionHeadersToList(headers, ex);
    }

    public void CompleteRequest() => this._application.CompleteRequest();
  }
}

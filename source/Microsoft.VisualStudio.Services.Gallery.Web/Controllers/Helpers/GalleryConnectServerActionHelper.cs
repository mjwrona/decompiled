// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Web.Controllers.Helpers.GalleryConnectServerActionHelper
// Assembly: Microsoft.VisualStudio.Services.Gallery.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17D36576-2EF3-4ABC-94BA-AF7891D15A3A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Web.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CloudConnected;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;

namespace Microsoft.VisualStudio.Services.Gallery.Web.Controllers.Helpers
{
  public class GalleryConnectServerActionHelper : GalleryControllerHelper
  {
    public GalleryConnectServerActionHelper(GalleryController controller)
      : base(controller)
    {
    }

    public string SaveConnectedServerSettings(
      string accountName,
      string accountId,
      string spsUrlstr,
      string installAction,
      string authorizationUrlstr)
    {
      this.TfsRequestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(this.TfsRequestContext, FrameworkSecurity.FrameworkNamespaceId).CheckPermission(this.TfsRequestContext, FrameworkSecurity.FrameworkNamespaceToken, 2);
      this.TfsRequestContext.GetService<ICloudConnectedService>().ConfigureServer(this.TfsRequestContext, accountName, Guid.Parse(accountId), spsUrlstr, authorizationUrlstr);
      string token = this.TfsRequestContext.GetService<IConnectedServerContextKeyService>().GetToken(this.TfsRequestContext, (Dictionary<string, string>) null);
      UriBuilder uriBuilder = new UriBuilder(installAction);
      NameValueCollection queryString = HttpUtility.ParseQueryString(uriBuilder.Query);
      queryString["serverKey"] = token;
      uriBuilder.Query = queryString.ToString();
      installAction = uriBuilder.Uri.ToString();
      return installAction;
    }
  }
}

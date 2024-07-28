// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns.AuthorizationsRevokeDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System;
using System.Collections;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns
{
  public class AuthorizationsRevokeDataProvider : IExtensionDataProvider
  {
    public string Name => "Admin.RevokeAuthorizations";

    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      try
      {
        IDelegatedAuthorizationService service = requestContext.GetService<IDelegatedAuthorizationService>();
        Guid userId = requestContext.GetUserId(true);
        object source;
        providerContext.Properties.TryGetValue("authIds", out source);
        if (source != null)
        {
          foreach (string input in ((IEnumerable) source).Cast<object>().Select<object, string>((Func<object, string>) (x => x.ToString())).ToArray<string>())
            service.Revoke(requestContext, userId, Guid.Parse(input));
        }
        return (object) new HttpStatusCodeResult(HttpStatusCode.NoContent);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10050074, "ProfileSettings", "DataProvider", ex);
      }
      return (object) null;
    }
  }
}

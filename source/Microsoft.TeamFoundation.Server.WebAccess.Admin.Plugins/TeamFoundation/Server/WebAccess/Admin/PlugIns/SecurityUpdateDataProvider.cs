// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns.SecurityUpdateDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System;
using System.Diagnostics;
using System.Net;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns
{
  public class SecurityUpdateDataProvider : IExtensionDataProvider
  {
    public string Name => "Admin.SecurityViewUpdate";

    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      try
      {
        TfsWebContext webContext = (TfsWebContext) WebContextFactory.GetWebContext(requestContext);
        bool flag = false;
        if (providerContext.Properties.ContainsKey("changeInheritance") && providerContext.Properties["changeInheritance"] != null)
          flag = bool.Parse(providerContext.Properties["changeInheritance"].ToString());
        if (flag)
        {
          Guid permissionSetId;
          string permissionSetToken;
          bool inheritPermissions;
          SecurityUpdateDataProvider.GetSecurityChangeInheritanceParams(providerContext, out permissionSetId, out permissionSetToken, out inheritPermissions);
          ArgumentUtility.CheckStringForNullOrEmpty(permissionSetToken, "token");
          ArgumentUtility.CheckForEmptyGuid(permissionSetId, "permissionSet");
          SecurityNamespacePermissionsManager permissionsManager = SecurityViewDataProvider.CreatePermissionsManager(webContext, new Guid?(permissionSetId), permissionSetToken);
          if (permissionsManager.CanTokenInheritPermissions && permissionsManager.InheritPermissions != inheritPermissions)
          {
            permissionsManager.InheritPermissions = inheritPermissions;
            permissionsManager.ChangeInheritance(requestContext, inheritPermissions);
          }
        }
        return (object) new HttpStatusCodeResult(HttpStatusCode.NoContent);
      }
      catch (Exception ex)
      {
        requestContext.Trace(10050078, TraceLevel.Error, "SecurityView", "DataProvider", ex.Message);
        return (object) new HttpStatusCodeResult(HttpStatusCode.PartialContent, ex.Message);
      }
    }

    private static void GetSecurityChangeInheritanceParams(
      DataProviderContext providerContext,
      out Guid permissionSetId,
      out string permissionSetToken,
      out bool inheritPermissions)
    {
      permissionSetId = new Guid();
      permissionSetToken = (string) null;
      inheritPermissions = false;
      if (providerContext.Properties.ContainsKey(nameof (permissionSetId)) && providerContext.Properties[nameof (permissionSetId)] != null)
        Guid.TryParse(providerContext.Properties[nameof (permissionSetId)].ToString(), out permissionSetId);
      if (providerContext.Properties.ContainsKey(nameof (permissionSetToken)) && providerContext.Properties[nameof (permissionSetToken)] != null)
        permissionSetToken = providerContext.Properties[nameof (permissionSetToken)].ToString();
      if (!providerContext.Properties.ContainsKey(nameof (inheritPermissions)) || providerContext.Properties[nameof (inheritPermissions)] == null)
        return;
      inheritPermissions = bool.Parse(providerContext.Properties[nameof (inheritPermissions)].ToString());
    }
  }
}

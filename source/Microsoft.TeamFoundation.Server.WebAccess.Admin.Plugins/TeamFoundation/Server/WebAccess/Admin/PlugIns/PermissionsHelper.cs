// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns.PermissionsHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns
{
  public static class PermissionsHelper
  {
    public static Guid GetPermissionSetId(NavigationContext navigationContext)
    {
      if (navigationContext.Levels.HasFlag((Enum) NavigationContextLevels.Project))
        return NamespacePermissionSetConstants.ProjectLevel;
      return navigationContext.Levels.HasFlag((Enum) NavigationContextLevels.Collection) ? NamespacePermissionSetConstants.CollectionLevel : new Guid();
    }

    public static string GetDefaultPermissionSetToken(
      IVssRequestContext requestContext,
      TfsWebContext webContext,
      Guid permissionSetId)
    {
      return permissionSetId == NamespacePermissionSetConstants.ProjectLevel ? webContext.Project.Uri : string.Empty;
    }

    public static void GetSecurityPermissionsParams(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      out string subjectDescriptor,
      out Guid permissionSetId,
      out string permissionSetToken,
      out string projectName)
    {
      subjectDescriptor = (string) null;
      permissionSetId = new Guid();
      permissionSetToken = (string) null;
      if (providerContext.Properties.ContainsKey(nameof (subjectDescriptor)) && providerContext.Properties[nameof (subjectDescriptor)] != null)
        subjectDescriptor = providerContext.Properties[nameof (subjectDescriptor)].ToString();
      if (providerContext.Properties.ContainsKey(nameof (permissionSetId)) && providerContext.Properties[nameof (permissionSetId)] != null)
        Guid.TryParse(providerContext.Properties[nameof (permissionSetId)].ToString(), out permissionSetId);
      if (providerContext.Properties.ContainsKey(nameof (permissionSetToken)) && providerContext.Properties[nameof (permissionSetToken)] != null)
        permissionSetToken = providerContext.Properties[nameof (permissionSetToken)].ToString();
      TfsWebContext webContext = (TfsWebContext) WebContextFactory.GetWebContext(requestContext);
      if (permissionSetId == new Guid())
        permissionSetId = PermissionsHelper.GetPermissionSetId(webContext.NavigationContext);
      if (permissionSetToken == null)
        permissionSetToken = PermissionsHelper.GetDefaultPermissionSetToken(requestContext, webContext, permissionSetId);
      projectName = webContext.ProjectContext?.Name;
    }
  }
}

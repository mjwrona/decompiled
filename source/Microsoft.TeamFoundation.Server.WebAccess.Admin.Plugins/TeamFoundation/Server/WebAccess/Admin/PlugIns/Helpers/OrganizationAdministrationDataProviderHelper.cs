// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.Helpers.OrganizationAdministrationDataProviderHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Organization;
using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.Helpers
{
  internal class OrganizationAdministrationDataProviderHelper
  {
    public static int GetModifyPermissionBits(IVssRequestContext requestContext, Guid? collectionId = null)
    {
      string collectionToken = OrganizationSecurity.GenerateCollectionToken(collectionId);
      if (!OrganizationAdministrationDataProviderHelper.HasPermissions(requestContext, 4, collectionToken))
        return 0;
      return collectionId.HasValue ? 16 : 1;
    }

    private static bool HasPermissions(
      IVssRequestContext requestContext,
      int requestedPermissions,
      string token)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Application);
      return vssRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, OrganizationSecurity.NamespaceId).HasPermission(vssRequestContext, token, requestedPermissions, requestedPermissions < 16);
    }

    public static int GetModifyPropertiesPermissionBits(IVssRequestContext requestContext) => !OrganizationAdministrationDataProviderHelper.HasPermissions(requestContext, 4, OrganizationSecurity.PropertiesToken) ? 0 : 2;

    public static void ValidateContext(IVssRequestContext requestContext)
    {
      requestContext.CheckHostedDeployment();
      requestContext.CheckServiceHostType(TeamFoundationHostType.ProjectCollection);
    }

    internal class OrganizationPermissions
    {
      public const int Modify = 1;
      public const int ModifyProperties = 2;
      public const int Delete = 4;
      public const int ModifyCollection = 16;
      public const int DeleteCollection = 32;
      public const int ReadCollectionDetails = 64;
    }
  }
}

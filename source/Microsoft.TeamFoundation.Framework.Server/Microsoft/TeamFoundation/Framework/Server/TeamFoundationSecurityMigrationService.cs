// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationSecurityMigrationService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class TeamFoundationSecurityMigrationService : 
    IVssFrameworkService,
    ITeamFoundationSecurityMigrationHandler
  {
    public void BulkImportSecurityData(
      IVssRequestContext requestContext,
      Guid namespaceId,
      IEnumerable<IAccessControlList> accessControlLists,
      bool overwriteACLs,
      bool mergePermissions,
      char separator)
    {
      using (SecurityComponent component = requestContext.CreateComponent<SecurityComponent>())
        component.BulkImportSecurityData(namespaceId, accessControlLists, overwriteACLs, mergePermissions, separator);
    }

    public IAccessControlList BulkExportSecurityData(
      IVssRequestContext requestContext,
      Guid namespaceId,
      string securityToken,
      char separator)
    {
      using (SecurityComponent component = requestContext.CreateComponent<SecurityComponent>())
      {
        ResultCollection resultCollection = component.BulkExportSecurityData(namespaceId, securityToken, separator);
        List<IAccessControlEntry> items = resultCollection.GetCurrent<IAccessControlEntry>().Items;
        resultCollection.NextResult();
        bool inherit = resultCollection.GetCurrent<bool>().Items[0];
        return (IAccessControlList) new AccessControlList(securityToken, inherit, (IEnumerable<IAccessControlEntry>) items);
      }
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }
  }
}

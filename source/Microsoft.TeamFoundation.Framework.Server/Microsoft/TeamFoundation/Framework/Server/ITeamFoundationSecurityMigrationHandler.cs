// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ITeamFoundationSecurityMigrationHandler
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal interface ITeamFoundationSecurityMigrationHandler
  {
    void BulkImportSecurityData(
      IVssRequestContext requestContext,
      Guid namespaceId,
      IEnumerable<IAccessControlList> accessControlLists,
      bool overwriteACLs,
      bool mergePermissions,
      char separator);

    IAccessControlList BulkExportSecurityData(
      IVssRequestContext requestContext,
      Guid namespaceId,
      string securityToken,
      char separator);
  }
}

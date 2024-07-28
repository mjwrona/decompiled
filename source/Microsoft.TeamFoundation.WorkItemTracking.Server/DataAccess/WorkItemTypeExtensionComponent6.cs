// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.WorkItemTypeExtensionComponent6
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess
{
  internal class WorkItemTypeExtensionComponent6 : WorkItemTypeExtensionComponent5
  {
    public override List<WorkItemTypeletRecord> GetExtensionsById(IList<Guid> ids)
    {
      this.PrepareStoredProcedure("prc_GetWorkItemTypeExtensionsByIds");
      this.BindGuidTable("@ids", (IEnumerable<Guid>) ids);
      return this.ReadExtensions();
    }

    public override List<WorkItemTypeletRecord> GetExtensions(Guid? projectId, Guid? ownerId)
    {
      this.PrepareStoredProcedure("prc_GetWorkItemTypeExtensions");
      if (projectId.HasValue)
        this.BindGuid("@projectId", projectId.Value);
      else
        this.BindNullValue("@projectId", SqlDbType.UniqueIdentifier);
      if (ownerId.HasValue)
        this.BindGuid("@ownerId", ownerId.Value);
      else
        this.BindNullValue("@ownerId", SqlDbType.UniqueIdentifier);
      return this.ReadExtensions();
    }
  }
}

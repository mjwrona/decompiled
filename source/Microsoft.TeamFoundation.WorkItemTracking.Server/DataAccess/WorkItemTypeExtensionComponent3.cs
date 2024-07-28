// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.WorkItemTypeExtensionComponent3
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess
{
  internal class WorkItemTypeExtensionComponent3 : WorkItemTypeExtensionComponent2
  {
    internal override bool DeleteExtensions(IEnumerable<Guid> extensionIds, Guid changedBy)
    {
      this.PrepareStoredProcedure("prc_DeleteWorkItemTypeExtensions");
      this.BindGuidTable("@ids", extensionIds);
      this.BindGuid("@eventAuthor", this.Author);
      this.ExecuteNonQueryEx();
      return true;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTrackingMetadataComponent56
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  internal class WorkItemTrackingMetadataComponent56 : WorkItemTrackingMetadataComponent55
  {
    internal override void CreateFields(
      IReadOnlyCollection<CustomFieldEntry> fields,
      Guid changedBy)
    {
      this.PrepareStoredProcedure("prc_ProvisionCustomFields");
      this.BindCustomFieldTable("@customFields", (IEnumerable<CustomFieldEntry>) fields);
      this.BindGuid("@changedBy", changedBy);
      this.ExecuteNonQuery();
    }

    public override void SetFieldLocked(int fieldId, Guid teamFoundationId, bool isLocked)
    {
      this.PrepareStoredProcedure("prc_SetFieldLocked");
      this.BindInt("@fieldId", fieldId);
      this.BindBoolean("@isLocked", isLocked);
      this.BindGuid("@changedBy", teamFoundationId);
      this.ExecuteNonQuery();
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTrackingMetadataComponent46
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  internal class WorkItemTrackingMetadataComponent46 : WorkItemTrackingMetadataComponent45
  {
    internal override void UpdateField(
      string referenceName,
      string description,
      Guid changedBy,
      Guid? convertToPicklistId,
      bool? isIdentityFromProcess)
    {
      this.PrepareStoredProcedure("prc_UpdateCustomField");
      this.BindString("@referenceName", referenceName, 386, false, SqlDbType.NVarChar);
      this.BindString("@description", description, 256, false, SqlDbType.NVarChar);
      this.BindGuid("@changedBy", changedBy);
      this.BindNullableGuid("@convertToPicklistId", convertToPicklistId);
      this.BindNullableBoolean("@isIdentityFromProcess", isIdentityFromProcess);
      this.ExecuteNonQuery();
    }
  }
}

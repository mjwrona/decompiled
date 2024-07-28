// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTrackingMetadataComponent39
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  internal class WorkItemTrackingMetadataComponent39 : WorkItemTrackingMetadataComponent38
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
      this.ExecuteNonQuery();
    }

    internal override IDictionary<int, IEnumerable<string>> GetAllowedValues(
      IEnumerable<int> fieldIds,
      int? projectId = null,
      string projectName = null,
      IEnumerable<string> workItemTypeNames = null,
      bool sortById = false,
      bool excludeIdentities = false)
    {
      this.PrepareStoredProcedure(nameof (GetAllowedValues));
      this.BindUserSid();
      this.BindBoolean("@metadataFilterEnabled", false);
      this.BindInt32Table("@fieldIds", fieldIds);
      this.BindString("@projectName", projectName, (int) byte.MaxValue, true, SqlDbType.NVarChar);
      this.BindBoolean("@sortById", sortById);
      this.BindBoolean("@excludeIdentities", excludeIdentities);
      if (!string.IsNullOrEmpty(projectName))
        this.BindStringTable("@workItemTypeNames", workItemTypeNames);
      return (IDictionary<int, IEnumerable<string>>) WorkItemTrackingResourceComponent.Bind<KeyValuePair<int, string>>(this.ExecuteReader(), (System.Func<IDataReader, KeyValuePair<int, string>>) (reader1 => new KeyValuePair<int, string>(reader1.GetInt32(0), reader1.GetString(1)))).GroupBy<KeyValuePair<int, string>, int, string>((System.Func<KeyValuePair<int, string>, int>) (pair => pair.Key), (System.Func<KeyValuePair<int, string>, string>) (pair => pair.Value)).ToDictionary<IGrouping<int, string>, int, IEnumerable<string>>((System.Func<IGrouping<int, string>, int>) (pair => pair.Key), (System.Func<IGrouping<int, string>, IEnumerable<string>>) (pair => pair.AsEnumerable<string>()));
    }

    internal override bool HasAnyWorkItemsOfTypeForProcess(
      Guid processType,
      string workItemTypeName)
    {
      this.PrepareStoredProcedure("prc_HasAnyWorkItemsOfType");
      this.BindString("@witName", workItemTypeName, 256, false, SqlDbType.NVarChar);
      this.BindGuid("@processType", processType);
      return Convert.ToBoolean(this.ExecuteScalar(), (IFormatProvider) CultureInfo.InvariantCulture);
    }
  }
}

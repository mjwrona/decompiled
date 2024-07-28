// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTrackingMetadataComponent20
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  internal class WorkItemTrackingMetadataComponent20 : WorkItemTrackingMetadataComponent19
  {
    protected SqlMetaData[] typ_ImsSyncIdentityTable = new SqlMetaData[6]
    {
      new SqlMetaData("DomainName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("AccountName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("DisplayName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("SID", SqlDbType.NVarChar, 256L),
      new SqlMetaData("TeamFoundationId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Deleted", SqlDbType.Bit)
    };

    protected override void BindTypeletParameter(ISet<MetadataTable> tableNames) => this.BindBoolean("@Typelets", tableNames.Contains(MetadataTable.Typelet));

    public override IEnumerable<Guid> GetForceProcessADObjects()
    {
      this.PrepareStoredProcedure("prc_GetForceProcessADObjects");
      return (IEnumerable<Guid>) this.ExecuteUnknown<List<Guid>>((System.Func<IDataReader, List<Guid>>) (reader => new WorkItemTrackingMetadataComponent.TeamFoundationIdBinder().BindAll(reader).ToList<Guid>()));
    }

    public override void ForceSyncADObjects(IEnumerable<ImsSyncIdentity> identities)
    {
      this.PrepareStoredProcedure("prc_ForceSyncADObjects");
      this.BindImsSyncIdentityTable(identities);
      this.ExecuteNonQuery();
    }

    protected SqlParameter BindImsSyncIdentityTable(IEnumerable<ImsSyncIdentity> identities) => this.BindTable("@identities", "typ_ImsSyncIdentityTable", identities.Select<ImsSyncIdentity, SqlDataRecord>((System.Func<ImsSyncIdentity, SqlDataRecord>) (identity =>
    {
      SqlDataRecord record = new SqlDataRecord(this.typ_ImsSyncIdentityTable);
      int ordinal1 = 0;
      int num1 = ordinal1 + 1;
      record.SetNullableString(ordinal1, identity.DomainName);
      int ordinal2 = num1;
      int num2 = ordinal2 + 1;
      record.SetNullableString(ordinal2, identity.AccountName);
      int ordinal3 = num2;
      int num3 = ordinal3 + 1;
      record.SetNullableString(ordinal3, identity.DisplayName);
      int ordinal4 = num3;
      int num4 = ordinal4 + 1;
      record.SetNullableString(ordinal4, identity.Sid);
      int ordinal5 = num4;
      int ordinal6 = ordinal5 + 1;
      record.SetGuid(ordinal5, identity.TeamFoundationId);
      record.SetBoolean(ordinal6, identity.Deleted);
      return record;
    })));

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
      this.ExecuteNonQuery();
    }

    internal override void DeleteProcess(Guid processId, Guid changedBy, bool deleteFields = true)
    {
      this.PrepareStoredProcedure("prc_DeleteProcess");
      this.BindGuid("@processId", processId);
      this.BindInt("@changedBy", -1);
      this.ExecuteNonQuery();
    }
  }
}

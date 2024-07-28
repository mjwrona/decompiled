// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTrackingMetadataComponent19
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  internal class WorkItemTrackingMetadataComponent19 : WorkItemTrackingMetadataComponent18
  {
    protected virtual SqlParameter BindCustomFieldTable(
      string parameterName,
      IEnumerable<CustomFieldEntry> fieldEntries)
    {
      return this.BindBasicTvp<CustomFieldEntry>((WorkItemTrackingResourceComponent.TvpRecordBinder<CustomFieldEntry>) new WorkItemTrackingMetadataComponent19.CustomFieldTableRecordBinder(), parameterName, fieldEntries);
    }

    protected override WorkItemTrackingMetadataComponent.FieldRecordBinder GetFieldRecordBinder() => (WorkItemTrackingMetadataComponent.FieldRecordBinder) new WorkItemTrackingMetadataComponent19.FieldRecordBinder4();

    internal override void CreateFields(
      IReadOnlyCollection<CustomFieldEntry> fields,
      Guid changedBy)
    {
      this.PrepareStoredProcedure("prc_ProvisionCustomFields");
      this.BindCustomFieldTable("@customFields", (IEnumerable<CustomFieldEntry>) fields);
      this.ExecuteNonQuery();
    }

    internal override IEnumerable<ConstantRecord> GetNonIdentityConstants(
      IEnumerable<string> displayTextValues,
      bool includeInactiveNonIdentityConstants = true)
    {
      this.PrepareStoredProcedure("GetConstantRecords");
      string[] array = displayTextValues.Distinct<string>().ToArray<string>();
      List<OrderedString> witOrderedStrings = new List<OrderedString>(array.Length);
      for (int index = 0; index < array.Length; ++index)
        witOrderedStrings.Add(new OrderedString()
        {
          Value = array[index],
          Order = index
        });
      this.BindUserSid();
      this.BindWitOrderedStringTable("@values", (IEnumerable<OrderedString>) witOrderedStrings);
      this.BindInt("@searchFactor", 5);
      this.BindGetInactiveIdentities(false);
      this.BindIncludeInactiveConstants(includeInactiveNonIdentityConstants);
      return this.ExecuteUnknown<IEnumerable<ConstantRecord>>((System.Func<IDataReader, IEnumerable<ConstantRecord>>) (reader => (IEnumerable<ConstantRecord>) this.GetConstantRecordBinder(true).BindAll(reader).ToList<ConstantRecord>()));
    }

    protected class CustomFieldTableRecordBinder : 
      WorkItemTrackingResourceComponent.TvpRecordBinder<CustomFieldEntry>
    {
      private static readonly SqlMetaData[] s_metadata = new SqlMetaData[12]
      {
        new SqlMetaData("Name", SqlDbType.NVarChar, 128L),
        new SqlMetaData("ReferenceName", SqlDbType.NVarChar, 386L),
        new SqlMetaData("Type", SqlDbType.Int),
        new SqlMetaData("ReportingType", SqlDbType.Int),
        new SqlMetaData("ReportingFormula", SqlDbType.Int),
        new SqlMetaData("ReportingEnabled", SqlDbType.Bit),
        new SqlMetaData("ReportingName", SqlDbType.NVarChar, 128L),
        new SqlMetaData("ReportingReferenceName", SqlDbType.NVarChar, 386L),
        new SqlMetaData("Usage", SqlDbType.Int),
        new SqlMetaData("ParentFieldId", SqlDbType.Int),
        new SqlMetaData("ProcessId", SqlDbType.UniqueIdentifier),
        new SqlMetaData("Description", SqlDbType.NVarChar, 256L)
      };

      public override string TypeName => "typ_WitCustomFieldTable4";

      protected override SqlMetaData[] TvpMetadata => WorkItemTrackingMetadataComponent19.CustomFieldTableRecordBinder.s_metadata;

      public override void SetRecordValues(
        WorkItemTrackingSqlDataRecord record,
        CustomFieldEntry fieldEntry)
      {
        record.SetString(0, fieldEntry.Name);
        record.SetString(1, fieldEntry.ReferenceName);
        record.SetInt32(2, fieldEntry.Type);
        record.SetInt32(3, fieldEntry.ReportingType);
        record.SetInt32(4, fieldEntry.ReportingFormula);
        record.SetBoolean(5, fieldEntry.ReportingEnabled);
        if (fieldEntry.ReportingName != null)
          record.SetString(6, fieldEntry.ReportingName);
        else
          record.SetDBNull(6);
        if (fieldEntry.ReportingReferenceName != null)
          record.SetString(7, fieldEntry.ReportingReferenceName);
        else
          record.SetDBNull(7);
        record.SetInt32(8, fieldEntry.Usage);
        record.SetInt32(9, fieldEntry.ParentFieldId);
        if (fieldEntry.ProcessId != new Guid())
          record.SetGuid(10, fieldEntry.ProcessId);
        else
          record.SetDBNull(10);
        if (fieldEntry.Description != null)
          record.SetString(11, fieldEntry.Description);
        else
          record.SetDBNull(11);
      }
    }

    protected class FieldRecordBinder4 : WorkItemTrackingMetadataComponent18.FieldRecordBinder3
    {
      protected SqlColumnBinder m_processId;
      protected SqlColumnBinder m_descriptor;

      public FieldRecordBinder4()
      {
        this.m_processId = new SqlColumnBinder("ProcessId");
        this.m_descriptor = new SqlColumnBinder("Description");
      }

      public override FieldRecord Bind(IDataReader reader)
      {
        FieldRecord fieldRecord = base.Bind(reader);
        fieldRecord.ProcessId = this.m_processId.GetGuid(reader, true);
        fieldRecord.Descriptor = this.m_descriptor.GetString(reader, true);
        return fieldRecord;
      }
    }
  }
}

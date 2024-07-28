// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DalMetadataChangeValidationElement
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DalMetadataChangeValidationElement : DalSqlElement
  {
    private static readonly Dictionary<MetadataTable, string> s_metadataTableMapping = new Dictionary<MetadataTable, string>()
    {
      {
        MetadataTable.Actions,
        "Actions"
      },
      {
        MetadataTable.Constants,
        "Constants"
      },
      {
        MetadataTable.ConstantSets,
        "Sets"
      },
      {
        MetadataTable.Fields,
        "tbl_Field"
      },
      {
        MetadataTable.FieldUsages,
        "tbl_FieldUsage"
      },
      {
        MetadataTable.Hierarchy,
        "TreeNodes"
      },
      {
        MetadataTable.HierarchyProperties,
        "TreeProperties"
      },
      {
        MetadataTable.LinkTypes,
        "LinkTypes"
      },
      {
        MetadataTable.Rules,
        "Rules"
      },
      {
        MetadataTable.WorkItemTypeCategories,
        "WorkItemTypeCategories"
      },
      {
        MetadataTable.WorkItemTypeCategoryMembers,
        "WorkItemTypeCategoryMembers"
      },
      {
        MetadataTable.WorkItemTypes,
        "WorkItemTypes"
      },
      {
        MetadataTable.WorkItemTypeUsages,
        "WorkItemTypeUsages"
      }
    };

    public void JoinBatch(MetadataDBStamps stampsSnapshot)
    {
      if (stampsSnapshot == null || stampsSnapshot.Count == 0)
        return;
      this.AppendSql("declare @cacheStampMismatch as bit");
      this.AppendSql(Environment.NewLine);
      foreach (MetadataTable key in stampsSnapshot.Keys)
      {
        string str1 = DalMetadataChangeValidationElement.s_metadataTableMapping[key];
        string str2 = this.SqlBatch.AddParameterBinary(DalMetadataSelectElement.ConvertLongToByteArray(stampsSnapshot[key]));
        this.AppendSql(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "select @cacheStampMismatch = case when exists (select * from dbo.[{0}] where dbo.[{0}].[{1}] > {2}", (object) str1, (object) "Cachestamp", (object) str2));
        if (this.IsSchemaPartitioned)
          this.AppendSql(string.Format((IFormatProvider) CultureInfo.InvariantCulture, " and dbo.[{0}].[{1}] = {2}", (object) str1, (object) "PartitionId", (object) "@partitionId"));
        this.AppendSql(") then 1 else 0 end");
        if (this.IsSchemaPartitioned)
          this.AppendSql(string.Format((IFormatProvider) CultureInfo.InvariantCulture, " option (optimize for ({0} unknown))", (object) "@partitionId"));
        this.AppendSql(string.Format((IFormatProvider) CultureInfo.InvariantCulture, " if (@cacheStampMismatch = 1) begin if (@@trancount <> 0) rollback tran; exec dbo.RaiseWITError {0},11,1; return; end;", (object) 600178));
        this.AppendSql(Environment.NewLine);
      }
    }
  }
}

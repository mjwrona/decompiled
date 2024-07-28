// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Column
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal sealed class Column
  {
    private Column()
    {
    }

    public Column(int objectId, FieldEntry field, ColumnType type, string value)
    {
      this.ObjectId = objectId;
      this.Field = field;
      this.ColType = type;
      switch (type)
      {
        case ColumnType.ServerDateTime:
          this.Value = (object) new ServerDefaultFieldValue(ServerDefaultType.ServerDateTime);
          break;
        case ColumnType.ServerRandomGuid:
          this.Value = (object) Guid.NewGuid();
          if (field.SqlType != SqlDbType.NVarChar)
            break;
          this.Value = (object) ((Guid) this.Value).ToString("D");
          break;
        case ColumnType.TrendDataValue:
          break;
        default:
          if (string.IsNullOrEmpty(value))
            break;
          switch (field.SqlType)
          {
            case SqlDbType.Bit:
              this.Value = (object) SqlBatchBuilder.ConvertToBoolean(value);
              return;
            case SqlDbType.DateTime:
              this.Value = (object) SqlBatchBuilder.ConvertToDateTime(value);
              return;
            case SqlDbType.Float:
              this.Value = (object) SqlBatchBuilder.ConvertToDouble(value);
              return;
            case SqlDbType.Int:
              this.Value = (object) SqlBatchBuilder.ConvertToInt32(value);
              return;
            case SqlDbType.UniqueIdentifier:
              this.Value = (object) SqlBatchBuilder.ConvertToGuid(value);
              if (!SpecialGuids.ServerDefaultGuidRuleConstant.Equals(this.Value))
                return;
              this.Value = (object) Guid.NewGuid();
              return;
            default:
              this.Value = (object) value;
              return;
          }
      }
    }

    public int ObjectId { get; internal set; }

    public FieldEntry Field { get; private set; }

    public ColumnType ColType { get; private set; }

    public object Value { get; set; }
  }
}

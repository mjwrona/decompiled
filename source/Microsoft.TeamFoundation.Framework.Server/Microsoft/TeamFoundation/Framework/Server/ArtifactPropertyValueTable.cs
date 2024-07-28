// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ArtifactPropertyValueTable
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Obsolete]
  public class ArtifactPropertyValueTable : TeamFoundationTableValueParameter<ArtifactPropertyValue>
  {
    private static readonly SqlMetaData[] s_metadata = new SqlMetaData[10]
    {
      new SqlMetaData("ArtifactId", SqlDbType.VarBinary, 64L),
      new SqlMetaData("Version", SqlDbType.Int),
      new SqlMetaData("Moniker", SqlDbType.NVarChar, 440L),
      new SqlMetaData("PropertyName", SqlDbType.NVarChar, 400L),
      new SqlMetaData("PropertyId", SqlDbType.Int),
      new SqlMetaData("IntValue", SqlDbType.Int),
      new SqlMetaData("DatetimeValue", SqlDbType.DateTime),
      new SqlMetaData("StringValue", SqlDbType.NVarChar, -1L),
      new SqlMetaData("DoubleValue", SqlDbType.Float),
      new SqlMetaData("BinaryValue", SqlDbType.VarBinary, -1L)
    };
    private const int c_propertyIdColIndex = 4;
    private static readonly TimeSpan s_oneMillisecond = new TimeSpan(0, 0, 0, 0, 1);

    public ArtifactPropertyValueTable(IEnumerable<ArtifactPropertyValue> props)
      : base(props, "typ_ArtifactPropertyValueTable", ArtifactPropertyValueTable.s_metadata)
    {
    }

    public override IEnumerator<SqlDataRecord> GetEnumerator()
    {
      ArtifactPropertyValueTable propertyValueTable = this;
      foreach (ArtifactPropertyValue apv in propertyValueTable.m_rows)
      {
        foreach (PropertyValue propertyValue in apv.PropertyValues)
          yield return propertyValueTable.SetRecord(apv.Spec, propertyValue);
      }
    }

    private SqlDataRecord SetRecord(ArtifactSpec spec, PropertyValue prop)
    {
      if (!string.IsNullOrEmpty(spec.Moniker))
      {
        this.m_record.SetDBNull(0);
        this.m_record.SetString(2, spec.Moniker);
      }
      else
      {
        this.m_record.SetBytes(0, 0L, spec.Id, 0, spec.Id.Length);
        this.m_record.SetDBNull(2);
      }
      this.m_record.SetInt32(1, spec.Version);
      this.m_record.SetString(3, prop.PropertyName);
      for (int ordinal = 4; ordinal < ArtifactPropertyValueTable.s_metadata.Length; ++ordinal)
        this.m_record.SetDBNull(ordinal);
      if (prop.Value != null)
      {
        Type type = prop.Value.GetType();
        switch (Type.GetTypeCode(type))
        {
          case TypeCode.Empty:
            throw new PropertyTypeNotSupportedException(prop.PropertyName, type);
          case TypeCode.Object:
            if (prop.Value is byte[])
            {
              byte[] buffer = (byte[]) prop.Value;
              this.m_record.SetBytes(9, 0L, buffer, 0, buffer.Length);
              break;
            }
            if (!(prop.Value is Guid))
              throw new PropertyTypeNotSupportedException(prop.PropertyName, type);
            this.m_record.SetString(7, ((Guid) prop.Value).ToString("N"));
            break;
          case TypeCode.DBNull:
            throw new PropertyTypeNotSupportedException(prop.PropertyName, type);
          case TypeCode.Int32:
            this.m_record.SetInt32(5, (int) prop.Value);
            break;
          case TypeCode.Double:
            this.m_record.SetDouble(8, (double) prop.Value);
            break;
          case TypeCode.DateTime:
            DateTime dateTime = (DateTime) prop.Value;
            if (dateTime.Equals(DateTime.MaxValue))
              dateTime = dateTime.Subtract(ArtifactPropertyValueTable.s_oneMillisecond);
            this.m_record.SetDateTime(6, dateTime);
            break;
          case TypeCode.String:
            this.m_record.SetString(7, (string) prop.Value);
            break;
          default:
            this.m_record.SetString(7, prop.Value.ToString());
            break;
        }
      }
      return this.m_record;
    }

    public override void SetRecord(ArtifactPropertyValue prop, SqlDataRecord record) => throw new NotImplementedException();
  }
}

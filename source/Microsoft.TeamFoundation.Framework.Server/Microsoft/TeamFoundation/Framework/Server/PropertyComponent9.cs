// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.PropertyComponent9
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class PropertyComponent9 : PropertyComponent8
  {
    private static readonly SqlMetaData[] typ_ArtifactPropertySpecTable2 = new SqlMetaData[4]
    {
      new SqlMetaData("ArtifactId", SqlDbType.VarBinary, 64L),
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("Moniker", SqlDbType.NVarChar, 440L),
      new SqlMetaData("Version", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] typ_ArtifactPropertySpecCopyTable = new SqlMetaData[4]
    {
      new SqlMetaData("ArtifactId", SqlDbType.VarBinary, 64L),
      new SqlMetaData("TargetArtifactId", SqlDbType.VarBinary, 64L),
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("TargetDataspaceId", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] typ_ArtifactPropertyValueTable2 = new SqlMetaData[11]
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
      new SqlMetaData("BinaryValue", SqlDbType.VarBinary, -1L),
      new SqlMetaData("DataspaceId", SqlDbType.Int)
    };

    protected override PropertyComponent.PropertyDefinitionColumns GetPropertyDefinitionColumns() => (PropertyComponent.PropertyDefinitionColumns) new PropertyComponent.PropertyDefinitionColumns2((PropertyComponent) this);

    internal override void DeleteArtifacts(
      IEnumerable<ArtifactSpec> artifactSpecs,
      ArtifactKind kind,
      PropertiesOptions options)
    {
      this.PrepareStoredProcedure("prc_DeleteArtifacts");
      this.BindInt("@kindId", kind.CompactKindId);
      this.BindGuid("@author", this.Author);
      System.Func<ArtifactSpec, SqlDataRecord> selector = (System.Func<ArtifactSpec, SqlDataRecord>) (artifactSpec =>
      {
        this.ValidateArtifactSpec(artifactSpec, options, kind);
        SqlDataRecord sqlDataRecord = new SqlDataRecord(PropertyComponent9.typ_ArtifactPropertySpecTable2);
        if (string.IsNullOrEmpty(artifactSpec.Moniker))
        {
          sqlDataRecord.SetBytes(0, 0L, artifactSpec.Id, 0, artifactSpec.Id.Length);
          sqlDataRecord.SetDBNull(2);
        }
        else
        {
          sqlDataRecord.SetDBNull(0);
          sqlDataRecord.SetString(2, artifactSpec.Moniker);
        }
        sqlDataRecord.SetInt32(1, this.GetDataspaceId(artifactSpec.DataspaceIdentifier));
        if (options == PropertiesOptions.None)
          sqlDataRecord.SetInt32(3, artifactSpec.Version);
        else
          sqlDataRecord.SetDBNull(3);
        return sqlDataRecord;
      });
      this.BindTable("@artifactSpecs", "typ_ArtifactPropertySpecTable2", artifactSpecs.Select<ArtifactSpec, SqlDataRecord>(selector));
      this.ExecuteNonQuery();
    }

    internal override bool SetPropertyValue(
      IEnumerable<ArtifactPropertyValue> artifactPropertyValues,
      ArtifactKind kind,
      DateTime? changedDate,
      Guid? changedBy)
    {
      this.PrepareStoredProcedure("prc_SetPropertyValue");
      this.BindInt("@kindId", kind.CompactKindId);
      this.BindGuid("@author", this.Author);
      this.BindTable("@propertyValues", "typ_ArtifactPropertyValueTable2", this.ConvertToSqlRecords(artifactPropertyValues, kind));
      if (changedDate.HasValue)
        this.BindDateTime("@changedDate", changedDate.Value);
      if (changedBy.HasValue)
        this.BindGuid("@changedBy", changedBy.Value);
      return this.SetPropertyValue();
    }

    protected virtual bool SetPropertyValue()
    {
      this.ExecuteNonQuery();
      return true;
    }

    private IEnumerable<SqlDataRecord> ConvertToSqlRecords(
      IEnumerable<ArtifactPropertyValue> artifactPropertyValues,
      ArtifactKind kind)
    {
      List<SqlDataRecord> sqlRecords = new List<SqlDataRecord>();
      foreach (ArtifactPropertyValue artifactPropertyValue in artifactPropertyValues)
      {
        ArgumentUtility.CheckForNull<ArtifactPropertyValue>(artifactPropertyValue, "artifactPropertyValue");
        this.ValidateArtifactSpec(artifactPropertyValue.Spec, PropertiesOptions.None, kind);
        sqlRecords.AddRange(PropertyComponent9.ConvertToSqlRecords(artifactPropertyValue, this.GetDataspaceId(artifactPropertyValue.Spec.DataspaceIdentifier), out string _));
      }
      return (IEnumerable<SqlDataRecord>) sqlRecords;
    }

    internal static IEnumerable<SqlDataRecord> ConvertToSqlRecords(
      ArtifactPropertyValue artifactPropertyValue,
      int dataspaceId,
      out string typeName)
    {
      ArgumentUtility.CheckForNull<ArtifactPropertyValue>(artifactPropertyValue, nameof (artifactPropertyValue));
      ArgumentUtility.CheckForNull<ArtifactSpec>(artifactPropertyValue.Spec, "Spec");
      List<SqlDataRecord> sqlRecords = new List<SqlDataRecord>();
      SqlMetaData[] valueTableMetadata = PropertyComponent9.GetArtifactPropertyValueTableMetadata(out typeName);
      ArtifactSpec spec = artifactPropertyValue.Spec;
      if (artifactPropertyValue.PropertyValues != null)
      {
        foreach (PropertyValue propertyValue in artifactPropertyValue.PropertyValues)
        {
          propertyValue.Validate();
          SqlDataRecord sqlDataRecord = new SqlDataRecord(valueTableMetadata);
          if (string.IsNullOrEmpty(spec.Moniker))
          {
            sqlDataRecord.SetBytes(0, 0L, spec.Id, 0, spec.Id.Length);
            sqlDataRecord.SetDBNull(2);
          }
          else
          {
            sqlDataRecord.SetDBNull(0);
            sqlDataRecord.SetString(2, spec.Moniker);
          }
          sqlDataRecord.SetInt32(1, spec.Version);
          sqlDataRecord.SetString(3, propertyValue.PropertyName);
          object obj = propertyValue.Value;
          int ordinal1 = -1;
          if (obj != null)
          {
            Type type = obj.GetType();
            TypeCode typeCode = Type.GetTypeCode(type);
            if (typeCode == TypeCode.Object && obj is byte[])
            {
              byte[] buffer = (byte[]) obj;
              ordinal1 = 9;
              sqlDataRecord.SetBytes(ordinal1, 0L, buffer, 0, buffer.Length);
            }
            else if (typeCode == TypeCode.Object && obj is Guid guid)
            {
              ordinal1 = 7;
              sqlDataRecord.SetString(7, guid.ToString("N"));
            }
            else
            {
              switch (typeCode)
              {
                case TypeCode.Empty:
                  throw new Microsoft.VisualStudio.Services.Common.PropertyTypeNotSupportedException(propertyValue.PropertyName, type);
                case TypeCode.Object:
                  throw new Microsoft.VisualStudio.Services.Common.PropertyTypeNotSupportedException(propertyValue.PropertyName, type);
                case TypeCode.DBNull:
                  throw new Microsoft.VisualStudio.Services.Common.PropertyTypeNotSupportedException(propertyValue.PropertyName, type);
                case TypeCode.Int32:
                  ordinal1 = 5;
                  sqlDataRecord.SetInt32(ordinal1, (int) obj);
                  break;
                case TypeCode.Double:
                  ordinal1 = 8;
                  sqlDataRecord.SetDouble(ordinal1, (double) obj);
                  break;
                case TypeCode.DateTime:
                  ordinal1 = 6;
                  sqlDataRecord.SetDateTime(ordinal1, (DateTime) obj);
                  break;
                case TypeCode.String:
                  ordinal1 = 7;
                  sqlDataRecord.SetString(7, (string) obj);
                  break;
                default:
                  ordinal1 = 7;
                  sqlDataRecord.SetString(7, obj.ToString());
                  break;
              }
            }
          }
          for (int ordinal2 = 5; ordinal2 <= 9; ++ordinal2)
          {
            if (ordinal2 != ordinal1)
              sqlDataRecord.SetDBNull(ordinal2);
          }
          sqlDataRecord.SetInt32(10, dataspaceId);
          sqlRecords.Add(sqlDataRecord);
        }
      }
      return (IEnumerable<SqlDataRecord>) sqlRecords;
    }

    private static SqlMetaData[] GetArtifactPropertyValueTableMetadata(out string typeName)
    {
      typeName = "typ_ArtifactPropertyValueTable2";
      return PropertyComponent9.typ_ArtifactPropertyValueTable2;
    }

    internal override void CopyPropertyValues(
      IEnumerable<KeyValuePair<ArtifactSpec, ArtifactSpec>> artifactSpecs,
      ArtifactKind kind)
    {
      this.PrepareStoredProcedure("prc_pCopyPropertyValues");
      this.BindGuid("@kind", kind.Kind);
      System.Func<KeyValuePair<ArtifactSpec, ArtifactSpec>, SqlDataRecord> selector = (System.Func<KeyValuePair<ArtifactSpec, ArtifactSpec>, SqlDataRecord>) (copyValue =>
      {
        ArtifactSpec key = copyValue.Key;
        ArtifactSpec artifactSpec = copyValue.Value;
        this.ValidateArtifactSpec(key, PropertiesOptions.None, kind);
        if (key.Kind != artifactSpec.Kind)
          throw new InvalidOperationException("Cannot change kinds during copy operation.");
        SqlDataRecord sqlDataRecord = new SqlDataRecord(PropertyComponent9.typ_ArtifactPropertySpecCopyTable);
        if (!string.IsNullOrEmpty(key.Moniker))
          throw new InvalidOperationException("Do not support copying using source spec moniker.");
        sqlDataRecord.SetBytes(0, 0L, key.Id, 0, key.Id.Length);
        if (!string.IsNullOrEmpty(artifactSpec.Moniker))
          throw new InvalidOperationException("Do not support copying using target spec moniker.");
        sqlDataRecord.SetBytes(1, 0L, artifactSpec.Id, 0, artifactSpec.Id.Length);
        sqlDataRecord.SetInt32(2, this.GetDataspaceId(key.DataspaceIdentifier));
        sqlDataRecord.SetInt32(3, this.GetDataspaceId(artifactSpec.DataspaceIdentifier));
        return sqlDataRecord;
      });
      this.BindTable("@propertySpecs", "typ_ArtifactPropertySpecCopyTable", artifactSpecs.Select<KeyValuePair<ArtifactSpec, ArtifactSpec>, SqlDataRecord>(selector));
      this.ExecuteNonQuery();
    }
  }
}

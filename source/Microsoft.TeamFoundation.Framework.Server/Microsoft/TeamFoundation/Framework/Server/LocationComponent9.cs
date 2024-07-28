// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.LocationComponent9
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.VisualStudio.Services.Location;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class LocationComponent9 : LocationComponent8
  {
    private static readonly SqlMetaData[] typ_ServiceDefinitionTable4 = new SqlMetaData[16]
    {
      new SqlMetaData("ServiceType", SqlDbType.NVarChar, 256L),
      new SqlMetaData("Identifier", SqlDbType.UniqueIdentifier),
      new SqlMetaData("DisplayName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("RelativeToSetting", SqlDbType.Int),
      new SqlMetaData("RelativePath", SqlDbType.NVarChar, 256L),
      new SqlMetaData("IsSingleton", SqlDbType.Bit),
      new SqlMetaData("Description", SqlDbType.NVarChar, -1L),
      new SqlMetaData("ToolType", SqlDbType.NVarChar, 256L),
      new SqlMetaData("ParentServiceType", SqlDbType.NVarChar, 256L),
      new SqlMetaData("ParentIdentifier", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Status", SqlDbType.TinyInt),
      new SqlMetaData("InheritLevel", SqlDbType.TinyInt),
      new SqlMetaData("ResourceVersion", SqlDbType.Int),
      new SqlMetaData("MinVersion", SqlDbType.VarChar, 30L),
      new SqlMetaData("MaxVersion", SqlDbType.VarChar, 30L),
      new SqlMetaData("ReleasedVersion", SqlDbType.VarChar, 30L)
    };

    protected override ServiceDefinitionDataColumns GetServiceDefinitionDataColumns() => (ServiceDefinitionDataColumns) new ServiceDefinitionDataColumns4();

    protected virtual string NormalizeToolType(string toolType) => toolType;

    protected override SqlParameter BindServiceDefinitionTable(
      string parameterName,
      IEnumerable<ServiceDefinition> rows,
      bool coreFieldsOnly = false)
    {
      rows = rows ?? Enumerable.Empty<ServiceDefinition>();
      System.Func<ServiceDefinition, SqlDataRecord> selector = (System.Func<ServiceDefinition, SqlDataRecord>) (serviceDefinition =>
      {
        SqlDataRecord record = new SqlDataRecord(LocationComponent9.typ_ServiceDefinitionTable4);
        record.SetString(0, serviceDefinition.ServiceType);
        record.SetGuid(1, serviceDefinition.Identifier);
        if (!coreFieldsOnly)
        {
          record.SetNullableString(2, serviceDefinition.DisplayName);
          record.SetInt32(3, (int) serviceDefinition.RelativeToSetting);
          record.SetNullableString(4, serviceDefinition.RelativePath);
          record.SetBoolean(5, false);
          record.SetNullableString(6, serviceDefinition.Description);
          record.SetNullableString(7, this.NormalizeToolType(serviceDefinition.ToolId));
          if (serviceDefinition.ParentIdentifier == Guid.Empty)
          {
            record.SetDBNull(8);
            record.SetDBNull(9);
          }
          else
          {
            record.SetString(8, serviceDefinition.ParentServiceType);
            record.SetGuid(9, serviceDefinition.ParentIdentifier);
          }
          record.SetByte(10, (byte) serviceDefinition.Status);
          record.SetByte(11, (byte) serviceDefinition.InheritLevel);
          if (serviceDefinition.ResourceVersion == 0)
            record.SetDBNull(12);
          else
            record.SetInt32(12, serviceDefinition.ResourceVersion);
          if (serviceDefinition.MinVersion == (System.Version) null)
            record.SetDBNull(13);
          else
            record.SetString(13, serviceDefinition.MinVersionString);
          if (serviceDefinition.MaxVersion == (System.Version) null)
            record.SetDBNull(14);
          else
            record.SetString(14, serviceDefinition.MaxVersionString);
          if (serviceDefinition.ReleasedVersion == (System.Version) null)
            record.SetDBNull(15);
          else
            record.SetString(15, serviceDefinition.ReleasedVersionString);
        }
        else
        {
          record.SetDBNull(2);
          record.SetDBNull(3);
          record.SetDBNull(4);
          record.SetDBNull(5);
          record.SetDBNull(6);
          record.SetDBNull(7);
          record.SetDBNull(8);
          record.SetDBNull(9);
          record.SetDBNull(10);
          record.SetDBNull(11);
          record.SetDBNull(12);
          record.SetDBNull(13);
          record.SetDBNull(14);
          record.SetDBNull(15);
        }
        return record;
      });
      return this.BindTable(parameterName, "typ_ServiceDefinitionTable4", rows.Select<ServiceDefinition, SqlDataRecord>(selector));
    }
  }
}

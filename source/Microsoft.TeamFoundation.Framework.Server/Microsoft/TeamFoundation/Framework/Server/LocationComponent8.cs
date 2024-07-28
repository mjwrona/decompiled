// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.LocationComponent8
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
  internal class LocationComponent8 : LocationComponent7
  {
    private static readonly SqlMetaData[] typ_ServiceDefinitionTable3 = new SqlMetaData[12]
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
      new SqlMetaData("InheritLevel", SqlDbType.TinyInt)
    };

    public override ResultCollection QueryServiceData()
    {
      this.PrepareStoredProcedure("prc_QueryServiceData");
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QueryServiceData", this.RequestContext);
      resultCollection.AddBinder<ServiceDefinition>((ObjectBinder<ServiceDefinition>) this.GetServiceDefinitionDataColumns());
      resultCollection.AddBinder<LocationMappingData>((ObjectBinder<LocationMappingData>) new LocationMappingDataColumns());
      resultCollection.AddBinder<AccessMapping>((ObjectBinder<AccessMapping>) new AccessMappingColumns());
      resultCollection.AddBinder<string>((ObjectBinder<string>) new DefaultAccessMappingColumn());
      resultCollection.AddBinder<long>(this.CreateLastChangeIdBinder());
      return resultCollection;
    }

    protected virtual ServiceDefinitionDataColumns GetServiceDefinitionDataColumns() => (ServiceDefinitionDataColumns) new ServiceDefinitionDataColumns3();

    protected override SqlParameter BindServiceDefinitionTable(
      string parameterName,
      IEnumerable<ServiceDefinition> rows,
      bool coreFieldsOnly = false)
    {
      rows = rows ?? Enumerable.Empty<ServiceDefinition>();
      System.Func<ServiceDefinition, SqlDataRecord> selector = (System.Func<ServiceDefinition, SqlDataRecord>) (serviceDefinition =>
      {
        SqlDataRecord record = new SqlDataRecord(LocationComponent8.typ_ServiceDefinitionTable3);
        record.SetString(0, serviceDefinition.ServiceType);
        record.SetGuid(1, serviceDefinition.Identifier);
        if (!coreFieldsOnly)
        {
          record.SetNullableString(2, serviceDefinition.DisplayName);
          record.SetInt32(3, (int) serviceDefinition.RelativeToSetting);
          record.SetNullableString(4, serviceDefinition.RelativePath);
          record.SetBoolean(5, false);
          record.SetNullableString(6, serviceDefinition.Description);
          record.SetNullableString(7, serviceDefinition.ToolId);
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
        }
        return record;
      });
      return this.BindTable(parameterName, "typ_ServiceDefinitionTable3", rows.Select<ServiceDefinition, SqlDataRecord>(selector));
    }
  }
}

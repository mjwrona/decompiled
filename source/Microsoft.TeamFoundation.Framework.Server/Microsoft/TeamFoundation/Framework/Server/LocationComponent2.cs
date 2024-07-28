// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.LocationComponent2
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Location;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class LocationComponent2 : LocationComponent
  {
    internal Guid ServiceHostId { get; set; }

    protected override void Initialize(
      IVssRequestContext requestContext,
      string databaseCategory,
      Guid dataspaceIdentifier,
      int serviceVersion,
      DatabaseConnectionType connectionType,
      ITFLogger logger)
    {
      base.Initialize(requestContext, databaseCategory, dataspaceIdentifier, serviceVersion, connectionType, logger);
      this.ServiceHostId = requestContext.ServiceHost.InstanceId;
    }

    public override void ConfigureAccessMapping(
      AccessMapping accessMapping,
      bool makeDefault,
      bool allowOverlapping)
    {
      this.PrepareStoredProcedure("prc_ConfigureAccessMapping");
      this.BindServiceHostId(true);
      this.BindString("@moniker", accessMapping.Moniker, 128, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@displayName", accessMapping.DisplayName, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@accessPoint", accessMapping.AccessPoint, -1, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindBoolean("@makeDefault", makeDefault);
      this.BindGuid("@writerIdentifier", this.Author);
      this.ExecuteNonQuery();
    }

    public override void RemoveAccessMappings(IEnumerable<AccessMapping> accessMappings)
    {
      this.PrepareStoredProcedure("prc_RemoveAccessMappings");
      this.BindServiceHostId(true);
      this.BindStringTable("@accessMapping", accessMappings.Select<AccessMapping, string>((System.Func<AccessMapping, string>) (mapping => mapping.Moniker)));
      this.BindBoolean("@inheritsMappings", !this.RequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment));
      this.BindGuid("@writerIdentifier", this.Author);
      this.ExecuteNonQuery();
    }

    public override bool SaveServiceDefinitions(IEnumerable<ServiceDefinition> serviceDefinitions)
    {
      this.PrepareStoredProcedure("prc_SaveServiceDefinitions");
      this.BindServiceDefinitionTable("@definition", serviceDefinitions);
      this.BindLocationMappingTable("@locationMapping", serviceDefinitions);
      this.BindGuid("@writerIdentifier", this.Author);
      this.ExecuteNonQuery();
      return true;
    }

    protected override void RemoveServiceDefinitions(IEnumerable<ServiceDefinition> definitions)
    {
      this.PrepareStoredProcedure("prc_RemoveServiceDefinitions");
      this.BindServiceDefinitionTable("@definition", definitions, true);
      this.BindGuid("@writerIdentifier", this.Author);
      this.ExecuteNonQuery();
    }

    public override ResultCollection QueryServiceData()
    {
      this.PrepareStoredProcedure("prc_QueryServiceData");
      this.BindServiceHostId(true);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QueryServiceData", this.RequestContext);
      resultCollection.AddBinder<ServiceDefinition>((ObjectBinder<ServiceDefinition>) new ServiceDefinitionDataColumns());
      resultCollection.AddBinder<LocationMappingData>((ObjectBinder<LocationMappingData>) new LocationMappingDataColumns());
      resultCollection.AddBinder<AccessMapping>((ObjectBinder<AccessMapping>) new AccessMappingColumns());
      resultCollection.AddBinder<string>((ObjectBinder<string>) new DefaultAccessMappingColumn());
      resultCollection.AddBinder<long>(this.CreateLastChangeIdBinder());
      return resultCollection;
    }

    protected virtual void BindServiceHostId(bool alwaysBind) => this.BindGuid("@hostId", this.ServiceHostId);
  }
}

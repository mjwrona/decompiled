// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildAnalyticsService
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public sealed class BuildAnalyticsService : IBuildAnalyticsService, IVssFrameworkService
  {
    private const string c_traceLayer = "BuildAnalyticsService";
    private readonly RegistryQuery s_readAXDataFromReplica = new RegistryQuery("/Service/Build2/Settings/ReadAXDataFromReplica");

    public async Task<List<BuildAnalyticsData>> GetBuildsByDateAsync(
      IVssRequestContext requestContext,
      int dataspaceId,
      int batchSize,
      DateTime? fromDate)
    {
      List<BuildAnalyticsData> buildsByDateAsync1;
      using (requestContext.TraceScope(nameof (BuildAnalyticsService), nameof (GetBuildsByDateAsync)))
      {
        List<BuildAnalyticsData> buildsByDateAsync2;
        using (Build2Component bc = requestContext.CreateComponent<Build2Component>(connectionType: new DatabaseConnectionType?(requestContext.GetService<IVssRegistryService>().GetValue<bool>(requestContext, in this.s_readAXDataFromReplica, true) ? DatabaseConnectionType.IntentReadOnly : DatabaseConnectionType.Default)))
          buildsByDateAsync2 = await bc.GetBuildsByDateAsync(dataspaceId, batchSize, fromDate);
        buildsByDateAsync1 = buildsByDateAsync2;
      }
      return buildsByDateAsync1;
    }

    public async Task<List<ShallowBuildAnalyticsData>> GetShallowBuildAnaltyticsDataByDateAsync(
      IVssRequestContext requestContext,
      int dataspaceId,
      int batchSize,
      DateTime? fromDate)
    {
      List<ShallowBuildAnalyticsData> analtyticsDataByDateAsync1;
      using (requestContext.TraceScope(nameof (BuildAnalyticsService), nameof (GetShallowBuildAnaltyticsDataByDateAsync)))
      {
        List<ShallowBuildAnalyticsData> analtyticsDataByDateAsync2;
        using (Build2Component bc = requestContext.CreateComponent<Build2Component>(connectionType: new DatabaseConnectionType?(requestContext.GetService<IVssRegistryService>().GetValue<bool>(requestContext, in this.s_readAXDataFromReplica, true) ? DatabaseConnectionType.IntentReadOnly : DatabaseConnectionType.Default)))
          analtyticsDataByDateAsync2 = await bc.GetShallowBuildAnaltyticsDataByDateAsync(dataspaceId, batchSize, fromDate);
        analtyticsDataByDateAsync1 = analtyticsDataByDateAsync2;
      }
      return analtyticsDataByDateAsync1;
    }

    public async Task<List<BuildDefinitionAnalyticsData>> GetBuildDefinitionsByDateAsync(
      IVssRequestContext requestContext,
      int dataspaceId,
      int batchSize,
      DateTime? fromDate)
    {
      List<BuildDefinitionAnalyticsData> definitionsByDateAsync1;
      using (requestContext.TraceScope(nameof (BuildAnalyticsService), nameof (GetBuildDefinitionsByDateAsync)))
      {
        List<BuildDefinitionAnalyticsData> definitionsByDateAsync2;
        using (Build2Component bc = requestContext.CreateComponent<Build2Component>(connectionType: new DatabaseConnectionType?(requestContext.GetService<IVssRegistryService>().GetValue<bool>(requestContext, in this.s_readAXDataFromReplica, true) ? DatabaseConnectionType.IntentReadOnly : DatabaseConnectionType.Default)))
          definitionsByDateAsync2 = await bc.GetBuildDefinitionsByDateAsync(dataspaceId, batchSize, fromDate);
        definitionsByDateAsync1 = definitionsByDateAsync2;
      }
      return definitionsByDateAsync1;
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }
  }
}

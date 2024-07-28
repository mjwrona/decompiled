// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.GeoReplication.IGeoReplicationService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server.GeoReplication
{
  [DefaultServiceImplementation(typeof (OnPremGeoReplicationService))]
  public interface IGeoReplicationService : IVssFrameworkService
  {
    List<GeoReplica> QueryReplicas(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties properties);

    void SetGeoReplicationState(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties properties,
      bool synchronous);

    string FailoverDatabase(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties properties,
      bool controlledFailOver,
      bool allowDataLoss);

    void WaitForDatabaseCopy(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties properties);

    bool IsPrimaryInstance(IVssRequestContext requestContext);

    GeoReplicationMode GetGeoReplicationMode(IVssRequestContext requestContext);

    string GetSecondaryAzureInstanceHost(IVssRequestContext requestContext);
  }
}

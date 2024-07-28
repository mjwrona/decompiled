// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.GeoReplication.OnPremGeoReplicationService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server.GeoReplication
{
  internal class OnPremGeoReplicationService : IGeoReplicationService, IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public List<GeoReplica> QueryReplicas(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties properties)
    {
      return new List<GeoReplica>();
    }

    public void SetGeoReplicationState(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties properties,
      bool synchronous)
    {
      throw new InvalidOperationException();
    }

    public string FailoverDatabase(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties properties,
      bool controlledFailOver,
      bool allowDataLoss)
    {
      throw new InvalidOperationException();
    }

    public void WaitForDatabaseCopy(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties properties)
    {
    }

    public bool IsPrimaryInstance(IVssRequestContext requestContext) => true;

    public GeoReplicationMode GetGeoReplicationMode(IVssRequestContext requestContext) => GeoReplicationMode.None;

    public string GetSecondaryAzureInstanceHost(IVssRequestContext requestContext) => (string) null;
  }
}

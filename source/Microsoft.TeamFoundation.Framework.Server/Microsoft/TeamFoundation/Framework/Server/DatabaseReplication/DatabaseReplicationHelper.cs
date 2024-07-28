// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseReplication.DatabaseReplicationHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server.GeoReplication;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server.DatabaseReplication
{
  public static class DatabaseReplicationHelper
  {
    public static (List<GeoReplica>, string) GetDatabaseReplicationRaw(
      ISqlConnectionInfo connectionInfo,
      ITFLogger logger)
    {
      string serverName;
      using (TeamFoundationDataTierComponent componentRaw = connectionInfo.CreateComponentRaw<TeamFoundationDataTierComponent>())
        serverName = componentRaw.GetServerName();
      List<GeoReplica> source;
      using (GeoReplicationRawComponent componentRaw = connectionInfo.CreateComponentRaw<GeoReplicationRawComponent>())
        source = componentRaw.QueryReplicas();
      if (source.Any<GeoReplica>((Func<GeoReplica, bool>) (r => !r.IsPrimary)))
        throw new InvalidOperationException("Database " + connectionInfo.InitialCatalog + " on Server " + serverName + " is not primary. Server " + source.First<GeoReplica>((Func<GeoReplica, bool>) (r => !r.IsPrimary)).PartnerServer + " is primary.");
      return (source, serverName);
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.GeoReplicationLinkStatus
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class GeoReplicationLinkStatus
  {
    public Guid LinkGuid { get; set; }

    public string PartnerServer { get; set; }

    public string PartnerDatabase { get; set; }

    public DateTimeOffset LastReplication { get; set; }

    public int ReplicationLagSec { get; set; }

    public byte ReplicationState { get; set; }

    public string ReplicationStateDescription { get; set; }

    public byte Role { get; set; }

    public string RoleDescription { get; set; }

    public byte SecondaryAllowConnections { get; set; }

    public string SecondaryAllowConnectionsDescription { get; set; }

    public DateTimeOffset LastCommit { get; set; }
  }
}

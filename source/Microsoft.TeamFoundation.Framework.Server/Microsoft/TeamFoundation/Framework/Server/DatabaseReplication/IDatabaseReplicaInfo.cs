// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseReplication.IDatabaseReplicaInfo
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using System;

namespace Microsoft.TeamFoundation.Framework.Server.DatabaseReplication
{
  public interface IDatabaseReplicaInfo
  {
    string DatabaseName { get; }

    ISqlConnectionInfo DataTierConnectionInfo { get; }

    TeamFoundationDatabaseCredential NewCredential { get; }

    string ReplicationState { get; }

    int ReplicationLagSeconds { get; }

    bool RegisterCredential(
      IVssRequestContext requestContext,
      string loginName,
      string loginPassword,
      string credentialName,
      ITFLogger logger);

    bool UpdateCredential(IVssRequestContext requestContext, ITFLogger logger);

    bool UnregisterCredential(
      IVssRequestContext requestContext,
      TeamFoundationDatabaseCredential credential,
      ITFLogger logger);

    void UpdateDatabaseProperties(
      IVssRequestContext requestContext,
      ITFLogger logger,
      Action<TeamFoundationDatabaseProperties> action);
  }
}

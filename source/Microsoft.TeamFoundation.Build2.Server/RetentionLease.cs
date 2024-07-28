// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.RetentionLease
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class RetentionLease
  {
    internal RetentionLease(
      int id,
      string ownerId,
      int runId,
      int definitionId,
      DateTime createdOn,
      DateTime validUntil,
      bool highPriority)
    {
      this.OwnerId = ownerId;
      this.RunId = runId;
      this.DefinitionId = definitionId;
      this.Id = id;
      this.CreatedOn = createdOn;
      this.ValidUntil = validUntil;
      this.HighPriority = highPriority;
    }

    public static implicit operator MinimalRetentionLease(RetentionLease lease) => new MinimalRetentionLease(lease.OwnerId, new int?(lease.RunId), new int?(lease.DefinitionId));

    public string OwnerId { get; }

    public int RunId { get; }

    public int DefinitionId { get; }

    public int Id { get; }

    public DateTime CreatedOn { get; }

    public DateTime ValidUntil { get; }

    public bool HighPriority { get; }
  }
}

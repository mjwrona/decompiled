// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabasePartition
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class DatabasePartition
  {
    internal DatabasePartition(
      int partitionId,
      Guid serviceHostId,
      DatabasePartitionState state,
      DateTime stateChangedDate,
      TeamFoundationHostType hostType)
    {
      this.PartitionId = partitionId;
      this.ServiceHostId = serviceHostId;
      this.State = state;
      this.StateChangedDate = stateChangedDate;
      this.HostType = hostType;
    }

    public int PartitionId { get; private set; }

    public Guid ServiceHostId { get; private set; }

    public DatabasePartitionState State { get; private set; }

    public DateTime StateChangedDate { get; private set; }

    public TeamFoundationHostType HostType { get; private set; }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "PartitionId: {0}; ServiceHostId: {1}; State: {2}; StateChangedDate: {3}; HostType: {4}", (object) this.PartitionId, (object) this.ServiceHostId, (object) this.State, (object) this.StateChangedDate, (object) this.HostType);
  }
}

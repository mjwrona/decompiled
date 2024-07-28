// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ContextInfoData
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Licensing;
using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public struct ContextInfoData
  {
    public ContextInfoData(
      int sequenceNumber,
      Guid activityId,
      Guid uniqueIdentifier,
      Guid hostId,
      Guid userId,
      ContextType type,
      bool isGoverned,
      int partitionId,
      AccountLicenseType accountLicenseType,
      string objectName)
    {
      this.SequenceNumber = sequenceNumber;
      this.ActivityId = activityId;
      this.UniqueIdentifier = uniqueIdentifier;
      this.HostId = hostId;
      this.UserId = userId;
      this.Type = type;
      this.IsGoverned = isGoverned;
      this.PartitionId = partitionId;
      this.AccountLicenseType = accountLicenseType;
      this.ObjectName = objectName;
    }

    public int SequenceNumber { get; private set; }

    public Guid ActivityId { get; private set; }

    public Guid UniqueIdentifier { get; private set; }

    public Guid HostId { get; private set; }

    public Guid UserId { get; private set; }

    public ContextType Type { get; private set; }

    public bool IsGoverned { get; private set; }

    public int PartitionId { get; private set; }

    public AccountLicenseType AccountLicenseType { get; private set; }

    public string ObjectName { get; private set; }
  }
}

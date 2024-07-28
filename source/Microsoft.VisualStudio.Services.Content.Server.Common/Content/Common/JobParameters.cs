// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.JobParameters
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using Microsoft.VisualStudio.Services.BlobStore.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  [Serializable]
  public class JobParameters
  {
    private const string RunIdDateFormat = "yyyy-MM-ddTHH:mm:ssZ";

    public int PartitionId { get; set; }

    public int TotalPartitions { get; set; }

    public string RunId { get; set; }

    public string DomainId { get; set; } = WellKnownDomainIds.DefaultDomainId.Serialize();

    [IgnoreDataMember]
    public DateTimeOffset RunDateTime => DateTimeOffset.ParseExact(this.RunId, "yyyy-MM-ddTHH:mm:ssZ", (IFormatProvider) null);

    [Obsolete("This method is obsolete. Call CreateNew with domain id parameter instead.", false)]
    public static JobParameters CreateNew(int partitionId, int totalPartitions) => new JobParameters()
    {
      PartitionId = partitionId,
      TotalPartitions = totalPartitions,
      RunId = UtcClock.Instance.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
    };

    public static JobParameters CreateNew(int partitionId, int totalPartitions, IDomainId domainId) => new JobParameters()
    {
      PartitionId = partitionId,
      TotalPartitions = totalPartitions,
      DomainId = domainId.Serialize(),
      RunId = UtcClock.Instance.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
    };
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingJobStats
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DataContract(Namespace = "", Name = "ServicingJobStats")]
  public class ServicingJobStats : IExtensibleDataObject
  {
    [DataMember(Name = "successCount", Order = 0)]
    public int SuccessCount { get; set; }

    [DataMember(Name = "failedCount", Order = 1)]
    public int FailedCount { get; set; }

    [DataMember(Name = "queuedCount", Order = 2)]
    public int QueuedCount { get; set; }

    [DataMember(Name = "inProgressCount", Order = 3)]
    public int InProgressCount { get; set; }

    [DataMember(Name = "averageSuccessfulMilliseconds", Order = 4)]
    public int AverageSuccessfulMilliseconds { get; set; }

    [DataMember(Name = "averageFailedMilliseconds", Order = 6)]
    public int AverageFailedMilliseconds { get; set; }

    [DataMember(Name = "averageRunningMilliseconds", Order = 7)]
    public int AverageRunningMilliseconds { get; set; }

    [DataMember(Name = "averageQueueWaitMilliseconds", Order = 8)]
    public int AverageQueueWaitMilliseconds { get; set; }

    ExtensionDataObject IExtensibleDataObject.ExtensionData { get; set; }
  }
}

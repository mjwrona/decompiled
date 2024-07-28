// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties.DepotIndexInfo
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties
{
  [DataContract]
  public class DepotIndexInfo
  {
    public DepotIndexInfo(
      long lastIndexedChangeId,
      DateTime lastIndexedChangeUtcTime,
      DateTime lastIndexedCommitTime)
    {
      this.LastIndexedChangeId = lastIndexedChangeId;
      this.LastIndexedChangeUtcTime = lastIndexedChangeUtcTime;
      this.LastIndexedCommitTime = lastIndexedCommitTime;
    }

    [DataMember(Order = 1)]
    public long LastIndexedChangeId { get; set; }

    [DataMember(Order = 2)]
    public DateTime LastIndexedChangeUtcTime { get; set; }

    [DataMember(Order = 3)]
    public DateTime LastIndexedCommitTime { get; set; }
  }
}

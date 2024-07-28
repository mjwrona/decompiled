// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamStatistics
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  [DataContract]
  public class UpstreamStatistics
  {
    public UpstreamStatistics(Guid id, UpstreamSourceType type, int waitTimeMs)
    {
      this.Id = id;
      this.Type = type;
      this.WaitTimeMs = waitTimeMs;
    }

    [DataMember]
    public Guid Id { get; }

    [DataMember]
    public UpstreamSourceType Type { get; }

    [DataMember]
    public int WaitTimeMs { get; set; }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.WebApi.Internal.PackageMetricsData
// Assembly: Microsoft.VisualStudio.Services.Feed.WebApi.Internal, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4BC34C1F-0F07-4DDD-8B37-907579B359F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.WebApi.Internal.dll

using Microsoft.VisualStudio.Services.Feed.WebApi.Internal.Converters;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Feed.WebApi.Internal
{
  [DataContract]
  [JsonConverter(typeof (PackageMetricsDataConverter))]
  public class PackageMetricsData
  {
    [DataMember]
    public string Protocol { get; set; }

    [DataMember]
    public Guid FeedId { get; set; }

    [DataMember]
    public Guid? ProjectId { get; set; }

    [DataMember]
    public string NormalizedPackageName { get; set; }

    [DataMember]
    public string NormalizedPackageVersion { get; set; }

    [DataMember]
    public RawMetricType RawMetricType { get; set; }

    [DataMember]
    public RawMetricData RawMetricData { get; set; }

    public string GetKey() => string.Format("{0}/{1}/{2}/{3}/{4}/{5}", (object) this.Protocol, (object) this.ProjectId, (object) this.FeedId, (object) this.NormalizedPackageName, (object) this.NormalizedPackageVersion, (object) this.RawMetricData.GetMetricKey());
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.PartitionKeyRange
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Microsoft.Azure.Documents.Routing;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;

namespace Microsoft.Azure.Documents
{
  internal sealed class PartitionKeyRange : Resource, IEquatable<PartitionKeyRange>
  {
    internal const string MasterPartitionKeyRangeId = "M";

    [JsonProperty(PropertyName = "minInclusive")]
    internal string MinInclusive
    {
      get => this.GetValue<string>("minInclusive");
      set => this.SetValue("minInclusive", (object) value);
    }

    [JsonProperty(PropertyName = "maxExclusive")]
    internal string MaxExclusive
    {
      get => this.GetValue<string>("maxExclusive");
      set => this.SetValue("maxExclusive", (object) value);
    }

    [JsonProperty(PropertyName = "ridPrefix")]
    internal int? RidPrefix
    {
      get => this.GetValue<int?>("ridPrefix");
      set => this.SetValue("ridPrefix", (object) value);
    }

    [JsonProperty(PropertyName = "throughputFraction")]
    internal double ThroughputFraction
    {
      get => this.GetValue<double>("throughputFraction");
      set => this.SetValue("throughputFraction", (object) value);
    }

    [JsonProperty(PropertyName = "status")]
    internal PartitionKeyRangeStatus Status
    {
      get => this.GetValue<PartitionKeyRangeStatus>("status");
      set => this.SetValue("status", (object) value);
    }

    [JsonProperty(PropertyName = "parents")]
    public Collection<string> Parents
    {
      get => this.GetValue<Collection<string>>("parents");
      set => this.SetValue("parents", (object) value);
    }

    internal Range<string> ToRange() => new Range<string>(this.MinInclusive, this.MaxExclusive, true, false);

    public override bool Equals(object obj) => this.Equals(obj as PartitionKeyRange);

    public override int GetHashCode() => ((0 * 397 ^ this.Id.GetHashCode()) * 397 ^ this.MinInclusive.GetHashCode()) * 397 ^ this.MaxExclusive.GetHashCode();

    public bool Equals(PartitionKeyRange other) => other != null && this.Id == other.Id && this.MinInclusive.Equals(other.MinInclusive) && this.MaxExclusive.Equals(other.MaxExclusive) && this.ThroughputFraction == other.ThroughputFraction;
  }
}

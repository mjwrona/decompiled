// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Tracing.TraceData.ConsistencyConfig
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Azure.Cosmos.Tracing.TraceData
{
  internal class ConsistencyConfig
  {
    private readonly Lazy<string> lazyString;
    private readonly Lazy<string> lazyJsonString;

    public ConsistencyConfig(
      Microsoft.Azure.Cosmos.ConsistencyLevel? consistencyLevel,
      IReadOnlyList<string> preferredRegions,
      string applicationRegion)
    {
      ConsistencyConfig consistencyConfig = this;
      this.ConsistencyLevel = consistencyLevel;
      this.PreferredRegions = preferredRegions;
      this.ApplicationRegion = applicationRegion;
      this.lazyString = new Lazy<string>((Func<string>) (() =>
      {
        CultureInfo invariantCulture = CultureInfo.InvariantCulture;
        ref Microsoft.Azure.Cosmos.ConsistencyLevel? local = ref consistencyLevel;
        string str1 = (local.HasValue ? local.GetValueOrDefault().ToString() : (string) null) ?? "NotSet";
        string str2 = ConsistencyConfig.PreferredRegionsInternal(preferredRegions);
        string str3 = applicationRegion;
        return string.Format((IFormatProvider) invariantCulture, "(consistency: {0}, prgns:[{1}], apprgn: {2})", (object) str1, (object) str2, (object) str3);
      }));
      this.lazyJsonString = new Lazy<string>((Func<string>) (() => JsonConvert.SerializeObject((object) consistencyConfig)));
    }

    public Microsoft.Azure.Cosmos.ConsistencyLevel? ConsistencyLevel { get; }

    public IReadOnlyList<string> PreferredRegions { get; }

    public string ApplicationRegion { get; }

    public override string ToString() => this.lazyString.Value;

    public string ToJsonString() => this.lazyJsonString.Value;

    private static string PreferredRegionsInternal(IReadOnlyList<string> applicationPreferredRegions) => applicationPreferredRegions == null ? string.Empty : string.Join(", ", (IEnumerable<string>) applicationPreferredRegions);
  }
}

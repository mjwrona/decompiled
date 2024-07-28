// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeedPolicy
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class ChangeFeedPolicy
  {
    [JsonProperty(PropertyName = "retentionDuration")]
    private int retentionDurationInMinutes;

    [JsonIgnore]
    public TimeSpan FullFidelityRetention
    {
      get => TimeSpan.FromMinutes((double) this.retentionDurationInMinutes);
      set
      {
        if (value.Seconds > 0 || value.Milliseconds > 0)
          throw new ArgumentOutOfRangeException(nameof (FullFidelityRetention), "Retention's granularity is minutes.");
        this.retentionDurationInMinutes = value.TotalMilliseconds >= 0.0 ? (int) value.TotalMinutes : throw new ArgumentOutOfRangeException(nameof (FullFidelityRetention), "Retention cannot be negative.");
      }
    }

    public static TimeSpan FullFidelityNoRetention => TimeSpan.Zero;

    [JsonExtensionData]
    internal IDictionary<string, JToken> AdditionalProperties { get; private set; }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.HttpTimeoutPolicyDefault
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class HttpTimeoutPolicyDefault : HttpTimeoutPolicy
  {
    public static readonly HttpTimeoutPolicy Instance = (HttpTimeoutPolicy) new HttpTimeoutPolicyDefault();
    private static readonly string Name = nameof (HttpTimeoutPolicyDefault);
    private readonly IReadOnlyList<(TimeSpan requestTimeout, TimeSpan delayForNextRequest)> TimeoutsAndDelays = (IReadOnlyList<(TimeSpan, TimeSpan)>) new List<(TimeSpan, TimeSpan)>()
    {
      (TimeSpan.FromSeconds(65.0), TimeSpan.Zero),
      (TimeSpan.FromSeconds(65.0), TimeSpan.FromSeconds(1.0)),
      (TimeSpan.FromSeconds(65.0), TimeSpan.Zero)
    };

    private HttpTimeoutPolicyDefault()
    {
    }

    public override string TimeoutPolicyName => HttpTimeoutPolicyDefault.Name;

    public override TimeSpan MaximumRetryTimeLimit => CosmosHttpClient.GatewayRequestTimeout;

    public override int TotalRetryCount => this.TimeoutsAndDelays.Count;

    public override IEnumerator<(TimeSpan requestTimeout, TimeSpan delayForNextRequest)> GetTimeoutEnumerator() => this.TimeoutsAndDelays.GetEnumerator();

    public override bool IsSafeToRetry(HttpMethod httpMethod) => httpMethod == HttpMethod.Get;

    public override bool ShouldRetryBasedOnResponse(
      HttpMethod requestHttpMethod,
      HttpResponseMessage responseMessage)
    {
      return false;
    }
  }
}

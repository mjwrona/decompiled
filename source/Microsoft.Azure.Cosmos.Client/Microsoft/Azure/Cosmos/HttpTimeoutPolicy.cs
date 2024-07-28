// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.HttpTimeoutPolicy
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Microsoft.Azure.Cosmos
{
  internal abstract class HttpTimeoutPolicy
  {
    public abstract string TimeoutPolicyName { get; }

    public abstract TimeSpan MaximumRetryTimeLimit { get; }

    public abstract int TotalRetryCount { get; }

    public abstract IEnumerator<(TimeSpan requestTimeout, TimeSpan delayForNextRequest)> GetTimeoutEnumerator();

    public abstract bool IsSafeToRetry(HttpMethod httpMethod);

    public abstract bool ShouldRetryBasedOnResponse(
      HttpMethod requestHttpMethod,
      HttpResponseMessage responseMessage);

    public static HttpTimeoutPolicy GetTimeoutPolicy(DocumentServiceRequest documentServiceRequest) => documentServiceRequest.ResourceType == ResourceType.Document && documentServiceRequest.OperationType == OperationType.QueryPlan || documentServiceRequest.ResourceType == ResourceType.PartitionKeyRange ? HttpTimeoutPolicyControlPlaneRetriableHotPath.Instance : HttpTimeoutPolicyDefault.Instance;
  }
}

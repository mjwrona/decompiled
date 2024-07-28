// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.GoneAndRetryRequestRetryPolicyContext
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System;

namespace Microsoft.Azure.Documents
{
  internal sealed class GoneAndRetryRequestRetryPolicyContext
  {
    public bool ForceRefresh { get; set; }

    public bool IsInRetry { get; set; }

    public TimeSpan RemainingTimeInMsOnClientRequest { get; set; }

    public int ClientRetryCount { get; set; }

    public int RegionRerouteAttemptCount { get; set; }

    public TimeSpan TimeoutForInBackoffRetryPolicy { get; set; }
  }
}

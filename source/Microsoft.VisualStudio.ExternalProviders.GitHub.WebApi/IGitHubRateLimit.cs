// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.IGitHubRateLimit
// Assembly: Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4FE25D33-B783-4B98-BAFC-7E522D8D8D08
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.dll

using System;

namespace Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi
{
  public interface IGitHubRateLimit
  {
    int Cost { get; set; }

    int Limit { get; set; }

    int NodeCount { get; set; }

    int Remaining { get; set; }

    DateTime ResetAt { get; set; }
  }
}

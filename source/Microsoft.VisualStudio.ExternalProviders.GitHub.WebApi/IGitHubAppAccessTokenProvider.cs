// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.IGitHubAppAccessTokenProvider
// Assembly: Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4FE25D33-B783-4B98-BAFC-7E522D8D8D08
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.dll

using System;
using System.ComponentModel.Composition;

namespace Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi
{
  [InheritedExport]
  public interface IGitHubAppAccessTokenProvider : IDisposable
  {
    void Initialize(object requestContext);

    void Initialize(object requestContext, string projectId, string endpointId);

    string CreateEncodedAppAccessToken();

    GitHubAppType AppType { get; }
  }
}

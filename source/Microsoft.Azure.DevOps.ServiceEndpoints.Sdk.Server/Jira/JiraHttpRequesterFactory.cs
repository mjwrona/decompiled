// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Jira.JiraHttpRequesterFactory
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Net.Http;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Jira
{
  internal sealed class JiraHttpRequesterFactory : IExternalProviderHttpRequesterFactory
  {
    private const string c_area = "ExternalProviders";
    private const string c_layer = "JiraHttpRequesterFactory";
    private readonly TimeSpan timeout;

    public JiraHttpRequesterFactory(TimeSpan timeout) => this.timeout = timeout;

    public string ProviderType => "jira";

    public void Initialize(object requestContext)
    {
    }

    public IExternalProviderHttpRequester GetRequester(HttpMessageHandler httpMessageHandler) => (IExternalProviderHttpRequester) new JiraHttpClientRequester(httpMessageHandler, this.timeout);
  }
}

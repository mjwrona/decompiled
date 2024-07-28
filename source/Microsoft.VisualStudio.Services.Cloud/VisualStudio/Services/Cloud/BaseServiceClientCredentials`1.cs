// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.BaseServiceClientCredentials`1
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Rest;
using Microsoft.TeamFoundation.Common;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public abstract class BaseServiceClientCredentials<TCredentials> : ServiceClientCredentials where TCredentials : ServiceClientCredentials
  {
    private Lazy<TCredentials> m_credentials;

    protected BaseServiceClientCredentials(bool useManagedIdentity, ITFLogger logger)
    {
      this.UseManagedIdentity = useManagedIdentity;
      this.Logger = logger;
      this.m_credentials = new Lazy<TCredentials>(new Func<TCredentials>(this.CreateCredentials));
    }

    public override void InitializeServiceClient<T>(ServiceClient<T> client) => this.m_credentials.Value.InitializeServiceClient<T>(client);

    public override Task ProcessHttpRequestAsync(
      HttpRequestMessage request,
      CancellationToken cancellationToken)
    {
      return this.m_credentials.Value.ProcessHttpRequestAsync(request, cancellationToken);
    }

    private TCredentials CreateCredentials()
    {
      if (!AzureRoleUtil.IsAvailable)
        return this.CreateDeploymentAgentCredentials();
      return !this.UseManagedIdentity ? this.CreateRuntimeServicePrincipalCredentials() : this.CreateManagedIdentityCredentials();
    }

    protected bool UseManagedIdentity { get; }

    protected ITFLogger Logger { get; }

    protected abstract TCredentials CreateDeploymentAgentCredentials();

    protected abstract TCredentials CreateRuntimeServicePrincipalCredentials();

    protected abstract TCredentials CreateManagedIdentityCredentials();
  }
}

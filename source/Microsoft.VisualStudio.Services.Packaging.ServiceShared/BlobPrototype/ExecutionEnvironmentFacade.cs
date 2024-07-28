// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.ExecutionEnvironmentFacade
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Organization;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class ExecutionEnvironmentFacade : IExecutionEnvironment
  {
    private readonly IVssRequestContext requestContext;

    public ExecutionEnvironmentFacade(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public Guid ActivityId => this.requestContext.ActivityId;

    public Guid HostId => this.requestContext.ServiceHost.InstanceId;

    public DateTime RequestStartTime => this.requestContext.StartTime();

    public bool IsHosted() => this.requestContext.ExecutionEnvironment.IsHostedDeployment;

    public bool IsHostType(TeamFoundationHostType hostType) => this.requestContext.ServiceHost.Is(hostType);

    public bool IsDevFabric() => this.requestContext.ExecutionEnvironment.IsDevFabricDeployment;

    public bool IsOrganizationAadBacked() => this.requestContext.IsOrganizationAadBacked();

    public Guid GetOrganizationAadTenantId() => this.requestContext.GetOrganizationAadTenantId();

    public bool IsHostProcessType(HostProcessType type) => this.requestContext.IsHostProcessType(type);

    public bool IsUnauthenticatedWebRequest()
    {
      if (!(this.requestContext.RootContext is IVssWebRequestContext))
        return false;
      string authenticationMechanism = this.requestContext.GetAuthenticationMechanism();
      return string.IsNullOrWhiteSpace(authenticationMechanism) || authenticationMechanism.Equals("None", StringComparison.OrdinalIgnoreCase) || authenticationMechanism.StartsWith("None!!", StringComparison.OrdinalIgnoreCase);
    }

    public bool IsCollectionInMicrosoftTenant() => this.requestContext.IsMicrosoftTenant();
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Ssh.Server.ExtensibilityProviders.RequestContextProvider
// Assembly: Microsoft.TeamFoundation.Ssh.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9CD799E8-FCEB-494E-B317-B5A120D16BCC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Ssh.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Ssh.Server.Core.ExtensibilityProviders;
using System;

namespace Microsoft.TeamFoundation.Ssh.Server.ExtensibilityProviders
{
  internal class RequestContextProvider : IVssRequestContextProvider
  {
    private readonly IVssDeploymentServiceHost m_deploymentHost;

    public RequestContextProvider(IVssDeploymentServiceHost deploymentServiceHost) => this.m_deploymentHost = deploymentServiceHost;

    public IVssRequestContext CreateSystemDeploymentContext() => this.m_deploymentHost.CreateSystemContext();

    public IVssRequestContext CreateSshDeploymentContext(
      IVssRequestContext systemDeploymentRC,
      string userAgent,
      string clientAddress,
      Guid? e2eId)
    {
      return SshRequestContext.CreateSshDeploymentRequestContext(systemDeploymentRC, userAgent, clientAddress, e2eId);
    }
  }
}

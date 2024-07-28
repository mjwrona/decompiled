// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Machine.BuildServiceClientCredentials
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Client;
using System.IdentityModel.Selectors;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Security;

namespace Microsoft.TeamFoundation.Build.Machine
{
  internal class BuildServiceClientCredentials : SecurityCredentialsManager, IEndpointBehavior
  {
    public BuildServiceClientCredentials()
      : this(BuildServiceClientCredentials.GetClientAuthCertificates())
    {
    }

    public BuildServiceClientCredentials(X509Certificate2Collection clientCertificates) => this.ClientCertificates = clientCertificates;

    private static X509Certificate2Collection GetClientAuthCertificates() => ClientCertificateManager.GetClientAuthCertificates(StoreLocation.LocalMachine);

    public X509Certificate2Collection ClientCertificates { get; set; }

    public X509Certificate2 CurrentCertificate { get; set; }

    public override SecurityTokenManager CreateSecurityTokenManager()
    {
      if (this.ClientCertificates != null && this.ClientCertificates.Count > 0)
        this.CurrentCertificate = this.ClientCertificates[0];
      return (SecurityTokenManager) new BuildServiceClientSecurityTokenManager(this);
    }

    public void AddBindingParameters(
      ServiceEndpoint endpoint,
      BindingParameterCollection bindingParameters)
    {
      bindingParameters.Add((object) this);
    }

    public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
    {
    }

    public void ApplyDispatchBehavior(
      ServiceEndpoint endpoint,
      EndpointDispatcher endpointDispatcher)
    {
    }

    public void Validate(ServiceEndpoint endpoint)
    {
    }
  }
}

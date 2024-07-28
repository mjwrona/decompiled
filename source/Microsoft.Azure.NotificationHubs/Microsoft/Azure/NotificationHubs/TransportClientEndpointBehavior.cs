// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.TransportClientEndpointBehavior
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Microsoft.Azure.NotificationHubs
{
  public sealed class TransportClientEndpointBehavior : IEndpointBehavior
  {
    private TokenProvider tokenProvider;

    public TransportClientEndpointBehavior() => this.CredentialType = TransportClientCredentialType.Unauthenticated;

    public TransportClientEndpointBehavior(TokenProvider tokenProvider)
      : this()
    {
      this.TokenProvider = tokenProvider;
    }

    public TokenProvider TokenProvider
    {
      get => this.CredentialType == TransportClientCredentialType.TokenProvider ? this.tokenProvider : (TokenProvider) null;
      set => this.UpdateCredentials(value);
    }

    internal TransportClientCredentialType CredentialType { get; set; }

    void IEndpointBehavior.AddBindingParameters(
      ServiceEndpoint endpoint,
      BindingParameterCollection bindingParameters)
    {
      bindingParameters.Add((object) this);
    }

    void IEndpointBehavior.ApplyClientBehavior(
      ServiceEndpoint endpoint,
      ClientRuntime clientRuntime)
    {
    }

    void IEndpointBehavior.ApplyDispatchBehavior(
      ServiceEndpoint endpoint,
      EndpointDispatcher endpointDispatcher)
    {
    }

    void IEndpointBehavior.Validate(ServiceEndpoint endpoint)
    {
    }

    private void UpdateCredentials(TokenProvider newTokenProvider)
    {
      if (newTokenProvider != null)
      {
        this.CredentialType = TransportClientCredentialType.TokenProvider;
        this.tokenProvider = newTokenProvider;
      }
      else
      {
        this.CredentialType = TransportClientCredentialType.Unauthenticated;
        this.tokenProvider = (TokenProvider) null;
      }
    }
  }
}

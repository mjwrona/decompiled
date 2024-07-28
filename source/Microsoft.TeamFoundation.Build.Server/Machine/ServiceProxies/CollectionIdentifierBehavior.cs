// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Machine.ServiceProxies.CollectionIdentifierBehavior
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Microsoft.TeamFoundation.Build.Machine.ServiceProxies
{
  internal class CollectionIdentifierBehavior : IEndpointBehavior
  {
    private Guid m_applicationId;
    private Guid m_collectionId;
    internal const string HeaderNamespace = "tfsb";
    internal const string ApplicationInstanceIdHeader = "ApplicationInstanceId";
    internal const string CollectionInstanceIdHeader = "CollectionInstanceId";
    internal const string InternalCallerIdHeader = "InternalCallerId";

    public CollectionIdentifierBehavior(Guid applicationId, Guid collectionId)
    {
      this.m_applicationId = applicationId;
      this.m_collectionId = collectionId;
    }

    public void AddBindingParameters(
      ServiceEndpoint endpoint,
      BindingParameterCollection bindingParameters)
    {
    }

    public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime) => clientRuntime.MessageInspectors.Add((IClientMessageInspector) new CollectionIdentifierBehavior.CollectionIdentifierClientInspector(this.m_applicationId, this.m_collectionId));

    public void Validate(ServiceEndpoint endpoint)
    {
    }

    public void ApplyDispatchBehavior(
      ServiceEndpoint endpoint,
      EndpointDispatcher endpointDispatcher)
    {
    }

    private class CollectionIdentifierClientInspector : IClientMessageInspector
    {
      private Guid m_collectionInstanceId;
      private Guid m_applicationInstanceId;

      public CollectionIdentifierClientInspector(Guid applicationId, Guid collectionId)
      {
        this.m_collectionInstanceId = collectionId;
        this.m_applicationInstanceId = applicationId;
      }

      public void AfterReceiveReply(ref Message reply, object correlationState)
      {
      }

      public object BeforeSendRequest(ref Message request, IClientChannel channel)
      {
        MessageHeader<Guid> messageHeader1 = new MessageHeader<Guid>(this.m_collectionInstanceId);
        MessageHeader<Guid> messageHeader2 = new MessageHeader<Guid>(this.m_applicationInstanceId);
        request.Headers.Add(messageHeader1.GetUntypedHeader("CollectionInstanceId", "tfsb"));
        request.Headers.Add(messageHeader2.GetUntypedHeader("ApplicationInstanceId", "tfsb"));
        return (object) null;
      }
    }
  }
}

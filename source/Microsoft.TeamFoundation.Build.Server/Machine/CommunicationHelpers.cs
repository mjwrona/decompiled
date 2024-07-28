// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Machine.CommunicationHelpers
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security.Tokens;
using System.Xml;

namespace Microsoft.TeamFoundation.Build.Machine
{
  internal static class CommunicationHelpers
  {
    public static Binding CreateBinding(SecurityMode securityMode, bool requireClientCertificates)
    {
      WSHttpBinding wsHttpBinding = new WSHttpBinding(securityMode);
      wsHttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
      wsHttpBinding.ReaderQuotas = XmlDictionaryReaderQuotas.Max;
      wsHttpBinding.MaxReceivedMessageSize = (long) XmlDictionaryReaderQuotas.Max.MaxStringContentLength;
      Binding binding = (Binding) wsHttpBinding;
      if (securityMode == SecurityMode.Transport & requireClientCertificates)
      {
        wsHttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;
        BindingElementCollection bindingElements = wsHttpBinding.CreateBindingElements();
        TransportSecurityBindingElement securityBindingElement = new TransportSecurityBindingElement();
        X509SecurityTokenParameters securityTokenParameters = new X509SecurityTokenParameters();
        securityTokenParameters.InclusionMode = SecurityTokenInclusionMode.AlwaysToRecipient;
        securityBindingElement.EndpointSupportingTokenParameters.Endorsing.Add((SecurityTokenParameters) securityTokenParameters);
        bindingElements.Insert(bindingElements.Count - 1, (BindingElement) securityBindingElement);
        binding = (Binding) new CustomBinding((IEnumerable<BindingElement>) bindingElements);
      }
      return binding;
    }

    public static void Shutdown(ICommunicationObject communicationObject)
    {
      if (communicationObject == null)
        return;
      try
      {
        communicationObject.BeginClose(new AsyncCallback(CommunicationHelpers.ShutdownCallback), (object) communicationObject);
      }
      catch (TimeoutException ex)
      {
        communicationObject.Abort();
      }
      catch (CommunicationException ex)
      {
        communicationObject.Abort();
      }
    }

    private static void ShutdownCallback(IAsyncResult result)
    {
      ICommunicationObject asyncState = result.AsyncState as ICommunicationObject;
      try
      {
        asyncState.EndClose(result);
      }
      catch (TimeoutException ex)
      {
        asyncState.Abort();
      }
      catch (CommunicationException ex)
      {
        asyncState.Abort();
      }
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.WebService.Client.ConnectorIncidentManagerClient
// Assembly: Microsoft.AzureAd.Icm.WebService.Client, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 85C23930-39A1-49EE-A03A-507264E2FE4B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.WebService.Client.dll

using Microsoft.AzureAd.Icm.Types;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.AzureAd.Icm.WebService.Client
{
  public class ConnectorIncidentManagerClient : 
    ConnectorIncidentManagerClientBase<IConnectorIncidentManager>,
    IConnectorIncidentManager
  {
    public ConnectorIncidentManagerClient()
    {
    }

    public ConnectorIncidentManagerClient(string endpointConfigurationName)
      : base(endpointConfigurationName)
    {
    }

    public ConnectorIncidentManagerClient(string endpointConfigurationName, string remoteAddress)
      : base(endpointConfigurationName, remoteAddress)
    {
    }

    public ConnectorIncidentManagerClient(
      string endpointConfigurationName,
      string remoteAddress,
      bool includeMachineNameInUserAgent)
      : base(endpointConfigurationName, remoteAddress, includeMachineNameInUserAgent)
    {
    }

    public ConnectorIncidentManagerClient(
      string endpointConfigurationName,
      EndpointAddress remoteAddress)
      : base(endpointConfigurationName, remoteAddress)
    {
    }

    public ConnectorIncidentManagerClient(Binding binding, EndpointAddress remoteAddress)
      : base(binding, remoteAddress)
    {
    }
  }
}

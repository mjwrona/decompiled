// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.WebService.Client.TaskBasedConnectorClient
// Assembly: Microsoft.AzureAd.Icm.WebService.Client, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 85C23930-39A1-49EE-A03A-507264E2FE4B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.WebService.Client.dll

using Microsoft.AzureAd.Icm.Types;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

namespace Microsoft.AzureAd.Icm.WebService.Client
{
  public class TaskBasedConnectorClient : 
    ConnectorIncidentManagerClientBase<ITaskBasedConnector>,
    ITaskBasedConnector,
    IConnectorIncidentManager
  {
    public TaskBasedConnectorClient()
    {
    }

    public TaskBasedConnectorClient(string endpointConfigurationName)
      : base(endpointConfigurationName)
    {
    }

    public TaskBasedConnectorClient(string endpointConfigurationName, string remoteAddress)
      : base(endpointConfigurationName, remoteAddress)
    {
    }

    public TaskBasedConnectorClient(
      string endpointConfigurationName,
      string remoteAddress,
      bool includeMachineNameInUserAgent)
      : base(endpointConfigurationName, remoteAddress, includeMachineNameInUserAgent)
    {
    }

    public TaskBasedConnectorClient(string endpointConfigurationName, EndpointAddress remoteAddress)
      : base(endpointConfigurationName, remoteAddress)
    {
    }

    public TaskBasedConnectorClient(Binding binding, EndpointAddress remoteAddress)
      : base(binding, remoteAddress)
    {
    }

    public Task<IncidentAddUpdateResult> AddOrUpdateIncident2Async(
      Guid connectorId,
      AlertSourceIncident incident,
      RoutingOptions routingOptions)
    {
      return this.ExecuteIcmWebMethod<Task<IncidentAddUpdateResult>>(connectorId, (Func<ITaskBasedConnector, Task<IncidentAddUpdateResult>>) (channel => channel.AddOrUpdateIncident2Async(connectorId, incident, routingOptions)), nameof (AddOrUpdateIncident2Async));
    }

    public Task<IncidentAddUpdateResult> AddOrUpdateIncident3Async(
      Guid connectorId,
      long siloId,
      AlertSourceIncident incident,
      RoutingOptions routingOptions)
    {
      return this.ExecuteIcmWebMethod<Task<IncidentAddUpdateResult>>(connectorId, (Func<ITaskBasedConnector, Task<IncidentAddUpdateResult>>) (channel => channel.AddOrUpdateIncident3Async(connectorId, siloId, incident, routingOptions)), nameof (AddOrUpdateIncident3Async));
    }

    public Task<IncidentChangeRequestResult> GetIncidentChanges3Async(
      string instanceName,
      Guid connectorId)
    {
      return this.ExecuteIcmWebMethod<Task<IncidentChangeRequestResult>>(connectorId, (Func<ITaskBasedConnector, Task<IncidentChangeRequestResult>>) (channel => channel.GetIncidentChanges3Async(instanceName, connectorId)), nameof (GetIncidentChanges3Async));
    }

    public Task<IncidentChangeRequestResult> GetIncidentChanges4Async(
      string instanceName,
      Guid connectorId,
      int maxChanges,
      int maxOwnershipMins)
    {
      return this.ExecuteIcmWebMethod<Task<IncidentChangeRequestResult>>(connectorId, (Func<ITaskBasedConnector, Task<IncidentChangeRequestResult>>) (channel => channel.GetIncidentChanges4Async(instanceName, connectorId, maxChanges, maxOwnershipMins)), nameof (GetIncidentChanges4Async));
    }

    public Task<IncidentChangeRequestResult> GetIncidentChanges5Async(
      string instanceName,
      Guid connectorId,
      int maxChanges,
      int maxOwnershipMins)
    {
      return this.ExecuteIcmWebMethod<Task<IncidentChangeRequestResult>>(connectorId, (Func<ITaskBasedConnector, Task<IncidentChangeRequestResult>>) (channel => channel.GetIncidentChanges5Async(instanceName, connectorId, maxChanges, maxOwnershipMins)), nameof (GetIncidentChanges5Async));
    }

    public Task<IncidentChangeRequestResult> GetIncidentChanges6Async(
      string instanceName,
      Guid connectorId,
      int maxChanges,
      int maxOwnershipMins)
    {
      return this.ExecuteIcmWebMethod<Task<IncidentChangeRequestResult>>(connectorId, (Func<ITaskBasedConnector, Task<IncidentChangeRequestResult>>) (channel => channel.GetIncidentChanges6Async(instanceName, connectorId, maxChanges, maxOwnershipMins)), nameof (GetIncidentChanges6Async));
    }

    public Task MarkChangesHandled2Async(
      string instanceName,
      IEnumerable<ChangeHandleResult> changeIds,
      Guid connectorId)
    {
      return this.ExecuteIcmWebMethod<Task>(connectorId, (Func<ITaskBasedConnector, Task>) (channel => channel.MarkChangesHandled2Async(instanceName, changeIds, connectorId)), nameof (MarkChangesHandled2Async));
    }

    public Task<Guid> CreateConnectorConfigurationEntryAsync(Guid connectorId, string value) => this.ExecuteIcmWebMethod<Task<Guid>>(connectorId, (Func<ITaskBasedConnector, Task<Guid>>) (channel => channel.CreateConnectorConfigurationEntryAsync(connectorId, value)), nameof (CreateConnectorConfigurationEntryAsync));

    public Task UpdateConnectorConfigurationEntryAsync(
      Guid connectorId,
      Guid configId,
      string value)
    {
      return this.ExecuteIcmWebMethod<Task>(connectorId, (Func<ITaskBasedConnector, Task>) (channel => channel.UpdateConnectorConfigurationEntryAsync(connectorId, configId, value)), nameof (UpdateConnectorConfigurationEntryAsync));
    }

    public Task<string> GetConnectorConfigurationEntryAsync(Guid connectorId, Guid configId) => this.ExecuteIcmWebMethod<Task<string>>(connectorId, (Func<ITaskBasedConnector, Task<string>>) (channel => channel.GetConnectorConfigurationEntryAsync(connectorId, configId)), nameof (GetConnectorConfigurationEntryAsync));

    public Task LogConnectorStatusAsync(Guid connectorId, ConnectorStatusType type, string message) => this.ExecuteIcmWebMethod<Task>(connectorId, (Func<ITaskBasedConnector, Task>) (channel => channel.LogConnectorStatusAsync(connectorId, type, message)), nameof (LogConnectorStatusAsync));

    public Task<IEnumerable<IncidentAlertSourceInfo>> GetIncidentAlertSourceInfo2Async(
      Guid connectorId,
      IEnumerable<string> sourceIds)
    {
      return this.ExecuteIcmWebMethod<Task<IEnumerable<IncidentAlertSourceInfo>>>(connectorId, (Func<ITaskBasedConnector, Task<IEnumerable<IncidentAlertSourceInfo>>>) (channel => channel.GetIncidentAlertSourceInfo2Async(connectorId, sourceIds)), nameof (GetIncidentAlertSourceInfo2Async));
    }
  }
}

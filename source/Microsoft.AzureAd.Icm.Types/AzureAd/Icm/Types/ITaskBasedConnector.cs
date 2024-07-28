// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.ITaskBasedConnector
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Microsoft.AzureAd.Icm.Types
{
  [ServiceContract]
  public interface ITaskBasedConnector : IConnectorIncidentManager
  {
    [OperationContract(Action = "http://tempuri.org/IConnectorIncidentManager/AddOrUpdateIncident3", ReplyAction = "http://tempuri.org/IConnectorIncidentManager/AddOrUpdateIncident3Response")]
    Task<IncidentAddUpdateResult> AddOrUpdateIncident3Async(
      Guid connectorId,
      long siloId,
      AlertSourceIncident incident,
      RoutingOptions routingOptions);

    [OperationContract(Action = "http://tempuri.org/IConnectorIncidentManager/AddOrUpdateIncident2", ReplyAction = "http://tempuri.org/IConnectorIncidentManager/AddOrUpdateIncident2Response")]
    Task<IncidentAddUpdateResult> AddOrUpdateIncident2Async(
      Guid connectorId,
      AlertSourceIncident incident,
      RoutingOptions routingOptions);

    [OperationContract(Action = "http://tempuri.org/IConnectorIncidentManager/GetIncidentChanges3", ReplyAction = "http://tempuri.org/IConnectorIncidentManager/GetIncidentChanges3Response")]
    Task<IncidentChangeRequestResult> GetIncidentChanges3Async(
      string instanceName,
      Guid connectorId);

    [OperationContract(Action = "http://tempuri.org/IConnectorIncidentManager/GetIncidentChanges4", ReplyAction = "http://tempuri.org/IConnectorIncidentManager/GetIncidentChanges4Response")]
    Task<IncidentChangeRequestResult> GetIncidentChanges4Async(
      string instanceName,
      Guid connectorId,
      int maxChanges,
      int maxOwnershipMins);

    [OperationContract(Action = "http://tempuri.org/IConnectorIncidentManager/GetIncidentChanges5", ReplyAction = "http://tempuri.org/IConnectorIncidentManager/GetIncidentChanges5Response")]
    Task<IncidentChangeRequestResult> GetIncidentChanges5Async(
      string instanceName,
      Guid connectorId,
      int maxChanges,
      int maxOwnershipMins);

    [OperationContract(Action = "http://tempuri.org/IConnectorIncidentManager/GetIncidentChanges6", ReplyAction = "http://tempuri.org/IConnectorIncidentManager/GetIncidentChanges6Response")]
    Task<IncidentChangeRequestResult> GetIncidentChanges6Async(
      string instanceName,
      Guid connectorId,
      int maxChanges,
      int maxOwnershipMins);

    [OperationContract(Action = "http://tempuri.org/IConnectorIncidentManager/MarkChangesHandled2", ReplyAction = "http://tempuri.org/IConnectorIncidentManager/MarkChangesHandled2Response")]
    Task MarkChangesHandled2Async(
      string instanceName,
      IEnumerable<ChangeHandleResult> changeIds,
      Guid connectorId);

    [OperationContract(Action = "http://tempuri.org/IConnectorIncidentManager/CreateConnectorConfigurationEntry", ReplyAction = "http://tempuri.org/IConnectorIncidentManager/CreateConnectorConfigurationEntryResponse")]
    Task<Guid> CreateConnectorConfigurationEntryAsync(Guid connectorId, string value);

    [OperationContract(Action = "http://tempuri.org/IConnectorIncidentManager/UpdateConnectorConfigurationEntry", ReplyAction = "http://tempuri.org/IConnectorIncidentManager/UpdateConnectorConfigurationEntryResponse")]
    Task UpdateConnectorConfigurationEntryAsync(Guid connectorId, Guid configId, string value);

    [OperationContract(Action = "http://tempuri.org/IConnectorIncidentManager/GetConnectorConfigurationEntry", ReplyAction = "http://tempuri.org/IConnectorIncidentManager/GetConnectorConfigurationEntryResponse")]
    Task<string> GetConnectorConfigurationEntryAsync(Guid connectorId, Guid configId);

    [OperationContract(Action = "http://tempuri.org/IConnectorIncidentManager/LogConnectorStatus", ReplyAction = "http://tempuri.org/IConnectorIncidentManager/LogConnectorStatusResponse")]
    Task LogConnectorStatusAsync(Guid connectorId, ConnectorStatusType type, string message);

    [OperationContract(Action = "http://tempuri.org/IConnectorIncidentManager/GetIncidentAlertSourceInfo2", ReplyAction = "http://tempuri.org/IConnectorIncidentManager/GetIncidentAlertSourceInfo2Response")]
    Task<IEnumerable<IncidentAlertSourceInfo>> GetIncidentAlertSourceInfo2Async(
      Guid connectorId,
      IEnumerable<string> sourceIds);
  }
}

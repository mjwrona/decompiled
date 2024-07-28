// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.IConnectorIncidentManager
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace Microsoft.AzureAd.Icm.Types
{
  [ServiceContract]
  public interface IConnectorIncidentManager
  {
    [OperationContract]
    [FaultContract(typeof (IcmFault))]
    IncidentAddUpdateResult AddOrUpdateIncident3(
      Guid connectorId,
      long siloId,
      AlertSourceIncident incident,
      RoutingOptions routingOptions);

    [OperationContract]
    [FaultContract(typeof (IcmFault))]
    IncidentAddUpdateResult AddOrUpdateIncident2(
      Guid connectorId,
      AlertSourceIncident incident,
      RoutingOptions routingOptions);

    [OperationContract]
    [FaultContract(typeof (IcmFault))]
    [Obsolete("This method has been deprecated. Use AddOrUpdateIncident2 instead.")]
    IncidentAddUpdateResult AddOrUpdateIncident(
      Guid connectorId,
      byte[] syncCookie,
      AlertSourceIncident incident,
      bool allowUpdate);

    [OperationContract]
    [FaultContract(typeof (IcmFault))]
    [Obsolete("This method has been deprecated. Use GetIncidentChanges3 instead.")]
    IncidentChangeRequestResult GetIncidentChanges(Guid connectorId, byte[] syncCookie);

    [OperationContract]
    [FaultContract(typeof (IcmFault))]
    [Obsolete("This method has been deprecated. Use GetIncidentAlertSourceInfo2 instead.")]
    IEnumerable<AlertSourceInfo> GetIncidentAlertSourceInfo(Guid connectorId, IEnumerable<long> ids);

    [OperationContract]
    [FaultContract(typeof (IcmFault))]
    [Obsolete("This method has been deprecated. Use MarkChangesHandled2 instead.")]
    void MarkChangesHandled(Guid connectorId, IEnumerable<Guid> changeIds);

    [OperationContract]
    [FaultContract(typeof (IcmFault))]
    [Obsolete("This method has been deprecated. Use GetIncidentChanges3 instead.")]
    IncidentChangeRequestResult GetIncidentChanges2(string instanceName, Guid connectorId);

    [OperationContract]
    [FaultContract(typeof (IcmFault))]
    IncidentChangeRequestResult GetIncidentChanges3(string instanceName, Guid connectorId);

    [OperationContract]
    [FaultContract(typeof (IcmFault))]
    IncidentChangeRequestResult GetIncidentChanges4(
      string instanceName,
      Guid connectorId,
      int maxChanges,
      int maxOwnershipMins);

    [OperationContract]
    [FaultContract(typeof (IcmFault))]
    IncidentChangeRequestResult GetIncidentChanges5(
      string instanceName,
      Guid connectorId,
      int maxChanges,
      int maxOwnershipMins);

    [OperationContract]
    [FaultContract(typeof (IcmFault))]
    IncidentChangeRequestResult GetIncidentChanges6(
      string instanceName,
      Guid connectorId,
      int maxChanges,
      int maxOwnershipMins);

    [OperationContract]
    [FaultContract(typeof (IcmFault))]
    void MarkChangesHandled2(
      string instanceName,
      IEnumerable<ChangeHandleResult> changeIds,
      Guid connectorId);

    [OperationContract]
    [FaultContract(typeof (IcmFault))]
    Guid CreateConnectorConfigurationEntry(Guid connectorId, string value);

    [OperationContract]
    [FaultContract(typeof (IcmFault))]
    void UpdateConnectorConfigurationEntry(Guid connectorId, Guid configId, string value);

    [OperationContract]
    [FaultContract(typeof (IcmFault))]
    string GetConnectorConfigurationEntry(Guid connectorId, Guid configId);

    [OperationContract]
    [FaultContract(typeof (IcmFault))]
    void LogConnectorStatus(Guid connectorId, ConnectorStatusType type, string message);

    [OperationContract]
    [FaultContract(typeof (IcmFault))]
    IEnumerable<IncidentAlertSourceInfo> GetIncidentAlertSourceInfo2(
      Guid connectorId,
      IEnumerable<string> sourceIds);
  }
}

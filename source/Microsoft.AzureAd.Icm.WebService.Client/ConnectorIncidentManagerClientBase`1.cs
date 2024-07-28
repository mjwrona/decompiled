// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.WebService.Client.ConnectorIncidentManagerClientBase`1
// Assembly: Microsoft.AzureAd.Icm.WebService.Client, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 85C23930-39A1-49EE-A03A-507264E2FE4B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.WebService.Client.dll

using Microsoft.AzureAd.Icm.Types;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.AzureAd.Icm.WebService.Client
{
  public abstract class ConnectorIncidentManagerClientBase<T> : ClientBase<T> where T : class, IConnectorIncidentManager
  {
    private readonly bool includeMachineNameInUserAgent;

    protected ConnectorIncidentManagerClientBase()
    {
    }

    protected ConnectorIncidentManagerClientBase(string endpointConfigurationName)
      : base(endpointConfigurationName)
    {
    }

    protected ConnectorIncidentManagerClientBase(
      string endpointConfigurationName,
      string remoteAddress)
      : this(endpointConfigurationName, remoteAddress, false)
    {
    }

    protected ConnectorIncidentManagerClientBase(
      string endpointConfigurationName,
      string remoteAddress,
      bool includeMachineNameInUserAgent)
      : base(endpointConfigurationName, remoteAddress)
    {
      this.includeMachineNameInUserAgent = includeMachineNameInUserAgent;
    }

    protected ConnectorIncidentManagerClientBase(
      string endpointConfigurationName,
      EndpointAddress remoteAddress)
      : base(endpointConfigurationName, remoteAddress)
    {
    }

    protected ConnectorIncidentManagerClientBase(Binding binding, EndpointAddress remoteAddress)
      : base(binding, remoteAddress)
    {
    }

    public IncidentAddUpdateResult AddOrUpdateIncident2(
      Guid connectorId,
      AlertSourceIncident incident,
      RoutingOptions routingOptions)
    {
      return this.ExecuteIcmWebMethod<IncidentAddUpdateResult>(connectorId, (Func<T, IncidentAddUpdateResult>) (channel => channel.AddOrUpdateIncident2(connectorId, incident, routingOptions)), nameof (AddOrUpdateIncident2));
    }

    public IncidentAddUpdateResult AddOrUpdateIncident3(
      Guid connectorId,
      long siloId,
      AlertSourceIncident incident,
      RoutingOptions routingOptions)
    {
      return this.ExecuteIcmWebMethod<IncidentAddUpdateResult>(connectorId, (Func<T, IncidentAddUpdateResult>) (channel => channel.AddOrUpdateIncident3(connectorId, siloId, incident, routingOptions)), nameof (AddOrUpdateIncident3));
    }

    [Obsolete("This method has been deprecated. Use AddOrUpdateIncident2 instead.")]
    public IncidentAddUpdateResult AddOrUpdateIncident(
      Guid connectorId,
      byte[] syncCookie,
      AlertSourceIncident incident,
      bool allowUpdate)
    {
      return this.ExecuteIcmWebMethod<IncidentAddUpdateResult>(connectorId, (Func<T, IncidentAddUpdateResult>) (channel => channel.AddOrUpdateIncident(connectorId, syncCookie, incident, allowUpdate)), nameof (AddOrUpdateIncident));
    }

    [Obsolete("This method has been deprecated. Use GetIncidentChanges3 instead.")]
    public IncidentChangeRequestResult GetIncidentChanges(Guid connectorId, byte[] syncCookie) => this.ExecuteIcmWebMethod<IncidentChangeRequestResult>(connectorId, (Func<T, IncidentChangeRequestResult>) (channel => channel.GetIncidentChanges(connectorId, syncCookie)), nameof (GetIncidentChanges));

    [Obsolete("This method has been deprecated. Use MarkChangesHandled2 instead.")]
    public void MarkChangesHandled(Guid connectorId, IEnumerable<Guid> changeIds) => this.ExecuteIcmWebMethod(connectorId, (Action<T>) (channel => channel.MarkChangesHandled(connectorId, changeIds)), nameof (MarkChangesHandled));

    [Obsolete("This method has been deprecated. Use GetIncidentChanges3 instead.")]
    public IncidentChangeRequestResult GetIncidentChanges2(string instanceName, Guid connectorId) => this.ExecuteIcmWebMethod<IncidentChangeRequestResult>(connectorId, (Func<T, IncidentChangeRequestResult>) (channel => channel.GetIncidentChanges2(instanceName, connectorId)), nameof (GetIncidentChanges2));

    public IncidentChangeRequestResult GetIncidentChanges3(string instanceName, Guid connectorId) => this.ExecuteIcmWebMethod<IncidentChangeRequestResult>(connectorId, (Func<T, IncidentChangeRequestResult>) (channel => channel.GetIncidentChanges3(instanceName, connectorId)), nameof (GetIncidentChanges3));

    public IncidentChangeRequestResult GetIncidentChanges4(
      string instanceName,
      Guid connectorId,
      int maxChanges,
      int maxOwnershipMins)
    {
      return this.ExecuteIcmWebMethod<IncidentChangeRequestResult>(connectorId, (Func<T, IncidentChangeRequestResult>) (channel => channel.GetIncidentChanges4(instanceName, connectorId, maxChanges, maxOwnershipMins)), nameof (GetIncidentChanges4));
    }

    public IncidentChangeRequestResult GetIncidentChanges5(
      string instanceName,
      Guid connectorId,
      int maxChanges,
      int maxOwnershipMins)
    {
      return this.ExecuteIcmWebMethod<IncidentChangeRequestResult>(connectorId, (Func<T, IncidentChangeRequestResult>) (channel => channel.GetIncidentChanges5(instanceName, connectorId, maxChanges, maxOwnershipMins)), nameof (GetIncidentChanges5));
    }

    public IncidentChangeRequestResult GetIncidentChanges6(
      string instanceName,
      Guid connectorId,
      int maxChanges,
      int maxOwnershipMins)
    {
      return this.ExecuteIcmWebMethod<IncidentChangeRequestResult>(connectorId, (Func<T, IncidentChangeRequestResult>) (channel => channel.GetIncidentChanges6(instanceName, connectorId, maxChanges, maxOwnershipMins)), nameof (GetIncidentChanges6));
    }

    public void MarkChangesHandled2(
      string instanceName,
      IEnumerable<ChangeHandleResult> changeIds,
      Guid connectorId)
    {
      this.ExecuteIcmWebMethod(connectorId, (Action<T>) (channel => channel.MarkChangesHandled2(instanceName, changeIds, connectorId)), nameof (MarkChangesHandled2));
    }

    public Guid CreateConnectorConfigurationEntry(Guid connectorId, string value) => this.ExecuteIcmWebMethod<Guid>(connectorId, (Func<T, Guid>) (channel => channel.CreateConnectorConfigurationEntry(connectorId, value)), nameof (CreateConnectorConfigurationEntry));

    public void UpdateConnectorConfigurationEntry(Guid connectorId, Guid configId, string value) => this.ExecuteIcmWebMethod(connectorId, (Action<T>) (channel => channel.UpdateConnectorConfigurationEntry(connectorId, configId, value)), nameof (UpdateConnectorConfigurationEntry));

    public string GetConnectorConfigurationEntry(Guid connectorId, Guid configId) => this.ExecuteIcmWebMethod<string>(connectorId, (Func<T, string>) (channel => channel.GetConnectorConfigurationEntry(connectorId, configId)), nameof (GetConnectorConfigurationEntry));

    public void LogConnectorStatus(Guid connectorId, ConnectorStatusType type, string message) => this.ExecuteIcmWebMethod(connectorId, (Action<T>) (channel => channel.LogConnectorStatus(connectorId, type, message)), nameof (LogConnectorStatus));

    public IEnumerable<IncidentAlertSourceInfo> GetIncidentAlertSourceInfo2(
      Guid connectorId,
      IEnumerable<string> sourceIds)
    {
      return this.Channel.GetIncidentAlertSourceInfo2(connectorId, sourceIds);
    }

    [Obsolete("This method has been deprecated. Use GetIncidentAlertSourceInfo2 instead.")]
    public IEnumerable<AlertSourceInfo> GetIncidentAlertSourceInfo(
      Guid connectorId,
      IEnumerable<long> ids)
    {
      return this.Channel.GetIncidentAlertSourceInfo(connectorId, ids);
    }

    protected TResult ExecuteIcmWebMethod<TResult>(
      Guid connectorId,
      Func<T, TResult> func,
      [CallerMemberName] string methodName = "Unspecified")
    {
      this.ValidateCertificate();
      using (new OperationContextScope((IContextChannel) this.InnerChannel))
      {
        this.AddUserAgent(connectorId, methodName);
        return func(this.Channel);
      }
    }

    protected void ExecuteIcmWebMethod(Guid connectorId, Action<T> action, [CallerMemberName] string methodName = "Unspecified")
    {
      this.ValidateCertificate();
      using (new OperationContextScope((IContextChannel) this.InnerChannel))
      {
        this.AddUserAgent(connectorId, methodName);
        action(this.Channel);
      }
    }

    private void ValidateCertificate()
    {
      if (this.ClientCredentials == null || this.ClientCredentials.ClientCertificate.Certificate == null)
        throw new InvalidOperationException("Client certificate is not attached to the request. See http://aka.ms/icmConnectorModel for troubleshooting information.");
      X509Certificate2 certificate = this.ClientCredentials.ClientCertificate.Certificate;
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Client certificate [{0}; {1}]", new object[2]
      {
        (object) certificate.SubjectName.Name,
        (object) certificate.Thumbprint
      });
      if (certificate.NotAfter <= DateTime.Now)
        throw new InvalidOperationException(str + " has expired. See http://aka.ms/icmConnectorModel for troubleshooting information.");
      if (!certificate.HasPrivateKey)
        throw new InvalidOperationException(str + " has no private key. See http://aka.ms/icmConnectorModel for troubleshooting information.");
      try
      {
        AsymmetricAlgorithm privateKey = certificate.PrivateKey;
      }
      catch (CryptographicException ex)
      {
        throw new InvalidOperationException("User account has no access to private key of " + str + ". See http://aka.ms/icmConnectorModel for troubleshooting information.", (Exception) ex);
      }
    }

    private void AddUserAgent(Guid connectorId, string methodName)
    {
      string str = string.Format("ConnectorId,{0},MethodName,{1},MachineName,{2}", (object) connectorId, (object) methodName, this.includeMachineNameInUserAgent ? (object) Environment.MachineName : (object) "Unspecified");
      OperationContext.Current.OutgoingMessageProperties.Add(HttpRequestMessageProperty.Name, (object) new HttpRequestMessageProperty()
      {
        Headers = {
          {
            HttpRequestHeader.UserAgent,
            str
          }
        }
      });
    }
  }
}

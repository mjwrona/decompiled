// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.IcM.AddIncidentAction
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.AzureAd.Icm.Types;
using Microsoft.AzureAd.Icm.WebService.Client;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceEndpoints.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.IcM
{
  [Export(typeof (ConsumerActionImplementation))]
  public class AddIncidentAction : ConsumerActionImplementation
  {
    private const string c_id = "addIncident";
    private const string certHeader = "-----BEGIN CERTIFICATE-----";
    private const string certFooter = "-----END CERTIFICATE-----";
    private const string certBindingName = "IcmBindingConfigCert";
    private const string icmUrlFormat = "{0}/connector2/ConnectorIncidentManager.svc";
    private const string originDeploymentConstant = "Deployment";
    public const string OwningTeamIdInputId = "owningTeamId";
    public const string RoutingIdInputId = "routingId";
    public const string IncidentDescriptionInputId = "incidentDescription";
    public const string SeverityInputId = "severity";
    private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
    private static readonly string[] s_supportedEventTypes = new string[1]
    {
      "ms.vss-release.deployment-completed-event"
    };
    private static readonly string[] s_resourceVersion30Preview1 = new string[1]
    {
      "3.0-preview.1"
    };
    private static readonly IDictionary<string, string[]> s_supportedResourceVersions = (IDictionary<string, string[]>) new Dictionary<string, string[]>()
    {
      {
        "ms.vss-release.deployment-completed-event",
        AddIncidentAction.s_resourceVersion30Preview1
      }
    };
    private static readonly IDictionary<string, Func<JObject, string, string, string, string, AlertSourceIncident>> s_eventAttachmentBuilders = (IDictionary<string, Func<JObject, string, string, string, string, AlertSourceIncident>>) new Dictionary<string, Func<JObject, string, string, string, string, AlertSourceIncident>>()
    {
      {
        "ms.vss-release.deployment-completed-event",
        new Func<JObject, string, string, string, string, AlertSourceIncident>(AddIncidentAction.CreateAlertSourceObject)
      }
    };

    public static string ConsumerActionId => "addIncident";

    public override string Id => "addIncident";

    public override string ConsumerId => "icm";

    public override string Name => IcMConsumerResources.AddIncidentActionName;

    public override string Description => IcMConsumerResources.AddIncidentActionDescription;

    public override string[] SupportedEventTypes => AddIncidentAction.s_supportedEventTypes;

    public override IDictionary<string, string[]> SupportedResourceVersions => AddIncidentAction.s_supportedResourceVersions;

    public override bool AllowResourceVersionOverride => false;

    public override IList<InputDescriptor> InputDescriptors => (IList<InputDescriptor>) new List<InputDescriptor>()
    {
      new InputDescriptor()
      {
        Name = IcMConsumerResources.AddIncidentAction_RoutingIdInputName,
        Description = IcMConsumerResources.AddIncidentAction_RoutingIdInputDescription,
        InputMode = InputMode.TextBox,
        Id = "routingId",
        IsConfidential = false,
        Validation = new InputValidation()
        {
          DataType = InputDataType.String,
          IsRequired = true
        }
      },
      new InputDescriptor()
      {
        Name = IcMConsumerResources.AddIncidentAction_OwningTeamIdInputName,
        Description = IcMConsumerResources.AddIncidentAction_OwningTeamIdInputDescription,
        InputMode = InputMode.TextBox,
        Id = "owningTeamId",
        IsConfidential = false,
        Validation = new InputValidation()
        {
          DataType = InputDataType.String,
          IsRequired = false
        }
      },
      new InputDescriptor()
      {
        Name = IcMConsumerResources.AddIncidentAction_IncidentDescriptionInputName,
        Description = IcMConsumerResources.AddIncidentAction_IncidentDescriptionInputDescription,
        InputMode = InputMode.TextBox,
        Id = "incidentDescription",
        IsConfidential = false,
        Validation = new InputValidation()
        {
          DataType = InputDataType.String,
          IsRequired = false
        }
      },
      new InputDescriptor()
      {
        Name = IcMConsumerResources.AddIncidentAction_SeverityInputName,
        Description = IcMConsumerResources.AddIncidentAction_SeverityInputDescription,
        InputMode = InputMode.Combo,
        Values = new InputValues()
        {
          PossibleValues = (IList<InputValue>) new List<InputValue>()
          {
            new InputValue() { Value = "1", DisplayValue = "1" },
            new InputValue() { Value = "2", DisplayValue = "2" },
            new InputValue() { Value = "3", DisplayValue = "3" },
            new InputValue() { Value = "4", DisplayValue = "4" }
          },
          IsLimitedToPossibleValues = true
        },
        Id = "severity",
        IsConfidential = false,
        Validation = new InputValidation()
        {
          DataType = InputDataType.String,
          IsRequired = true
        }
      }
    };

    public ActionTask HandleEvent(
      IVssRequestContext requestContext,
      Event raisedEvent,
      HandleEventArgs eventArgs)
    {
      string requestMessage = string.Empty;
      string responseMessage = string.Empty;
      if (AddIncidentAction.s_eventAttachmentBuilders.ContainsKey(raisedEvent.EventType))
      {
        IncidentAddUpdateResult incidentAddUpdateResult = new IncidentAddUpdateResult();
        string consumerInput1 = eventArgs.Notification.GetConsumerInput("url", true);
        string consumerInput2 = eventArgs.Notification.GetConsumerInput("connectorId", true);
        string consumerInput3 = eventArgs.Notification.GetConsumerInput("icmCert", true);
        string consumerInput4 = eventArgs.Notification.GetConsumerInput("icmPvtKey", true);
        string consumerInput5 = eventArgs.Notification.GetConsumerInput("severity", true);
        string consumerInput6 = eventArgs.Notification.GetConsumerInput("incidentDescription");
        string consumerInput7 = eventArgs.Notification.GetConsumerInput("routingId", true);
        string consumerInput8 = eventArgs.Notification.GetConsumerInput("owningTeamId");
        ArgumentUtility.CheckStringForNullOrEmpty(consumerInput1, "url");
        ArgumentUtility.CheckStringForNullOrEmpty(consumerInput3, "icmCert");
        if (!(raisedEvent.Resource is JObject resource) && raisedEvent.Resource != null)
          resource = JObject.FromObject(raisedEvent.Resource, JsonSerializer.Create(CommonConsumerSettings.JsonSerializerSettings));
        AlertSourceIncident alertSourceObject = AddIncidentAction.CreateAlertSourceObject(resource, consumerInput5, consumerInput6, consumerInput8, consumerInput7);
        Guid connectorId = new Guid(consumerInput2);
        ConnectorIncidentManagerClient connectorClient = AddIncidentAction.CreateConnectorClient(raisedEvent, consumerInput1, consumerInput3, consumerInput4);
        IncidentAddUpdateResult result;
        try
        {
          result = connectorClient.AddOrUpdateIncident2(connectorId, alertSourceObject, RoutingOptions.None);
        }
        catch (Exception ex)
        {
          throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, IcMConsumerResources.ErrorWhileAddingIncident, (object) ex.Message), ex.InnerException);
        }
        requestMessage = AddIncidentAction.BuildRequestMessage(resource);
        responseMessage = AddIncidentAction.BuildResponseMessage(result);
      }
      return (ActionTask) new NoopActionTask(requestMessage, responseMessage);
    }

    private static ConnectorIncidentManagerClient CreateConnectorClient(
      Event raisedEvent,
      string icmWebServiceBaseUrl,
      string certificateData,
      string privateKeyData)
    {
      try
      {
        string uri = string.Format("{0}/connector2/ConnectorIncidentManager.svc", (object) icmWebServiceBaseUrl);
        WS2007HttpBinding ws2007HttpBinding = new WS2007HttpBinding(SecurityMode.Transport);
        ws2007HttpBinding.Name = "IcmBindingConfigCert";
        ws2007HttpBinding.MaxBufferPoolSize = 4194304L;
        ws2007HttpBinding.MaxReceivedMessageSize = 16777216L;
        ws2007HttpBinding.Security.Transport.Realm = string.Empty;
        ws2007HttpBinding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.None;
        ws2007HttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;
        ws2007HttpBinding.ReaderQuotas.MaxArrayLength = 16384;
        ws2007HttpBinding.ReaderQuotas.MaxBytesPerRead = 1048576;
        ws2007HttpBinding.ReaderQuotas.MaxStringContentLength = 1048576;
        ws2007HttpBinding.Security.Message.EstablishSecurityContext = false;
        ws2007HttpBinding.Security.Message.NegotiateServiceCredential = true;
        ws2007HttpBinding.Security.Message.AlgorithmSuite = SecurityAlgorithmSuite.Default;
        ws2007HttpBinding.Security.Message.ClientCredentialType = MessageCredentialType.Certificate;
        ConnectorIncidentManagerClient connectorClient = new ConnectorIncidentManagerClient((Binding) ws2007HttpBinding, new EndpointAddress(uri));
        X509Certificate2 clientCertificate = AddIncidentAction.GetClientCertificate(certificateData, privateKeyData);
        if (connectorClient.ClientCredentials != null)
          connectorClient.ClientCredentials.ClientCertificate.Certificate = clientCertificate;
        return connectorClient;
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, IcMConsumerResources.ErrorWhileCreatingIcMClient, (object) ex.Message), ex.InnerException);
      }
    }

    private static X509Certificate2 GetClientCertificate(string clientCert, string clientKey)
    {
      try
      {
        RSAParameters parameters = new RSAParameters();
        if (!string.IsNullOrWhiteSpace(clientKey))
          parameters = new RsaUtils().GetRsaParameters(clientKey);
        RSACryptoServiceProvider cryptoServiceProvider = new RSACryptoServiceProvider(new CspParameters()
        {
          KeyContainerName = Guid.NewGuid().ToString(),
          KeyNumber = 1,
          Flags = CspProviderFlags.NoFlags
        });
        cryptoServiceProvider.ImportParameters(parameters);
        if (clientCert.IndexOf("-----BEGIN CERTIFICATE-----") >= 0)
        {
          int startIndex = clientCert.IndexOf("-----BEGIN CERTIFICATE-----") + "-----BEGIN CERTIFICATE-----".Length;
          int num = clientCert.IndexOf("-----END CERTIFICATE-----");
          clientCert = clientCert.Substring(startIndex, num - startIndex).Trim();
        }
        return new X509Certificate2(new X509Certificate2(Convert.FromBase64String(clientCert))
        {
          PrivateKey = ((AsymmetricAlgorithm) cryptoServiceProvider)
        }.Export(X509ContentType.Pfx), string.Empty);
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, IcMConsumerResources.InvalidCertificateOrPrivateKeyFormat, (object) ex.Message), ex.InnerException);
      }
    }

    private static AlertSourceIncident CreateAlertSourceObject(
      JObject resource,
      string severity,
      string userDescription,
      string owningteamId,
      string routingId)
    {
      JToken jtokenFromJobject = ReleaseHelper.GetJTokenFromJObject(resource, "deployment");
      JToken childTokenFromJtoken = ReleaseHelper.GetChildTokenFromJToken(ReleaseHelper.GetJTokenFromJObject(resource, "environment"), "owner");
      DateTime utcNow = DateTime.UtcNow;
      AlertSourceInfo alertSourceInfo = new AlertSourceInfo();
      alertSourceInfo.Origin = "Deployment";
      alertSourceInfo.CreatedBy = AddIncidentAction.GetOwnerName(childTokenFromJtoken);
      alertSourceInfo.CreateDate = utcNow;
      alertSourceInfo.IncidentId = Guid.NewGuid().ToString("N");
      alertSourceInfo.ModifiedDate = utcNow;
      IncidentLocation incidentLocation1 = new IncidentLocation();
      IncidentLocation incidentLocation2 = new IncidentLocation();
      IncidentStatus incidentStatus = IncidentStatus.Active;
      AlertSourceIncident alertSourceObject = new AlertSourceIncident();
      alertSourceObject.Source = alertSourceInfo;
      alertSourceObject.RoutingId = routingId;
      alertSourceObject.OwningTeamId = owningteamId;
      alertSourceObject.RaisingLocation = incidentLocation1;
      alertSourceObject.OccurringLocation = incidentLocation2;
      alertSourceObject.Title = AddIncidentAction.GetDeploymentCompletedEventIncidentTitle(jtokenFromJobject);
      alertSourceObject.Status = incidentStatus;
      alertSourceObject.Severity = new int?(int.Parse(severity));
      if (!string.IsNullOrEmpty(userDescription))
        alertSourceObject.DescriptionEntries = new DescriptionEntry[2]
        {
          AddIncidentAction.GetDeploymentCompletedEventDescriptionEntry(jtokenFromJobject, utcNow),
          AddIncidentAction.GetUserDescriptionEntry(childTokenFromJtoken, userDescription, utcNow)
        };
      else
        alertSourceObject.DescriptionEntries = new DescriptionEntry[1]
        {
          AddIncidentAction.GetDeploymentCompletedEventDescriptionEntry(jtokenFromJobject, utcNow)
        };
      return alertSourceObject;
    }

    private static string GetDeploymentCompletedEventIncidentTitle(JToken deployment) => string.Format(IcMConsumerResources.AddIncidentAction_DeployementCompletedEventIncidentTitle, (object) ReleaseHelper.GetDeploymentStatus(deployment));

    private static string GetOwnerName(JToken owner)
    {
      string ownerUniqueName = ReleaseHelper.GetOwnerUniqueName(owner);
      if (!string.IsNullOrEmpty(ownerUniqueName))
        return ownerUniqueName;
      string ownerDisplayName = ReleaseHelper.GetOwnerDisplayName(owner);
      ArgumentUtility.CheckStringForNullOrEmpty(ownerDisplayName, "ownerDisplayName");
      return ownerDisplayName;
    }

    private static DescriptionEntry GetDeploymentCompletedEventDescriptionEntry(
      JToken deployment,
      DateTime date)
    {
      string empty = string.Empty;
      string deploymentStatus = ReleaseHelper.GetDeploymentStatus(deployment);
      string valueFromDeployment1 = ReleaseHelper.GetReleasePropertyLinkValueFromDeployment(deployment, "release");
      string valueFromDeployment2 = ReleaseHelper.GetReleasePropertyLinkValueFromDeployment(deployment, "releaseEnvironment");
      string str;
      if (string.IsNullOrEmpty(valueFromDeployment1) || string.IsNullOrEmpty(valueFromDeployment2))
        str = IcMConsumerResources.DefaultDeploymentCompletedDescriptionIfDeploymentIsNotThere;
      else
        str = string.Format(IcMConsumerResources.AddIncidentAction_IncidentDescriptionFormat, (object[]) new string[3]
        {
          valueFromDeployment1,
          valueFromDeployment2,
          deploymentStatus
        });
      return new DescriptionEntry()
      {
        Cause = DescriptionEntryCause.Created,
        Date = date,
        ChangedBy = "Azure DevOps",
        SubmitDate = date,
        SubmittedBy = "Azure DevOps",
        RenderType = DescriptionTextRenderType.Html,
        Text = "<html><body>" + str + "</body><html/>"
      };
    }

    private static DescriptionEntry GetUserDescriptionEntry(
      JToken owner,
      string userDescription,
      DateTime date)
    {
      return new DescriptionEntry()
      {
        Cause = DescriptionEntryCause.Created,
        Date = date,
        ChangedBy = ReleaseHelper.GetOwnerDisplayName(owner),
        SubmitDate = date,
        SubmittedBy = ReleaseHelper.GetOwnerDisplayName(owner),
        RenderType = DescriptionTextRenderType.Plaintext,
        Text = userDescription
      };
    }

    private static string BuildRequestMessage(JObject resource)
    {
      string empty = string.Empty;
      JToken jtokenFromJobject = ReleaseHelper.GetJTokenFromJObject(resource, "deployment");
      string valueFromDeployment1 = ReleaseHelper.GetReleasePropertyNameValueFromDeployment(jtokenFromJobject, "release");
      string valueFromDeployment2 = ReleaseHelper.GetReleasePropertyNameValueFromDeployment(jtokenFromJobject, "releaseEnvironment");
      string str;
      if (string.IsNullOrEmpty(valueFromDeployment1) || string.IsNullOrEmpty(valueFromDeployment2))
        str = IcMConsumerResources.DefaultRequestMessageForDeploymentCompletedIfDeploymentIsNotThere;
      else
        str = string.Format(IcMConsumerResources.AddIncidentAction_RequestMessageFormat, (object[]) new string[2]
        {
          valueFromDeployment1,
          valueFromDeployment2
        });
      return str;
    }

    private static string BuildResponseMessage(IncidentAddUpdateResult result)
    {
      try
      {
        string empty = string.Empty;
        return string.Format(IcMConsumerResources.AddIncidentAction_ResponseMessageFormat, (object) result.IncidentId.ToString());
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, IcMConsumerResources.ErrorWhileBuildingResponse, (object) ex.Message), ex.InnerException);
      }
    }
  }
}

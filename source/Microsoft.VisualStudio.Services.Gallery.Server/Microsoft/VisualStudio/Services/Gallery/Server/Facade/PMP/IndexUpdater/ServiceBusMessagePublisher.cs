// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Facade.PMP.IndexUpdater.ServiceBusMessagePublisher
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Azure.Messaging.ServiceBus;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.Server.Facade.PMP.IndexUpdater.PMPEvents;
using Microsoft.VisualStudio.Services.Gallery.Server.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Facade.PMP.IndexUpdater
{
  public class ServiceBusMessagePublisher : IMessagePublisher, IVssFrameworkService
  {
    private const string Layer = "ServiceBusMessagePublisher";
    private int _retryCount = 3;
    private int _delayTime = 10;
    private string _indexUpdaterTopic;
    private string _SyncFunctionTopic;
    private JsonSerializerSettings jsonSerializerSettings;
    private string _primaryServiceBusConnectionString;
    private ServiceBusClient _primaryServiceBusClient;
    private ServiceBusSender _primaryServiceBusSender;
    private ServiceBusSender _primaryServiceBusSenderForSyncFunction;
    private string _secondaryServiceBusConnectionString;
    private ServiceBusClient _secondaryServiceBusClient;
    private ServiceBusSender _secondaryServiceBusSender;
    private ServiceBusSender _secondaryServiceBusSenderForSyncFunction;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      using (systemRequestContext.TraceBlock(12062103, 12062103, "gallery", nameof (ServiceBusMessagePublisher), nameof (ServiceStart)))
      {
        this.jsonSerializerSettings = new JsonSerializerSettings()
        {
          ContractResolver = (IContractResolver) new LowercaseContractResolver(),
          Formatting = Formatting.Indented
        };
        systemRequestContext.GetService<IVssRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.ConfigurationChangeCallback), false, "/Configuration/Service/Gallery/PMP/**");
        systemRequestContext.GetService<ITeamFoundationStrongBoxService>().RegisterNotification(systemRequestContext, new StrongBoxItemChangedCallback(this.StrongboxValueChangeCallback), "ConfigurationSecrets", (IEnumerable<string>) new string[2]
        {
          "PrimaryServiceBusConnectionString",
          "SecondaryServiceBusConnectionString"
        });
        this.LoadVSCodeIndexUpdaterConfiguration(systemRequestContext);
      }
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    private void ConfigurationChangeCallback(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.LoadVSCodeIndexUpdaterConfiguration(requestContext);
    }

    private void StrongboxValueChangeCallback(
      IVssRequestContext requestContext,
      IEnumerable<StrongBoxItemName> itemNames)
    {
      this.LoadVSCodeIndexUpdaterConfiguration(requestContext);
    }

    public async Task PublishEventAsync<T>(IVssRequestContext requestContext, PMPEvent<T> pmpEvent)
    {
      requestContext.CheckDeploymentRequestContext();
      requestContext.TraceEnter(12062103, "gallery", nameof (ServiceBusMessagePublisher), nameof (PublishEventAsync));
      ServiceBusMessage message = new ServiceBusMessage(JsonConvert.SerializeObject((object) ServiceBusMessagePublisher.ToMessageBody<T>(requestContext, pmpEvent), this.jsonSerializerSettings));
      try
      {
        await this._primaryServiceBusSender.SendMessageAsync(message, new CancellationToken());
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12062103, "gallery", nameof (ServiceBusMessagePublisher), ex);
      }
      try
      {
        await this._secondaryServiceBusSender.SendMessageAsync(message, new CancellationToken());
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12062103, "gallery", nameof (ServiceBusMessagePublisher), ex);
      }
      requestContext.TraceLeave(12062103, "gallery", nameof (ServiceBusMessagePublisher), nameof (PublishEventAsync));
      message = (ServiceBusMessage) null;
    }

    public async Task PublishSyncFunctionMessageAsync<T>(
      IVssRequestContext requestContext,
      T messageData)
    {
      requestContext.CheckDeploymentRequestContext();
      requestContext.TraceEnter(12062103, "gallery", nameof (ServiceBusMessagePublisher), nameof (PublishSyncFunctionMessageAsync));
      ServiceBusMessage message = new ServiceBusMessage(JsonConvert.SerializeObject((object) (T) messageData, this.jsonSerializerSettings));
      try
      {
        await this._primaryServiceBusSenderForSyncFunction.SendMessageAsync(message, new CancellationToken());
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12062103, "gallery", nameof (ServiceBusMessagePublisher), ex);
      }
      try
      {
        await this._secondaryServiceBusSenderForSyncFunction.SendMessageAsync(message, new CancellationToken());
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12062103, "gallery", nameof (ServiceBusMessagePublisher), ex);
      }
      requestContext.TraceLeave(12062103, "gallery", nameof (ServiceBusMessagePublisher), nameof (PublishSyncFunctionMessageAsync));
      message = (ServiceBusMessage) null;
    }

    private void LoadVSCodeIndexUpdaterConfiguration(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(12062103, "gallery", nameof (ServiceBusMessagePublisher), nameof (LoadVSCodeIndexUpdaterConfiguration));
      RegistryEntryCollection registryEntryCollection = requestContext.GetService<IVssRegistryService>().ReadEntries(requestContext, (RegistryQuery) "/Configuration/Service/Gallery/PMP/**");
      this._retryCount = registryEntryCollection.GetValueFromPath<int>("/Configuration/Service/Gallery/PMPIntegration/ServiceBusRetryCount", 3);
      this._delayTime = registryEntryCollection.GetValueFromPath<int>("/Configuration/Service/Gallery/PMPIntegration/ServiceBusDelayTime", 2);
      this._indexUpdaterTopic = registryEntryCollection.GetValueFromPath<string>("/Configuration/Service/Gallery/PMP/VSCodeEventsTopic", "vscodeevents");
      this._SyncFunctionTopic = registryEntryCollection.GetValueFromPath<string>("/Configuration/Service/Gallery/PMP/SyncFunctionTopic", "vscodedataImport");
      this.LoadServiceBusConfigurations(requestContext);
    }

    private void LoadPrimaryServiceBusConfiguration(IVssRequestContext requestContext)
    {
      ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
      StrongBoxItemInfo itemInfo = service.GetItemInfo(requestContext, "ConfigurationSecrets", "PrimaryServiceBusConnectionString", true);
      this._primaryServiceBusConnectionString = service.GetString(requestContext, itemInfo.DrawerId, itemInfo.LookupKey);
      if (string.IsNullOrWhiteSpace(this._primaryServiceBusConnectionString))
        throw new Exception("Unable to read primary serviceBus connection string from strong box item info for key:PrimaryServiceBusConnectionString");
      this._primaryServiceBusClient = new ServiceBusClient(this._primaryServiceBusConnectionString, new ServiceBusClientOptions()
      {
        RetryOptions = new ServiceBusRetryOptions()
        {
          Mode = (ServiceBusRetryMode) 1,
          Delay = TimeSpan.FromSeconds((double) this._delayTime),
          MaxRetries = this._retryCount
        }
      });
      this._primaryServiceBusSender = this._primaryServiceBusClient.CreateSender(this._indexUpdaterTopic);
      this._primaryServiceBusSenderForSyncFunction = this._primaryServiceBusClient.CreateSender(this._SyncFunctionTopic);
    }

    private void LoadSecondaryServiceBusConfiguration(IVssRequestContext requestContext)
    {
      ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
      StrongBoxItemInfo itemInfo = service.GetItemInfo(requestContext, "ConfigurationSecrets", "SecondaryServiceBusConnectionString", true);
      this._secondaryServiceBusConnectionString = service.GetString(requestContext, itemInfo.DrawerId, itemInfo.LookupKey);
      if (string.IsNullOrWhiteSpace(this._secondaryServiceBusConnectionString))
        throw new Exception("Unable to read secondary serviceBus connection string from strong box item info for key:SecondaryServiceBusConnectionString");
      this._secondaryServiceBusClient = new ServiceBusClient(this._secondaryServiceBusConnectionString, new ServiceBusClientOptions()
      {
        RetryOptions = new ServiceBusRetryOptions()
        {
          Mode = (ServiceBusRetryMode) 1,
          Delay = TimeSpan.FromSeconds((double) this._delayTime),
          MaxRetries = this._retryCount
        }
      });
      this._secondaryServiceBusSender = this._secondaryServiceBusClient.CreateSender(this._indexUpdaterTopic);
      this._secondaryServiceBusSenderForSyncFunction = this._secondaryServiceBusClient.CreateSender(this._SyncFunctionTopic);
    }

    private void LoadServiceBusConfigurations(IVssRequestContext requestContext)
    {
      this.LoadPrimaryServiceBusConfiguration(requestContext);
      this.LoadSecondaryServiceBusConfiguration(requestContext);
    }

    private static MessageBody<T> ToMessageBody<T>(
      IVssRequestContext requestContext,
      PMPEvent<T> pmpEvent)
    {
      return new MessageBody<T>()
      {
        ID = pmpEvent.Id,
        Data = pmpEvent.Data,
        Source = pmpEvent.Source,
        Type = pmpEvent.Type,
        CorrelationId = requestContext.ActivityId,
        Time = (DateTimeOffset) DateTime.UtcNow,
        Version = pmpEvent.Version
      };
    }
  }
}

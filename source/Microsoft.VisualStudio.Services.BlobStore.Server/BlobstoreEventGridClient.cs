// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.BlobstoreEventGridClient
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Rest;
using Microsoft.TeamFoundation.Framework.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public class BlobstoreEventGridClient : IBlobstoreEventGridClient, IVssFrameworkService
  {
    private static readonly RegistryQuery RegistrySettingsTopicHostnameQuery = (RegistryQuery) "/Configuration/BlobStore/EventGridClient/TopicHostname";
    private IEventGridClient eventGridClient;
    private string topicHostname;

    public Task PublishEventsAsync(
      IList<EventGridEvent> events,
      CancellationToken cancellationToken)
    {
      IEventGridClient eventGridClient = this.eventGridClient;
      string topicHostname = this.topicHostname;
      IList<EventGridEvent> eventGridEventList = events;
      if (eventGridEventList == null)
        throw new ArgumentNullException(nameof (events));
      CancellationToken cancellationToken1 = cancellationToken;
      return EventGridClientExtensions.PublishEventsAsync(eventGridClient, topicHostname, eventGridEventList, cancellationToken1);
    }

    private void OnTopicKeyUpdated(
      IVssRequestContext requestContext,
      IEnumerable<StrongBoxItemName> itemNames)
    {
      Volatile.Write<IEventGridClient>(ref this.eventGridClient, this.CreateEventGridClient(requestContext));
    }

    private void OnTopicHostnameUpdated(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      Volatile.Write<string>(ref this.topicHostname, this.GetTopicHostname(requestContext));
      IEventGridClient eventGridClient = this.eventGridClient;
      this.RegisterStrongBoxTopicKeyNotification(requestContext);
      Interlocked.CompareExchange<IEventGridClient>(ref this.eventGridClient, this.CreateEventGridClient(requestContext), eventGridClient);
    }

    private void RegisterStrongBoxTopicKeyNotification(IVssRequestContext requestContext)
    {
      IVssRequestContext deploymentRequestContext = requestContext.GetElevatedDeploymentRequestContext();
      deploymentRequestContext.GetService<ITeamFoundationStrongBoxService>().RegisterNotification(deploymentRequestContext, new StrongBoxItemChangedCallback(this.OnTopicKeyUpdated), "ConfigurationSecrets", (IEnumerable<string>) new string[1]
      {
        this.topicHostname
      });
    }

    private IEventGridClient CreateEventGridClient(IVssRequestContext requestContext)
    {
      IVssRequestContext deploymentRequestContext = requestContext.GetElevatedDeploymentRequestContext();
      ITeamFoundationStrongBoxService service = deploymentRequestContext.GetService<ITeamFoundationStrongBoxService>();
      StrongBoxItemInfo itemInfo = service.GetItemInfo(deploymentRequestContext, "ConfigurationSecrets", this.topicHostname, true);
      return (IEventGridClient) new EventGridClient((ServiceClientCredentials) new TopicCredentials(service.GetString(deploymentRequestContext, itemInfo)), Array.Empty<DelegatingHandler>())
      {
        SerializationSettings = {
          Formatting = Formatting.None
        }
      };
    }

    private string GetTopicHostname(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue(requestContext, in BlobstoreEventGridClient.RegistrySettingsTopicHostnameQuery, true);

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.GetService<IVssRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnTopicHostnameUpdated), in BlobstoreEventGridClient.RegistrySettingsTopicHostnameQuery);
      Interlocked.CompareExchange<string>(ref this.topicHostname, this.GetTopicHostname(systemRequestContext), (string) null);
      this.RegisterStrongBoxTopicKeyNotification(systemRequestContext);
      Interlocked.CompareExchange<IEventGridClient>(ref this.eventGridClient, this.CreateEventGridClient(systemRequestContext), (IEventGridClient) null);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnTopicHostnameUpdated));
      systemRequestContext.GetElevatedDeploymentRequestContext().GetService<ITeamFoundationStrongBoxService>().UnregisterNotification(systemRequestContext, new StrongBoxItemChangedCallback(this.OnTopicKeyUpdated));
    }
  }
}

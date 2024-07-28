// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.ServiceHooksPublisherService
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3575E571-FF3A-4E7B-A8CC-64FFB01E8C91
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.Notifications.WebApi.Clients;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server
{
  public class ServiceHooksPublisherService : IServiceHooksPublisherService, IVssFrameworkService
  {
    private static readonly string s_layer = typeof (ServiceHooksPublisherService).Name;
    private static readonly string s_area = typeof (ServiceHooksPublisherService).Namespace;
    private IDisposableReadOnlyList<ServiceHooksPublisher> m_pluginPublisherImpls;
    private Dictionary<string, ServiceHooksPublisher> m_pluginPublisherImplsById;
    private Dictionary<string, ServiceHooksPublisher> m_contributedPublisherImplsById = new Dictionary<string, ServiceHooksPublisher>();
    private Dictionary<string, ServiceHooksPublisher> m_eventTypeToPublisherImpl = new Dictionary<string, ServiceHooksPublisher>();
    public const string s_ContributionTargetForPublishers = "ms.vss-servicehooks.publishers";
    public const string s_ContributionTypeNamePublisher = "ms.vss-servicehooks.publisher";
    public const string s_ContributionTypeNameEvent = "ms.vss-servicehooks.event";
    public const string s_ContributionTypeNameInputDescriptor = "ms.vss-servicehooks.input-descriptor";
    public const string s_ContributionTypeNameInputForm = "ms.vss-servicehooks.input-form";
    private static string[] s_contributionTargets = new string[1]
    {
      "ms.vss-servicehooks.publishers"
    };
    private static HashSet<string> s_contributionTypeNames = new HashSet<string>((IEnumerable<string>) new string[4]
    {
      "ms.vss-servicehooks.publisher",
      "ms.vss-servicehooks.event",
      "ms.vss-servicehooks.input-descriptor",
      "ms.vss-servicehooks.input-form"
    }, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private const string c_TfsFallbackPublisherResourceName = "ServiceHooksPublishing.FallbackPublishers.TfsServiceHooksPublisher.json";
    private const string c_RmFallbackPublisherResourceName = "ServiceHooksPublishing.FallbackPublishers.RmServiceHooksPublisher.json";
    private static ContributedServiceHooksPublisher s_TfsFallbackPublisher = (ContributedServiceHooksPublisher) null;
    private static ContributedServiceHooksPublisher s_RmFallbackPublisher = (ContributedServiceHooksPublisher) null;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(1063200, ServiceHooksPublisherService.s_area, ServiceHooksPublisherService.s_layer, nameof (ServiceStart));
      try
      {
        this.LoadPlugins(systemRequestContext);
        this.DiscoverContributedPublishers(systemRequestContext);
      }
      catch (Exception ex)
      {
        systemRequestContext.TraceException(1063200, ServiceHooksPublisherService.s_area, ServiceHooksPublisherService.s_layer, ex);
        throw;
      }
      finally
      {
        systemRequestContext.TraceLeave(1063200, ServiceHooksPublisherService.s_area, ServiceHooksPublisherService.s_layer, nameof (ServiceStart));
      }
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public ServiceHooksPublisher GetPublisherForEventType(
      IVssRequestContext requestContext,
      string eventType)
    {
      ServiceHooksPublisher publisherForEventType;
      if (this.m_eventTypeToPublisherImpl.TryGetValue(eventType, out publisherForEventType))
        return publisherForEventType;
      throw new PublisherNotFoundException(eventType);
    }

    public ServiceHooksPublisher GetPublisher(IVssRequestContext requestContext, string publisherId)
    {
      ServiceHooksPublisher publisher;
      if (this.m_pluginPublisherImplsById.TryGetValue(publisherId, out publisher) || this.m_contributedPublisherImplsById.TryGetValue(publisherId, out publisher))
        return publisher;
      this.DiscoverContributedPublishers(requestContext);
      if (this.m_contributedPublisherImplsById.TryGetValue(publisherId, out publisher))
        return publisher;
      return (ServiceHooksPublisher) (this.GetFallbackPublisher(requestContext, publisherId) ?? throw new PublisherNotFoundException(publisherId));
    }

    public IEnumerable<ServiceHooksPublisher> GetPublishers(IVssRequestContext requestContext)
    {
      SortedList<string, ServiceHooksPublisher> sortedList = new SortedList<string, ServiceHooksPublisher>();
      this.DiscoverContributedPublishers(requestContext);
      if (!this.m_contributedPublisherImplsById.ContainsKey("tfs"))
      {
        ContributedServiceHooksPublisher fallbackPublisher = this.GetFallbackPublisher(requestContext, "tfs");
        this.m_contributedPublisherImplsById.Add(fallbackPublisher.Id, (ServiceHooksPublisher) fallbackPublisher);
        foreach (string publisherEventType in fallbackPublisher.GetSupportedPublisherEventTypes(requestContext))
        {
          if (!this.m_eventTypeToPublisherImpl.ContainsKey(publisherEventType))
            this.m_eventTypeToPublisherImpl.Add(publisherEventType, (ServiceHooksPublisher) fallbackPublisher);
        }
      }
      foreach (ServiceHooksPublisher serviceHooksPublisher in this.m_contributedPublisherImplsById.Values)
        sortedList.Add(serviceHooksPublisher.Id, serviceHooksPublisher);
      if (this.m_pluginPublisherImplsById.Count > 0)
      {
        foreach (ServiceHooksPublisher serviceHooksPublisher in this.m_pluginPublisherImplsById.Values)
          sortedList.Add(serviceHooksPublisher.Id, serviceHooksPublisher);
      }
      return (IEnumerable<ServiceHooksPublisher>) sortedList.Values;
    }

    public NotificationHttpClient GetNotificationHttpClientForPublisher(
      IVssRequestContext requestContext,
      string publisherId)
    {
      ServiceHooksPublisher publisher = this.GetPublisher(requestContext, publisherId);
      if (publisher == null)
        throw new ArgumentException("Service hooks publisher does not exist for id '" + publisherId + "'");
      if (!(publisher is ContributedServiceHooksPublisher serviceHooksPublisher))
        throw new ArgumentException("Service hooks publisher '" + publisherId + "' is not a contributed publisher");
      return requestContext.GetClient<NotificationHttpClient>(new Guid(serviceHooksPublisher.ServiceInstanceType));
    }

    private void LoadPlugins(IVssRequestContext systemRequestContext)
    {
      this.m_pluginPublisherImpls = systemRequestContext.GetExtensions<ServiceHooksPublisher>(ExtensionLifetime.Service);
      this.m_pluginPublisherImplsById = new Dictionary<string, ServiceHooksPublisher>(this.m_pluginPublisherImpls.Count<ServiceHooksPublisher>());
      foreach (ServiceHooksPublisher pluginPublisherImpl in (IEnumerable<ServiceHooksPublisher>) this.m_pluginPublisherImpls)
      {
        if (pluginPublisherImpl is IRemotableServiceHooksPublisher serviceHooksPublisher)
        {
          if (this.IsOwningService(systemRequestContext, serviceHooksPublisher.ServiceOwner, systemRequestContext.ServiceInstanceType()))
          {
            if (serviceHooksPublisher.IsRemotePlugin())
              continue;
          }
          else if (!serviceHooksPublisher.IsRemotePlugin())
            continue;
        }
        if (this.m_pluginPublisherImplsById.ContainsKey(pluginPublisherImpl.Id))
          throw new ServiceHookException(ServiceHooksPublisherResources.Error_DuplicatePublisherIdentifier);
        this.m_pluginPublisherImplsById.Add(pluginPublisherImpl.Id, pluginPublisherImpl);
        try
        {
          pluginPublisherImpl.Start(systemRequestContext);
        }
        catch (Exception ex)
        {
          throw new ServiceHookException(string.Format(ServiceHooksPublisherResources.Error_FailedToStartPublisherTemplate, (object) pluginPublisherImpl.GetType().AssemblyQualifiedName), ex);
        }
      }
    }

    private bool IsOwningService(
      IVssRequestContext requestContext,
      Guid owningService,
      Guid loadingService)
    {
      if (owningService == loadingService)
        return true;
      return requestContext.ExecutionEnvironment.IsOnPremisesDeployment && owningService == ServiceInstanceTypes.TFS && loadingService == ServiceInstanceTypes.TFSOnPremises;
    }

    private void DiscoverContributedPublishers(IVssRequestContext requestContext)
    {
      string[] extensionNames = (string[]) null;
      foreach (Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Publisher contributedPublisher in this.LoadContributedPublishers(requestContext, out extensionNames))
      {
        if (!this.m_contributedPublisherImplsById.ContainsKey(contributedPublisher.Id))
        {
          ContributedServiceHooksPublisher serviceHooksPublisher = new ContributedServiceHooksPublisher(contributedPublisher);
          serviceHooksPublisher.Start(requestContext);
          this.m_contributedPublisherImplsById.Add(contributedPublisher.Id, (ServiceHooksPublisher) serviceHooksPublisher);
          foreach (string publisherEventType in serviceHooksPublisher.GetSupportedPublisherEventTypes(requestContext))
          {
            if (!this.m_eventTypeToPublisherImpl.ContainsKey(publisherEventType))
              this.m_eventTypeToPublisherImpl.Add(publisherEventType, (ServiceHooksPublisher) serviceHooksPublisher);
            else
              requestContext.Trace(1064968, TraceLevel.Info, ServiceHooksPublisherService.s_area, ServiceHooksPublisherService.s_layer, "Publisher Id: " + contributedPublisher.Id + ", EventType: " + publisherEventType + ", Publisher Conflicting Id: " + this.m_eventTypeToPublisherImpl[publisherEventType].Id);
          }
        }
      }
    }

    private Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Publisher[] LoadContributedPublishers(
      IVssRequestContext requestContext,
      out string[] extensionNames)
    {
      IEnumerable<Contribution> contributions = requestContext.GetService<IContributionService>().QueryContributions(requestContext, (IEnumerable<string>) ServiceHooksPublisherService.s_contributionTargets, ServiceHooksPublisherService.s_contributionTypeNames, ContributionQueryOptions.IncludeSubTree);
      if (contributions == null)
      {
        extensionNames = Array.Empty<string>();
        return Array.Empty<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Publisher>();
      }
      Dictionary<string, Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Publisher> dictionary1 = new Dictionary<string, Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Publisher>();
      Dictionary<string, EventTypeDescriptor> dictionary2 = new Dictionary<string, EventTypeDescriptor>();
      Dictionary<string, InputDescriptor> dictionary3 = new Dictionary<string, InputDescriptor>();
      Dictionary<string, InputForm> dictionary4 = new Dictionary<string, InputForm>();
      HashSet<string> source = new HashSet<string>();
      foreach (Contribution contribution in contributions)
      {
        string id = contribution.Id;
        if (string.Equals(contribution.Type, "ms.vss-servicehooks.publisher", StringComparison.Ordinal))
          dictionary1.Add(id, JsonConvert.DeserializeObject<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Publisher>(contribution.Properties.ToString()));
        else if (string.Equals(contribution.Type, "ms.vss-servicehooks.event", StringComparison.Ordinal))
          dictionary2.Add(id, JsonConvert.DeserializeObject<EventTypeDescriptor>(contribution.Properties.ToString()));
        else if (string.Equals(contribution.Type, "ms.vss-servicehooks.input-descriptor", StringComparison.Ordinal))
          dictionary3.Add(id, JsonConvert.DeserializeObject<InputDescriptor>(contribution.Properties.ToString()));
        else if (string.Equals(contribution.Type, "ms.vss-servicehooks.input-form", StringComparison.Ordinal))
          dictionary4.Add(id, JsonConvert.DeserializeObject<InputForm>(contribution.Properties.ToString()));
      }
      foreach (Contribution contribution in contributions)
      {
        if (string.Equals(contribution.Type, "ms.vss-servicehooks.event", StringComparison.Ordinal))
        {
          EventTypeDescriptor eventTypeDescriptor;
          if (dictionary2.TryGetValue(contribution.Id, out eventTypeDescriptor))
          {
            foreach (string target in contribution.Targets)
            {
              Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Publisher publisher;
              if (dictionary1.TryGetValue(target, out publisher))
              {
                if (publisher.SupportedEvents == null)
                  publisher.SupportedEvents = new List<EventTypeDescriptor>();
                publisher.SupportedEvents.Add(eventTypeDescriptor);
              }
            }
          }
        }
        else
        {
          InputForm inputForm;
          if (string.Equals(contribution.Type, "ms.vss-servicehooks.input-form", StringComparison.Ordinal) && dictionary4.TryGetValue(contribution.Id, out inputForm) && contribution.Targets != null)
          {
            foreach (string target in contribution.Targets)
            {
              EventTypeDescriptor eventTypeDescriptor;
              if (dictionary2.TryGetValue(target, out eventTypeDescriptor) && inputForm.InputDescriptors != null)
              {
                foreach (string inputDescriptor1 in inputForm.InputDescriptors)
                {
                  InputDescriptor inputDescriptor2;
                  if (dictionary3.TryGetValue(inputDescriptor1, out inputDescriptor2))
                  {
                    if (eventTypeDescriptor.InputDescriptors == null)
                      eventTypeDescriptor.InputDescriptors = new List<InputDescriptor>();
                    eventTypeDescriptor.InputDescriptors.Add(inputDescriptor2);
                  }
                }
              }
            }
          }
        }
        ContributionIdentifier contributionIdentifier = new ContributionIdentifier(contribution.Id);
        string str = contributionIdentifier.PublisherName + "." + contributionIdentifier.ExtensionName;
        if (!source.Contains(str))
          source.Add(str);
      }
      extensionNames = source.ToArray<string>();
      return dictionary1.Values.ToArray<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Publisher>();
    }

    private ContributedServiceHooksPublisher GetFallbackPublisher(
      IVssRequestContext requestContext,
      string publisherId)
    {
      if (string.Equals(publisherId, "tfs", StringComparison.OrdinalIgnoreCase))
      {
        if (ServiceHooksPublisherService.s_TfsFallbackPublisher == null)
          ServiceHooksPublisherService.s_TfsFallbackPublisher = this.LoadFallbackPublisher(requestContext, "ServiceHooksPublishing.FallbackPublishers.TfsServiceHooksPublisher.json");
        return ServiceHooksPublisherService.s_TfsFallbackPublisher;
      }
      if (!string.Equals(publisherId, "rm", StringComparison.OrdinalIgnoreCase))
        return (ContributedServiceHooksPublisher) null;
      if (ServiceHooksPublisherService.s_RmFallbackPublisher == null)
        ServiceHooksPublisherService.s_RmFallbackPublisher = this.LoadFallbackPublisher(requestContext, "ServiceHooksPublishing.FallbackPublishers.RmServiceHooksPublisher.json");
      return ServiceHooksPublisherService.s_RmFallbackPublisher;
    }

    private ContributedServiceHooksPublisher LoadFallbackPublisher(
      IVssRequestContext requestContext,
      string resourceName)
    {
      requestContext.Trace(1063200, TraceLevel.Error, ServiceHooksPublisherService.s_area, ServiceHooksPublisherService.s_layer, string.Format("Loading Service Hooks fallback publisher {0}", (object) resourceName));
      try
      {
        string name = this.GetType().Namespace + "." + resourceName;
        string end;
        using (StreamReader streamReader = new StreamReader(this.GetType().Assembly.GetManifestResourceStream(name)))
          end = streamReader.ReadToEnd();
        ContributedServiceHooksPublisher serviceHooksPublisher = new ContributedServiceHooksPublisher(JsonConvert.DeserializeObject<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Publisher>(end));
        serviceHooksPublisher.Start(requestContext);
        return serviceHooksPublisher;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1063200, ServiceHooksPublisherService.s_area, ServiceHooksPublisherService.s_layer, ex);
        throw;
      }
    }
  }
}

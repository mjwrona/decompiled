// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.NotificationTransformService
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Notifications.Common;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal class NotificationTransformService : INotificationTransformService, IVssFrameworkService
  {
    public const string CTX_MESSAGE_RECIPIENT_COUNTS = "$NotifMsgeRecipientCounts$";
    public const string CTX_GROUP_EXPANSION_ENABLED = "$NotifGroupExpansion$";
    private string m_xslSearchPath;
    private Dictionary<string, string> m_templateOverrides = new Dictionary<string, string>();
    private const string XslExtension = ".xsl";
    private const string PlainTextXslExtension = ".plainTextXsl";
    private static readonly HashSet<string> s_emailTemplateContributionTypes = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      "ms.vss-notifications.email-template"
    };
    private static readonly HashSet<string> s_emailDataTemplateContributionTypes = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      "ms.vss-notifications.notification-data"
    };
    internal static JsonSerializer s_serializer = new NotificationTranformJsonFormatter().CreateJsonSerializer();
    private IDisposableReadOnlyList<IMatcherBasedTemplateInputProvider> m_matcherInputProviders;
    private IDisposableReadOnlyList<IEventTypeBasedTemplateInputProvider> m_eventTypeInputProviders;
    private IDisposableReadOnlyList<IChannelBasedTemplateInputProvider> m_channelInputProviders;

    public virtual void ServiceStart(IVssRequestContext systemRequestContext)
    {
      this.m_eventTypeInputProviders = systemRequestContext.GetExtensions<IEventTypeBasedTemplateInputProvider>();
      this.m_matcherInputProviders = systemRequestContext.GetExtensions<IMatcherBasedTemplateInputProvider>();
      this.m_channelInputProviders = systemRequestContext.GetExtensions<IChannelBasedTemplateInputProvider>();
      RegistryEntryCollection source = systemRequestContext.GetService<IVssRegistryService>().ReadEntriesFallThru(systemRequestContext, (RegistryQuery) (FrameworkServerConstants.NotificationRootPath + "/**"));
      this.m_xslSearchPath = source["EmailNotificationXslSearchPath"].GetValue("Transforms");
      if (!Path.IsPathRooted(this.m_xslSearchPath))
        this.m_xslSearchPath = Path.Combine(systemRequestContext.ServiceHost.PhysicalDirectory, this.m_xslSearchPath);
      this.m_xslSearchPath = FileSpec.GetFullPath(this.m_xslSearchPath);
      foreach (RegistryEntry registryEntry in source.Where<RegistryEntry>((Func<RegistryEntry, bool>) (x => x.Path.StartsWith(FrameworkServerConstants.EmailTemplates))))
      {
        if (!string.IsNullOrEmpty(registryEntry.Value))
          this.m_templateOverrides[registryEntry.Name] = registryEntry.Value;
      }
    }

    public virtual void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      if (this.m_eventTypeInputProviders != null)
      {
        this.m_eventTypeInputProviders.Dispose();
        this.m_eventTypeInputProviders = (IDisposableReadOnlyList<IEventTypeBasedTemplateInputProvider>) null;
      }
      if (this.m_matcherInputProviders != null)
      {
        this.m_matcherInputProviders.Dispose();
        this.m_matcherInputProviders = (IDisposableReadOnlyList<IMatcherBasedTemplateInputProvider>) null;
      }
      if (this.m_channelInputProviders == null)
        return;
      this.m_channelInputProviders.Dispose();
      this.m_channelInputProviders = (IDisposableReadOnlyList<IChannelBasedTemplateInputProvider>) null;
    }

    public NotificationTransformResult ApplyTransform(
      IVssRequestContext requestContext,
      TeamFoundationNotification notification,
      NotificationStopwatch templateFetchStopwatch = null,
      NotificationStopwatch transformStopwatch = null)
    {
      requestContext.WarnIfContributionsInFallbackMode(nameof (ApplyTransform));
      NotificationTransformResult notificationTransformResult;
      using (new AutoStopwatch((Stopwatch) transformStopwatch))
      {
        if (!NotificationFrameworkConstants.EmailTargetDeliveryChannels.Contains(notification.Channel))
          throw new EventTypeNotSupportedException(CoreRes.NoEmailFormatterAssociatedWithType((object) notification.EventFieldContainer.GetType().Name));
        NotificationTransformContext transformContext = this.CreateTransformContext(requestContext, notification);
        notificationTransformResult = this.TransformJson(requestContext, transformContext, templateFetchStopwatch) ?? this.TransformXml(requestContext, transformContext);
      }
      return notificationTransformResult != null ? notificationTransformResult : throw new EventTypeNotSupportedException(CoreRes.NoEmailFormatterAssociatedWithType((object) notification.EventFieldContainer.GetType().Name));
    }

    public EventTransformResult TransformSampleEvent(
      IVssRequestContext requestContext,
      EventTransformRequest transformRequest)
    {
      string legacy = EventTypeMapper.ToLegacy(requestContext, transformRequest.EventType);
      TeamFoundationEvent teamFoundationEvent = TeamFoundationEventFactory.GetEvent(legacy, transformRequest.EventPayload);
      NotificationPrepareContent notifPrepareContent = new NotificationPrepareContent()
      {
        EventType = legacy,
        EventSource = teamFoundationEvent.GetFieldContainer()
      };
      NotificationTransformService.ApplyFilter(requestContext, notifPrepareContent);
      NotificationTransformContext transformContext = new NotificationTransformContext()
      {
        EventType = legacy,
        EventFieldContainer = notifPrepareContent.EventSource,
        SystemInputs = new Dictionary<string, string>()
      };
      NotificationTransformService.UpdateBrandingInputs(requestContext, transformContext);
      foreach (KeyValuePair<string, string> systemInput in transformRequest.SystemInputs)
      {
        if (systemInput.Value != null)
          transformContext.SystemInputs[systemInput.Key] = systemInput.Value;
      }
      NotificationTransformResult notificationTransformResult = this.TransformJson(requestContext, transformContext);
      if (notificationTransformResult == null)
        throw new NotificationEventTypeNotSupportedFormatException(CoreRes.NoEmailFormatterAssociatedWithType((object) notifPrepareContent.EventSource.GetType().Name));
      return new EventTransformResult()
      {
        Content = notificationTransformResult.Content,
        SystemInputs = notificationTransformResult.SystemInputs,
        Data = (object) notificationTransformResult.Data
      };
    }

    internal NotificationTransformContext CreateTransformContext(
      IVssRequestContext requestContext,
      TeamFoundationNotification notification)
    {
      NotificationTransformContext transformContext = new NotificationTransformContext()
      {
        EventType = notification.Event.EventType,
        EventFieldContainer = notification.EventFieldContainer,
        DeliveryDetails = notification.DeliveryDetails,
        Channel = notification.Channel
      };
      NotificationTransformService.UpdateSystemInputs(requestContext, transformContext);
      this.m_matcherInputProviders.FirstOrDefault<IMatcherBasedTemplateInputProvider>((Func<IMatcherBasedTemplateInputProvider, bool>) (t => t.SupportedMatcherTypes != null && ((IEnumerable<string>) t.SupportedMatcherTypes).Contains<string>(notification.DeliveryDetails.Matcher)))?.UpdateSystemInputs(requestContext, transformContext);
      this.m_eventTypeInputProviders.FirstOrDefault<IEventTypeBasedTemplateInputProvider>((Func<IEventTypeBasedTemplateInputProvider, bool>) (t => t.SupportedEventTypes != null && ((IEnumerable<string>) t.SupportedEventTypes).Contains<string>(notification.Event.EventType)))?.UpdateSystemInputs(requestContext, transformContext);
      this.m_channelInputProviders.ForEach<IChannelBasedTemplateInputProvider>((Action<IChannelBasedTemplateInputProvider>) (provider =>
      {
        if (provider.SupportedChannelTypes == null || !((IEnumerable<string>) provider.SupportedChannelTypes).Contains<string>(notification.Channel))
          return;
        provider.UpdateSystemInputs(requestContext, transformContext);
      }));
      NotificationTrackingData trackingData = NotificationTrackingUtils.CreateTrackingData(requestContext, notification);
      transformContext.SystemInputs["Tracking"] = trackingData.GetEncodedString(true);
      transformContext.SystemInputs["Telemetry"] = transformContext.SystemInputs["Tracking"];
      return transformContext;
    }

    private static void ApplyFilter(
      IVssRequestContext requestContext,
      NotificationPrepareContent notifPrepareContent)
    {
      using (IDisposableReadOnlyList<ISampleEventTransformFilter> extensions = requestContext.GetExtensions<ISampleEventTransformFilter>())
      {
        foreach (ISampleEventTransformFilter eventTransformFilter in (IEnumerable<ISampleEventTransformFilter>) extensions)
        {
          if (eventTransformFilter.SupportedEventTypes.Contains<string>(notifPrepareContent.EventType))
          {
            eventTransformFilter.PrepareSampleEvent(requestContext, notifPrepareContent);
            break;
          }
        }
      }
    }

    private static bool GetXmlContainer(
      IFieldContainer fieldContainer,
      out XmlDocumentFieldContainer xmlContainer)
    {
      ref XmlDocumentFieldContainer local = ref xmlContainer;
      if (!(fieldContainer is XmlDocumentFieldContainer documentFieldContainer))
        documentFieldContainer = fieldContainer.GetDynamicFieldContainer(DynamicFieldContainerType.Xml) as XmlDocumentFieldContainer;
      local = documentFieldContainer;
      return xmlContainer != null;
    }

    private static bool GetJsonContainer(
      IFieldContainer fieldContainer,
      out JsonDocumentFieldContainer jsonContainer)
    {
      ref JsonDocumentFieldContainer local = ref jsonContainer;
      if (!(fieldContainer is JsonDocumentFieldContainer documentFieldContainer))
        documentFieldContainer = fieldContainer.GetDynamicFieldContainer(DynamicFieldContainerType.Json) as JsonDocumentFieldContainer;
      local = documentFieldContainer;
      return jsonContainer != null;
    }

    private NotificationTransformResult TransformXml(
      IVssRequestContext requestContext,
      NotificationTransformContext transformContext)
    {
      NotificationTransformResult notificationTransformResult = (NotificationTransformResult) null;
      if (NotificationTransformService.GetXmlContainer(transformContext.EventFieldContainer, out XmlDocumentFieldContainer _))
      {
        string xslFileName = this.GetXslFileName(transformContext);
        if (this.UseLegacyXslTemplate(requestContext, xslFileName))
        {
          notificationTransformResult = new XslEmailTemplateTransform(this.m_xslSearchPath, xslFileName).Transform(requestContext, transformContext);
          notificationTransformResult.SystemInputs = transformContext.SystemInputs;
        }
      }
      return notificationTransformResult;
    }

    private NotificationTransformResult TransformJson(
      IVssRequestContext requestContext,
      NotificationTransformContext transformContext,
      NotificationStopwatch templateFetchStopwatch = null)
    {
      NotificationTransformResult notificationTransformResult = (NotificationTransformResult) null;
      string contributed = EventTypeMapper.ToContributed(requestContext, transformContext.EventType);
      JsonDocumentFieldContainer jsonContainer;
      if (NotificationTransformService.GetJsonContainer(transformContext.EventFieldContainer, out jsonContainer))
      {
        IContributedTemplateService service1 = requestContext.GetService<IContributedTemplateService>();
        try
        {
          List<ContributedTemplate> templatesForEvent1;
          using (new AutoStopwatch((Stopwatch) templateFetchStopwatch))
            templatesForEvent1 = service1.GetTemplatesForEvent(requestContext, contributed, NotificationTransformService.s_emailTemplateContributionTypes);
          if (templatesForEvent1.Any<ContributedTemplate>())
          {
            JObject templateContext = new JObject()
            {
              ["event"] = (JToken) jsonContainer.GetJObject()
            };
            if (transformContext.SystemInputs != null)
              templateContext["system"] = (JToken) JObject.FromObject((object) transformContext.SystemInputs, NotificationTransformService.s_serializer);
            IContributedDataTemplateService service2 = requestContext.GetService<IContributedDataTemplateService>();
            List<ContributedDataTemplate> templatesForEvent2;
            using (new AutoStopwatch((Stopwatch) templateFetchStopwatch))
              templatesForEvent2 = service2.GetTemplatesForEvent(requestContext, contributed, NotificationTransformService.s_emailDataTemplateContributionTypes);
            ContributedDataTemplate template = service2.SelectEventTemplate(requestContext, templatesForEvent2, templateContext);
            JObject jobject = (JObject) null;
            if (template != null)
            {
              jobject = new HandlebarsDataTemplateTransform(template).Transform(requestContext, templateContext);
              templateContext["data"] = (JToken) jobject;
            }
            notificationTransformResult = new HandlebarsTemplateTransform(service1.SelectEventTemplate(requestContext, templatesForEvent1, templateContext) ?? throw new EventTypeTransformNotFoundException(CoreRes.EventTransformTeplateNotFound((object) contributed))).Transform(requestContext, templateContext);
            if (notificationTransformResult != null)
            {
              notificationTransformResult.Data = jobject;
              notificationTransformResult.SystemInputs = transformContext.SystemInputs;
            }
          }
        }
        catch (Exception ex)
        {
          if (!transformContext.EventFieldContainer.DualExecution)
            throw;
          else
            requestContext.TraceException(1002313, TraceLevel.Error, "Notifications", contributed, ex);
        }
      }
      return !transformContext.EventFieldContainer.DualExecution ? notificationTransformResult : (NotificationTransformResult) null;
    }

    private static void UpdateBrandingInputs(
      IVssRequestContext requestContext,
      NotificationTransformContext transformContext)
    {
      transformContext.SystemInputs["ProductIcon"] = !NotificationTransformUtils.ShouldUseV2EmailTemplates(requestContext) ? "vsts-flat-email.png" : "v3/vsts.png";
      if (requestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        if (requestContext.IsFeatureEnabled("VisualStudio.Services.WebPlatform.UseNewBranding"))
          transformContext.SystemInputs["ProductName"] = "Azure DevOps";
        else
          transformContext.SystemInputs["ProductName"] = "Azure DevOps Services";
      }
      else
        transformContext.SystemInputs["ProductName"] = "Azure DevOps Server";
    }

    private static void UpdateSystemInputs(
      IVssRequestContext requestContext,
      NotificationTransformContext transformContext)
    {
      NotificationDeliveryDetails deliveryDetails = transformContext.DeliveryDetails;
      Dictionary<string, string> systemInputs = transformContext.SystemInputs;
      bool isContainer = deliveryDetails.SourceIdentity.IsContainer;
      systemInputs["SubscriptionType"] = deliveryDetails.Matcher;
      systemInputs["IsTeamSubscription"] = isContainer ? true.ToString() : false.ToString();
      systemInputs["SubscriptionId"] = deliveryDetails.NotificationSource;
      string displayName = deliveryDetails.SourceIdentity.DisplayName;
      systemInputs["Subscriber"] = displayName ?? string.Empty;
      Guid result;
      int num = string.IsNullOrEmpty(deliveryDetails.NotificationSource) || int.TryParse(deliveryDetails.NotificationSource, out int _) ? 1 : (Guid.TryParse(deliveryDetails.NotificationSource, out result) ? 1 : 0);
      bool isSystem = deliveryDetails.IsSystem;
      systemInputs["SubscriptionReason"] = num == 0 ? (isSystem ? CoreRes.SubscriptionReasonSystem() : CoreRes.SubscriptionReasonDefault()) : (!isContainer || string.IsNullOrEmpty(displayName) ? CoreRes.SubscriptionReasonPersonal() : CoreRes.SubscriptionReasonGroup((object) displayName));
      if (!isContainer && deliveryDetails.Recipients.Count == 1)
      {
        Dictionary<string, string> dictionary = systemInputs;
        result = deliveryDetails.Recipients[0].Id;
        string str = result.ToString();
        dictionary["SingleRecipientId"] = str;
      }
      bool flag = false;
      if (!string.IsNullOrEmpty(deliveryDetails.SourceUrl))
      {
        flag = true;
        systemInputs["SubscriptionUrl"] = deliveryDetails.SourceUrl;
        if (!isContainer || deliveryDetails.IsOptOutable)
        {
          Uri uri = new Uri(deliveryDetails.SourceUrl);
          systemInputs["SubscriptionUnsubscribeUrl"] = uri.AppendQuery("action", "unsubscribe").ToString();
          systemInputs["SubscriptionUnsubscribeAction"] = CoreRes.SubscriptionUnsubscribeActionName();
        }
      }
      systemInputs["SubscriptionName"] = deliveryDetails.Description ?? (flag ? CoreRes.EmailNotificationViewLink() : deliveryDetails.NotificationSource ?? string.Empty);
      NotificationTransformService.UpdateBrandingInputs(requestContext, transformContext);
    }

    private bool UseLegacyXslTemplate(IVssRequestContext requestContext, string xslFileName)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<INotificationTemplateTypeCache>().HasXslTemplate(vssRequestContext, this.m_xslSearchPath, xslFileName);
    }

    private string GetXslFileName(NotificationTransformContext transformContext)
    {
      string str1 = NotificationFrameworkConstants.EmailTargetDeliveryChannels.Contains(transformContext.Channel) ? ".xsl" : ".plainTextXsl";
      string key = transformContext.EventType + str1;
      string str2;
      if (this.m_templateOverrides != null && this.m_templateOverrides.TryGetValue(key, out str2))
        key = str2;
      return key;
    }
  }
}

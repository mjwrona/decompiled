// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.ServiceHooksConsumerService
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceHooks.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Azure;
using Microsoft.VisualStudio.Services.ServiceHooks.Consumers.DataDriven;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers
{
  public class ServiceHooksConsumerService : VssBaseService, IVssFrameworkService
  {
    private const double c_notifyConsumerTimeoutSeconds = 30.0;
    private const string c_falseLower = "false";
    private const string c_schemeHttps = "https";
    private const string c_eventHandlerPrefixLowercase = "handle";
    private const string c_eventHandlerGenericNameLowercase = "handleevent";
    private const string c_consumerContributionsKey = "shConsumerContributions";
    private const string c_registryPathServiceHooksRoot = "/Service/ServiceHooks";
    private const string c_registryPathAllowHttpNotifications = "/Service/ServiceHooks/Settings/AllowHttpNotifications";
    private const string c_registryPathNotifyConsumerCustomTimeout = "/Service/ServiceHooks/Settings/NotifyConsumerCustomTimeout";
    private static readonly string s_layer = typeof (ServiceHooksConsumerService).Name;
    private static readonly string s_area = typeof (ServiceHooksConsumerService).Namespace;
    private static char[] s_splitOnPeriod = new char[1]
    {
      '.'
    };
    private static ServiceHooksConsumerService.InputDescriptorIdComparer s_inputDescriptorIdComparer = new ServiceHooksConsumerService.InputDescriptorIdComparer();
    private ILockName m_lockName;
    private IVssRegistryService m_registryService;
    private IVssDeploymentServiceHost m_deploymentServiceHost;
    private IDisposableReadOnlyList<ConsumerImplementation> m_consumerImpls;
    private IDisposableReadOnlyList<ConsumerActionImplementation> m_actionImpls;
    private Dictionary<string, ConsumerImplementation> m_consumerImplsById;
    private Dictionary<string, Dictionary<string, ConsumerActionImplementation>> m_actionImplsByConsumerId;
    private Dictionary<ConsumerActionImplementation, Dictionary<string, MethodInfo>> m_eventHandlerMapsByActionImpl;
    private Dictionary<string, SortedList<Version, ConsumerImplementation>> m_extensionConsumerByIdAndVersion = new Dictionary<string, SortedList<Version, ConsumerImplementation>>();
    private Dictionary<string, SortedList<Version, Dictionary<string, ConsumerActionImplementation>>> m_extensionActionByConsumerIdAndVersionAndAction = new Dictionary<string, SortedList<Version, Dictionary<string, ConsumerActionImplementation>>>();
    private static HashSet<string> s_firstPartyHighlyAvailableConsumers = new HashSet<string>()
    {
      "azureAppService",
      AzureServiceBusConsumer.ConsumerId,
      AzureStorageConsumer.ConsumerId,
      "workplaceMessagingApps"
    };

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return;
      ArgumentUtility.CheckForNull<IVssRequestContext>(systemRequestContext, nameof (systemRequestContext));
      systemRequestContext.TraceEnter(1063430, ServiceHooksConsumerService.s_area, ServiceHooksConsumerService.s_layer, nameof (ServiceStart));
      this.m_lockName = this.CreateLockName(systemRequestContext, "servicehooksconsumerservice");
      try
      {
        this.LoadConsumers(systemRequestContext);
        this.LoadConsumerActions(systemRequestContext);
        this.m_registryService = systemRequestContext.GetService<IVssRegistryService>();
        this.m_deploymentServiceHost = (IVssDeploymentServiceHost) systemRequestContext.To(TeamFoundationHostType.Deployment).ServiceHost;
      }
      catch (Exception ex)
      {
        systemRequestContext.TraceException(1063430, ServiceHooksConsumerService.s_area, ServiceHooksConsumerService.s_layer, ex);
        throw;
      }
      finally
      {
        systemRequestContext.TraceLeave(1063430, ServiceHooksConsumerService.s_area, ServiceHooksConsumerService.s_layer, nameof (ServiceStart));
      }
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return;
      ArgumentUtility.CheckForNull<IVssRequestContext>(systemRequestContext, nameof (systemRequestContext));
      systemRequestContext.TraceEnter(1063430, ServiceHooksConsumerService.s_area, ServiceHooksConsumerService.s_layer, nameof (ServiceEnd));
      try
      {
        if (this.m_consumerImpls != null)
        {
          this.m_consumerImpls.Dispose();
          this.m_consumerImpls = (IDisposableReadOnlyList<ConsumerImplementation>) null;
        }
        if (this.m_actionImpls == null)
          return;
        this.m_actionImpls.Dispose();
        this.m_actionImpls = (IDisposableReadOnlyList<ConsumerActionImplementation>) null;
      }
      catch (Exception ex)
      {
        systemRequestContext.TraceException(1063430, ServiceHooksConsumerService.s_area, ServiceHooksConsumerService.s_layer, ex);
        throw;
      }
      finally
      {
        systemRequestContext.TraceLeave(1063430, ServiceHooksConsumerService.s_area, ServiceHooksConsumerService.s_layer, nameof (ServiceEnd));
      }
    }

    public IEnumerable<ConsumerImplementation> GetConsumers(IVssRequestContext requestContext)
    {
      IEnumerable<Contribution> consumerContributions;
      requestContext.TryGetItem<IEnumerable<Contribution>>("shConsumerContributions", out consumerContributions);
      if (consumerContributions == null)
      {
        consumerContributions = this.GetConsumerContributions(requestContext);
        if (consumerContributions != null && consumerContributions.Count<Contribution>() > 0)
          this.UpdateConsumers(requestContext, consumerContributions);
      }
      IVssRequestContext context = requestContext.To(TeamFoundationHostType.Deployment);
      List<ConsumerImplementation> consumers = new List<ConsumerImplementation>();
      ILockName lockName = this.m_lockName;
      using (context.AcquireReaderLock(lockName))
      {
        foreach (Contribution contribution in consumerContributions)
        {
          if (this.m_extensionConsumerByIdAndVersion.ContainsKey(contribution.Id))
          {
            Version key = new Version(this.GetVersionForContribution(requestContext, contribution));
            SortedList<Version, ConsumerImplementation> sortedList = this.m_extensionConsumerByIdAndVersion[contribution.Id];
            if (sortedList.ContainsKey(key))
              consumers.Add(sortedList[key]);
          }
        }
        foreach (ConsumerImplementation consumerImpl in (IEnumerable<ConsumerImplementation>) this.m_consumerImpls)
          consumers.Add(consumerImpl);
      }
      for (int index = consumers.Count - 1; index >= 0; --index)
      {
        if (!consumers[index].IsFeatureAvailable(requestContext))
          consumers.RemoveAt(index);
      }
      consumers.Sort((Comparison<ConsumerImplementation>) ((c1, c2) => string.Compare(c1.Id, c2.Id)));
      return (IEnumerable<ConsumerImplementation>) consumers;
    }

    public ConsumerImplementation GetConsumer(IVssRequestContext requestContext, string consumerId)
    {
      IEnumerable<Contribution> contributions;
      requestContext.TryGetItem<IEnumerable<Contribution>>("shConsumerContributions", out contributions);
      if (contributions == null)
      {
        IEnumerable<Contribution> consumerContributions = this.GetConsumerContributions(requestContext);
        if (consumerContributions != null && consumerContributions.Count<Contribution>() > 0)
          this.UpdateConsumers(requestContext, consumerContributions);
      }
      ConsumerImplementation consumerImplementation;
      using (requestContext.To(TeamFoundationHostType.Deployment).AcquireReaderLock(this.m_lockName))
      {
        if (this.m_extensionConsumerByIdAndVersion.ContainsKey(consumerId))
        {
          SortedList<Version, ConsumerImplementation> sortedList = this.m_extensionConsumerByIdAndVersion[consumerId];
          consumerImplementation = sortedList.Values[sortedList.Count - 1];
        }
        else
          this.m_consumerImplsById.TryGetValue(consumerId, out consumerImplementation);
      }
      if (consumerImplementation == null)
        throw new ConsumerNotFoundException(consumerId);
      return consumerImplementation.IsFeatureAvailable(requestContext) ? consumerImplementation : throw new ConsumerNotAvailableException(consumerId);
    }

    public ConsumerActionImplementation GetConsumerAction(
      IVssRequestContext requestContext,
      string consumerId,
      string consumerActionId)
    {
      using (requestContext.To(TeamFoundationHostType.Deployment).AcquireReaderLock(this.m_lockName))
      {
        ConsumerActionImplementation actionImplementation = (ConsumerActionImplementation) null;
        if (!this.m_actionImplsByConsumerId.ContainsKey(consumerId) && !this.m_extensionActionByConsumerIdAndVersionAndAction.ContainsKey(consumerId))
          throw new ServiceHookException(ServiceHooksConsumerResources.Error_NoActionsForConsumerId);
        if (this.m_actionImplsByConsumerId.ContainsKey(consumerId))
        {
          this.m_actionImplsByConsumerId[consumerId].TryGetValue(consumerActionId, out actionImplementation);
        }
        else
        {
          SortedList<Version, Dictionary<string, ConsumerActionImplementation>> sortedList = this.m_extensionActionByConsumerIdAndVersionAndAction[consumerId];
          if (sortedList.Count > 0)
            sortedList.Values[sortedList.Count - 1].TryGetValue(consumerActionId, out actionImplementation);
        }
        return actionImplementation != null ? actionImplementation : throw new ConsumerActionNotFoundException(consumerId, consumerActionId);
      }
    }

    public ActionTask GetConsumerActionTaskForNotification(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification notification)
    {
      ConsumerActionImplementation consumerAction = this.GetConsumerAction(requestContext, notification.Details.ConsumerId, notification.Details.ConsumerActionId);
      Dictionary<string, MethodInfo> eventHandlerMaps = this.GetEventHandlerMaps(requestContext, consumerAction);
      string eventType = notification.Details.EventType;
      MethodInfo methodInfo;
      if (eventHandlerMaps == null || !eventHandlerMaps.TryGetValue(eventType, out methodInfo) && !eventHandlerMaps.TryGetValue("*", out methodInfo))
        throw new MissingMethodException(string.Format(ServiceHooksConsumerResources.Error_MatchingEventHandlerMethodNotFoundTemplate, (object) eventType, (object) consumerAction.GetType().AssemblyQualifiedName));
      this.AttachSessionTokenToEventIfNeeded(requestContext, notification, consumerAction);
      ActionTask taskForNotification = (ActionTask) methodInfo.Invoke((object) consumerAction, new object[3]
      {
        (object) requestContext,
        (object) notification.Details.Event,
        (object) new HandleEventArgs()
        {
          Notification = notification
        }
      });
      if (taskForNotification == null)
        throw new ServiceHookException(string.Format(ServiceHooksConsumerResources.Error_EventHandlerReturnedNullActionTaskTemplate, (object) methodInfo.Name, (object) notification.Details.ConsumerActionId, (object) notification.Details.ConsumerId));
      if (!(taskForNotification is HttpActionTask httpActionTask) || httpActionTask.HttpRequestMessage == null || !(httpActionTask.HttpRequestMessage.RequestUri != (Uri) null) || httpActionTask.HttpRequestMessage.RequestUri.IsAbsoluteUri && !(httpActionTask.HttpRequestMessage.RequestUri.Scheme != "https") || !(notification.Details.ConsumerId != "webHooks") || !(notification.Details.ConsumerId != "jenkins") || !(notification.Details.ConsumerId != "bamboo") || !(notification.Details.ConsumerId != "grafana") || !(this.m_registryService.GetValue(requestContext, (RegistryQuery) "/Service/ServiceHooks/Settings/AllowHttpNotifications", true, "false").ToLower() == "false"))
        return taskForNotification;
      throw new ServiceHookException(string.Format(ServiceHooksConsumerResources.Error_HttpConsumersMustUseHttpsTemplate, (object) notification.Details.ConsumerId, (object) notification.Details.ConsumerActionId));
    }

    public async Task<ActionTaskResult> RunActionTaskAsync(
      IVssRequestContext requestContext,
      ActionTask actionTask,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification notification,
      INotificationTracer notificationTracer)
    {
      actionTask.NotificationTracer = notificationTracer;
      actionTask.Notification = notification;
      ActionTaskResult actionTaskResult;
      try
      {
        actionTaskResult = await actionTask.RunAsync(requestContext, TimeSpan.FromSeconds(requestContext.GetService<IVssRegistryService>().GetValue<double>(requestContext, (RegistryQuery) "/Service/ServiceHooks/Settings/NotifyConsumerCustomTimeout", true, 30.0)));
        if (actionTaskResult != null)
        {
          if (actionTaskResult.Exception != null)
            requestContext.TraceException(1051260, ServiceHooksConsumerService.s_area, ServiceHooksConsumerService.s_layer, actionTaskResult.Exception);
        }
      }
      catch (Exception ex)
      {
        actionTaskResult = new ActionTaskResult(ActionTaskResultLevel.EnduringFailure, ex, ex.Message);
        requestContext.TraceException(1051260, ServiceHooksConsumerService.s_area, ServiceHooksConsumerService.s_layer, ex);
      }
      return actionTaskResult;
    }

    private Dictionary<string, MethodInfo> GetEventHandlerMaps(
      IVssRequestContext requestContext,
      ConsumerActionImplementation actionImpl)
    {
      using (requestContext.To(TeamFoundationHostType.Deployment).AcquireReaderLock(this.m_lockName))
        return this.m_eventHandlerMapsByActionImpl.ContainsKey(actionImpl) ? this.m_eventHandlerMapsByActionImpl[actionImpl] : (Dictionary<string, MethodInfo>) null;
    }

    public static bool IsFirstPartyHighlyAvailableConsumer(string consumerId) => ServiceHooksConsumerService.s_firstPartyHighlyAvailableConsumers.Contains(consumerId);

    private IEnumerable<Contribution> GetConsumerContributions(IVssRequestContext requestContext) => requestContext.GetService<IContributionService>().QueryContributionsForTarget(requestContext, "ms.vss-servicehooks.consumers");

    private string GetVersionForContribution(
      IVssRequestContext requestContext,
      Contribution contribution)
    {
      ContributionIdentifier contributionIdentifier = new ContributionIdentifier(contribution.Id);
      return requestContext.GetService<IInstalledExtensionManager>().GetInstalledExtension(requestContext, contributionIdentifier.PublisherName, contributionIdentifier.ExtensionName)?.Version;
    }

    private void UpdateConsumers(
      IVssRequestContext requestContext,
      IEnumerable<Contribution> consumerContributions)
    {
      IVssRequestContext context = requestContext.To(TeamFoundationHostType.Deployment);
      List<Contribution> contributionList = (List<Contribution>) null;
      using (context.AcquireReaderLock(this.m_lockName))
      {
        foreach (Contribution consumerContribution in consumerContributions)
        {
          Version key = new Version(this.GetVersionForContribution(requestContext, consumerContribution));
          if (!this.m_extensionConsumerByIdAndVersion.ContainsKey(consumerContribution.Id) || !this.m_extensionConsumerByIdAndVersion[consumerContribution.Id].ContainsKey(key))
          {
            if (contributionList == null)
              contributionList = new List<Contribution>();
            contributionList.Add(consumerContribution);
          }
        }
      }
      if (contributionList == null)
        return;
      using (context.AcquireWriterLock(this.m_lockName))
      {
        foreach (Contribution contribution in contributionList)
        {
          ConsumerImplementation consumerForContribution = this.CreateConsumerForContribution(requestContext, contribution);
          if (consumerForContribution != null)
          {
            string id = contribution.Id;
            Version key = new Version(this.GetVersionForContribution(requestContext, contribution));
            if (!this.m_extensionConsumerByIdAndVersion.ContainsKey(id))
              this.m_extensionConsumerByIdAndVersion.Add(id, new SortedList<Version, ConsumerImplementation>());
            this.m_extensionConsumerByIdAndVersion[id].Add(key, consumerForContribution);
            Dictionary<string, ConsumerActionImplementation> dictionary = new Dictionary<string, ConsumerActionImplementation>();
            foreach (ConsumerActionImplementation action in (IEnumerable<ConsumerActionImplementation>) consumerForContribution.Actions)
            {
              dictionary.Add(action.Id, action);
              this.MapSupportedEventTypesToEventHandlers(requestContext, action);
            }
            if (!this.m_extensionActionByConsumerIdAndVersionAndAction.ContainsKey(id))
              this.m_extensionActionByConsumerIdAndVersionAndAction.Add(id, new SortedList<Version, Dictionary<string, ConsumerActionImplementation>>());
            this.m_extensionActionByConsumerIdAndVersionAndAction[id].Add(key, dictionary);
          }
        }
      }
    }

    private ConsumerImplementation CreateConsumerForContribution(
      IVssRequestContext requestContext,
      Contribution contribution)
    {
      if (string.Equals(contribution.Type, "ms.vss-servicehooks.consumer", StringComparison.OrdinalIgnoreCase))
      {
        try
        {
          this.GetVersionForContribution(requestContext, contribution);
          ExtensionConsumer consumerForContribution = new ExtensionConsumer(contribution.Properties);
          consumerForContribution.SetExtensionConsumerId(contribution.Id);
          return (ConsumerImplementation) consumerForContribution;
        }
        catch (Exception ex)
        {
          string message = "Cannot create consumer for " + contribution.Id + ".";
          requestContext.Trace(1051925, TraceLevel.Error, ServiceHooksConsumerService.s_area, ServiceHooksConsumerService.s_layer, message);
          requestContext.TraceException(1051925, ServiceHooksConsumerService.s_area, ServiceHooksConsumerService.s_layer, ex);
        }
      }
      return (ConsumerImplementation) null;
    }

    private void AttachSessionTokenToEventIfNeeded(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification notification,
      ConsumerActionImplementation actionImpl)
    {
      string errorReason = (string) null;
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(1051790, ServiceHooksConsumerService.s_area, ServiceHooksConsumerService.s_layer, nameof (AttachSessionTokenToEventIfNeeded));
      try
      {
        SessionTokenConfigurationDescriptor configurationDescriptor = actionImpl.GetSessionTokenConfigurationDescriptor();
        if (configurationDescriptor == null)
          return;
        string name1 = this.GetConsumer(requestContext, actionImpl.ConsumerId).ToConsumer().Name;
        string name2 = actionImpl.Name;
        if (ServiceHooksConsumerService.CanIssueSessionToken(notification, out errorReason))
        {
          try
          {
            NotificationToken token = ServiceHooksTokenHelper.GetToken(requestContext, notification, configurationDescriptor, string.Format(ServiceHooksConsumerResources.SessionTokenNameFormat, (object) name1, (object) name2));
            notification.Details.Event.SessionToken = new SessionToken()
            {
              Token = token.AccessToken,
              ValidTo = token.ExpirationDate
            };
            return;
          }
          catch (NotificationTokenAcquisitionException ex)
          {
            errorReason = ex.Reason;
          }
          catch (Exception ex)
          {
            errorReason = ex.Message;
          }
        }
        string message = string.Format(ServiceHooksConsumerResources.Error_CouldNotIssueSessionTokenForConsumerAction, (object) name1, (object) name2, (object) errorReason);
        if (configurationDescriptor.Required)
          throw requestContext.TraceThrow<ServiceHookException>(1051800, ServiceHooksConsumerService.s_area, ServiceHooksConsumerService.s_layer, new ServiceHookException(message));
        notification.Details.Event.SessionToken = new SessionToken()
        {
          Error = message
        };
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1051800, ServiceHooksConsumerService.s_area, ServiceHooksConsumerService.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1051810, ServiceHooksConsumerService.s_area, ServiceHooksConsumerService.s_layer, string.Format("AttachSessionTokenToEventIfNeeded (Token Error: {0})", (object) (errorReason ?? string.Empty)));
      }
    }

    private static bool CanIssueSessionToken(Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification notification, out string errorReason)
    {
      errorReason = (string) null;
      if (!(notification is TestNotification) || !(notification.SubscriberId != ((TestNotification) notification).TesterUserId))
        return true;
      errorReason = ServiceHooksConsumerResources.Error_CannotIssueSessionTokensForTestNotifications_Reason_TesterIsNotTheSubscriber;
      return false;
    }

    private void LoadConsumers(IVssRequestContext systemRequestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(systemRequestContext, nameof (systemRequestContext));
      systemRequestContext.TraceEnter(1063430, ServiceHooksConsumerService.s_area, ServiceHooksConsumerService.s_layer, nameof (LoadConsumers));
      try
      {
        this.m_consumerImpls = systemRequestContext.GetExtensions<ConsumerImplementation>();
        if (this.m_consumerImpls.GroupBy<ConsumerImplementation, string>((Func<ConsumerImplementation, string>) (c => c.Id)).Where<IGrouping<string, ConsumerImplementation>>((Func<IGrouping<string, ConsumerImplementation>, bool>) (g => g.Count<ConsumerImplementation>() > 1)).Any<IGrouping<string, ConsumerImplementation>>())
          throw new ServiceHookException(ServiceHooksConsumerResources.Error_DuplicateConsumerIdentifier);
        this.m_consumerImplsById = new Dictionary<string, ConsumerImplementation>(this.m_consumerImpls.Count);
        foreach (ConsumerImplementation consumerImpl in (IEnumerable<ConsumerImplementation>) this.m_consumerImpls)
        {
          this.m_consumerImplsById.Add(consumerImpl.Id, consumerImpl);
          if (consumerImpl.InputDescriptors != null && consumerImpl.InputDescriptors.Count != 0 && consumerImpl.InputDescriptors.GroupBy<InputDescriptor, string>((Func<InputDescriptor, string>) (pd => pd.Id)).Where<IGrouping<string, InputDescriptor>>((Func<IGrouping<string, InputDescriptor>, bool>) (g => g.Count<InputDescriptor>() > 1)).Any<IGrouping<string, InputDescriptor>>())
            throw new ServiceHookException(string.Format(ServiceHooksConsumerResources.Error_DuplicateConsumerInputDescriptorIdTemplate, (object) consumerImpl.GetType().AssemblyQualifiedName));
          this.ValidateInputDescriptors(consumerImpl.InputDescriptors);
        }
      }
      catch (Exception ex)
      {
        systemRequestContext.TraceException(1063430, ServiceHooksConsumerService.s_area, ServiceHooksConsumerService.s_layer, ex);
        throw;
      }
      finally
      {
        systemRequestContext.TraceLeave(1063430, ServiceHooksConsumerService.s_area, ServiceHooksConsumerService.s_layer, nameof (LoadConsumers));
      }
    }

    private void LoadConsumerActions(IVssRequestContext systemRequestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(systemRequestContext, nameof (systemRequestContext));
      systemRequestContext.TraceEnter(1063430, ServiceHooksConsumerService.s_area, ServiceHooksConsumerService.s_layer, nameof (LoadConsumerActions));
      try
      {
        this.m_actionImpls = systemRequestContext.GetExtensions<ConsumerActionImplementation>();
        this.m_actionImplsByConsumerId = new Dictionary<string, Dictionary<string, ConsumerActionImplementation>>(this.m_actionImpls.Count);
        this.m_eventHandlerMapsByActionImpl = new Dictionary<ConsumerActionImplementation, Dictionary<string, MethodInfo>>(this.m_actionImpls.Count);
        foreach (ConsumerActionImplementation actionImpl1 in (IEnumerable<ConsumerActionImplementation>) this.m_actionImpls)
        {
          ConsumerActionImplementation actionImpl = actionImpl1;
          ConsumerImplementation consumerImplementation;
          if (!this.m_consumerImplsById.TryGetValue(actionImpl.ConsumerId, out consumerImplementation))
            throw new ServiceHookException(string.Format(ServiceHooksConsumerResources.Error_NoMatchForConsumerIdOnActionTemplate, (object) actionImpl.GetType().AssemblyQualifiedName));
          if (actionImpl.InputDescriptors != null && actionImpl.InputDescriptors.Count != 0 && actionImpl.InputDescriptors.GroupBy<InputDescriptor, string>((Func<InputDescriptor, string>) (pd => pd.Id)).Where<IGrouping<string, InputDescriptor>>((Func<IGrouping<string, InputDescriptor>, bool>) (g => g.Count<InputDescriptor>() > 1)).Any<IGrouping<string, InputDescriptor>>() || consumerImplementation.InputDescriptors != null && consumerImplementation.InputDescriptors.Intersect<InputDescriptor>((IEnumerable<InputDescriptor>) actionImpl.InputDescriptors, (IEqualityComparer<InputDescriptor>) ServiceHooksConsumerService.s_inputDescriptorIdComparer).Count<InputDescriptor>() != 0)
            throw new ServiceHookException(string.Format(ServiceHooksConsumerResources.Error_DuplicateConsumerInputDescriptorIdTemplate, (object) actionImpl.GetType().AssemblyQualifiedName));
          this.ValidateInputDescriptors(actionImpl.InputDescriptors);
          if (consumerImplementation.Actions == null)
            consumerImplementation.Actions = (IList<ConsumerActionImplementation>) new List<ConsumerActionImplementation>();
          consumerImplementation.Actions.Add(actionImpl);
          Dictionary<string, ConsumerActionImplementation> dictionary;
          if (!this.m_actionImplsByConsumerId.TryGetValue(actionImpl.ConsumerId, out dictionary))
          {
            dictionary = new Dictionary<string, ConsumerActionImplementation>(this.m_actionImpls.Where<ConsumerActionImplementation>((Func<ConsumerActionImplementation, bool>) (a => a.ConsumerId == actionImpl.ConsumerId)).Count<ConsumerActionImplementation>());
            this.m_actionImplsByConsumerId.Add(actionImpl.ConsumerId, dictionary);
          }
          dictionary.Add(actionImpl.Id, actionImpl);
          this.MapSupportedEventTypesToEventHandlers(systemRequestContext, actionImpl);
        }
      }
      catch (Exception ex)
      {
        systemRequestContext.TraceException(1063430, ServiceHooksConsumerService.s_area, ServiceHooksConsumerService.s_layer, ex);
        throw;
      }
      finally
      {
        systemRequestContext.TraceLeave(1063430, ServiceHooksConsumerService.s_area, ServiceHooksConsumerService.s_layer, nameof (LoadConsumerActions));
      }
    }

    private void MapSupportedEventTypesToEventHandlers(
      IVssRequestContext requestContext,
      ConsumerActionImplementation actionImpl)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(1063430, ServiceHooksConsumerService.s_area, ServiceHooksConsumerService.s_layer, nameof (MapSupportedEventTypesToEventHandlers));
      try
      {
        ParameterInfo[] parameterInfos;
        IEnumerable<MethodInfo> source = ((IEnumerable<MethodInfo>) actionImpl.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public)).Where<MethodInfo>((Func<MethodInfo, bool>) (mi => mi.Name.Length > "handle".Length && mi.Name.ToLower().StartsWith("handle") && typeof (ActionTask).IsAssignableFrom(mi.ReturnType) && (parameterInfos = mi.GetParameters()).Length == 3 && parameterInfos[0].ParameterType == typeof (IVssRequestContext) && parameterInfos[1].ParameterType == typeof (Event) && parameterInfos[2].ParameterType == typeof (HandleEventArgs)));
        Dictionary<string, MethodInfo> dictionary = new Dictionary<string, MethodInfo>(((IEnumerable<string>) actionImpl.SupportedEventTypes).Count<string>());
        foreach (string supportedEventType in actionImpl.SupportedEventTypes)
        {
          MethodInfo methodInfo = (MethodInfo) null;
          string[] strArray = supportedEventType.ToLower().Split(ServiceHooksConsumerService.s_splitOnPeriod, StringSplitOptions.RemoveEmptyEntries);
          for (int length = strArray.Length; length > 0; --length)
          {
            string eventHandlerSuffixLowercase = string.Join(string.Empty, strArray, 0, length);
            methodInfo = source.SingleOrDefault<MethodInfo>((Func<MethodInfo, bool>) (eh => eh.Name.ToLower() == "handle" + eventHandlerSuffixLowercase));
            if (methodInfo != (MethodInfo) null)
              break;
          }
          if (methodInfo == (MethodInfo) null)
          {
            methodInfo = source.SingleOrDefault<MethodInfo>((Func<MethodInfo, bool>) (eh => eh.Name.ToLower() == "handleevent"));
            if (methodInfo == (MethodInfo) null)
              throw new MissingMethodException(string.Format(ServiceHooksConsumerResources.Error_MatchingEventHandlerMethodNotFoundTemplate, (object) supportedEventType, (object) actionImpl.GetType().AssemblyQualifiedName));
          }
          dictionary.Add(supportedEventType, methodInfo);
        }
        this.m_eventHandlerMapsByActionImpl.Add(actionImpl, dictionary);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1063430, ServiceHooksConsumerService.s_area, ServiceHooksConsumerService.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1063430, ServiceHooksConsumerService.s_area, ServiceHooksConsumerService.s_layer, nameof (MapSupportedEventTypesToEventHandlers));
      }
    }

    private void ValidateInputDescriptors(IList<InputDescriptor> inputDescriptors)
    {
      foreach (InputDescriptor inputDescriptor in (IEnumerable<InputDescriptor>) inputDescriptors)
        ArgumentUtility.CheckStringForNullOrWhiteSpace(inputDescriptor.Name, "InputDescriptor.Name");
    }

    private class InputDescriptorIdComparer : IEqualityComparer<InputDescriptor>
    {
      public bool Equals(InputDescriptor sid1, InputDescriptor sid2) => sid1.Id == sid2.Id;

      public int GetHashCode(InputDescriptor obj) => obj.Id.GetHashCode();
    }
  }
}

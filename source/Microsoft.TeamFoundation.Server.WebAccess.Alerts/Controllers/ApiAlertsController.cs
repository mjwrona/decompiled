// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Alerts.Controllers.ApiAlertsController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Alerts, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0FF2CB39-6514-430A-A4E9-A45535A580D6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Alerts.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Notifications;
using Microsoft.VisualStudio.Services.Notifications.Common;
using Microsoft.VisualStudio.Services.Notifications.Server;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Alerts.Controllers
{
  [SupportedRouteArea("Api", NavigationContextLevels.Collection | NavigationContextLevels.Project | NavigationContextLevels.Team)]
  [Microsoft.TeamFoundation.Server.WebAccess.DemandFeature("00000000-0000-0000-0000-000000000000", false)]
  [OutputCache(CacheProfile = "NoCache")]
  public class ApiAlertsController : AlertsAreaController
  {
    private const int WebAccessExceptionEaten = 599999;
    private const string s_subscriptionToken = "$SUBSCRIPTION:";

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(511000, 511010)]
    public ActionResult Index() => (ActionResult) this.View();

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(511040, 511050)]
    public ActionResult Subscriptions(
      ApiSubscriptionScope? scope,
      string userId,
      string filter,
      string classification = "",
      bool includeTeamsAlerts = false)
    {
      INotificationSubscriptionService service1 = this.TfsRequestContext.GetService<INotificationSubscriptionService>();
      TeamFoundationIdentityService service2 = this.TfsRequestContext.GetService<TeamFoundationIdentityService>();
      IVssSecurityNamespace securityNamespace = this.TfsRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(this.TfsRequestContext, FrameworkSecurity.EventSubscriptionNamespaceId);
      Dictionary<string, TfsSubscriptionAdapter> dictionary = new Dictionary<string, TfsSubscriptionAdapter>();
      if (!scope.HasValue)
        scope = new ApiSubscriptionScope?(ApiSubscriptionScope.My);
      if (string.IsNullOrWhiteSpace(classification))
        classification = string.Empty;
      List<Microsoft.VisualStudio.Services.Notifications.Server.Subscription> source = new List<Microsoft.VisualStudio.Services.Notifications.Server.Subscription>();
      IEnumerable<SubscriberInfoModel> subscriberInfoModels = (IEnumerable<SubscriberInfoModel>) Array.Empty<SubscriberInfoModel>();
      if (!string.IsNullOrEmpty(userId))
      {
        Guid tfid = new Guid(userId);
        TeamFoundationIdentity readIdentity = service2.ReadIdentities(this.TfsRequestContext, new Guid[1]
        {
          tfid
        }, MembershipQuery.Expanded, ReadIdentityOptions.ExtendedProperties, (IEnumerable<string>) new string[1]
        {
          "CustomNotificationAddresses"
        }, IdentityPropertyScope.Global)[0];
        if (readIdentity == null)
          throw new IdentityNotFoundException(tfid);
        Dictionary<Guid, SubscriberInfoModel> subscriberModels = new Dictionary<Guid, SubscriberInfoModel>();
        source.AddRange((IEnumerable<Microsoft.VisualStudio.Services.Notifications.Server.Subscription>) this.GetAllUserTeamsSubscriptions(service1, service2, securityNamespace, readIdentity, subscriberModels, classification, false));
        subscriberInfoModels = (IEnumerable<SubscriberInfoModel>) subscriberModels.Values;
      }
      else if (scope.Value == ApiSubscriptionScope.Team)
      {
        if (this.TfsWebContext.Team != null)
        {
          IVssRequestContext requestContext = this.TfsRequestContext;
          if (this.GetGroupsWithManageMembershipPermission((IEnumerable<TeamFoundationIdentity>) new TeamFoundationIdentity[1]
          {
            IdentityUtil.Convert(this.TfsWebContext.Team.Identity)
          }).Count == 1)
            requestContext = this.TfsRequestContext.CreateUserContext(this.TfsWebContext.Team.Identity.Descriptor);
          try
          {
            SubscriptionLookup anyFieldLookup = SubscriptionLookup.CreateAnyFieldLookup(subscriberId: new Guid?(this.TfsWebContext.Team.Identity.Id), matcher: "XPathMatcher", dataspaceId: new Guid?(this.TfsWebContext.CurrentProjectGuid), classification: classification);
            source = service1.QuerySubscriptions(requestContext, anyFieldLookup);
            subscriberInfoModels = (IEnumerable<SubscriberInfoModel>) new SubscriberInfoModel[1]
            {
              new SubscriberInfoModel(IdentityUtil.Convert(this.TfsWebContext.Team.Identity))
            };
          }
          finally
          {
            if (requestContext != this.TfsRequestContext)
              requestContext.Dispose();
          }
        }
      }
      else if (scope.Value == ApiSubscriptionScope.All)
      {
        SubscriptionLookup matcherLookup = SubscriptionLookup.CreateMatcherLookup("XPathMatcher");
        source = service1.QuerySubscriptions(this.TfsRequestContext, matcherLookup);
        subscriberInfoModels = this.GetSubscribers(service2, (IEnumerable<Microsoft.VisualStudio.Services.Notifications.Server.Subscription>) source).Select<TeamFoundationIdentity, SubscriberInfoModel>((Func<TeamFoundationIdentity, SubscriberInfoModel>) (id => new SubscriberInfoModel(id)));
      }
      else if (!includeTeamsAlerts)
      {
        Guid? subscriberId = new Guid?(this.TfsRequestContext.GetUserId());
        string classification1 = classification;
        Guid? dataspaceId = new Guid?(this.TfsWebContext.CurrentProjectGuid);
        SubscriptionLookup anyFieldLookup = SubscriptionLookup.CreateAnyFieldLookup(subscriberId: subscriberId, matcher: "XPathMatcher", dataspaceId: dataspaceId, classification: classification1);
        source = service1.QuerySubscriptions(this.TfsRequestContext, anyFieldLookup);
        subscriberInfoModels = (IEnumerable<SubscriberInfoModel>) new SubscriberInfoModel[1]
        {
          new SubscriberInfoModel(service2.ReadIdentity(this.TfsRequestContext, IdentitySearchFactor.Identifier, this.TfsRequestContext.UserContext.Identifier, MembershipQuery.None, ReadIdentityOptions.ExtendedProperties, (IEnumerable<string>) new string[1]
          {
            "CustomNotificationAddresses"
          }, IdentityPropertyScope.Global))
        };
      }
      else
      {
        TeamFoundationIdentity user = service2.ReadIdentity(this.TfsRequestContext, this.TfsRequestContext.UserContext, MembershipQuery.Expanded, ReadIdentityOptions.ExtendedProperties, (IEnumerable<string>) new string[1]
        {
          "CustomNotificationAddresses"
        }, IdentityPropertyScope.Global);
        if (user == null)
          throw new IdentityNotFoundException(this.TfsRequestContext.UserContext);
        Dictionary<Guid, SubscriberInfoModel> subscriberModels = new Dictionary<Guid, SubscriberInfoModel>();
        source.AddRange((IEnumerable<Microsoft.VisualStudio.Services.Notifications.Server.Subscription>) this.GetAllUserTeamsSubscriptions(service1, service2, securityNamespace, user, subscriberModels, classification, true));
        subscriberInfoModels = (IEnumerable<SubscriberInfoModel>) subscriberModels.Values.ToArray<SubscriberInfoModel>();
      }
      List<Microsoft.VisualStudio.Services.Notifications.Server.Subscription> list = source.Where<Microsoft.VisualStudio.Services.Notifications.Server.Subscription>((Func<Microsoft.VisualStudio.Services.Notifications.Server.Subscription, bool>) (x => x.Status != SubscriptionStatus.PendingDeletion && !x.IsServiceHooksDelivery && x.ScopeId.Equals(Guid.Empty))).ToList<Microsoft.VisualStudio.Services.Notifications.Server.Subscription>();
      DataContractJsonResult contractJsonResult = new DataContractJsonResult((object) new SubscriptionsCollectionModel()
      {
        Subscriptions = list.Select<Microsoft.VisualStudio.Services.Notifications.Server.Subscription, SubscriptionModel>((Func<Microsoft.VisualStudio.Services.Notifications.Server.Subscription, SubscriptionModel>) (subscription => new SubscriptionModel(this.TfsRequestContext, subscription, TfsSubscriptionAdapter.CreateAdapter(this.TfsRequestContext, this.TfsWebContext, subscription.SubscriptionFilter.EventType)))),
        Subscribers = subscriberInfoModels
      });
      contractJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) contractJsonResult;
    }

    private List<Microsoft.VisualStudio.Services.Notifications.Server.Subscription> GetAllUserTeamsSubscriptions(
      INotificationSubscriptionService notificationSvc,
      TeamFoundationIdentityService identityService,
      IVssSecurityNamespace eventSecurity,
      TeamFoundationIdentity user,
      Dictionary<Guid, SubscriberInfoModel> subscriberModels,
      string classification,
      bool checkPermissions)
    {
      SubscriptionLookup[] subscriptionLookupArray = new SubscriptionLookup[2];
      string str = classification;
      Guid? dataspaceId1 = new Guid?(this.TfsWebContext.CurrentProjectGuid);
      SubscriptionFlags? nullable = new SubscriptionFlags?(SubscriptionFlags.GroupSubscription);
      int? subscriptionId = new int?();
      Guid? subscriberId1 = new Guid?();
      Guid? dataspaceId2 = dataspaceId1;
      string classification1 = str;
      SubscriptionFlags? flags = nullable;
      Guid? scopeId = new Guid?();
      Guid? subscriberId2 = new Guid?();
      Guid? uniqueId = subscriberId2;
      subscriptionLookupArray[0] = SubscriptionLookup.CreateAnyFieldLookup(subscriptionId, subscriberId: subscriberId1, matcher: "XPathMatcher", dataspaceId: dataspaceId2, classification: classification1, flags: flags, scopeId: scopeId, uniqueId: uniqueId);
      string classification2 = classification;
      dataspaceId1 = new Guid?(this.TfsWebContext.CurrentProjectGuid);
      subscriberId2 = new Guid?(user.TeamFoundationId);
      subscriptionLookupArray[1] = SubscriptionLookup.CreateAnyFieldLookup(subscriberId: subscriberId2, matcher: "XPathMatcher", dataspaceId: dataspaceId1, classification: classification2);
      SubscriptionLookup[] subscriptionKeys = subscriptionLookupArray;
      List<Microsoft.VisualStudio.Services.Notifications.Server.Subscription> source = notificationSvc.QuerySubscriptions(this.TfsRequestContext, (IEnumerable<SubscriptionLookup>) subscriptionKeys);
      Guid[] array = source.Select<Microsoft.VisualStudio.Services.Notifications.Server.Subscription, Guid>((Func<Microsoft.VisualStudio.Services.Notifications.Server.Subscription, Guid>) (sub => sub.SubscriberId)).Distinct<Guid>().ToArray<Guid>();
      foreach (TeamFoundationIdentity identity in ((IEnumerable<TeamFoundationIdentity>) identityService.ReadIdentities(this.TfsRequestContext, array, MembershipQuery.None, ReadIdentityOptions.ExtendedProperties, (IEnumerable<string>) new string[1]
      {
        "CustomNotificationAddresses"
      }, IdentityPropertyScope.Global)).Where<TeamFoundationIdentity>((Func<TeamFoundationIdentity, bool>) (tfId => tfId != null)))
      {
        if (identity != null && !subscriberModels.ContainsKey(identity.TeamFoundationId))
          subscriberModels[identity.TeamFoundationId] = new SubscriberInfoModel(identity);
      }
      SubscriberInfoModel subscriberInfoModel = new SubscriberInfoModel(user);
      subscriberModels[user.TeamFoundationId] = subscriberInfoModel;
      return source;
    }

    private IEnumerable<TeamFoundationIdentity> GetSubscribers(
      TeamFoundationIdentityService identityService,
      IEnumerable<Microsoft.VisualStudio.Services.Notifications.Server.Subscription> subscriptions)
    {
      if (!subscriptions.Any<Microsoft.VisualStudio.Services.Notifications.Server.Subscription>())
        return (IEnumerable<TeamFoundationIdentity>) Array.Empty<TeamFoundationIdentity>();
      Guid[] array = subscriptions.Select<Microsoft.VisualStudio.Services.Notifications.Server.Subscription, Guid>((Func<Microsoft.VisualStudio.Services.Notifications.Server.Subscription, Guid>) (sub => sub.SubscriberId)).Distinct<Guid>().ToArray<Guid>();
      return ((IEnumerable<TeamFoundationIdentity>) identityService.ReadIdentities(this.TfsRequestContext, array, MembershipQuery.None, ReadIdentityOptions.ExtendedProperties, (IEnumerable<string>) new string[1]
      {
        "CustomNotificationAddresses"
      }, IdentityPropertyScope.Global)).Where<TeamFoundationIdentity>((Func<TeamFoundationIdentity, bool>) (tfId => tfId != null));
    }

    [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Put)]
    [ValidateInput(false)]
    public ActionResult SaveSubscriptions(string[] subscriptionsJson, bool skipWarning = true)
    {
      ArgumentUtility.CheckForNull<string[]>(subscriptionsJson, nameof (subscriptionsJson));
      SubscriptionModel[] subscriptionsJson1 = this.ParseSubscriptionsJSON(subscriptionsJson);
      HashSet<Guid> source1 = new HashSet<Guid>();
      foreach (SubscriptionModel subscriptionModel in subscriptionsJson1)
        source1.Add(subscriptionModel.SubscriberId);
      source1.Add(this.TfsRequestContext.GetUserId());
      IEnumerable<TeamFoundationIdentity> source2 = ((IEnumerable<TeamFoundationIdentity>) this.TfsRequestContext.GetService<TeamFoundationIdentityService>().ReadIdentities(this.TfsRequestContext, source1.ToArray<Guid>())).Where<TeamFoundationIdentity>((Func<TeamFoundationIdentity, bool>) (id => id != null));
      Dictionary<Guid, TeamFoundationIdentity> dictionary = source2.ToDictionary<TeamFoundationIdentity, Guid>((Func<TeamFoundationIdentity, Guid>) (id => id.TeamFoundationId));
      HashSet<TeamFoundationIdentity> membershipPermission = this.GetGroupsWithManageMembershipPermission(source2.Where<TeamFoundationIdentity>((Func<TeamFoundationIdentity, bool>) (id => id.IsContainer)));
      INotificationSubscriptionService service = this.TfsRequestContext.GetService<INotificationSubscriptionService>();
      for (int index = 0; index < subscriptionsJson1.Length; ++index)
      {
        subscriptionsJson1[index].LastError = (string) null;
        subscriptionsJson1[index].LastWarning = (string) null;
        if (!string.IsNullOrEmpty(subscriptionsJson1[index].Name) && subscriptionsJson1[index].Name.Length > (int) byte.MaxValue)
        {
          subscriptionsJson1[index].LastError = AlertsServerResources.ErrorAlertNameTooLong;
        }
        else
        {
          IVssRequestContext requestContext = this.TfsRequestContext;
          try
          {
            ArgumentUtility.CheckForNull<DeliveryPreferenceModel>(subscriptionsJson1[index].DeliveryPreference, "DeliveryPreference");
            ArgumentUtility.CheckForNull<ExpressionFilterModel>(subscriptionsJson1[index].Filter, "Filter");
            TeamFoundationIdentity foundationIdentity;
            if (!dictionary.TryGetValue(subscriptionsJson1[index].SubscriberId, out foundationIdentity))
            {
              subscriptionsJson1[index].LastError = string.Format(AlertsServerResources.ErrorIdentityCouldNotBeFoundFormat, (object) subscriptionsJson1[index].SubscriberId);
            }
            else
            {
              DeliveryPreference preference = subscriptionsJson1[index].DeliveryPreference.GetPreference();
              if (foundationIdentity.IsContainer)
              {
                if (!membershipPermission.Contains(foundationIdentity))
                {
                  subscriptionsJson1[index].LastError = string.Format(AlertsServerResources.ErrorNoManageGroupPermission, (object) this.TfsWebContext.CurrentUserDisplayName, (object) foundationIdentity.DisplayName);
                  continue;
                }
                requestContext = this.TfsRequestContext.CreateUserContext(foundationIdentity.Descriptor);
              }
              Microsoft.VisualStudio.Services.Identity.Identity userIdentity = this.TfsRequestContext.GetUserIdentity();
              TfsSubscriptionAdapter adapter = TfsSubscriptionAdapter.CreateAdapter(this.TfsRequestContext, this.TfsWebContext, subscriptionsJson1[index].SubscriptionType);
              string conditionString = adapter.ToConditionString(this.TfsRequestContext, subscriptionsJson1[index].Filter);
              string matcher = adapter.GetMatcher(this.TfsRequestContext, subscriptionsJson1[index].EventTypeName);
              EventSerializerType serializationFormatForEvent = adapter.GetSerializationFormatForEvent(this.TfsRequestContext, subscriptionsJson1[index].EventTypeName);
              Microsoft.VisualStudio.Services.Notifications.Server.Subscription subscription = new Microsoft.VisualStudio.Services.Notifications.Server.Subscription((ISubscriptionAdapter) adapter)
              {
                ID = subscriptionsJson1[index].Id,
                ConditionString = conditionString,
                DeliveryAddress = preference.Address,
                Channel = DeliveryTypeChannelMapper.GetChannelName(subscriptionsJson1[index].DeliveryPreference.Type),
                Description = subscriptionsJson1[index].Name,
                LastModifiedBy = userIdentity.Id,
                SubscriberId = foundationIdentity.TeamFoundationId,
                SubscriptionFilter = (ISubscriptionFilter) new ExpressionFilter(subscriptionsJson1[index].EventTypeName),
                ProjectId = this.TfsWebContext.CurrentProjectGuid,
                Tag = subscriptionsJson1[index].GetTagForSave(),
                Matcher = matcher
              };
              this.ValidateConditionString(serializationFormatForEvent, subscription.ConditionString);
              if (subscriptionsJson1[index].Id > 0)
              {
                SubscriptionUpdate subscriptionUpdate = new SubscriptionUpdate(subscriptionsJson1[index].Id)
                {
                  Expression = subscription.ConditionString,
                  Channel = subscription.Channel,
                  Description = subscription.Description,
                  Classification = subscription.Tag,
                  LastModifiedBy = new Guid?(subscription.LastModifiedBy),
                  Address = subscription.DeliveryAddress
                };
                service.UpdateSubscription(requestContext, subscription, subscriptionUpdate);
              }
              else
                subscriptionsJson1[index].Id = service.CreateSubscription(requestContext, subscription);
              subscriptionsJson1[index].LastWarning = subscription.Warning;
            }
          }
          catch (Exception ex)
          {
            this.TraceException(599999, ex);
            subscriptionsJson1[index].LastError = ex.Message;
          }
          finally
          {
            if (requestContext != this.TfsRequestContext)
              requestContext.Dispose();
          }
        }
      }
      DataContractJsonResult contractJsonResult = new DataContractJsonResult((object) subscriptionsJson1);
      contractJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) contractJsonResult;
    }

    [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Put)]
    [ValidateInput(false)]
    public ActionResult DeleteSubscriptions(string[] alertsToDeleteJson)
    {
      ArgumentUtility.CheckForNull<string[]>(alertsToDeleteJson, nameof (alertsToDeleteJson));
      SubscriptionModel[] subscriptionsJson = this.ParseSubscriptionsJSON(alertsToDeleteJson);
      Guid[] array = ((IEnumerable<SubscriptionModel>) subscriptionsJson).Select<SubscriptionModel, Guid>((Func<SubscriptionModel, Guid>) (sub => sub.SubscriberId)).Distinct<Guid>().ToArray<Guid>();
      IEnumerable<TeamFoundationIdentity> source = ((IEnumerable<TeamFoundationIdentity>) this.TfsRequestContext.GetService<TeamFoundationIdentityService>().ReadIdentities(this.TfsRequestContext, array)).Where<TeamFoundationIdentity>((Func<TeamFoundationIdentity, bool>) (id => id != null));
      Dictionary<Guid, TeamFoundationIdentity> dictionary = source.ToDictionary<TeamFoundationIdentity, Guid>((Func<TeamFoundationIdentity, Guid>) (id => id.TeamFoundationId));
      HashSet<TeamFoundationIdentity> membershipPermission = this.GetGroupsWithManageMembershipPermission(source.Where<TeamFoundationIdentity>((Func<TeamFoundationIdentity, bool>) (id => id.IsContainer)));
      INotificationSubscriptionService service = this.TfsRequestContext.GetService<INotificationSubscriptionService>();
      List<string> stringList = new List<string>();
      List<int> intList = new List<int>();
      foreach (SubscriptionModel subscriptionModel in subscriptionsJson)
      {
        IVssRequestContext requestContext = this.TfsRequestContext;
        try
        {
          TeamFoundationIdentity foundationIdentity;
          if (!dictionary.TryGetValue(subscriptionModel.SubscriberId, out foundationIdentity))
          {
            stringList.Add(string.Format(AlertsServerResources.ErrorIdentityCouldNotBeFoundFormat, (object) subscriptionModel.SubscriberId));
            intList.Add(subscriptionModel.Id);
          }
          else
          {
            if (foundationIdentity.IsContainer)
            {
              if (!membershipPermission.Contains(foundationIdentity))
              {
                stringList.Add(string.Format(AlertsServerResources.ErrorNoManageGroupPermission, (object) this.TfsWebContext.CurrentUserDisplayName, (object) foundationIdentity.DisplayName));
                intList.Add(subscriptionModel.Id);
                continue;
              }
              requestContext = this.TfsRequestContext.CreateUserContext(foundationIdentity.Descriptor);
            }
            service.DeleteSubscription(requestContext, subscriptionModel.Id);
          }
        }
        catch (Exception ex)
        {
          this.TraceException(5200016, ex);
          stringList.Add(string.Format(AlertsServerResources.UnsubscribeErrorFormat, (object) subscriptionModel.Id, (object) ex.Message));
          intList.Add(subscriptionModel.Id);
        }
        finally
        {
          if (requestContext != this.TfsRequestContext)
            requestContext.Dispose();
        }
      }
      JsObject data = new JsObject();
      data["errors"] = (object) stringList;
      data["idsWithErrors"] = (object) intList;
      return (ActionResult) this.Json((object) data, JsonRequestBehavior.AllowGet);
    }

    private SubscriptionModel[] ParseSubscriptionsJSON(string[] subscriptionsJson)
    {
      bool enableSafeDeserializer = this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.EnableSafeDeserializer");
      return ((IEnumerable<string>) subscriptionsJson).Where<string>((Func<string, bool>) (subscriptionJson => !string.IsNullOrEmpty(subscriptionJson))).Select<string, SubscriptionModel>((Func<string, SubscriptionModel>) (subscriptionJson => !enableSafeDeserializer ? new JavaScriptSerializer().Deserialize<SubscriptionModel>(subscriptionJson) : JsonConvert.DeserializeObject<SubscriptionModel>(subscriptionJson))).Where<SubscriptionModel>((Func<SubscriptionModel, bool>) (subscription => subscription != null)).ToArray<SubscriptionModel>();
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult GetSubscriptionFilterFields(string type)
    {
      List<ExpressionFilterField> source = new List<ExpressionFilterField>();
      TfsSubscriptionAdapter adapter = TfsSubscriptionAdapter.CreateAdapter(this.TfsRequestContext, this.TfsWebContext, type);
      source.AddRange((IEnumerable<ExpressionFilterField>) adapter.GetFields(this.TfsRequestContext, this.TfsWebContext).Values.ToList<ExpressionFilterField>());
      return (ActionResult) this.Json((object) source.OrderBy<ExpressionFilterField, string>((Func<ExpressionFilterField, string>) (f => f.LocalizedFieldName), (IComparer<string>) StringComparer.CurrentCultureIgnoreCase).Select<ExpressionFilterField, Dictionary<string, object>>((Func<ExpressionFilterField, Dictionary<string, object>>) (f => f.ToJson(this.TfsRequestContext))).ToArray<Dictionary<string, object>>(), JsonRequestBehavior.AllowGet);
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult GetBasicSubscriptionTemplates()
    {
      DataContractJsonResult subscriptionTemplates = new DataContractJsonResult((object) SubscriptionTemplate.GetBasicTemplates(this.TfsRequestContext, this.TfsWebContext).ToList<SubscriptionTemplate>());
      subscriptionTemplates.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) subscriptionTemplates;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult GetCustomSubscriptionTemplates()
    {
      DataContractJsonResult subscriptionTemplates = new DataContractJsonResult((object) SubscriptionTemplate.GetCustomTemplates(this.TfsRequestContext, this.TfsWebContext).OrderBy<SubscriptionTemplate, string>((Func<SubscriptionTemplate, string>) (t => t.SubscriptionType.ToString()), (IComparer<string>) StringComparer.CurrentCultureIgnoreCase).ThenBy<SubscriptionTemplate, string>((Func<SubscriptionTemplate, string>) (t => t.TemplateName), (IComparer<string>) StringComparer.CurrentCultureIgnoreCase).ToList<SubscriptionTemplate>());
      subscriptionTemplates.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) subscriptionTemplates;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult GetChatRoomSubscriptionTemplates()
    {
      DataContractJsonResult subscriptionTemplates = new DataContractJsonResult((object) SubscriptionTemplate.GetChatRoomTemplates(this.TfsRequestContext, this.TfsWebContext).ToList<SubscriptionTemplate>());
      subscriptionTemplates.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) subscriptionTemplates;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult GetCustomNotificationAddress()
    {
      TeamFoundationIdentity foundationIdentity = this.TfsRequestContext.GetService<TeamFoundationIdentityService>().ReadIdentity(this.TfsRequestContext, IdentitySearchFactor.Identifier, this.TfsRequestContext.UserContext.Identifier, MembershipQuery.None, ReadIdentityOptions.ExtendedProperties, (IEnumerable<string>) new string[1]
      {
        "CustomNotificationAddresses"
      });
      string str = (string) null;
      if (foundationIdentity != null)
      {
        object obj = (object) null;
        if (foundationIdentity.TryGetProperty(IdentityPropertyScope.Global, "CustomNotificationAddresses", out obj))
          str = obj as string;
      }
      return (ActionResult) this.Json((object) new
      {
        customAddress = str
      }, JsonRequestBehavior.AllowGet);
    }

    [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Put)]
    [ValidateInput(false)]
    public ActionResult SetCustomNotificationAddress(string customNotificationAddress)
    {
      string preferredEmailAddress = (string) null;
      if (customNotificationAddress != null)
      {
        customNotificationAddress = customNotificationAddress.Trim();
        if (string.IsNullOrEmpty(customNotificationAddress))
        {
          preferredEmailAddress = string.Empty;
        }
        else
        {
          try
          {
            preferredEmailAddress = new MailAddress(customNotificationAddress).Address;
          }
          catch (FormatException ex)
          {
            throw new TeamFoundationServiceException(string.Format(AlertsResources.InvalidEmailAddressFormat, (object) customNotificationAddress));
          }
        }
      }
      this.TfsRequestContext.GetService<TeamFoundationIdentityService>().SetPreferredEmailAddress(this.TfsRequestContext, preferredEmailAddress);
      return this.GetCustomNotificationAddress();
    }

    private void ValidateConditionString(EventSerializerType serializer, string conditionString)
    {
      TeamFoundationEventConditionParser parser = TeamFoundationEventConditionParser.GetParser(serializer, conditionString);
      try
      {
        parser.Parse();
      }
      catch (Exception ex)
      {
        throw new ArgumentException(string.Format(AlertsServerResources.InvalidXPathExpressionMessageFormat, (object) ex.Message));
      }
    }

    private HashSet<TeamFoundationIdentity> GetGroupsWithManageMembershipPermission(
      IEnumerable<TeamFoundationIdentity> groups)
    {
      HashSet<TeamFoundationIdentity> membershipPermission = new HashSet<TeamFoundationIdentity>();
      if (groups.Any<TeamFoundationIdentity>())
      {
        TeamFoundationIdentityService service = this.TfsRequestContext.GetService<TeamFoundationIdentityService>();
        IVssSecurityNamespace securityNamespace = this.TfsRequestContext.GetService<SecuredTeamFoundationSecurityService>().GetSecurityNamespace(this.TfsRequestContext, FrameworkSecurity.IdentitiesNamespaceId);
        foreach (TeamFoundationIdentity group in groups)
        {
          string securableToken = service.GetSecurableToken(this.TfsRequestContext, group.Descriptor, out TeamFoundationIdentity _);
          if (securityNamespace.HasPermission(this.TfsRequestContext, securableToken, 8))
            membershipPermission.Add(group);
        }
      }
      return membershipPermission;
    }
  }
}

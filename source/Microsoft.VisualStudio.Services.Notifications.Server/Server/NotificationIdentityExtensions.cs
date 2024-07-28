// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.NotificationIdentityExtensions
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal static class NotificationIdentityExtensions
  {
    internal static IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      this IdentityService identityService,
      IVssRequestContext requestContext,
      IList<Guid> identityIds,
      Stopwatch stopwatch = null)
    {
      try
      {
        stopwatch?.Start();
        return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) identityService.ReadIdentities(requestContext, identityIds, QueryMembership.Direct, (IEnumerable<string>) null).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (i => i.IsUsable() && i.CanReceiveMail(requestContext))).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
      }
      finally
      {
        stopwatch?.Stop();
      }
    }

    internal static IList<VsIdentityWithGroupInformation> ReadIdentitiesWithAllGroups(
      this IdentityService identityService,
      IVssRequestContext requestContext,
      IList<Guid> identityIds,
      HashSet<Guid> missingIdenties,
      Stopwatch stopwatch = null)
    {
      try
      {
        stopwatch?.Start();
        missingIdenties.AddRange<Guid, HashSet<Guid>>((IEnumerable<Guid>) identityIds);
        return (IList<VsIdentityWithGroupInformation>) identityService.ReadIdentities(requestContext, identityIds, QueryMembership.Direct, (IEnumerable<string>) null).Select<Microsoft.VisualStudio.Services.Identity.Identity, VsIdentityWithGroupInformation>((Func<Microsoft.VisualStudio.Services.Identity.Identity, VsIdentityWithGroupInformation>) (i =>
        {
          if (!i.IsUsable())
            return (VsIdentityWithGroupInformation) null;
          missingIdenties.Remove(i.Id);
          return new VsIdentityWithGroupInformation()
          {
            Identity = i,
            IsAadGroup = i.IsContainer && AadIdentityHelper.IsAadGroup(i.Descriptor)
          };
        })).Where<VsIdentityWithGroupInformation>((Func<VsIdentityWithGroupInformation, bool>) (i => i != null)).ToList<VsIdentityWithGroupInformation>();
      }
      finally
      {
        stopwatch?.Stop();
      }
    }

    internal static Microsoft.VisualStudio.Services.Identity.Identity GetProjectValidUsersGroup(
      this IdentityService identityService,
      IVssRequestContext requestContext,
      Guid projectId)
    {
      IdentityScope scope = identityService.GetScope(requestContext, projectId);
      IdentityDescriptor identityDescriptor = IdentityMapper.MapFromWellKnownIdentifier(GroupWellKnownIdentityDescriptors.EveryoneGroup, scope.Id);
      return identityService.ReadIdentities(requestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        identityDescriptor
      }, QueryMembership.None, (IEnumerable<string>) null)[0];
    }

    internal static IList<Microsoft.VisualStudio.Services.Identity.Identity> ExpandIdentities(
      this IdentityService identityService,
      IVssRequestContext requestContext,
      IList<Guid> identityIds,
      ExpansionFlags expansionFlags,
      HashSet<Guid> exclusionIds = null,
      ProcessingIdentities diagProcessingIdentities = null,
      int maxDepth = 32,
      int maxIdentities = 2147483647,
      Stopwatch identitiesStopwatch = null,
      Stopwatch subscriberStopWatch = null)
    {
      try
      {
        identitiesStopwatch?.Start();
        HashSet<Guid> processedIds = new HashSet<Guid>();
        if (exclusionIds != null)
          exclusionIds.ForEach<Guid>((Action<Guid>) (id => processedIds.Add(id)));
        NotificationSubscriberService service = requestContext.GetService<NotificationSubscriberService>();
        Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> identityMap = new Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>();
        if (diagProcessingIdentities != null)
        {
          diagProcessingIdentities.Properties[nameof (expansionFlags)] = expansionFlags.ToString();
          diagProcessingIdentities.Properties[nameof (maxDepth)] = maxDepth.ToString();
          diagProcessingIdentities.Properties[nameof (maxIdentities)] = maxIdentities.ToString();
          diagProcessingIdentities.Properties[nameof (identityIds)] = string.Join<Guid>(", ", (IEnumerable<Guid>) identityIds);
        }
        NotificationIdentityExtensions.ExpandIdentitiesRecursively(requestContext, identityService, service, identityIds, processedIds, diagProcessingIdentities, identityMap, maxDepth, maxIdentities, expansionFlags, subscriberStopWatch);
        return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) identityMap.Values.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (vsid => exclusionIds == null || !exclusionIds.Contains(vsid.Id))).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
      }
      finally
      {
        identitiesStopwatch?.Stop();
      }
    }

    private static void ExpandIdentitiesRecursively(
      IVssRequestContext requestContext,
      IdentityService identityService,
      NotificationSubscriberService subscriberService,
      IList<Guid> identityIds,
      HashSet<Guid> processedIds,
      ProcessingIdentities diagProcessingIdentities,
      Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> identityMap,
      int maxDepth,
      int maxIdentities,
      ExpansionFlags expansionFlags,
      Stopwatch subscriberStopWatch)
    {
      HashSet<Guid> source = new HashSet<Guid>();
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = identityService.ReadIdentities(requestContext, identityIds, QueryMembership.Direct, (IEnumerable<string>) null);
      bool flag = maxDepth > 1;
      foreach (Guid identityId in (IEnumerable<Guid>) identityIds)
        processedIds.Add(identityId);
      if (diagProcessingIdentities != null)
        diagProcessingIdentities.MissingIdentities.AddRange<Guid, ISet<Guid>>((IEnumerable<Guid>) identityIds);
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identityList)
      {
        if (identity.IsUsable())
        {
          diagProcessingIdentities?.MissingIdentities.Remove(identity.Id);
          SubscriberType subscriberType;
          NotificationSubscriber subscriber;
          identity.GetMinimalSubscriberInfo(requestContext, subscriberService, subscriberStopWatch, out subscriberType, out subscriber);
          ProcessingDiagnosticIdentity diagnosticIdentity = diagProcessingIdentities == null ? (ProcessingDiagnosticIdentity) null : identity.ToProcessingDiagnosticIdentity(subscriber);
          if (NotificationIdentityExtensions.IsExpandableGroup(subscriber, subscriberType, expansionFlags, diagnosticIdentity))
          {
            if (expansionFlags.HasFlag((Enum) ExpansionFlags.IncludeExpandedGroups))
              identityMap[identity.Id] = identity;
            if (diagProcessingIdentities != null)
              diagProcessingIdentities.IncludedIdentities.AddOrUpate<Guid, ProcessingDiagnosticIdentity>(diagnosticIdentity.Id, diagnosticIdentity);
            if (!flag)
            {
              if (diagProcessingIdentities != null)
              {
                diagProcessingIdentities.Messages.Append(TraceLevel.Warning, "Expansion limit reached recursion depth");
                continue;
              }
              continue;
            }
            if (!processedIds.Contains(identity.Id))
              source.Add(identity.Id);
            if (identity.MemberIds != null)
            {
              foreach (Guid memberId in (IEnumerable<Guid>) identity.MemberIds)
              {
                if (!processedIds.Contains(memberId))
                {
                  source.Add(memberId);
                  processedIds.Add(memberId);
                }
              }
            }
          }
          else if (identity.CanReceiveMail(subscriber, subscriberType, diagnosticIdentity))
          {
            identityMap[identity.Id] = identity;
            if (diagProcessingIdentities != null)
              diagProcessingIdentities.IncludedIdentities.AddOrUpate<Guid, ProcessingDiagnosticIdentity>(diagnosticIdentity.Id, diagnosticIdentity);
          }
          else if (diagProcessingIdentities != null)
            diagProcessingIdentities.ExcludedIdentities.AddOrUpate<Guid, ProcessingDiagnosticIdentity>(diagnosticIdentity.Id, diagnosticIdentity);
          if (identityMap.Count + source.Count >= maxIdentities)
          {
            if (diagProcessingIdentities != null)
            {
              diagProcessingIdentities.Messages.Append(TraceLevel.Warning, "Expansion limit reached maximum identities");
              break;
            }
            break;
          }
        }
      }
      if (source.Count <= 0)
        return;
      expansionFlags &= ~ExpansionFlags.AlwaysExpandStartingGroups;
      expansionFlags &= ~ExpansionFlags.CheckForNoDeliveryBeforeExpandingStartingGroup;
      NotificationIdentityExtensions.ExpandIdentitiesRecursively(requestContext, identityService, subscriberService, (IList<Guid>) source.ToList<Guid>(), processedIds, diagProcessingIdentities, identityMap, maxDepth - 1, maxIdentities, expansionFlags, subscriberStopWatch);
    }

    internal static bool IsMember(
      this IdentityService identityService,
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity group,
      Microsoft.VisualStudio.Services.Identity.Identity user,
      Stopwatch stopwatch = null)
    {
      stopwatch?.Start();
      try
      {
        return identityService.IsMember(requestContext, group.Descriptor, user.Descriptor);
      }
      finally
      {
        stopwatch?.Stop();
      }
    }

    internal static string GetPreferredNotificationEmailAddress(
      this Microsoft.VisualStudio.Services.Identity.Identity identity,
      IVssRequestContext requestContext,
      Stopwatch stopwatch = null)
    {
      return identity.GetPreferredNotificationEmailAddress(requestContext, out NotificationSubscriber _, stopwatch);
    }

    internal static string GetPreferredNotificationEmailAddress(
      this Microsoft.VisualStudio.Services.Identity.Identity identity,
      IVssRequestContext requestContext,
      out NotificationSubscriber subscriber,
      Stopwatch stopwatch = null)
    {
      stopwatch?.Start();
      NotificationSubscriberService service = requestContext.GetService<NotificationSubscriberService>();
      subscriber = service.GetSubscriber(requestContext, identity.Id);
      stopwatch?.Stop();
      return subscriber.PreferredEmailAddress;
    }

    internal static bool IsUsable(this Microsoft.VisualStudio.Services.Identity.Identity identity) => identity != null;

    internal static bool IsNonAadGroup(this Microsoft.VisualStudio.Services.Identity.Identity identity) => identity.IsContainer && !AadIdentityHelper.IsAadGroup(identity.Descriptor);

    internal static bool IsExpandableGroup(
      NotificationSubscriber subscriber,
      SubscriberType subscriberType,
      ExpansionFlags expansionFlags,
      ProcessingDiagnosticIdentity diagIdentity = null)
    {
      bool flag = false;
      if (subscriberType == SubscriberType.VsGroup)
      {
        ArgumentUtility.CheckForNull<NotificationSubscriber>(subscriber, nameof (subscriber));
        if (expansionFlags.HasFlag((Enum) ExpansionFlags.CheckForNoDeliveryBeforeExpandingStartingGroup) && subscriber.DeliveryPreference.Equals((object) NotificationSubscriberDeliveryPreference.NoDelivery))
        {
          if (diagIdentity != null)
            diagIdentity.SetMessage(string.Format("not expanding due to subscription channel and delivery preference {0}", (object) subscriber.DeliveryPreference));
          return flag;
        }
        if (expansionFlags.HasFlag((Enum) ExpansionFlags.AlwaysExpandGroups))
        {
          flag = true;
          if (diagIdentity != null)
            diagIdentity.SetMessage("expanding due to expansion flag AlwaysExpandGroups");
        }
        else
        {
          switch (subscriber.DeliveryPreference)
          {
            case NotificationSubscriberDeliveryPreference.NoDelivery:
              flag = expansionFlags.HasFlag((Enum) ExpansionFlags.AlwaysExpandStartingGroups);
              if (flag)
              {
                if (diagIdentity != null)
                {
                  diagIdentity.SetMessage("expanding due to expansion flag AlwaysExpandStartingGroups");
                  break;
                }
                break;
              }
              if (diagIdentity != null)
              {
                diagIdentity.SetMessage(string.Format("not expanding due to delivery preference {0}", (object) subscriber.DeliveryPreference));
                break;
              }
              break;
            case NotificationSubscriberDeliveryPreference.PreferredEmailAddress:
              flag = false;
              if (diagIdentity != null)
              {
                diagIdentity.SetMessage(string.Format("not expanding due to delivery preference {0}", (object) subscriber.DeliveryPreference));
                break;
              }
              break;
            case NotificationSubscriberDeliveryPreference.EachMember:
            case NotificationSubscriberDeliveryPreference.UseDefault:
              flag = true;
              if (diagIdentity != null)
              {
                diagIdentity.SetMessage(string.Format("expanding due to delivery preference {0}", (object) subscriber.DeliveryPreference));
                break;
              }
              break;
            default:
              flag = false;
              if (diagIdentity != null)
              {
                diagIdentity.SetMessage(string.Format("not expanding due to unknown delivery preference {0}", (object) subscriber.DeliveryPreference));
                break;
              }
              break;
          }
        }
      }
      return flag;
    }

    internal static bool CanReceiveMail(
      this Microsoft.VisualStudio.Services.Identity.Identity identity,
      IVssRequestContext requestContext,
      ProcessingDiagnosticIdentity diagIdentity = null)
    {
      NotificationSubscriberDeliveryPreference deliveryPreference = NotificationSubscriberDeliveryPreference.PreferredEmailAddress;
      SubscriberType subscriberType;
      bool canReceiveMail;
      if (!identity.IsContainer)
      {
        subscriberType = SubscriberType.Individual;
        canReceiveMail = true;
      }
      else if (AadIdentityHelper.IsAadGroup(identity.Descriptor))
      {
        subscriberType = SubscriberType.AadGroup;
        canReceiveMail = !string.IsNullOrEmpty(identity.GetProperty<string>("Mail", string.Empty));
      }
      else
      {
        subscriberType = SubscriberType.VsGroup;
        NotificationSubscriber subscriber;
        canReceiveMail = !string.IsNullOrEmpty(identity.GetPreferredNotificationEmailAddress(requestContext, out subscriber));
        deliveryPreference = subscriber.DeliveryPreference;
      }
      NotificationIdentityExtensions.SetDiagnosticIdentityMessage(canReceiveMail, subscriberType, deliveryPreference, diagIdentity);
      return canReceiveMail;
    }

    internal static bool CanReceiveMail(
      this Microsoft.VisualStudio.Services.Identity.Identity identity,
      NotificationSubscriber subscriber,
      SubscriberType subscriberType,
      ProcessingDiagnosticIdentity diagIdentity = null)
    {
      bool canReceiveMail = false;
      NotificationSubscriberDeliveryPreference deliveryPreference = subscriber != null ? subscriber.DeliveryPreference : NotificationSubscriberDeliveryPreference.PreferredEmailAddress;
      switch (subscriberType)
      {
        case SubscriberType.VsGroup:
          ArgumentUtility.CheckForNull<NotificationSubscriber>(subscriber, nameof (subscriber));
          canReceiveMail = deliveryPreference == NotificationSubscriberDeliveryPreference.PreferredEmailAddress && !string.IsNullOrEmpty(subscriber.PreferredEmailAddress);
          break;
        case SubscriberType.AadGroup:
          canReceiveMail = !string.IsNullOrEmpty(identity.GetProperty<string>("Mail", string.Empty));
          break;
        case SubscriberType.Individual:
          canReceiveMail = !identity.IsAADServicePrincipal;
          break;
      }
      NotificationIdentityExtensions.SetDiagnosticIdentityMessage(canReceiveMail, subscriberType, deliveryPreference, diagIdentity);
      return canReceiveMail;
    }

    private static void SetDiagnosticIdentityMessage(
      bool canReceiveMail,
      SubscriberType subscriberType,
      NotificationSubscriberDeliveryPreference deliveryPreference,
      ProcessingDiagnosticIdentity diagIdentity)
    {
      if (diagIdentity == null || !string.IsNullOrEmpty(diagIdentity.Message))
        return;
      string str = (string) null;
      switch (deliveryPreference)
      {
        case NotificationSubscriberDeliveryPreference.NoDelivery:
          str = "no delivery";
          break;
        case NotificationSubscriberDeliveryPreference.PreferredEmailAddress:
          str = canReceiveMail ? "has preferred mail" : "no preferred email";
          break;
        case NotificationSubscriberDeliveryPreference.EachMember:
          str = "group is expanded";
          break;
      }
      diagIdentity.Message = subscriberType.ToString() + ":" + str;
    }

    private static void SetMessage(this ProcessingDiagnosticIdentity diagIdentity, string message)
    {
      if (diagIdentity == null)
        return;
      diagIdentity.Message = message;
    }

    internal static void GetMinimalSubscriberInfo(
      this Microsoft.VisualStudio.Services.Identity.Identity identity,
      IVssRequestContext requestContext,
      NotificationSubscriberService subscriberService,
      Stopwatch subscriberStopWatch,
      out SubscriberType subscriberType,
      out NotificationSubscriber subscriber)
    {
      subscriberType = subscriberService.GetSubscriberType(identity);
      if (subscriberType == SubscriberType.VsGroup)
      {
        try
        {
          subscriberStopWatch?.Start();
          subscriber = subscriberService.GetSubscriber(requestContext, identity.Id, subscriberType, NotificationSubscriberService.ExpansionApiFlags);
        }
        finally
        {
          subscriberStopWatch?.Stop();
        }
      }
      else
        subscriber = (NotificationSubscriber) null;
    }
  }
}

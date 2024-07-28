// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.SubscriptionLookup
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class SubscriptionLookup
  {
    private SubscriptionLookup()
    {
    }

    public SubscriptionLookupType LookupType { get; set; }

    internal string ContributedSubscriptionName { get; set; }

    public int? SubscriptionId { get; set; }

    public string IndexedExpression { get; set; }

    public string EventType { get; set; }

    public Guid? SubscriberId { get; set; }

    public string Matcher { get; set; }

    public Guid? DataspaceId { get; set; }

    public string Classification { get; set; }

    public string Channel { get; set; }

    public string Metadata { get; set; }

    public SubscriptionFlags? Flags { get; set; }

    public Guid? ScopeId { get; set; }

    public Guid? UniqueId { get; set; }

    public static SubscriptionLookup CreateAnyFieldLookup(
      int? subscriptionId = null,
      string indexedExpression = null,
      string eventType = null,
      Guid? subscriberId = null,
      string matcher = null,
      Guid? dataspaceId = null,
      string classification = null,
      string channel = null,
      string metadata = null,
      SubscriptionFlags? flags = null,
      Guid? scopeId = null,
      string contributedSubscriptionName = null,
      Guid? uniqueId = null)
    {
      return new SubscriptionLookup()
      {
        LookupType = SubscriptionLookupType.Any,
        SubscriptionId = subscriptionId,
        IndexedExpression = indexedExpression,
        EventType = eventType,
        SubscriberId = subscriberId,
        Matcher = matcher,
        DataspaceId = dataspaceId,
        Classification = classification,
        Channel = channel,
        Metadata = metadata,
        Flags = flags,
        ScopeId = scopeId,
        ContributedSubscriptionName = contributedSubscriptionName,
        UniqueId = uniqueId
      };
    }

    public static SubscriptionLookup CreateFollowsMatcherProcessLookup(string indexedExpression) => new SubscriptionLookup()
    {
      LookupType = SubscriptionLookupType.FollowsMatcherProcess,
      IndexedExpression = indexedExpression
    };

    private void ValidateFollowsMatcherProcess() => this.ValidateMember("IndexedExpression", (Func<bool>) (() => !string.IsNullOrWhiteSpace(this.IndexedExpression)));

    public static SubscriptionLookup CreateUniqueIdLookup(Guid uniqueId) => new SubscriptionLookup()
    {
      LookupType = SubscriptionLookupType.UniqueId,
      UniqueId = new Guid?(uniqueId)
    };

    private void ValidateUniqueId() => this.ValidateMember("UniqueId", (Func<bool>) (() => this.UniqueId.HasValue));

    public static SubscriptionLookup CreateSubscriberIdLookup(Guid subscriberId) => new SubscriptionLookup()
    {
      LookupType = SubscriptionLookupType.SubscriberId,
      SubscriberId = new Guid?(subscriberId)
    };

    private void ValidateSubscriberId() => this.ValidateMember("SubscriberId", (Func<bool>) (() => this.SubscriberId.HasValue));

    public static SubscriptionLookup CreateAnyGroupLookup() => new SubscriptionLookup()
    {
      LookupType = SubscriptionLookupType.AnyGroup,
      Flags = new SubscriptionFlags?(SubscriptionFlags.GroupSubscription)
    };

    private void ValidateAnyGroup() => this.ValidateMember("Flags", (Func<bool>) (() =>
    {
      SubscriptionFlags? flags = this.Flags;
      SubscriptionFlags subscriptionFlags = SubscriptionFlags.GroupSubscription;
      return flags.GetValueOrDefault() == subscriptionFlags & flags.HasValue;
    }));

    public static SubscriptionLookup CreateAllLookup() => new SubscriptionLookup()
    {
      LookupType = SubscriptionLookupType.All
    };

    public static SubscriptionLookup CreateMatcherLookup(string matcher) => new SubscriptionLookup()
    {
      LookupType = SubscriptionLookupType.Matcher,
      Matcher = matcher
    };

    private void ValidateMatcher() => this.ValidateMember("Matcher", (Func<bool>) (() => !string.IsNullOrWhiteSpace(this.Matcher)));

    public static SubscriptionLookup CreateForTargetLookup(Guid targetId) => new SubscriptionLookup()
    {
      LookupType = SubscriptionLookupType.ForTarget,
      SubscriberId = new Guid?(targetId)
    };

    private void ValidateForTarget() => this.ValidateMember("SubscriberId", (Func<bool>) (() => this.SubscriberId.HasValue));

    public static SubscriptionLookup CreateSubscriptionIdLookup(int subscriptionId) => new SubscriptionLookup()
    {
      LookupType = SubscriptionLookupType.SubscriptionId,
      SubscriptionId = new int?(subscriptionId)
    };

    private void ValidateSubscriptionId() => this.ValidateMember("SubscriptionId", (Func<bool>) (() => this.SubscriptionId.HasValue));

    public static SubscriptionLookup CreateEventTypeLookup(string eventType, string matcher = null) => new SubscriptionLookup()
    {
      LookupType = SubscriptionLookupType.EventType,
      EventType = eventType,
      Matcher = matcher
    };

    private void ValidateEventType() => this.ValidateMember("EventType", (Func<bool>) (() => !string.IsNullOrWhiteSpace(this.EventType)));

    public static SubscriptionLookup CreateFollowArtifactForUserLookup(
      string indexedExpression,
      Guid subscriberId)
    {
      return new SubscriptionLookup()
      {
        LookupType = SubscriptionLookupType.FollowArtifactForUser,
        IndexedExpression = indexedExpression,
        SubscriberId = new Guid?(subscriberId)
      };
    }

    private void ValidateFollowArtifactForUser()
    {
      this.ValidateMember("IndexedExpression", (Func<bool>) (() => !string.IsNullOrWhiteSpace(this.IndexedExpression)));
      this.ValidateMember("SubscriberId", (Func<bool>) (() => this.SubscriberId.HasValue));
    }

    public static SubscriptionLookup CreateFollowArtifactTypesForUserLookup(
      string artifactType,
      Guid subscriberId)
    {
      return new SubscriptionLookup()
      {
        LookupType = SubscriptionLookupType.FollowArtifactTypesForUser,
        Metadata = artifactType,
        SubscriberId = new Guid?(subscriberId)
      };
    }

    private void ValidateFollowArtifactTypesForUser()
    {
      this.ValidateMember("Metadata", (Func<bool>) (() => !string.IsNullOrWhiteSpace(this.Metadata)));
      this.ValidateMember("SubscriberId", (Func<bool>) (() => this.SubscriberId.HasValue));
    }

    internal void Valididate()
    {
      switch (this.LookupType)
      {
        case SubscriptionLookupType.Any:
          break;
        case SubscriptionLookupType.FollowsMatcherProcess:
          this.ValidateFollowsMatcherProcess();
          break;
        case SubscriptionLookupType.UniqueId:
          this.ValidateUniqueId();
          break;
        case SubscriptionLookupType.SubscriberId:
          this.ValidateSubscriberId();
          break;
        case SubscriptionLookupType.AnyGroup:
          this.ValidateAnyGroup();
          break;
        case SubscriptionLookupType.All:
          break;
        case SubscriptionLookupType.Matcher:
          this.ValidateMatcher();
          break;
        case SubscriptionLookupType.ForTarget:
          this.ValidateForTarget();
          break;
        case SubscriptionLookupType.SubscriptionId:
          this.ValidateSubscriptionId();
          break;
        case SubscriptionLookupType.EventType:
          this.ValidateEventType();
          break;
        case SubscriptionLookupType.FollowArtifactForUser:
          this.ValidateFollowArtifactForUser();
          break;
        case SubscriptionLookupType.FollowArtifactTypesForUser:
          this.ValidateFollowArtifactTypesForUser();
          break;
        default:
          throw new ArgumentOutOfRangeException("LookupType");
      }
    }

    private void ValidateMember(string memberName, Func<bool> MemberValidator)
    {
      if (!MemberValidator())
        throw new ArgumentException(memberName);
    }
  }
}

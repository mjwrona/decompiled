// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.EventNotificationComponent
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.Common;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class EventNotificationComponent : 
    TeamFoundationSqlResourceComponent,
    IEventNotificationComponent,
    IDisposable
  {
    internal const string s_area = "Notifications";
    internal const string s_layer = "SqlComponent";
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[22]
    {
      (IComponentCreator) new ComponentCreator<EventNotificationComponent>(1, true),
      (IComponentCreator) new ComponentCreator<EventNotificationComponent17>(17, false),
      (IComponentCreator) new ComponentCreator<EventNotificationComponent107>(107, false),
      (IComponentCreator) new ComponentCreator<EventNotificationComponent108>(108, false),
      (IComponentCreator) new ComponentCreator<EventNotificationComponent1120>(1120, false),
      (IComponentCreator) new ComponentCreator<EventNotificationComponent1130>(1130, false),
      (IComponentCreator) new ComponentCreator<EventNotificationComponent1150>(1150, false),
      (IComponentCreator) new ComponentCreator<EventNotificationComponent1160>(1160, false),
      (IComponentCreator) new ComponentCreator<EventNotificationComponent1200>(1200, false),
      (IComponentCreator) new ComponentCreator<EventNotificationComponent1220>(1220, false),
      (IComponentCreator) new ComponentCreator<EventNotificationComponent1260>(1260, false),
      (IComponentCreator) new ComponentCreator<EventNotificationComponent1290>(1290, false),
      (IComponentCreator) new ComponentCreator<EventNotificationComponent1310>(1310, false),
      (IComponentCreator) new ComponentCreator<EventNotificationComponent1313>(1313, false),
      (IComponentCreator) new ComponentCreator<EventNotificationComponent1313>(1321, false),
      (IComponentCreator) new ComponentCreator<EventNotificationComponent1330>(1330, false),
      (IComponentCreator) new ComponentCreator<EventNotificationComponent1340>(1340, false),
      (IComponentCreator) new ComponentCreator<EventNotificationComponent1360>(1360, false),
      (IComponentCreator) new ComponentCreator<EventNotificationComponent1380>(1380, false),
      (IComponentCreator) new ComponentCreator<EventNotificationComponent1430>(1430, false),
      (IComponentCreator) new ComponentCreator<EventNotificationComponent1440>(1440, false),
      (IComponentCreator) new ComponentCreator<EventNotificationComponent2170>(2170, false)
    }, "EventNotification");
    private static Dictionary<int, SqlExceptionFactory> s_exceptionFactories = new Dictionary<int, SqlExceptionFactory>();
    public HookGetDataspaceId HookGetDataspaceId;
    public HookGetDataspaceIdentifier HookGetDataspaceIdentifier;

    static EventNotificationComponent()
    {
      EventNotificationComponent.s_exceptionFactories.Add(800023, new SqlExceptionFactory(typeof (EventTypeDoesNotExistException)));
      EventNotificationComponent.s_exceptionFactories.Add(800047, new SqlExceptionFactory(typeof (SubscriptionNotFoundException)));
      EventNotificationComponent.s_exceptionFactories.Add(800060, new SqlExceptionFactory(typeof (InvalidEventsException)));
    }

    public EventNotificationComponent() => this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public override int GetDataspaceId(Guid dataspaceIdentifier) => this.HookGetDataspaceId != null ? this.HookGetDataspaceId(dataspaceIdentifier) : base.GetDataspaceId(dataspaceIdentifier);

    public override Guid GetDataspaceIdentifier(int dataspaceId) => this.HookGetDataspaceIdentifier != null ? this.HookGetDataspaceIdentifier(dataspaceId) : base.GetDataspaceIdentifier(dataspaceId);

    protected virtual void BindDataspace(Guid dataspaceIdentifier) => this.BindInt("@dataspaceId", this.GetDataspaceId(dataspaceIdentifier));

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) EventNotificationComponent.s_exceptionFactories;

    protected virtual int ChannelLength => 25;

    internal virtual SubscriptionBinder GetSubscriptionVersion() => new SubscriptionBinder((TeamFoundationSqlResourceComponent) this);

    public virtual void DeleteSubscription(int subscriptionId)
    {
      this.PrepareStoredProcedure("prc_DeleteSubscription");
      this.BindInt("@subscriptionId", subscriptionId);
      this.ExecuteNonQuery();
    }

    public virtual void DeleteSubscriptions(IEnumerable<int> subscriptionIds)
    {
      this.PrepareStoredProcedure("prc_DeleteSubscriptions");
      this.BindInt32Table("@subscriptionIds", subscriptionIds);
      this.ExecuteNonQuery();
    }

    public virtual void UpdateNotificationStatistics(IEnumerable<NotificationStatisticEntry> stats)
    {
    }

    public virtual void UpdateSubscriptionProject(Subscription subscription, Guid newProjectId)
    {
      this.PrepareStoredProcedure("prc_UpdateSubscriptionDataspace");
      SubscriptionBinder.MatcherMustBeXPath(subscription.Matcher);
      SubscriptionBinder.ChannelMustBeDeliveryType(subscription.Channel);
      this.BindInt("@subscriptionId", subscription.ID);
      this.BindInt("@oldDataspace", this.GetDataspaceId(subscription.ProjectId));
      this.BindInt("@newDataspace", this.GetDataspaceId(newProjectId));
      this.BindString("@eventType", subscription.EventTypeName, 128, false, SqlDbType.NVarChar);
      this.BindString("@filterExpression", subscription.Expression, int.MaxValue, false, SqlDbType.NVarChar);
      this.BindGuid("@subscriberId", subscription.SubscriberId);
      this.BindString("@address", subscription.DeliveryAddress, 1024, false, SqlDbType.NVarChar);
      this.BindString("@classification", subscription.Tag, 2048, false, SqlDbType.NVarChar);
      this.BindString("@matcher", subscription.Matcher, 25, false, SqlDbType.VarChar);
      this.ExecuteNonQuery();
    }

    public virtual void UpdateEventSubscription(List<SubscriptionUpdate> subscriptionUpdates)
    {
      this.PrepareStoredProcedure("prc_UpdateEventSubscription4");
      this.BindSubscriptionUpdates("@subscriptionUpdates", subscriptionUpdates);
      this.ExecuteNonQuery();
    }

    public virtual List<Subscription> QuerySubscriptions(
      IEnumerable<SubscriptionLookup> subscriptionKeys,
      bool includeDeleted)
    {
      this.PrepareStoredProcedure("prc_QuerySubscriptions");
      this.BindSubscriptionKeys("@subscriptionKeys", subscriptionKeys);
      this.BindBoolean("@includeDeleted", includeDeleted);
      this.DataspaceRlsEnabled = false;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<Subscription>((ObjectBinder<Subscription>) this.GetSubscriptionVersion());
        return resultCollection.GetCurrent<Subscription>().Items;
      }
    }

    public virtual int SubscribeEvent(Subscription subscription, bool allowDuplicates = false)
    {
      this.PrepareStoredProcedure("prc_SubscribeEvent2");
      this.BindSubscription("@subscriptions", subscription);
      this.BindBoolean("@allowDuplicates", allowDuplicates);
      return (int) this.ExecuteScalar();
    }

    public virtual List<Subscription> GetSubscriptions(IEnumerable<string> eventTypes)
    {
      this.PrepareStoredProcedure("prc_GetSubscriptions");
      this.BindGetSubscriptionParameters((IEnumerable<string>) eventTypes.ToHashSet(), (IEnumerable<string>) new HashSet<string>()
      {
        "XPathMatcher",
        "JsonPathMatcher",
        "PathMatcher",
        "ActorMatcher"
      });
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<Subscription>((ObjectBinder<Subscription>) this.GetSubscriptionVersion());
        return resultCollection.GetCurrent<Subscription>().Items;
      }
    }

    public virtual void BindGetSubscriptionParameters(
      IEnumerable<string> eventTypes,
      IEnumerable<string> matchers)
    {
      this.BindStringTable("@eventTypes", eventTypes);
    }

    public virtual List<TeamFoundationNotification> QueryNotifications(
      IEnumerable<NotificationLookup> notificationKeys)
    {
      throw new NotSupportedException("QueryNotifications not supported in this version");
    }

    public virtual List<DefaultSubscriptionUserCandidate> GetDefaultSubscriptionsUserDisabled(
      IEnumerable<DefaultSubscriptionUserCandidate> candidates)
    {
      return this.GetDefaultSubscriptionsUserEnabledDisabled(candidates, "prc_GetDefaultSubscriptionsUserDisabled");
    }

    public virtual List<DefaultSubscriptionAdminCandidate> GetDefaultSubscriptionsAdminDisabled(
      HashSet<string> candidates)
    {
      return this.GetDefaultSubscriptionsAdminEnabledDisabled(candidates, "prc_GetDefaultSubscriptionsAdminDisabled");
    }

    public virtual void UpdateDefaultSubscriptionsUserEnabled(
      Guid subscriberId,
      string subscriptionName,
      bool enabled)
    {
      this.PrepareStoredProcedure("prc_UpdateDefaultSubscriptionsUserEnabled");
      this.BindGuid("@subscriberId", subscriberId);
      this.BindString("@subscriptionName", subscriptionName, 100, false, SqlDbType.VarChar);
      this.BindInt("@enabled", enabled ? 1 : 0);
      this.ExecuteNonQuery();
    }

    protected List<DefaultSubscriptionUserCandidate> GetDefaultSubscriptionsUserEnabledDisabled(
      IEnumerable<DefaultSubscriptionUserCandidate> candidates,
      string sproc)
    {
      this.PrepareStoredProcedure(sproc);
      this.BindDefaultSubscriptionUserCandidates("@candidates", candidates);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<DefaultSubscriptionUserCandidate>((ObjectBinder<DefaultSubscriptionUserCandidate>) new DefaultSubscriptionUserCandidateBinder());
        return resultCollection.GetCurrent<DefaultSubscriptionUserCandidate>().Items;
      }
    }

    protected List<DefaultSubscriptionAdminCandidate> GetDefaultSubscriptionsAdminEnabledDisabled(
      HashSet<string> candidates,
      string sproc)
    {
      this.PrepareStoredProcedure(sproc);
      this.BindDefaultSubscriptionAdminCandidates("@candidates", (IEnumerable<string>) candidates);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<DefaultSubscriptionAdminCandidate>((ObjectBinder<DefaultSubscriptionAdminCandidate>) new DefaultSubscriptionAdminCandidateBinder());
        return resultCollection.GetCurrent<DefaultSubscriptionAdminCandidate>().Items;
      }
    }

    internal void BindDefaultSubscriptionAdminCandidates(
      string parameterName,
      IEnumerable<string> candidates)
    {
      List<SqlDataRecord> rows = new List<SqlDataRecord>();
      foreach (string candidate in candidates)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(NotificationEventTypes.typ_DefaultSubscriptionAdminCandidates);
        sqlDataRecord.SetString(0, candidate);
        rows.Add(sqlDataRecord);
      }
      this.BindTable(parameterName, "typ_DefaultSubscriptionAdminCandidates", (IEnumerable<SqlDataRecord>) rows);
    }

    public virtual void UpdateDefaultSubscriptionsAdminEnabled(
      string subscriptionName,
      bool disabled,
      bool blockUserDisable,
      Guid modifiedBy)
    {
      this.PrepareStoredProcedure("prc_UpdateDefaultSubscriptionsAdminEnabled");
      this.BindString("@subscriptionName", subscriptionName, 100, false, SqlDbType.VarChar);
      this.BindBoolean("@disabled", disabled);
      this.BindBoolean("@blockUserDisable", blockUserDisable);
      this.ExecuteNonQuery();
    }

    protected virtual void BindChannels(IEnumerable<string> channels)
    {
      List<SqlDataRecord> rows = new List<SqlDataRecord>();
      foreach (string channel in channels)
      {
        if (channel != null)
        {
          SqlDataRecord sqlDataRecord = new SqlDataRecord(NotificationEventTypes.typ_ChannelTable);
          sqlDataRecord.SetString(0, channel);
          rows.Add(sqlDataRecord);
        }
      }
      this.BindTable("@channels", "typ_ChannelTable", (IEnumerable<SqlDataRecord>) rows);
    }

    internal void BindDefaultSubscriptionUserCandidates(
      string parameterName,
      IEnumerable<DefaultSubscriptionUserCandidate> candidates)
    {
      List<SqlDataRecord> rows = new List<SqlDataRecord>();
      foreach (DefaultSubscriptionUserCandidate candidate in candidates)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(NotificationEventTypes.typ_DefaultSubscriptionUserCandidates);
        sqlDataRecord.SetGuid(0, candidate.SubscriberId);
        sqlDataRecord.SetString(1, candidate.SubscriptionName);
        rows.Add(sqlDataRecord);
      }
      this.BindTable(parameterName, "typ_DefaultSubscriptionUserCandidates", (IEnumerable<SqlDataRecord>) rows);
    }

    protected virtual void BindSubscriptionKeys(
      string parameterName,
      IEnumerable<SubscriptionLookup> keys)
    {
      List<SqlDataRecord> rows = new List<SqlDataRecord>();
      foreach (SubscriptionLookup key in keys)
      {
        if (key != null)
        {
          key.Valididate();
          SqlDataRecord sqlDataRecord = this.BindSubscriptionKeyRecord(key);
          rows.Add(sqlDataRecord);
        }
      }
      this.BindTable(parameterName, this.SubscriptionKeyTypeName, (IEnumerable<SqlDataRecord>) rows);
    }

    protected virtual string SubscriptionKeyTypeName => "typ_SubscriptionKey3";

    protected virtual SqlMetaData[] SubscriptionKeyType => NotificationEventTypes.typ_SubscriptionKey3;

    protected virtual SqlDataRecord BindSubscriptionKeyRecord(SubscriptionLookup key)
    {
      SqlDataRecord record1 = new SqlDataRecord(this.SubscriptionKeyType);
      record1.SetNullableInt32(0, key.SubscriptionId);
      SqlDataRecord record2 = record1;
      Guid? nullable1 = key.SubscriberId;
      Guid guid1 = nullable1 ?? Guid.Empty;
      record2.SetNullableGuid(1, guid1);
      record1.SetNullableString(2, key.Matcher);
      record1.SetNullableString(3, key.EventType);
      record1.SetNullableString(4, key.IndexedExpression);
      SqlDataRecord record3 = record1;
      nullable1 = key.DataspaceId;
      int? nullable2;
      if (!nullable1.HasValue)
      {
        nullable2 = new int?();
      }
      else
      {
        nullable1 = key.DataspaceId;
        nullable2 = new int?(this.GetDataspaceId(nullable1.Value));
      }
      record3.SetNullableInt32(5, nullable2);
      record1.SetNullableString(6, string.IsNullOrEmpty(key.Classification) ? (string) null : key.Classification);
      record1.SetNullableString(7, key.Channel);
      record1.SetNullableString(8, key.Metadata);
      if (key.Flags.HasValue)
        record1.SetInt32(9, (int) key.Flags.Value);
      else
        record1.SetDBNull(9);
      record1.SetBoolean(10, false);
      SqlDataRecord record4 = record1;
      nullable1 = key.ScopeId;
      Guid guid2 = nullable1 ?? Guid.Empty;
      record4.SetNullableGuid(11, guid2);
      return record1;
    }

    protected virtual void BindSubscription(string parameterName, Subscription subscription) => this.BindTable(parameterName, this.SubcriptionTypeName, (IEnumerable<SqlDataRecord>) new List<SqlDataRecord>()
    {
      this.BindSubscriptionRecord(subscription)
    });

    protected virtual string SubcriptionTypeName => "typ_Subscription2";

    protected virtual SqlMetaData[] SubscriptionType => NotificationEventTypes.typ_Subscription2;

    protected virtual SqlDataRecord BindSubscriptionRecord(Subscription subscription)
    {
      SqlDataRecord record = new SqlDataRecord(this.SubscriptionType);
      record.SetNullableString(0, subscription.EventTypeName);
      record.SetNullableString(1, subscription.Expression);
      record.SetGuid(2, subscription.SubscriberId);
      record.SetNullableString(3, subscription.DeliveryAddress);
      record.SetNullableString(4, subscription.Tag);
      record.SetNullableString(5, subscription.Description);
      record.SetInt32(6, this.GetDataspaceId(subscription.ProjectId));
      record.SetNullableGuid(7, subscription.LastModifiedBy);
      record.SetNullableString(8, subscription.IndexedExpression);
      record.SetNullableString(9, subscription.Matcher);
      record.SetNullableString(10, subscription.Metadata);
      record.SetNullableString(11, subscription.Channel);
      record.SetNullableInt32(12, new int?((int) subscription.Status));
      record.SetNullableString(13, subscription.StatusMessage);
      record.SetNullableInt32(14, new int?((int) subscription.Flags));
      record.SetNullableGuid(15, subscription.ScopeId);
      return record;
    }

    protected virtual void BindSubscriptionUpdates(
      string parameterName,
      List<SubscriptionUpdate> updates)
    {
      List<SqlDataRecord> rows = new List<SqlDataRecord>();
      foreach (SubscriptionUpdate update in updates)
      {
        SqlDataRecord sqlDataRecord = this.BindSubscriptionUpdateRecord(update);
        rows.Add(sqlDataRecord);
      }
      this.BindTable(parameterName, this.SubscriptionUpdateTypeName, (IEnumerable<SqlDataRecord>) rows);
    }

    protected virtual string SubscriptionUpdateTypeName => "typ_SubscriptionUpdate4";

    protected virtual SqlMetaData[] SubscriptionUpdateType => NotificationEventTypes.typ_SubscriptionUpdate4;

    protected virtual SqlDataRecord BindSubscriptionUpdateRecord(SubscriptionUpdate update)
    {
      SqlDataRecord record = new SqlDataRecord(this.SubscriptionUpdateType);
      record.SetInt32(0, update.Id);
      record.SetNullableString(1, update.EventTypeName);
      record.SetNullableString(2, update.Expression);
      record.SetNullableString(3, update.IndexedExpression);
      record.SetNullableString(4, update.Channel);
      if (update.Address == null)
        record.SetNullableString(5, update.Address);
      else
        record.SetString(5, update.Address);
      record.SetNullableString(6, update.Classification);
      record.SetNullableString(7, update.Description);
      Guid? nullable = update.LastModifiedBy;
      if (nullable.HasValue)
      {
        SqlDataRecord sqlDataRecord = record;
        nullable = update.LastModifiedBy;
        Guid guid = nullable.Value;
        sqlDataRecord.SetGuid(8, guid);
      }
      else
        record.SetDBNull(8);
      record.SetNullableString(9, update.MetaData);
      SubscriptionStatus? status = update.Status;
      if (status.HasValue)
      {
        SqlDataRecord sqlDataRecord = record;
        status = update.Status;
        int num = (int) status.Value;
        sqlDataRecord.SetInt32(10, num);
      }
      else
        record.SetDBNull(10);
      record.SetNullableString(11, update.StatusMessage);
      SubscriptionFlags? flags = update.Flags;
      if (flags.HasValue)
      {
        SqlDataRecord sqlDataRecord = record;
        flags = update.Flags;
        int num = (int) flags.Value;
        sqlDataRecord.SetInt32(12, num);
      }
      else
        record.SetDBNull(12);
      record.SetNullableString(13, update.Matcher);
      nullable = update.ScopeId;
      if (!nullable.HasValue)
      {
        record.SetNullableGuid(14, Guid.Empty);
      }
      else
      {
        SqlDataRecord sqlDataRecord = record;
        nullable = update.ScopeId;
        Guid guid = nullable.Value;
        sqlDataRecord.SetGuid(14, guid);
      }
      return record;
    }

    internal virtual void BindTransformedEvents(
      string parameterName,
      IList<TeamFoundationEvent> events)
    {
      List<SqlDataRecord> rows = new List<SqlDataRecord>();
      foreach (TeamFoundationEvent teamFoundationEvent in (IEnumerable<TeamFoundationEvent>) events)
      {
        SqlDataRecord record = new SqlDataRecord(NotificationEventTypes.typ_TransformedEvent);
        string str = string.Empty;
        if (!string.IsNullOrEmpty(teamFoundationEvent.EventData))
        {
          SerializedNotificationEvent evt = new SerializedNotificationEvent(teamFoundationEvent.EventData, teamFoundationEvent.EventType);
          evt.Actors.AddRange((IEnumerable<EventActor>) teamFoundationEvent.Actors);
          evt.AllowedChannels = teamFoundationEvent.AllowedChannels;
          evt.ArtifactUris.Add(teamFoundationEvent.ArtifactUri);
          evt.ProcessQueue = teamFoundationEvent.ProcessQueue;
          evt.Scopes.AddRange((IEnumerable<EventScope>) teamFoundationEvent.Scopes);
          str = evt.SerializeEvent();
        }
        record.SetInt32(0, teamFoundationEvent.Id);
        record.SetNullableString(1, str);
        record.SetDBNull(2);
        rows.Add(record);
      }
      this.BindTable(parameterName, "typ_TransformedEvent", (IEnumerable<SqlDataRecord>) rows);
    }

    public virtual (int, int) CleanupNotificationsEvents(
      int eventAgeMins,
      int notificationAgeMins,
      int batchSize)
    {
      this.PrepareStoredProcedure("prc_CleanupEventsNotifications", 3600);
      this.BindInt("@eventAgeMins", eventAgeMins);
      this.BindInt("@notificationAgeMins", notificationAgeMins);
      this.BindInt("@batchSize", batchSize);
      this.ExecuteNonQuery();
      return (-1, -1);
    }

    public virtual void CleanupSubscriptions(int subscriptionAgeDays, int batchSize)
    {
      this.PrepareStoredProcedure("prc_CleanupSubscriptions", 3600);
      this.BindInt("@subscriptionAgeDays", subscriptionAgeDays);
      this.BindInt("@batchSize", batchSize);
      this.ExecuteNonQuery();
    }

    public virtual int CleanupNotificationLog(int logAgeMins, int batchSize) => -1;

    public virtual void FireEvents(IEnumerable<SerializedNotificationEvent> theEvents)
    {
      this.PrepareStoredProcedure("prc_FireEvents2");
      this.BindEventEntryTable("@eventsToFire", theEvents);
      this.ExecuteNonQuery();
    }

    public virtual List<TeamFoundationEvent> GetUnprocessedEvents(
      int eventBatchSize,
      IEnumerable<string> processQueues)
    {
      this.PrepareStoredProcedure("prc_GetUnprocessedEvents2");
      this.BindInt("@batchSize", eventBatchSize);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<TeamFoundationEvent>(this.GetTeamFoundationEventVersion());
        return resultCollection.GetCurrent<TeamFoundationEvent>().Items;
      }
    }

    public virtual void SaveTransformedEvents(IList<TeamFoundationEvent> events)
    {
      this.PrepareStoredProcedure("prc_SaveTransformedEvents");
      this.BindTransformedEvents("@events", events);
      this.ExecuteNonQuery();
    }

    public virtual void SaveProcessedEvents(
      IList<TeamFoundationEvent> events,
      IList<TeamFoundationNotification> notifications)
    {
      this.PrepareStoredProcedure("prc_SaveProcessedEvents");
      this.BindEventTable("@events", (IEnumerable<TeamFoundationEvent>) events);
      this.BindNotificationTable("@notifications", (IEnumerable<TeamFoundationNotification>) notifications);
      this.ExecuteNonQuery();
    }

    internal virtual ObjectBinder<TeamFoundationEvent> GetTeamFoundationEventVersion() => (ObjectBinder<TeamFoundationEvent>) new TeamFoundationEventBinder();

    protected virtual SqlParameter BindEventEntryTable(
      string parameterName,
      IEnumerable<SerializedNotificationEvent> rows)
    {
      rows = rows ?? Enumerable.Empty<SerializedNotificationEvent>();
      System.Func<SerializedNotificationEvent, SqlDataRecord> selector = (System.Func<SerializedNotificationEvent, SqlDataRecord>) (notifEvent =>
      {
        SqlDataRecord record = this.NewEventEntryRecord();
        this.BindEventEntryRow(record, notifEvent);
        return record;
      });
      return this.BindTable(parameterName, this.EventEntryTableName, rows.Select<SerializedNotificationEvent, SqlDataRecord>(selector));
    }

    protected virtual void BindEventEntryRow(
      SqlDataRecord record,
      SerializedNotificationEvent notifEvent)
    {
      record.SetString(0, notifEvent.EventType);
      record.SetString(1, notifEvent.SerializeEvent());
      record.SetDBNull(2);
      record.SetDBNull(3);
      record.SetDBNull(4);
    }

    protected virtual SqlDataRecord NewEventEntryRecord() => new SqlDataRecord(NotificationEventTypes.typ_EventEntryTable4);

    protected virtual string EventEntryTableName => "typ_EventEntryTable4";

    protected virtual SqlParameter BindEventTable(
      string parameterName,
      IEnumerable<TeamFoundationEvent> rows)
    {
      rows = rows ?? Enumerable.Empty<TeamFoundationEvent>();
      System.Func<TeamFoundationEvent, SqlDataRecord> selector = (System.Func<TeamFoundationEvent, SqlDataRecord>) (teamFoundationEvent =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(NotificationEventTypes.typ_EventTable2);
        sqlDataRecord.SetInt32(0, teamFoundationEvent.Id);
        sqlDataRecord.SetBoolean(1, teamFoundationEvent.Matches > 0);
        sqlDataRecord.SetBoolean(2, teamFoundationEvent.IsBlocked);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_EventTable2", rows.Select<TeamFoundationEvent, SqlDataRecord>(selector));
    }

    public virtual void UpdateEventExpirationTime(
      IEnumerable<int> eventIds,
      DateTime expirationTime)
    {
    }

    public virtual List<TeamFoundationNotification> GetUnprocessedNotifications(
      int lastNotificationId,
      int notificationBatchSize,
      IEnumerable<string> channels,
      IEnumerable<string> processQueues,
      int failedRetryInterval = 0)
    {
      List<TeamFoundationNotification> notificationsWorker = this.GetUnprocessedNotificationsWorker(lastNotificationId, notificationBatchSize, channels, processQueues, failedRetryInterval);
      TeamFoundationNotificationBinder.PostBind(this.RequestContext, (IEnumerable<TeamFoundationNotification>) notificationsWorker);
      return notificationsWorker;
    }

    public virtual List<TeamFoundationNotification> GetNotificationsById(List<int> notificationId) => new List<TeamFoundationNotification>();

    public virtual void SaveNotificationLog(INotificationDiagnosticLog log)
    {
    }

    public virtual List<INotificationDiagnosticLog> QueryNotificationLog(
      NotificationDiagnosticsQuery query)
    {
      return new List<INotificationDiagnosticLog>();
    }

    public virtual List<TeamFoundationNotification> GetUnprocessedNotificationsWorker(
      int lastNotificationId,
      int notificationBatchSize,
      IEnumerable<string> channels,
      IEnumerable<string> processQueues,
      int failedRetryInterval)
    {
      this.PrepareStoredProcedure("prc_GetUnprocessedNotifications");
      this.BindInt("@lastNotificationId", lastNotificationId);
      this.BindInt("@batchSize", notificationBatchSize);
      this.BindBoolean("@includePreviouslyFailed", true);
      this.BindInt("@failedRetryInterval", failedRetryInterval);
      this.BindChannels(channels);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<TeamFoundationNotification>((ObjectBinder<TeamFoundationNotification>) this.GetNotificationVersion());
      return resultCollection.GetCurrent<TeamFoundationNotification>().Items;
    }

    public virtual void SaveProcessedNotifications(IList<TeamFoundationNotification> notifications)
    {
      this.PrepareStoredProcedure("prc_SaveProcessedNotifications");
      List<int> rows1 = new List<int>();
      List<int> rows2 = new List<int>();
      foreach (TeamFoundationNotification notification in (IEnumerable<TeamFoundationNotification>) notifications)
      {
        if (notification.SendNotificationState.IsFailedRetryState())
          rows2.Add(notification.Id);
        else
          rows1.Add(notification.Id);
      }
      this.BindInt32Table("@failedNotifications", (IEnumerable<int>) rows2);
      this.BindInt32Table("@deliveredNotifications", (IEnumerable<int>) rows1);
      this.ExecuteNonQuery();
    }

    internal virtual TeamFoundationNotificationBinder GetNotificationVersion() => new TeamFoundationNotificationBinder((TeamFoundationSqlResourceComponent) this);

    protected virtual SqlDataRecord NotificationTableRowBinder(
      TeamFoundationNotification notification)
    {
      SqlDataRecord notificationTableRecord = this.NotificationTableRecord;
      notificationTableRecord.SetInt32(0, notification.Event.Id);
      notificationTableRecord.SetInt32(1, -1);
      notificationTableRecord.SetInt32(2, 5);
      notificationTableRecord.SetGuid(3, notification.SingleSubscriberOverrideId);
      notificationTableRecord.SetInt32(4, 0);
      notificationTableRecord.SetString(5, notification.Channel);
      if (notification.Metadata != null)
        notificationTableRecord.SetString(6, notification.Metadata);
      else
        notificationTableRecord.SetDBNull(6);
      if (!string.IsNullOrEmpty(notification.DeliveryDetails?.NotificationSource))
        notificationTableRecord.SetString(7, notification.DeliveryDetails.NotificationSource);
      else
        notificationTableRecord.SetDBNull(7);
      return notificationTableRecord;
    }

    protected virtual SqlParameter BindNotificationTable(
      string parameterName,
      IEnumerable<TeamFoundationNotification> rows)
    {
      rows = rows ?? Enumerable.Empty<TeamFoundationNotification>();
      return this.BindTable(parameterName, this.NotificationTableName, rows.Select<TeamFoundationNotification, SqlDataRecord>(new System.Func<TeamFoundationNotification, SqlDataRecord>(this.NotificationTableRowBinder)));
    }

    public virtual void SuspendUnprocessedNotifications(
      IEnumerable<NotificationQueryCondition> subscriptionIds,
      int batchSize)
    {
    }

    public virtual DateTime GetNextEventProcessTime(IEnumerable<string> processQueues) => DateTime.MaxValue;

    public virtual DateTime GetNextNotificationProcessTime(
      IEnumerable<string> channels,
      IEnumerable<string> processQueues)
    {
      return DateTime.MaxValue;
    }

    protected virtual string NotificationTableName => "typ_NotificationTable5";

    protected virtual string NotificationStatisticsTableName => "typ_NotificationStatistics";

    protected virtual SqlDataRecord NotificationTableRecord => new SqlDataRecord(NotificationEventTypes.typ_NotificationTable5);

    public virtual int CleanupStatistics(
      int dailyStatisticsAgeDays,
      int hourlyStatisticsAgeDays,
      int keepTopN,
      int batchSize)
    {
      return -1;
    }

    public virtual List<NotificationStatistic> QueryNotificationStatistics(
      IEnumerable<NotificationStatisticsQueryConditions> query)
    {
      return new List<NotificationStatistic>();
    }

    public virtual NotificationEventBacklogStatus QueryNotificationBacklogStatus(
      int maxAllowedDelayDays,
      HashSet<Tuple<string, string>> processQueueChannels)
    {
      return new NotificationEventBacklogStatus();
    }

    internal virtual EventBacklogStatusBinder GetEventBacklogStatusVersion() => new EventBacklogStatusBinder();

    internal virtual NotificationBacklogStatusBinder GetNotificationBacklogStatusVersion() => new NotificationBacklogStatusBinder();
  }
}

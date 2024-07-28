// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.NotificationEventTypes
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.SqlServer.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal class NotificationEventTypes
  {
    internal static readonly SqlMetaData[] typ_DefaultSubscriptionUserCandidates = new SqlMetaData[2]
    {
      new SqlMetaData("SubscriberId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("SubscriptionName", SqlDbType.VarChar, 100L)
    };
    internal static readonly SqlMetaData[] typ_DefaultSubscriptionAdminCandidates = new SqlMetaData[1]
    {
      new SqlMetaData("SubscriptionName", SqlDbType.VarChar, 100L)
    };
    internal static readonly SqlMetaData[] typ_NotificationKey = new SqlMetaData[5]
    {
      new SqlMetaData("KeyId", SqlDbType.Int),
      new SqlMetaData("QueryType", SqlDbType.Int),
      new SqlMetaData("NotificationId", SqlDbType.Int),
      new SqlMetaData("SubscriptionUniqueId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("IncludeResultDetail", SqlDbType.Int)
    };
    internal static readonly SqlMetaData[] typ_SubscriptionKey3 = new SqlMetaData[12]
    {
      new SqlMetaData("Id", SqlDbType.Int),
      new SqlMetaData("SubscriberId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Matcher", SqlDbType.VarChar, 25L),
      new SqlMetaData("EventTypeName", SqlDbType.VarChar, 100L),
      new SqlMetaData("IndexedExpression", SqlDbType.NVarChar, 350L),
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("Classification", SqlDbType.NVarChar, 2048L),
      new SqlMetaData("Channel", SqlDbType.VarChar, 25L),
      new SqlMetaData("Metadata", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Flags", SqlDbType.Int),
      new SqlMetaData("RestrictToDataspace", SqlDbType.Bit),
      new SqlMetaData("ScopeId", SqlDbType.UniqueIdentifier)
    };
    internal static readonly SqlMetaData[] typ_SubscriptionKey4 = new SqlMetaData[13]
    {
      new SqlMetaData("Id", SqlDbType.Int),
      new SqlMetaData("SubscriberId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Matcher", SqlDbType.VarChar, 25L),
      new SqlMetaData("EventTypeName", SqlDbType.VarChar, 100L),
      new SqlMetaData("IndexedExpression", SqlDbType.NVarChar, 350L),
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("Classification", SqlDbType.NVarChar, 2048L),
      new SqlMetaData("Channel", SqlDbType.VarChar, 25L),
      new SqlMetaData("Metadata", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Flags", SqlDbType.Int),
      new SqlMetaData("RestrictToDataspace", SqlDbType.Bit),
      new SqlMetaData("ScopeId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("UniqueId", SqlDbType.UniqueIdentifier)
    };
    internal static readonly SqlMetaData[] typ_SubscriptionKey5 = new SqlMetaData[13]
    {
      new SqlMetaData("QueryType", SqlDbType.Int),
      new SqlMetaData("Id", SqlDbType.Int),
      new SqlMetaData("SubscriberId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Matcher", SqlDbType.VarChar, 25L),
      new SqlMetaData("EventTypeName", SqlDbType.VarChar, 100L),
      new SqlMetaData("IndexedExpression", SqlDbType.NVarChar, 350L),
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("Classification", SqlDbType.NVarChar, 2048L),
      new SqlMetaData("Channel", SqlDbType.VarChar, 25L),
      new SqlMetaData("Metadata", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Flags", SqlDbType.Int),
      new SqlMetaData("ScopeId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("UniqueId", SqlDbType.UniqueIdentifier)
    };
    internal static readonly SqlMetaData[] typ_SubscriptionKey6 = new SqlMetaData[13]
    {
      new SqlMetaData("QueryType", SqlDbType.Int),
      new SqlMetaData("Id", SqlDbType.Int),
      new SqlMetaData("SubscriberId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Matcher", SqlDbType.VarChar, 25L),
      new SqlMetaData("EventTypeName", SqlDbType.VarChar, 100L),
      new SqlMetaData("IndexedExpression", SqlDbType.NVarChar, 350L),
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("Classification", SqlDbType.NVarChar, 2048L),
      new SqlMetaData("Channel", SqlDbType.VarChar, 100L),
      new SqlMetaData("Metadata", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Flags", SqlDbType.Int),
      new SqlMetaData("ScopeId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("UniqueId", SqlDbType.UniqueIdentifier)
    };
    internal static readonly SqlMetaData[] typ_SubscriptionUpdate4 = new SqlMetaData[15]
    {
      new SqlMetaData("SubscriptionId", SqlDbType.Int),
      new SqlMetaData("EventTypeName", SqlDbType.VarChar, 100L),
      new SqlMetaData("Expression", SqlDbType.NVarChar, -1L),
      new SqlMetaData("IndexedExpression", SqlDbType.NVarChar, 350L),
      new SqlMetaData("Channel", SqlDbType.VarChar, 25L),
      new SqlMetaData("Address", SqlDbType.NVarChar, 1024L),
      new SqlMetaData("Classification", SqlDbType.NVarChar, 2048L),
      new SqlMetaData("Description", SqlDbType.NVarChar, 2048L),
      new SqlMetaData("OwnerId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Metadata", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Status", SqlDbType.Int),
      new SqlMetaData("StatusMessage", SqlDbType.NVarChar, 1024L),
      new SqlMetaData("Flags", SqlDbType.Int),
      new SqlMetaData("Matcher", SqlDbType.VarChar, 25L),
      new SqlMetaData("ScopeId", SqlDbType.UniqueIdentifier)
    };
    internal static readonly SqlMetaData[] typ_SubscriptionUpdate5 = new SqlMetaData[16]
    {
      new SqlMetaData("SubscriptionId", SqlDbType.Int),
      new SqlMetaData("EventTypeName", SqlDbType.VarChar, 100L),
      new SqlMetaData("Expression", SqlDbType.NVarChar, -1L),
      new SqlMetaData("IndexedExpression", SqlDbType.NVarChar, 350L),
      new SqlMetaData("Channel", SqlDbType.VarChar, 25L),
      new SqlMetaData("Address", SqlDbType.NVarChar, 1024L),
      new SqlMetaData("Classification", SqlDbType.NVarChar, 2048L),
      new SqlMetaData("Description", SqlDbType.NVarChar, 2048L),
      new SqlMetaData("OwnerId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Metadata", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Status", SqlDbType.Int),
      new SqlMetaData("StatusMessage", SqlDbType.NVarChar, 1024L),
      new SqlMetaData("Flags", SqlDbType.Int),
      new SqlMetaData("Matcher", SqlDbType.VarChar, 25L),
      new SqlMetaData("ScopeId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Diagnostics", SqlDbType.VarChar, 2048L)
    };
    internal static readonly SqlMetaData[] typ_SubscriptionUpdate6 = new SqlMetaData[17]
    {
      new SqlMetaData("SubscriptionId", SqlDbType.Int),
      new SqlMetaData("EventTypeName", SqlDbType.VarChar, 100L),
      new SqlMetaData("Expression", SqlDbType.NVarChar, -1L),
      new SqlMetaData("IndexedExpression", SqlDbType.NVarChar, 350L),
      new SqlMetaData("Channel", SqlDbType.VarChar, 25L),
      new SqlMetaData("Address", SqlDbType.NVarChar, 1024L),
      new SqlMetaData("Classification", SqlDbType.NVarChar, 2048L),
      new SqlMetaData("Description", SqlDbType.NVarChar, 2048L),
      new SqlMetaData("OwnerId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Metadata", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Status", SqlDbType.Int),
      new SqlMetaData("StatusMessage", SqlDbType.NVarChar, 1024L),
      new SqlMetaData("Flags", SqlDbType.Int),
      new SqlMetaData("Matcher", SqlDbType.VarChar, 25L),
      new SqlMetaData("ScopeId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Diagnostics", SqlDbType.VarChar, 2048L),
      new SqlMetaData("SubscriberId", SqlDbType.UniqueIdentifier)
    };
    internal static readonly SqlMetaData[] typ_SubscriptionUpdate7 = new SqlMetaData[17]
    {
      new SqlMetaData("SubscriptionId", SqlDbType.Int),
      new SqlMetaData("EventTypeName", SqlDbType.VarChar, 100L),
      new SqlMetaData("Expression", SqlDbType.NVarChar, -1L),
      new SqlMetaData("IndexedExpression", SqlDbType.NVarChar, 350L),
      new SqlMetaData("Channel", SqlDbType.VarChar, 100L),
      new SqlMetaData("Address", SqlDbType.NVarChar, 1024L),
      new SqlMetaData("Classification", SqlDbType.NVarChar, 2048L),
      new SqlMetaData("Description", SqlDbType.NVarChar, 2048L),
      new SqlMetaData("OwnerId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Metadata", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Status", SqlDbType.Int),
      new SqlMetaData("StatusMessage", SqlDbType.NVarChar, 1024L),
      new SqlMetaData("Flags", SqlDbType.Int),
      new SqlMetaData("Matcher", SqlDbType.VarChar, 25L),
      new SqlMetaData("ScopeId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Diagnostics", SqlDbType.VarChar, 2048L),
      new SqlMetaData("SubscriberId", SqlDbType.UniqueIdentifier)
    };
    internal static readonly SqlMetaData[] typ_Subscription2 = new SqlMetaData[16]
    {
      new SqlMetaData("EventTypeName", SqlDbType.NVarChar, 100L),
      new SqlMetaData("Expression", SqlDbType.NVarChar, -1L),
      new SqlMetaData("SubscriberId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Address", SqlDbType.NVarChar, 1024L),
      new SqlMetaData("Classification", SqlDbType.NVarChar, 2048L),
      new SqlMetaData("Description", SqlDbType.NVarChar, 2048L),
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("OwnerId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("IndexedExpression", SqlDbType.NVarChar, 350L),
      new SqlMetaData("Matcher", SqlDbType.NVarChar, 25L),
      new SqlMetaData("Metadata", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Channel", SqlDbType.VarChar, 25L),
      new SqlMetaData("Status", SqlDbType.Int),
      new SqlMetaData("StatusMessage", SqlDbType.NVarChar, 1024L),
      new SqlMetaData("Flags", SqlDbType.Int),
      new SqlMetaData("ScopeId", SqlDbType.UniqueIdentifier)
    };
    internal static readonly SqlMetaData[] typ_Subscription3 = new SqlMetaData[17]
    {
      new SqlMetaData("EventTypeName", SqlDbType.NVarChar, 100L),
      new SqlMetaData("Expression", SqlDbType.NVarChar, -1L),
      new SqlMetaData("SubscriberId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Address", SqlDbType.NVarChar, 1024L),
      new SqlMetaData("Classification", SqlDbType.NVarChar, 2048L),
      new SqlMetaData("Description", SqlDbType.NVarChar, 2048L),
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("OwnerId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("IndexedExpression", SqlDbType.NVarChar, 350L),
      new SqlMetaData("Matcher", SqlDbType.NVarChar, 25L),
      new SqlMetaData("Metadata", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Channel", SqlDbType.VarChar, 25L),
      new SqlMetaData("Status", SqlDbType.Int),
      new SqlMetaData("StatusMessage", SqlDbType.NVarChar, 1024L),
      new SqlMetaData("Flags", SqlDbType.Int),
      new SqlMetaData("ScopeId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("UniqueId", SqlDbType.UniqueIdentifier)
    };
    internal static readonly SqlMetaData[] typ_Subscription4 = new SqlMetaData[18]
    {
      new SqlMetaData("EventTypeName", SqlDbType.NVarChar, 100L),
      new SqlMetaData("Expression", SqlDbType.NVarChar, -1L),
      new SqlMetaData("SubscriberId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Address", SqlDbType.NVarChar, 1024L),
      new SqlMetaData("Classification", SqlDbType.NVarChar, 2048L),
      new SqlMetaData("Description", SqlDbType.NVarChar, 2048L),
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("OwnerId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("IndexedExpression", SqlDbType.NVarChar, 350L),
      new SqlMetaData("Matcher", SqlDbType.NVarChar, 25L),
      new SqlMetaData("Metadata", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Channel", SqlDbType.VarChar, 25L),
      new SqlMetaData("Status", SqlDbType.Int),
      new SqlMetaData("StatusMessage", SqlDbType.NVarChar, 1024L),
      new SqlMetaData("Flags", SqlDbType.Int),
      new SqlMetaData("ScopeId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("UniqueId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Diagnostics", SqlDbType.VarChar, 2048L)
    };
    internal static readonly SqlMetaData[] typ_Subscription5 = new SqlMetaData[18]
    {
      new SqlMetaData("EventTypeName", SqlDbType.NVarChar, 100L),
      new SqlMetaData("Expression", SqlDbType.NVarChar, -1L),
      new SqlMetaData("SubscriberId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Address", SqlDbType.NVarChar, 1024L),
      new SqlMetaData("Classification", SqlDbType.NVarChar, 2048L),
      new SqlMetaData("Description", SqlDbType.NVarChar, 2048L),
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("OwnerId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("IndexedExpression", SqlDbType.NVarChar, 350L),
      new SqlMetaData("Matcher", SqlDbType.NVarChar, 25L),
      new SqlMetaData("Metadata", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Channel", SqlDbType.VarChar, 100L),
      new SqlMetaData("Status", SqlDbType.Int),
      new SqlMetaData("StatusMessage", SqlDbType.NVarChar, 1024L),
      new SqlMetaData("Flags", SqlDbType.Int),
      new SqlMetaData("ScopeId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("UniqueId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Diagnostics", SqlDbType.VarChar, 2048L)
    };
    internal static readonly SqlMetaData[] typ_ChannelTable = new SqlMetaData[1]
    {
      new SqlMetaData("Channel", SqlDbType.VarChar, 25L)
    };
    internal static readonly SqlMetaData[] typ_GetSubscriptionTable = new SqlMetaData[2]
    {
      new SqlMetaData("EventTypeName", SqlDbType.VarChar, 100L),
      new SqlMetaData("Matcher", SqlDbType.VarChar, 25L)
    };
    internal static readonly SqlMetaData[] typ_EventTypeTable = new SqlMetaData[3]
    {
      new SqlMetaData("ToolType", SqlDbType.NVarChar, 20L),
      new SqlMetaData("Name", SqlDbType.NVarChar, 128L),
      new SqlMetaData("EventTypeSchema", SqlDbType.NVarChar, -1L)
    };
    internal static readonly SqlMetaData[] typ_EventTable2 = new SqlMetaData[3]
    {
      new SqlMetaData("Id", SqlDbType.Int),
      new SqlMetaData("Matched", SqlDbType.Bit),
      new SqlMetaData("Ignored", SqlDbType.Bit)
    };
    internal static readonly SqlMetaData[] typ_EventTable3 = new SqlMetaData[5]
    {
      new SqlMetaData("Id", SqlDbType.Int),
      new SqlMetaData("Matches", SqlDbType.Int),
      new SqlMetaData("Status", SqlDbType.VarChar, 10L),
      new SqlMetaData("NextProcessTime", SqlDbType.DateTime),
      new SqlMetaData("ExpirationTime", SqlDbType.DateTime)
    };
    internal static readonly SqlMetaData[] typ_EventEntryTable4 = new SqlMetaData[5]
    {
      new SqlMetaData("EventTypeName", SqlDbType.VarChar, 100L),
      new SqlMetaData("Event", SqlDbType.NVarChar, -1L),
      new SqlMetaData("ArtifactUri", SqlDbType.NVarChar, 350L),
      new SqlMetaData("Actors", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Scopes", SqlDbType.NVarChar, -1L)
    };
    internal static readonly SqlMetaData[] typ_EventEntryTable5 = new SqlMetaData[6]
    {
      new SqlMetaData("EventTypeName", SqlDbType.VarChar, 100L),
      new SqlMetaData("Event", SqlDbType.NVarChar, -1L),
      new SqlMetaData("ArtifactUri", SqlDbType.NVarChar, 350L),
      new SqlMetaData("Actors", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Scopes", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Initiator", SqlDbType.UniqueIdentifier)
    };
    internal static readonly SqlMetaData[] typ_EventEntryTable6 = new SqlMetaData[7]
    {
      new SqlMetaData("EventTypeName", SqlDbType.VarChar, 100L),
      new SqlMetaData("Event", SqlDbType.NVarChar, -1L),
      new SqlMetaData("ArtifactUri", SqlDbType.NVarChar, 350L),
      new SqlMetaData("Actors", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Scopes", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Initiator", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ProcessQueue", SqlDbType.VarChar, 105L)
    };
    internal static readonly SqlMetaData[] typ_EventEntryTable7 = new SqlMetaData[8]
    {
      new SqlMetaData("EventTypeName", SqlDbType.VarChar, 100L),
      new SqlMetaData("Event", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Initiator", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ProcessQueue", SqlDbType.VarChar, 105L),
      new SqlMetaData("Status", SqlDbType.VarChar, 10L),
      new SqlMetaData("NextProcessTime", SqlDbType.DateTime),
      new SqlMetaData("ExpirationTime", SqlDbType.DateTime),
      new SqlMetaData("SourceEventCreatedTime", SqlDbType.DateTime)
    };
    internal static readonly SqlMetaData[] typ_TransformedEvent = new SqlMetaData[3]
    {
      new SqlMetaData("EventId", SqlDbType.Int),
      new SqlMetaData("Event", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Actors", SqlDbType.NVarChar, -1L)
    };
    internal static readonly SqlMetaData[] typ_NotificationTable5 = new SqlMetaData[8]
    {
      new SqlMetaData("EventId", SqlDbType.Int),
      new SqlMetaData("SubscriptionID", SqlDbType.Int),
      new SqlMetaData("RetryCount", SqlDbType.Int),
      new SqlMetaData("SubscriberOverrideId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("Channel", SqlDbType.NVarChar, 25L),
      new SqlMetaData("NotificationMetadata", SqlDbType.NVarChar, -1L),
      new SqlMetaData("NotificationSource", SqlDbType.VarChar, 100L)
    };
    internal static readonly SqlMetaData[] typ_NotificationTable6 = new SqlMetaData[9]
    {
      new SqlMetaData("EventId", SqlDbType.Int),
      new SqlMetaData("SubscriptionID", SqlDbType.Int),
      new SqlMetaData("RetryCount", SqlDbType.Int),
      new SqlMetaData("SubscriberOverrideId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("Channel", SqlDbType.NVarChar, 25L),
      new SqlMetaData("NotificationMetadata", SqlDbType.NVarChar, -1L),
      new SqlMetaData("NotificationSource", SqlDbType.VarChar, 100L),
      new SqlMetaData("ProcessQueue", SqlDbType.VarChar, 100L)
    };
    internal static readonly SqlMetaData[] typ_NotificationTable7 = new SqlMetaData[9]
    {
      new SqlMetaData("EventId", SqlDbType.Int),
      new SqlMetaData("SubscriptionID", SqlDbType.Int),
      new SqlMetaData("RetryCount", SqlDbType.Int),
      new SqlMetaData("SubscriberOverrideId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("Channel", SqlDbType.NVarChar, 25L),
      new SqlMetaData("NotificationMetadata", SqlDbType.NVarChar, -1L),
      new SqlMetaData("NotificationSource", SqlDbType.VarChar, 100L),
      new SqlMetaData("ProcessQueue", SqlDbType.VarChar, 105L)
    };
    internal static readonly SqlMetaData[] typ_NotificationTable8 = new SqlMetaData[8]
    {
      new SqlMetaData("EventId", SqlDbType.Int),
      new SqlMetaData("SubscriberOverrideId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Channel", SqlDbType.NVarChar, 100L),
      new SqlMetaData("NotificationMetadata", SqlDbType.NVarChar, -1L),
      new SqlMetaData("NotificationSource", SqlDbType.VarChar, 100L),
      new SqlMetaData("ProcessQueue", SqlDbType.VarChar, 105L),
      new SqlMetaData("NextProcessTime", SqlDbType.DateTime),
      new SqlMetaData("ExpirationTime", SqlDbType.DateTime)
    };
    internal static readonly SqlMetaData[] typ_SuspendNotificationKey = new SqlMetaData[5]
    {
      new SqlMetaData("SubscriberId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("EventInitiator", SqlDbType.UniqueIdentifier),
      new SqlMetaData("NotificationSource", SqlDbType.VarChar, 100L),
      new SqlMetaData("EventTypeName", SqlDbType.VarChar, 100L),
      new SqlMetaData("Status", SqlDbType.VarChar, 15L)
    };
    internal static readonly SqlMetaData[] typ_NotificationStatistics = new SqlMetaData[5]
    {
      new SqlMetaData("TrackingDate", SqlDbType.DateTime),
      new SqlMetaData("StatisticType", SqlDbType.Int),
      new SqlMetaData("StatisticPath", SqlDbType.VarChar, 100L),
      new SqlMetaData("UserId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("HitCount", SqlDbType.Int)
    };
    internal static readonly SqlMetaData[] typ_NotificationStatisticsQuery2 = new SqlMetaData[6]
    {
      new SqlMetaData("TrackingDate", SqlDbType.DateTime),
      new SqlMetaData("StatisticType", SqlDbType.Int),
      new SqlMetaData("StatisticPath", SqlDbType.VarChar, 100L),
      new SqlMetaData("UserId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("HitCount", SqlDbType.Int),
      new SqlMetaData("EndDate", SqlDbType.DateTime)
    };
    internal static readonly SqlMetaData[] typ_NotificationStatusTable = new SqlMetaData[2]
    {
      new SqlMetaData("Id", SqlDbType.Int),
      new SqlMetaData("Status", SqlDbType.VarChar, 15L)
    };
    internal static readonly SqlMetaData[] typ_NotificationStatusTable2 = new SqlMetaData[4]
    {
      new SqlMetaData("Id", SqlDbType.Int),
      new SqlMetaData("Status", SqlDbType.VarChar, 15L),
      new SqlMetaData("Result", SqlDbType.NVarChar, -1L),
      new SqlMetaData("ResultDetail", SqlDbType.NVarChar, -1L)
    };
    internal static readonly SqlMetaData[] typ_NotificationStatusTable3 = new SqlMetaData[6]
    {
      new SqlMetaData("Id", SqlDbType.Int),
      new SqlMetaData("Status", SqlDbType.VarChar, 15L),
      new SqlMetaData("Result", SqlDbType.NVarChar, -1L),
      new SqlMetaData("ResultDetail", SqlDbType.NVarChar, -1L),
      new SqlMetaData("NextProcessTime", SqlDbType.DateTime),
      new SqlMetaData("ExpirationTime", SqlDbType.DateTime)
    };
    internal static readonly SqlMetaData[] typ_StringVarcharTable = new SqlMetaData[1]
    {
      new SqlMetaData("Data", SqlDbType.VarChar, -1L)
    };
    internal static readonly SqlMetaData[] typ_ProcessQueueChannelTable = new SqlMetaData[2]
    {
      new SqlMetaData("ProcessQueue", SqlDbType.VarChar, 100L),
      new SqlMetaData("Channel", SqlDbType.VarChar, 100L)
    };
    internal static readonly SqlMetaData[] typ_ProcessQueueTable = new SqlMetaData[1]
    {
      new SqlMetaData("ProcessQueue", SqlDbType.VarChar, 100L)
    };
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.SubscriptionBinder
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.Common;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal class SubscriptionBinder : ObjectBinder<Subscription>
  {
    private SqlColumnBinder LastModifiedByColumn = new SqlColumnBinder("LastModifiedBy");
    private SqlColumnBinder EventTypeNameColumn = new SqlColumnBinder("EventTypeName");
    private SqlColumnBinder StatusColumn = new SqlColumnBinder("Status");
    private SqlColumnBinder FlagsColumn = new SqlColumnBinder("Flags");
    private SqlColumnBinder StatusMessageColumn = new SqlColumnBinder("StatusMessage");
    private SqlColumnBinder ScopeIdColumn = new SqlColumnBinder("ScopeId");
    private SqlColumnBinder DataspaceIdColumn = new SqlColumnBinder("DataspaceId");
    private SqlColumnBinder IdColumn = new SqlColumnBinder("Id");
    private SqlColumnBinder EventTypeColumn = new SqlColumnBinder("EventType");
    private SqlColumnBinder ExpressionColumn = new SqlColumnBinder("Expression");
    private SqlColumnBinder IndexedExpressionColumn = new SqlColumnBinder("IndexedExpression");
    private SqlColumnBinder SubscriberIdColumn = new SqlColumnBinder("SubscriberId");
    private SqlColumnBinder TagColumn = new SqlColumnBinder("Classification");
    private SqlColumnBinder AddressColumn = new SqlColumnBinder("Address");
    private SqlColumnBinder DeliveryTypeColumn = new SqlColumnBinder("DeliveryType");
    private SqlColumnBinder ScheduleColumn = new SqlColumnBinder("Schedule");
    private SqlColumnBinder LastModifiedTimeColumn = new SqlColumnBinder("LastModifiedTime");
    private SqlColumnBinder OwnerIdColumn = new SqlColumnBinder("OwnerId");
    private SqlColumnBinder DescriptionColumn = new SqlColumnBinder("Description");
    private SqlColumnBinder SubscriptionMetadataColumn = new SqlColumnBinder("Metadata");
    private SqlColumnBinder MatcherColumn = new SqlColumnBinder("Matcher");
    private SqlColumnBinder ChannelColumn = new SqlColumnBinder("Channel");
    private SqlColumnBinder UniqueIdColumn = new SqlColumnBinder("UniqueId");
    private SqlColumnBinder DiagnosticsColumn = new SqlColumnBinder("Diagnostics");
    private TeamFoundationSqlResourceComponent sqlResourceComponent;

    public SubscriptionBinder(TeamFoundationSqlResourceComponent component) => this.sqlResourceComponent = component;

    protected override Subscription Bind() => this.BindSubscription();

    internal static void MatcherMustBeXPath(string matcher)
    {
      if (matcher != "XPathMatcher")
        throw new ServiceVersionNotSupportedException();
    }

    internal static void ChannelMustBeDeliveryType(string channel)
    {
      if (DeliveryTypeChannelMapper.GetDeliveryType(channel) == (DeliveryType) 2147483647)
        throw new ServiceVersionNotSupportedException();
    }

    protected virtual Subscription BindSubscription()
    {
      Subscription subscription = new Subscription();
      subscription.ID = this.IdColumn.GetInt32((IDataReader) this.Reader);
      subscription.EventTypeName = this.EventTypeNameColumn.GetString((IDataReader) this.Reader, true) ?? string.Empty;
      subscription.Expression = this.ExpressionColumn.GetString((IDataReader) this.Reader, true) ?? string.Empty;
      subscription.SubscriberId = this.SubscriberIdColumn.GetGuid((IDataReader) this.Reader);
      subscription.Tag = this.TagColumn.GetString((IDataReader) this.Reader, true) ?? string.Empty;
      subscription.DeliveryAddress = this.AddressColumn.GetString((IDataReader) this.Reader, true) ?? string.Empty;
      subscription.ProjectId = this.sqlResourceComponent.GetDataspaceIdentifier(this.DataspaceIdColumn.GetInt32((IDataReader) this.Reader));
      subscription.Matcher = this.MatcherColumn.GetString((IDataReader) this.Reader, false);
      subscription.Channel = this.ChannelColumn.GetString((IDataReader) this.Reader, false);
      subscription.IndexedExpression = this.IndexedExpressionColumn.ColumnExists((IDataReader) this.Reader) ? this.IndexedExpressionColumn.GetString((IDataReader) this.Reader, true) : (string) null;
      subscription.Metadata = this.SubscriptionMetadataColumn.ColumnExists((IDataReader) this.Reader) ? this.SubscriptionMetadataColumn.GetString((IDataReader) this.Reader, true) : (string) null;
      subscription.ModifiedTime = this.LastModifiedTimeColumn.ColumnExists((IDataReader) this.Reader) ? this.LastModifiedTimeColumn.GetDateTime((IDataReader) this.Reader) : DateTime.MinValue;
      subscription.LastModifiedBy = this.LastModifiedByColumn.ColumnExists((IDataReader) this.Reader) ? this.LastModifiedByColumn.GetGuid((IDataReader) this.Reader, true) : Guid.Empty;
      subscription.Description = this.DescriptionColumn.ColumnExists((IDataReader) this.Reader) ? this.DescriptionColumn.GetString((IDataReader) this.Reader, true) : (string) null;
      subscription.Status = this.StatusColumn.ColumnExists((IDataReader) this.Reader) ? (SubscriptionStatus) this.StatusColumn.GetInt32((IDataReader) this.Reader, 0) : SubscriptionStatus.Enabled;
      subscription.Flags = this.FlagsColumn.ColumnExists((IDataReader) this.Reader) ? (SubscriptionFlags) this.FlagsColumn.GetInt32((IDataReader) this.Reader, 0) : SubscriptionFlags.None;
      subscription.StatusMessage = this.StatusMessageColumn.ColumnExists((IDataReader) this.Reader) ? this.StatusMessageColumn.GetString((IDataReader) this.Reader, true) : (string) null;
      SubscriptionScope subscriptionScope = new SubscriptionScope();
      subscriptionScope.Id = this.ScopeIdColumn.ColumnExists((IDataReader) this.Reader) ? this.ScopeIdColumn.GetGuid((IDataReader) this.Reader, true) : Guid.Empty;
      subscription.SubscriptionScope = subscriptionScope;
      subscription.UniqueId = this.UniqueIdColumn.ColumnExists((IDataReader) this.Reader) ? this.UniqueIdColumn.GetGuid((IDataReader) this.Reader, true) : Guid.Empty;
      subscription.Diagnostics = DiagnosticUtils.DeserializeDiagnostics(this.DiagnosticsColumn.GetString((IDataReader) this.Reader, (string) null));
      return subscription;
    }
  }
}

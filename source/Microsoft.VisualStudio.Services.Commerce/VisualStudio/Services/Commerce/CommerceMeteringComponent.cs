// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceMeteringComponent
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class CommerceMeteringComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[16]
    {
      (IComponentCreator) new ComponentCreator<CommerceMeteringComponent2>(3),
      (IComponentCreator) new ComponentCreator<CommerceMeteringComponent3>(4),
      (IComponentCreator) new ComponentCreator<CommerceMeteringComponent4>(5),
      (IComponentCreator) new ComponentCreator<CommerceMeteringComponent5>(6),
      (IComponentCreator) new ComponentCreator<CommerceMeteringComponent6>(7),
      (IComponentCreator) new ComponentCreator<CommerceMeteringComponent7>(8),
      (IComponentCreator) new ComponentCreator<CommerceMeteringComponent8>(9),
      (IComponentCreator) new ComponentCreator<CommerceMeteringComponent9>(10),
      (IComponentCreator) new ComponentCreator<CommerceMeteringComponent10>(11),
      (IComponentCreator) new ComponentCreator<CommerceMeteringComponent11>(12),
      (IComponentCreator) new ComponentCreator<CommerceMeteringComponent12>(13),
      (IComponentCreator) new ComponentCreator<CommerceMeteringComponent13>(14),
      (IComponentCreator) new ComponentCreator<CommerceMeteringComponent14>(15),
      (IComponentCreator) new ComponentCreator<CommerceMeteringComponent15>(16),
      (IComponentCreator) new ComponentCreator<CommerceMeteringComponent16>(17),
      (IComponentCreator) new ComponentCreator<CommerceMeteringComponent17>(18)
    }, "VsCommerce_Partition");
    private static List<SqlMetaData> typ_SubscriptionResourceUsageTable = new List<SqlMetaData>()
    {
      new SqlMetaData("ResourceId", SqlDbType.Int),
      new SqlMetaData("ResourceSeq", SqlDbType.TinyInt),
      new SqlMetaData("CurrentQuantity", SqlDbType.Int),
      new SqlMetaData("CommittedQuantity", SqlDbType.Int),
      new SqlMetaData("IncludedQuantity", SqlDbType.Int),
      new SqlMetaData("MaxQuantity", SqlDbType.Int),
      new SqlMetaData("IsPaidBillingEnabled", SqlDbType.Bit),
      new SqlMetaData("PaidBillingUpdated", SqlDbType.DateTime),
      new SqlMetaData("LastResetDate", SqlDbType.DateTime),
      new SqlMetaData("LastUpdated", SqlDbType.DateTime),
      new SqlMetaData("LastUpdatedBy", SqlDbType.UniqueIdentifier),
      new SqlMetaData("IsTrialOrPreview", SqlDbType.Bit),
      new SqlMetaData("StartDate", SqlDbType.DateTime)
    };
    private static Dictionary<int, SqlExceptionFactory> sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>()
    {
      {
        2100002,
        new SqlExceptionFactory(typeof (CommerceInvalidQuantitySqlException))
      },
      {
        2100006,
        new SqlExceptionFactory(typeof (CommerceInvalidQuantitySqlException))
      },
      {
        2100005,
        new SqlExceptionFactory(typeof (CommerceInvalidQuantitySqlException))
      },
      {
        2100003,
        new SqlExceptionFactory(typeof (CommerceInvalidQuantitySqlException))
      },
      {
        2100004,
        new SqlExceptionFactory(typeof (CommerceInvalidQuantitySqlException))
      },
      {
        2100007,
        new SqlExceptionFactory(typeof (CommerceInvalidQuantitySqlException))
      },
      {
        2100008,
        new SqlExceptionFactory(typeof (CommerceInvalidResourceSqlException))
      },
      {
        2100012,
        new SqlExceptionFactory(typeof (CommerceResourceUpdateFailedSqlException))
      }
    };

    public CommerceMeteringComponent() => this.ContainerErrorCode = 50000;

    protected override void SetupComponentCircuitBreaker(
      IVssRequestContext requestContext,
      string databaseName,
      ApplicationIntent applicationIntent)
    {
      string key = "CommerceMeteringComponent-" + databaseName;
      this.ComponentLevelCommandSetter.AndCommandKey((Microsoft.VisualStudio.Services.CircuitBreaker.CommandKey) key);
      this.ComponentLevelCircuitBreakerProperties = (ICommandProperties) new CommandPropertiesRegistry(requestContext, (Microsoft.VisualStudio.Services.CircuitBreaker.CommandKey) key, this.ComponentLevelCommandSetter.CommandPropertiesDefaults);
    }

    [ExcludeFromCodeCoverage]
    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) CommerceMeteringComponent.sqlExceptionFactories;

    [ExcludeFromCodeCoverage]
    internal virtual IVssRequestContext ComponentRequestContext => this.RequestContext;

    protected override string TraceArea => "Commerce";

    protected virtual string Layer => nameof (CommerceMeteringComponent);

    internal virtual IEnumerable<OfferSubscriptionInternal> GetMeteredResources(int? meterId) => throw new NotSupportedException();

    internal virtual IEnumerable<OfferSubscriptionInternal> GetMeteredResources(
      int? meterId,
      byte? resourceSeq)
    {
      throw new NotSupportedException();
    }

    internal virtual void UpdatePaidBillingMode(
      int meterId,
      bool paidBillingEnabled,
      Guid userIdentityId)
    {
      throw new NotSupportedException();
    }

    internal virtual AggregateUsageEventResult AggregateUsageEvents(
      int meterId,
      byte resourceSeq,
      UsageEvent usageEvent,
      Guid lastUpdatedBy,
      int defaultIncludedQuantities,
      ResourceBillingMode billingMode,
      Guid meterGuid,
      BillingProvider billingProvider)
    {
      throw new NotSupportedException();
    }

    internal virtual AggregateUsageEventResult AggregateUsageEvents(
      int meterId,
      byte resourceSeq,
      UsageEvent usageEvent,
      Guid lastUpdatedBy,
      int defaultIncludedQuantities,
      ResourceBillingMode billingMode,
      bool prorateAllowed,
      BillingProvider billingProvider,
      DateTime? executionDate = null)
    {
      throw new NotSupportedException();
    }

    internal virtual MeterResetEvents ResetResourceUsage(
      bool monthlyReset,
      Guid subscriptionId,
      IEnumerable<KeyValuePair<int, int>> includedQuantities,
      IEnumerable<KeyValuePair<int, string>> billingModes,
      bool isResetOnlyCurrentQuantities,
      DateTime? executionDate = null)
    {
      throw new NotSupportedException();
    }

    internal virtual void UpdateAccountQuantities(
      int meterId,
      int includedQuantity,
      int maximumQuantity,
      int defaultIncludedQuantity,
      int defaultMaxQuantity,
      int absoluteMaxQuantity,
      ResourceBillingMode billingMode,
      Guid userIdentityId,
      bool resetUsage)
    {
      throw new NotSupportedException();
    }

    internal virtual void UpdateAccountQuantities(
      int meterId,
      byte resourceSeq,
      int? includedQuantity,
      int? maximumQuantity,
      int defaultIncludedQuantity,
      int defaultMaxQuantity,
      int absoluteMaxQuantity,
      ResourceBillingMode billingMode,
      Guid userIdentityId,
      bool resetUsage)
    {
      throw new NotSupportedException();
    }

    internal virtual OfferSubscriptionInternal AddTrialOfferSubscription(
      int meterId,
      byte resourceSeq,
      Guid lastUpdatedBy,
      int includedQuantity = 0)
    {
      throw new NotSupportedException();
    }

    internal virtual OfferSubscriptionInternal RemoveTrialForPaidOfferSubscription(
      int meterId,
      byte resourceSeq,
      int defaultIncludedQuantity,
      Guid lastUpdatedBy)
    {
      throw new NotSupportedException();
    }

    internal virtual void SetSubscriptionResourceUsage(
      IEnumerable<SubscriptionResourceUsage> accountQuantities)
    {
      throw new NotSupportedException();
    }

    internal virtual void MigrateSubscriptionResourceUsages(
      IEnumerable<SubscriptionResourceUsage> resourceUsages,
      bool isTarget)
    {
      throw new NotSupportedException();
    }

    internal virtual OfferSubscriptionInternal ExtendTrialOfferSubscription(
      int meterId,
      byte resourceSeq,
      Guid lastUpdatedBy,
      int trialDays)
    {
      throw new NotSupportedException();
    }

    internal virtual int UpdateCommittedAndCurrentQuantities(
      int meterId,
      byte resourceSeq,
      int committedQuantity,
      int currentQuantity,
      Guid lastUpdatedBy)
    {
      throw new NotSupportedException();
    }

    internal virtual OfferSubscriptionInternal ResetCloudLoadTestUsage(
      int meterId,
      byte resourceSeq,
      Guid lastUpdatedBy)
    {
      throw new NotSupportedException();
    }

    public IEnumerable<SqlDataRecord> BindSubscriptionResourceUsage(
      IEnumerable<SubscriptionResourceUsage> resourceUsages)
    {
      foreach (SubscriptionResourceUsage resourceUsage in resourceUsages)
        yield return this.BindSingleSubscriptionResourceUsage(resourceUsage);
    }

    protected virtual SqlDataRecord BindSingleSubscriptionResourceUsage(
      SubscriptionResourceUsage resourceUsage)
    {
      SqlDataRecord record = new SqlDataRecord(this.Typ_SubscriptionResourceUsageTable.ToArray());
      record.SetInt32(0, resourceUsage.ResourceId);
      record.SetByte(1, resourceUsage.ResourceSeq);
      record.SetInt32(2, resourceUsage.CurrentQuantity);
      record.SetInt32(3, resourceUsage.CommittedQuantity);
      record.SetInt32(4, resourceUsage.IncludedQuantity);
      record.SetInt32(5, resourceUsage.MaxQuantity);
      record.SetBoolean(6, resourceUsage.IsPaidBillingEnabled);
      record.SetDateTime(7, resourceUsage.PaidBillingUpdated);
      record.SetNullableDateTime(8, resourceUsage.LastResetDate);
      record.SetDateTime(9, resourceUsage.LastUpdated);
      record.SetGuid(10, resourceUsage.LastUpdatedBy);
      record.SetBoolean(11, resourceUsage.IsTrialOrPreview);
      record.SetNullableDateTime(12, resourceUsage.StartDate);
      return record;
    }

    protected virtual string Typ_SubscriptionResourceUsageTableName => "Commerce.typ_SubscriptionResourceUsageTable";

    protected virtual List<SqlMetaData> Typ_SubscriptionResourceUsageTable => CommerceMeteringComponent.typ_SubscriptionResourceUsageTable;
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.OfferSubscriptionBinder
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class OfferSubscriptionBinder : ObjectBinder<OfferSubscriptionInternal>
  {
    private SqlColumnBinder meterIdColumn = new SqlColumnBinder("ResourceId");
    private SqlColumnBinder resourceSeqColumn = new SqlColumnBinder("ResourceSeq");
    private SqlColumnBinder currentQuantityColumn = new SqlColumnBinder("CurrentQuantity");
    private SqlColumnBinder committedQuantityColumn = new SqlColumnBinder("CommittedQuantity");
    private SqlColumnBinder includedQuantityColumn = new SqlColumnBinder("IncludedQuantity");
    private SqlColumnBinder isPaidBillingEnabledColumn = new SqlColumnBinder("IsPaidBillingEnabled");
    private SqlColumnBinder paidBillingUpdatedDateColumn = new SqlColumnBinder("PaidBillingUpdated");
    private SqlColumnBinder maxQuantityColumn = new SqlColumnBinder("MaxQuantity");
    private SqlColumnBinder lastResetDateColumn = new SqlColumnBinder("LastResetDate");
    private SqlColumnBinder isTrialOrPreviewColumn = new SqlColumnBinder("IsTrialOrPreview");
    private SqlColumnBinder startDateColumn = new SqlColumnBinder("StartDate");
    private SqlColumnBinder lastUpdatedColumn = new SqlColumnBinder("LastUpdated");
    private SqlColumnBinder trialDaysColumn = new SqlColumnBinder("TrialDays");
    private SqlColumnBinder autoAssignOnAccessColumn = new SqlColumnBinder("AutoAssignOnAccess");
    private IVssRequestContext requestContext;
    private static string Area = "Commerce";
    private static string Layer = nameof (OfferSubscriptionBinder);

    public OfferSubscriptionBinder(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(5203010, OfferSubscriptionBinder.Area, OfferSubscriptionBinder.Layer, ".ctor");
      this.requestContext = requestContext;
      this.requestContext.TraceLeave(5203012, OfferSubscriptionBinder.Area, OfferSubscriptionBinder.Layer, ".ctor");
    }

    [ExcludeFromCodeCoverage]
    protected override OfferSubscriptionInternal Bind() => this.GetResourceFromReader((DbDataReader) this.Reader);

    internal OfferSubscriptionInternal GetResourceFromReader(DbDataReader reader)
    {
      try
      {
        this.requestContext.TraceEnter(5203014, OfferSubscriptionBinder.Area, OfferSubscriptionBinder.Layer, nameof (GetResourceFromReader));
        OfferSubscriptionInternal offerSub = new OfferSubscriptionInternal()
        {
          CommittedQuantity = this.committedQuantityColumn.GetInt32((IDataReader) reader, 0),
          CurrentQuantity = this.currentQuantityColumn.GetInt32((IDataReader) reader, 0),
          IncludedQuantity = this.includedQuantityColumn.GetInt32((IDataReader) reader, -1),
          IsPaidBillingEnabled = this.isPaidBillingEnabledColumn.GetBoolean((IDataReader) reader),
          PaidBillingUpdatedDate = this.paidBillingUpdatedDateColumn.GetDateTime((IDataReader) reader),
          MeterId = this.meterIdColumn.GetInt32((IDataReader) reader),
          RenewalGroup = (ResourceRenewalGroup) this.resourceSeqColumn.GetByte((IDataReader) reader, (byte) 0, (byte) 0),
          MaximumQuantity = this.maxQuantityColumn.GetInt32((IDataReader) reader, -1),
          LastResetDate = this.lastResetDateColumn.GetDateTime((IDataReader) reader, DateTime.MinValue),
          IsTrialOrPreview = this.isTrialOrPreviewColumn.ColumnExists((IDataReader) reader) && this.isTrialOrPreviewColumn.GetBoolean((IDataReader) reader, false),
          StartDate = new DateTime?(this.startDateColumn.GetDateTime((IDataReader) reader, DateTime.MinValue)),
          IsDefaultEmptyEntry = false,
          LastUpdated = this.lastUpdatedColumn.GetDateTime((IDataReader) reader, DateTime.MinValue),
          TrialDays = this.trialDaysColumn.GetInt32((IDataReader) reader, -1, -1)
        };
        this.SetAutoAssignOnAccess(reader, offerSub);
        return offerSub;
      }
      catch (Exception ex)
      {
        this.requestContext.TraceException(5203016, OfferSubscriptionBinder.Area, OfferSubscriptionBinder.Layer, ex);
        throw;
      }
      finally
      {
        this.requestContext.TraceLeave(5203018, OfferSubscriptionBinder.Area, OfferSubscriptionBinder.Layer, nameof (GetResourceFromReader));
      }
    }

    private void SetAutoAssignOnAccess(DbDataReader reader, OfferSubscriptionInternal offerSub)
    {
      bool? state = !this.autoAssignOnAccessColumn.ColumnExists((IDataReader) reader) || this.autoAssignOnAccessColumn.IsNull((IDataReader) reader) ? new bool?() : new bool?(this.autoAssignOnAccessColumn.GetBoolean((IDataReader) reader));
      offerSub.SetAutoAssignOnAccess(state);
    }
  }
}

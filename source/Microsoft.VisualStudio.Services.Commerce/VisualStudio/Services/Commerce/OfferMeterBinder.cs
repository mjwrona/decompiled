// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.OfferMeterBinder
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
  internal class OfferMeterBinder : ObjectBinder<OfferMeter>
  {
    private SqlColumnBinder meterIdColumn = new SqlColumnBinder("MeterId");
    private SqlColumnBinder platformMeterIdColumn = new SqlColumnBinder("PlatformMeterId");
    private SqlColumnBinder galleryIdColumn = new SqlColumnBinder("GalleryId");
    private SqlColumnBinder nameColumn = new SqlColumnBinder("Name");
    private SqlColumnBinder renewalFrequencyColumn = new SqlColumnBinder("RenewalFrequency");
    private SqlColumnBinder categoryColumn = new SqlColumnBinder("Category");
    private SqlColumnBinder unitColumn = new SqlColumnBinder("Unit");
    private SqlColumnBinder billingModeColumn = new SqlColumnBinder("BillingMode");
    private SqlColumnBinder scopeColumn = new SqlColumnBinder("Scope");
    private SqlColumnBinder billingStateColumn = new SqlColumnBinder("BillingState");
    private SqlColumnBinder meterStatusColumn = new SqlColumnBinder("MeterStatus");
    private SqlColumnBinder assignmentModelColumn = new SqlColumnBinder("AssignmentModel");
    private SqlColumnBinder billingStartDateColumn = new SqlColumnBinder("BillingStartDate");
    private SqlColumnBinder trialDaysColumn = new SqlColumnBinder("TrialDays");
    private SqlColumnBinder previewGraceDaysColumn = new SqlColumnBinder("PreviewGraceDays");
    private SqlColumnBinder currentQuantityColumn = new SqlColumnBinder("CurrentQuantity");
    private SqlColumnBinder committedQuantityColumn = new SqlColumnBinder("CommittedQuantity");
    private SqlColumnBinder includedQuantityColumn = new SqlColumnBinder("IncludedQuantity");
    private SqlColumnBinder maxQuantityColumn = new SqlColumnBinder("MaximumQuantity");
    private SqlColumnBinder absMaxQuantityColumn = new SqlColumnBinder("AbsoluteMaximumQuantity");
    private SqlColumnBinder trialCyclesColumn = new SqlColumnBinder("TrialCycles");
    private SqlColumnBinder billingProviderColumn = new SqlColumnBinder("BillingProvider");
    private SqlColumnBinder minRequiredAccessLevel = new SqlColumnBinder("MinimumRequiredAccessLevel");
    private SqlColumnBinder IncludedInLicenseLevel = new SqlColumnBinder(nameof (IncludedInLicenseLevel));
    private SqlColumnBinder autoAssignOnAccess = new SqlColumnBinder("AutoAssignOnAccess");
    private IVssRequestContext requestContext;
    private static string Area = "Commerce";
    private static string Layer = nameof (OfferMeterBinder);

    public OfferMeterBinder(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(5203010, OfferMeterBinder.Area, OfferMeterBinder.Layer, ".ctor");
      this.requestContext = requestContext;
      this.requestContext.TraceLeave(5203012, OfferMeterBinder.Area, OfferMeterBinder.Layer, ".ctor");
    }

    [ExcludeFromCodeCoverage]
    protected override OfferMeter Bind() => this.GetResourceFromReader((DbDataReader) this.Reader);

    internal OfferMeter GetResourceFromReader(DbDataReader reader)
    {
      try
      {
        this.requestContext.TraceEnter(5203014, OfferMeterBinder.Area, OfferMeterBinder.Layer, nameof (GetResourceFromReader));
        DateTime dateTime = this.billingStartDateColumn.GetDateTime((IDataReader) reader, DateTime.MinValue);
        return new OfferMeter()
        {
          MeterId = this.meterIdColumn.GetInt32((IDataReader) reader),
          PlatformMeterId = this.platformMeterIdColumn.GetGuid((IDataReader) reader),
          GalleryId = this.galleryIdColumn.GetString((IDataReader) reader, true),
          Name = this.nameColumn.GetString((IDataReader) reader, false),
          RenewalFrequency = (MeterRenewalFrequecy) this.renewalFrequencyColumn.GetByte((IDataReader) reader),
          BillingMode = OfferMeterBinder.GetBillingMode(this.billingModeColumn.GetString((IDataReader) reader, false)[0]),
          Category = (MeterCategory) this.categoryColumn.GetByte((IDataReader) reader),
          OfferScope = (OfferScope) this.scopeColumn.GetByte((IDataReader) reader),
          BillingState = (MeterBillingState) this.billingStateColumn.GetByte((IDataReader) reader),
          Status = (MeterState) this.meterStatusColumn.GetByte((IDataReader) reader),
          Unit = this.unitColumn.GetString((IDataReader) reader, false),
          AssignmentModel = (OfferMeterAssignmentModel) this.assignmentModelColumn.GetByte((IDataReader) reader, (byte) 0, (byte) 0),
          BillingStartDate = dateTime == DateTime.MinValue ? new DateTime?() : new DateTime?(dateTime),
          TrialDays = this.trialDaysColumn.GetByte((IDataReader) reader, (byte) 0, (byte) 0),
          PreviewGraceDays = this.previewGraceDaysColumn.GetByte((IDataReader) reader, (byte) 0, (byte) 0),
          CommittedQuantity = this.committedQuantityColumn.GetInt32((IDataReader) reader, 0),
          CurrentQuantity = this.currentQuantityColumn.GetInt32((IDataReader) reader, 0),
          IncludedQuantity = this.includedQuantityColumn.GetInt32((IDataReader) reader, 0),
          MaximumQuantity = this.maxQuantityColumn.GetInt32((IDataReader) reader, int.MaxValue),
          AbsoluteMaximumQuantity = this.absMaxQuantityColumn.GetInt32((IDataReader) reader, int.MaxValue),
          TrialCycles = this.trialCyclesColumn.GetInt32((IDataReader) reader, 0),
          BillingEntity = (BillingProvider) this.billingProviderColumn.GetByte((IDataReader) reader, (byte) 0, (byte) 0),
          MinimumRequiredAccessLevel = (MinimumRequiredServiceLevel) this.minRequiredAccessLevel.GetByte((IDataReader) reader, (byte) 0, (byte) 0),
          IncludedInLicenseLevel = (MinimumRequiredServiceLevel) this.IncludedInLicenseLevel.GetByte((IDataReader) reader, (byte) 0, (byte) 0),
          AutoAssignOnAccess = this.autoAssignOnAccess.GetBoolean((IDataReader) reader, false, false)
        };
      }
      catch (Exception ex)
      {
        this.requestContext.TraceException(5203016, OfferMeterBinder.Area, OfferMeterBinder.Layer, ex);
        throw;
      }
      finally
      {
        this.requestContext.TraceLeave(5203018, OfferMeterBinder.Area, OfferMeterBinder.Layer, nameof (GetResourceFromReader));
      }
    }

    private static ResourceBillingMode GetBillingMode(char billingCode) => char.ToUpperInvariant(billingCode) == 'C' ? ResourceBillingMode.Committment : ResourceBillingMode.PayAsYouGo;
  }
}

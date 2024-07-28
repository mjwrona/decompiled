// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceSqlComponent22
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class CommerceSqlComponent22 : CommerceSqlComponent21
  {
    public override int CleanupAzureResourceAccounts()
    {
      int num = 0;
      try
      {
        this.TraceEnter(5106500, nameof (CleanupAzureResourceAccounts));
        this.Trace(5106501, TraceLevel.Info, string.Format("{0}: calling {1}. at Utc {2}", (object) nameof (CleanupAzureResourceAccounts), (object) "prc_CleanupAzureResourceAccount", (object) DateTime.UtcNow));
        this.PrepareStoredProcedure("prc_CleanupAzureResourceAccount");
        SqlParameter sqlParameter = this.BindInt("@deletedAccountsCount", 0);
        sqlParameter.Direction = ParameterDirection.Output;
        this.ExecuteNonQuery();
        return (int) sqlParameter.Value;
      }
      catch (Exception ex)
      {
        this.ComponentRequestContext.TraceException(5106502, this.TraceArea, this.Layer, ex);
        throw;
      }
      finally
      {
        this.Trace(5106503, TraceLevel.Info, string.Format("{0} completed. Deleted {1} rows.", (object) nameof (CleanupAzureResourceAccounts), (object) num));
        this.TraceLeave(5106504, nameof (CleanupAzureResourceAccounts));
      }
    }

    public override void CreateOfferMeterDefinition(IOfferMeter meterConfig)
    {
      try
      {
        this.TraceEnter(5107273, nameof (CreateOfferMeterDefinition));
        string str1 = string.Format("{0}: calling {1}. (PlatformMeterId: {2}, Name: {3}, GalleryId: {4}, ", (object) nameof (CreateOfferMeterDefinition), (object) "prc_CreateOfferMeterDefinition", (object) meterConfig.PlatformMeterId, (object) meterConfig.Name, (object) meterConfig.GalleryId);
        string str2 = string.Format(" Category: {0}, RenewalFrequency: {1}, Scope: {2}, BillingState: {3},", (object) meterConfig.Category, (object) meterConfig.RenewalFrequency, (object) meterConfig.OfferScope, (object) meterConfig.BillingState);
        string unit = meterConfig.Unit;
        // ISSUE: variable of a boxed type
        __Boxed<OfferMeterAssignmentModel> assignmentModel = (Enum) meterConfig.AssignmentModel;
        DateTime? billingStartDate = meterConfig.BillingStartDate;
        // ISSUE: variable of a boxed type
        __Boxed<DateTime> local = (System.ValueType) (billingStartDate ?? new DateTime(2000, 1, 1));
        string str3 = string.Format(" Unit: {0}, AssignmentModel: {1}, BillingStartDate: {2},", (object) unit, (object) assignmentModel, (object) local);
        string str4 = string.Format(" TrialDays: {0}, PreviewGraceDays: {1}, BillingProvider: {2}", (object) meterConfig.TrialDays, (object) meterConfig.PreviewGraceDays, (object) meterConfig.BillingEntity);
        this.Trace(5107276, TraceLevel.Info, str1 + str2 + str3 + str4);
        this.PrepareStoredProcedure("prc_CreateOfferMeterDefinition");
        this.BindGuid("@platformMeterId", meterConfig.PlatformMeterId);
        this.BindString("@galleryId", meterConfig.GalleryId, 256, false, SqlDbType.VarChar);
        this.BindString("@name", meterConfig.Name, 256, false, SqlDbType.VarChar);
        this.BindInt("@category", (int) meterConfig.Category);
        this.BindInt("@renewalFrequency", (int) meterConfig.RenewalFrequency);
        string parameterValue = string.Empty;
        if (meterConfig.BillingMode == ResourceBillingMode.Committment)
          parameterValue = "C";
        else if (meterConfig.BillingMode == ResourceBillingMode.PayAsYouGo)
          parameterValue = "P";
        this.BindString("@billingMode", parameterValue, 1, false, SqlDbType.Char);
        this.BindInt("@scope", (int) meterConfig.OfferScope);
        this.BindInt("@billingState", (int) meterConfig.BillingState);
        this.BindString("@unit", meterConfig.Unit, 20, false, SqlDbType.VarChar);
        this.BindInt("@assignmentModel", (int) meterConfig.AssignmentModel);
        billingStartDate = meterConfig.BillingStartDate;
        this.BindDateTime("@billingStartDate", billingStartDate ?? new DateTime(2000, 1, 1));
        this.BindInt("@trialDays", (int) meterConfig.TrialDays);
        this.BindInt("@previewGraceDays", (int) meterConfig.PreviewGraceDays);
        this.BindInt("@billingProvider", (int) meterConfig.BillingEntity);
        this.BindInt("@minimumRequiredAccessLevel", (int) meterConfig.MinimumRequiredAccessLevel);
        this.BindInt("@includedInLicenseLevel", (int) meterConfig.IncludedInLicenseLevel);
        this.BindBoolean("@autoAssignOnAccess", meterConfig.AutoAssignOnAccess);
        SqlParameter sqlParameter = this.BindInt("@meterId", 0);
        sqlParameter.Direction = ParameterDirection.Output;
        this.ExecuteNonQuery();
        if (meterConfig.FixedQuantityPlans.IsNullOrEmpty<AzureOfferPlanDefinition>())
          return;
        this.CreateOfferMeterDefinitionPlan(meterConfig, (int) sqlParameter.Value);
      }
      catch (Exception ex)
      {
        this.TraceException(5107274, ex);
      }
      finally
      {
        this.Trace(5107277, TraceLevel.Info, string.Format("{0}: calling {1}. (PlatformMeterId: {2}, Name: {3})", (object) nameof (CreateOfferMeterDefinition), (object) "prc_CreateOfferMeterDefinition", (object) meterConfig.PlatformMeterId, (object) meterConfig.Name));
        this.TraceLeave(5107277, nameof (CreateOfferMeterDefinition));
      }
    }

    public override void AddOfferMeterPrice(
      IEnumerable<OfferMeterPrice> priceList,
      string meterName,
      bool isFirstParty)
    {
      try
      {
        this.TraceEnter(5108454, nameof (AddOfferMeterPrice));
        this.Trace(5108457, TraceLevel.Info, string.Format("{0}: calling {1}. (meterName: {2}; count: {3})", (object) nameof (AddOfferMeterPrice), (object) "prc_AddOfferMeterPrice", (object) priceList.FirstOrDefault<OfferMeterPrice>()?.MeterName, (object) priceList.Count<OfferMeterPrice>()));
        if (priceList.Any<OfferMeterPrice>())
          this.Trace(5108460, TraceLevel.Info, "Inserting price for regions " + string.Join(", ", priceList.Select<OfferMeterPrice, string>((System.Func<OfferMeterPrice, string>) (x => x.Region))));
        this.PrepareStoredProcedure("prc_AddOfferMeterPrice");
        this.BindString("@meterName", meterName, 200, false, SqlDbType.VarChar);
        this.BindTable("@pricePlans", "Commerce.typ_OfferMeterPriceTable", new OfferMeterPriceBinder(this.ComponentRequestContext).BindOfferMeterPriceTable(priceList));
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        this.TraceException(5108455, ex);
      }
      finally
      {
        this.Trace(5108458, TraceLevel.Info, string.Format("{0}: calling {1}. (meterName: {2} count: {3})", (object) nameof (AddOfferMeterPrice), (object) "prc_AddOfferMeterPrice", (object) priceList.FirstOrDefault<OfferMeterPrice>()?.MeterName, (object) priceList.Count<OfferMeterPrice>()));
        this.TraceLeave(5108458, nameof (AddOfferMeterPrice));
      }
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceSqlComponent11
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class CommerceSqlComponent11 : CommerceSqlComponent10
  {
    private static SqlMetaData[] typ_AzureOfferPlanTable2 = new SqlMetaData[10]
    {
      new SqlMetaData("MeterId", SqlDbType.Int),
      new SqlMetaData("PlanId", SqlDbType.NVarChar, 200L),
      new SqlMetaData("Publisher", SqlDbType.NVarChar, 200L),
      new SqlMetaData("OfferName", SqlDbType.NVarChar, 200L),
      new SqlMetaData("OfferId", SqlDbType.NVarChar, 200L),
      new SqlMetaData("PlanName", SqlDbType.NVarChar, 200L),
      new SqlMetaData("PlanVersion", SqlDbType.NVarChar, 200L),
      new SqlMetaData("Quantity", SqlDbType.Int),
      new SqlMetaData("IsPublic", SqlDbType.Bit),
      new SqlMetaData("PublisherName", SqlDbType.NVarChar, 200L)
    };

    public override void CreateOfferMeterDefinition(IOfferMeter meterConfig)
    {
      try
      {
        this.TraceEnter(5107273, nameof (CreateOfferMeterDefinition));
        this.Trace(5107276, TraceLevel.Info, string.Format("{0}: calling {1}. (MeterId: {2}; PlatformMeterId: {3}, Name: {4})", (object) nameof (CreateOfferMeterDefinition), (object) "prc_CreateOfferMeterDefinition", (object) meterConfig.MeterId, (object) meterConfig.PlatformMeterId, (object) meterConfig.Name));
        this.PrepareStoredProcedure("prc_CreateOfferMeterDefinition");
        this.BindInt("@meterId", meterConfig.MeterId);
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
        this.BindDateTime("@billingStartDate", meterConfig.BillingStartDate ?? new DateTime(2000, 1, 1));
        this.BindInt("@trialDays", (int) meterConfig.TrialDays);
        this.BindInt("@previewGraceDays", (int) meterConfig.PreviewGraceDays);
        this.BindInt("@billingProvider", (int) meterConfig.BillingEntity);
        this.BindInt("@minimumRequiredAccessLevel", (int) meterConfig.MinimumRequiredAccessLevel);
        int meterId = (int) this.ExecuteNonQuery(true);
        if (meterConfig.FixedQuantityPlans == null || !meterConfig.FixedQuantityPlans.Any<AzureOfferPlanDefinition>())
          return;
        this.CreateOfferMeterDefinitionPlan(meterConfig, meterId);
      }
      catch (Exception ex)
      {
        this.TraceException(5107274, ex);
      }
      finally
      {
        this.Trace(5107277, TraceLevel.Info, string.Format("{0}: calling {1}. (MeterId: {2}; PlatformMeterId: {3}, Name: {4})", (object) nameof (CreateOfferMeterDefinition), (object) "prc_CreateOfferMeterDefinition", (object) meterConfig.MeterId, (object) meterConfig.PlatformMeterId, (object) meterConfig.Name));
        this.TraceLeave(5107277, nameof (CreateOfferMeterDefinition));
      }
    }

    protected override SqlParameter BindAzureOfferPlanDefinitionTable(
      string parameterName,
      IEnumerable<AzureOfferPlanDefinition> azurePlans)
    {
      azurePlans = azurePlans ?? Enumerable.Empty<AzureOfferPlanDefinition>();
      return this.BindTable(parameterName, "Commerce.typ_AzureOfferPlanTable2", this.BindAzureOfferPlanRows2(azurePlans));
    }

    private IEnumerable<SqlDataRecord> BindAzureOfferPlanRows2(
      IEnumerable<AzureOfferPlanDefinition> azurePlans)
    {
      foreach (AzureOfferPlanDefinition azurePlan in azurePlans)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(CommerceSqlComponent11.typ_AzureOfferPlanTable2);
        sqlDataRecord.SetInt32(0, azurePlan.MeterId);
        sqlDataRecord.SetString(1, azurePlan.PlanId);
        sqlDataRecord.SetString(2, azurePlan.Publisher);
        sqlDataRecord.SetString(3, azurePlan.OfferName);
        sqlDataRecord.SetString(4, azurePlan.OfferId);
        sqlDataRecord.SetString(5, azurePlan.PlanName);
        sqlDataRecord.SetString(6, azurePlan.PlanVersion ?? "null");
        sqlDataRecord.SetInt32(7, azurePlan.Quantity);
        sqlDataRecord.SetBoolean(8, azurePlan.IsPublic);
        sqlDataRecord.SetString(9, azurePlan.PublisherName);
        yield return sqlDataRecord;
      }
    }
  }
}

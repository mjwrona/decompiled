// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceSqlComponent9
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Common;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class CommerceSqlComponent9 : CommerceSqlComponent8
  {
    protected override SqlCommand PrepareStoredProcedure(string storedProcedure) => base.PrepareStoredProcedure("Commerce." + storedProcedure);

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
        this.BindInt("@category", 2);
        this.BindInt("@renewalFrequency", (int) meterConfig.RenewalFrequency);
        string parameterValue = string.Empty;
        if (meterConfig.BillingMode == ResourceBillingMode.Committment)
          parameterValue = "C";
        else if (meterConfig.BillingMode == ResourceBillingMode.PayAsYouGo)
          parameterValue = "P";
        this.BindString("@billingMode", parameterValue, 1, false, SqlDbType.Char);
        this.BindInt("@scope", 1);
        this.BindInt("@billingState", (int) meterConfig.BillingState);
        this.BindString("@unit", meterConfig.Unit, 20, false, SqlDbType.VarChar);
        this.BindInt("@assignmentModel", (int) meterConfig.AssignmentModel);
        this.BindDateTime("@billingStartDate", meterConfig.BillingStartDate ?? new DateTime(2000, 1, 1));
        this.BindInt("@trialDays", (int) meterConfig.TrialDays);
        this.BindInt("@previewGraceDays", (int) meterConfig.PreviewGraceDays);
        this.BindInt("@billingProvider", (int) meterConfig.BillingEntity);
        int meterId = (int) this.ExecuteNonQuery(true);
        if (meterConfig.FixedQuantityPlans.IsNullOrEmpty<AzureOfferPlanDefinition>())
          return;
        this.CreateOfferMeterDefinitionPlan(meterConfig, meterId);
      }
      catch (Exception ex)
      {
        this.TraceException(5107274, ex);
        throw;
      }
      finally
      {
        this.Trace(5107277, TraceLevel.Info, string.Format("{0}: calling {1}. (MeterId: {2}; PlatformMeterId: {3}, Name: {4})", (object) nameof (CreateOfferMeterDefinition), (object) "prc_CreateOfferMeterDefinition", (object) meterConfig.MeterId, (object) meterConfig.PlatformMeterId, (object) meterConfig.Name));
        this.TraceLeave(5107277, nameof (CreateOfferMeterDefinition));
      }
    }

    public override void CreateOfferMeterDefinitionPlan(IOfferMeter meterConfig, int meterId)
    {
      try
      {
        this.TraceEnter(5107278, nameof (CreateOfferMeterDefinitionPlan));
        this.Trace(5107281, TraceLevel.Info, string.Format("{0}: calling {1}. (meterId: {2}; publisher: {3}, ", (object) nameof (CreateOfferMeterDefinitionPlan), (object) "prc_CreateOfferMeterDefinitionPlan", (object) meterConfig.MeterId, (object) meterConfig.FixedQuantityPlans.FirstOrDefault<AzureOfferPlanDefinition>()?.PublisherName) + string.Format("isPublic: {0}, billingEntity: {1})", (object) meterConfig.FixedQuantityPlans.FirstOrDefault<AzureOfferPlanDefinition>()?.IsPublic, (object) meterConfig.BillingEntity));
        this.PrepareStoredProcedure("prc_CreateOfferMeterDefinitionPlan");
        this.BindInt("@meterId", meterId);
        this.BindAzureOfferPlanDefinitionTable("@azurePlans", meterConfig.FixedQuantityPlans);
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        this.TraceException(5107279, ex);
        throw;
      }
      finally
      {
        this.Trace(5107282, TraceLevel.Info, string.Format("{0}: calling {1}. (MeterId: {2}; PlatformMeterId: {3}, Name: {4})", (object) nameof (CreateOfferMeterDefinitionPlan), (object) "prc_CreateOfferMeterDefinitionPlan", (object) meterConfig.MeterId, (object) meterConfig.PlatformMeterId, (object) meterConfig.Name));
        this.TraceLeave(5107282, nameof (CreateOfferMeterDefinitionPlan));
      }
    }
  }
}

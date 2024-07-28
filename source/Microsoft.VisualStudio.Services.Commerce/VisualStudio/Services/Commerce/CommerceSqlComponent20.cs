// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceSqlComponent20
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class CommerceSqlComponent20 : CommerceSqlComponent19
  {
    public override void CreateOfferMeterDefinitionPlan(IOfferMeter meterConfig, int meterId)
    {
      try
      {
        AzureOfferPlanDefinition offerPlanDefinition = meterConfig.FixedQuantityPlans.FirstOrDefault<AzureOfferPlanDefinition>();
        bool parameterValue = (object) offerPlanDefinition != null && offerPlanDefinition.IsPublic;
        this.TraceEnter(5107278, nameof (CreateOfferMeterDefinitionPlan));
        this.Trace(5107281, TraceLevel.Info, string.Format("{0}: calling {1}. (meterId: {2}; publisher: {3}, ", (object) nameof (CreateOfferMeterDefinitionPlan), (object) "prc_CreateOfferMeterDefinitionPlan", (object) meterConfig.MeterId, (object) meterConfig.FixedQuantityPlans.FirstOrDefault<AzureOfferPlanDefinition>()?.PublisherName) + string.Format("isPublic: {0}, billingEntity: {1})", (object) parameterValue, (object) meterConfig.BillingEntity));
        this.PrepareStoredProcedure("prc_CreateOfferMeterDefinitionPlan");
        this.BindInt("@meterId", meterId);
        this.BindBoolean("@isPublic", parameterValue);
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

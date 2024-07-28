// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceSqlComponent8
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class CommerceSqlComponent8 : CommerceSqlComponent7
  {
    private static SqlMetaData[] typ_AzureOfferPlanTable = new SqlMetaData[8]
    {
      new SqlMetaData("MeterId", SqlDbType.Int),
      new SqlMetaData("PlanId", SqlDbType.NVarChar, 200L),
      new SqlMetaData("Publisher", SqlDbType.NVarChar, 200L),
      new SqlMetaData("OfferName", SqlDbType.NVarChar, 200L),
      new SqlMetaData("PlanName", SqlDbType.NVarChar, 200L),
      new SqlMetaData("PlanVersion", SqlDbType.NVarChar, 200L),
      new SqlMetaData("Quantity", SqlDbType.Int),
      new SqlMetaData("IsPublic", SqlDbType.Bit)
    };

    public virtual IList<OfferMeter> GetOfferMeterResults()
    {
      using (SqlDataReader reader = this.ExecuteReader())
      {
        if (reader != null)
        {
          ResultCollection resultCollection = new ResultCollection((IDataReader) reader, this.ProcedureName, this.RequestContext);
          resultCollection.AddBinder<OfferMeter>((ObjectBinder<OfferMeter>) new OfferMeterBinder(this.RequestContext));
          resultCollection.AddBinder<AzureOfferPlanDefinition>((ObjectBinder<AzureOfferPlanDefinition>) new OfferMeterAzurePlanBinder(this.RequestContext));
          List<OfferMeter> items = resultCollection.GetCurrent<OfferMeter>().Items;
          resultCollection.NextResult();
          foreach (IGrouping<int, AzureOfferPlanDefinition> grouping1 in resultCollection.GetCurrent<AzureOfferPlanDefinition>().Items.GroupBy<AzureOfferPlanDefinition, int>((System.Func<AzureOfferPlanDefinition, int>) (x => x.MeterId)))
          {
            IGrouping<int, AzureOfferPlanDefinition> grouping = grouping1;
            OfferMeter offerMeter = items.FirstOrDefault<OfferMeter>((System.Func<OfferMeter, bool>) (x => x.MeterId == grouping.Key));
            if (offerMeter != (OfferMeter) null)
              offerMeter.FixedQuantityPlans = (IEnumerable<AzureOfferPlanDefinition>) grouping;
          }
          return (IList<OfferMeter>) items;
        }
      }
      return (IList<OfferMeter>) new List<OfferMeter>();
    }

    public override IList<OfferMeter> GetOfferMeterConfiguration(int? meterId)
    {
      try
      {
        this.TraceEnter(5108426, nameof (GetOfferMeterConfiguration));
        this.PrepareStoredProcedure("prc_GetOfferMeters");
        if (meterId.HasValue)
          this.BindInt("@meterId", meterId.Value);
        return this.GetOfferMeterResults();
      }
      catch (Exception ex)
      {
        this.TraceException(5108427, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(5108428, nameof (GetOfferMeterConfiguration));
      }
    }

    public override OfferMeter GetOfferMeterConfigurationByName(string meterName)
    {
      try
      {
        this.TraceEnter(5108426, nameof (GetOfferMeterConfigurationByName));
        this.PrepareStoredProcedure("prc_GetOfferMeterByName");
        this.BindString("@meterName", meterName, 100, false, SqlDbType.VarChar);
        return this.GetOfferMeterResults().FirstOrDefault<OfferMeter>();
      }
      catch (Exception ex)
      {
        this.TraceException(5108427, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(5108428, nameof (GetOfferMeterConfigurationByName));
      }
    }

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

    public override void CreateOfferMeterDefinitionPlan(IOfferMeter meterConfig, int meterId)
    {
      try
      {
        this.TraceEnter(5107278, nameof (CreateOfferMeterDefinitionPlan));
        this.Trace(5107281, TraceLevel.Info, string.Format("{0}: calling {1}. (meterId: {2}; publisher: {3}, ", (object) nameof (CreateOfferMeterDefinitionPlan), (object) "prc_CreateOfferMeterDefinitionPlan", (object) meterConfig.MeterId, (object) meterConfig.FixedQuantityPlans.FirstOrDefault<AzureOfferPlanDefinition>()?.Publisher) + string.Format("isPublic: {0}, billingEntity: {1})", (object) meterConfig.FixedQuantityPlans.FirstOrDefault<AzureOfferPlanDefinition>()?.IsPublic, (object) meterConfig.BillingEntity));
        this.PrepareStoredProcedure("prc_CreateOfferMeterDefinitionPlan");
        this.BindInt("@meterId", meterId);
        this.BindAzureOfferPlanDefinitionTable("@azurePlans", meterConfig.FixedQuantityPlans);
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        this.TraceException(5107279, ex);
      }
      finally
      {
        this.Trace(5107282, TraceLevel.Info, string.Format("{0}: calling {1}. (MeterId: {2}; PlatformMeterId: {3}, Name: {4})", (object) nameof (CreateOfferMeterDefinitionPlan), (object) "prc_CreateOfferMeterDefinitionPlan", (object) meterConfig.MeterId, (object) meterConfig.PlatformMeterId, (object) meterConfig.Name));
        this.TraceLeave(5107282, nameof (CreateOfferMeterDefinitionPlan));
      }
    }

    protected virtual SqlParameter BindAzureOfferPlanDefinitionTable(
      string parameterName,
      IEnumerable<AzureOfferPlanDefinition> azurePlans)
    {
      azurePlans = azurePlans ?? Enumerable.Empty<AzureOfferPlanDefinition>();
      return this.BindTable(parameterName, "typ_AzureOfferPlanTable", this.BindAzureOfferPlanRows(azurePlans));
    }

    private IEnumerable<SqlDataRecord> BindAzureOfferPlanRows(
      IEnumerable<AzureOfferPlanDefinition> azurePlans)
    {
      foreach (AzureOfferPlanDefinition azurePlan in azurePlans)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(CommerceSqlComponent8.typ_AzureOfferPlanTable);
        sqlDataRecord.SetInt32(0, azurePlan.MeterId);
        sqlDataRecord.SetString(1, azurePlan.PlanId);
        sqlDataRecord.SetString(2, azurePlan.Publisher);
        sqlDataRecord.SetString(3, azurePlan.OfferName);
        sqlDataRecord.SetString(4, azurePlan.PlanName);
        sqlDataRecord.SetString(5, azurePlan.PlanVersion ?? "null");
        sqlDataRecord.SetInt32(6, azurePlan.Quantity);
        sqlDataRecord.SetBoolean(7, azurePlan.IsPublic);
        yield return sqlDataRecord;
      }
    }
  }
}

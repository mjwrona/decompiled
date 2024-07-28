// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.OfferMeterAzurePlanBinder
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
  internal class OfferMeterAzurePlanBinder : ObjectBinder<AzureOfferPlanDefinition>
  {
    private SqlColumnBinder meterIdColumn = new SqlColumnBinder("MeterId");
    private SqlColumnBinder planIdColumn = new SqlColumnBinder("PlanId");
    private SqlColumnBinder publisherColumn = new SqlColumnBinder("Publisher");
    private SqlColumnBinder offerNameColumn = new SqlColumnBinder("OfferName");
    private SqlColumnBinder planNameColumn = new SqlColumnBinder("PlanName");
    private SqlColumnBinder quantityColumn = new SqlColumnBinder("Quantity");
    private SqlColumnBinder planVersionColumn = new SqlColumnBinder("PlanVersion");
    private SqlColumnBinder isPublicColumn = new SqlColumnBinder("IsPublic");
    private SqlColumnBinder publisherNameColumn = new SqlColumnBinder("PublisherName");
    private SqlColumnBinder offerIdColumn = new SqlColumnBinder("OfferId");
    private IVssRequestContext requestContext;
    private const string Area = "Commerce";
    private const string Layer = "OfferMeterAzurePlanBinder";

    public OfferMeterAzurePlanBinder(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(5107230, "Commerce", nameof (OfferMeterAzurePlanBinder), ".ctor");
      this.requestContext = requestContext;
      this.requestContext.TraceLeave(5107239, "Commerce", nameof (OfferMeterAzurePlanBinder), ".ctor");
    }

    [ExcludeFromCodeCoverage]
    protected override AzureOfferPlanDefinition Bind() => this.GetResourceFromReader((DbDataReader) this.Reader);

    internal AzureOfferPlanDefinition GetResourceFromReader(DbDataReader reader)
    {
      try
      {
        this.requestContext.TraceEnter(5107240, "Commerce", nameof (OfferMeterAzurePlanBinder), nameof (GetResourceFromReader));
        return new AzureOfferPlanDefinition()
        {
          MeterId = this.meterIdColumn.GetInt32((IDataReader) reader),
          PlanId = this.planIdColumn.GetString((IDataReader) reader, false),
          Publisher = this.publisherColumn.GetString((IDataReader) reader, false),
          OfferName = this.offerNameColumn.GetString((IDataReader) reader, false),
          PlanName = this.planNameColumn.GetString((IDataReader) reader, false),
          PlanVersion = this.planVersionColumn.GetString((IDataReader) reader, true),
          Quantity = this.quantityColumn.GetInt32((IDataReader) reader),
          IsPublic = this.isPublicColumn.GetBoolean((IDataReader) reader),
          OfferId = this.offerIdColumn.GetString((IDataReader) reader, string.Empty),
          PublisherName = this.publisherNameColumn.GetString((IDataReader) reader, string.Empty)
        };
      }
      catch (Exception ex)
      {
        this.requestContext.TraceException(5107248, "Commerce", nameof (OfferMeterAzurePlanBinder), ex);
        throw;
      }
      finally
      {
        this.requestContext.TraceLeave(5107249, "Commerce", nameof (OfferMeterAzurePlanBinder), nameof (GetResourceFromReader));
      }
    }
  }
}

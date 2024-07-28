// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.OfferSubscriptionQuantityBinder
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class OfferSubscriptionQuantityBinder : ObjectBinder<OfferSubscriptionQuantity>
  {
    private SqlColumnBinder azureSubscriptionIdColumn = new SqlColumnBinder("AzureSubscriptionId");
    private SqlColumnBinder meterIdColumn = new SqlColumnBinder("MeterId");
    private SqlColumnBinder includedQuantityColumn = new SqlColumnBinder("IncludedQuantity");
    private SqlColumnBinder lastUpdatedColumn = new SqlColumnBinder("LastUpdated");
    private SqlColumnBinder lastUpdatedByColumn = new SqlColumnBinder("LastUpdatedBy");
    private IVssRequestContext requestContext;
    private const string Area = "Commerce";
    private const string Layer = "OfferSubscriptionQuantityBinder";

    public OfferSubscriptionQuantityBinder(IVssRequestContext requestContext)
    {
      if (requestContext == null)
        return;
      this.requestContext = requestContext;
      this.requestContext.Trace(5203000, TraceLevel.Info, "Commerce", nameof (OfferSubscriptionQuantityBinder), "OfferSubscriptionQuantityBinder.ctor");
    }

    [ExcludeFromCodeCoverage]
    protected override OfferSubscriptionQuantity Bind() => this.GetOfferSubscriptionQuantityFromReader((DbDataReader) this.Reader);

    internal OfferSubscriptionQuantity GetOfferSubscriptionQuantityFromReader(DbDataReader reader)
    {
      try
      {
        if (this.requestContext != null)
          this.requestContext.TraceEnter(5203004, "Commerce", nameof (OfferSubscriptionQuantityBinder), nameof (GetOfferSubscriptionQuantityFromReader));
        return new OfferSubscriptionQuantity()
        {
          AzureSubscriptionId = this.azureSubscriptionIdColumn.GetGuid((IDataReader) reader, false),
          MeterId = this.meterIdColumn.GetInt32((IDataReader) reader),
          IncludedQuantity = this.includedQuantityColumn.GetInt32((IDataReader) reader),
          LastUpdated = this.lastUpdatedColumn.GetDateTime((IDataReader) reader),
          LastUpdatedBy = this.lastUpdatedByColumn.GetGuid((IDataReader) reader, false)
        };
      }
      catch (Exception ex)
      {
        if (this.requestContext != null)
          this.requestContext.TraceException(5203006, "Commerce", nameof (OfferSubscriptionQuantityBinder), ex);
        throw;
      }
      finally
      {
        if (this.requestContext != null)
          this.requestContext.TraceLeave(5203008, "Commerce", nameof (OfferSubscriptionQuantityBinder), nameof (GetOfferSubscriptionQuantityFromReader));
      }
    }
  }
}

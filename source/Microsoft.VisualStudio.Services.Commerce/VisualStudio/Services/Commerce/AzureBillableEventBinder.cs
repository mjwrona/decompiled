// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.AzureBillableEventBinder
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
  internal class AzureBillableEventBinder : ObjectBinder<AzureBillableEvent>
  {
    private SqlColumnBinder rowKeyColumn = new SqlColumnBinder("RowKey");
    private SqlColumnBinder subsGuidColumn = new SqlColumnBinder("SubscriptionGuid");
    private SqlColumnBinder quantityColumn = new SqlColumnBinder("Quantity");
    private SqlColumnBinder meterGuidColumn = new SqlColumnBinder("MeterGuid");
    private IVssRequestContext requestContext;
    private const string Area = "Commerce";
    private const string Layer = "AzureBillableEventBinder";

    public AzureBillableEventBinder(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(5203020, "Commerce", nameof (AzureBillableEventBinder), ".ctor");
      this.requestContext = requestContext;
      this.requestContext.TraceLeave(5203022, "Commerce", nameof (AzureBillableEventBinder), ".ctor");
    }

    [ExcludeFromCodeCoverage]
    protected override AzureBillableEvent Bind() => this.GetEventFromReader((DbDataReader) this.Reader);

    internal AzureBillableEvent GetEventFromReader(DbDataReader reader)
    {
      try
      {
        this.requestContext.TraceEnter(5203024, "Commerce", nameof (AzureBillableEventBinder), nameof (GetEventFromReader));
        return new AzureBillableEvent(DateTime.UtcNow, this.rowKeyColumn.GetString((IDataReader) reader, false))
        {
          SubscriptionId = this.subsGuidColumn.GetGuid((IDataReader) reader, true, Guid.Empty).ToString(),
          UsageResourceQuantity = this.quantityColumn.GetDouble((IDataReader) reader),
          ResourceGuid = this.meterGuidColumn.GetGuid((IDataReader) reader).ToString(),
          AccountId = this.requestContext.ServiceHost.InstanceId
        };
      }
      catch (Exception ex)
      {
        this.requestContext.TraceException(5203026, "Commerce", nameof (AzureBillableEventBinder), ex);
        throw;
      }
      finally
      {
        this.requestContext.TraceLeave(5203028, "Commerce", nameof (AzureBillableEventBinder), nameof (GetEventFromReader));
      }
    }
  }
}

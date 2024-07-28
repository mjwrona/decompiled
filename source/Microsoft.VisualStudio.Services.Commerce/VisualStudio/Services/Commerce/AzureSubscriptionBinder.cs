// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.AzureSubscriptionBinder
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
  internal class AzureSubscriptionBinder : ObjectBinder<AzureSubscriptionInternal>
  {
    private SqlColumnBinder azureSubscriptionIdColumn = new SqlColumnBinder("AzureSubscriptionId");
    private SqlColumnBinder azureSubscriptionStatusIdColumn = new SqlColumnBinder("AzureSubscriptionStatusId");
    private SqlColumnBinder azureSubscriptionNamespaceIdColumn = new SqlColumnBinder("ProviderNamespaceId");
    private SqlColumnBinder azureSubscriptionAnniversaryDayColumn = new SqlColumnBinder("AzureSubscriptionAnniversaryDay");
    private SqlColumnBinder azureSubscriptionSourceColumn = new SqlColumnBinder("SubscriptionSource");
    private SqlColumnBinder azureOfferCodeColumn = new SqlColumnBinder("AzureOfferCode");
    private SqlColumnBinder createdColumn = new SqlColumnBinder("Created");
    private SqlColumnBinder lastUpdatedColumn = new SqlColumnBinder("LastUpdated");
    private IVssRequestContext requestContext;
    private const string Area = "Commerce";
    private const string Layer = "AzureSubscriptionBinder";

    public AzureSubscriptionBinder(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(5203000, "Commerce", nameof (AzureSubscriptionBinder), ".ctor");
      this.requestContext = requestContext;
      this.requestContext.TraceLeave(5203002, "Commerce", nameof (AzureSubscriptionBinder), ".ctor");
    }

    [ExcludeFromCodeCoverage]
    protected override AzureSubscriptionInternal Bind() => this.GetAzureSubscriptionFromReader((DbDataReader) this.Reader);

    internal AzureSubscriptionInternal GetAzureSubscriptionFromReader(DbDataReader reader)
    {
      try
      {
        this.requestContext.TraceEnter(5203004, "Commerce", nameof (AzureSubscriptionBinder), nameof (GetAzureSubscriptionFromReader));
        return new AzureSubscriptionInternal()
        {
          AzureSubscriptionId = this.azureSubscriptionIdColumn.GetGuid((IDataReader) reader, false),
          AzureSubscriptionStatusId = (SubscriptionStatus) this.azureSubscriptionStatusIdColumn.GetInt16((IDataReader) reader),
          AzureSubscriptionSource = (SubscriptionSource) this.azureSubscriptionSourceColumn.GetInt32((IDataReader) reader),
          AzureOfferCode = this.azureOfferCodeColumn.GetString((IDataReader) reader, (string) null),
          Created = this.createdColumn.GetDateTime((IDataReader) reader),
          LastUpdated = this.lastUpdatedColumn.GetDateTime((IDataReader) reader)
        };
      }
      catch (Exception ex)
      {
        this.requestContext.TraceException(5203006, "Commerce", nameof (AzureSubscriptionBinder), ex);
        throw;
      }
      finally
      {
        this.requestContext.TraceLeave(5203008, "Commerce", nameof (AzureSubscriptionBinder), nameof (GetAzureSubscriptionFromReader));
      }
    }
  }
}

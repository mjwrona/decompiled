// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.AzureSubscriptionAccountBinder
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.WindowsAzure.CloudServiceManagement.ResourceProviderCommunication;
using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class AzureSubscriptionAccountBinder : ObjectBinder<AzureSubscriptionAccount>
  {
    private SqlColumnBinder accountIdColumn = new SqlColumnBinder("AccountId");
    private SqlColumnBinder subscriptionIdColumn = new SqlColumnBinder("SubscriptionId");
    private SqlColumnBinder subscriptionStatusIdColumn = new SqlColumnBinder("SubscriptionStatusId");
    private SqlColumnBinder resourceGroupNameColumn = new SqlColumnBinder("ResourceGroupName");
    private SqlColumnBinder geoLocationColumn = new SqlColumnBinder("GeoLocation");
    private SqlColumnBinder resourceNameColumn = new SqlColumnBinder("ResourceName");
    private SqlColumnBinder collectionIdColumn = new SqlColumnBinder("CollectionId");
    private SqlColumnBinder operationResultColumn = new SqlColumnBinder("OperationResult");
    private SqlColumnBinder azureOfferCodeColumn = new SqlColumnBinder("AzureOfferCode");
    private IVssRequestContext requestContext;
    private const string areaName = "Commerce";
    private const string layerName = "AzureSubscriptionAccountBinder";

    public AzureSubscriptionAccountBinder(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(5106151, "Commerce", nameof (AzureSubscriptionAccountBinder), ".ctor");
      this.requestContext = requestContext;
      this.requestContext.TraceLeave(5106152, "Commerce", nameof (AzureSubscriptionAccountBinder), ".ctor");
    }

    [ExcludeFromCodeCoverage]
    protected override AzureSubscriptionAccount Bind() => this.GetAzureSubscriptionAccountFromReader((DbDataReader) this.Reader);

    internal AzureSubscriptionAccount GetAzureSubscriptionAccountFromReader(DbDataReader reader)
    {
      try
      {
        this.requestContext.TraceEnter(5106153, "Commerce", nameof (AzureSubscriptionAccountBinder), nameof (GetAzureSubscriptionAccountFromReader));
        Guid? nullable = new Guid?(this.subscriptionIdColumn.GetGuid((IDataReader) reader, true));
        AzureQuotaId.Mapping mapping;
        bool flag = AzureQuotaId.TryMap(this.azureOfferCodeColumn.GetString((IDataReader) reader, (string) null), out mapping);
        return new AzureSubscriptionAccount()
        {
          AccountId = this.accountIdColumn.GetGuid((IDataReader) reader, false),
          SubscriptionId = Guid.Empty.Equals((object) nullable) ? new Guid?() : nullable,
          SubscriptionStatusId = (SubscriptionStatus) this.subscriptionStatusIdColumn.GetInt16((IDataReader) reader, (short) 0),
          ResourceGroupName = this.resourceGroupNameColumn.GetString((IDataReader) reader, true),
          SubscriptionOfferType = flag ? new AzureOfferType?(mapping.OfferType) : new AzureOfferType?(),
          GeoLocation = this.geoLocationColumn.GetString((IDataReader) reader, true),
          OperationResult = (OperationResult) this.operationResultColumn.GetInt32((IDataReader) reader, -1, 1),
          ResourceName = this.resourceNameColumn.GetString((IDataReader) reader, true),
          CollectionId = this.collectionIdColumn.GetGuid((IDataReader) reader, true, new Guid())
        };
      }
      catch (Exception ex)
      {
        this.requestContext.TraceException(5106154, "Commerce", nameof (AzureSubscriptionAccountBinder), ex);
        throw;
      }
      finally
      {
        this.requestContext.TraceLeave(5106155, "Commerce", nameof (AzureSubscriptionAccountBinder), nameof (GetAzureSubscriptionAccountFromReader));
      }
    }
  }
}

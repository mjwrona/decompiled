// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.AzureResourceAccountBinder
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.WindowsAzure.CloudServiceManagement.ResourceProviderCommunication;
using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class AzureResourceAccountBinder : ObjectBinder<AzureResourceAccount>
  {
    private SqlColumnBinder accountIdColumn = new SqlColumnBinder("AccountId");
    private SqlColumnBinder collectionIdColumn = new SqlColumnBinder("CollectionId");
    private SqlColumnBinder azureSubscriptionIdColumn = new SqlColumnBinder("AzureSubscriptionId");
    private SqlColumnBinder providerNamespaceIdColumn = new SqlColumnBinder("ProviderNamespaceId");
    private SqlColumnBinder azureCloudServiceNameColumn = new SqlColumnBinder("AzureCloudServiceName");
    private SqlColumnBinder alternateCloudServiceNameColumn = new SqlColumnBinder("AlternateCloudServiceName");
    private SqlColumnBinder azureResourceNameColumn = new SqlColumnBinder("AzureResourceName");
    private SqlColumnBinder azureGeoRegionColumn = new SqlColumnBinder("AzureGeoRegion");
    private SqlColumnBinder ETagColumn = new SqlColumnBinder("ETag");
    private SqlColumnBinder OperationStatusColumn = new SqlColumnBinder("OperationStatus");
    private SqlColumnBinder createdColumn = new SqlColumnBinder("Created");
    private SqlColumnBinder lastUpdatedColumn = new SqlColumnBinder("LastUpdated");
    private IVssRequestContext requestContext;
    private const string Area = "Commerce";
    private const string Layer = "AzureResourceAccountBinder";

    public AzureResourceAccountBinder(IVssRequestContext requestContext)
    {
      if (requestContext == null)
        return;
      this.requestContext = requestContext;
      this.requestContext.Trace(5203000, TraceLevel.Info, "Commerce", nameof (AzureResourceAccountBinder), "AzureResourceBinder.ctor");
    }

    [ExcludeFromCodeCoverage]
    protected override AzureResourceAccount Bind() => this.GetResourceAccountFromReader((DbDataReader) this.Reader);

    internal AzureResourceAccount GetResourceAccountFromReader(DbDataReader reader)
    {
      try
      {
        if (this.requestContext != null)
          this.requestContext.TraceEnter(5203004, "Commerce", nameof (AzureResourceAccountBinder), nameof (GetResourceAccountFromReader));
        return new AzureResourceAccount()
        {
          AccountId = this.accountIdColumn.GetGuid((IDataReader) reader, true),
          AzureSubscriptionId = this.azureSubscriptionIdColumn.GetGuid((IDataReader) reader, false),
          ProviderNamespaceId = (AccountProviderNamespace) this.providerNamespaceIdColumn.GetInt16((IDataReader) reader),
          AzureCloudServiceName = this.azureCloudServiceNameColumn.GetString((IDataReader) reader, false),
          AzureResourceName = this.azureResourceNameColumn.GetString((IDataReader) reader, false),
          AzureGeoRegion = this.azureGeoRegionColumn.GetString((IDataReader) reader, false),
          ETag = this.ETagColumn.GetString((IDataReader) reader, false),
          OperationResult = (OperationResult) this.OperationStatusColumn.GetInt32((IDataReader) reader),
          Created = this.createdColumn.GetDateTime((IDataReader) reader),
          LastUpdated = this.lastUpdatedColumn.GetDateTime((IDataReader) reader),
          CollectionId = this.collectionIdColumn.GetGuid((IDataReader) reader, true, Guid.Empty),
          AlternateCloudServiceName = this.alternateCloudServiceNameColumn.GetString((IDataReader) reader, true)
        };
      }
      catch (Exception ex)
      {
        if (this.requestContext != null)
          this.requestContext.TraceException(5203006, "Commerce", nameof (AzureResourceAccountBinder), ex);
        throw;
      }
      finally
      {
        if (this.requestContext != null)
          this.requestContext.TraceLeave(5203008, "Commerce", nameof (AzureResourceAccountBinder), nameof (GetResourceAccountFromReader));
      }
    }
  }
}

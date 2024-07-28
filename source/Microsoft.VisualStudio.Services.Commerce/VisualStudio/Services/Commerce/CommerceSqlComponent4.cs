// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceSqlComponent4
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.WindowsAzure.CloudServiceManagement.ResourceProviderCommunication;
using System;
using System.Data;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class CommerceSqlComponent4 : CommerceSqlComponent3
  {
    public override void AddAzureResourceAccount(
      Guid accountId,
      Guid collectionId,
      Guid azureSubscriptionId,
      AccountProviderNamespace providerNamespaceId,
      string azureCloudServiceName,
      string alternateCloudServiceName,
      string azureResourceName,
      string eTag,
      string azureGeoRegion,
      OperationResult operationResult)
    {
      try
      {
        this.TraceEnter(5105471, nameof (AddAzureResourceAccount));
        this.Trace(5105472, TraceLevel.Info, string.Format("{0}: calling {1}. (azureSubscriptionId: {2}; resourceName: {3}, accountId: {4})", (object) nameof (AddAzureResourceAccount), (object) "prc_AddAzureResourceAccount", (object) azureSubscriptionId, (object) azureResourceName, (object) accountId));
        this.PrepareStoredProcedure("prc_AddAzureResourceAccount");
        this.BindGuid("@azureSubscriptionId", azureSubscriptionId);
        this.BindInt("@providerNamespaceId", (int) providerNamespaceId);
        this.BindGuid("@accountId", accountId);
        this.BindString("@azureCloudServiceName", azureCloudServiceName, 256, false, SqlDbType.VarChar);
        this.BindString("@alternateCloudServiceName", alternateCloudServiceName, 256, true, SqlDbType.VarChar);
        this.BindString("@azureResourceName", azureResourceName, 256, false, SqlDbType.VarChar);
        this.BindString("@azureGeoRegion", azureGeoRegion, 256, false, SqlDbType.VarChar);
        this.BindString("@eTag", eTag, 256, false, SqlDbType.VarChar);
        this.BindDateTime("@created", DateTime.UtcNow);
        this.BindDateTime("@lastUpdated", DateTime.UtcNow);
        this.BindInt("@operationResult", (int) operationResult);
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        this.TraceException(5105478, ex);
        throw;
      }
      finally
      {
        this.Trace(5105479, TraceLevel.Info, string.Format("{0}: Complete. azureSubscriptionId: {1} accountId: {2}", (object) nameof (AddAzureResourceAccount), (object) azureSubscriptionId, (object) accountId));
        this.TraceLeave(5105480, nameof (AddAzureResourceAccount));
      }
    }
  }
}

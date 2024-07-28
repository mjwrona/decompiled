// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceSqlComponent15
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System;
using System.Data;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class CommerceSqlComponent15 : CommerceSqlComponent14
  {
    public override void UpdateAzureResourceAccount(
      Guid accountId,
      Guid collectionId,
      Guid azureSubscriptionId,
      AccountProviderNamespace providerNamespaceId,
      string azureCloudServiceName,
      string azureResourceName)
    {
      try
      {
        this.TraceEnter(CommerceTracePoints.SqlComponentUpdateAzureResourceAccountEnter, nameof (UpdateAzureResourceAccount));
        this.Trace(CommerceTracePoints.SqlComponentUpdateAzureResourceAccountPrcCall, TraceLevel.Info, string.Format("{0}: calling {1}. (azureSubscriptionId: {2}; resourceName: {3}, accountId: {4})", (object) nameof (UpdateAzureResourceAccount), (object) "prc_UpdateAzureResourceAccount", (object) azureSubscriptionId, (object) azureResourceName, (object) collectionId));
        this.PrepareStoredProcedure("prc_UpdateAzureResourceAccount");
        this.BindGuid("@accountId", accountId);
        this.BindGuid("@azureSubscriptionId", azureSubscriptionId);
        this.BindInt("@providerNamespaceId", (int) providerNamespaceId);
        this.BindGuid("@collectionId", collectionId);
        this.BindString("@azureCloudServiceName", azureCloudServiceName, 256, false, SqlDbType.VarChar);
        this.BindString("@azureResourceName", azureResourceName, 256, false, SqlDbType.VarChar);
        this.BindDateTime("@lastUpdated", DateTime.UtcNow);
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        this.TraceException(CommerceTracePoints.SqlComponentUpdateAzureResourceAccountException, ex);
        throw;
      }
      finally
      {
        this.Trace(CommerceTracePoints.SqlComponentUpdateAzureResourceAccountComplete, TraceLevel.Info, string.Format("{0}: Complete. azureSubscriptionId: {1} collectionId: {2}", (object) nameof (UpdateAzureResourceAccount), (object) azureSubscriptionId, (object) collectionId));
        this.TraceLeave(CommerceTracePoints.SqlComponentUpdateAzureResourceAccountLeave, nameof (UpdateAzureResourceAccount));
      }
    }
  }
}

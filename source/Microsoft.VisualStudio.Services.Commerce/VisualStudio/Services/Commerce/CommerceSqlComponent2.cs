// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceSqlComponent2
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.WindowsAzure.CloudServiceManagement.ResourceProviderCommunication;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class CommerceSqlComponent2 : CommerceSqlComponent
  {
    public virtual T GetResult<T>(ObjectBinder<T> binder)
    {
      using (SqlDataReader reader = this.ExecuteReader())
      {
        if (reader == null)
          return default (T);
        ResultCollection resultCollection = new ResultCollection((IDataReader) reader, this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<T>(binder);
        return resultCollection.GetCurrent<T>().FirstOrDefault<T>();
      }
    }

    public virtual IEnumerable<T> GetResults<T>(ObjectBinder<T> binder)
    {
      using (SqlDataReader reader = this.ExecuteReader())
      {
        if (reader == null)
          return (IEnumerable<T>) null;
        ResultCollection resultCollection = new ResultCollection((IDataReader) reader, this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<T>(binder);
        return (IEnumerable<T>) resultCollection.GetCurrent<T>().Items;
      }
    }

    public override AzureSubscriptionInternal GetAzureSubscription(
      Guid azureSubscriptionId,
      AccountProviderNamespace providerNamespaceId)
    {
      try
      {
        this.ComponentRequestContext.TraceEnter(5105401, this.TraceArea, this.Layer, nameof (GetAzureSubscription));
        this.ComponentRequestContext.Trace(5105402, TraceLevel.Info, this.TraceArea, this.Layer, string.Format("{0}: calling {1}. (azureSubscriptionId: {2}, provider namespace: {3})", (object) nameof (GetAzureSubscription), (object) "prc_GetAzureSubscription", (object) azureSubscriptionId, (object) providerNamespaceId));
        this.PrepareStoredProcedure("prc_GetAzureSubscription");
        this.BindGuid("@azureSubscriptionId", azureSubscriptionId);
        this.BindInt("@providerNamespaceId", (int) providerNamespaceId);
        return this.GetResult<AzureSubscriptionInternal>((ObjectBinder<AzureSubscriptionInternal>) new AzureSubscriptionBinder(this.ComponentRequestContext));
      }
      catch (Exception ex)
      {
        this.ComponentRequestContext.TraceException(5105408, this.TraceArea, this.Layer, ex);
        throw;
      }
      finally
      {
        this.ComponentRequestContext.Trace(5105409, TraceLevel.Info, this.TraceArea, this.Layer, string.Format("{0}: Complete. azureSubscriptionId: {1}", (object) nameof (GetAzureSubscription), (object) azureSubscriptionId));
        this.ComponentRequestContext.TraceLeave(5105410, this.TraceArea, this.Layer, nameof (GetAzureSubscription));
      }
    }

    public override AzureSubscriptionInternal GetAzureSubscription(Guid azureSubscriptionId) => this.GetAzureSubscription(azureSubscriptionId, AccountProviderNamespace.Marketplace);

    public override void AddAzureSubscription(
      Guid azureSubscriptionId,
      AccountProviderNamespace providerNamespaceId,
      int azureSubscriptionAnniversaryDay,
      SubscriptionStatus subscriptionStatusId,
      SubscriptionSource subscriptionSource)
    {
      try
      {
        this.ComponentRequestContext.TraceEnter(5105411, this.TraceArea, this.Layer, nameof (AddAzureSubscription));
        this.ComponentRequestContext.Trace(5105412, TraceLevel.Info, this.TraceArea, this.Layer, string.Format("{0}: calling {1}. azureSubscriptionId: {2}", (object) nameof (AddAzureSubscription), (object) "prc_AddAzureSubscription", (object) azureSubscriptionId));
        this.PrepareStoredProcedure("prc_AddAzureSubscription");
        this.BindGuid("@azureSubscriptionId", azureSubscriptionId);
        this.BindInt("@providerNamespaceId", (int) providerNamespaceId);
        this.BindInt("@azureSubscriptionStatusId", (int) subscriptionStatusId);
        this.BindInt("@azureSubscriptionAnniversaryDay", azureSubscriptionAnniversaryDay);
        this.BindInt("@subscriptionSource", (int) subscriptionSource);
        this.BindDateTime("@created", DateTime.UtcNow);
        this.BindDateTime("@lastUpdated", DateTime.UtcNow);
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        this.ComponentRequestContext.TraceException(5105418, this.TraceArea, this.Layer, ex);
        throw;
      }
      finally
      {
        this.ComponentRequestContext.Trace(5105419, TraceLevel.Info, this.TraceArea, this.Layer, string.Format("{0}: Complete. azureSubscriptionId: {1}", (object) nameof (AddAzureSubscription), (object) azureSubscriptionId));
        this.ComponentRequestContext.TraceLeave(5105420, this.TraceArea, this.Layer, nameof (AddAzureSubscription));
      }
    }

    public override void AddAzureSubscription(
      Guid azureSubscriptionId,
      SubscriptionStatus subscriptionStatusId,
      SubscriptionSource subscriptionSource)
    {
      this.AddAzureSubscription(azureSubscriptionId, AccountProviderNamespace.Marketplace, 1, subscriptionStatusId, subscriptionSource);
    }

    public override int GetAzureResourceAccountsCountBySubscriptionId(
      Guid azureSubscriptionId,
      AccountProviderNamespace providerNamespaceId)
    {
      try
      {
        this.ComponentRequestContext.TraceEnter(5105431, this.TraceArea, this.Layer, nameof (GetAzureResourceAccountsCountBySubscriptionId));
        this.ComponentRequestContext.Trace(5105432, TraceLevel.Info, this.TraceArea, this.Layer, string.Format("{0}: calling {1}. (azureSubscriptionId: {2})", (object) nameof (GetAzureResourceAccountsCountBySubscriptionId), (object) "prc_GetAzureResourceAccountCount", (object) azureSubscriptionId));
        this.PrepareStoredProcedure("prc_GetAzureResourceAccountCount");
        this.BindGuid("@azureSubscriptionId", azureSubscriptionId);
        this.BindInt("@providerNamespaceId", (int) providerNamespaceId);
        return this.GetResult<int>((ObjectBinder<int>) new SimpleObjectBinder<int>((System.Func<IDataReader, int>) (r => new SqlColumnBinder("AzureResourceAccountCount").GetInt32(r))));
      }
      catch (Exception ex)
      {
        this.ComponentRequestContext.TraceException(5105438, this.TraceArea, this.Layer, ex);
        throw;
      }
      finally
      {
        this.ComponentRequestContext.Trace(5105439, TraceLevel.Info, this.TraceArea, this.Layer, string.Format("{0}: Complete. azureSubscriptionId: {1}", (object) nameof (GetAzureResourceAccountsCountBySubscriptionId), (object) azureSubscriptionId));
        this.ComponentRequestContext.TraceLeave(5105440, this.TraceArea, this.Layer, nameof (GetAzureResourceAccountsCountBySubscriptionId));
      }
    }

    public override IEnumerable<AzureResourceAccount> GetAzureResourceAccountsBySubscriptionId(
      Guid azureSubscriptionId,
      AccountProviderNamespace providerNamespaceId,
      bool useCollectionId)
    {
      return (IEnumerable<AzureResourceAccount>) this.GetAzureResourceAccounts(azureSubscriptionId, providerNamespaceId, (string) null, useCollectionId).ToList<AzureResourceAccount>();
    }

    public override AzureResourceAccount GetAzureResourceAccount(
      Guid azureSubscriptionId,
      AccountProviderNamespace providerNamespaceId,
      string azureResourceName,
      bool useCollectionId)
    {
      return this.GetAzureResourceAccounts(azureSubscriptionId, providerNamespaceId, azureResourceName, useCollectionId).FirstOrDefault<AzureResourceAccount>();
    }

    internal virtual IEnumerable<AzureResourceAccount> GetAzureResourceAccounts(
      Guid azureSubscriptionId,
      AccountProviderNamespace providerNamespaceId,
      string azureResourceName,
      bool useCollectionId)
    {
      try
      {
        this.ComponentRequestContext.TraceEnter(5105461, this.TraceArea, this.Layer, nameof (GetAzureResourceAccounts));
        this.ComponentRequestContext.Trace(5105462, TraceLevel.Info, this.TraceArea, this.Layer, string.Format("{0}: calling {1}. (azureSubscriptionId: {2})", (object) nameof (GetAzureResourceAccounts), (object) "prc_GetAzureResourceAccount", (object) azureSubscriptionId));
        this.PrepareStoredProcedure("prc_GetAzureResourceAccount");
        this.BindGuid("@azureSubscriptionId", azureSubscriptionId);
        this.BindInt("@providerNamespaceId", (int) providerNamespaceId);
        if (!string.IsNullOrEmpty(azureResourceName))
          this.BindString("@azureResourceName", azureResourceName, 256, false, SqlDbType.VarChar);
        return this.GetResults<AzureResourceAccount>((ObjectBinder<AzureResourceAccount>) new AzureResourceAccountBinder(this.ComponentRequestContext)).UseCollectionHostAsAccountId(useCollectionId);
      }
      catch (Exception ex)
      {
        this.ComponentRequestContext.TraceException(5105468, this.TraceArea, this.Layer, ex);
        throw;
      }
      finally
      {
        this.ComponentRequestContext.Trace(5105469, TraceLevel.Info, this.TraceArea, this.Layer, string.Format("{0}: Complete. azureSubscriptionId: {1}, azureResourceName: {2}", (object) nameof (GetAzureResourceAccounts), (object) azureSubscriptionId, (object) azureResourceName));
        this.ComponentRequestContext.TraceLeave(5105470, this.TraceArea, this.Layer, nameof (GetAzureResourceAccounts));
      }
    }

    public override AzureResourceAccount GetAzureResourceAccountByAccountId(
      Guid accountId,
      bool useCollectionId)
    {
      try
      {
        this.ComponentRequestContext.TraceEnter(5105451, this.TraceArea, this.Layer, nameof (GetAzureResourceAccountByAccountId));
        this.ComponentRequestContext.Trace(5105452, TraceLevel.Info, this.TraceArea, this.Layer, string.Format("{0}: calling {1}. (accountId: {2})", (object) nameof (GetAzureResourceAccountByAccountId), (object) "prc_GetAzureResourceAccountByAccountId", (object) accountId));
        this.PrepareStoredProcedure("prc_GetAzureResourceAccountByAccountId");
        this.BindGuid("@accountId", accountId);
        return this.GetResult<AzureResourceAccount>((ObjectBinder<AzureResourceAccount>) new AzureResourceAccountBinder(this.ComponentRequestContext)).UseCollectionHostAsAccountId(useCollectionId);
      }
      catch (Exception ex)
      {
        this.ComponentRequestContext.TraceException(5105458, this.TraceArea, this.Layer, ex);
        throw;
      }
      finally
      {
        this.ComponentRequestContext.Trace(5105459, TraceLevel.Info, this.TraceArea, this.Layer, string.Format("{0}: Complete. accountId: {1}", (object) nameof (GetAzureResourceAccountByAccountId), (object) accountId));
        this.ComponentRequestContext.TraceLeave(5105460, this.TraceArea, this.Layer, nameof (GetAzureResourceAccountByAccountId));
      }
    }

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
        this.ComponentRequestContext.TraceEnter(5105471, this.TraceArea, this.Layer, nameof (AddAzureResourceAccount));
        this.ComponentRequestContext.Trace(5105472, TraceLevel.Info, this.TraceArea, this.Layer, string.Format("{0}: calling {1}. (azureSubscriptionId: {2}; resourceName: {3}, accountId: {4})", (object) nameof (AddAzureResourceAccount), (object) "prc_AddAzureResourceAccount", (object) azureSubscriptionId, (object) azureResourceName, (object) accountId));
        this.PrepareStoredProcedure("prc_AddAzureResourceAccount");
        this.BindGuid("@azureSubscriptionId", azureSubscriptionId);
        this.BindInt("@providerNamespaceId", (int) providerNamespaceId);
        this.BindGuid("@accountId", accountId);
        this.BindString("@azureCloudServiceName", azureCloudServiceName, 256, false, SqlDbType.VarChar);
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
        this.ComponentRequestContext.TraceException(5105478, this.TraceArea, this.Layer, ex);
        throw;
      }
      finally
      {
        this.ComponentRequestContext.Trace(5105479, TraceLevel.Info, this.TraceArea, this.Layer, string.Format("{0}: Complete. azureSubscriptionId: {1} accountId: {2}", (object) nameof (AddAzureResourceAccount), (object) azureSubscriptionId, (object) accountId));
        this.ComponentRequestContext.TraceLeave(5105480, this.TraceArea, this.Layer, nameof (AddAzureResourceAccount));
      }
    }

    public override void RemoveAzureResourceAccount(
      Guid azureSubscriptionId,
      AccountProviderNamespace providerNamespaceId,
      string azureResourceName,
      string azureCloudServiceName)
    {
      try
      {
        this.ComponentRequestContext.TraceEnter(5105491, this.TraceArea, this.Layer, nameof (RemoveAzureResourceAccount));
        this.ComponentRequestContext.Trace(5105492, TraceLevel.Info, this.TraceArea, this.Layer, string.Format("{0}: calling {1}. azureSubscriptionId: {2}, azureResourceName: {3}", (object) nameof (RemoveAzureResourceAccount), (object) "prc_RemoveAzureResourceAccount", (object) azureSubscriptionId, (object) azureResourceName));
        this.PrepareStoredProcedure("prc_RemoveAzureResourceAccount");
        this.BindGuid("@azureSubscriptionId", azureSubscriptionId);
        this.BindInt("@providerNamespaceId", (int) providerNamespaceId);
        this.BindString("@azureResourceName", azureResourceName, 256, false, SqlDbType.VarChar);
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        this.ComponentRequestContext.TraceException(5105498, this.TraceArea, this.Layer, ex);
        throw;
      }
      finally
      {
        this.ComponentRequestContext.Trace(5105499, TraceLevel.Info, this.TraceArea, this.Layer, string.Format("{0}: Complete. azureSubscriptionId: {1}, azureResourceName: {2}", (object) nameof (RemoveAzureResourceAccount), (object) azureSubscriptionId, (object) azureResourceName));
        this.ComponentRequestContext.TraceLeave(5105500, this.TraceArea, this.Layer, nameof (RemoveAzureResourceAccount));
      }
    }

    public override IEnumerable<AzureResourceAccount> RemoveCloudService(
      Guid azureSubscriptionId,
      AccountProviderNamespace providerNamespaceId,
      string azureCloudServiceName,
      bool useCollectionId)
    {
      try
      {
        this.ComponentRequestContext.TraceEnter(5105581, this.TraceArea, this.Layer, nameof (RemoveCloudService));
        this.ComponentRequestContext.Trace(5105582, TraceLevel.Info, this.TraceArea, this.Layer, string.Format("{0}: calling {1}. azureSubscriptionId: {2}, cloudServiceName: {3}", (object) nameof (RemoveCloudService), (object) "prc_RemoveCloudService", (object) azureSubscriptionId, (object) azureCloudServiceName));
        this.PrepareStoredProcedure("prc_RemoveCloudService");
        this.BindGuid("@azureSubscriptionId", azureSubscriptionId);
        this.BindInt("@providerNamespaceId", (int) providerNamespaceId);
        this.BindString("@azureCloudServiceName", azureCloudServiceName, 256, false, SqlDbType.VarChar);
        return this.GetResults<AzureResourceAccount>((ObjectBinder<AzureResourceAccount>) new AzureResourceAccountBinder(this.ComponentRequestContext)).UseCollectionHostAsAccountId(useCollectionId);
      }
      catch (Exception ex)
      {
        this.ComponentRequestContext.TraceException(5105588, this.TraceArea, this.Layer, ex);
        throw;
      }
      finally
      {
        this.ComponentRequestContext.Trace(5105589, TraceLevel.Info, this.TraceArea, this.Layer, string.Format("{0}: Complete. azureSubscriptionId: {1}, azureCloudServiceName: {2}", (object) nameof (RemoveCloudService), (object) azureSubscriptionId, (object) azureCloudServiceName));
        this.ComponentRequestContext.TraceLeave(5105590, this.TraceArea, this.Layer, nameof (RemoveCloudService));
      }
    }

    public override List<AzureSubscriptionAccount> GetSubscriptionsForAccounts(
      IEnumerable<Guid> accountIds,
      AccountProviderNamespace providerNamespaceId,
      bool useCollectionId)
    {
      try
      {
        this.ComponentRequestContext.TraceEnter(5106141, this.TraceArea, this.Layer, nameof (GetSubscriptionsForAccounts));
        this.ComponentRequestContext.Trace(5106142, TraceLevel.Info, this.TraceArea, this.Layer, string.Format("{0}: calling {1}. providerNamespaceId: {2}", (object) nameof (GetSubscriptionsForAccounts), (object) "prc_GetSubscriptionsForAccounts", (object) providerNamespaceId));
        this.PrepareStoredProcedure("prc_GetSubscriptionsForAccounts");
        this.BindGuidTable("@accountIds", accountIds);
        this.BindInt("@providerNamespaceId", (int) providerNamespaceId);
        return this.GetResults<AzureSubscriptionAccount>((ObjectBinder<AzureSubscriptionAccount>) new AzureSubscriptionAccountBinder(this.ComponentRequestContext)).UseCollectionHostAsAccountId(useCollectionId).ToList<AzureSubscriptionAccount>();
      }
      catch (Exception ex)
      {
        this.ComponentRequestContext.TraceException(5106144, this.TraceArea, this.Layer, ex);
        throw;
      }
      finally
      {
        this.ComponentRequestContext.Trace(5106145, TraceLevel.Info, this.TraceArea, this.Layer, string.Format("{0}: Complete. providerNamespaceId: {1}", (object) nameof (GetSubscriptionsForAccounts), (object) providerNamespaceId));
        this.ComponentRequestContext.TraceLeave(5106146, this.TraceArea, this.Layer, nameof (GetSubscriptionsForAccounts));
      }
    }

    public override IList<AzureResourceAccount> GetAzureSubscriptionAccounts(
      AccountProviderNamespace providerNamespaceId,
      Guid? startingAccountId,
      int maxRecordCountToReturn,
      bool useCollectionId)
    {
      try
      {
        this.ComponentRequestContext.TraceEnter(5106211, this.TraceArea, this.Layer, nameof (GetAzureSubscriptionAccounts));
        this.PrepareStoredProcedure("prc_GetAzureSubscriptionAccounts");
        this.BindInt("@providerNamespaceId", (int) (short) providerNamespaceId);
        if (startingAccountId.HasValue)
          this.BindGuid("@startingAccountId", startingAccountId.Value);
        return (IList<AzureResourceAccount>) this.GetResults<AzureResourceAccount>((ObjectBinder<AzureResourceAccount>) new AzureResourceAccountBinder(this.ComponentRequestContext)).Take<AzureResourceAccount>(maxRecordCountToReturn).UseCollectionHostAsAccountId(useCollectionId).ToList<AzureResourceAccount>();
      }
      catch (Exception ex)
      {
        this.ComponentRequestContext.TraceException(5106212, this.TraceArea, this.Layer, ex);
        throw;
      }
      finally
      {
        this.ComponentRequestContext.TraceLeave(5106213, this.TraceArea, this.Layer, nameof (GetAzureSubscriptionAccounts));
      }
    }
  }
}

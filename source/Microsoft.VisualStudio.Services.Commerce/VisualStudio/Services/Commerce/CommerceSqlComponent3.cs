// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceSqlComponent3
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class CommerceSqlComponent3 : CommerceSqlComponent2
  {
    public override IList<AzureResourceAccount> GetAzureResourceAccountsInResourceGroup(
      Guid subscriptionId,
      AccountProviderNamespace providerNamespaceId,
      string resourceGroupName,
      bool useCollectionId)
    {
      try
      {
        this.TraceEnter(5106225, nameof (GetAzureResourceAccountsInResourceGroup));
        this.Trace(5106226, TraceLevel.Info, string.Format("{0}: calling {1}. (subscriptionId:{2}, providerNamespaceId:{3}, resourceGroupName:{4})", (object) nameof (GetAzureResourceAccountsInResourceGroup), (object) "prc_GetAzureResourceAccountsInResourceGroup", (object) subscriptionId, (object) providerNamespaceId, (object) resourceGroupName));
        this.PrepareStoredProcedure("prc_GetAzureResourceAccountsInResourceGroup");
        this.BindGuid("@azureSubscriptionId", subscriptionId);
        this.BindInt("@providerNamespaceId", (int) providerNamespaceId);
        this.BindString("@resourceGroupName", resourceGroupName, 256, false, SqlDbType.VarChar);
        return (IList<AzureResourceAccount>) this.GetResults<AzureResourceAccount>((ObjectBinder<AzureResourceAccount>) new AzureResourceAccountBinder(this.ComponentRequestContext)).UseCollectionHostAsAccountId(useCollectionId).ToList<AzureResourceAccount>();
      }
      catch (Exception ex)
      {
        this.TraceException(5105468, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(5106230, nameof (GetAzureResourceAccountsInResourceGroup));
      }
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceSqlComponent6
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class CommerceSqlComponent6 : CommerceSqlComponent5
  {
    public override IList<AzureResourceAccount> GetAzureSubscriptionAccounts(
      AccountProviderNamespace providerNamespaceId,
      Guid? startingAccountId,
      int maxRecordCountToReturn,
      bool useCollectionId)
    {
      try
      {
        this.TraceEnter(5106211, nameof (GetAzureSubscriptionAccounts));
        this.PrepareStoredProcedure("prc_GetAzureSubscriptionAccounts");
        this.BindInt("@providerNamespaceId", (int) (short) providerNamespaceId);
        this.BindInt("@maxRecord", (int) (short) maxRecordCountToReturn);
        if (startingAccountId.HasValue)
          this.BindGuid("@startingAccountId", startingAccountId.Value);
        return (IList<AzureResourceAccount>) this.GetResults<AzureResourceAccount>((ObjectBinder<AzureResourceAccount>) new AzureResourceAccountBinder(this.ComponentRequestContext)).UseCollectionHostAsAccountId(useCollectionId).ToList<AzureResourceAccount>();
      }
      catch (Exception ex)
      {
        this.TraceException(5106212, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(5106213, nameof (GetAzureSubscriptionAccounts));
      }
    }
  }
}

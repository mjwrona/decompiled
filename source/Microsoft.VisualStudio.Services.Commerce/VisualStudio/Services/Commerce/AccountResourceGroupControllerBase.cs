// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.AccountResourceGroupControllerBase
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public abstract class AccountResourceGroupControllerBase : CsmControllerBase
  {
    private static readonly string ControllerName = typeof (AccountResourceGroupControllerBase).Name;

    internal override string Layer => AccountResourceGroupControllerBase.ControllerName;

    protected internal override bool ExemptFromGlobalExceptionFormatting => true;

    [HttpGet]
    [TraceDetailsFilter(5106235, 5106239)]
    [CsmControllerExceptionHandler(5106238)]
    [ClientResponseCode(HttpStatusCode.OK, "The operation succeeded. The response contains the details of the Azure DevOps Services account resources linked to the Azure subscription.", false)]
    public virtual AccountResourceListResult Accounts_ListByResourceGroup(
      Guid subscriptionId,
      string resourceGroupName)
    {
      this.CheckParameters(subscriptionId, resourceGroupName);
      this.TfsRequestContext.Trace(5106234, TraceLevel.Verbose, this.Area, this.Layer, string.Format("Get Resources for RG {0} of user CUID {1}", (object) resourceGroupName, (object) this.TfsRequestContext.GetUserCuid()));
      List<AccountResource> list = this.TfsRequestContext.GetService<PlatformSubscriptionService>().GetAccountsInResourceGroups(this.TfsRequestContext, subscriptionId, AccountProviderNamespace.VisualStudioOnline, resourceGroupName).Where<AzureResourceAccount>((Func<AzureResourceAccount, bool>) (a => string.Equals(a.AzureCloudServiceName, resourceGroupName, StringComparison.OrdinalIgnoreCase))).ToList<AzureResourceAccount>().Select<AzureResourceAccount, AccountResource>((Func<AzureResourceAccount, AccountResource>) (account => this.CreateGetResponseBody(account, nameof (account)))).ToList<AccountResource>();
      return new AccountResourceListResult()
      {
        Value = list.ToArray()
      };
    }

    internal virtual AccountResource CreateGetResponseBody(
      AzureResourceAccount azureResourceAccount,
      string resourceType)
    {
      return CsmUtilities.CreateResourceGetResponse(this.TfsRequestContext, azureResourceAccount, "Microsoft.VisualStudio", resourceType);
    }

    private void CheckParameters(Guid subscriptionId, string resourceGroupName)
    {
      ArgumentUtility.CheckForEmptyGuid(subscriptionId, nameof (subscriptionId));
      ArgumentUtility.CheckStringForNullOrEmpty(resourceGroupName, nameof (resourceGroupName));
    }

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) CommerceControllerBase.httpExceptions;
  }
}

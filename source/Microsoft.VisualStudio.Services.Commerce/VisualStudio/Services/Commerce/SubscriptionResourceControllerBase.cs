// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.SubscriptionResourceControllerBase
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public abstract class SubscriptionResourceControllerBase : CsmControllerBase
  {
    protected internal override bool ExemptFromGlobalExceptionFormatting => true;

    [HttpGet]
    [TraceDetailsFilter(5108745, 5108755)]
    [CsmControllerExceptionHandler(5108754)]
    [ClientResponseCode(HttpStatusCode.OK, "The operation succeeded. The response contains the Azure DevOps Services account resource details associated with the Azure subscription.", false)]
    public virtual CsmSubscriptionResourceListResult SubscriptionResources_List(Guid subscriptionId)
    {
      ArgumentUtility.CheckForEmptyGuid(subscriptionId, nameof (subscriptionId));
      PlatformSubscriptionService service = this.TfsRequestContext.GetService<PlatformSubscriptionService>();
      service.CheckPermission(this.TfsRequestContext, 1);
      IEnumerable<AzureResourceAccount> subscriptionIdFromDatabase = service.GetAzureResourceAccountsBySubscriptionIdFromDatabase(this.TfsRequestContext, subscriptionId, AccountProviderNamespace.VisualStudioOnline);
      return new CsmSubscriptionResourceListResult()
      {
        Value = (subscriptionIdFromDatabase != null ? subscriptionIdFromDatabase.Select<AzureResourceAccount, CsmSubscriptionResource>((Func<AzureResourceAccount, CsmSubscriptionResource>) (x => this.CreateGetResponseBody(x, "account"))).ToArray<CsmSubscriptionResource>() : (CsmSubscriptionResource[]) null) ?? Array.Empty<CsmSubscriptionResource>()
      };
    }

    internal virtual CsmSubscriptionResource CreateGetResponseBody(
      AzureResourceAccount azureResourceAccount,
      string resourceType)
    {
      AccountResource resourceGetResponse = CsmUtilities.CreateResourceGetResponse(this.TfsRequestContext, azureResourceAccount, "Microsoft.VisualStudio", resourceType);
      CsmSubscriptionResource getResponseBody = new CsmSubscriptionResource(resourceGetResponse.id, resourceGetResponse.name, resourceGetResponse.type);
      getResponseBody.location = resourceGetResponse.location;
      getResponseBody.properties = resourceGetResponse.properties;
      getResponseBody.tags = resourceGetResponse.tags;
      return getResponseBody;
    }

    internal override string Layer => nameof (SubscriptionResourceControllerBase);
  }
}

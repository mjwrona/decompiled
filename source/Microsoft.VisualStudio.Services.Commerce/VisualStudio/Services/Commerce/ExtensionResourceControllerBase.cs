// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.ExtensionResourceControllerBase
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [CaptureExternalOperationIdHeader("x-ms-correlation-request-id")]
  [SetCsmV2ResponseHeaders]
  public abstract class ExtensionResourceControllerBase : ExtensionResourceControllerInternalBase
  {
    internal override string Layer => nameof (ExtensionResourceControllerBase);

    [HttpPut]
    [TraceDetailsFilter(5107290, 5107299)]
    [CsmControllerExceptionHandler(5107298)]
    [ClientResponseCode(HttpStatusCode.OK, "The operation succeeded. The extension resource was created for the specified account.", false)]
    [ClientResponseType(typeof (ExtensionResource), null, null)]
    public virtual HttpResponseMessage Extensions_Create(
      Guid subscriptionId,
      string resourceGroupName,
      string accountResourceName,
      string extensionResourceName,
      [FromBody] ExtensionResourceRequest requestData)
    {
      this.CheckParameters(subscriptionId, resourceGroupName, "account", accountResourceName, "extension", extensionResourceName);
      ExtensionResourceRequestInternal request = new ExtensionResourceRequestInternal(requestData, subscriptionId, resourceGroupName, accountResourceName, extensionResourceName, "Microsoft.VisualStudio");
      if (string.IsNullOrEmpty(requestData.Plan?.name))
        throw new ArgumentException("Plan data not found on request.");
      return this.CreateHttpResponse((object) this.SetResourceUsage(this.TfsRequestContext, request));
    }

    [HttpPatch]
    [TraceDetailsFilter(5108730, 5108739)]
    [CsmControllerExceptionHandler(5108738)]
    [ClientResponseCode(HttpStatusCode.OK, "The operation succeeded. The extension resource was updated for the specified account.", false)]
    [ClientResponseType(typeof (ExtensionResource), null, null)]
    public virtual HttpResponseMessage Extensions_Update(
      Guid subscriptionId,
      string resourceGroupName,
      string accountResourceName,
      string extensionResourceName,
      [FromBody] ExtensionResourceRequest requestData)
    {
      this.CheckParameters(subscriptionId, resourceGroupName, "account", accountResourceName, "extension", extensionResourceName);
      ExtensionResourceRequestInternal request = new ExtensionResourceRequestInternal(requestData, subscriptionId, resourceGroupName, accountResourceName, extensionResourceName, "Microsoft.VisualStudio");
      if (!string.IsNullOrEmpty(request.Plan?.name))
        return this.CreateHttpResponse((object) this.SetResourceUsage(this.TfsRequestContext, request));
      AzureResourceAccount azureResourceAccount = this.GetAzureResourceAccount(this.TfsRequestContext, request);
      ExtensionResourcePlan planData = (ExtensionResourcePlan) null;
      CollectionHelper.WithCollectionContext(this.TfsRequestContext, azureResourceAccount.AccountId, (Action<IVssRequestContext>) (collectionContext =>
      {
        IResourceTaggingService service = collectionContext.GetService<IResourceTaggingService>();
        if (request.Tags != null)
          service.SaveTags(collectionContext, request.ResourceName, request.Tags);
        else
          request.Tags = service.GetTags(collectionContext, request.ResourceName);
        planData = this.GetPlanFromOfferMeter(collectionContext, collectionContext.ServiceHost.InstanceId, request.ResourceName);
      }), method: nameof (Extensions_Update));
      return this.CreateHttpResponse((object) this.CreateGetResponseBody(request.ResourceName, azureResourceAccount, planData, request.Tags));
    }

    [HttpDelete]
    [TraceDetailsFilter(5107310, 5107319)]
    [CsmControllerExceptionHandler(5107318)]
    [ClientResponseCode(HttpStatusCode.OK, "The operation succeeded. The extension resource has been deleted for the specified account.", false)]
    [ClientResponseType(typeof (void), null, null)]
    public virtual HttpResponseMessage Extensions_Delete(
      Guid subscriptionId,
      string resourceGroupName,
      string accountResourceName,
      string extensionResourceName)
    {
      if (this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.EnableDeleteResourceGroupFromIbiza"))
        return new HttpResponseMessage(HttpStatusCode.OK);
      this.CheckParameters(subscriptionId, resourceGroupName, "account", accountResourceName, "extension", extensionResourceName);
      ExtensionResourceRequestInternal request = new ExtensionResourceRequestInternal(subscriptionId, resourceGroupName, accountResourceName, extensionResourceName, "Microsoft.VisualStudio");
      CollectionHelper.WithCollectionContext(this.TfsRequestContext, this.GetAzureResourceAccount(this.TfsRequestContext, request).AccountId, (Action<IVssRequestContext>) (collectionContext =>
      {
        if (collectionContext.IsFeatureEnabled("VisualStudio.Services.Commerce.DisableSetResourceUsageOnExtensionDelete"))
          return;
        this.SetResourceUsage(this.TfsRequestContext, request, true);
      }), method: nameof (Extensions_Delete));
      return new HttpResponseMessage(HttpStatusCode.OK);
    }

    [HttpGet]
    [TraceDetailsFilter(5107320, 5107329)]
    [CsmControllerExceptionHandler(5107328)]
    [ClientResponseType(typeof (ExtensionResource), null, null)]
    [ClientResponseCode(HttpStatusCode.OK, "The operation succeeded. The response contains the extension resource details for the specified account.", false)]
    [ClientResponseCode(HttpStatusCode.NotFound, "The specified extension has no plans defined.", true)]
    public virtual HttpResponseMessage Extensions_Get(
      Guid subscriptionId,
      string resourceGroupName,
      string accountResourceName,
      string extensionResourceName)
    {
      this.CheckParameters(subscriptionId, resourceGroupName, "account", accountResourceName, "extension", extensionResourceName);
      ExtensionResourceRequestInternal request = new ExtensionResourceRequestInternal(subscriptionId, resourceGroupName, accountResourceName, extensionResourceName, "Microsoft.VisualStudio");
      AzureResourceAccount azureResourceAccount = this.GetAzureResourceAccount(this.TfsRequestContext, request);
      ExtensionResourcePlan plan = (ExtensionResourcePlan) null;
      Dictionary<string, string> tags = (Dictionary<string, string>) null;
      CollectionHelper.WithCollectionContext(this.TfsRequestContext, azureResourceAccount.AccountId, (Action<IVssRequestContext>) (collectionContext =>
      {
        plan = this.GetPlanFromOfferMeter(collectionContext, collectionContext.ServiceHost.InstanceId, request.ResourceName);
        tags = collectionContext.GetService<IResourceTaggingService>().GetTags(collectionContext, request.ResourceName);
      }), method: nameof (Extensions_Get));
      return plan == null ? new HttpResponseMessage(HttpStatusCode.NotFound) : this.CreateHttpResponse((object) this.CreateGetResponseBody(extensionResourceName, azureResourceAccount, plan, tags));
    }
  }
}

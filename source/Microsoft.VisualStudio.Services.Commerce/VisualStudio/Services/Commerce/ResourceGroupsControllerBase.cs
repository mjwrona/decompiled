// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.ResourceGroupsControllerBase
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public abstract class ResourceGroupsControllerBase : CsmControllerBase
  {
    private const string ValidateMoveResourcesOperationName = "validateMoveResources";
    private const string MoveResourcesOperationName = "moveResources";
    private static readonly string ControllerName;
    private static readonly IEnumerable<string> SupportedOperationNames = (IEnumerable<string>) new string[2]
    {
      "validateMoveResources",
      "moveResources"
    };
    private readonly ResourceGroupsControllerBase.IdentityFetcher getOrCreateBindPendingIdentity;

    static ResourceGroupsControllerBase() => ResourceGroupsControllerBase.ControllerName = typeof (ResourceGroupsControllerBase).Name;

    public ResourceGroupsControllerBase()
      : this(ResourceGroupsControllerBase.\u003C\u003EO.\u003C0\u003E__GetOrCreateBindPendingIdentity ?? (ResourceGroupsControllerBase.\u003C\u003EO.\u003C0\u003E__GetOrCreateBindPendingIdentity = new ResourceGroupsControllerBase.IdentityFetcher(IdentityHelper.GetOrCreateBindPendingIdentity)))
    {
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
    }

    protected ResourceGroupsControllerBase(
      ResourceGroupsControllerBase.IdentityFetcher getOrCreateBindPendingIdentity)
    {
      this.getOrCreateBindPendingIdentity = getOrCreateBindPendingIdentity;
    }

    internal override string Layer => ResourceGroupsControllerBase.ControllerName;

    protected internal override bool ExemptFromGlobalExceptionFormatting => true;

    [HttpPost]
    [TraceDetailsFilter(5109106, 5109108)]
    [CsmControllerExceptionHandler(5109107)]
    [CaptureExternalOperationIdHeader("x-ms-correlation-request-id")]
    [SetCsmV2ResponseHeaders]
    [ClientResponseType(typeof (void), null, null)]
    [ClientResponseCode(HttpStatusCode.OK, "The move operation succeeded. The specified account resource is successfully moved to the target subscription.", false)]
    [ClientResponseCode(HttpStatusCode.NotFound, "The move operation is currently not supported by the resource provider.<a href='https://go.microsoft.com/fwlink/?linkid=870050'>Learn more</a>", false)]
    [ClientResponseCode(HttpStatusCode.BadRequest, "The move operation failed. One or more input values is/are not valid.", false)]
    public virtual HttpResponseMessage MoveResources(
      Guid subscriptionId,
      string resourceGroupName,
      string operationName,
      [FromBody] ResourcesMoveRequest resourcesMoveRequest)
    {
      if (!this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.MoveAccountResource"))
        return this.Request.CreateErrorResponse(HttpStatusCode.NotFound, HostingResources.MoveNotSupported());
      ResourcesMoveRequestInternal resourcesMoveRequest1 = new ResourcesMoveRequestInternal(resourcesMoveRequest);
      try
      {
        ResourceGroupsControllerBase.ValidateInput(subscriptionId, resourceGroupName, operationName, resourcesMoveRequest1);
      }
      catch (ArgumentException ex)
      {
        this.TfsRequestContext.TraceException(5109109, this.Area, this.Layer, (Exception) ex);
        throw new ArgumentException(HostingResources.InvalidMoveRequestErrorDetails0((object) ex.Message), ex.ParamName);
      }
      resourcesMoveRequest1.SetIdentityInfo(this.Request.Headers);
      Microsoft.VisualStudio.Services.Identity.Identity identity = this.getOrCreateBindPendingIdentity(this.TfsRequestContext.Elevate(), resourcesMoveRequest1.TenantId, resourcesMoveRequest1.Email, callerName: nameof (MoveResources));
      string collectionName = resourcesMoveRequest1.GetCollectionNames().First<string>();
      using (IVssRequestContext userContext = this.TfsRequestContext.CreateUserContext(identity.Descriptor))
      {
        bool validateOnly = string.Equals(operationName, "validateMoveResources", StringComparison.OrdinalIgnoreCase);
        userContext.GetService<PlatformSubscriptionService>().MoveCollection(userContext, collectionName, subscriptionId, resourceGroupName, resourcesMoveRequest1.GetTargetSubscriptionId(), resourcesMoveRequest1.GetTargetResourceGroupName(), validateOnly);
      }
      return this.Request.CreateResponse(HttpStatusCode.OK);
    }

    private static void ValidateInput(
      Guid subscriptionId,
      string resourceGroupName,
      string operationName,
      ResourcesMoveRequestInternal resourcesMoveRequest)
    {
      ArgumentUtility.CheckForEmptyGuid(subscriptionId, nameof (subscriptionId));
      ArgumentUtility.CheckStringForNullOrEmpty(resourceGroupName, nameof (resourceGroupName));
      ResourceGroupsControllerBase.ValidateOperationName(operationName, nameof (operationName));
      resourcesMoveRequest.Validate(subscriptionId, resourceGroupName);
    }

    private static void ValidateOperationName(string operationName, string paramName)
    {
      if (!ResourceGroupsControllerBase.SupportedOperationNames.Contains<string>(operationName, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
        throw new ArgumentException(HostingResources.OperationName0InvalidOrNotSupported((object) operationName), paramName);
    }

    public delegate Microsoft.VisualStudio.Services.Identity.Identity IdentityFetcher(
      IVssRequestContext requestContext,
      string domain,
      string accountName,
      string puid = null,
      IDictionary<string, object> properties = null,
      bool forceEntityToMatchDomain = false,
      [CallerMemberName] string callerName = null);
  }
}

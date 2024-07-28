// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackagingApiControllerBase
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ValidationUtils;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Web.Http.Controllers;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  [PutReadMigrationHeaderOnRequestContext]
  [PutBlockWriteOperationsOnGETOnRequestContext]
  [DecorateResponseWithCommitLogBookmarkHeader]
  [DecorateResponseWithPackagingMigrationHeader]
  [TaskYieldOnException]
  public abstract class PackagingApiControllerBase : TfsProjectApiController
  {
    private ITracerService? _tracerService;

    protected abstract string? GetClientSessionIdFrom(HttpRequestMessage requestMessage);

    protected ITracerBlock EnterTracer([CallerMemberName] string? methodName = null)
    {
      if (this._tracerService == null)
        this._tracerService = this.TfsRequestContext.GetTracerFacade();
      return this._tracerService.Enter((object) this, methodName);
    }

    protected override void InitializeInternal(HttpControllerContext controllerContext)
    {
      base.InitializeInternal(controllerContext);
      this.TfsRequestContext.Items["Packaging.ClientSessionId"] = (object) this.GetClientSessionIdFrom(controllerContext.Request);
    }

    protected ProtocolAgnosticFeedRequest GetProtocolAgnosticFeedRequest(
      string feedNameOrId,
      IValidator<FeedCore>? validator = null)
    {
      using (this.TfsRequestContext.GetTracerFacade().Enter((object) this, nameof (GetProtocolAgnosticFeedRequest)))
        return RequestInitHelper.GetProtocolAgnosticFeedRequest(this.TfsRequestContext, RequestInitHelper.GetUserSuppliedProjectNameOrId(this.RequestContext), this.ProjectId, feedNameOrId, validator);
    }

    protected IFeedRequest GetFeedRequest(
      IProtocol protocol,
      string feedNameOrId,
      IValidator<FeedCore>? validator = null)
    {
      using (this.TfsRequestContext.GetTracerFacade().Enter((object) this, nameof (GetFeedRequest)))
        return RequestInitHelper.GetFeedRequest(this.TfsRequestContext, RequestInitHelper.GetUserSuppliedProjectNameOrId(this.RequestContext), this.ProjectId, protocol, feedNameOrId, validator);
    }
  }
}

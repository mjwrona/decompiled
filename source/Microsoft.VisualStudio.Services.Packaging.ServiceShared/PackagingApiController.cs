// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackagingApiController
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ValidationUtils;
using System.Web.Http.Controllers;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public abstract class PackagingApiController : PackagingApiControllerBase
  {
    protected virtual IFeedRequest GetFeedRequest(
      string feedNameOrId,
      IValidator<FeedCore>? validator = null)
    {
      return this.GetFeedRequest(this.GetProtocol(), feedNameOrId, validator);
    }

    protected abstract IProtocol GetProtocol();

    protected override void InitializeInternal(HttpControllerContext controllerContext)
    {
      base.InitializeInternal(controllerContext);
      this.TfsRequestContext.SetProtocolForPackagingTraces(this.GetProtocol());
    }
  }
}

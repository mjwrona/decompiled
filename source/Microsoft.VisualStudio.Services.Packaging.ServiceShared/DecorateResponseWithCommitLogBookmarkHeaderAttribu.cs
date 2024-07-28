// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.DecorateResponseWithCommitLogBookmarkHeaderAttribute
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using System.Net.Http;
using System.Web.Http.Filters;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  internal class DecorateResponseWithCommitLogBookmarkHeaderAttribute : ActionFilterAttribute
  {
    public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
    {
      if (!(actionExecutedContext?.ActionContext?.ControllerContext?.Controller is TfsApiController controller))
        return;
      IVssRequestContext tfsRequestContext = controller.TfsRequestContext;
      HttpResponseMessage response = actionExecutedContext?.Response;
      if (response?.Headers == null)
        return;
      string str = new ValueFromCacheFactory<string>("Packaging.OperationId", (ICache<string, object>) new RequestContextItemsAsCacheFacade(tfsRequestContext)).Get();
      if (str == null)
        return;
      response.Headers.Add("X-Packaging-OperationId", str);
    }
  }
}

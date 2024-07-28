// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.RequiresAnyNuGetApiKeyAttribute
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.WebApi.Exceptions;
using System.Net.Http.Headers;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class RequiresAnyNuGetApiKeyAttribute : ActionFilterAttribute
  {
    private const string NuGetApiKeyHeader = "X-NUGET-APIKEY";

    public override void OnActionExecuting(HttpActionContext actionContext)
    {
      HttpRequestHeaders headers = actionContext?.Request?.Headers;
      if (headers == null || !headers.Contains("X-NUGET-APIKEY"))
        throw new NuGetApiKeyRequiredException(Resources.Error_NuGetApiKeyRequired());
    }
  }
}

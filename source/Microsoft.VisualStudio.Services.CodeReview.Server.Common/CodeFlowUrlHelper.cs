// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.Common.CodeFlowUrlHelper
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F16CDF2D-8103-4EAE-A2A8-4FA5B1C1BE58
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using System;

namespace Microsoft.VisualStudio.Services.CodeReview.Server.Common
{
  public static class CodeFlowUrlHelper
  {
    public static string GenerateCodeFlowUrl(
      IVssRequestContext requestContext,
      string projectName,
      int reviewId)
    {
      return string.Format("codeflow://open?server={0}&review={1}.{2}&host=vso&alert=true", (object) Uri.EscapeDataString(CodeFlowUrlHelper.GetAccountBaseUrl(requestContext)), (object) projectName, (object) reviewId);
    }

    private static string GetAccountBaseUrl(IVssRequestContext requestContext)
    {
      string accountBaseUrl = requestContext.GetService<ILocationService>().GetLocationServiceUrl(requestContext, Guid.Empty, AccessMappingConstants.ClientAccessMappingMoniker);
      if (accountBaseUrl == null)
      {
        string absoluteUri = requestContext.RequestUri().AbsoluteUri;
        accountBaseUrl = absoluteUri.Substring(0, absoluteUri.IndexOf(HttpRouteCollectionExtensions.DefaultRoutePrefix));
      }
      return accountBaseUrl;
    }
  }
}

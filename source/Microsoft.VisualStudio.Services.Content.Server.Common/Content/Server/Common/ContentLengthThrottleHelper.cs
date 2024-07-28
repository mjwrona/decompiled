// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Common.ContentLengthThrottleHelper
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common.Exceptions;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.Content.Server.Common
{
  public static class ContentLengthThrottleHelper
  {
    public const int DefaultThrottleWhenContentLengthAbove = 4096;

    public static void AssertMaxRequestContentLength(
      IVssRequestContext requestContext,
      string maxRequestContentLengthRegistryPath,
      long length,
      bool measuredByContentLengthHeader,
      long defaultMaxRequestContentLength)
    {
      long num = requestContext.GetService<IVssRegistryService>().GetValue<long>(requestContext, (RegistryQuery) maxRequestContentLengthRegistryPath, false, defaultMaxRequestContentLength);
      if (length > num)
        throw new MaxRequestContentLengthException(string.Format("The request content size is too long {0}: {1}, ", measuredByContentLengthHeader ? (object) "per Content-Length header" : (object) "after deserialization", (object) length) + string.Format("(should be less than {0}).", (object) num));
    }

    public static bool IsThrottled(
      IVssRequestContext requestContext,
      HttpRequestMessage request,
      string registryRootPath,
      string controllerLabel,
      int defaultContentLengthThreshold = 4096)
    {
      int throttlingParameter = ThrottleHelper.GetThrottlingParameter<int>(requestContext, registryRootPath, ThrottleHelper.RegistryKey.ContentLengthUpperThreshold, controllerLabel, defaultContentLengthThreshold);
      return ((long?) request?.Content?.Headers?.ContentLength ?? (long) int.MaxValue) > (long) throttlingParameter;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.ImageResizeUtils
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public class ImageResizeUtils : DefaultImageResizeUtility
  {
    private const string c_layer = "ImageResizeUtility";

    public ImageResizeUtils(IVssRequestContext requestContext)
      : base((Func<Exception, bool>) (ex => ImageResizeUtils.ExceptionHandle(ex, requestContext)), requestContext.ExecutionEnvironment.IsHostedDeployment)
    {
    }

    private static bool ExceptionHandle(Exception ex, IVssRequestContext requestContext)
    {
      requestContext.TraceException(12061066, "Gallery", "ImageResizeUtility", ex);
      return false;
    }
  }
}

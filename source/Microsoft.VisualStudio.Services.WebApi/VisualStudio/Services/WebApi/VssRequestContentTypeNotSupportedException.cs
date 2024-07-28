// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.VssRequestContentTypeNotSupportedException
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.WebApi
{
  [ExceptionMapping("0.0", "3.0", "VssRequestContentTypeNotSupportedException", "Microsoft.VisualStudio.Services.WebApi.VssRequestContentTypeNotSupportedException, Microsoft.VisualStudio.Services.WebApi, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public class VssRequestContentTypeNotSupportedException : VssServiceException
  {
    public VssRequestContentTypeNotSupportedException(
      string contentType,
      string httpMethod,
      IEnumerable<string> validContentTypes)
      : base(WebApiResources.RequestContentTypeNotSupported((object) contentType, (object) httpMethod, (object) string.Join(", ", validContentTypes)))
    {
    }
  }
}

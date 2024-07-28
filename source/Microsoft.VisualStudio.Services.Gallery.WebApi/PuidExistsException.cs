// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.WebApi.PuidExistsException
// Assembly: Microsoft.VisualStudio.Services.Gallery.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EE9D0AAA-B110-4AD6-813B-50FA04AC401A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Gallery.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Gallery.WebApi
{
  [ExceptionMapping("0.0", "3.0", "PuidExistsException", "Microsoft.VisualStudio.Services.Gallery.WebApi.PuidExistsException, Microsoft.VisualStudio.Services.Gallery.WebApi, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public class PuidExistsException : VssServiceException
  {
    public PuidExistsException(string message)
      : base(message)
    {
    }

    public PuidExistsException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}

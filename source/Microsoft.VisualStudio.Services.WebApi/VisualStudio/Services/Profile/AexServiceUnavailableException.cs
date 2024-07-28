// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Profile.AexServiceUnavailableException
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Profile
{
  [ExceptionMapping("0.0", "3.0", "AexServiceUnavailableException", "Microsoft.VisualStudio.Services.Profile.AexServiceUnavailableException, Microsoft.VisualStudio.Services.WebApi, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public class AexServiceUnavailableException : VssServiceException
  {
    public AexServiceUnavailableException()
    {
    }

    public AexServiceUnavailableException(string message)
      : base(message)
    {
    }

    public AexServiceUnavailableException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected AexServiceUnavailableException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}

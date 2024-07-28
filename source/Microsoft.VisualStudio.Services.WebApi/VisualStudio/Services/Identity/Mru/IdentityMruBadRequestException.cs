// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Mru.IdentityMruBadRequestException
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Identity.Mru
{
  [ExceptionMapping("0.0", "3.0", "IdentityMruBadRequestException", "Microsoft.VisualStudio.Services.Identity.Mru.IdentityMruBadRequestException, Microsoft.VisualStudio.Services.WebApi, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public class IdentityMruBadRequestException : IdentityMruException
  {
    protected IdentityMruBadRequestException()
    {
    }

    public IdentityMruBadRequestException(string message)
      : base(message)
    {
    }

    public IdentityMruBadRequestException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected IdentityMruBadRequestException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}

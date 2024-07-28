// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.InvalidIdentityKeyException
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Identity
{
  [ExceptionMapping("0.0", "3.0", "InvalidIdentityKeyException", "Microsoft.VisualStudio.Services.Identity.InvalidIdentityKeyException, Microsoft.VisualStudio.Services.WebApi, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public class InvalidIdentityKeyException : IdentityServiceException
  {
    public InvalidIdentityKeyException()
    {
    }

    public InvalidIdentityKeyException(string message)
      : base(message)
    {
    }
  }
}

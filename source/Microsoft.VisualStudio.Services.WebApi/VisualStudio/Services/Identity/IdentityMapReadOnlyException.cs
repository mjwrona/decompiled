// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityMapReadOnlyException
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Identity
{
  [ExceptionMapping("0.0", "3.0", "IdentityMapReadOnlyException", "Microsoft.VisualStudio.Services.Identity.IdentityMapReadOnlyException, Microsoft.VisualStudio.Services.WebApi, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public class IdentityMapReadOnlyException : IdentityServiceException
  {
    public IdentityMapReadOnlyException()
      : this((Exception) null)
    {
    }

    public IdentityMapReadOnlyException(Exception innerException)
      : base(IdentityResources.IdentityMapReadOnlyException(), innerException)
    {
    }

    public IdentityMapReadOnlyException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected IdentityMapReadOnlyException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}

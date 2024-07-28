// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.Jwt.DigestUnsupportedException
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.WebApi.Jwt
{
  [ExceptionMapping("0.0", "3.0", "DigestUnsupportedException", "Microsoft.VisualStudio.Services.WebApi.Jwt.DigestUnsupportedException, Microsoft.VisualStudio.Services.WebApi, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public class DigestUnsupportedException : JsonWebTokenException
  {
    public DigestUnsupportedException(string supportedDigest, string invalidDigest)
      : base(JwtResources.DigestUnsupportedException((object) supportedDigest, (object) invalidDigest))
    {
    }
  }
}

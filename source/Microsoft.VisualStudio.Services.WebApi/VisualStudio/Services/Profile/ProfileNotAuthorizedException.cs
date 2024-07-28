// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Profile.ProfileNotAuthorizedException
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Profile
{
  [ExceptionMapping("0.0", "3.0", "ProfileNotAuthorizedException", "Microsoft.VisualStudio.Services.Profile.ProfileNotAuthorizedException, Microsoft.VisualStudio.Services.WebApi, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public class ProfileNotAuthorizedException : ProfileException
  {
    public ProfileNotAuthorizedException()
    {
    }

    public ProfileNotAuthorizedException(string message, string url)
      : base(message)
    {
      this.HelpLink = url;
    }

    public ProfileNotAuthorizedException(string message, string url, Exception innerException)
      : base(message, innerException)
    {
      this.HelpLink = url;
    }

    protected ProfileNotAuthorizedException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public override sealed string HelpLink { get; set; }
  }
}

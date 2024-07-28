// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Profile.ProfileDoesNotExistException
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Profile
{
  [ExceptionMapping("0.0", "3.0", "ProfileDoesNotExistException", "Microsoft.VisualStudio.Services.Profile.ProfileDoesNotExistException, Microsoft.VisualStudio.Services.WebApi, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public class ProfileDoesNotExistException : ProfileException
  {
    public ProfileDoesNotExistException()
    {
    }

    public ProfileDoesNotExistException(string message)
      : base(message)
    {
    }

    public ProfileDoesNotExistException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected ProfileDoesNotExistException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}

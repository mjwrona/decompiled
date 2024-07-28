// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.InvalidClientVersionException
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Licensing
{
  [ExceptionMapping("0.0", "3.0", "InvalidClientVersionException", "Microsoft.VisualStudio.Services.Licensing.InvalidClientVersionException, Microsoft.VisualStudio.Services.WebApi, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public class InvalidClientVersionException : LicensingException
  {
    public InvalidClientVersionException()
      : base(string.Empty)
    {
    }

    public InvalidClientVersionException(string message)
      : base(message)
    {
    }

    public InvalidClientVersionException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}

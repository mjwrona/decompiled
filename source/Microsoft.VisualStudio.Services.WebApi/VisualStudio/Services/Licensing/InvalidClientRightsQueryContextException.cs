// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.InvalidClientRightsQueryContextException
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Licensing
{
  [ExceptionMapping("0.0", "3.0", "InvalidClientRightsQueryContextException", "Microsoft.VisualStudio.Services.Licensing.InvalidClientRightsQueryContextException, Microsoft.VisualStudio.Services.WebApi, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public class InvalidClientRightsQueryContextException : LicensingException
  {
    public InvalidClientRightsQueryContextException()
      : base(string.Empty)
    {
    }

    public InvalidClientRightsQueryContextException(string message)
      : base(message)
    {
    }

    public InvalidClientRightsQueryContextException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}

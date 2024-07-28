// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.OAuthWhitelist.OAuthWhitelistEntryAlreadyExistsException
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.OAuthWhitelist
{
  [ExceptionMapping("0.0", "3.0", "OAuthWhitelistEntryAlreadyExistsException", "Microsoft.VisualStudio.Services.OAuthWhitelist.OAuthWhitelistEntryAlreadyExistsException, Microsoft.VisualStudio.Services.WebApi, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public class OAuthWhitelistEntryAlreadyExistsException : VssServiceException
  {
    public OAuthWhitelistEntryAlreadyExistsException()
    {
    }

    public OAuthWhitelistEntryAlreadyExistsException(string message)
      : base(message)
    {
    }

    public OAuthWhitelistEntryAlreadyExistsException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Settings.InvalidSettingsScopeException
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Settings
{
  [Serializable]
  public class InvalidSettingsScopeException : VssServiceException
  {
    public InvalidSettingsScopeException(string message)
      : base(message)
    {
    }

    public InvalidSettingsScopeException(string message, Exception ex)
      : base(message, ex)
    {
    }
  }
}

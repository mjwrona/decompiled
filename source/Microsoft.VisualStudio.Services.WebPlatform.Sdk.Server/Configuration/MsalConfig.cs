// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Configuration.MsalConfig
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Configuration
{
  public class MsalConfig
  {
    public string CacheLocation { get; set; }

    public HashSet<string> FatalErrorCodes { get; set; }

    public int LogLevel { get; set; }

    public int CacheLookupPolicy { get; set; }

    public int? IframeHashTimeout { get; set; }

    public long? CacheWatermark { get; set; }
  }
}

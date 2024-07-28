// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions.LoaderPriority
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions
{
  public enum LoaderPriority
  {
    Immediate = 10, // 0x0000000A
    TTI = 20, // 0x00000014
    PostTTI = 30, // 0x0000001E
    Deferred = 100, // 0x00000064
  }
}

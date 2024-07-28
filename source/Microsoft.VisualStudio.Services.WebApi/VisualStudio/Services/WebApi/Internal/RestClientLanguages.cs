// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.Internal.RestClientLanguages
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.WebApi.Internal
{
  [Flags]
  public enum RestClientLanguages
  {
    All = -1, // 0xFFFFFFFF
    CSharp = 1,
    Java = 2,
    TypeScript = 4,
    Nodejs = 8,
    [Obsolete("DocMD has been replaced by Swagger generated REST Documentation.")] DocMD = 16, // 0x00000010
    Swagger2 = 32, // 0x00000020
    Python = 64, // 0x00000040
    TypeScriptWebPlatform = 128, // 0x00000080
    Go = 256, // 0x00000100
  }
}

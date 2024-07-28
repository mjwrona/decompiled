// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Profile.CoreProfileAttributes
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.Profile
{
  [Flags]
  public enum CoreProfileAttributes
  {
    Minimal = 0,
    Email = 1,
    Avatar = 2,
    DisplayName = 4,
    ContactWithOffers = 8,
    All = 65535, // 0x0000FFFF
  }
}

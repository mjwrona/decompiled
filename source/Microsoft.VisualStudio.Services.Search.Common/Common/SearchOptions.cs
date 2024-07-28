// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.SearchOptions
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  [Flags]
  public enum SearchOptions
  {
    None = 0,
    Highlighting = 1,
    Faceting = 2,
    Ranking = 4,
    Rescore = 8,
    AllowSpellingErrors = 16, // 0x00000010
    All = AllowSpellingErrors | Rescore | Ranking | Faceting | Highlighting, // 0x0000001F
  }
}

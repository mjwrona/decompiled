// Decompiled with JetBrains decompiler
// Type: Pegasus.Common.Highlighting.HighlightRuleCollection`1
// Assembly: Pegasus.Common, Version=4.0.14.0, Culture=neutral, PublicKeyToken=28c69b6c6d100f4a
// MVID: 081C50C0-D236-41F7-86F9-E3F2168B7118
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Pegasus.Common.dll

using System.Collections.Generic;

namespace Pegasus.Common.Highlighting
{
  public class HighlightRuleCollection<T> : List<HighlightRule<T>>
  {
    public void Add(string pattern, T value) => this.Add(new HighlightRule<T>(pattern, value));
  }
}

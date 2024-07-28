// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Common.VariableMatch
// Assembly: Microsoft.TeamFoundation.Build.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AD9C54FA-787C-49B8-AA73-C4A6EF8CE391
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build.Common.dll

namespace Microsoft.TeamFoundation.Build.Common
{
  internal sealed class VariableMatch
  {
    public VariableMatch(string name, int startIndex, int endIndex)
    {
      this.Name = name;
      this.StartIndex = startIndex;
      this.EndIndex = endIndex;
    }

    public string Name { get; private set; }

    public int EndIndex { get; private set; }

    public int StartIndex { get; private set; }
  }
}

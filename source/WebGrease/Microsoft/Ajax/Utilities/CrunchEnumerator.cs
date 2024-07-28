// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.CrunchEnumerator
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System.Collections.Generic;
using System.Text;

namespace Microsoft.Ajax.Utilities
{
  public class CrunchEnumerator
  {
    private HashSet<string> m_skipNames;
    private int m_currentName = -1;
    private static string s_varFirstLetters = "ntirufeoshclavypwbkdg";
    private static string s_varPartLetters = "tirufeoshclavypwbkdgn";

    public static string FirstLetters
    {
      get => CrunchEnumerator.s_varFirstLetters;
      set => CrunchEnumerator.s_varFirstLetters = value;
    }

    public static string PartLetters
    {
      get => CrunchEnumerator.s_varPartLetters ?? CrunchEnumerator.s_varFirstLetters;
      set => CrunchEnumerator.s_varPartLetters = value;
    }

    internal CrunchEnumerator(IEnumerable<string> avoidNames) => this.m_skipNames = new HashSet<string>(avoidNames);

    internal string NextName()
    {
      string currentName;
      do
      {
        ++this.m_currentName;
        currentName = this.CurrentName;
      }
      while (this.m_skipNames.Contains(currentName) || JSScanner.IsKeyword(currentName, true));
      return currentName;
    }

    private string CurrentName => CrunchEnumerator.GenerateNameFromNumber(this.m_currentName);

    public static string CrunchedLabel(int nestLevel)
    {
      string name = (string) null;
      if (nestLevel >= 0)
      {
        name = CrunchEnumerator.GenerateNameFromNumber(nestLevel);
        if (JSScanner.IsKeyword(name, true))
          name = '_'.ToString() + name;
      }
      return name;
    }

    public static string GenerateNameFromNumber(int index)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (index >= 0)
      {
        stringBuilder.Append(CrunchEnumerator.s_varFirstLetters[index % CrunchEnumerator.s_varFirstLetters.Length]);
        for (index /= CrunchEnumerator.s_varFirstLetters.Length; --index >= 0; index /= CrunchEnumerator.s_varPartLetters.Length)
          stringBuilder.Append(CrunchEnumerator.s_varPartLetters[index % CrunchEnumerator.s_varPartLetters.Length]);
      }
      return stringBuilder.ToString();
    }
  }
}

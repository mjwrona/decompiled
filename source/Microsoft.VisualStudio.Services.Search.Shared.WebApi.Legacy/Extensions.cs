// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Extensions
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8C7C9E57-44B4-4654-9458-CC8B59C2B681
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.dll

using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy
{
  internal static class Extensions
  {
    private const int c_tabstopLen = 2;
    private const int c_resultsLen = 16;
    private static readonly string[] s_results = new string[16];

    internal static string GetIndentSpacing(int indentLevel)
    {
      string indentSpacing;
      if (indentLevel == 0)
        indentSpacing = string.Empty;
      else if (indentLevel < 16)
      {
        indentSpacing = Extensions.s_results[indentLevel];
        if (indentSpacing == null)
          Extensions.s_results[indentLevel] = indentSpacing = new string(' ', indentLevel * 2);
      }
      else
        indentSpacing = new string(' ', indentLevel * 2);
      return indentSpacing;
    }

    internal static StringBuilder Append(this StringBuilder sb, string spacing, string s) => sb.Append(spacing).Append(s);

    internal static StringBuilder AppendLine(this StringBuilder sb, string spacing, string s) => sb.Append(spacing).AppendLine(s);
  }
}

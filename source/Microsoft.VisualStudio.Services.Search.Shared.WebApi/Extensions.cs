// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Extensions
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 504F400B-CBC4-4007-9816-31A8DED1C3FC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.WebApi.dll

using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Shared.WebApi
{
  internal static class Extensions
  {
    private const string Tabstop = "  ";

    internal static string GetIndentSpacing(int indentLevel) => indentLevel == 0 ? string.Empty : new StringBuilder().Append(' ', "  ".Length * indentLevel).ToString();

    internal static StringBuilder Append(this StringBuilder sb, string spacing, string s) => sb.Append(spacing).Append(s);

    internal static StringBuilder AppendLine(this StringBuilder sb, string spacing, string s) => sb.Append(spacing).AppendLine(s);
  }
}

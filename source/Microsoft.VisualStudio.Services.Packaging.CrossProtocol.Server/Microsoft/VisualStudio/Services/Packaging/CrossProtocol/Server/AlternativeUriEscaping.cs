// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server.AlternativeUriEscaping
// Assembly: Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9BF15B3F-0578-4452-9C4B-B5237E218DF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server.dll

using System;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server
{
  public class AlternativeUriEscaping
  {
    private static readonly Regex SpecialCharRegex = new Regex("=(?<code>[0-9a-f]{4})", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static string UnescapeString(string input) => AlternativeUriEscaping.SpecialCharRegex.Replace(input, (MatchEvaluator) (match => new string((char) Convert.ToUInt16(match.Groups["code"].Value, 16), 1)));
  }
}

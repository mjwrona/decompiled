// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.OAuth2.ScopeHelpers
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server.OAuth2
{
  public static class ScopeHelpers
  {
    public const char ScopeDelimiter = ':';
    public const string ScopeDelimiterStr = ":";
    private const string Space = " ";
    private const string EncodedSpace = "%20";

    public static string ConstructScope(string scopeIdentifier, params string[] variables)
    {
      string str1 = scopeIdentifier;
      if (variables.Length != 0)
      {
        string str2 = string.Join(":", ((IEnumerable<string>) variables).Select<string, string>((Func<string, string>) (v => ScopeHelpers.EncodeSpace(v))));
        str1 = scopeIdentifier + ":" + str2;
      }
      return str1;
    }

    public static void DeconstructScope(
      string scope,
      out string scopeIdentifier,
      out string[] variables)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(scope, nameof (scope));
      string[] source = scope.Split(':');
      scopeIdentifier = source[0];
      if (source.Length > 1)
        variables = ((IEnumerable<string>) source).Skip<string>(1).Select<string, string>((Func<string, string>) (v => ScopeHelpers.DecodeSpace(v))).ToArray<string>();
      else
        variables = Array.Empty<string>();
    }

    private static string EncodeSpace(string variable) => variable?.Replace(" ", "%20");

    private static string DecodeSpace(string encodedVariable) => encodedVariable?.Replace("%20", " ");
  }
}

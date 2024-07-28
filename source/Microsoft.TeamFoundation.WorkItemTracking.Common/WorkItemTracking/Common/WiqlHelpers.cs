// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.WiqlHelpers
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class WiqlHelpers
  {
    private const string cSingleQuote = "'";
    private const string cTwoSingleQuotes = "''";
    private const string cSingleQuotedStringFormat = "'{0}'";
    private const string cDoubleQuotedStringFormat = "\"{0}\"";
    private const string cBracketedStringFormat = "[{0}]";

    public static Dictionary<string, string> GetParameterDictionary(string projectName) => new Dictionary<string, string>()
    {
      {
        "project",
        projectName
      }
    };

    public static string GetEscapedSingleQuotedValue(string value) => WiqlHelpers.GetSingleQuotedValue(value.Replace("'", "''"));

    public static string GetSingleQuotedValue(string value) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "'{0}'", (object) value);

    public static string GetDoubleQuotedValue(string value) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"{0}\"", (object) value);

    public static string GetEnclosedField(string value) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[{0}]", (object) value);
  }
}

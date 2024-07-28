// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.WebApi.JsonPathSubscriptionExpression
// Assembly: Microsoft.VisualStudio.Services.Notifications.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF217E0A-7730-437B-BE9F-877363CB7392
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.WebApi.dll

using System;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.Notifications.WebApi
{
  public class JsonPathSubscriptionExpression : PathSubscriptionExpression
  {
    private const string c_jsonFieldRegex = "\\$\\.(?<name>[a-zA-Z0-9\\[?(0-9)?\\]\\.]*)";
    private const string c_jsonOperatorRegex = "(?<operator>\\=|\\!\\=|\\<|\\<=|>=)";
    private const string c_jsonValueRegex = "(?<value>[a-zA-Z0-9\\.]*)";
    private string c_jsonPathFormat = "$.{0} {1} '{2}'";
    private string c_jsonPathRegex = string.Format("\\$\\.(?<name>[a-zA-Z0-9\\[?(0-9)?\\]\\.]*)(?<operator>\\=|\\!\\=|\\<|\\<=|>=)(?<value>[a-zA-Z0-9\\.]*)");

    public override PathSubscriptionExpression ParsePath(string jsonPath)
    {
      JsonPathSubscriptionExpression path1 = new JsonPathSubscriptionExpression();
      string path2;
      string filter;
      string postFilterPath;
      if (!JsonPathSubscriptionExpression.ParseLastJsonPathFilter(jsonPath, out path2, out filter, out postFilterPath))
      {
        path1.Path = jsonPath;
        path1.FilterType = FieldFilterType.None;
      }
      else
      {
        path1.Path = path2;
        path1.PostFilterPath = postFilterPath;
        path1.FilterType = FieldFilterType.NoOperator;
        path1.FilterName = (Token) new XPathToken(filter);
        path1.FilterName.EvaluatePathFunctions();
        Match match = new Regex(this.c_jsonPathRegex).Match(jsonPath);
        if (match.Success)
        {
          path1.FilterType = this.GetFilterType(match.Groups["operator"].Value);
          path1.FilterName = (Token) new JsonPathToken(match.Groups["name"].Value);
          path1.FilterValue = (Token) new JsonPathToken(match.Groups["value"].Value);
          path1.FilterNameIgnoreCase = true;
        }
      }
      return (PathSubscriptionExpression) path1;
    }

    private FieldFilterType GetFilterType(string operatorStr)
    {
      switch (operatorStr)
      {
        case "=":
          return FieldFilterType.Equals;
        case "!=":
          return FieldFilterType.NotEquals;
        default:
          throw new NotImplementedException("This operator hasn't been implemented yet");
      }
    }

    public override string ToPath()
    {
      string rawOperator = JsonPathSubscriptionExpression.GetRawOperator(this.FilterType);
      string spelling1 = this.FilterName.Spelling;
      string spelling2 = this.FilterValue.Spelling;
      return string.Format(this.c_jsonPathFormat, (object) spelling1, (object) rawOperator, (object) spelling2);
    }

    private static string GetRawOperator(FieldFilterType filterType)
    {
      switch (filterType)
      {
        case FieldFilterType.None:
        case FieldFilterType.NoOperator:
          return string.Empty;
        case FieldFilterType.Equals:
          return "=";
        case FieldFilterType.NotEquals:
          return "!=";
        case FieldFilterType.Contains:
          return Token.spellings[15];
        case FieldFilterType.NotContains:
          return Token.spellings[25];
        case FieldFilterType.CountGT:
          return ">";
        case FieldFilterType.CountEqualTo:
          return "=";
        default:
          throw new NotImplementedException(string.Format("The operator {0} hasn't been implemented yet", (object) filterType));
      }
    }

    public static bool ParseLastJsonPathFilter(
      string jsonPath,
      out string path,
      out string filter,
      out string postFilterPath)
    {
      Match match = new Regex("\\$\\.(?<name>[a-zA-Z0-9\\[?(0-9)?\\]\\.]*)").Match(jsonPath);
      if (match.Success)
      {
        path = match.Groups["name"].Value;
        filter = path;
        postFilterPath = (string) null;
        return true;
      }
      path = (string) null;
      filter = (string) null;
      postFilterPath = (string) null;
      return false;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.StringFieldCompare
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class StringFieldCompare : FieldCompare<string>
  {
    public override bool Compare(byte op, EvaluationContext evaluationContext, string a, string b)
    {
      switch (op)
      {
        case 15:
        case 16:
          b = StringFieldCompare.OptimizeRegexMatch(b);
          return Regex.IsMatch(a, b, RegexOptions.IgnoreCase, evaluationContext.RegexTimeout);
        case 25:
          b = StringFieldCompare.OptimizeRegexMatch(b);
          return !Regex.IsMatch(a, b, RegexOptions.IgnoreCase, evaluationContext.RegexTimeout);
        default:
          return base.Compare(op, evaluationContext, a, b);
      }
    }

    private static string OptimizeRegexMatch(string evaluatedTarget)
    {
      if (evaluatedTarget.Length >= 4 && evaluatedTarget.StartsWith(".*") && evaluatedTarget.EndsWith(".*"))
        evaluatedTarget = evaluatedTarget.Substring(2, evaluatedTarget.Length - 4);
      return evaluatedTarget;
    }

    public override string GetComparable(object o) => o.ToString();
  }
}

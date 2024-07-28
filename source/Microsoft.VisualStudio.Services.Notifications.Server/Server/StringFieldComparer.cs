// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.StringFieldComparer
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using System;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class StringFieldComparer : FieldComparer
  {
    protected override IComparable FromObject(object o) => (IComparable) o.ToString();

    public override bool Compare(
      byte op,
      EvaluationContext evaluationContext,
      IComparable a,
      IComparable b)
    {
      string str = this.FromObject((object) a) as string;
      string pattern = this.FromObject((object) b) as string;
      switch (op)
      {
        case 15:
        case 16:
          b = (IComparable) StringFieldComparer.OptimizeRegexMatch(str);
          return Regex.IsMatch(str, pattern, RegexOptions.IgnoreCase, evaluationContext.RegexTimeout);
        case 25:
          b = (IComparable) StringFieldComparer.OptimizeRegexMatch(str);
          return !Regex.IsMatch(str, pattern, RegexOptions.IgnoreCase, evaluationContext.RegexTimeout);
        default:
          return base.Compare(op, evaluationContext, a, b);
      }
    }

    public static string OptimizeRegexMatch(string evaluatedTarget)
    {
      if (evaluatedTarget.Length >= 4 && evaluatedTarget.StartsWith(".*") && evaluatedTarget.EndsWith(".*"))
        evaluatedTarget = evaluatedTarget.Substring(2, evaluatedTarget.Length - 4);
      return evaluatedTarget;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Declarations.MatchRuleDeclaration
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Declarations
{
  public class MatchRuleDeclaration : WorkItemRuleDeclaration
  {
    public MatchRuleDeclaration(XElement xmlRuleElement, Action<string> logError)
      : base(xmlRuleElement)
    {
      this.Pattern = Utilities.RequireAttribute(xmlRuleElement, (XName) "pattern", logError);
    }

    public override WorkItemRuleName Name => WorkItemRuleName.Match;

    public string Pattern { get; set; }

    public override bool Equals(
      WorkItemRuleDeclaration other,
      bool deep,
      Func<WorkItemRuleDeclaration, bool> applicable)
    {
      if (!base.Equals(other, deep, applicable))
        return false;
      MatchRuleDeclaration matchRuleDeclaration = other as MatchRuleDeclaration;
      return StringComparer.OrdinalIgnoreCase.Equals(this.Pattern ?? "", matchRuleDeclaration?.Pattern ?? "");
    }

    public override int CompareTo(WorkItemRuleDeclaration other)
    {
      int num = base.CompareTo(other);
      if (num == 0 && other is MatchRuleDeclaration matchRuleDeclaration)
        num = StringComparer.OrdinalIgnoreCase.Compare(this.Pattern, matchRuleDeclaration.Pattern);
      return num;
    }
  }
}

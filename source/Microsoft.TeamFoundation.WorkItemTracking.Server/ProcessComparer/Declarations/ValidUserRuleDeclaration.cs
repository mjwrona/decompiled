// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Declarations.ValidUserRuleDeclaration
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Declarations
{
  public class ValidUserRuleDeclaration : WorkItemRuleDeclaration
  {
    public ValidUserRuleDeclaration(XElement xmlRuleElement)
      : base(xmlRuleElement)
    {
      this.Group = xmlRuleElement.Attribute((XName) "group")?.Value;
    }

    public override WorkItemRuleName Name => WorkItemRuleName.ValidUser;

    public string Group { get; set; }

    public override bool Equals(
      WorkItemRuleDeclaration other,
      bool deep,
      Func<WorkItemRuleDeclaration, bool> applicable)
    {
      return base.Equals(other, deep, applicable) && StringComparer.OrdinalIgnoreCase.Equals(this.Group ?? "", (other is ValidUserRuleDeclaration userRuleDeclaration ? userRuleDeclaration.Group : (string) null) ?? "");
    }

    public override int CompareTo(WorkItemRuleDeclaration other)
    {
      int num = base.CompareTo(other);
      return num == 0 && other is ValidUserRuleDeclaration userRuleDeclaration ? StringComparer.OrdinalIgnoreCase.Compare(this.Group, userRuleDeclaration.Group) : num;
    }
  }
}

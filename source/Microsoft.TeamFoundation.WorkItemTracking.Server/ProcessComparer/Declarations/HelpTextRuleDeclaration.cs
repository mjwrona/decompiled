// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Declarations.HelpTextRuleDeclaration
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Declarations
{
  public class HelpTextRuleDeclaration : WorkItemRuleDeclaration
  {
    public HelpTextRuleDeclaration(XElement xmlRuleElement)
      : base(xmlRuleElement)
    {
      this.Content = xmlRuleElement.Value;
    }

    public override WorkItemRuleName Name => WorkItemRuleName.HelpText;

    public string Content { get; set; }

    public override bool Equals(
      WorkItemRuleDeclaration other,
      bool deep,
      Func<WorkItemRuleDeclaration, bool> applicable)
    {
      return base.Equals(other, deep, applicable) && StringComparer.OrdinalIgnoreCase.Equals(this.Content ?? "", (other is HelpTextRuleDeclaration textRuleDeclaration ? textRuleDeclaration.Content : (string) null) ?? "");
    }

    public override int CompareTo(WorkItemRuleDeclaration other)
    {
      int num = base.CompareTo(other);
      return num == 0 && other is HelpTextRuleDeclaration textRuleDeclaration ? StringComparer.OrdinalIgnoreCase.Compare(this.Content, textRuleDeclaration.Content) : num;
    }
  }
}

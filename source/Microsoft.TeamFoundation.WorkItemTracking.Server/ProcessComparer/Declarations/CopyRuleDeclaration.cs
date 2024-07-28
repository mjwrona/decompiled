// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Declarations.CopyRuleDeclaration
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Declarations
{
  public class CopyRuleDeclaration : DependentRuleDeclaration
  {
    public CopyRuleDeclaration(XElement xmlRuleElement, Action<string> logError)
      : base(xmlRuleElement, false, logError)
    {
      this.From = Utilities.RequireAttribute(xmlRuleElement, (XName) "from", logError);
      switch (this.From)
      {
        case "field":
          if (!string.IsNullOrEmpty(this.Field))
            break;
          logError(string.Format("The attribute 'field' is required for {0}.", (object) xmlRuleElement.Name));
          break;
        case "clock":
          break;
        case "currentuser":
          break;
        case "value":
          this.Value = xmlRuleElement.Attribute((XName) "value")?.Value;
          break;
        default:
          logError("The attribute value '" + this.From + "' for 'from' is not supported.");
          break;
      }
    }

    public override WorkItemRuleName Name => WorkItemRuleName.Copy;

    public string From { get; set; }

    public string Value { get; set; }

    public override bool Equals(
      WorkItemRuleDeclaration other,
      bool deep,
      Func<WorkItemRuleDeclaration, bool> applicable)
    {
      if (!base.Equals(other, deep, applicable))
        return false;
      CopyRuleDeclaration copyRuleDeclaration = other as CopyRuleDeclaration;
      return StringComparer.OrdinalIgnoreCase.Equals(this.From ?? "", copyRuleDeclaration?.From ?? "") && StringComparer.OrdinalIgnoreCase.Equals(this.Value ?? "", copyRuleDeclaration?.Value ?? "");
    }

    public override int CompareTo(WorkItemRuleDeclaration other)
    {
      int num = base.CompareTo(other);
      if (num == 0 && other is CopyRuleDeclaration copyRuleDeclaration)
      {
        num = StringComparer.OrdinalIgnoreCase.Compare(this.From, copyRuleDeclaration.From);
        if (num == 0)
          num = StringComparer.OrdinalIgnoreCase.Compare(this.Value, copyRuleDeclaration.Value);
      }
      return num;
    }
  }
}

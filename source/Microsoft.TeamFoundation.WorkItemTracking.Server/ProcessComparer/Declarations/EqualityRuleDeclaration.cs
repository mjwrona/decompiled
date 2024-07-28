// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Declarations.EqualityRuleDeclaration
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Declarations
{
  public abstract class EqualityRuleDeclaration : ConditionalRuleDeclaration
  {
    public EqualityRuleDeclaration(string field, string value)
      : base(field)
    {
      this.Value = value;
    }

    public EqualityRuleDeclaration(XElement xmlRuleElement, Action<string> logError)
      : base(xmlRuleElement, logError)
    {
      this.Value = Utilities.RequireAttribute(xmlRuleElement, (XName) "value", logError);
    }

    public string Value { get; private set; }

    public override bool Equals(
      WorkItemRuleDeclaration other,
      bool deep,
      Func<WorkItemRuleDeclaration, bool> applicable)
    {
      if (!base.Equals(other, deep, applicable))
        return false;
      EqualityRuleDeclaration equalityRuleDeclaration = other as EqualityRuleDeclaration;
      return StringComparer.OrdinalIgnoreCase.Equals(this.Value ?? "", equalityRuleDeclaration?.Value ?? "");
    }

    public override int CompareTo(WorkItemRuleDeclaration other)
    {
      int num = base.CompareTo(other);
      if (num == 0 && other is EqualityRuleDeclaration equalityRuleDeclaration)
        num = StringComparer.OrdinalIgnoreCase.Compare(this.Value, equalityRuleDeclaration.Value);
      return num;
    }
  }
}

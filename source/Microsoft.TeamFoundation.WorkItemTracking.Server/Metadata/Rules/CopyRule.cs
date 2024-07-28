// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules.CopyRule
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules
{
  [XmlType("copy")]
  [XmlInclude(typeof (IdentityCopyRule))]
  public class CopyRule : ActionRule
  {
    public CopyRule() => this.Name = WorkItemRuleName.Copy;

    internal override RuleEnginePhase Phase => RuleEnginePhase.CopyRules;

    public override bool Equals(WorkItemRule other, bool deep)
    {
      if (!base.Equals(other, deep))
        return false;
      CopyRule copyRule = other as CopyRule;
      return this.ValueFrom == copyRule.ValueFrom && this.Value == copyRule.Value;
    }

    protected internal override int RuleWeight => 80;
  }
}

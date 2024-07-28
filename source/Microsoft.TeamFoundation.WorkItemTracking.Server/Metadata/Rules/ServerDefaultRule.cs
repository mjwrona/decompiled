// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules.ServerDefaultRule
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Common;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules
{
  [XmlType("server-default")]
  public class ServerDefaultRule : WorkItemRule
  {
    private ServerDefaultType m_From;

    public ServerDefaultRule() => this.Name = WorkItemRuleName.ServerDefault;

    internal override RuleEnginePhase Phase => RuleEnginePhase.CopyRules;

    protected internal override int RuleWeight => 100;

    [XmlAttribute("from")]
    public ServerDefaultType From
    {
      get => this.m_From;
      set
      {
        this.m_From = value;
        this.ReportRulesModification(nameof (From));
      }
    }

    public override bool Equals(WorkItemRule other, bool deep) => base.Equals(other, deep) && this.From == ((ServerDefaultRule) other).From;
  }
}

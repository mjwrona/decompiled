// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules.IdentityDefaultRule
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules
{
  [XmlType("identity-default")]
  public class IdentityDefaultRule : DefaultRule
  {
    private Guid m_Vsid;
    private int m_ConstId;

    [XmlAttribute("vsid")]
    public Guid Vsid
    {
      get => this.m_Vsid;
      set
      {
        this.m_Vsid = value;
        this.ReportRulesModification(nameof (Vsid));
      }
    }

    [XmlIgnore]
    public int ConstId
    {
      get => this.m_ConstId;
      internal set
      {
        this.m_ConstId = value;
        this.ReportRulesModification(nameof (ConstId));
      }
    }

    internal override IEnumerable<string> ExtractConstants() => Enumerable.Empty<string>();
  }
}

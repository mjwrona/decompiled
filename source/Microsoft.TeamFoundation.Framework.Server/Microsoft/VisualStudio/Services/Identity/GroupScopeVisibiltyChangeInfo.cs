// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.GroupScopeVisibiltyChangeInfo
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Text;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Identity
{
  [XmlType("gsvc")]
  public class GroupScopeVisibiltyChangeInfo
  {
    [XmlAttribute("scopeId")]
    public Guid ScopeId { get; set; }

    [XmlAttribute("groupScopeType")]
    public GroupScopeType GroupScopeType { get; set; }

    [XmlAttribute("identityId")]
    public Guid IdentityId { get; set; }

    [XmlAttribute("active")]
    public bool Active { get; set; }

    public override string ToString() => new StringBuilder().Append("{ ").Append("ScopeId: ").Append((object) this.ScopeId).Append(", GroupScopeType: ").Append((object) this.GroupScopeType).Append(", IdentityId: ").Append((object) this.IdentityId).Append(", Active: ").Append(this.Active).Append(" }").ToString();
  }
}

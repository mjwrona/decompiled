// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ScopeWellKnownGroupPermissionEntry
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class ScopeWellKnownGroupPermissionEntry
  {
    [XmlAttribute("ScopeType")]
    public string ScopeType;
    [XmlAttribute("AllowBits")]
    public int AllowBits;
    [XmlAttribute("DenyBits")]
    public int DenyBits;
    [XmlAttribute("IdentityDescriptor")]
    public string IdentityDescriptor;

    [XmlIgnore]
    public GroupScopeType GroupScopeType => (GroupScopeType) Enum.Parse(typeof (GroupScopeType), this.ScopeType);
  }
}

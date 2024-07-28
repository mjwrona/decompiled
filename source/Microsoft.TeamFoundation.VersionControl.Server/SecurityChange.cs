// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.SecurityChange
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.VersionControl.Common;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [XmlInclude(typeof (PermissionChange))]
  [XmlInclude(typeof (InheritanceChange))]
  public abstract class SecurityChange : IValidatable
  {
    private string m_item;

    [XmlAttribute("item")]
    public string Item
    {
      get => this.m_item;
      set => this.m_item = value;
    }

    internal virtual void Validate(
      VersionControlRequestContext versionControlRequestContext,
      string parameterName)
    {
      PathLength serverPathLength = versionControlRequestContext.MaxSupportedServerPathLength;
      versionControlRequestContext.Validation.checkItem(ref this.m_item, "item", false, false, true, false, serverPathLength);
    }

    void IValidatable.Validate(
      VersionControlRequestContext versionControlRequestContext,
      string parameterName)
    {
      this.Validate(versionControlRequestContext, parameterName);
    }
  }
}

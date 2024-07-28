// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.UpdatePackage
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common.Internal;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class UpdatePackage
  {
    private UpdatePackageOptions m_options;
    private string m_description;
    private string m_shortDescription;

    [XmlAttribute("kb")]
    public int KB { get; set; }

    [XmlIgnore]
    public int PatchNumber { get; set; }

    [XmlElement("Description")]
    public string Description
    {
      get => this.m_description != null ? StringUtil.ReplaceResources(this.m_description, out bool _) : (string) null;
      set => this.m_description = value;
    }

    [XmlElement("ShortDescription")]
    public string ShortDescription
    {
      get => this.m_shortDescription != null ? StringUtil.ReplaceResources(this.m_shortDescription, out bool _) : (string) null;
      set => this.m_shortDescription = value;
    }

    [XmlAttribute("testPatch")]
    public bool TestPatch
    {
      get => this.GetOption(UpdatePackageOptions.TestPatch);
      set => this.SetOption(UpdatePackageOptions.TestPatch, value);
    }

    [XmlIgnore]
    public bool Removed { get; set; }

    [XmlIgnore]
    public UpdatePackageOptions Options
    {
      get => this.m_options;
      internal set => this.m_options = value;
    }

    private void SetOption(UpdatePackageOptions option, bool value)
    {
      if (value)
        this.m_options |= option;
      else
        this.m_options &= ~option;
    }

    private bool GetOption(UpdatePackageOptions option) => (this.Options & option) == option;
  }
}

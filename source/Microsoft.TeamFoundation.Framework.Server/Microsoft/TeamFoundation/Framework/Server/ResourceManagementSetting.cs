// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ResourceManagementSetting
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DebuggerDisplay("Name: {Name}, Value: {Value}")]
  public class ResourceManagementSetting
  {
    public ResourceManagementSetting()
    {
    }

    public ResourceManagementSetting(string name, string value)
    {
      this.Name = name;
      this.Value = value;
    }

    [XmlAttribute("name")]
    public string Name { get; set; }

    [XmlAttribute("value")]
    public string Value { get; set; }
  }
}

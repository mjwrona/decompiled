// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Configuration
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class Configuration
  {
    [XmlAttribute("Platform")]
    public string Platform { get; set; }

    [XmlAttribute("Flavor")]
    public string Flavor { get; set; }

    public override bool Equals(object obj)
    {
      Configuration configuration = obj as Configuration;
      return string.Equals(this.Platform, configuration.Platform, StringComparison.OrdinalIgnoreCase) && string.Equals(this.Flavor, configuration.Flavor, StringComparison.OrdinalIgnoreCase);
    }

    public override int GetHashCode() => 1;
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Models.Xml.MavenPluginList
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Maven.Server.Models.Xml
{
  [DataContract]
  [XmlRoot("metadata")]
  public class MavenPluginList : MavenXml
  {
    public MavenPluginList() => this.Plugins = new List<MavenPluginItem>();

    [DataMember(Name = "plugins")]
    [XmlArray("plugins")]
    [XmlArrayItem("plugin")]
    public List<MavenPluginItem> Plugins { get; set; }
  }
}

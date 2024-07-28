// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.IndexColumn
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public class IndexColumn
  {
    [XmlAttribute("name")]
    public string Name { get; set; }

    [XmlAttribute("desc")]
    public bool IsDescending { get; set; }

    [XmlAttribute("included")]
    public bool IsIncludedColumn { get; set; }
  }
}

// Decompiled with JetBrains decompiler
// Type: XMimeMap
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

[GeneratedCode("xsd", "4.7.2558.0")]
[DebuggerStepThrough]
[DesignerCategory("code")]
[XmlRoot("Mime-Map", Namespace = "", IsNullable = false)]
[Serializable]
public class XMimeMap
{
  private XExtension[] extensionsField;
  private XMimeMapDisplayFilters displayFiltersField;
  private XMimeHexHeader[] hexHeadersField;

  [XmlArrayItem("Extension", IsNullable = false)]
  public XExtension[] Extensions
  {
    get => this.extensionsField;
    set => this.extensionsField = value;
  }

  [XmlElement("Display-Filters")]
  public XMimeMapDisplayFilters DisplayFilters
  {
    get => this.displayFiltersField;
    set => this.displayFiltersField = value;
  }

  [XmlArray("Hex-Headers")]
  [XmlArrayItem("HexHeader", IsNullable = false)]
  public XMimeHexHeader[] HexHeaders
  {
    get => this.hexHeadersField;
    set => this.hexHeadersField = value;
  }
}

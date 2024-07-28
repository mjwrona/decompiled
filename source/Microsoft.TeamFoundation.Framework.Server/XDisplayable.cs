// Decompiled with JetBrains decompiler
// Type: XDisplayable
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
[Serializable]
public class XDisplayable
{
  private XParameter[] parameterField;
  private string valueField;
  private XDisplayableType typeField;
  private string filterField;

  public XDisplayable() => this.typeField = XDisplayableType.contenttype;

  [XmlElement("Parameter")]
  public XParameter[] Parameter
  {
    get => this.parameterField;
    set => this.parameterField = value;
  }

  [XmlAttribute]
  public string value
  {
    get => this.valueField;
    set => this.valueField = value;
  }

  [XmlAttribute]
  [DefaultValue(XDisplayableType.contenttype)]
  public XDisplayableType type
  {
    get => this.typeField;
    set => this.typeField = value;
  }

  [XmlAttribute]
  public string filter
  {
    get => this.filterField;
    set => this.filterField = value;
  }
}

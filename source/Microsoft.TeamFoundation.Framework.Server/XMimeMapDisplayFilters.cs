// Decompiled with JetBrains decompiler
// Type: XMimeMapDisplayFilters
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
[XmlType(AnonymousType = true)]
[Serializable]
public class XMimeMapDisplayFilters
{
  private XDisplayable[] addField;
  private XDisplayable[] removeField;

  [XmlElement("Add")]
  public XDisplayable[] Add
  {
    get => this.addField;
    set => this.addField = value;
  }

  [XmlElement("Remove")]
  public XDisplayable[] Remove
  {
    get => this.removeField;
    set => this.removeField = value;
  }
}

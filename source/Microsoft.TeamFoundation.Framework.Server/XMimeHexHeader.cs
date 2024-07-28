// Decompiled with JetBrains decompiler
// Type: XMimeHexHeader
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
public class XMimeHexHeader
{
  private string headerbytesField;
  private string contenttypeField;

  [XmlAttribute("header-bytes")]
  public string headerbytes
  {
    get => this.headerbytesField;
    set => this.headerbytesField = value;
  }

  [XmlAttribute("content-type")]
  public string contenttype
  {
    get => this.contenttypeField;
    set => this.contenttypeField = value;
  }
}

// Decompiled with JetBrains decompiler
// Type: XExtension
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
public class XExtension
{
  private string nameField;
  private string contenttypeField;
  private string perceivedtypeField;

  [XmlAttribute]
  public string name
  {
    get => this.nameField;
    set => this.nameField = value;
  }

  [XmlAttribute("content-type")]
  public string contenttype
  {
    get => this.contenttypeField;
    set => this.contenttypeField = value;
  }

  [XmlAttribute("perceived-type")]
  public string perceivedtype
  {
    get => this.perceivedtypeField;
    set => this.perceivedtypeField = value;
  }
}

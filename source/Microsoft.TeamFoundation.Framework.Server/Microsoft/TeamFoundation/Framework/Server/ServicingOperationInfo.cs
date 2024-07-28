// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingOperationInfo
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [XmlRoot("Operation")]
  public class ServicingOperationInfo
  {
    [XmlAttribute("name")]
    public string Name { get; set; }

    [XmlAttribute("type")]
    public ServicingOperationType OperationType { get; set; }

    [XmlAttribute("target")]
    public ServicingOperationTarget Target { get; set; }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Security.Messages.RemoteSecurityNamespaceDataChangedMessage2
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Security.Messages
{
  [XmlType("data")]
  public class RemoteSecurityNamespaceDataChangedMessage2
  {
    [XmlAttribute("serviceOwner")]
    public Guid ServiceOwner { get; set; }

    [XmlAttribute("namespaceId")]
    public Guid NamespaceId { get; set; }

    [XmlAttribute("aclStoreId")]
    public Guid AclStoreId { get; set; }

    [XmlAttribute("newSequenceId")]
    public long[] NewSequenceId { get; set; }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.UserAccountMapping.CollectionDeleteEventData
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Organization;
using System;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.UserAccountMapping
{
  [XmlType("data")]
  public class CollectionDeleteEventData
  {
    [XmlAttribute("collectionId")]
    public Guid CollectionId { get; set; }

    [XmlAttribute("newCollectionStatus")]
    public CollectionStatus NewCollectionStatus { get; set; }
  }
}

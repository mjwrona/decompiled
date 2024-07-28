// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeExtensionBehaviorRecord
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [XmlType("behavior")]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class WorkItemTypeExtensionBehaviorRecord
  {
    [XmlAttribute("workItemTypeId")]
    public Guid WorkItemTypeId { get; set; }

    [XmlAttribute("referenceName")]
    public string BehaviorReferenceName { get; set; }

    [XmlAttribute("isDefault")]
    public bool IsDefault { get; set; }
  }
}

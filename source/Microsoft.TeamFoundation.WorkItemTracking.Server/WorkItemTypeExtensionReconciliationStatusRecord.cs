// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeExtensionReconciliationStatusRecord
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [XmlType("extension-reconciliation-status")]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class WorkItemTypeExtensionReconciliationStatusRecord
  {
    [XmlElement("id")]
    public Guid Id { get; set; }

    [XmlElement("reconciliation-status")]
    public int ReconciliationStatus { get; set; }

    [XmlElement("reconciliation-watermark")]
    public Guid ReconciliationWatermark { get; set; }

    [XmlElement("reconciliation-message")]
    public string ReconciliationMessage { get; set; }
  }
}

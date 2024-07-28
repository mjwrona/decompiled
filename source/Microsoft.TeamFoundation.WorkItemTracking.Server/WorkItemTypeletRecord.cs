// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeletRecord
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [XmlType("extension")]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class WorkItemTypeletRecord
  {
    [XmlElement("id")]
    public Guid Id { get; set; }

    [XmlElement("project-id")]
    public Guid ProjectId { get; set; }

    [XmlElement("owner-id")]
    public Guid OwnerId { get; set; }

    [XmlElement("name")]
    public string Name { get; set; }

    [XmlElement("refname")]
    public string ReferenceName { get; set; }

    [XmlElement("description")]
    public string Description { get; set; }

    [XmlElement("marker-field")]
    public int MarkerField { get; set; }

    [XmlElement("predicate")]
    public string Predicate { get; set; }

    [XmlElement("rules")]
    public string Rules { get; set; }

    [XmlElement("disabled-rules")]
    public string DisabledRules { get; set; }

    [XmlElement("form")]
    public string Form { get; set; }

    [XmlElement("rank")]
    public int Rank { get; set; }

    [XmlElement("last-change-date")]
    public DateTime LastChangeDate { get; set; }

    [XmlElement("reconciliation-status")]
    public int ReconciliationStatus { get; set; }

    [XmlElement("reconciliation-watermark")]
    public Guid ReconciliationWatermark { get; set; }

    [XmlElement("reconciliation-message")]
    public string ReconciliationMessage { get; set; }

    [XmlArray("fields")]
    [XmlArrayItem("field")]
    public WorkItemTypeletFieldRecord[] Fields { get; set; }

    [XmlElement("process-id")]
    public Guid ProcessId { get; set; }

    [XmlElement("parent-type-refname")]
    public string ParentTypeRefName { get; set; }

    [XmlElement("typelet-type")]
    public int TypeletType { get; set; }

    [XmlElement("is-deleted")]
    public bool IsDeleted { get; set; }

    [XmlElement("color")]
    public string Color { get; set; }

    [XmlElement("icon")]
    public string Icon { get; set; }

    [XmlElement("is-abstract")]
    public bool IsAbstract { get; set; }

    [XmlArray("behaviors")]
    [XmlArrayItem("behavior")]
    public WorkItemTypeExtensionBehaviorRecord[] Behaviors { get; set; }

    [XmlElement("overridden")]
    public bool Overridden { get; set; }

    [XmlArray("properties")]
    [XmlArrayItem("property")]
    public WorkItemTypeExtensionPropertyRecord[] Properties { get; set; }

    [XmlElement("disabled")]
    public bool Disabled { get; set; }
  }
}

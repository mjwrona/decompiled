// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemOobState
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  [XmlType("state")]
  public class WorkItemOobState
  {
    private WorkItemStateCategory? m_stateCategory;

    internal WorkItemOobState()
    {
    }

    internal WorkItemOobState(Guid Id, WorkItemStateCategory StateCategory)
    {
      this.Id = Id;
      this.m_stateCategory = new WorkItemStateCategory?(StateCategory);
    }

    [XmlAttribute("id")]
    public Guid Id { get; set; }

    [XmlAttribute("name")]
    public string Name { get; set; }

    [XmlAttribute("category")]
    public int CategoryFromXml { get; set; }

    public WorkItemStateCategory StateCategory
    {
      get
      {
        if (!this.m_stateCategory.HasValue)
          this.m_stateCategory = Enum.IsDefined(typeof (WorkItemStateCategory), (object) this.CategoryFromXml) ? new WorkItemStateCategory?((WorkItemStateCategory) this.CategoryFromXml) : throw new InvalidCastException(string.Format("The state value : {0} obtained from the xml is not one of the supported values for StateCategory enum", (object) this.CategoryFromXml.ToString()));
        return this.m_stateCategory.Value;
      }
      set => this.m_stateCategory = new WorkItemStateCategory?(value);
    }
  }
}

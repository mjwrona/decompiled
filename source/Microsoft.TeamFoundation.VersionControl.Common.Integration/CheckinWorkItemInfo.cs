// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Common.CheckinWorkItemInfo
// Assembly: Microsoft.TeamFoundation.VersionControl.Common.Integration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2A9D748-4CEE-4498-9785-584B91A44F85
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Common.Integration.dll

using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Common
{
  public class CheckinWorkItemInfo
  {
    [XmlAttribute("Url")]
    public string WorkItemUrl;
    [XmlAttribute("Id")]
    public int Id;
    [XmlAttribute("CheckinAction")]
    public CheckinWorkItemCheckinAction CheckinAction;
    [XmlAttribute("Title")]
    public string Title;
    [XmlAttribute("Type")]
    public string Type;
    [XmlAttribute("State")]
    public string State;
    [XmlAttribute("AssignedTo")]
    public string AssignedTo;

    public CheckinWorkItemInfo()
    {
    }

    public CheckinWorkItemInfo(int id, CheckinWorkItemCheckinAction checkinAction)
    {
      this.Id = id;
      this.CheckinAction = checkinAction;
    }
  }
}

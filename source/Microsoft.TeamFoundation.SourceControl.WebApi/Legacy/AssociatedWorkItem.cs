// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.AssociatedWorkItem
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi.Legacy
{
  [DataContract]
  public class AssociatedWorkItem : VersionControlSecuredObject
  {
    [DataMember(Name = "id")]
    public int Id { get; set; }

    [DataMember(Name = "workItemType", EmitDefaultValue = false)]
    public string WorkItemType { get; set; }

    [DataMember(Name = "title", EmitDefaultValue = false)]
    public string Title { get; set; }

    [DataMember(Name = "state", EmitDefaultValue = false)]
    public string State { get; set; }

    [DataMember(Name = "assignedTo", EmitDefaultValue = false)]
    public string AssignedTo { get; set; }
  }
}

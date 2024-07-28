// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemArtifactLink
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels
{
  [DataContract]
  public class WorkItemArtifactLink : BaseSecuredObject
  {
    public WorkItemArtifactLink(WorkItemResourceLinkInfo link, ISecuredObject securedObject)
      : base(securedObject)
    {
      this.Location = link.Location;
      this.Name = link.Name;
      this.ResourceLinkType = link.ResourceType;
      this.Comment = link.Comment;
    }

    [DataMember(Name = "location")]
    public string Location { get; set; }

    [DataMember(Name = "name")]
    public string Name { get; set; }

    [DataMember(Name = "resourceLinkType")]
    public ResourceLinkType ResourceLinkType { get; set; }

    [DataMember(Name = "comment")]
    public string Comment { get; set; }
  }
}

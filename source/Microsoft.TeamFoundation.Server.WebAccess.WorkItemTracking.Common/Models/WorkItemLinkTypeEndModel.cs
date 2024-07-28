// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.WorkItemLinkTypeEndModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models
{
  [DataContract]
  public class WorkItemLinkTypeEndModel : BaseSecuredObjectModel
  {
    public WorkItemLinkTypeEndModel(WorkItemLinkTypeEnd linkTypeEnd, ISecuredObject securedObject)
      : base(securedObject)
    {
      this.Id = linkTypeEnd.Id;
      this.ImmutableName = linkTypeEnd.ImmutableName;
      this.IsForwardLink = linkTypeEnd.IsForwardLink;
      this.Name = linkTypeEnd.Name;
    }

    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public string ImmutableName { get; set; }

    [DataMember]
    public bool IsForwardLink { get; set; }

    [DataMember]
    public string Name { get; set; }
  }
}

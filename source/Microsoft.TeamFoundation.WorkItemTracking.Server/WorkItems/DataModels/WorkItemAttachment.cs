// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemAttachment
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels
{
  [DataContract]
  public class WorkItemAttachment : BaseSecuredObject
  {
    public WorkItemAttachment(
      IVssRequestContext requestContext,
      WorkItemResourceLinkInfo link,
      WorkItem workItem)
      : base((ISecuredObject) workItem)
    {
      try
      {
        Guid projectGuid = workItem.GetProjectGuid(requestContext);
        this.Url = requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "wit", WitConstants.WorkItemTrackingLocationIds.Attachments, (object) new
        {
          id = link.Location,
          fileName = link.Name,
          project = projectGuid
        }).AbsoluteUri;
      }
      catch (Exception ex)
      {
      }
      this.Id = link.Location;
      this.FileName = link.Name;
      this.Comment = link.Comment;
      this.Size = link.ResourceSize;
    }

    [DataMember(Name = "id")]
    public string Id { get; set; }

    [DataMember(Name = "fileName")]
    public string FileName { get; set; }

    [DataMember(Name = "comment")]
    public string Comment { get; set; }

    [DataMember(Name = "size")]
    public int Size { get; set; }

    [DataMember(Name = "url")]
    public string Url { get; set; }
  }
}

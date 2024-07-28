// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTemplateDescriptor
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class WorkItemTemplateDescriptor
  {
    public Guid Id { get; }

    public string Name { get; }

    public string Description { get; }

    public string WorkItemTypeName { get; protected set; }

    public Guid OwnerId { get; }

    public Guid ProjectId { get; }

    public WorkItemTemplateDescriptor(
      Guid id,
      string name,
      string description,
      string workItemTypeName,
      Guid ownerId,
      Guid projectId)
    {
      this.Id = id;
      this.Name = name;
      this.Description = description;
      this.WorkItemTypeName = workItemTypeName;
      this.OwnerId = ownerId;
      this.ProjectId = projectId;
    }
  }
}

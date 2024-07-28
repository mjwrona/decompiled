// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTypeCategory
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  public class WorkItemTypeCategory : ProcessReadSecuredObject
  {
    public WorkItemTypeCategory(
      string name,
      string referenceName,
      IEnumerable<string> workItemTypeNames,
      string defaultWorkItemTypeName)
    {
      this.Name = name;
      this.ReferenceName = referenceName;
      this.DefaultWorkItemTypeName = defaultWorkItemTypeName;
      this.WorkItemTypeNames = workItemTypeNames;
      this.WorkItemTypeCategoryMembers = workItemTypeNames.Select<string, WorkItemTypeCategoryMember>((Func<string, WorkItemTypeCategoryMember>) (t => new WorkItemTypeCategoryMember(t)));
    }

    public WorkItemTypeCategory(
      Guid projectId,
      int? id,
      string name,
      string referenceName,
      IEnumerable<string> workItemTypeNames,
      string defaultWorkItemTypeName)
      : this(name, referenceName, workItemTypeNames, defaultWorkItemTypeName)
    {
      this.ProjectId = projectId;
      this.Id = id;
    }

    internal WorkItemTypeCategory(WorkItemTypeCategoryRecord categoryRecord, Guid projectId)
    {
      this.Id = new int?(categoryRecord.Id);
      this.Name = categoryRecord.Name;
      this.ReferenceName = categoryRecord.ReferenceName;
      this.DefaultWorkItemTypeName = categoryRecord.DefaultWorkItemTypeName;
      this.ProjectId = projectId;
      if (categoryRecord.WorkItemTypeMembers != null)
      {
        this.WorkItemTypeCategoryMembers = categoryRecord.WorkItemTypeMembers.Select<WorkItemTypeCategoryMemberRecord, WorkItemTypeCategoryMember>((Func<WorkItemTypeCategoryMemberRecord, WorkItemTypeCategoryMember>) (m => new WorkItemTypeCategoryMember(m.Id, m.WorkItemTypeName)));
        this.WorkItemTypeNames = categoryRecord.WorkItemTypeMembers.Select<WorkItemTypeCategoryMemberRecord, string>((Func<WorkItemTypeCategoryMemberRecord, string>) (m => m.WorkItemTypeName));
      }
      else
        this.WorkItemTypeNames = categoryRecord.WorkItemTypeNames;
    }

    public WorkItemTypeCategory ShallowCopy() => (WorkItemTypeCategory) this.MemberwiseClone();

    public int? Id { get; private set; }

    public string Name { get; private set; }

    public string ReferenceName { get; private set; }

    public string DefaultWorkItemTypeName { get; private set; }

    public IEnumerable<string> WorkItemTypeNames { get; private set; }

    public IEnumerable<WorkItemTypeCategoryMember> WorkItemTypeCategoryMembers { get; private set; }

    public Guid ProjectId { get; set; }
  }
}

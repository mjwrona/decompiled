// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.WorkItem.WorkItemSearchHit
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.WorkItem
{
  public class WorkItemSearchHit : SearchHit
  {
    public WorkItemSearchHit() => this.Hits = Enumerable.Empty<WorkItemHit>();

    public WorkItemSearchHit(
      IEnumerable<WorkItemHit> hits,
      IDictionary<string, IEnumerable<string>> fields)
      : base(fields)
    {
      this.Hits = hits;
    }

    public IEnumerable<WorkItemHit> Hits { get; }

    internal static WorkItemResult CreateWorkItemResult(WorkItemSearchHit platformHit)
    {
      string fieldValue1 = platformHit.GetFieldValue("projectName");
      string fieldValue2 = platformHit.GetFieldValue("projectId");
      string fieldValue3 = platformHit.GetFieldValue(WorkItemContract.ServiceFieldNames.Id, string.Empty);
      string fieldValue4 = platformHit.GetFieldValue(WorkItemContract.ServiceFieldNames.Type, string.Empty);
      string fieldValue5 = platformHit.GetFieldValue(WorkItemContract.ServiceFieldNames.Title, string.Empty);
      string fieldValue6 = platformHit.GetFieldValue(WorkItemContract.ServiceFieldNames.AssignedTo, string.Empty);
      string fieldValue7 = platformHit.GetFieldValue(WorkItemContract.ServiceFieldNames.State, string.Empty);
      string fieldValue8 = platformHit.GetFieldValue(WorkItemContract.ServiceFieldNames.Tags, string.Empty);
      string fieldValue9 = platformHit.GetFieldValue(WorkItemContract.ServiceFieldNames.Revision, string.Empty);
      string fieldValue10 = platformHit.GetFieldValue(WorkItemContract.ServiceFieldNames.CreatedDate, string.Empty);
      string fieldValue11 = platformHit.GetFieldValue(WorkItemContract.ServiceFieldNames.ChangedDate, string.Empty);
      List<WorkItemField> workItemFieldList = new List<WorkItemField>()
      {
        new WorkItemField()
        {
          ReferenceName = WorkItemContract.ServiceFieldNames.Id,
          Value = fieldValue3
        },
        new WorkItemField()
        {
          ReferenceName = WorkItemContract.ServiceFieldNames.Type,
          Value = fieldValue4
        },
        new WorkItemField()
        {
          ReferenceName = WorkItemContract.ServiceFieldNames.Title,
          Value = fieldValue5
        },
        new WorkItemField()
        {
          ReferenceName = WorkItemContract.ServiceFieldNames.AssignedTo,
          Value = fieldValue6
        },
        new WorkItemField()
        {
          ReferenceName = WorkItemContract.ServiceFieldNames.State,
          Value = fieldValue7
        },
        new WorkItemField()
        {
          ReferenceName = WorkItemContract.ServiceFieldNames.Tags,
          Value = fieldValue8
        },
        new WorkItemField()
        {
          ReferenceName = WorkItemContract.ServiceFieldNames.Revision,
          Value = fieldValue9
        },
        new WorkItemField()
        {
          ReferenceName = WorkItemContract.ServiceFieldNames.CreatedDate,
          Value = fieldValue10
        },
        new WorkItemField()
        {
          ReferenceName = WorkItemContract.ServiceFieldNames.ChangedDate,
          Value = fieldValue11
        }
      };
      return new WorkItemResult()
      {
        Project = fieldValue1,
        ProjectId = fieldValue2,
        Fields = (IEnumerable<WorkItemField>) workItemFieldList,
        Hits = platformHit.Hits
      };
    }
  }
}

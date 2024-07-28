// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.ConverterUtilities
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Microsoft.TeamFoundation.CodeSense.Platform.Abstraction;
using Microsoft.TeamFoundation.Framework.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.CodeSense.Server
{
  public static class ConverterUtilities
  {
    public static IEnumerable<WorkItemData> GetWorkItems(
      IEnumerable<int> workItemIds,
      SourceControlDataV3 sourceControlData)
    {
      List<WorkItemData> workItemDataList = new List<WorkItemData>();
      if (workItemIds != null && sourceControlData != null)
      {
        foreach (WorkItemDataV3 workItemDataV3 in workItemIds.Select<int, WorkItemDataV3>((Func<int, WorkItemDataV3>) (w => sourceControlData.FindWorkItem(w))).Where<WorkItemDataV3>((Func<WorkItemDataV3, bool>) (w => w != null)))
        {
          UserData user1 = workItemDataV3.CreatedBy != null ? sourceControlData.FindUser(workItemDataV3.CreatedBy) : (UserData) null;
          UserData user2 = workItemDataV3.AssignedTo != null ? sourceControlData.FindUser(workItemDataV3.AssignedTo) : (UserData) null;
          workItemDataList.Add(new WorkItemData(workItemDataV3.Id, workItemDataV3.Title, workItemDataV3.Type, workItemDataV3.Category, workItemDataV3.State, user2, user1, workItemDataV3.LastModified));
        }
      }
      return workItemDataList.Count <= 0 ? (IEnumerable<WorkItemData>) null : (IEnumerable<WorkItemData>) workItemDataList;
    }

    public static void FillBranchLinks(
      IEnumerable<BranchLinkData> branchLinks,
      SourceControlDataV3 sourceControlData)
    {
      if (branchLinks == null || sourceControlData == null)
        return;
      foreach (BranchLinkData branchLink in branchLinks)
      {
        CommitDataV3 changeset = sourceControlData.FindChangeset(branchLink.TargetChangesId);
        if (changeset != null)
        {
          branchLink.ChangesComment = changeset.ChangesComment;
          branchLink.Date = changeset.Date;
          UserData user = sourceControlData.FindUser(changeset.AuthorUniqueName);
          if (user != null)
          {
            branchLink.AuthorDisplayName = user.DisplayName;
            branchLink.AuthorUniqueName = user.UniqueName;
            branchLink.AuthorEmail = user.Email;
          }
        }
      }
    }

    public static void UpdateBranchMapData<T>(
      IVssRequestContext requestContext,
      IEnumerable<T> branchLinks,
      ProjectMapCache projectMapCache)
      where T : IBranchMap
    {
      if (branchLinks == null)
        return;
      T obj;
      foreach (T branchLink in branchLinks)
      {
        if (branchLink.SourcePath.GetProjectName().Equals(branchLink.TargetPath.GetProjectName(), StringComparison.OrdinalIgnoreCase))
        {
          Guid projectId = ProjectServiceHelper.GetProjectId(requestContext, branchLink.SourcePath.GetProjectName(), projectMapCache);
          if (!projectId.Equals(Guid.Empty))
          {
            ref T local1 = ref branchLink;
            obj = default (T);
            if ((object) obj == null)
            {
              obj = local1;
              local1 = ref obj;
            }
            string str1 = branchLink.SourcePath.ReplaceProjectName(projectId.ToString());
            local1.SourcePath = str1;
            ref T local2 = ref branchLink;
            obj = default (T);
            if ((object) obj == null)
            {
              obj = local2;
              local2 = ref obj;
            }
            string str2 = branchLink.TargetPath.ReplaceProjectName(projectId.ToString());
            local2.TargetPath = str2;
          }
        }
        else
        {
          ref T local3 = ref branchLink;
          obj = default (T);
          if ((object) obj == null)
          {
            obj = local3;
            local3 = ref obj;
          }
          string str3 = branchLink.SourcePath.ReplaceProjectNameWithGuid(requestContext, projectMapCache);
          local3.SourcePath = str3;
          ref T local4 = ref branchLink;
          obj = default (T);
          if ((object) obj == null)
          {
            obj = local4;
            local4 = ref obj;
          }
          string str4 = branchLink.TargetPath.ReplaceProjectNameWithGuid(requestContext, projectMapCache);
          local4.TargetPath = str4;
        }
      }
    }

    public static void ReplaceServerPathsWithGuid<T>(
      IVssRequestContext requestContext,
      List<T> branchList,
      ProjectMapCache projectMapCache)
      where T : IBranchResult
    {
      if (branchList == null)
        return;
      foreach (T branch in branchList)
      {
        ref T local = ref branch;
        T obj = default (T);
        if ((object) obj == null)
        {
          obj = local;
          local = ref obj;
        }
        string str = branch.ServerPath.ReplaceProjectNameWithGuid(requestContext, projectMapCache);
        local.ServerPath = str;
      }
    }

    public static IDictionary<string, HashSet<int>> ReplaceServerPathsInChanges(
      IVssRequestContext requestContext,
      IDictionary<string, HashSet<int>> includedChanges,
      ProjectMapCache projectMapCache)
    {
      IDictionary<string, HashSet<int>> dictionary = (IDictionary<string, HashSet<int>>) new Dictionary<string, HashSet<int>>();
      if (includedChanges != null && includedChanges.Any<KeyValuePair<string, HashSet<int>>>())
      {
        foreach (string key in (IEnumerable<string>) includedChanges.Keys)
          dictionary.Add(key.ReplaceProjectNameWithGuid(requestContext, projectMapCache), includedChanges[key]);
      }
      return dictionary;
    }

    public static T Deserialize<T>(IVssRequestContext requestContext, string content)
    {
      try
      {
        return JsonConvert.DeserializeObject<T>(content, CodeSenseSerializationSettings.JsonSerializerSettings);
      }
      catch (JsonException ex)
      {
        requestContext.Trace(1025037, TraceLevel.Info, "CodeSense", TraceLayer.Job, "Exception while deserializing json: {0}", (object) ex);
      }
      catch (OutOfMemoryException ex)
      {
        requestContext.TraceException(1025040, "CodeSense", TraceLayer.Job, (Exception) ex);
      }
      return default (T);
    }

    public static void FetchData(
      this SourceControlDataV3 sourceControlData,
      CodeElementChangeResult data)
    {
      if (data == null || sourceControlData.FindChangeset(data.ChangesId) != null)
        return;
      IEnumerable<WorkItemDataV3> workItemDataV3s = data.WorkItems != null ? data.WorkItems.Select<WorkItemData, WorkItemDataV3>((Func<WorkItemData, WorkItemDataV3>) (w => new WorkItemDataV3(w))) : (IEnumerable<WorkItemDataV3>) null;
      CommitDataV3 changeset = new CommitDataV3(data.ChangesId, data.ChangesComment, data.AuthorUniqueName, data.Date, workItemDataV3s != null ? workItemDataV3s.Select<WorkItemDataV3, int>((Func<WorkItemDataV3, int>) (w => w.Id)) : (IEnumerable<int>) null);
      UserData user = new UserData(data.AuthorUniqueName, data.AuthorDisplayName, data.AuthorEmail);
      sourceControlData.UpdateChangeset(changeset);
      sourceControlData.UpdateWorkItems(workItemDataV3s);
      sourceControlData.UpdateUser(user);
      if (data.WorkItems == null)
        return;
      foreach (WorkItemData workItem in data.WorkItems)
      {
        if (workItem.CreatedBy != null)
          sourceControlData.UpdateUser(workItem.CreatedBy);
        if (workItem.AssignedTo != null)
          sourceControlData.UpdateUser(workItem.AssignedTo);
      }
    }
  }
}

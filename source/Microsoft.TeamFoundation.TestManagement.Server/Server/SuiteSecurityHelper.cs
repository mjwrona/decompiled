// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.SuiteSecurityHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class SuiteSecurityHelper
  {
    internal static void CheckClonePermissions(
      TestManagementRequestContext context,
      GuidAndString projectId,
      int fromSuiteId,
      GuidAndString targetProjectId,
      int destSuiteId,
      CloneOptions options,
      IEnumerable<int> childSuiteEntries = null)
    {
      List<int> suiteIds1 = new List<int>() { fromSuiteId };
      if (childSuiteEntries != null)
        suiteIds1.AddRange(childSuiteEntries);
      SuiteSecurityHelper.CheckTestSuiteReadPermission(context, (IEnumerable<int>) suiteIds1);
      List<int> suiteIds2 = new List<int>() { destSuiteId };
      if (options == null || !options.ResolvedFieldDetails.Any<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (fd => string.CompareOrdinal(fd.Key, WorkItemFieldRefNames.AreaId) == 0)))
      {
        if (childSuiteEntries != null)
          suiteIds2.AddRange(childSuiteEntries);
      }
      else
      {
        WITCreator witCreator = new WITCreator(context);
        KeyValuePair<string, string> keyValuePair = options.ResolvedFieldDetails.Where<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (fd => string.CompareOrdinal(fd.Key, WorkItemFieldRefNames.AreaId) == 0)).FirstOrDefault<KeyValuePair<string, string>>();
        string areaUri = string.Empty;
        if (options.OverrideFieldDetails != null && options.OverrideFieldDetails.Count > 0)
        {
          foreach (NameValuePair overrideFieldDetail in options.OverrideFieldDetails)
          {
            string resolvedFieldValue = string.Empty;
            string resolvedFieldId;
            witCreator.ResolveFieldNameAndValue(overrideFieldDetail.Name, overrideFieldDetail.Value, targetProjectId, true, out resolvedFieldId, out resolvedFieldValue);
            if (string.CompareOrdinal(resolvedFieldId, keyValuePair.Key) == 0 && resolvedFieldValue != null && string.Equals(keyValuePair.Value, resolvedFieldValue.ToString()))
            {
              areaUri = context.AreaPathsCache.GetCssNodeAndThrow(context, overrideFieldDetail.Value).Uri;
              break;
            }
          }
        }
        else if (!string.IsNullOrEmpty(options.OverrideFieldName) && !string.IsNullOrEmpty(options.OverrideFieldValue))
        {
          areaUri = context.AreaPathsCache.GetCssNodeAndThrow(context, options.OverrideFieldValue).Uri;
        }
        else
        {
          if (string.IsNullOrEmpty(options.OverrideFieldName))
            throw new TestManagementValidationException(ServerResources.DeepCopyFieldOverriddenFieldNameEmpty);
          throw new TestManagementValidationException(ServerResources.FieldValueMissing);
        }
        context.SecurityManager.CheckManageTestSuitesPermission(context, areaUri);
        context.SecurityManager.CheckWorkItemWritePermission(context, areaUri);
      }
      SuiteSecurityHelper.CheckTestSuiteUpdatePermission(context, (IEnumerable<int>) suiteIds2, true);
    }

    private static string QueryPlanAreaUriBySuite(
      TestManagementRequestContext context,
      TestPlanningDatabase db,
      int projectId,
      int suiteId)
    {
      try
      {
        context.TraceEnter("BusinessLayer", "ServerTestSuite.QueryPlanAreaUriBySuite");
        return db.QueryPlanAreaUriBySuite(projectId, suiteId);
      }
      finally
      {
        context.TraceLeave("BusinessLayer", "ServerTestSuite.QueryPlanAreaUriBySuite");
      }
    }

    internal static void CheckTestSuiteUpdatePermission(
      TestManagementRequestContext context,
      IEnumerable<int> suiteIds,
      bool checkWorkItemWritePermission = false)
    {
      foreach (string distinctSuiteAreaUri in SuiteSecurityHelper.GetDistinctSuiteAreaUris(context, suiteIds))
      {
        context.SecurityManager.CheckManageTestSuitesPermission(context, distinctSuiteAreaUri);
        if (checkWorkItemWritePermission)
          context.SecurityManager.CheckWorkItemWritePermission(context, distinctSuiteAreaUri);
      }
    }

    private static void CheckTestSuiteReadPermission(
      TestManagementRequestContext context,
      IEnumerable<int> suiteIds)
    {
      foreach (string distinctSuiteAreaUri in SuiteSecurityHelper.GetDistinctSuiteAreaUris(context, suiteIds))
        context.SecurityManager.CheckWorkItemReadPermission(context, distinctSuiteAreaUri);
    }

    private static IEnumerable<string> GetDistinctSuiteAreaUris(
      TestManagementRequestContext context,
      IEnumerable<int> suiteIds)
    {
      IWitHelper witHelper = context.RequestContext.GetService<IWitHelper>();
      if (suiteIds == null || suiteIds.Count<int>() <= 0)
        return (IEnumerable<string>) new List<string>();
      IEnumerable<WorkItem> workItems = witHelper.GetWorkItems(context.RequestContext, suiteIds.ToList<int>(), new List<string>()
      {
        "System.AreaPath"
      }, true);
      List<string> source = new List<string>();
      foreach (WorkItem workItem in workItems)
      {
        string str;
        if (workItem.Fields.TryGetValue<string, string>("System.AreaPath", out str) && !string.IsNullOrEmpty(str))
          source.Add(str);
      }
      return source.Select<string, string>((Func<string, string>) (ap => witHelper.AreaPathToUri(context, ap)));
    }
  }
}

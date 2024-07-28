// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.SecurityManager
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class SecurityManager : DefaultSecurityManager
  {
    internal SecurityManager()
    {
    }

    public override bool HasGenericReadPermission(TestManagementRequestContext context) => this.HasCollectionPermission(context, AuthorizationNamespacePermissions.GenericRead);

    public override void CheckManageTestControllersPermission(TestManagementRequestContext context)
    {
      if (!this.HasManageTestControllersPermission(context))
        throw new AccessDeniedException(ServerResources.CannotManageTestControllers);
    }

    public override bool HasManageTestControllersPermission(TestManagementRequestContext context) => this.HasCollectionPermission(context, AuthorizationNamespacePermissions.ManageTestControllers);

    public override void CheckManageTestEnvironmentsPermission(
      TestManagementRequestContext context,
      string projectUri)
    {
      if (!this.HasManageTestEnvironmentsPermission(context, projectUri))
        throw new AccessDeniedException(ServerResources.CannotManageTestEnvironments);
    }

    public override bool HasManageTestEnvironmentsPermission(
      TestManagementRequestContext context,
      string projectUri)
    {
      return this.HasProjectPermission(context, projectUri, AuthorizationProjectPermissions.ManageTestEnvironments);
    }

    public override void CheckManageTestConfigurationsPermission(
      TestManagementRequestContext context,
      string projectUri)
    {
      if (!this.HasManageTestConfigurationsPermission(context, projectUri))
        throw new AccessDeniedException(ServerResources.CannotManageTestConfigurations);
    }

    public override bool HasManageTestConfigurationsPermission(
      TestManagementRequestContext context,
      string projectUri)
    {
      return this.HasProjectPermission(context, projectUri, AuthorizationProjectPermissions.ManageTestConfigurations);
    }

    public override void CheckManageTestPlansPermission(
      TestManagementRequestContext context,
      string areaUri)
    {
      if (this.HasManageTestPlansPermission(context, areaUri))
        return;
      this.CheckForViewNodeAndThrow(context, areaUri, ServerResources.CannotManageTestPlans);
    }

    private bool HasManageTestPlansPermission(TestManagementRequestContext context, string areaUri) => this.HasAreaPathPermission(context, areaUri, AuthorizationCssNodePermissions.ManageTestPlans);

    public override void CheckManageTestSuitesPermission(
      TestManagementRequestContext context,
      string areaUri)
    {
      if (this.HasManageTestSuitesPermission(context, areaUri))
        return;
      this.CheckForViewNodeAndThrow(context, areaUri, ServerResources.CannotManageTestSuites);
    }

    private bool HasManageTestSuitesPermission(TestManagementRequestContext context, string areaUri) => this.HasAreaPathPermission(context, areaUri, AuthorizationCssNodePermissions.ManageTestSuites);

    private bool HasViewNodePermission(TestManagementRequestContext context, string areaUri) => this.HasAreaPathPermission(context, areaUri, AuthorizationCssNodePermissions.GenericRead);

    public override void CheckForViewNodeAndThrow(
      TestManagementRequestContext context,
      string areaUri,
      string exceptionMessage)
    {
      string empty = string.Empty;
      string workItemPathFromUri;
      if (this.HasViewNodePermission(context, areaUri) && !string.IsNullOrEmpty(workItemPathFromUri = context.CSSHelper.GetWorkItemPathFromUri(areaUri)))
        throw new AccessDeniedException(string.Format(ServerResources.AccessDeniedExceptionMessage, (object) exceptionMessage, (object) workItemPathFromUri));
      throw new AccessDeniedException(exceptionMessage);
    }

    public override bool IsJobAgent(TestManagementRequestContext context) => context.RequestContext.IsHostProcessType(HostProcessType.JobAgent);

    public override bool IsServiceAccount(TestManagementRequestContext context)
    {
      using (PerfManager.Measure(context.RequestContext, "CrossService", TraceUtils.GetActionName(nameof (IsServiceAccount), "Identity")))
        return this.IsJobAgent(context) || IdentityDescriptorComparer.Instance.Equals(GroupWellKnownIdentityDescriptors.ServiceUsersGroup, context.UserPrincipal) || context.RequestContext.GetService<TeamFoundationIdentityService>().IsMember(context.RequestContext, GroupWellKnownIdentityDescriptors.ServiceUsersGroup, context.UserPrincipal);
    }

    public override void CheckServiceAccount(TestManagementRequestContext context)
    {
      if (!this.IsServiceAccount(context))
        throw new AccessDeniedException(ServerResources.ServiceAccountRequired);
    }

    public override void CheckWorkItemWritePermission(
      TestManagementRequestContext context,
      string areaUri)
    {
      if (this.HasWorkItemWritePermission(context, areaUri))
        return;
      this.CheckForViewNodeAndThrow(context, areaUri, ServerResources.CannotEditWorkItems);
    }

    private bool HasWorkItemWritePermission(TestManagementRequestContext context, string areaUri) => this.HasAreaPathPermission(context, areaUri, AuthorizationCssNodePermissions.WorkItemWrite, false);

    public override void CheckWorkItemReadPermission(
      TestManagementRequestContext context,
      string areaUri)
    {
      if (this.HasWorkItemReadPermission(context, areaUri))
        return;
      this.CheckForViewNodeAndThrow(context, areaUri, ServerResources.CannotViewWorkItems);
    }

    public override bool HasWorkItemReadPermission(
      TestManagementRequestContext context,
      string areaUri)
    {
      return this.HasAreaPathPermission(context, areaUri, AuthorizationCssNodePermissions.WorkItemRead, false);
    }

    public override List<T> FilterViewWorkItemOnAreaPath<T>(
      TestManagementRequestContext context,
      IEnumerable<KeyValuePair<int, T>> items,
      ITestManagementWorkItemCacheService workItemCacheService = null)
    {
      try
      {
        context.TraceEnter("BusinessLayer", "SecurityManager.FilterViewWorkItemOnAreaPath");
        using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "SecurityManager.FilterViewWorkItemOnAreaPath"))
        {
          IList<KeyValuePair<int, T>> list = (IList<KeyValuePair<int, T>>) items.ToList<KeyValuePair<int, T>>();
          IDictionary<int, string> dictionary1 = (IDictionary<int, string>) new Dictionary<int, string>();
          int count = list.Count;
          IWitHelper service = context.RequestContext.GetService<IWitHelper>();
          IDictionary<int, string> dictionary2 = workItemCacheService == null ? service.GetWorkItemAreaUris((TestManagementRequestContext) (context as TfsTestManagementRequestContext), list.Select<KeyValuePair<int, T>, int>((Func<KeyValuePair<int, T>, int>) (kvp => kvp.Key)), false) : this.GetAreaUriForWorkItemsFromCache(context, (IList<int>) items.Select<KeyValuePair<int, T>, int>((Func<KeyValuePair<int, T>, int>) (i => i.Key)).ToList<int>(), workItemCacheService);
          Dictionary<string, bool> dictionary3 = new Dictionary<string, bool>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          for (int index = 0; index < list.Count; ++index)
          {
            IDictionary<int, string> dictionary4 = dictionary2;
            KeyValuePair<int, T> keyValuePair = list[index];
            int key = keyValuePair.Key;
            string str;
            ref string local = ref str;
            if (dictionary4.TryGetValue(key, out local))
            {
              keyValuePair = list[index];
              if (keyValuePair.Value is IAreaUriProperty areaUriProperty && string.IsNullOrEmpty(areaUriProperty.AreaUri))
                areaUriProperty.AreaUri = str;
            }
            else
            {
              list.RemoveAt(index);
              --index;
            }
          }
          context.TraceInfo("BusinessLayer", "Filtered out {0} items. Returning {1} items.", (object) (count - list.Count), (object) list.Count);
          return list.Select<KeyValuePair<int, T>, T>((Func<KeyValuePair<int, T>, T>) (kvp => kvp.Value)).ToList<T>();
        }
      }
      finally
      {
        context.TraceLeave("BusinessLayer", "SecurityManager.FilterViewWorkItemOnAreaPath");
      }
    }

    private IDictionary<int, string> GetAreaUriForWorkItemsFromCache(
      TestManagementRequestContext context,
      IList<int> workItemIds,
      ITestManagementWorkItemCacheService workItemCacheService)
    {
      Dictionary<int, string> dictionary1 = new Dictionary<int, string>();
      IDictionary<int, string> validatedIdAreaUriMap = (IDictionary<int, string>) new Dictionary<int, string>();
      List<int> intList = new List<int>();
      HashSet<string> stringSet = new HashSet<string>();
      IWitHelper service = context.RequestContext.GetService<IWitHelper>();
      foreach (int workItemId in (IEnumerable<int>) workItemIds)
      {
        CachedWorkItemData cachedWorkItem;
        if (workItemCacheService.TryGetCachedWorkItemData(context.RequestContext, workItemId, out cachedWorkItem))
        {
          dictionary1.Add(workItemId, cachedWorkItem.AreaUri);
          stringSet.Add(cachedWorkItem.AreaUri);
        }
        else
          intList.Add(workItemId);
      }
      Dictionary<string, bool> dictionary2 = new Dictionary<string, bool>();
      foreach (string str in stringSet)
      {
        bool flag = context.SecurityManager.HasWorkItemReadPermission(context, str);
        dictionary2.Add(str, flag);
      }
      foreach (int key in dictionary1.Keys)
      {
        if (dictionary2[dictionary1[key]])
          validatedIdAreaUriMap.Add(key, dictionary1[key]);
      }
      if (intList.Any<int>())
      {
        IDictionary<int, CachedWorkItemData> fetchedIdAreaMap = service.GetWorkItemCacheData((TestManagementRequestContext) (context as TfsTestManagementRequestContext), (IEnumerable<int>) intList, false);
        workItemCacheService.TryUpdateWorkItemCache(context.RequestContext, fetchedIdAreaMap.Values.ToList<CachedWorkItemData>());
        fetchedIdAreaMap.Keys.ToList<int>().ForEach((Action<int>) (k => validatedIdAreaUriMap.Add(k, fetchedIdAreaMap[k].AreaUri)));
      }
      return validatedIdAreaUriMap;
    }

    public override void FilterViewWorkItemOptional<T>(
      TestManagementRequestContext context,
      IList<T> list)
    {
      if (context.RequestContext.IsFeatureEnabled("TestManagement.Server.RemoveFilterTestCaseResultsByViewWorkItem"))
        return;
      int count = list.Count;
      Dictionary<string, bool> dictionary = new Dictionary<string, bool>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      for (int index = 0; index < list.Count; ++index)
      {
        IAreaUriProperty areaUriProperty = (IAreaUriProperty) (object) list[index];
        bool flag = string.IsNullOrEmpty(areaUriProperty.AreaUri);
        if (!flag && !dictionary.TryGetValue(areaUriProperty.AreaUri, out flag))
        {
          flag = this.HasWorkItemReadPermission(context, areaUriProperty.AreaUri);
          dictionary.Add(areaUriProperty.AreaUri, flag);
        }
        if (!flag)
        {
          list.RemoveAt(index);
          --index;
        }
      }
      context.RequestContext.Trace(1015096, TraceLevel.Info, "TestManagement", "BusinessLayer", "Filtered out {0} items. Returning {1} items.", (object) (count - list.Count), (object) list.Count);
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.TfWorkItemFactory
// Assembly: Microsoft.TeamFoundation.CodeSense.Platform.OnPrem, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B5DEEFA-3C5E-4BFB-92E2-3ADDA47952C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Platform.OnPrem.dll

using Microsoft.TeamFoundation.CodeSense.Platform.Abstraction;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.CodeSense.Server
{
  public sealed class TfWorkItemFactory : TfWorkItemFactoryBase
  {
    private const string UnknownWorkItemTypeCategory = "Microsoft.UnknownCategory";
    private static readonly int[] RequiredFields = new int[9]
    {
      -3,
      1,
      33,
      25,
      2,
      24,
      -4,
      -2,
      -7
    };
    private Dictionary<int, List<WorkItemTypeCategory>> workItemTypeCategoryCache = new Dictionary<int, List<WorkItemTypeCategory>>();

    private TfWorkItemFactory(IVssRequestContext requestContext)
      : base(requestContext, TfWorkItemFactory.RequiredFields)
    {
    }

    public static IEnumerable<WorkItemData> Create(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds,
      DateTime? asOf)
    {
      TfWorkItemFactory tfWorkItemFactory = new TfWorkItemFactory(requestContext);
      IEnumerable<WorkItemFieldData> workItems = tfWorkItemFactory.PageWorkItems(workItemIds, asOf);
      return (IEnumerable<WorkItemData>) tfWorkItemFactory.CreateWorkItemData(requestContext, workItems).ToArray<WorkItemData>();
    }

    private string LookupWorkItemCategory(
      IVssRequestContext requestContext,
      int areaPathId,
      string workItemType)
    {
      TreeNode project = this.treeSnapshot.LegacyGetTreeNode(areaPathId).Project;
      if (!this.workItemTypeCategoryCache.ContainsKey(project.Id))
      {
        List<WorkItemTypeCategory> list = requestContext.GetService<WorkItemTypeCategoryService>().GetWorkItemTypeCategories(requestContext, project.CssNodeId).ToList<WorkItemTypeCategory>();
        this.workItemTypeCategoryCache.Add(project.Id, list);
      }
      string str = "Microsoft.UnknownCategory";
      WorkItemTypeCategory itemTypeCategory = this.workItemTypeCategoryCache[project.Id].FirstOrDefault<WorkItemTypeCategory>((Func<WorkItemTypeCategory, bool>) (category => category.WorkItemTypeNames.Contains<string>(workItemType, (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName)));
      if (itemTypeCategory != null)
        str = itemTypeCategory.ReferenceName;
      return str;
    }

    private IEnumerable<WorkItemData> CreateWorkItemData(
      IVssRequestContext requestContext,
      IEnumerable<WorkItemFieldData> workItems)
    {
      TfWorkItemFactory tfWorkItemFactory = this;
      foreach (WorkItemFieldData workItem in workItems)
      {
        int id = workItem.Id;
        string title = workItem.Title;
        string workItemType = workItem.WorkItemType;
        string state = workItem.State;
        DateTime modifiedDate = workItem.ModifiedDate;
        string assignedTo1 = workItem.AssignedTo;
        UserData assignedTo2 = tfWorkItemFactory.BuildUserData(requestContext, assignedTo1);
        string createdBy1 = workItem.CreatedBy;
        UserData createdBy2 = tfWorkItemFactory.BuildUserData(requestContext, createdBy1);
        int areaId = workItem.AreaId;
        string category = tfWorkItemFactory.LookupWorkItemCategory(requestContext, tfWorkItemFactory.GetLatestAreaId(id, areaId), workItemType);
        yield return new WorkItemData(id, title, workItemType, category, state, assignedTo2, createdBy2, new DateTime?(modifiedDate));
      }
    }

    private UserData BuildUserData(IVssRequestContext requestContext, string accountName)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      UserData userData = (UserData) null;
      if (!string.IsNullOrEmpty(accountName))
      {
        using (new CodeSenseTraceWatch(requestContext, 1025805, TraceLayer.ExternalFramework, "Reading identity '{0}'", new object[1]
        {
          (object) accountName
        }))
        {
          TeamFoundationIdentityService service = requestContext.GetService<TeamFoundationIdentityService>();
          if (service != null)
          {
            TeamFoundationIdentity foundationIdentity = (TeamFoundationIdentity) null;
            try
            {
              TeamFoundationIdentity[] source = ((IEnumerable<TeamFoundationIdentity[]>) service.ReadIdentities(requestContext, IdentitySearchFactor.DisplayName, new string[1]
              {
                accountName
              }, MembershipQuery.None, ReadIdentityOptions.None, (IEnumerable<string>) null, IdentityPropertyScope.Both)).First<TeamFoundationIdentity[]>();
              if (source.Length != 0)
                foundationIdentity = ((IEnumerable<TeamFoundationIdentity>) source).FirstOrDefault<TeamFoundationIdentity>((Func<TeamFoundationIdentity, bool>) (id => id.IsActive)) ?? source[0];
            }
            catch (Exception ex)
            {
              requestContext.TraceException(1024653, "CodeSense", TraceLayer.ExternalFramework, ex);
            }
            if (foundationIdentity != null)
            {
              using (new CodeSenseTraceWatch(requestContext, 1025810, TraceLayer.ExternalFramework, "Getting preferred email for id '{0}'", new object[1]
              {
                (object) foundationIdentity.TeamFoundationId
              }))
              {
                string preferredEmailAddress = service.GetPreferredEmailAddress(requestContext, foundationIdentity.TeamFoundationId);
                userData = new UserData(foundationIdentity.UniqueName, foundationIdentity.DisplayName, preferredEmailAddress);
              }
            }
          }
        }
      }
      return userData;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ProvisionHelper
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class ProvisionHelper
  {
    private IVssRequestContext m_requestContext;
    private WorkItemTrackingTreeService m_treeService;
    private IVssSecurityNamespace m_securityNamespace;
    private HashSet<int> m_projects;
    private HashSet<int> m_fields;
    private HashSet<int> m_fieldUsages;
    private HashSet<int> m_treeProperties;
    private HashSet<int> m_rules;
    private HashSet<int> m_constants;
    private HashSet<int> m_sets;
    private HashSet<int> m_wits;
    private HashSet<int> m_typeUsages;
    private HashSet<int> m_actions;
    private HashSet<int> m_categories;
    private HashSet<int> m_categoryMembers;

    public ProvisionHelper(IVssRequestContext requestContext)
    {
      this.m_requestContext = requestContext;
      this.m_projects = new HashSet<int>();
      this.m_fields = new HashSet<int>();
      this.m_fieldUsages = new HashSet<int>();
      this.m_treeProperties = new HashSet<int>();
      this.m_rules = new HashSet<int>();
      this.m_constants = new HashSet<int>();
      this.m_sets = new HashSet<int>();
      this.m_wits = new HashSet<int>();
      this.m_typeUsages = new HashSet<int>();
      this.m_actions = new HashSet<int>();
      this.m_categories = new HashSet<int>();
      this.m_categoryMembers = new HashSet<int>();
    }

    public void CheckClientVersion()
    {
      if (this.m_requestContext.GetClientVersion() < this.m_requestContext.GetService<WorkItemTrackingConfigurationSettingService>().GetConfigurationInfo(this.m_requestContext).MinClientVersionToProvision)
        throw new LegacyValidationException(DalResourceStrings.Get("CannotImportFromOlderClient"), 600171);
    }

    public void Authorize(bool requireAnyProjectPermission)
    {
      this.CheckClientVersion();
      if (this.SecurityNamespace.HasPermission(this.m_requestContext, "$", 1, false))
      {
        if (!this.HasFieldOperation)
          return;
      }
      else if (this.m_projects.Contains(0) || this.HasDeleteField)
        throw new LegacyDeniedOrNotExist();
      if (this.m_projects.Count > 0)
      {
        this.CheckProjectPermission((IEnumerable<int>) this.m_projects);
        requireAnyProjectPermission = false;
      }
      if (this.HasUpdatedObjects)
      {
        HashSet<int> projectIds = new HashSet<int>();
        using (ScopedMetadataComponent component = ScopedMetadataComponent.CreateComponent(this.m_requestContext))
        {
          foreach (int num in component.GetProjectsForProvisioning((IEnumerable<int>) this.m_fields, (IEnumerable<int>) this.m_fieldUsages, (IEnumerable<int>) this.m_treeProperties, (IEnumerable<int>) this.m_rules, (IEnumerable<int>) this.m_constants, (IEnumerable<int>) this.m_sets, (IEnumerable<int>) this.m_wits, (IEnumerable<int>) this.m_typeUsages, (IEnumerable<int>) this.m_actions, (IEnumerable<int>) this.m_categories, (IEnumerable<int>) this.m_categoryMembers))
          {
            if (!this.m_projects.Contains(num))
              projectIds.Add(num);
          }
        }
        if (projectIds.Count > 0)
        {
          this.CheckProjectPermission((IEnumerable<int>) projectIds);
          requireAnyProjectPermission = false;
        }
      }
      if (!requireAnyProjectPermission)
        return;
      bool flag = false;
      foreach (ProjectInfo project in this.m_requestContext.GetService<IProjectService>().GetProjects(this.m_requestContext.Elevate()))
      {
        if (this.SecurityNamespace.HasPermission(this.m_requestContext, "$/" + project.Id.ToString("D"), 1, false))
        {
          flag = true;
          break;
        }
      }
      if (!flag)
        throw new LegacyDeniedOrNotExist();
    }

    public void AddProject(int projectId) => this.m_projects.Add(projectId);

    public void AddProject(string projectName)
    {
      Guid projectId = this.m_requestContext.GetService<IProjectService>().GetProjectId(this.m_requestContext.Elevate(), projectName);
      TreeNode treeNode = this.TreeService.GetTreeNode(this.m_requestContext, projectId, projectId, false);
      if (treeNode == null)
        return;
      this.AddProject(treeNode.Id);
    }

    public void AddField(int fieldId) => this.m_fields.Add(fieldId);

    public void AddFieldUsage(int fieldUsageId) => this.m_fieldUsages.Add(fieldUsageId);

    public void AddTreeProperty(int treePropId) => this.m_treeProperties.Add(treePropId);

    public void AddRule(int ruleId) => this.m_rules.Add(ruleId);

    public void AddConstant(int constId) => this.m_constants.Add(constId);

    public void AddConstantSet(int setId) => this.m_sets.Add(setId);

    public void AddWorkItemType(int typeId) => this.m_wits.Add(typeId);

    public void AddWorkItemTypeUsage(int typeUsageId) => this.m_typeUsages.Add(typeUsageId);

    public void AddAction(int actionId) => this.m_actions.Add(actionId);

    public void AddWorkItemTypeCategory(int categoryId) => this.m_categories.Add(categoryId);

    public void AddWorkItemTypeCategoryMember(int memberId) => this.m_categoryMembers.Add(memberId);

    public bool HasDeleteField { get; set; }

    public bool HasFieldOperation => this.HasDeleteField || this.m_fields.Count > 0 || this.m_fieldUsages.Count > 0;

    internal HashSet<int> ProjectIds => this.m_projects;

    private void CheckProjectPermission(IEnumerable<int> projectIds)
    {
      foreach (int projectId in projectIds)
      {
        string token = projectId == 0 ? "$" : "$/" + this.TreeService.LegacyGetTreeNode(this.m_requestContext, projectId).CssNodeId.ToString("D");
        bool alwaysAllowAdministrators = false;
        this.m_requestContext.GetService<ITeamFoundationFeatureAvailabilityService>();
        if (WorkItemTrackingFeatureFlags.AreXMLProcessesEnabled(this.m_requestContext))
        {
          Guid guid;
          alwaysAllowAdministrators = this.m_requestContext.TryGetItem<Guid>("PromoteHostedProjectToInheritedUser", out guid) && guid == this.m_requestContext.GetUserId();
        }
        if (!this.SecurityNamespace.HasPermission(this.m_requestContext, token, 1, alwaysAllowAdministrators))
          throw new LegacyDeniedOrNotExist();
      }
    }

    private bool HasUpdatedObjects => this.m_fields.Count > 0 || this.m_fieldUsages.Count > 0 || this.m_treeProperties.Count > 0 || this.m_rules.Count > 0 || this.m_constants.Count > 0 || this.m_sets.Count > 0 || this.m_wits.Count > 0 || this.m_typeUsages.Count > 0 || this.m_actions.Count > 0 || this.m_categories.Count > 0 || this.m_categoryMembers.Count > 0;

    private WorkItemTrackingTreeService TreeService
    {
      get
      {
        if (this.m_treeService == null)
        {
          this.m_treeService = this.m_requestContext.GetService<WorkItemTrackingTreeService>();
          this.m_treeService.InvalidateCache(this.m_requestContext);
        }
        return this.m_treeService;
      }
    }

    private IVssSecurityNamespace SecurityNamespace
    {
      get
      {
        if (this.m_securityNamespace == null)
          this.m_securityNamespace = this.m_requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(this.m_requestContext, WitProvisionSecurity.NamespaceId);
        return this.m_securityNamespace;
      }
    }
  }
}

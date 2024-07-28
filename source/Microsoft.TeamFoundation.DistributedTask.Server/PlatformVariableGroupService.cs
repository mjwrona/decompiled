// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.PlatformVariableGroupService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.DistributedTask.Core.Server.Plugins;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Server.Converters;
using Microsoft.TeamFoundation.DistributedTask.Server.DataAccess;
using Microsoft.TeamFoundation.DistributedTask.Server.Exceptions;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal class PlatformVariableGroupService : IVariableGroupService, IVssFrameworkService
  {
    private readonly IVariableGroupSecretsHelper secretsHelper;
    private readonly LibrarySecurityProvider SecurityProvider;
    private const string c_layer = "PlatformVariableGroupService";

    public PlatformVariableGroupService()
      : this((LibrarySecurityProvider) new VariableGroupSecurityProvider(), (IVariableGroupSecretsHelper) new VariableGroupSecretsHelper())
    {
    }

    public PlatformVariableGroupService(
      LibrarySecurityProvider securityProvider,
      IVariableGroupSecretsHelper secretHelper)
    {
      this.secretsHelper = secretHelper;
      this.SecurityProvider = securityProvider;
    }

    public VariableGroup AddVariableGroup(
      IVssRequestContext requestContext,
      Guid projectId,
      VariableGroupParameters variableGroupParameters)
    {
      using (new MethodScope(requestContext, nameof (PlatformVariableGroupService), nameof (AddVariableGroup)))
      {
        VariableGroup variableGroup1 = variableGroupParameters.ToVariableGroup(requestContext, 0, projectId);
        ArgumentValidation.CheckVariableGroup(variableGroup1, "group");
        List<Guid> projectIds = new List<Guid>()
        {
          projectId
        };
        this.CheckProjectLevelCreatePermission(requestContext, (IEnumerable<Guid>) projectIds);
        PlatformVariableGroupService.PublishDecisionPoints(requestContext, projectIds, variableGroup1, AuditAction.Add);
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        variableGroup1.CreatedBy = new IdentityRef()
        {
          Id = userIdentity.Id.ToString("D")
        };
        VariableGroup variableGroup2;
        using (VariableGroupComponent component = requestContext.CreateComponent<VariableGroupComponent>())
          variableGroup2 = component.AddVariableGroup(projectId, variableGroup1);
        if (variableGroup2 != null)
        {
          variableGroup1.Id = variableGroup2.Id;
          this.AssignProjectLevelAdminPermission(requestContext, projectId, variableGroup1.Id, userIdentity);
          this.AssignCollectionLevelAdminPermission(requestContext, variableGroup1.Id, userIdentity);
          VariableGroupSecurityProvider.PromoteProjectLevelAdminsToCollectionLevelAdminsForVariableGroup(requestContext, (ServicingContext) null, projectId.ToString(), variableGroup1.Id);
          this.secretsHelper.StoreSecrets(requestContext, variableGroup1);
        }
        this.FillVariableGroupProjectReferencesAndSharedStatus(requestContext, variableGroup2, true);
        return variableGroup2;
      }
    }

    public VariableGroup UpdateVariableGroup(
      IVssRequestContext requestContext,
      Guid projectId,
      int groupId,
      VariableGroupParameters variableGroupParameters)
    {
      using (new MethodScope(requestContext, nameof (PlatformVariableGroupService), nameof (UpdateVariableGroup)))
      {
        VariableGroup variableGroup1 = variableGroupParameters.ToVariableGroup(requestContext, groupId, projectId);
        ArgumentValidation.CheckVariableGroup(variableGroup1, "group");
        List<Guid> projectIds = new List<Guid>()
        {
          projectId
        };
        if (requestContext.IsFeatureEnabled("WebAccess.DistributedTask.ShareVariableGroups") && this.IsCollectionLevelVariableGroupChanged(requestContext, variableGroup1))
          this.CheckCollectionLevelAdminPermission(requestContext, variableGroup1.Id);
        this.CheckProjectLevelAdminOrCollectionLevelAdminPermission(requestContext, (IList<Guid>) projectIds, groupId, TaskResources.VariableGroupAccessDeniedForAdminOperation());
        this.ValidateAzureKeyVaultVariableGroupUpdate(requestContext, variableGroup1);
        PlatformVariableGroupService.PublishDecisionPoints(requestContext, projectIds, variableGroup1, AuditAction.Update);
        variableGroup1.ModifiedBy = new IdentityRef()
        {
          Id = requestContext.GetUserIdentity().Id.ToString("D")
        };
        VariableGroup variableGroup2;
        using (VariableGroupComponent component = requestContext.CreateComponent<VariableGroupComponent>())
          variableGroup2 = component.UpdateVariableGroup(projectId, variableGroup1);
        if (variableGroup2 != null)
        {
          this.MigrateExistingSecretsFromProjectToCollectionLevel(requestContext, variableGroup2);
          this.secretsHelper.UpdateSecrets(requestContext, variableGroup1);
        }
        this.FillVariableGroupProjectReferencesAndSharedStatus(requestContext, variableGroup2, true);
        return variableGroup2;
      }
    }

    public VariableGroup GetVariableGroup(
      IVssRequestContext requestContext,
      Guid projectId,
      int groupId)
    {
      using (new MethodScope(requestContext, nameof (PlatformVariableGroupService), nameof (GetVariableGroup)))
      {
        ArgumentUtility.CheckForNonnegativeInt(groupId, nameof (groupId));
        VariableGroup variableGroup;
        using (VariableGroupComponent component = requestContext.CreateComponent<VariableGroupComponent>())
          variableGroup = component.GetVariableGroup(projectId, groupId);
        if (variableGroup == null)
          return (VariableGroup) null;
        if (!this.SecurityProvider.HasPermissions(requestContext, new Guid?(projectId), groupId.ToString(), 1, checkBothProjectAndCollectionScope: true))
          return new VariableGroup()
          {
            Name = variableGroup.Name,
            Id = variableGroup.Id
          };
        PlatformVariableGroupService.FillIdentityDetails(requestContext, (IList<VariableGroup>) new VariableGroup[1]
        {
          variableGroup
        });
        if ((requestContext.IsSystemContext ? 1 : (ServicePrincipals.IsServicePrincipalThatCanReadSecretVariables(requestContext) ? 1 : 0)) != 0)
          this.secretsHelper.ReadSecrets(requestContext, projectId, variableGroup);
        this.FillVariableGroupProjectReferencesAndSharedStatus(requestContext, variableGroup, true);
        return variableGroup;
      }
    }

    public IList<VariableGroup> GetVariableGroups(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<int> groupIds,
      VariableGroupActionFilter actionFilter = VariableGroupActionFilter.None)
    {
      using (new MethodScope(requestContext, nameof (PlatformVariableGroupService), nameof (GetVariableGroups)))
      {
        IList<VariableGroup> variableGroups1;
        if (groupIds != null && groupIds.Any<int>())
        {
          groupIds = (IList<int>) groupIds.Distinct<int>().ToList<int>();
          IList<VariableGroup> variableGroups2;
          using (VariableGroupComponent component = requestContext.CreateComponent<VariableGroupComponent>())
            variableGroups2 = (IList<VariableGroup>) component.GetVariableGroups(projectId, (IEnumerable<int>) groupIds);
          variableGroups1 = this.FilterVariableGroups(requestContext, projectId, variableGroups2, actionFilter, true);
          PlatformVariableGroupService.FillIdentityDetails(requestContext, variableGroups1);
          if ((requestContext.IsSystemContext ? 1 : (ServicePrincipals.IsServicePrincipalThatCanReadSecretVariables(requestContext) ? 1 : 0)) != 0)
          {
            foreach (VariableGroup group in (IEnumerable<VariableGroup>) variableGroups1)
              this.secretsHelper.ReadSecrets(requestContext, projectId, group);
          }
        }
        else
          variableGroups1 = (IList<VariableGroup>) Array.Empty<VariableGroup>();
        this.PopulateVariableGroupsSharedStatus(requestContext, projectId, variableGroups1);
        return variableGroups1;
      }
    }

    public IList<VariableGroup> GetVariableGroups(
      IVssRequestContext requestContext,
      Guid projectId,
      string groupName,
      VariableGroupActionFilter actionFilter = VariableGroupActionFilter.None,
      int? continuationToken = null,
      int top = 0,
      VariableGroupQueryOrder queryOrder = VariableGroupQueryOrder.IdDescending)
    {
      using (new MethodScope(requestContext, nameof (PlatformVariableGroupService), nameof (GetVariableGroups)))
      {
        this.SecurityProvider.CheckAndInitializeLibraryPermissions(requestContext, new Guid?(projectId));
        IList<VariableGroup> variableGroups1;
        using (VariableGroupComponent component = requestContext.CreateComponent<VariableGroupComponent>())
          variableGroups1 = (IList<VariableGroup>) component.GetVariableGroups(projectId, groupName, continuationToken, top, queryOrder);
        IList<VariableGroup> variableGroups2 = this.FilterVariableGroups(requestContext, projectId, variableGroups1, actionFilter, false);
        PlatformVariableGroupService.FillIdentityDetails(requestContext, variableGroups2);
        this.PopulateVariableGroupsSharedStatus(requestContext, projectId, variableGroups2);
        return variableGroups2;
      }
    }

    public void DeleteVariableGroup(IVssRequestContext requestContext, Guid projectId, int groupId)
    {
      using (new MethodScope(requestContext, nameof (PlatformVariableGroupService), nameof (DeleteVariableGroup)))
      {
        IVssRequestContext requestContext1 = requestContext;
        List<Guid> projectIds1 = new List<Guid>();
        projectIds1.Add(projectId);
        int groupId1 = groupId;
        string errorMessage = TaskResources.VariableGroupAccessDeniedForAdminOperation();
        this.CheckProjectLevelAdminOrCollectionLevelAdminPermission(requestContext1, (IList<Guid>) projectIds1, groupId1, errorMessage);
        using (VariableGroupComponent component = requestContext.CreateComponent<VariableGroupComponent>())
          component.DeleteVariableGroup(projectId, groupId);
        IVssRequestContext requestContext2 = requestContext;
        List<Guid> projectIds2 = new List<Guid>();
        projectIds2.Add(projectId);
        int groupId2 = groupId;
        this.RemoveSecretsAndPermissions(requestContext2, (IList<Guid>) projectIds2, groupId2);
        IVssRequestContext requestContext3 = requestContext;
        List<Guid> projectIds3 = new List<Guid>();
        projectIds3.Add(projectId);
        int groupId3 = groupId;
        PlatformVariableGroupService.DeletePipelinePermissionsForResource(requestContext3, (IList<Guid>) projectIds3, groupId3);
      }
    }

    public void DeleteTeamProject(IVssRequestContext requestContext, Guid projectId)
    {
      IList<VariableGroup> variableGroups = this.GetVariableGroups(requestContext, projectId, (IList<int>) null, VariableGroupActionFilter.None);
      this.PopulateVariableGroupsSharedStatus(requestContext, projectId, variableGroups);
      foreach (VariableGroup variableGroup in (IEnumerable<VariableGroup>) variableGroups)
      {
        if (!variableGroup.IsShared)
          this.secretsHelper.DeleteSecrets(requestContext, projectId, variableGroup.Id);
      }
      using (VariableGroupComponent component = requestContext.CreateComponent<VariableGroupComponent>())
        component.DeleteTeamProject(projectId);
    }

    public VariableGroup AddVariableGroupForCollection(
      IVssRequestContext requestContext,
      VariableGroupParameters variableGroupParameters)
    {
      using (new MethodScope(requestContext, nameof (PlatformVariableGroupService), nameof (AddVariableGroupForCollection)))
      {
        if (variableGroupParameters.VariableGroupProjectReferences == null || variableGroupParameters.VariableGroupProjectReferences.Count<VariableGroupProjectReference>() == 0)
          throw new InvalidRequestException(TaskResources.AtleastOneProjectReferenceRequired());
        if (variableGroupParameters.VariableGroupProjectReferences.Count<VariableGroupProjectReference>() > 1 && !requestContext.IsFeatureEnabled("WebAccess.DistributedTask.ShareVariableGroups"))
          throw new InvalidRequestException(TaskResources.SharingVariableGroupNotAllowed());
        if (!requestContext.IsFeatureEnabled("WebAccess.DistributedTask.VariableGroupCollectionApis"))
          return this.AddVariableGroup(requestContext, variableGroupParameters.VariableGroupProjectReferences[0].ProjectReference.Id, variableGroupParameters);
        VariableGroup variableGroup1 = variableGroupParameters.ToVariableGroup(requestContext, 0, Guid.Empty);
        ArgumentValidation.CheckVariableGroup(variableGroup1, "group");
        List<Guid> list = variableGroupParameters.VariableGroupProjectReferences.Select<VariableGroupProjectReference, Guid>((Func<VariableGroupProjectReference, Guid>) (x => x.ProjectReference.Id)).ToList<Guid>();
        this.CheckProjectLevelCreatePermission(requestContext, (IEnumerable<Guid>) list);
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        foreach (VariableGroupProjectReference projectReference in (IEnumerable<VariableGroupProjectReference>) variableGroup1.VariableGroupProjectReferences)
          this.ValidateServiceEndpointOfAzureKeyVaultVariableGroup(requestContext, projectReference.ProjectReference, variableGroup1);
        variableGroup1.CreatedBy = new IdentityRef()
        {
          Id = userIdentity.Id.ToString("D")
        };
        VariableGroup variableGroup2;
        using (VariableGroupComponent component = requestContext.CreateComponent<VariableGroupComponent>())
        {
          variableGroup2 = component.AddVariableGroupCollection(variableGroup1);
          variableGroup1.Id = variableGroup2.Id;
        }
        if (variableGroup2 != null)
        {
          this.secretsHelper.StoreSecrets(requestContext, variableGroup1);
          this.AssignCollectionLevelAdminPermission(requestContext, variableGroup2.Id, userIdentity);
          foreach (VariableGroupProjectReference projectReference in (IEnumerable<VariableGroupProjectReference>) variableGroup1.VariableGroupProjectReferences)
            this.AssignProjectLevelAdminPermission(requestContext, projectReference.ProjectReference.Id, variableGroup2.Id, userIdentity);
          if (variableGroup1.VariableGroupProjectReferences.Count<VariableGroupProjectReference>() == 1)
            VariableGroupSecurityProvider.PromoteProjectLevelAdminsToCollectionLevelAdminsForVariableGroup(requestContext, (ServicingContext) null, variableGroup1.VariableGroupProjectReferences[0].ProjectReference.Id.ToString(), variableGroup2.Id);
        }
        PlatformVariableGroupService.PublishDecisionPoints(requestContext, list, variableGroup1, AuditAction.Add);
        this.FillVariableGroupProjectReferencesAndSharedStatus(requestContext, variableGroup2, true);
        return variableGroup2;
      }
    }

    public VariableGroup UpdateVariableGroupForCollection(
      IVssRequestContext requestContext,
      int groupId,
      VariableGroupParameters variableGroupParameters)
    {
      using (new MethodScope(requestContext, nameof (PlatformVariableGroupService), nameof (UpdateVariableGroupForCollection)))
      {
        if (!requestContext.IsFeatureEnabled("WebAccess.DistributedTask.VariableGroupCollectionApis"))
          return this.UpdateVariableGroup(requestContext, variableGroupParameters.VariableGroupProjectReferences[0].ProjectReference.Id, groupId, variableGroupParameters);
        VariableGroup variableGroup1 = variableGroupParameters.ToVariableGroup(requestContext, groupId, Guid.Empty);
        ArgumentValidation.CheckVariableGroup(variableGroup1, "group");
        List<Guid> list = variableGroup1.VariableGroupProjectReferences.Select<VariableGroupProjectReference, Guid>((Func<VariableGroupProjectReference, Guid>) (x => x.ProjectReference.Id)).ToList<Guid>();
        this.ValidateVariableGroupIsAlreadyPresentInProjects(requestContext, variableGroup1.Id, (IList<Guid>) list);
        this.CheckProjectLevelAdminOrCollectionLevelAdminPermission(requestContext, (IList<Guid>) list, variableGroup1.Id, TaskResources.VariableGroupAccessDeniedForAdminOperation());
        bool isCollectionLevelVariableGroupChanged = this.IsCollectionLevelVariableGroupChanged(requestContext, variableGroup1);
        if (isCollectionLevelVariableGroupChanged)
        {
          this.CheckCollectionLevelAdminPermission(requestContext, variableGroup1.Id);
          this.ValidateAzureKeyVaultVariableGroupUpdate(requestContext, variableGroup1);
        }
        PlatformVariableGroupService.PublishDecisionPoints(requestContext, list, variableGroup1, AuditAction.Update);
        variableGroup1.ModifiedBy = new IdentityRef()
        {
          Id = requestContext.GetUserIdentity().Id.ToString("D")
        };
        VariableGroup variableGroup2;
        using (VariableGroupComponent component = requestContext.CreateComponent<VariableGroupComponent>())
          variableGroup2 = component.UpdateVariableGroupCollection(variableGroup1, isCollectionLevelVariableGroupChanged);
        if (variableGroup2 != null & isCollectionLevelVariableGroupChanged)
        {
          this.MigrateExistingSecretsFromProjectToCollectionLevel(requestContext, variableGroup2);
          this.secretsHelper.UpdateSecrets(requestContext, variableGroup1);
        }
        this.FillVariableGroupProjectReferencesAndSharedStatus(requestContext, variableGroup2, true);
        return variableGroup2;
      }
    }

    public void ShareVariableGroupForCollection(
      IVssRequestContext requestContext,
      int variableGroupId,
      IList<VariableGroupProjectReference> variableGroupProjectReferences)
    {
      if (!requestContext.IsFeatureEnabled("WebAccess.DistributedTask.ShareVariableGroups"))
        throw new InvalidRequestException(TaskResources.SharingVariableGroupNotAllowed());
      foreach (VariableGroupProjectReference projectReference in (IEnumerable<VariableGroupProjectReference>) variableGroupProjectReferences)
        ArgumentValidation.CheckVariableGroupProjectReference(projectReference, "VariableGroupProjectReference");
      this.CheckCollectionLevelAdminPermission(requestContext, variableGroupId);
      this.CheckProjectLevelCreatePermission(requestContext, variableGroupProjectReferences.Select<VariableGroupProjectReference, Guid>((Func<VariableGroupProjectReference, Guid>) (x => x.ProjectReference.Id)));
      VariableGroup variableGroup = this.GetVariableGroup(requestContext, variableGroupId);
      this.MigrateExistingSecretsFromProjectToCollectionLevel(requestContext, variableGroup);
      foreach (VariableGroupProjectReference projectReference in (IEnumerable<VariableGroupProjectReference>) variableGroupProjectReferences)
        this.ValidateServiceEndpointOfAzureKeyVaultVariableGroup(requestContext, projectReference.ProjectReference, variableGroup);
      using (VariableGroupComponent component = requestContext.CreateComponent<VariableGroupComponent>())
        component.ShareVariableGroup(variableGroup.Id, variableGroupProjectReferences);
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      foreach (VariableGroupProjectReference projectReference in (IEnumerable<VariableGroupProjectReference>) variableGroupProjectReferences)
        this.AssignProjectLevelAdminPermission(requestContext, projectReference.ProjectReference.Id, variableGroupId, userIdentity);
    }

    public void DeleteVariableGroupForCollection(
      IVssRequestContext requestContext,
      int groupId,
      IList<Guid> projectIds)
    {
      using (new MethodScope(requestContext, nameof (PlatformVariableGroupService), nameof (DeleteVariableGroupForCollection)))
      {
        if (!requestContext.IsFeatureEnabled("WebAccess.DistributedTask.VariableGroupCollectionApis"))
        {
          if (projectIds == null || projectIds.Count == 0)
            throw new InvalidRequestException(TaskResources.AtleastOneProjectRequiredForDeletion());
          this.DeleteVariableGroup(requestContext, projectIds[0], groupId);
        }
        else
        {
          if (projectIds == null || projectIds.Count == 0)
            projectIds = (IList<Guid>) (this.GetVariableGroup(requestContext, groupId) ?? throw new InvalidRequestException(TaskResources.VariableGroupNotFound((object) groupId))).VariableGroupProjectReferences.Select<VariableGroupProjectReference, Guid>((Func<VariableGroupProjectReference, Guid>) (x => x.ProjectReference.Id)).ToList<Guid>();
          else
            this.ValidateVariableGroupIsAlreadyPresentInProjects(requestContext, groupId, projectIds);
          this.CheckProjectLevelAdminOrCollectionLevelAdminPermission(requestContext, projectIds, groupId, TaskResources.VariableGroupAccessDeniedForDeleteOperation());
          using (VariableGroupComponent component = requestContext.CreateComponent<VariableGroupComponent>())
            component.DeleteVariableGroupFromProjects(groupId, projectIds);
          this.RemoveSecretsAndPermissions(requestContext, projectIds, groupId);
          PlatformVariableGroupService.DeletePipelinePermissionsForResource(requestContext, projectIds, groupId);
        }
      }
    }

    private VariableGroup GetVariableGroup(IVssRequestContext requestContext, int groupId)
    {
      using (new MethodScope(requestContext, nameof (PlatformVariableGroupService), nameof (GetVariableGroup)))
      {
        ArgumentUtility.CheckForNonnegativeInt(groupId, nameof (groupId));
        VariableGroup variableGroup;
        using (VariableGroupComponent component = requestContext.CreateComponent<VariableGroupComponent>())
          variableGroup = component.GetVariableGroup(groupId);
        return variableGroup;
      }
    }

    private void FillVariableGroupProjectReferencesAndSharedStatus(
      IVssRequestContext requestContext,
      VariableGroup variableGroup,
      bool fillProjectDetails)
    {
      if (variableGroup == null)
        return;
      IList<VariableGroupProjectReference> projectReferences;
      using (VariableGroupComponent component = requestContext.CreateComponent<VariableGroupComponent>())
        projectReferences = component.GetVariableGroupProjectReferences(variableGroup.Id);
      if (fillProjectDetails)
        VariableGroupHelper.FillVariableGroupProjectReferencesProjectDetail(requestContext, projectReferences);
      variableGroup.VariableGroupProjectReferences = projectReferences;
      variableGroup.IsShared = projectReferences.Count > 1;
    }

    private static void FillIdentityDetails(
      IVssRequestContext requestContext,
      IList<VariableGroup> variableGroups)
    {
      if (variableGroups == null || !variableGroups.Any<VariableGroup>())
        return;
      HashSet<string> identityIds = new HashSet<string>(variableGroups.Count);
      Dictionary<string, List<VariableGroup>> dictionary1 = new Dictionary<string, List<VariableGroup>>(variableGroups.Count);
      foreach (VariableGroup variableGroup in (IEnumerable<VariableGroup>) variableGroups)
      {
        if (variableGroup.CreatedBy != null && !string.IsNullOrWhiteSpace(variableGroup.CreatedBy.Id))
        {
          identityIds.Add(variableGroup.CreatedBy.Id);
          List<VariableGroup> variableGroupList;
          if (dictionary1.TryGetValue(variableGroup.CreatedBy.Id, out variableGroupList))
            variableGroupList.Add(variableGroup);
          else
            dictionary1.Add(variableGroup.CreatedBy.Id, new List<VariableGroup>()
            {
              variableGroup
            });
        }
        if (variableGroup.ModifiedBy != null && !string.IsNullOrWhiteSpace(variableGroup.ModifiedBy.Id))
        {
          identityIds.Add(variableGroup.ModifiedBy.Id);
          List<VariableGroup> variableGroupList;
          if (dictionary1.TryGetValue(variableGroup.ModifiedBy.Id, out variableGroupList))
            variableGroupList.Add(variableGroup);
          else
            dictionary1.Add(variableGroup.ModifiedBy.Id, new List<VariableGroup>()
            {
              variableGroup
            });
        }
      }
      IDictionary<string, IdentityRef> dictionary2 = identityIds.QueryIdentities(requestContext);
      foreach (string key in (IEnumerable<string>) dictionary2.Keys)
      {
        List<VariableGroup> variableGroupList;
        if (dictionary1.TryGetValue(key, out variableGroupList))
        {
          foreach (VariableGroup variableGroup in variableGroupList)
          {
            if (variableGroup.CreatedBy != null && variableGroup.CreatedBy.Id == key)
              variableGroup.CreatedBy = dictionary2[key];
            if (variableGroup.ModifiedBy != null && variableGroup.ModifiedBy.Id == key)
              variableGroup.ModifiedBy = dictionary2[key];
          }
        }
      }
    }

    private IList<VariableGroup> FilterVariableGroups(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<VariableGroup> variableGroups,
      VariableGroupActionFilter actionFilter,
      bool allowShallowVariableGroupOnNoPermission)
    {
      List<VariableGroup> variableGroupList = new List<VariableGroup>();
      if (variableGroups != null && variableGroups.Any<VariableGroup>())
      {
        int requiredPermissions = 1;
        if ((actionFilter & VariableGroupActionFilter.Use) == VariableGroupActionFilter.Use)
          requiredPermissions |= 16;
        if ((actionFilter & VariableGroupActionFilter.Manage) == VariableGroupActionFilter.Manage)
          requiredPermissions |= 2;
        foreach (VariableGroup variableGroup in (IEnumerable<VariableGroup>) variableGroups)
        {
          if (this.SecurityProvider.HasPermissions(requestContext, new Guid?(projectId), variableGroup.Id.ToString(), requiredPermissions, true, true))
            variableGroupList.Add(variableGroup);
          else if (allowShallowVariableGroupOnNoPermission && actionFilter == VariableGroupActionFilter.None)
            variableGroupList.Add(new VariableGroup()
            {
              Id = variableGroup.Id,
              Name = variableGroup.Name
            });
        }
      }
      return (IList<VariableGroup>) variableGroupList;
    }

    private void PopulateVariableGroupsSharedStatus(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<VariableGroup> variableGroups)
    {
      if (!requestContext.IsFeatureEnabled("WebAccess.DistributedTask.ShareVariableGroups"))
        return;
      IList<int> sharedVariableGroups = this.GetSharedVariableGroups(requestContext, projectId, (IList<int>) variableGroups.Select<VariableGroup, int>((Func<VariableGroup, int>) (x => x.Id)).ToList<int>());
      if (sharedVariableGroups.Count <= 0)
        return;
      Dictionary<int, int> dictionary = sharedVariableGroups.ToDictionary<int, int>((Func<int, int>) (x => x));
      foreach (VariableGroup variableGroup in (IEnumerable<VariableGroup>) variableGroups)
      {
        if (dictionary.ContainsKey(variableGroup.Id))
          variableGroup.IsShared = true;
      }
    }

    private IList<int> GetSharedVariableGroups(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<int> groupIds)
    {
      groupIds = (IList<int>) groupIds.Distinct<int>().ToList<int>();
      using (VariableGroupComponent component = requestContext.CreateComponent<VariableGroupComponent>())
        return (IList<int>) component.GetSharedVariableGroups(projectId, (IEnumerable<int>) groupIds);
    }

    private void ValidateAzureKeyVaultVariableGroupUpdate(
      IVssRequestContext requestContext,
      VariableGroup updatedVariableGroup)
    {
      if (!(updatedVariableGroup.Type == "AzureKeyVault"))
        return;
      VariableGroup variableGroup = requestContext.IsFeatureEnabled("WebAccess.DistributedTask.VariableGroupCollectionApis") ? this.GetVariableGroup(requestContext, updatedVariableGroup.Id) : this.GetVariableGroup(requestContext, updatedVariableGroup.VariableGroupProjectReferences[0].ProjectReference.Id, updatedVariableGroup.Id);
      bool flag = false;
      if (variableGroup.Type != "AzureKeyVault")
        flag = true;
      else if (!(variableGroup.ProviderData as AzureKeyVaultVariableGroupProviderData).ServiceEndpointId.Equals((updatedVariableGroup.ProviderData as AzureKeyVaultVariableGroupProviderData).ServiceEndpointId))
        flag = true;
      if (!flag)
        return;
      List<Microsoft.TeamFoundation.DistributedTask.WebApi.ProjectReference> list = variableGroup.VariableGroupProjectReferences.Select<VariableGroupProjectReference, Microsoft.TeamFoundation.DistributedTask.WebApi.ProjectReference>((Func<VariableGroupProjectReference, Microsoft.TeamFoundation.DistributedTask.WebApi.ProjectReference>) (x => x.ProjectReference)).ToList<Microsoft.TeamFoundation.DistributedTask.WebApi.ProjectReference>();
      if (list.Count<Microsoft.TeamFoundation.DistributedTask.WebApi.ProjectReference>() <= 0)
        return;
      requestContext.GetService<IServiceEndpointService2>();
      foreach (Microsoft.TeamFoundation.DistributedTask.WebApi.ProjectReference projectReference in list)
        this.ValidateServiceEndpointOfAzureKeyVaultVariableGroup(requestContext, projectReference, updatedVariableGroup);
    }

    private void ValidateServiceEndpointOfAzureKeyVaultVariableGroup(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.DistributedTask.WebApi.ProjectReference projectReference,
      VariableGroup variableGroup)
    {
      if (!(variableGroup.Type == "AzureKeyVault"))
        return;
      Guid serviceEndpointId = (variableGroup.ProviderData as AzureKeyVaultVariableGroupProviderData).ServiceEndpointId;
      if (requestContext.GetService<IServiceEndpointService2>().GetServiceEndpoint(requestContext, projectReference.Id, serviceEndpointId, ServiceEndpointActionFilter.Use) == null)
        throw new InvalidRequestException(TaskResources.ServiceEndpointNotShared((object) serviceEndpointId, (object) projectReference.Name));
    }

    private static void PublishDecisionPoints(
      IVssRequestContext requestContext,
      List<Guid> projectIds,
      VariableGroup group,
      AuditAction auditAction)
    {
      ITeamFoundationEventService service = requestContext.GetService<ITeamFoundationEventService>();
      foreach (Guid projectId in projectIds)
      {
        string format = string.Empty;
        switch (auditAction)
        {
          case AuditAction.Add:
            format = string.Format("Publishing VariableGroupChangingEvent decision point: adding variable group in project {0}", (object) projectId);
            break;
          case AuditAction.Update:
            format = string.Format("Publishing VariableGroupChangingEvent decision point: updating variable group {0}", (object) group.Id);
            break;
        }
        requestContext.TraceInfo(10015174, "DistributedTask", format);
        service.PublishDecisionPoint(requestContext, (object) new VariableGroupChangingEvent(projectId, auditAction, group));
      }
    }

    private void AssignProjectLevelAdminPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      int variableGroupId,
      Microsoft.VisualStudio.Services.Identity.Identity creatorIdentity)
    {
      this.SecurityProvider.AddLibraryItemCreatorAsItemAdministrator(requestContext, new Guid?(projectId), creatorIdentity, variableGroupId.ToString());
    }

    private void AssignCollectionLevelAdminPermission(
      IVssRequestContext requestContext,
      int variableGroupId,
      Microsoft.VisualStudio.Services.Identity.Identity creatorIdentity)
    {
      this.SecurityProvider.AddLibraryItemCreatorAsCollectionAdmin(requestContext, creatorIdentity, variableGroupId.ToString());
    }

    private void CheckProjectLevelCreatePermission(
      IVssRequestContext requestContext,
      IEnumerable<Guid> projectIds)
    {
      foreach (Guid projectId in projectIds)
        this.SecurityProvider.CheckCreatePermissions(requestContext, new Guid?(projectId), TaskResources.VariableGroup());
    }

    private void CheckCollectionLevelAdminPermission(IVssRequestContext requestContext, int groupId)
    {
      string errorMessage = requestContext.IsFeatureEnabled("WebAccess.DistributedTask.ShareVariableGroups") ? TaskResources.VariableGroupAccessDeniedForCollectionAdminOperation() : TaskResources.VariableGroupAccessDeniedForAdminOperation();
      this.SecurityProvider.CheckPermissions(requestContext, new Guid?(), groupId.ToString(), 2, false, errorMessage);
    }

    private void CheckProjectLevelAdminOrCollectionLevelAdminPermission(
      IVssRequestContext requestContext,
      IList<Guid> projectIds,
      int groupId,
      string errorMessage)
    {
      if (this.SecurityProvider.HasPermissions(requestContext, new Guid?(), groupId.ToString(), 2))
        return;
      foreach (Guid projectId in (IEnumerable<Guid>) projectIds)
        this.SecurityProvider.CheckPermissions(requestContext, new Guid?(projectId), groupId.ToString(), 2, false, TaskResources.VariableGroupAccessDeniedForAdminOperation());
    }

    private void MigrateExistingSecretsFromProjectToCollectionLevel(
      IVssRequestContext requestContext,
      VariableGroup variableGroup)
    {
      if (variableGroup.VariableGroupProjectReferences == null || variableGroup.VariableGroupProjectReferences.Count<VariableGroupProjectReference>() == 0)
        this.FillVariableGroupProjectReferencesAndSharedStatus(requestContext, variableGroup, false);
      if (variableGroup.VariableGroupProjectReferences.Count<VariableGroupProjectReference>() != 1)
        return;
      this.secretsHelper.MigrateSecretsFromProjectToCollectionLevel(requestContext, variableGroup.VariableGroupProjectReferences[0].ProjectReference.Id, variableGroup.Id);
    }

    private bool IsCollectionLevelVariableGroupChanged(
      IVssRequestContext requestContext,
      VariableGroup updatedGroup)
    {
      VariableGroup variableGroup = this.GetVariableGroup(requestContext, updatedGroup.Id);
      if (variableGroup.ProviderData == null && updatedGroup.ProviderData != null || variableGroup.ProviderData != null && updatedGroup.ProviderData == null || variableGroup.ProviderData != null && updatedGroup.ProviderData != null && !variableGroup.ProviderData.Equals((object) updatedGroup.ProviderData) || variableGroup.Type != updatedGroup.Type || variableGroup.Variables.Count<KeyValuePair<string, VariableValue>>() != updatedGroup.Variables.Count<KeyValuePair<string, VariableValue>>())
        return true;
      foreach (KeyValuePair<string, VariableValue> variable in (IEnumerable<KeyValuePair<string, VariableValue>>) variableGroup.Variables)
      {
        if (!updatedGroup.Variables.ContainsKey(variable.Key) || !updatedGroup.Variables[variable.Key].Equals(variable.Value))
          return true;
      }
      return false;
    }

    private void RemoveSecretsAndPermissions(
      IVssRequestContext requestContext,
      IList<Guid> projectIds,
      int groupId)
    {
      try
      {
        bool flag = false;
        using (VariableGroupComponent component = requestContext.CreateComponent<VariableGroupComponent>())
          flag = component.GetVariableGroupProjectReferences(groupId).Count<VariableGroupProjectReference>() > 0;
        if (!flag)
        {
          if (projectIds.Count<Guid>() == 1)
            this.secretsHelper.DeleteSecrets(requestContext, projectIds[0], groupId);
          else
            this.secretsHelper.DeleteSecrets(requestContext, groupId);
          this.SecurityProvider.RemoveCollectionLevelAccessControlLists(requestContext, groupId.ToString());
        }
        foreach (Guid projectId in (IEnumerable<Guid>) projectIds)
          this.SecurityProvider.RemoveAccessControlLists(requestContext, projectId, groupId.ToString());
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10015013, "VariableGroup", ex);
      }
    }

    private static void DeletePipelinePermissionsForResource(
      IVssRequestContext requestContext,
      IList<Guid> projectIds,
      int groupId)
    {
      foreach (Guid projectId1 in (IEnumerable<Guid>) projectIds)
      {
        Guid projectId = projectId1;
        try
        {
          IPipelineResourceAuthorizationProxyService authorizationProxyService = requestContext.GetService<IPipelineResourceAuthorizationProxyService>();
          requestContext.RunSynchronously((Func<Task>) (() => authorizationProxyService.DeletePipelinePermissionsForResource(requestContext.Elevate(), projectId, groupId.ToString(), "variablegroup")));
        }
        catch (Exception ex)
        {
          string format = "Deleting pipeline permissions for variable group with ID: {0} failed with the following error: {1}";
          requestContext.TraceError(10015178, "DistributedTask", format, (object) groupId.ToString(), (object) ex.Message);
        }
      }
    }

    private void ValidateVariableGroupIsAlreadyPresentInProjects(
      IVssRequestContext requestContext,
      int variableGroupId,
      IList<Guid> newProjectIds)
    {
      List<Guid> list = (this.GetVariableGroup(requestContext, variableGroupId) ?? throw new InvalidRequestException(TaskResources.VariableGroupNotFound((object) variableGroupId))).VariableGroupProjectReferences.Select<VariableGroupProjectReference, Guid>((Func<VariableGroupProjectReference, Guid>) (x => x.ProjectReference.Id)).ToList<Guid>();
      IDictionary<Guid, bool> dictionary = (IDictionary<Guid, bool>) new Dictionary<Guid, bool>();
      foreach (Guid key in (IEnumerable<Guid>) list)
        dictionary.Add(key, true);
      foreach (Guid newProjectId in (IEnumerable<Guid>) newProjectIds)
      {
        if (!dictionary.ContainsKey(newProjectId))
          throw new ArgumentException(TaskResources.VariableGroupNotPartOfProject((object) newProjectId)).Expected("DistributedTask");
      }
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext) => requestContext.CheckProjectCollectionRequestContext();

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }
  }
}

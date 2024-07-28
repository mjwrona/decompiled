// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.Server.CheckConfigurationService
// Assembly: Microsoft.Azure.Pipelines.Checks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D08C7285-654E-4A4D-BA46-748F0D1E96AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.Server.dll

using Microsoft.Azure.Pipelines.Approval.WebApi;
using Microsoft.Azure.Pipelines.Checks.Common;
using Microsoft.Azure.Pipelines.Checks.Server.DataAccess;
using Microsoft.Azure.Pipelines.Checks.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Pipelines.Checks.Server
{
  internal class CheckConfigurationService : ICheckConfigurationService, IVssFrameworkService
  {
    public CheckConfiguration AddCheckConfiguration(
      IVssRequestContext requestContext,
      Guid projectId,
      CheckConfiguration checkConfiguration)
    {
      this.ValidateServiceBasicData(requestContext, projectId);
      using (new MethodScope(requestContext, nameof (CheckConfigurationService), nameof (AddCheckConfiguration)))
      {
        ArgumentValidation.ValidateCheckConfigurationParameters(checkConfiguration);
        checkConfiguration.Resource.Id = checkConfiguration.Resource.Id.ToLower();
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        checkConfiguration.CreatedBy = new IdentityRef()
        {
          Id = userIdentity.Id.ToString("D")
        };
        ICheckType checkInstance = requestContext.GetService<IChecksExtensionService>().GetCheckInstance(checkConfiguration.Type);
        checkConfiguration.Type.Id = checkInstance.CheckTypeId;
        requestContext.TraceInfo(34001919, nameof (CheckConfigurationService), "CheckConfiguration::AddCheckConfiguration Check configuration request for type: {0} with id: {1}.Creating configuration for type: {2} with id: {3} on resource: {4} with id: {5}.", (object) checkConfiguration.Type.Name, (object) checkConfiguration.Type.Id, (object) checkInstance.CheckTypeName, (object) checkInstance.CheckTypeId, (object) checkConfiguration.Resource.Type, (object) checkConfiguration.Resource.Id);
        try
        {
          checkConfiguration = checkInstance.ValidateAndGetCheckConfiguration(requestContext, projectId, checkConfiguration);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(34001903, nameof (CheckConfigurationService), ex);
          throw new InvalidCheckConfigurationException(ex.Message);
        }
        this.EnsureCheckConfigurationsAreValid(requestContext, (IList<CheckConfiguration>) new List<CheckConfiguration>()
        {
          checkConfiguration
        });
        Resource forConfiguration = Utilities.GetPermissibleResourceForConfiguration(requestContext, projectId, (CheckConfigurationRef) checkConfiguration, ResourcePermission.Admin);
        checkConfiguration.Resource.Name = forConfiguration.Name;
        this.EnsureNoDuplicateCheckTypes(requestContext, projectId, (CheckConfiguration) null, checkConfiguration, out bool _, out string _, out string _);
        CheckConfiguration checkConfiguration1;
        using (CheckConfigurationComponent component = requestContext.CreateComponent<CheckConfigurationComponent>())
        {
          checkConfiguration1 = component.AddCheckConfiguration(checkConfiguration, projectId);
          requestContext.TraceInfo(34001916, nameof (CheckConfigurationService), "CheckConfigurationService::AddCheckConfiguration Check configuration added for {0}", (object) checkInstance.CheckTypeName);
        }
        this.PopulateMissingDetails(requestContext, projectId, (IList<CheckConfiguration>) new CheckConfiguration[1]
        {
          checkConfiguration1
        }, (IList<Resource>) new List<Resource>()
        {
          forConfiguration
        });
        this.publishEventToTelemetry(requestContext, projectId, checkConfiguration, "Create", checkInstance);
        return checkConfiguration1;
      }
    }

    public IList<CheckConfiguration> AddCheckConfigurations(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<CheckConfiguration> checkConfigurations)
    {
      this.ValidateServiceBasicData(requestContext, projectId);
      using (new MethodScope(requestContext, nameof (CheckConfigurationService), nameof (AddCheckConfigurations)))
      {
        checkConfigurations.ForEach<CheckConfiguration>((Action<CheckConfiguration>) (checkConfiguration => ArgumentValidation.ValidateCheckConfigurationParameters(checkConfiguration)));
        requestContext.TraceInfo(34001919, nameof (CheckConfigurationService), "CheckConfiguration::AddCheckConfigurations Check configuration for {0} configurations", (object) checkConfigurations.Count<CheckConfiguration>());
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        IdentityRef identityRef = new IdentityRef()
        {
          Id = userIdentity.Id.ToString("D")
        };
        IChecksExtensionService service = requestContext.GetService<IChecksExtensionService>();
        for (int index = 0; index < checkConfigurations.Count<CheckConfiguration>(); ++index)
        {
          ICheckType checkInstance = service.GetCheckInstance(checkConfigurations[index].Type);
          ArgumentUtility.CheckForEmptyGuid(checkInstance.CheckTypeId, "checkTypeId", "Pipeline.Checks");
          checkConfigurations[index].Type.Id = checkInstance.CheckTypeId;
          try
          {
            checkConfigurations[index] = checkInstance.ValidateAndGetCheckConfiguration(requestContext, projectId, checkConfigurations[index]);
          }
          catch (Exception ex)
          {
            requestContext.TraceException(34001903, nameof (CheckConfigurationService), ex);
            throw new InvalidCheckConfigurationException(PipelineChecksResources.InValidCheckConfiguration((object) ex.Message, (object) checkConfigurations[index].Type.Id));
          }
          checkConfigurations[index].CreatedBy = identityRef;
        }
        this.EnsureCheckConfigurationsAreValid(requestContext, checkConfigurations);
        List<Resource> forConfigurations = Utilities.GetPermissibleResourcesForConfigurations(requestContext, projectId, checkConfigurations.Cast<CheckConfigurationRef>().ToList<CheckConfigurationRef>(), ResourcePermission.Admin);
        foreach (CheckConfiguration checkConfiguration1 in (IEnumerable<CheckConfiguration>) checkConfigurations)
        {
          CheckConfiguration checkConfiguration = checkConfiguration1;
          checkConfiguration.Resource.Name = (forConfigurations.Find((Predicate<Resource>) (r => r.Equals(checkConfiguration.Resource))) ?? throw new AccessDeniedException(PipelineChecksResources.ResourceAccessDenied())).Name;
        }
        IList<CheckConfiguration> checkConfigurations1 = (IList<CheckConfiguration>) new List<CheckConfiguration>();
        if (checkConfigurations.Any<CheckConfiguration>())
        {
          using (CheckConfigurationComponent component = requestContext.CreateComponent<CheckConfigurationComponent>())
          {
            checkConfigurations1 = component.AddCheckConfigurations(checkConfigurations);
            requestContext.TraceInfo(34001916, nameof (CheckConfigurationService), "CheckConfigurationService::AddCheckConfigurations {0} Check configuration added", (object) checkConfigurations.Count<CheckConfiguration>());
          }
        }
        this.PopulateMissingDetails(requestContext, projectId, checkConfigurations1, (IList<Resource>) forConfigurations);
        return checkConfigurations1;
      }
    }

    public CheckConfiguration GetCheckConfiguration(
      IVssRequestContext requestContext,
      Guid projectId,
      int id,
      CheckConfigurationExpandParameter expand = CheckConfigurationExpandParameter.None)
    {
      this.ValidateServiceBasicData(requestContext, projectId);
      using (new MethodScope(requestContext, nameof (CheckConfigurationService), nameof (GetCheckConfiguration)))
      {
        bool includeSettings = this.ShouldIncludeSettings(expand);
        requestContext.TraceInfo(34001919, nameof (CheckConfigurationService), "CheckConfiguration::GetCheckConfiguration Get check configuration request for id: {0}", (object) id);
        CheckConfiguration checkConfiguration1;
        using (CheckConfigurationComponent component = requestContext.CreateComponent<CheckConfigurationComponent>())
          checkConfiguration1 = component.GetCheckConfiguration(id, includeSettings);
        if (checkConfiguration1 == null)
        {
          requestContext.TraceInfo(34001904, nameof (CheckConfigurationService), "CheckConfigurationService::GetCheckConfiguration Configuration not found: {0}.", (object) id);
          throw new CheckConfigurationNotFoundException(PipelineChecksResources.ChecksConfigurationNotFound((object) id));
        }
        List<CheckConfiguration> permissibleResources = this.GetCheckConfigurationsWithPermissibleResources(requestContext, projectId, new List<CheckConfiguration>()
        {
          checkConfiguration1
        }, (List<Resource>) null);
        if (permissibleResources.Count == 0)
        {
          requestContext.TraceAlways(34001915, nameof (CheckConfigurationService), "CheckConfigurationService::GetCheckConfiguration User does not have permission to view configuration on the resource");
          throw new AccessDeniedException(PipelineChecksResources.AccessDeniedException());
        }
        this.PopulateMissingDetails(requestContext, projectId, (IList<CheckConfiguration>) permissibleResources, (IList<Resource>) null, expand);
        CheckConfiguration checkConfiguration2 = permissibleResources.FirstOrDefault<CheckConfiguration>();
        requestContext.TraceInfo(34001919, nameof (CheckConfigurationService), "CheckConfigurationService::GetCheckConfiguration Returning check configuration with id: {0} of type: {1} on resource {2} with id: {3}", (object) checkConfiguration2.Id, (object) checkConfiguration2.Type.Name, (object) checkConfiguration2.Resource.Type, (object) checkConfiguration2.Resource.Id);
        return checkConfiguration2;
      }
    }

    public List<CheckConfiguration> GetCheckConfigurationsByIdVersion(
      IVssRequestContext requestContext,
      Guid projectId,
      List<CheckConfigurationRef> checkParams,
      CheckConfigurationExpandParameter expand = CheckConfigurationExpandParameter.None,
      bool includeDeletedChecks = false)
    {
      this.ValidateServiceBasicData(requestContext, projectId);
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) checkParams, nameof (checkParams), "Pipeline.Checks");
      requestContext.TraceInfo(34001919, nameof (CheckConfigurationService), "CheckConfigurationService::GetCheckConfigurationsByIdVersion for {0} requested checkParams", (object) checkParams.Count);
      using (new MethodScope(requestContext, nameof (CheckConfigurationService), nameof (GetCheckConfigurationsByIdVersion)))
      {
        bool includeSettings = this.ShouldIncludeSettings(expand);
        requestContext.TraceInfo(34001919, nameof (CheckConfigurationService), "CheckConfigurationService::GetCheckConfigurationsByIdVersion Get check configuration request for {0} checkParams.", (object) checkParams.Count);
        List<CheckConfiguration> configurationsByIdVersion;
        using (CheckConfigurationComponent component = requestContext.CreateComponent<CheckConfigurationComponent>())
          configurationsByIdVersion = component.GetCheckConfigurationsByIdVersion((IEnumerable<CheckConfigurationRef>) checkParams, includeSettings, includeDeletedChecks);
        List<CheckConfiguration> permissibleResources = this.GetCheckConfigurationsWithPermissibleResources(requestContext, projectId, configurationsByIdVersion, (List<Resource>) null);
        this.PopulateMissingDetails(requestContext, projectId, (IList<CheckConfiguration>) permissibleResources, (IList<Resource>) null, expand);
        requestContext.TraceInfo(34001919, nameof (CheckConfigurationService), "CheckConfigurationService::GetCheckConfigurationsByIdVersion Returning {0} configurations on {1} checkParams.", (object) permissibleResources.Count<CheckConfiguration>(), (object) checkParams.Count);
        return permissibleResources;
      }
    }

    public List<CheckConfiguration> GetCheckConfigurationsOnResource(
      IVssRequestContext requestContext,
      Guid projectId,
      Resource resource,
      bool includeDisabledChecks,
      CheckConfigurationExpandParameter expand = CheckConfigurationExpandParameter.None)
    {
      this.ValidateServiceBasicData(requestContext, projectId);
      requestContext.TraceInfo(34001919, nameof (CheckConfigurationService), "CheckConfigurationService::GetCheckConfigurationOnResource Get check configuration request for resource: {0} and id: {1}", (object) resource.Type, (object) resource.Id);
      IVssRequestContext requestContext1 = requestContext;
      Guid projectId1 = projectId;
      List<Resource> resources = new List<Resource>();
      resources.Add(resource);
      int num = includeDisabledChecks ? 1 : 0;
      int expand1 = (int) expand;
      List<CheckConfiguration> configurationsOnResources = this.GetCheckConfigurationsOnResources(requestContext1, projectId1, resources, num != 0, (CheckConfigurationExpandParameter) expand1, false);
      requestContext.TraceInfo(34001919, nameof (CheckConfigurationService), "CheckConfigurationService::GetCheckConfigurationOnResource Returning {0} configurations on resource {1} with id: {2}", (object) configurationsOnResources.Count<CheckConfiguration>(), (object) resource.Type, (object) resource.Id);
      return configurationsOnResources;
    }

    public List<CheckConfiguration> GetCheckConfigurationsOnResources(
      IVssRequestContext requestContext,
      Guid projectId,
      List<Resource> resources,
      bool includeDisabledChecks,
      CheckConfigurationExpandParameter expand = CheckConfigurationExpandParameter.None,
      bool includeDeletedChecks = false)
    {
      this.ValidateServiceBasicData(requestContext, projectId);
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) resources, nameof (resources), "Pipeline.Checks");
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      resources.ForEach(CheckConfigurationService.\u003C\u003EO.\u003C0\u003E__ValidateResource ?? (CheckConfigurationService.\u003C\u003EO.\u003C0\u003E__ValidateResource = new Action<Resource>(ArgumentValidation.ValidateResource)));
      requestContext.TraceInfo(34001919, nameof (CheckConfigurationService), "CheckConfigurationService::GetCheckConfigurationsOnResources for {0} requested resources", (object) resources.Count);
      List<Resource> resourcesWithPermission = ChecksUtilities.GetResourcesWithPermission(requestContext, projectId, resources);
      int count = resourcesWithPermission != null ? resourcesWithPermission.Count : 0;
      requestContext.TraceInfo(34001919, nameof (CheckConfigurationService), "CheckConfigurationService::GetCheckConfigurationsOnResources {0} out of {1} requested resources are permissible for this user.", (object) count, (object) resources.Count);
      if (count <= 0)
        return new List<CheckConfiguration>();
      using (new MethodScope(requestContext, nameof (CheckConfigurationService), nameof (GetCheckConfigurationsOnResources)))
      {
        bool includeSettings = this.ShouldIncludeSettings(expand);
        requestContext.TraceInfo(34001919, nameof (CheckConfigurationService), "CheckConfigurationService::GetCheckConfigurationsOnResources Get check configuration request for {0} resources.", (object) resourcesWithPermission.Count);
        List<CheckConfiguration> configurationsOnResources;
        using (CheckConfigurationComponent component = requestContext.CreateComponent<CheckConfigurationComponent>())
          configurationsOnResources = component.GetCheckConfigurationsOnResources(resourcesWithPermission, includeDisabledChecks, includeSettings, includeDeletedChecks);
        List<CheckConfiguration> permissibleResources = this.GetCheckConfigurationsWithPermissibleResources(requestContext, projectId, configurationsOnResources, resourcesWithPermission);
        this.PopulateMissingDetails(requestContext, projectId, (IList<CheckConfiguration>) permissibleResources, (IList<Resource>) null, expand);
        requestContext.TraceInfo(34001919, nameof (CheckConfigurationService), "CheckConfigurationService::GetCheckConfigurationsOnResources Returning {0} configurations on {1} resource.", (object) permissibleResources.Count<CheckConfiguration>(), (object) resourcesWithPermission.Count);
        return permissibleResources;
      }
    }

    public CheckConfiguration UpdateCheckConfiguration(
      IVssRequestContext requestContext,
      Guid projectId,
      int id,
      CheckConfiguration checkConfiguration)
    {
      this.ValidateServiceBasicData(requestContext, projectId);
      using (new MethodScope(requestContext, nameof (CheckConfigurationService), nameof (UpdateCheckConfiguration)))
      {
        if (!requestContext.IsFeatureEnabled("Pipelines.Checks.EnableDisabledChecksFeature"))
        {
          ArgumentUtility.CheckForNull<CheckConfiguration>(checkConfiguration, nameof (checkConfiguration), "Pipeline.Checks");
          ArgumentUtility.CheckForNonPositiveInt(id, nameof (id), "Pipeline.Checks");
          object configurationSettings = checkConfiguration.GetCheckConfigurationSettings();
          ArgumentUtility.CheckForNull<object>(configurationSettings, "checkConfigurationSettings", "Pipeline.Checks");
          ArgumentValidation.CheckSettingsLength(configurationSettings);
        }
        else
          ArgumentValidation.ValidateCheckConfigurationParametersOnUpdate(id, checkConfiguration);
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        checkConfiguration.ModifiedBy = new IdentityRef()
        {
          Id = userIdentity.Id.ToString("D")
        };
        CheckConfiguration checkConfiguration1;
        using (CheckConfigurationComponent component = requestContext.CreateComponent<CheckConfigurationComponent>())
          checkConfiguration1 = component.GetCheckConfiguration(id, true);
        if (checkConfiguration1 == null)
        {
          requestContext.TraceInfo(34001904, nameof (CheckConfigurationService), "Check configuration with id {0} is not found.", (object) id);
          throw new CheckConfigurationNotFoundException(PipelineChecksResources.ChecksConfigurationNotFound((object) id));
        }
        ICheckType checkInstance = requestContext.GetService<IChecksExtensionService>().GetCheckInstance(checkConfiguration1.Type);
        requestContext.TraceInfo(34001919, nameof (CheckConfigurationService), "CheckConfigurationService::UpdateCheckConfiguration Update Check configuration request for type: {0} with id: {1}.Updating configuration for type: {2} with id: {3} on resource: {4} with id: {5}.", (object) checkConfiguration.Type.Name, (object) checkConfiguration.Type.Id, (object) checkInstance.CheckTypeName, (object) checkInstance.CheckTypeId, (object) checkConfiguration1.Resource.Type, (object) checkConfiguration1.Resource.Id);
        try
        {
          checkConfiguration = checkInstance.ValidateAndGetCheckConfiguration(requestContext, projectId, checkConfiguration);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(34001903, nameof (CheckConfigurationService), ex);
          throw new InvalidCheckConfigurationException(ex.Message);
        }
        this.EnsureCheckConfigurationsAreValid(requestContext, (IList<CheckConfiguration>) new List<CheckConfiguration>()
        {
          checkConfiguration
        });
        Resource forConfiguration = Utilities.GetPermissibleResourceForConfiguration(requestContext, projectId, (CheckConfigurationRef) checkConfiguration1, ResourcePermission.Admin);
        checkConfiguration.Resource.Name = forConfiguration.Name;
        bool isAChangeCheckTypeRequest;
        string originalType;
        string finalType;
        this.EnsureNoDuplicateCheckTypes(requestContext, projectId, checkConfiguration1, checkConfiguration, out isAChangeCheckTypeRequest, out originalType, out finalType);
        CheckConfiguration checkConfiguration2;
        using (CheckConfigurationComponent component = requestContext.CreateComponent<CheckConfigurationComponent>())
        {
          if (requestContext.IsFeatureEnabled("Pipelines.Checks.EnableOrderingChecks") && this.ShouldUpdateVersion(checkConfiguration, checkConfiguration1))
          {
            checkConfiguration2 = component.AddNewCheckConfigurationVersion(checkConfiguration, projectId);
            requestContext.TraceInfo(34001917, nameof (CheckConfigurationService), "CheckConfigurationService::UpdateCheckConfiguration new version check configuration version created for {0}", (object) checkInstance.CheckTypeName);
          }
          else
          {
            checkConfiguration2 = component.UpdateCheckConfiguration(id, checkConfiguration, projectId);
            requestContext.TraceInfo(34001917, nameof (CheckConfigurationService), "CheckConfigurationService::UpdateCheckConfiguration Check configuration updated for {0}", (object) checkInstance.CheckTypeName);
          }
        }
        this.PopulateMissingDetails(requestContext, projectId, (IList<CheckConfiguration>) new CheckConfiguration[1]
        {
          checkConfiguration2
        }, (IList<Resource>) new List<Resource>()
        {
          forConfiguration
        });
        if (requestContext.IsFeatureEnabled("Pipelines.Checks.EnableDisabledChecksFeature") && checkConfiguration1.IsDisabled != checkConfiguration2.IsDisabled)
          CheckConfigurationService.LogCheckDisableAuditEvent(requestContext, projectId, checkConfiguration2);
        this.publishEventToTelemetry(requestContext, projectId, checkConfiguration2, "Update", checkInstance);
        if (isAChangeCheckTypeRequest && requestContext.IsFeatureEnabled("Pipelines.Checks.EnableOrderingChecks") && requestContext.IsFeatureEnabled("Pipelines.Checks.AllowChangingApprovalOrder") && checkConfiguration2.Type.Id == ApprovalCheckConstants.ApprovalCheckTypeId)
        {
          this.PublishApprovalChangeOrderEventToTelemetry(requestContext, projectId, originalType, finalType, checkConfiguration2);
          CheckConfigurationService.LogApprovalChangeOrderEvent(requestContext, projectId, originalType, finalType, checkConfiguration2);
        }
        return checkConfiguration2;
      }
    }

    private bool ShouldUpdateVersion(
      CheckConfiguration checkConfiguration,
      CheckConfiguration persistedCheckConfiguration)
    {
      if (object.Equals((object) checkConfiguration, (object) persistedCheckConfiguration))
        return false;
      if (!object.Equals((object) (int?) checkConfiguration?.Timeout, (object) (int?) persistedCheckConfiguration?.Timeout))
        return true;
      object configurationSettings1 = persistedCheckConfiguration.GetCheckConfigurationSettings();
      object configurationSettings2 = checkConfiguration.GetCheckConfigurationSettings();
      return !object.Equals((object) JsonUtility.ToString(configurationSettings1), (object) JsonUtility.ToString(configurationSettings2));
    }

    private void EnsureNoDuplicateCheckTypes(
      IVssRequestContext requestContext,
      Guid projectId,
      CheckConfiguration oldCheckConfiguration,
      CheckConfiguration newCheckConfiguration,
      out bool isAChangeCheckTypeRequest,
      out string originalType,
      out string finalType)
    {
      isAChangeCheckTypeRequest = false;
      originalType = "";
      finalType = "";
      if (!requestContext.IsFeatureEnabled("Pipelines.Checks.DoNotAllowDuplicateCheckTypes"))
        return;
      IChecksExtensionService service = requestContext.GetService<IChecksExtensionService>();
      ICheckType checkType = service.GetCheckInstance(newCheckConfiguration.Type);
      CheckDefinitionData checkDefinition = checkType.GetCheckDefinition(requestContext, newCheckConfiguration);
      if (checkDefinition == null || checkDefinition.AllowMultipleConfigurations)
        return;
      finalType = checkDefinition.Name;
      if (oldCheckConfiguration != null)
      {
        ICheckType checkInstance = service.GetCheckInstance(oldCheckConfiguration.Type);
        originalType = checkInstance.GetCheckDefinition(requestContext, oldCheckConfiguration)?.Name;
      }
      Guid? id1 = oldCheckConfiguration?.Type?.Id;
      Guid? id2 = newCheckConfiguration?.Type?.Id;
      if ((id1.HasValue == id2.HasValue ? (id1.HasValue ? (id1.GetValueOrDefault() != id2.GetValueOrDefault() ? 1 : 0) : 0) : 1) != 0)
        isAChangeCheckTypeRequest = true;
      else if (originalType != finalType)
        isAChangeCheckTypeRequest = true;
      if (!isAChangeCheckTypeRequest)
        return;
      List<CheckConfiguration> configurationsOnResource = this.GetCheckConfigurationsOnResource(requestContext, projectId, newCheckConfiguration.Resource, true, CheckConfigurationExpandParameter.Settings);
      string typeToCompare = finalType;
      Func<CheckConfiguration, bool> predicate = (Func<CheckConfiguration, bool>) (check => !(check.Type.Id != newCheckConfiguration.Type.Id) && checkType.GetCheckDefinition(requestContext, check)?.Name == typeToCompare);
      if (configurationsOnResource.Where<CheckConfiguration>(predicate).Count<CheckConfiguration>() > 0)
      {
        requestContext.TraceWarning(nameof (CheckConfigurationService), "Attempt to create a duplicate check of type {0}. Throwing an exception.", (object) finalType);
        throw new InvalidCheckConfigurationException(PipelineChecksResources.CheckTypeAlreadyExists((object) finalType));
      }
    }

    private static void LogCheckDisableAuditEvent(
      IVssRequestContext requestContext,
      Guid projectId,
      CheckConfiguration checkConfiguration)
    {
      Dictionary<string, object> dictionary = new Dictionary<string, object>()
      {
        ["CheckId"] = (object) checkConfiguration.Id,
        ["Type"] = (object) checkConfiguration.Type.Name,
        ["ResourceType"] = (object) checkConfiguration.Resource.Type,
        ["ResourceName"] = (object) checkConfiguration.Resource.Name,
        ["ResourceId"] = (object) checkConfiguration.Resource.Id,
        ["IsDisabled"] = (object) checkConfiguration.IsDisabled
      };
      IVssRequestContext requestContext1 = requestContext;
      string actionId = checkConfiguration.IsDisabled ? "CheckConfiguration.Disabled" : "CheckConfiguration.Enabled";
      Dictionary<string, object> data = dictionary;
      Guid guid = projectId;
      Guid targetHostId = new Guid();
      Guid projectId1 = guid;
      requestContext1.LogAuditEvent(actionId, data, targetHostId, projectId1);
    }

    private static void LogApprovalChangeOrderEvent(
      IVssRequestContext requestContext,
      Guid projectId,
      string originalType,
      string finalType,
      CheckConfiguration checkConfiguration)
    {
      Dictionary<string, object> dictionary = new Dictionary<string, object>()
      {
        ["CheckId"] = (object) checkConfiguration.Id,
        ["ResourceType"] = (object) checkConfiguration.Resource.Type,
        ["ResourceName"] = (object) checkConfiguration.Resource.Name,
        ["OriginalApprovalType"] = (object) originalType,
        ["FinalApprovalType"] = (object) finalType
      };
      IVssRequestContext requestContext1 = requestContext;
      Dictionary<string, object> data = dictionary;
      Guid guid = projectId;
      Guid targetHostId = new Guid();
      Guid projectId1 = guid;
      requestContext1.LogAuditEvent("CheckConfiguration.ApprovalCheckOrderChanged", data, targetHostId, projectId1);
    }

    public void DeleteCheckConfiguration(IVssRequestContext requestContext, Guid projectId, int id)
    {
      this.ValidateServiceBasicData(requestContext, projectId);
      using (new MethodScope(requestContext, nameof (CheckConfigurationService), nameof (DeleteCheckConfiguration)))
      {
        ArgumentUtility.CheckForNonPositiveInt(id, "checkConfigurationId");
        requestContext.TraceInfo(34001919, nameof (CheckConfigurationService), "CheckConfigurationService::DeleteCheckConfiguration Delete check configuration request with id: {0}", (object) id);
        CheckConfiguration checkConfiguration;
        using (CheckConfigurationComponent component = requestContext.CreateComponent<CheckConfigurationComponent>())
          checkConfiguration = component.GetCheckConfiguration(id);
        if (checkConfiguration == null)
        {
          requestContext.TraceInfo(34001904, nameof (CheckConfigurationService), "CheckConfigurationService::DeleteCheckConfiguration Configuration not found");
        }
        else
        {
          Resource forConfiguration = Utilities.GetPermissibleResourceForConfiguration(requestContext, projectId, (CheckConfigurationRef) checkConfiguration, ResourcePermission.Admin);
          ICheckType checkInstance = requestContext.GetService<IChecksExtensionService>().GetCheckInstance(checkConfiguration.Type);
          checkConfiguration.Type.Name = checkInstance.CheckTypeName;
          checkConfiguration.Resource.Name = forConfiguration.Name;
          using (CheckConfigurationComponent component = requestContext.CreateComponent<CheckConfigurationComponent>())
          {
            component.DeleteCheckConfiguration(id, checkConfiguration, projectId);
            requestContext.TraceInfo(34001918, nameof (CheckConfigurationService), "CheckConfigurationService::DeleteCheckConfiguration Check configuration deleted with id: {0} for type: {0} with id: {1} on resource: {0} with id: {1}", (object) id, (object) checkInstance.CheckTypeName, (object) checkInstance.CheckTypeId, (object) checkConfiguration.Resource.Type, (object) checkConfiguration.Resource.Id);
          }
          this.publishEventToTelemetry(requestContext, projectId, checkConfiguration, "Delete", checkInstance);
        }
      }
    }

    public void ServiceStart(IVssRequestContext requestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    private void ValidateServiceBasicData(IVssRequestContext requestContext, Guid projectId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext), "Pipeline.Checks");
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Pipeline.Checks");
    }

    private void PopulateMissingDetails(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<CheckConfiguration> checkConfigurations,
      IList<Resource> resources,
      CheckConfigurationExpandParameter expand = CheckConfigurationExpandParameter.None)
    {
      if (checkConfigurations == null || !checkConfigurations.Any<CheckConfiguration>())
        return;
      IdentityService service = requestContext.GetService<IdentityService>();
      List<string> identityIds = new List<string>();
      foreach (CheckConfiguration checkConfiguration in (IEnumerable<CheckConfiguration>) checkConfigurations)
      {
        if (checkConfiguration.CreatedBy != null && !string.IsNullOrWhiteSpace(checkConfiguration.CreatedBy.Id))
          identityIds.Add(checkConfiguration.CreatedBy.Id);
        if (checkConfiguration.ModifiedBy != null && !string.IsNullOrWhiteSpace(checkConfiguration.ModifiedBy.Id))
          identityIds.Add(checkConfiguration.ModifiedBy.Id);
      }
      IDictionary<string, IdentityRef> dictionary = service.QueryIdentities(requestContext, (IList<string>) identityIds);
      foreach (CheckConfiguration checkConfiguration1 in (IEnumerable<CheckConfiguration>) checkConfigurations)
      {
        CheckConfiguration checkConfiguration = checkConfiguration1;
        if (dictionary != null && dictionary.Count > 0)
        {
          if (checkConfiguration.CreatedBy != null && !string.IsNullOrWhiteSpace(checkConfiguration.CreatedBy.Id))
            checkConfiguration.CreatedBy = dictionary[checkConfiguration.CreatedBy.Id];
          if (checkConfiguration.ModifiedBy != null && !string.IsNullOrWhiteSpace(checkConfiguration.ModifiedBy.Id))
            checkConfiguration.ModifiedBy = dictionary[checkConfiguration.ModifiedBy.Id];
        }
        if (resources != null && resources.Count > 0)
        {
          Resource resource = resources.FirstOrDefault<Resource>((Func<Resource, bool>) (r => r.Equals(checkConfiguration.Resource)));
          checkConfiguration.Resource = resource ?? checkConfiguration.Resource;
        }
        this.PopulateReferenceLinks(requestContext, projectId, checkConfiguration, expand);
        this.PopulateTypeName(requestContext, checkConfiguration);
      }
      this.UpdateAndGetNonCompliantCheckConfigurations(requestContext, checkConfigurations);
    }

    private void PopulateReferenceLinks(
      IVssRequestContext requestContext,
      Guid projectId,
      CheckConfiguration checkConfiguration,
      CheckConfigurationExpandParameter expand)
    {
      ReferenceLinks referenceLinks = new ReferenceLinks();
      IDictionary<string, string> parameters = (IDictionary<string, string>) new Dictionary<string, string>();
      if (expand != CheckConfigurationExpandParameter.None)
        parameters.Add("$expand", expand.ToString("D"));
      string configurationUrl = checkConfiguration.GetCheckConfigurationUrl(requestContext, projectId, checkConfiguration.Id, parameters);
      referenceLinks.AddLink("self", configurationUrl);
      checkConfiguration.Links = referenceLinks;
      checkConfiguration.Url = configurationUrl;
    }

    private void PopulateTypeName(
      IVssRequestContext requestContext,
      CheckConfiguration checkConfiguration)
    {
      ICheckType checkInstance = requestContext.GetService<IChecksExtensionService>().GetCheckInstance(checkConfiguration.Type);
      checkConfiguration.Type.Name = checkInstance.CheckTypeName;
    }

    private void EnsureCheckConfigurationsAreValid(
      IVssRequestContext requestContext,
      IList<CheckConfiguration> checkConfigurations)
    {
      IEnumerable<CheckConfiguration> checkConfigurations1 = (IEnumerable<CheckConfiguration>) this.UpdateAndGetNonCompliantCheckConfigurations(requestContext, checkConfigurations);
      if (checkConfigurations1.Any<CheckConfiguration>())
        requestContext.TraceAlways(34001935, nameof (CheckConfigurationService), "Non-compliant check configurations creation/update: {0}", (object) checkConfigurations1.Select<CheckConfiguration, string>((Func<CheckConfiguration, string>) (cc => cc.ToStringEx())).Serialize<IEnumerable<string>>());
      IEnumerable<CheckConfiguration> source = !requestContext.IsFeatureEnabled("Pipelines.Checks.EnableDisabledChecksFeature") ? checkConfigurations1 : checkConfigurations1.Where<CheckConfiguration>((Func<CheckConfiguration, bool>) (cc => !cc.IsDisabled));
      if (source.Any<CheckConfiguration>() && requestContext.IsScalabilityComplianceCheckErrorFeatureEnabled())
        throw new InvalidCheckConfigurationException(PipelineChecksResources.NonCompliantCheckConfigurations((object) string.Join(", ", source.Select<CheckConfiguration, string>((Func<CheckConfiguration, string>) (cc => cc.Issue.Description)))));
    }

    private IList<CheckConfiguration> UpdateAndGetNonCompliantCheckConfigurations(
      IVssRequestContext requestContext,
      IList<CheckConfiguration> checkConfigurations)
    {
      List<CheckConfiguration> checkConfigurations1 = new List<CheckConfiguration>();
      if (checkConfigurations != null)
      {
        IChecksExtensionService extensionService = requestContext.GetService<IChecksExtensionService>();
        foreach (KeyValuePair<ICheckType, IList<CheckConfiguration>> keyValuePair in checkConfigurations.GroupBy<CheckConfiguration, ICheckType>((Func<CheckConfiguration, ICheckType>) (cc => extensionService.GetCheckInstance(cc.Type))).ToDictionary<IGrouping<ICheckType, CheckConfiguration>, ICheckType, IList<CheckConfiguration>>((Func<IGrouping<ICheckType, CheckConfiguration>, ICheckType>) (cc => cc.Key), (Func<IGrouping<ICheckType, CheckConfiguration>, IList<CheckConfiguration>>) (cc => (IList<CheckConfiguration>) cc.ToList<CheckConfiguration>())))
        {
          ICheckType key = keyValuePair.Key;
          IList<CheckConfiguration> checkConfigurations2 = keyValuePair.Value;
          checkConfigurations1.AddRange((IEnumerable<CheckConfiguration>) key.UpdateAndGetNonCompliantCheckConfigurations(requestContext, checkConfigurations2));
        }
      }
      return (IList<CheckConfiguration>) checkConfigurations1;
    }

    private List<CheckConfiguration> GetCheckConfigurationsWithPermissibleResources(
      IVssRequestContext requestContext,
      Guid projectId,
      List<CheckConfiguration> checkConfigurations,
      List<Resource> permissibleResources)
    {
      if (checkConfigurations == null || checkConfigurations.Count <= 0)
        return new List<CheckConfiguration>();
      if (permissibleResources == null)
        permissibleResources = Utilities.GetPermissibleResourcesForConfigurations(requestContext, projectId, new List<CheckConfigurationRef>((IEnumerable<CheckConfigurationRef>) checkConfigurations));
      List<CheckConfiguration> permissibleResources1;
      Dictionary<string, List<string>> dictionary;
      if (requestContext.IsFeatureEnabled("Pipelines.Policy.ResourceListOptimizations"))
        (permissibleResources1, dictionary) = CheckConfigurationService.FilterCheckConfigurationsWithPermissibleResourcesOptimized(checkConfigurations, permissibleResources);
      else
        (permissibleResources1, dictionary) = CheckConfigurationService.FilterCheckConfigurationsWithPermissibleResources(checkConfigurations, permissibleResources);
      foreach (KeyValuePair<string, List<string>> keyValuePair in dictionary)
        requestContext.TraceError(34001905, nameof (CheckConfigurationService), "Access Denied to the resource {0} - Resource Ids: {1}", (object) keyValuePair.Key, (object) string.Join(", ", (IEnumerable<string>) keyValuePair.Value));
      requestContext.TraceInfo(34001906, nameof (CheckConfigurationService), "Total check configurations: {0} - Filtered check configurations: {1}", (object) checkConfigurations.Count, (object) permissibleResources1.Count);
      return permissibleResources1;
    }

    private static (List<CheckConfiguration>, Dictionary<string, List<string>>) FilterCheckConfigurationsWithPermissibleResourcesOptimized(
      List<CheckConfiguration> checkConfigurations,
      List<Resource> permissibleResources)
    {
      List<CheckConfiguration> checkConfigurationList = new List<CheckConfiguration>();
      Dictionary<string, List<string>> accessDeniedResourceTypeToIdMap = new Dictionary<string, List<string>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      checkConfigurations.Sort((Comparison<CheckConfiguration>) ((cc1, cc2) => ChecksUtilities.CompareResources(cc1.Resource, cc2.Resource)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      permissibleResources.Sort(CheckConfigurationService.\u003C\u003EO.\u003C1\u003E__CompareResources ?? (CheckConfigurationService.\u003C\u003EO.\u003C1\u003E__CompareResources = new Comparison<Resource>(ChecksUtilities.CompareResources)));
      int index1 = 0;
      int index2 = 0;
      while (index1 < checkConfigurations.Count)
      {
        CheckConfiguration checkConfiguration = checkConfigurations[index1];
        if (index2 >= permissibleResources.Count)
        {
          CheckConfigurationService.ProcessAccessDeniedResource(accessDeniedResourceTypeToIdMap, checkConfiguration);
          ++index1;
        }
        else
        {
          Resource permissibleResource = permissibleResources[index2];
          int num = ChecksUtilities.CompareResources(checkConfiguration.Resource, permissibleResource);
          if (num > 0)
          {
            ++index2;
          }
          else
          {
            if (num == 0)
            {
              checkConfiguration.Resource = permissibleResource;
              checkConfigurationList.Add(checkConfiguration);
            }
            else
              CheckConfigurationService.ProcessAccessDeniedResource(accessDeniedResourceTypeToIdMap, checkConfiguration);
            ++index1;
          }
        }
      }
      return (checkConfigurationList, accessDeniedResourceTypeToIdMap);
    }

    private static (List<CheckConfiguration>, Dictionary<string, List<string>>) FilterCheckConfigurationsWithPermissibleResources(
      List<CheckConfiguration> checkConfigurations,
      List<Resource> permissibleResources)
    {
      List<CheckConfiguration> checkConfigurationList = new List<CheckConfiguration>();
      Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (CheckConfiguration checkConfiguration in checkConfigurations)
      {
        CheckConfiguration config = checkConfiguration;
        string lowerInvariant1 = config.Resource.Id.ToLowerInvariant();
        string lowerInvariant2 = config.Resource.Type.ToLowerInvariant();
        Resource resource = permissibleResources.FirstOrDefault<Resource>((Func<Resource, bool>) (r => r.Equals(config.Resource)));
        if (resource != null)
        {
          config.Resource = resource;
          checkConfigurationList.Add(config);
        }
        else
        {
          List<string> stringList;
          if (dictionary.TryGetValue(lowerInvariant2, out stringList))
          {
            stringList.Add(lowerInvariant1);
          }
          else
          {
            stringList = new List<string>()
            {
              lowerInvariant1
            };
            dictionary[lowerInvariant2] = stringList;
          }
        }
      }
      return (checkConfigurationList, dictionary);
    }

    private static void ProcessAccessDeniedResource(
      Dictionary<string, List<string>> accessDeniedResourceTypeToIdMap,
      CheckConfiguration currentConfig)
    {
      string lowerInvariant1 = currentConfig.Resource.Id.ToLowerInvariant();
      string lowerInvariant2 = currentConfig.Resource.Type.ToLowerInvariant();
      List<string> stringList1;
      if (accessDeniedResourceTypeToIdMap.TryGetValue(lowerInvariant2, out stringList1))
      {
        stringList1.Add(lowerInvariant1);
      }
      else
      {
        List<string> stringList2 = new List<string>()
        {
          lowerInvariant1
        };
        accessDeniedResourceTypeToIdMap[lowerInvariant2] = stringList2;
      }
    }

    private void publishEventToTelemetry(
      IVssRequestContext requestContext,
      Guid projectId,
      CheckConfiguration checkConfiguration,
      string action,
      ICheckType checkType)
    {
      try
      {
        string feature = string.Format("{0}.{1}", (object) nameof (CheckConfigurationService), (object) action);
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        string type = checkConfiguration.Resource.Type;
        string id = checkConfiguration.Resource.Id;
        int? nullable = checkConfiguration.Timeout.HasValue ? checkConfiguration.Timeout : new int?(-1);
        string str1 = checkType.CheckTypeName;
        if (requestContext.IsFeatureEnabled("Pipelines.Checks.EnableOrderingChecks") && checkConfiguration.Type.Id == ApprovalCheckConstants.ApprovalCheckTypeId)
          str1 = checkType.GetCheckDefinition(requestContext, checkConfiguration)?.Name ?? checkType.CheckTypeName;
        string str2 = string.Format("{0}:{1}:{2}", (object) projectId, (object) type, (object) id);
        properties.Add("ProjectAndResourceIdentifier", str2);
        properties.Add("Check type", str1);
        properties.Add("Timeout", nullable.ToString());
        Dictionary<string, string> telemetryData = checkType.GetTelemetryData(checkConfiguration);
        if (telemetryData != null)
        {
          foreach (KeyValuePair<string, string> keyValuePair in telemetryData)
            properties.Add(keyValuePair.Key, keyValuePair.Value);
        }
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Pipeline.Checks", feature, properties);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(34001908, nameof (CheckConfigurationService), ex);
      }
    }

    private void PublishApprovalChangeOrderEventToTelemetry(
      IVssRequestContext requestContext,
      Guid projectId,
      string originalType,
      string finalType,
      CheckConfiguration checkConfiguration)
    {
      try
      {
        string feature = string.Format("{0}.{1}", (object) nameof (CheckConfigurationService), (object) "ChangeApprovalOrder");
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        string str = string.Format("{0}:{1}:{2}", (object) projectId, (object) checkConfiguration.Resource.Type, (object) checkConfiguration.Resource.Id);
        properties.Add("ProjectAndResourceIdentifier", str);
        properties.Add("ApprovalCheckOriginalType", originalType);
        properties.Add("ApprovalCheckFinalType", finalType);
        properties.Add("CheckId", (double) checkConfiguration.Id);
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Pipeline.Checks", feature, properties);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(34001908, nameof (CheckConfigurationService), ex);
      }
    }

    private bool ShouldIncludeSettings(CheckConfigurationExpandParameter expand) => (expand & CheckConfigurationExpandParameter.Settings) != 0;
  }
}

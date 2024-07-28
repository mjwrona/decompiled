// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services.ReleaseSettingsService
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services
{
  public class ReleaseSettingsService : ReleaseManagement2ServiceBase
  {
    private readonly Guid releaseSettingsArtifact = new Guid("4FCE6B7C-0B39-4D75-BC07-8F194F70F663");

    public ReleaseSettings GetReleaseSettings(IVssRequestContext context, Guid projectId)
    {
      ReleaseSettings releaseSettings = new ReleaseSettings();
      List<string> stringList = new List<string>()
      {
        "RetentionSettings",
        "ComplianceSettings"
      };
      ITeamFoundationPropertyService service = context.GetService<ITeamFoundationPropertyService>();
      ArtifactSpec artifactSpec1 = new ArtifactSpec(this.releaseSettingsArtifact, "ReleaseManagement.ReleaseSettings", 0, projectId);
      IVssRequestContext requestContext = context;
      ArtifactSpec artifactSpec2 = artifactSpec1;
      List<string> propertyNameFilters = stringList;
      using (TeamFoundationDataReader properties = service.GetProperties(requestContext, artifactSpec2, (IEnumerable<string>) propertyNameFilters))
      {
        if (properties != null)
        {
          foreach (ArtifactPropertyValue current in properties.CurrentEnumerable<ArtifactPropertyValue>())
          {
            foreach (PropertyValue propertyValue in new List<PropertyValue>((IEnumerable<PropertyValue>) current.PropertyValues))
            {
              if (propertyValue.PropertyName == "RetentionSettings")
                releaseSettings.RetentionSettings = JsonConvert.DeserializeObject<RetentionSettings>((string) propertyValue.Value);
              else if (propertyValue.PropertyName == "ComplianceSettings")
                releaseSettings.ComplianceSettings = JsonConvert.DeserializeObject<ComplianceSettings>((string) propertyValue.Value);
            }
          }
        }
      }
      ReleaseSettingsService.ValidateAndFixReleaseSettingsForGet(context, releaseSettings);
      return releaseSettings;
    }

    public ReleaseSettings UpdateReleaseSettings(
      IVssRequestContext context,
      Guid projectId,
      ReleaseSettings releaseSettings)
    {
      if (releaseSettings == null)
        throw new ArgumentNullException(nameof (releaseSettings));
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      ReleaseSettingsService.CheckManageReleaseSettingsPermissions(context, projectId);
      ReleaseSettings releaseSettings1 = this.GetReleaseSettings(context, projectId);
      ReleaseSettingsService.ValidateAndFixReleaseSettingsForUpdate(context, releaseSettings1, releaseSettings);
      string str1 = JsonConvert.SerializeObject((object) releaseSettings.RetentionSettings);
      string str2 = JsonConvert.SerializeObject((object) releaseSettings.ComplianceSettings);
      IList<PropertyValue> propertyValueList1 = (IList<PropertyValue>) new List<PropertyValue>();
      propertyValueList1.Add(new PropertyValue("RetentionSettings", (object) str1));
      propertyValueList1.Add(new PropertyValue("ComplianceSettings", (object) str2));
      ITeamFoundationPropertyService service = context.GetService<ITeamFoundationPropertyService>();
      ArtifactSpec artifactSpec1 = new ArtifactSpec(this.releaseSettingsArtifact, "ReleaseManagement.ReleaseSettings", 0, projectId);
      IVssRequestContext requestContext = context;
      ArtifactSpec artifactSpec2 = artifactSpec1;
      IList<PropertyValue> propertyValueList2 = propertyValueList1;
      service.SetProperties(requestContext, artifactSpec2, (IEnumerable<PropertyValue>) propertyValueList2);
      return releaseSettings;
    }

    internal static void ValidateAndFixReleaseSettingsForGet(
      IVssRequestContext context,
      ReleaseSettings releaseSettings)
    {
      if (releaseSettings == null)
        throw new ArgumentNullException(nameof (releaseSettings));
      if (releaseSettings.RetentionSettings == null)
        releaseSettings.RetentionSettings = new RetentionSettings();
      if (releaseSettings.ComplianceSettings == null)
        releaseSettings.ComplianceSettings = new ComplianceSettings();
      if (!releaseSettings.ComplianceSettings.CheckForCredentialsAndOtherSecrets.HasValue)
        releaseSettings.ComplianceSettings.CheckForCredentialsAndOtherSecrets = new bool?(context.IsMicrosoftTenant());
      if (releaseSettings.RetentionSettings.MaximumEnvironmentRetentionPolicy == null)
      {
        releaseSettings.RetentionSettings.MaximumEnvironmentRetentionPolicy = new EnvironmentRetentionPolicy()
        {
          DaysToKeep = 365,
          ReleasesToKeep = 25,
          RetainBuild = true
        };
      }
      else
      {
        if (releaseSettings.RetentionSettings.MaximumEnvironmentRetentionPolicy.DaysToKeep <= 0)
          releaseSettings.RetentionSettings.MaximumEnvironmentRetentionPolicy.DaysToKeep = 365;
        if (releaseSettings.RetentionSettings.MaximumEnvironmentRetentionPolicy.ReleasesToKeep <= 0)
          releaseSettings.RetentionSettings.MaximumEnvironmentRetentionPolicy.ReleasesToKeep = 25;
      }
      if (releaseSettings.RetentionSettings.DefaultEnvironmentRetentionPolicy == null)
      {
        releaseSettings.RetentionSettings.DefaultEnvironmentRetentionPolicy = new EnvironmentRetentionPolicy()
        {
          DaysToKeep = 30,
          ReleasesToKeep = 3,
          RetainBuild = true
        };
      }
      else
      {
        if (releaseSettings.RetentionSettings.DefaultEnvironmentRetentionPolicy.DaysToKeep <= 0 || releaseSettings.RetentionSettings.DefaultEnvironmentRetentionPolicy.DaysToKeep > releaseSettings.RetentionSettings.MaximumEnvironmentRetentionPolicy.DaysToKeep)
          releaseSettings.RetentionSettings.DefaultEnvironmentRetentionPolicy.DaysToKeep = 30;
        if (releaseSettings.RetentionSettings.DefaultEnvironmentRetentionPolicy.ReleasesToKeep <= 0 || releaseSettings.RetentionSettings.DefaultEnvironmentRetentionPolicy.ReleasesToKeep > releaseSettings.RetentionSettings.MaximumEnvironmentRetentionPolicy.ReleasesToKeep)
          releaseSettings.RetentionSettings.DefaultEnvironmentRetentionPolicy.ReleasesToKeep = 3;
      }
      if (releaseSettings.RetentionSettings.DaysToKeepDeletedReleases > 0)
        return;
      releaseSettings.RetentionSettings.DaysToKeepDeletedReleases = 14;
    }

    internal static void ValidateAndFixReleaseSettingsForUpdate(
      IVssRequestContext context,
      ReleaseSettings existingReleaseSettings,
      ReleaseSettings newReleaseSettings)
    {
      bool hostedDeployment = context.ExecutionEnvironment.IsHostedDeployment;
      bool isReleaseRetentionSettingsEditEnabled = context.IsFeatureEnabled("VisualStudio.ReleaseManagement.ReleaseRetentionSettingsEditEnabled");
      bool isReleaseRetentionSettingsMaxDaysToKeepEditEnabled = context.IsFeatureEnabled("VisualStudio.ReleaseManagement.ReleaseRetentionSettingsMaxDaysToKeepEditEnabled");
      ReleaseSettingsService.ValidateAndFixReleaseSettingsForNullValues(existingReleaseSettings, newReleaseSettings);
      ReleaseSettingsService.ValidateAndFixMaximumRetentionPolicySettings(context, newReleaseSettings, existingReleaseSettings, hostedDeployment, isReleaseRetentionSettingsMaxDaysToKeepEditEnabled, isReleaseRetentionSettingsEditEnabled);
      ReleaseSettingsService.ValidateAndFixDefaultRetentionPolicySettings(newReleaseSettings, existingReleaseSettings, hostedDeployment, isReleaseRetentionSettingsMaxDaysToKeepEditEnabled, isReleaseRetentionSettingsEditEnabled);
      ReleaseSettingsService.ValidateAndFixDaysToKeepDeletedReleases(newReleaseSettings, existingReleaseSettings, hostedDeployment, isReleaseRetentionSettingsMaxDaysToKeepEditEnabled, isReleaseRetentionSettingsEditEnabled);
      ReleaseSettingsService.ValidateComplianceSettings(newReleaseSettings, existingReleaseSettings, hostedDeployment, isReleaseRetentionSettingsEditEnabled);
    }

    private static void ValidateComplianceSettings(
      ReleaseSettings newReleaseSettings,
      ReleaseSettings existingReleaseSettings,
      bool isHostedDepolyment,
      bool isReleaseRetentionSettingsEditEnabled)
    {
      if (!isHostedDepolyment || isReleaseRetentionSettingsEditEnabled)
        return;
      bool? credentialsAndOtherSecrets1 = newReleaseSettings.ComplianceSettings.CheckForCredentialsAndOtherSecrets;
      bool? credentialsAndOtherSecrets2 = existingReleaseSettings.ComplianceSettings.CheckForCredentialsAndOtherSecrets;
      if (!(credentialsAndOtherSecrets1.GetValueOrDefault() == credentialsAndOtherSecrets2.GetValueOrDefault() & credentialsAndOtherSecrets1.HasValue == credentialsAndOtherSecrets2.HasValue))
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.ReleaseSettingsOnlyDefaultRetainBuildUpdateAllowedForHostedSetup));
    }

    private static void ValidateAndFixReleaseSettingsForNullValues(
      ReleaseSettings existingReleaseSettings,
      ReleaseSettings newReleaseSettings)
    {
      if (newReleaseSettings.RetentionSettings == null)
      {
        newReleaseSettings.RetentionSettings = existingReleaseSettings.RetentionSettings;
      }
      else
      {
        if (newReleaseSettings.RetentionSettings.MaximumEnvironmentRetentionPolicy == null)
          newReleaseSettings.RetentionSettings.MaximumEnvironmentRetentionPolicy = existingReleaseSettings.RetentionSettings.MaximumEnvironmentRetentionPolicy;
        if (newReleaseSettings.RetentionSettings.DefaultEnvironmentRetentionPolicy == null)
          newReleaseSettings.RetentionSettings.DefaultEnvironmentRetentionPolicy = existingReleaseSettings.RetentionSettings.DefaultEnvironmentRetentionPolicy;
        if (newReleaseSettings.ComplianceSettings != null)
          return;
        newReleaseSettings.ComplianceSettings = existingReleaseSettings.ComplianceSettings;
      }
    }

    private static void ValidateAndFixMaximumRetentionPolicySettings(
      IVssRequestContext context,
      ReleaseSettings newReleaseSettings,
      ReleaseSettings existingReleaseSettings,
      bool isHostedDepolyment,
      bool isReleaseRetentionSettingsMaxDaysToKeepEditEnabled,
      bool isReleaseRetentionSettingsEditEnabled)
    {
      if (isHostedDepolyment)
      {
        if (isReleaseRetentionSettingsMaxDaysToKeepEditEnabled)
        {
          int num = context.GetService<IVssRegistryService>().GetValue<int>(context, (RegistryQuery) "/Service/ReleaseManagement/RetentionExtendedMaximumDaysToKeepForVsts", 2555);
          if (newReleaseSettings.RetentionSettings.MaximumEnvironmentRetentionPolicy.DaysToKeep > num)
            newReleaseSettings.RetentionSettings.MaximumEnvironmentRetentionPolicy.DaysToKeep = num;
          else if (newReleaseSettings.RetentionSettings.MaximumEnvironmentRetentionPolicy.DaysToKeep <= 0)
            newReleaseSettings.RetentionSettings.MaximumEnvironmentRetentionPolicy.DaysToKeep = existingReleaseSettings.RetentionSettings.MaximumEnvironmentRetentionPolicy.DaysToKeep;
        }
        else if (isReleaseRetentionSettingsEditEnabled)
        {
          if (newReleaseSettings.RetentionSettings.MaximumEnvironmentRetentionPolicy.DaysToKeep > 730)
            newReleaseSettings.RetentionSettings.MaximumEnvironmentRetentionPolicy.DaysToKeep = 730;
          else if (newReleaseSettings.RetentionSettings.MaximumEnvironmentRetentionPolicy.DaysToKeep <= 0)
            newReleaseSettings.RetentionSettings.MaximumEnvironmentRetentionPolicy.DaysToKeep = existingReleaseSettings.RetentionSettings.MaximumEnvironmentRetentionPolicy.DaysToKeep;
        }
        else if (newReleaseSettings.RetentionSettings.MaximumEnvironmentRetentionPolicy.DaysToKeep != existingReleaseSettings.RetentionSettings.MaximumEnvironmentRetentionPolicy.DaysToKeep)
          throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.ReleaseSettingsOnlyDefaultRetainBuildUpdateAllowedForHostedSetup));
        if (isReleaseRetentionSettingsEditEnabled && !isReleaseRetentionSettingsMaxDaysToKeepEditEnabled)
        {
          if (newReleaseSettings.RetentionSettings.MaximumEnvironmentRetentionPolicy.ReleasesToKeep > 25)
            newReleaseSettings.RetentionSettings.MaximumEnvironmentRetentionPolicy.ReleasesToKeep = 25;
          else if (newReleaseSettings.RetentionSettings.MaximumEnvironmentRetentionPolicy.ReleasesToKeep <= 0)
            newReleaseSettings.RetentionSettings.MaximumEnvironmentRetentionPolicy.ReleasesToKeep = existingReleaseSettings.RetentionSettings.MaximumEnvironmentRetentionPolicy.ReleasesToKeep;
        }
        else if (newReleaseSettings.RetentionSettings.MaximumEnvironmentRetentionPolicy.ReleasesToKeep != existingReleaseSettings.RetentionSettings.MaximumEnvironmentRetentionPolicy.ReleasesToKeep)
          ReleaseSettingsService.ThrowInvalidRequestException(isReleaseRetentionSettingsMaxDaysToKeepEditEnabled);
        if (existingReleaseSettings.RetentionSettings.MaximumEnvironmentRetentionPolicy.RetainBuild != newReleaseSettings.RetentionSettings.MaximumEnvironmentRetentionPolicy.RetainBuild)
          throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.ReleaseSettingsOnlyDefaultRetainBuildUpdateAllowedForHostedSetup));
      }
      else
      {
        if (newReleaseSettings.RetentionSettings.MaximumEnvironmentRetentionPolicy.DaysToKeep <= 0)
          newReleaseSettings.RetentionSettings.MaximumEnvironmentRetentionPolicy.DaysToKeep = existingReleaseSettings.RetentionSettings.MaximumEnvironmentRetentionPolicy.DaysToKeep;
        if (newReleaseSettings.RetentionSettings.MaximumEnvironmentRetentionPolicy.ReleasesToKeep > 0)
          return;
        newReleaseSettings.RetentionSettings.MaximumEnvironmentRetentionPolicy.ReleasesToKeep = existingReleaseSettings.RetentionSettings.MaximumEnvironmentRetentionPolicy.ReleasesToKeep;
      }
    }

    private static void ThrowInvalidRequestException(
      bool isReleaseRetentionSettingsMaxDaysToKeepEditEnabled)
    {
      throw new InvalidRequestException(isReleaseRetentionSettingsMaxDaysToKeepEditEnabled ? Resources.ReleaseSettingsOnlyMaximumRetentionDaysAndDefaultRetainBuildUpdateAllowedForHostedSetup : Resources.ReleaseSettingsOnlyDefaultRetainBuildUpdateAllowedForHostedSetup);
    }

    private static void ValidateAndFixDefaultRetentionPolicySettings(
      ReleaseSettings newReleaseSettings,
      ReleaseSettings existingReleaseSettings,
      bool isHostedDepolyment,
      bool isReleaseRetentionSettingsMaxDaysToKeepEditEnabled,
      bool isReleaseRetentionSettingsEditEnabled)
    {
      if (isHostedDepolyment)
      {
        if (isReleaseRetentionSettingsEditEnabled && !isReleaseRetentionSettingsMaxDaysToKeepEditEnabled)
        {
          ReleaseSettingsService.ValidateAndFixDefaultRetentionPolicySettingsWhenEditIsAllowed(newReleaseSettings);
        }
        else
        {
          if (newReleaseSettings.RetentionSettings.DefaultEnvironmentRetentionPolicy.DaysToKeep == existingReleaseSettings.RetentionSettings.DefaultEnvironmentRetentionPolicy.DaysToKeep && newReleaseSettings.RetentionSettings.DefaultEnvironmentRetentionPolicy.ReleasesToKeep == existingReleaseSettings.RetentionSettings.DefaultEnvironmentRetentionPolicy.ReleasesToKeep)
            return;
          ReleaseSettingsService.ThrowInvalidRequestException(isReleaseRetentionSettingsMaxDaysToKeepEditEnabled);
        }
      }
      else
        ReleaseSettingsService.ValidateAndFixDefaultRetentionPolicySettingsWhenEditIsAllowed(newReleaseSettings);
    }

    private static void ValidateAndFixDefaultRetentionPolicySettingsWhenEditIsAllowed(
      ReleaseSettings newReleaseSettings)
    {
      if (newReleaseSettings.RetentionSettings.DefaultEnvironmentRetentionPolicy.DaysToKeep > newReleaseSettings.RetentionSettings.MaximumEnvironmentRetentionPolicy.DaysToKeep)
        newReleaseSettings.RetentionSettings.DefaultEnvironmentRetentionPolicy.DaysToKeep = newReleaseSettings.RetentionSettings.MaximumEnvironmentRetentionPolicy.DaysToKeep;
      else if (newReleaseSettings.RetentionSettings.DefaultEnvironmentRetentionPolicy.DaysToKeep <= 0)
        newReleaseSettings.RetentionSettings.DefaultEnvironmentRetentionPolicy.DaysToKeep = 30;
      if (newReleaseSettings.RetentionSettings.DefaultEnvironmentRetentionPolicy.ReleasesToKeep > newReleaseSettings.RetentionSettings.MaximumEnvironmentRetentionPolicy.ReleasesToKeep)
      {
        newReleaseSettings.RetentionSettings.DefaultEnvironmentRetentionPolicy.ReleasesToKeep = newReleaseSettings.RetentionSettings.MaximumEnvironmentRetentionPolicy.ReleasesToKeep;
      }
      else
      {
        if (newReleaseSettings.RetentionSettings.DefaultEnvironmentRetentionPolicy.ReleasesToKeep > 0)
          return;
        newReleaseSettings.RetentionSettings.DefaultEnvironmentRetentionPolicy.ReleasesToKeep = 3;
      }
    }

    private static void ValidateAndFixDaysToKeepDeletedReleases(
      ReleaseSettings newReleaseSettings,
      ReleaseSettings existingReleaseSettings,
      bool isHostedDepolyment,
      bool isReleaseRetentionSettingsMaxDaysToKeepEditEnabled,
      bool isReleaseRetentionSettingsEditEnabled)
    {
      if (isHostedDepolyment)
      {
        if (isReleaseRetentionSettingsEditEnabled && !isReleaseRetentionSettingsMaxDaysToKeepEditEnabled)
        {
          if (newReleaseSettings.RetentionSettings.DaysToKeepDeletedReleases > 14)
          {
            newReleaseSettings.RetentionSettings.DaysToKeepDeletedReleases = 14;
          }
          else
          {
            if (newReleaseSettings.RetentionSettings.DaysToKeepDeletedReleases > 0)
              return;
            newReleaseSettings.RetentionSettings.DaysToKeepDeletedReleases = 14;
          }
        }
        else
        {
          if (newReleaseSettings.RetentionSettings.DaysToKeepDeletedReleases == existingReleaseSettings.RetentionSettings.DaysToKeepDeletedReleases)
            return;
          ReleaseSettingsService.ThrowInvalidRequestException(isReleaseRetentionSettingsMaxDaysToKeepEditEnabled);
        }
      }
      else
      {
        if (newReleaseSettings.RetentionSettings.DaysToKeepDeletedReleases > 0)
          return;
        newReleaseSettings.RetentionSettings.DaysToKeepDeletedReleases = 14;
      }
    }

    private static void CheckManageReleaseSettingsPermissions(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      if (!requestContext.HasPermission(projectId, (string) null, -1, ReleaseManagementSecurityPermissions.ManageReleaseSettings))
      {
        string str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ReleaseSettingsSecurityErrorMessage, (object) requestContext.RootContext.GetUserId());
        throw new UnauthorizedRequestException(str, (Exception) new ResourceAccessException(str));
      }
    }
  }
}

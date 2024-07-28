// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.EnvironmentTemplates.EnvironmentTemplateExtensionsRetriever
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.EnvironmentTemplates
{
  public static class EnvironmentTemplateExtensionsRetriever
  {
    private const string TemplatesContributionId = "ms.vss-environmenttemplate.environment-templates";

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We don't want to throw if loading a single template failed. We log the error and move on")]
    public static IList<ReleaseDefinitionEnvironmentTemplate> GetExtensionEnvironmentTemplates(
      IVssRequestContext requestContext)
    {
      List<ReleaseDefinitionEnvironmentTemplate> environmentTemplates = new List<ReleaseDefinitionEnvironmentTemplate>();
      foreach (Contribution templateContribution in EnvironmentTemplateExtensionsRetriever.GetInstalledTemplateContributions(requestContext))
      {
        try
        {
          ReleaseDefinitionEnvironmentTemplate environmentTemplate = templateContribution.ToEnvironmentTemplate(requestContext);
          environmentTemplates.Add(environmentTemplate);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(1976385, TraceLevel.Info, "ReleaseManagementService", "Service", ex);
        }
      }
      return (IList<ReleaseDefinitionEnvironmentTemplate>) environmentTemplates;
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "This is to make sure that RM calls do not fail when EMS is not available")]
    private static IEnumerable<Contribution> GetInstalledTemplateContributions(
      IVssRequestContext requestContext)
    {
      try
      {
        return requestContext.GetService<IContributionService>().QueryContributionsForTarget(requestContext, "ms.vss-environmenttemplate.environment-templates");
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1976385, TraceLevel.Error, "ReleaseManagementService", "Service", ex);
        return (IEnumerable<Contribution>) new List<Contribution>();
      }
    }

    private static ReleaseDefinitionEnvironmentTemplate ToEnvironmentTemplate(
      this Contribution environmentTemplateContribution,
      IVssRequestContext requestContext)
    {
      EnvironmentTemplateContributionDefinition templateContributionDefinition = environmentTemplateContribution.Properties.ToObject<EnvironmentTemplateContributionDefinition>();
      return new ReleaseDefinitionEnvironmentTemplate()
      {
        Description = templateContributionDefinition.Description,
        Name = templateContributionDefinition.Name,
        Id = templateContributionDefinition.Id,
        Category = "Deployment",
        Environment = EnvironmentTemplateExtensionsRetriever.InitializeEnvironmentForTemplate(templateContributionDefinition),
        IconUri = EnvironmentTemplateExtensionsRetriever.GetIconUri(requestContext, environmentTemplateContribution)
      };
    }

    private static Uri GetIconUri(
      IVssRequestContext requestContext,
      Contribution environmentTemplateContribution)
    {
      if (environmentTemplateContribution.Properties["icon"] == null)
        return (Uri) null;
      string assetType = environmentTemplateContribution.Properties["icon"].ToString();
      Uri result;
      Uri.TryCreate(requestContext.GetService<IContributionService>().QueryAssetLocation(requestContext, environmentTemplateContribution.Id, assetType), UriKind.Absolute, out result);
      return result;
    }

    private static ReleaseDefinitionEnvironment InitializeEnvironmentForTemplate(
      EnvironmentTemplateContributionDefinition templateContributionDefinition)
    {
      return new ReleaseDefinitionEnvironment()
      {
        Id = 0,
        Name = string.Empty,
        PreDeployApprovals = {
          Approvals = {
            new ReleaseDefinitionApprovalStep()
            {
              Rank = 1,
              IsAutomated = true,
              IsNotificationOn = false
            }
          }
        },
        PostDeployApprovals = {
          Approvals = {
            new ReleaseDefinitionApprovalStep()
            {
              Rank = 1,
              IsAutomated = true,
              IsNotificationOn = false
            }
          }
        },
        DeployPhases = templateContributionDefinition.DeployPhases,
        Variables = templateContributionDefinition.Variables
      };
    }
  }
}

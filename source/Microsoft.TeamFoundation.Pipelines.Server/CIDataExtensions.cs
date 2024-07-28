// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.CIDataExtensions
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Pipelines.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExternalEvent;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public static class CIDataExtensions
  {
    public static void AddDefinitionData(
      this CustomerIntelligenceData ciData,
      IVssRequestContext requestContext,
      BuildDefinition definition)
    {
      ArgumentUtility.CheckForNull<CustomerIntelligenceData>(ciData, nameof (ciData));
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<BuildDefinition>(definition, nameof (definition));
      ArgumentUtility.CheckForNull<PropertiesCollection>(definition.Properties, "Properties");
      ciData.Add("AccountId", (object) requestContext.ServiceHost.CollectionServiceHost.InstanceId);
      ciData.Add("ProjectId", (object) definition.ProjectId);
      ciData.Add("ProviderId", definition.Properties.GetValue<string>(PipelineConstants.DefinitionPropertyProviderKey, string.Empty));
      ciData.Add("DefinitionId", (double) definition.Id);
      ciData.Add("DefinitionVersion", (object) definition.Revision);
      ciData.Add("DefinitionName", definition.Name);
      ciData.Add("DefinitionProperties", definition.Properties.Serialize<PropertiesCollection>(true));
      if (definition.Repository == null)
        return;
      ciData.AddRepositoryData(requestContext, definition.Repository);
    }

    public static void AddBuildData(
      this CustomerIntelligenceData ciData,
      IVssRequestContext requestContext,
      IReadOnlyBuildData build,
      BuildDefinition buildDefinition)
    {
      ArgumentUtility.CheckForNull<CustomerIntelligenceData>(ciData, nameof (ciData));
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IReadOnlyBuildData>(build, nameof (build));
      ArgumentUtility.CheckForNull<PropertiesCollection>(build.Properties, "Properties");
      ciData.AddDefinitionData(requestContext, buildDefinition);
      ciData.Add("BuildId", (double) build.Id);
      ciData.Add("BuildNumber", build.BuildNumber);
      ciData.Add("BuildType", nameof (build));
      ciData.Add("BuildUri", (object) build.Uri);
      ciData.Add("BuildResult", (object) build.Result);
      ciData.AddPropertyIfFound(build.TriggerInfo, "pr.isFork", "PullRequestIsFork");
      ciData.Add("BuildProperties", build.Properties.Serialize<PropertiesCollection>(true));
    }

    public static void AddProjectData(
      this CustomerIntelligenceData ciData,
      CreatePipelineConnectionInputs inputs)
    {
      ArgumentUtility.CheckForNull<CustomerIntelligenceData>(ciData, nameof (ciData));
      ArgumentUtility.CheckForNull<CreatePipelineConnectionInputs>(inputs, nameof (inputs));
      ArgumentUtility.CheckForNull<Dictionary<string, string>>(inputs.ProviderData, "ProviderData");
      ciData.Add("AccountId", inputs.ProviderData[PipelineConstants.ProviderAccountIdKey]);
      ciData.Add("ProjectId", (object) inputs.Project.Id);
      ciData.Add("ProviderId", inputs.ProviderId);
      ciData.Add("UserId", inputs.ProviderData[PipelineConstants.ProviderDataVstsUserKey]);
    }

    public static void AddConnectionData(
      this CustomerIntelligenceData ciData,
      PipelineConnection connection,
      CreatePipelineConnectionInputs inputs)
    {
      ArgumentUtility.CheckForNull<CustomerIntelligenceData>(ciData, nameof (ciData));
      ArgumentUtility.CheckForNull<PipelineConnection>(connection, nameof (connection));
      ArgumentUtility.CheckForNull<CreatePipelineConnectionInputs>(inputs, nameof (inputs));
      ArgumentUtility.CheckForNull<Dictionary<string, string>>(inputs.ProviderData, "ProviderData");
      ciData.Add("AccountId", (object) connection.AccountId);
      ciData.Add("ProjectId", (object) connection.TeamProjectId);
      ciData.AddProjectData(inputs);
    }

    public static void AddRepositoryData(
      this CustomerIntelligenceData ciData,
      IVssRequestContext requestContext,
      BuildRepository repository)
    {
      ArgumentUtility.CheckForNull<CustomerIntelligenceData>(ciData, nameof (ciData));
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<BuildRepository>(repository, nameof (repository));
      ArgumentUtility.CheckForNull<IDictionary<string, string>>(repository.Properties, "Properties");
      ciData.Add("RepositoryType", repository.Type);
      ciData.Add("RepositoryUrl", CustomerIntelligenceAnonymizer.AnonymizeRepositoryUri(repository.Url));
      ciData.AddPropertyIfFound(repository.Properties, "fullName", "RepositoryFullName");
      ciData.AddPropertyIfFound(repository.Properties, "isPrivate", "RepositoryIsPrivate");
    }

    public static CustomerIntelligenceData AddSourceRepositoryData(
      this CustomerIntelligenceData ciData,
      IVssRequestContext requestContext,
      SourceRepository sourceRepository)
    {
      ArgumentUtility.CheckForNull<CustomerIntelligenceData>(ciData, nameof (ciData));
      ArgumentUtility.CheckForNull<SourceRepository>(sourceRepository, nameof (sourceRepository));
      ciData.Add("RepositoryFullName", CIDataExtensions.GetRepositoryProperty(sourceRepository, "safeRepository", "fullName"));
      ciData.Add("RepositoryId", CIDataExtensions.GetRepositoryProperty(sourceRepository, "safeId", defaultValue: sourceRepository.Id));
      ciData.Add("RepositoryIsFork", CIDataExtensions.GetRepositoryProperty(sourceRepository, "isFork"));
      ciData.Add("RepositoryIsPrivate", CIDataExtensions.GetRepositoryProperty(sourceRepository, "isPrivate"));
      ciData.Add("RepositoryIsUserRepo", CIDataExtensions.GetRepositoryProperty(sourceRepository, "ownerIsAUser"));
      ciData.Add("RepositoryLanguages", CIDataExtensions.GetRepositoryProperty(sourceRepository, "languages"));
      ciData.Add("RepositoryName", CIDataExtensions.GetRepositoryProperty(sourceRepository, "shortName"));
      ciData.Add("RepositoryOrg", CIDataExtensions.GetRepositoryProperty(sourceRepository, "ownerId"));
      ciData.Add("RepositoryPrimaryLanguage", CIDataExtensions.GetRepositoryProperty(sourceRepository, "primaryLanguage"));
      ciData.Add("RepositoryType", sourceRepository.SourceProviderName);
      ciData.Add("RepositoryUrl", CustomerIntelligenceAnonymizer.AnonymizeRepositoryUri(sourceRepository.Url?.AbsoluteUri));
      ciData.Add("RepositoryUrlHash", CustomerIntelligenceAnonymizer.AnonymizeRepositoryUri(sourceRepository.Url?.AbsoluteUri, true));
      return ciData;
    }

    public static void AddInstallationRepositoriesEventData(
      this CustomerIntelligenceData ciData,
      ProviderInstallationRepositoriesEvent installationRepoEvent)
    {
      ArgumentUtility.CheckForNull<CustomerIntelligenceData>(ciData, nameof (ciData));
      ArgumentUtility.CheckForNull<ProviderInstallationRepositoriesEvent>(installationRepoEvent, nameof (installationRepoEvent));
      ciData.Add("Action", installationRepoEvent.Action);
      ciData.Add("AppId", installationRepoEvent.Installation.AppId);
      ciData.Add("AppName", installationRepoEvent.AppName);
      ciData.Add("EventType", "ProviderInstallationRepositoriesEvent");
      ciData.AddIfNotNull("InstallationId", (object) installationRepoEvent.Installation.Id);
      ciData.Add("ExternalOwnerName", installationRepoEvent.Installation.OrgName);
      ciData.Add("ExternalOwnerIsAUser", installationRepoEvent.Installation.IsOrgAUser);
      ciData.Add("IsAllRepoSelected", installationRepoEvent.IsAllRepoSelected);
      ciData.Add("PrivateRepoCount", (double) installationRepoEvent.RepositoriesAdded.Count<ExternalGitRepo>((Func<ExternalGitRepo, bool>) (r => r.IsPrivate)));
      ciData.Add("PublicRepoCount", (double) installationRepoEvent.RepositoriesAdded.Count<ExternalGitRepo>((Func<ExternalGitRepo, bool>) (r => !r.IsPrivate)));
      ciData.Add("PrivateRepoRemovedCount", (double) installationRepoEvent.RepositoriesRemoved.Count<ExternalGitRepo>((Func<ExternalGitRepo, bool>) (r => r.IsPrivate)));
      ciData.Add("PublicRepoRemovedCount", (double) installationRepoEvent.RepositoriesRemoved.Count<ExternalGitRepo>((Func<ExternalGitRepo, bool>) (r => !r.IsPrivate)));
    }

    public static void AddInstallationEventData(
      this CustomerIntelligenceData ciData,
      ProviderInstallationEvent installationEvent)
    {
      ArgumentUtility.CheckForNull<CustomerIntelligenceData>(ciData, nameof (ciData));
      ArgumentUtility.CheckForNull<ProviderInstallationEvent>(installationEvent, nameof (installationEvent));
      ciData.Add("Action", installationEvent.Action);
      ciData.Add("AppId", installationEvent.Installation.AppId);
      ciData.Add("AppName", installationEvent.AppName);
      ciData.Add("EventType", "ProviderInstallationEvent");
      ciData.AddIfNotNull("InstallationId", (object) installationEvent.Installation.Id);
      ciData.Add("ExternalOwnerId", installationEvent.Installation.OrgId);
      ciData.Add("ExternalOwnerName", installationEvent.Installation.OrgName);
      ciData.Add("ExternalOwnerIsAUser", installationEvent.Installation.IsOrgAUser);
      ciData.Add("IsAllRepoSelected", installationEvent.IsAllRepoSelected);
      ciData.Add("PrivateRepoCount", (double) installationEvent.Repositories.Where<ExternalGitRepo>((Func<ExternalGitRepo, bool>) (r => r.IsPrivate)).Count<ExternalGitRepo>());
      ciData.Add("PublicRepoCount", (double) installationEvent.Repositories.Where<ExternalGitRepo>((Func<ExternalGitRepo, bool>) (r => !r.IsPrivate)).Count<ExternalGitRepo>());
    }

    public static void AddMarketplacePurchaseEventData(
      this CustomerIntelligenceData ciData,
      ProviderMarketplacePurchaseEvent marketplacePurchaseEvent)
    {
      ArgumentUtility.CheckForNull<CustomerIntelligenceData>(ciData, nameof (ciData));
      ArgumentUtility.CheckForNull<ProviderMarketplacePurchaseEvent>(marketplacePurchaseEvent, nameof (marketplacePurchaseEvent));
      ciData.Add("Action", marketplacePurchaseEvent.Action);
      ciData.Add("AppName", marketplacePurchaseEvent.AppName);
      ciData.Add("EventType", "ProviderMarketplacePurchaseEvent");
      ciData.Add("ExternalOwnerId", marketplacePurchaseEvent.Installation.OrgId);
      ciData.Add("ExternalOwnerName", marketplacePurchaseEvent.Installation.OrgName);
      ciData.Add("ExternalOwnerIsAUser", marketplacePurchaseEvent.Installation.IsOrgAUser);
      ciData.Add("PlanName", marketplacePurchaseEvent.MarketplacePurchase.PlanName);
      ciData.Add("UnitCount", (double) marketplacePurchaseEvent.MarketplacePurchase.UnitCount);
      ciData.Add("UnitName", marketplacePurchaseEvent.MarketplacePurchase.UnitName);
      ciData.Add("BillingCycle", marketplacePurchaseEvent.MarketplacePurchase.BillingCycle);
      ciData.Add("NextBillingDate", marketplacePurchaseEvent.MarketplacePurchase.NextBillingDate);
      ciData.Add("EffectiveDate", marketplacePurchaseEvent.EffectiveDate);
    }

    public static void AddRecommendationAnalysis(
      this CustomerIntelligenceData ciData,
      TreeAnalysis treeAnalysis,
      string repositoryType,
      long elapsedTimeMs,
      IEnumerable<string> missedConfigFiles)
    {
      ArgumentUtility.CheckForNull<TreeAnalysis>(treeAnalysis, nameof (treeAnalysis));
      ciData.Add("RepositoryType", repositoryType);
      ciData.Add("TreeDirectoryCount", (double) treeAnalysis.DirectoryCount);
      ciData.Add("TreeFileCount", (double) treeAnalysis.FileCount);
      ciData.Add("TreeTotalNodeCount", (double) (treeAnalysis.FileCount + treeAnalysis.DirectoryCount));
      ciData.Add("TimeToRetrieveDataMs", (double) treeAnalysis.TimeToRetrieveDataMs);
      ciData.Add("TimeToTraverseTreeMs", (double) treeAnalysis.TimeToTraverseTreeMs);
      ciData.Add("TimeToAnalyzeResultsMs", (double) elapsedTimeMs);
      ciData.Add("MissedConfigFiles", (object) missedConfigFiles);
    }

    public static void PublishCI(
      this CustomerIntelligenceData ciData,
      IVssRequestContext requestContext,
      string ciFeature,
      string userId)
    {
      ArgumentUtility.CheckForNull<CustomerIntelligenceData>(ciData, nameof (ciData));
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<string>(ciFeature, nameof (ciFeature));
      ArgumentUtility.CheckForNull<string>(userId, nameof (userId));
      Guid result;
      if (!Guid.TryParse(userId, out result))
        return;
      CIDataExtensions.PublishCI(requestContext, ciData, ciFeature, userId, result);
    }

    public static void PublishCI(
      this CustomerIntelligenceData ciData,
      IVssRequestContext requestContext,
      string ciFeature,
      Guid identityId)
    {
      ArgumentUtility.CheckForNull<CustomerIntelligenceData>(ciData, nameof (ciData));
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<string>(ciFeature, nameof (ciFeature));
      CIDataExtensions.PublishCI(requestContext, ciData, ciFeature, identityId.ToString(), identityId);
    }

    public static void PublishCI(
      this CustomerIntelligenceData ciData,
      IVssRequestContext requestContext,
      string ciFeature)
    {
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Pipelines", ciFeature, ciData);
    }

    private static void PublishCI(
      IVssRequestContext requestContext,
      CustomerIntelligenceData ciData,
      string ciFeature,
      string userId,
      Guid identityId)
    {
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, requestContext.ServiceHost.InstanceId, userId, identityId, IdentityCuidHelper.GetCuidByVsid(requestContext, identityId), DateTime.UtcNow, "Pipelines", ciFeature, ciData);
    }

    private static void AddPropertyIfFound(
      this CustomerIntelligenceData ciData,
      IDictionary<string, string> properties,
      string propertyName,
      string ciKey)
    {
      string str;
      if (!properties.TryGetValue(propertyName, out str))
        return;
      ciData.Add(ciKey, str);
    }

    private static void AddIfNotNull(
      this CustomerIntelligenceData ciData,
      string key,
      object value)
    {
      if (value == null)
        return;
      ciData.Add(key, value);
    }

    private static string GetRepositoryProperty(
      SourceRepository sourceRepository,
      string propertyName,
      string altPropertyName = null,
      string defaultValue = null)
    {
      if (sourceRepository != null && sourceRepository.Properties != null)
      {
        string repositoryProperty1;
        if (sourceRepository.Properties.TryGetValue(propertyName, out repositoryProperty1))
          return repositoryProperty1;
        string repositoryProperty2;
        if (altPropertyName != null && sourceRepository.Properties.TryGetValue(altPropertyName, out repositoryProperty2))
          return repositoryProperty2;
      }
      return defaultValue ?? string.Empty;
    }
  }
}

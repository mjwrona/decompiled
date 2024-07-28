// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.ProjectFeatureProvisioningService
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  [Obsolete("This service is obselete and not supported other than in old servicing step")]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class ProjectFeatureProvisioningService : IVssFrameworkService, IProjectFeatureProvisioning
  {
    private List<IProjectFeature> m_features;

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(1004017, "ProjectFeatureProvisioning", TfsTraceLayers.Framework, "ServiceStart");
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new InvalidOperationException(FrameworkResources.UnexpectedHostType((object) systemRequestContext.ServiceHost.HostType.ToString()));
      this.m_features = new List<IProjectFeature>();
      this.m_features.Add((IProjectFeature) new CodeReviewFeature());
      this.m_features.Add((IProjectFeature) new MyWorkFeature());
      this.m_features.Add((IProjectFeature) new FeedbackFeature());
      this.m_features.Add((IProjectFeature) new PlanningToolsFeature());
      this.m_features.Add((IProjectFeature) new StoryboardIntegrationFeature());
      this.m_features.Add((IProjectFeature) new PortfolioBacklogsFeature());
      this.m_features.Add((IProjectFeature) new SharedParametersFeature());
      this.m_features.Add((IProjectFeature) new TcmFeature());
      this.m_features.Add((IProjectFeature) new CoreFeature());
      this.m_features.Add((IProjectFeature) new BugBehaviorFeature());
      systemRequestContext.TraceLeave(1004018, "ProjectFeatureProvisioning", TfsTraceLayers.Framework, "ServiceStart");
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public bool HasPermissions(IVssRequestContext requestContext, string projectUri)
    {
      bool flag = false;
      try
      {
        this.CheckPermission(requestContext, projectUri);
        flag = true;
      }
      catch
      {
      }
      requestContext.Trace(1004019, TraceLevel.Verbose, "ProjectFeatureProvisioning", TfsTraceLayers.Framework, "HasPermissions return {0}", (object) flag);
      return flag;
    }

    public IEnumerable<ProjectFeatureStatus> GetFeatures(
      IVssRequestContext requestContext,
      string projectUri)
    {
      return this.GetFeaturesStates(requestContext, projectUri).Select<Tuple<IProjectFeature, ProjectFeatureStatus>, ProjectFeatureStatus>((Func<Tuple<IProjectFeature, ProjectFeatureStatus>, ProjectFeatureStatus>) (s => s.Item2));
    }

    public ProjectFeatureStatus GetFeatureState(
      IVssRequestContext requestContext,
      string projectUri,
      string featureName)
    {
      IProjectMetadata projectMetaData = ProjectProvisioningContext.GetProjectMetaData(requestContext, projectUri);
      ProjectFeatureStatus featureState = (ProjectFeatureStatus) null;
      IProjectFeature projectFeature = this.m_features.Where<IProjectFeature>((Func<IProjectFeature, bool>) (f => f.Name.Equals(featureName))).FirstOrDefault<IProjectFeature>();
      if (projectFeature != null)
        featureState = new ProjectFeatureStatus(projectFeature.Name, projectFeature.GetState(projectMetaData), projectFeature.IsHidden);
      return featureState;
    }

    public IEnumerable<IProjectFeatureProvisioningDetails> ValidateProcessTemplates(
      IVssRequestContext requestContext,
      string projectUri)
    {
      return this.ValidateProcessTemplates(requestContext, projectUri, (IEnumerable<IProjectFeature>) this.m_features);
    }

    public IEnumerable<IProjectFeatureProvisioningDetails> ValidateProcessTemplates(
      IVssRequestContext requestContext,
      string projectUri,
      IEnumerable<IProjectFeature> features)
    {
      requestContext.TraceEnter(1004020, "ProjectFeatureProvisioning", TfsTraceLayers.Framework, nameof (ValidateProcessTemplates));
      try
      {
        List<IProjectFeatureProvisioningDetails> enablementPackages = new List<IProjectFeatureProvisioningDetails>();
        List<IProjectFeatureProvisioningDetails> invalidPackages = new List<IProjectFeatureProvisioningDetails>();
        List<IProjectFeature> list1 = this.GetNotConfiguredFeatures(requestContext, projectUri).ToList<IProjectFeature>();
        List<IProjectFeature> projectFeatureList = new List<IProjectFeature>();
        List<IProjectFeature> list2 = features.ToList<IProjectFeature>();
        foreach (IProjectFeature projectFeature in list1)
        {
          IProjectFeature feature = projectFeature;
          if (list2.Exists((Predicate<IProjectFeature>) (f => f.Name.Equals(feature.Name, StringComparison.OrdinalIgnoreCase))))
            projectFeatureList.Add(feature);
        }
        if (!projectFeatureList.Any<IProjectFeature>())
          return (IEnumerable<IProjectFeatureProvisioningDetails>) enablementPackages;
        requestContext.ResetMetadataDbStamps();
        requestContext.GetService<WorkItemTrackingFieldService>().InvalidateCache(requestContext);
        requestContext.GetService<LegacyWorkItemTypeDictionary>().InvalidateCache(requestContext);
        foreach (ProcessDescriptor processDescriptor in (IEnumerable<ProcessDescriptor>) requestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptors(requestContext))
        {
          ProjectProvisioningContext provisioningContext = new ProjectProvisioningContext(requestContext, projectUri, processDescriptor);
          provisioningContext.Validate((IEnumerable<IProjectFeature>) projectFeatureList);
          if (provisioningContext.HasCriticalError)
          {
            invalidPackages.Add((IProjectFeatureProvisioningDetails) provisioningContext);
            requestContext.Trace(1004022, TraceLevel.Info, "ProjectFeatureProvisioning", TfsTraceLayers.Framework, "ProcessTemplates {0} is not applicate, Reason {1}: ", (object) processDescriptor.Name, (object) provisioningContext.Issues.First<ProjectProvisioningIssue>((Func<ProjectProvisioningIssue, bool>) (i => i.Level == IssueLevel.Critical)));
          }
          else
            enablementPackages.Add((IProjectFeatureProvisioningDetails) provisioningContext);
        }
        if (enablementPackages.Count == 0)
          this.LogCriticalErrorsForProcessTemplates((IEnumerable<IProjectFeatureProvisioningDetails>) invalidPackages, projectUri);
        IProjectFeatureProvisioningDetails eligibleTemplate = ProjectFeatureProvisioningService.GetMostEligibleTemplate(requestContext, (IEnumerable<IProjectFeatureProvisioningDetails>) enablementPackages);
        if (eligibleTemplate != null)
          ProjectFeatureProvisioningService.MarkAsRecommended(eligibleTemplate);
        return (IEnumerable<IProjectFeatureProvisioningDetails>) enablementPackages;
      }
      finally
      {
        requestContext.TraceLeave(1004021, "ProjectFeatureProvisioning", TfsTraceLayers.Framework, nameof (ValidateProcessTemplates));
      }
    }

    public void ProvisionFeatures(
      IVssRequestContext requestContext,
      string projectUri,
      Guid processTemplateId)
    {
      requestContext.TraceEnter(1004023, "ProjectFeatureProvisioning", TfsTraceLayers.Framework, nameof (ProvisionFeatures));
      requestContext.Items["IsFeatureEnablement"] = (object) true;
      this.CheckPermission(requestContext, projectUri);
      IEnumerable<IProjectFeature> configuredFeatures = this.GetNotConfiguredFeatures(requestContext, projectUri);
      if (!configuredFeatures.Any<IProjectFeature>())
      {
        requestContext.Trace(1004025, TraceLevel.Error, "ProjectFeatureProvisioning", TfsTraceLayers.Framework, "Trying to run enablement but all features are already enabled!");
        TeamFoundationEventLog.Default.Log("Trying to run enablement but all features are already enabled!", 0, EventLogEntryType.Error);
      }
      else
      {
        ProcessDescriptor processDescriptor = requestContext.GetService<ITeamFoundationProcessService>().GetSpecificProcessDescriptor(requestContext, processTemplateId);
        new ProjectProvisioningContext(requestContext, projectUri, processDescriptor).Enable(configuredFeatures);
        requestContext.TraceLeave(1004024, "ProjectFeatureProvisioning", TfsTraceLayers.Framework, nameof (ProvisionFeatures));
      }
    }

    internal static IProjectFeatureProvisioningDetails GetMostEligibleTemplate(
      IVssRequestContext requestContext,
      IEnumerable<IProjectFeatureProvisioningDetails> enablementPackages)
    {
      IEnumerable<IProjectFeatureProvisioningDetails> source = enablementPackages.Where<IProjectFeatureProvisioningDetails>((Func<IProjectFeatureProvisioningDetails, bool>) (pkg => pkg.IsValid));
      if (source.Count<IProjectFeatureProvisioningDetails>() == 1)
        return source.First<IProjectFeatureProvisioningDetails>();
      ProcessVersion processVersion = (ProcessVersion) null;
      IProjectFeatureProvisioningDetails eligibleTemplate = (IProjectFeatureProvisioningDetails) null;
      foreach (IProjectFeatureProvisioningDetails provisioningDetails in source)
      {
        if (processVersion == null || provisioningDetails.ProcessTemplateDescriptor.IsSystem)
        {
          processVersion = provisioningDetails.ProcessTemplateDescriptor.Version;
          eligibleTemplate = provisioningDetails;
        }
      }
      return eligibleTemplate;
    }

    private static void MarkAsRecommended(IProjectFeatureProvisioningDetails package) => (package as ProjectProvisioningContext).IsRecommended = true;

    private void LogCriticalErrorsForProcessTemplates(
      IEnumerable<IProjectFeatureProvisioningDetails> invalidPackages,
      string projectUri)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "No valid process template found for project {0}", (object) projectUri));
      foreach (IProjectFeatureProvisioningDetails invalidPackage in invalidPackages)
      {
        stringBuilder.AppendLine("[Process Template Errors]");
        stringBuilder.AppendLine(invalidPackage.ProcessTemplateDescriptor.Name);
        foreach (ProjectProvisioningIssue issue in invalidPackage.Issues)
          stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[{0}] {1}", (object) issue.Level.ToString(), (object) issue.Message.ToString()));
      }
      TeamFoundationEventLog.Default.Log(stringBuilder.ToString(), 0, EventLogEntryType.Warning);
    }

    private IEnumerable<IProjectFeature> GetNotConfiguredFeatures(
      IVssRequestContext requestContext,
      string projectUri)
    {
      return this.GetFeaturesStates(requestContext, projectUri).Where<Tuple<IProjectFeature, ProjectFeatureStatus>>((Func<Tuple<IProjectFeature, ProjectFeatureStatus>, bool>) (s => s.Item2.State == ProjectFeatureState.NotConfigured)).Select<Tuple<IProjectFeature, ProjectFeatureStatus>, IProjectFeature>((Func<Tuple<IProjectFeature, ProjectFeatureStatus>, IProjectFeature>) (s => s.Item1));
    }

    private IEnumerable<Tuple<IProjectFeature, ProjectFeatureStatus>> GetFeaturesStates(
      IVssRequestContext requestContext,
      string projectUri)
    {
      List<Tuple<IProjectFeature, ProjectFeatureStatus>> featuresStates = new List<Tuple<IProjectFeature, ProjectFeatureStatus>>();
      IProjectMetadata projectMetaData = ProjectProvisioningContext.GetProjectMetaData(requestContext, projectUri);
      foreach (IProjectFeature feature in this.m_features)
      {
        ProjectFeatureState state = feature.GetState(projectMetaData);
        featuresStates.Add(new Tuple<IProjectFeature, ProjectFeatureStatus>(feature, new ProjectFeatureStatus(feature.Name, state, feature.IsHidden)));
      }
      return (IEnumerable<Tuple<IProjectFeature, ProjectFeatureStatus>>) featuresStates;
    }

    private void CheckPermission(IVssRequestContext requestContext, string projectUri)
    {
      string toolSpecificId = LinkingUtilities.DecodeUri(projectUri).ToolSpecificId;
      requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(requestContext, WitProvisionSecurity.NamespaceId).CheckPermission(requestContext, "$/" + toolSpecificId, 1);
    }
  }
}

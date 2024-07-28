// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters.AutoTriggerIssueConverter
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters
{
  public static class AutoTriggerIssueConverter
  {
    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.AutoTriggerIssue ConvertAutoTriggerIssueModelToContract(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.AutoTriggerIssue autoTriggerIssue,
      IVssRequestContext context)
    {
      if (autoTriggerIssue == null)
        throw new ArgumentNullException(nameof (autoTriggerIssue));
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      if (autoTriggerIssue.ReleaseTriggerType != Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseTriggerType.ArtifactSource && autoTriggerIssue.ReleaseTriggerType != Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseTriggerType.SourceRepo && autoTriggerIssue.ReleaseTriggerType != Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseTriggerType.ContainerImage)
        return (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.AutoTriggerIssue) null;
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ContinuousDeploymentTriggerIssue contract = (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ContinuousDeploymentTriggerIssue) null;
      if (autoTriggerIssue is Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ContinuousDeploymentTriggerIssue deploymentTriggerIssue1)
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ContinuousDeploymentTriggerIssue deploymentTriggerIssue = new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ContinuousDeploymentTriggerIssue();
        deploymentTriggerIssue.ArtifactType = deploymentTriggerIssue1.ArtifactType;
        deploymentTriggerIssue.ArtifactVersionId = deploymentTriggerIssue1.ArtifactVersionId;
        deploymentTriggerIssue.SourceId = deploymentTriggerIssue1.SourceId;
        deploymentTriggerIssue.IssueSource = AutoTriggerIssueConverter.GetIssueSource(deploymentTriggerIssue1.IssueSource);
        deploymentTriggerIssue.Issue = new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Issue()
        {
          IssueType = AutoTriggerIssueConverter.GetIssueType(deploymentTriggerIssue1.Issue.IssueType),
          Message = deploymentTriggerIssue1.Issue.Message
        };
        deploymentTriggerIssue.ReleaseDefinitionReference = new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDefinitionShallowReference()
        {
          Name = deploymentTriggerIssue1.ReleaseDefinitionName,
          Id = deploymentTriggerIssue1.ReleaseDefinitionId,
          Url = RestUrlHelper.GetRestUrlForReleaseDefinition(context, deploymentTriggerIssue1.ProjectId, deploymentTriggerIssue1.ReleaseDefinitionId)
        };
        deploymentTriggerIssue.Project = new ProjectReference()
        {
          Id = deploymentTriggerIssue1.ProjectId
        };
        deploymentTriggerIssue.ReleaseTriggerType = (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseTriggerType) autoTriggerIssue.ReleaseTriggerType;
        contract = deploymentTriggerIssue;
      }
      return (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.AutoTriggerIssue) contract;
    }

    private static string GetIssueType(IssueType issueType)
    {
      if (issueType == IssueType.Error)
        return Resources.ReleaseIssuesErrorText;
      return issueType == IssueType.Warning ? Resources.ReleaseIssuesWarningText : string.Empty;
    }

    private static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.IssueSource GetIssueSource(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.IssueSource issueSource)
    {
      if (issueSource == Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.IssueSource.User)
        return Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.IssueSource.User;
      return issueSource == Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.IssueSource.System ? Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.IssueSource.System : Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.IssueSource.None;
    }
  }
}

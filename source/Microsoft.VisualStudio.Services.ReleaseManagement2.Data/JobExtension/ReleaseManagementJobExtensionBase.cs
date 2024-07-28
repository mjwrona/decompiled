// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.JobExtension.ReleaseManagementJobExtensionBase
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.JobExtension
{
  public abstract class ReleaseManagementJobExtensionBase : ITeamFoundationJobExtension
  {
    public const string PipelinesFeatureId = "ms.vss-build.pipelines";

    protected abstract bool DeleteJobDefinitionAfterSuccessfulRun { get; }

    [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Replicating similar to Run method which framework provides")]
    protected abstract TeamFoundationJobExecutionResult RunJob(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime,
      out string resultMessage);

    [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "This is reviewed")]
    protected abstract bool IsProjectScoped(
      TeamFoundationJobDefinition jobDefinition,
      out Guid projectId);

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to log and fail for all exceptions")]
    public TeamFoundationJobExecutionResult Run(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime,
      out string resultMessage)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (jobDefinition == null)
        throw new ArgumentNullException(nameof (jobDefinition));
      TeamFoundationJobExecutionResult jobExecutionResult = TeamFoundationJobExecutionResult.Succeeded;
      if (this.CanRunJob(requestContext, jobDefinition, out resultMessage))
        jobExecutionResult = this.RunJob(requestContext, jobDefinition, queueTime, out resultMessage);
      if (requestContext.IsFeatureEnabled("VisualStudio.ReleaseManagement.DeleteJobDefinitionAfterSuccessfulRun") && this.DeleteJobDefinitionAfterSuccessfulRun && jobExecutionResult == TeamFoundationJobExecutionResult.Succeeded)
        requestContext.GetService<TeamFoundationJobService>().DeleteJobDefinitions(requestContext, (IEnumerable<Guid>) new Guid[1]
        {
          jobDefinition.JobId
        });
      return jobExecutionResult;
    }

    private bool CanRunJob(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      out string resultMessage)
    {
      resultMessage = string.Empty;
      Guid projectId;
      if (this.IsProjectScoped(jobDefinition, out projectId))
      {
        try
        {
          requestContext.GetService<IDataspaceService>().QueryDataspace(requestContext, "ReleaseManagement", projectId, true);
        }
        catch (DataspaceNotFoundException ex)
        {
          resultMessage = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Ignoring the job run as the project {0} is not associated with ReleaseManagement. JobDefinition Details: {1}", (object) projectId, jobDefinition.Data == null || jobDefinition.Data.InnerXml == null ? (object) string.Empty : (object) jobDefinition.Data.InnerXml);
          return false;
        }
      }
      return true;
    }
  }
}

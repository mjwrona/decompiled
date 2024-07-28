// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services.ReleaseAttachmentsService
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Processors;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services
{
  public class ReleaseAttachmentsService : ReleaseManagement2ServiceBase
  {
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "environmentId and attemptId will be used in future")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    public IEnumerable<TaskAttachment> GetAttachments(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseId,
      int environmentId,
      int attemptId,
      Guid planId,
      string type)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      DeployPhaseTypes deployPhaseType = ReleaseAttachmentsService.GetDeployPhaseType(requestContext, projectId, releaseId, environmentId, planId);
      return new ReleaseAttachmentsProcessor().GetAttachments(requestContext, projectId, planId, deployPhaseType, type);
    }

    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "environmentId and attemptId will be used in future")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    public Stream GetAttachment(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseId,
      int environmentId,
      int attemptId,
      Guid planId,
      Guid timelineId,
      Guid recordId,
      string type,
      string name)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      DeployPhaseTypes deployPhaseType = ReleaseAttachmentsService.GetDeployPhaseType(requestContext, projectId, releaseId, environmentId, planId);
      return new ReleaseAttachmentsProcessor().GetAttachment(requestContext, projectId, planId, deployPhaseType, timelineId, recordId, type, name);
    }

    private static DeployPhaseTypes GetDeployPhaseType(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      Guid planId)
    {
      Func<ReleaseSqlComponent, ReleaseDeployPhase> action = (Func<ReleaseSqlComponent, ReleaseDeployPhase>) (component => component.GetReleaseDeployPhase(projectId, releaseId, releaseEnvironmentId, 0, new Guid?(planId)));
      return (requestContext.ExecuteWithinUsingWithComponent<ReleaseSqlComponent, ReleaseDeployPhase>(action) ?? throw new ReleaseManagementObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.DeployPhaseNotFound, (object) projectId, (object) releaseId, (object) releaseEnvironmentId, (object) planId))).PhaseType;
    }
  }
}

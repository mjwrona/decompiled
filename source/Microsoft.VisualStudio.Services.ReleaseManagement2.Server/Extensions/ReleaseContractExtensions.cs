// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions.ReleaseContractExtensions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions
{
  public static class ReleaseContractExtensions
  {
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These are optional parameters")]
    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release FillReleaseTasks(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release webApiRelease,
      IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release serverModelRelease,
      bool includeTasksOnlyForInProgressEnvironments = false,
      int numberOfGateRecords = 5)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return ReleaseContractExtensions.FillReleaseTasksImplementation(webApiRelease, requestContext, projectId, serverModelRelease, ReleaseContractExtensions.\u003C\u003EO.\u003C0\u003E__GetTimelineRecords ?? (ReleaseContractExtensions.\u003C\u003EO.\u003C0\u003E__GetTimelineRecords = new Func<IVssRequestContext, Guid, Guid, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes, IEnumerable<TimelineRecord>>(ReleaseEnvironmentContractExtensions.GetTimelineRecords)), numberOfGateRecords, includeTasksOnlyForInProgressEnvironments);
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for complexity of type")]
    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release FillReleaseTasksImplementation(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release webApiRelease,
      IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release serverModelRelease,
      Func<IVssRequestContext, Guid, Guid, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes, IEnumerable<TimelineRecord>> getTimelineRecords,
      int numberOfGateRecords,
      bool includeTasksOnlyForInProgressEnvironments)
    {
      if (serverModelRelease == null)
        throw new ArgumentNullException(nameof (serverModelRelease));
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (webApiRelease == null)
        throw new ArgumentNullException(nameof (webApiRelease));
      if (getTimelineRecords == null)
        throw new ArgumentNullException(nameof (getTimelineRecords));
      if (!ReleaseContractExtensions.IsAnyDeployOrGateStepPresentInRelease(serverModelRelease))
        return webApiRelease;
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment environment in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment>) serverModelRelease.Environments)
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment serverModelEnvironment = environment;
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment webApiEnvironment = webApiRelease.Environments.FirstOrDefault<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment, bool>) (e => e.Id == serverModelEnvironment.Id));
        if (webApiEnvironment != null && (!includeTasksOnlyForInProgressEnvironments || webApiEnvironment.Status == EnvironmentStatus.InProgress))
          webApiEnvironment.FillEnvironmentTasks(requestContext, projectId, serverModelEnvironment, getTimelineRecords, numberOfGateRecords);
      }
      return webApiRelease;
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment GetReleaseEnvironmentByDefinitionEnvironmentId(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release webApiRelease,
      int environmentId)
    {
      return webApiRelease == null ? (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment) null : webApiRelease.Environments.FirstOrDefault<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment, bool>) (e => e.DefinitionEnvironmentId == environmentId));
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment GetReleaseEnvironmentById(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release serverRelease,
      int environmentId)
    {
      return serverRelease == null ? (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment) null : serverRelease.Environments.FirstOrDefault<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, bool>) (e => e.Id == environmentId));
    }

    public static IEnumerable<ReleaseTaskAttachment> ToReleaseTaskAttachments(
      this IEnumerable<TaskAttachment> taskAttachments,
      IVssRequestContext context)
    {
      if (taskAttachments == null)
        return (IEnumerable<ReleaseTaskAttachment>) null;
      if (!taskAttachments.Any<TaskAttachment>())
        return (IEnumerable<ReleaseTaskAttachment>) new List<ReleaseTaskAttachment>();
      Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities.IdentityHelper identityHelper = taskAttachments.PopulateIdentities<TaskAttachment>(context, (Func<TaskAttachment, IEnumerable<string>>) (attachment =>
      {
        List<string> releaseTaskAttachments = new List<string>();
        if (attachment.LastChangedBy != Guid.Empty)
          releaseTaskAttachments.Add(attachment.LastChangedBy.ToString());
        return (IEnumerable<string>) releaseTaskAttachments;
      }));
      List<ReleaseTaskAttachment> releaseTaskAttachments1 = new List<ReleaseTaskAttachment>();
      foreach (TaskAttachment taskAttachment in taskAttachments)
      {
        ReleaseTaskAttachment releaseTaskAttachment = new ReleaseTaskAttachment()
        {
          Type = taskAttachment.Type,
          CreatedOn = taskAttachment.CreatedOn,
          ModifiedBy = identityHelper.GetIdentity(new IdentityRef()
          {
            Id = taskAttachment.LastChangedBy.ToString()
          }),
          ModifiedOn = taskAttachment.LastChangedOn,
          Name = taskAttachment.Name,
          RecordId = taskAttachment.RecordId,
          TimelineId = taskAttachment.TimelineId,
          Links = taskAttachment.Links
        };
        releaseTaskAttachments1.Add(releaseTaskAttachment);
      }
      return (IEnumerable<ReleaseTaskAttachment>) releaseTaskAttachments1;
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is required")]
    public static Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities.IdentityHelper PopulateIdentities<T>(
      this IEnumerable<T> items,
      IVssRequestContext context,
      Func<T, IEnumerable<string>> getIdentityIds)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      if (items == null)
        throw new ArgumentNullException(nameof (items));
      if (getIdentityIds == null)
        throw new ArgumentNullException(nameof (getIdentityIds));
      using (ReleaseManagementTimer.Create(context, "Service", "ReleaseContractExtensions.PopulateIdentities", 1961101))
      {
        HashSet<string> source = new HashSet<string>();
        foreach (T obj in items)
          source.UnionWith(getIdentityIds(obj));
        return Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities.IdentityHelper.GetIdentityHelper(context, (ICollection<string>) source.ToList<string>(), false);
      }
    }

    private static bool IsAnyDeployOrGateStepPresentInRelease(Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release serverModelRelease) => serverModelRelease.Environments.Any<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, bool>) (env => env.GetDeployOrGateSteps().Any<ReleaseEnvironmentStep>()));
  }
}

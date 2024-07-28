// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.SecureFileTraceabilityRcExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  public static class SecureFileTraceabilityRcExtensions
  {
    private const string c_downloadSecureFileTaskName = "DownloadSecureFile";

    public static void TraceSecureFileEvent(
      this IVssRequestContext requestContext,
      TraceLevel level,
      Guid projectId,
      SecureFile secureFile,
      string tracePayload)
    {
      if (!requestContext.IsFeatureEnabled("DistributedTask.SecureFileTraceability"))
        return;
      requestContext.TraceAlways(10016162, level, "DistributedTask", "SecureFileTrace", new
      {
        Note = "Secure File trace.",
        TracePayload = tracePayload,
        SecureFileId = secureFile.Id,
        SecureFileName = secureFile.Name,
        SecureFileCreatedOn = (secureFile.CreatedOn != new DateTime() ? new DateTime?(secureFile.CreatedOn) : new DateTime?()),
        ProjectId = projectId
      }.Serialize());
    }

    public static void TraceSecureFileEvent(
      this IVssRequestContext requestContext,
      TraceLevel level,
      Guid projectId,
      Guid secureFileId,
      string tracePayload)
    {
      IVssRequestContext requestContext1 = requestContext;
      int level1 = (int) level;
      Guid projectId1 = projectId;
      SecureFile secureFile = new SecureFile();
      secureFile.Id = secureFileId;
      string tracePayload1 = tracePayload;
      requestContext1.TraceSecureFileEvent((TraceLevel) level1, projectId1, secureFile, tracePayload1);
    }

    public static void TraceSecureFileResources(
      this IVssRequestContext requestContext,
      TaskOrchestrationPlan plan)
    {
      if (!requestContext.IsFeatureEnabled("DistributedTask.SecureFileTraceability"))
        return;
      try
      {
        if (!(plan.ProcessEnvironment is PipelineEnvironment processEnvironment) || processEnvironment == null)
          return;
        int? count = processEnvironment.Resources?.Files.Count;
        int num = 0;
        if (!(count.GetValueOrDefault() > num & count.HasValue))
          return;
        using (requestContext.CreateOrchestrationIdScope(plan.GetOrchestrationId()))
          requestContext.TraceAlways(10016163, TraceLevel.Verbose, "DistributedTask", "TaskHub", "The PipelineEnvironment has Secure File resource(s): " + processEnvironment.Resources.Files.Serialize<ISet<SecureFileReference>>());
      }
      catch (Exception ex)
      {
        requestContext.TraceAlways(10016163, TraceLevel.Verbose, "DistributedTask", "TaskHub", string.Format("Encountered {0} while logging info about Secure Files", (object) ex.GetType()));
      }
    }

    public static void TraceDownloadSecureFileTasks(
      this IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Job job,
      JobResources jobResources)
    {
      if (!requestContext.IsFeatureEnabled("DistributedTask.SecureFileTraceability"))
        return;
      try
      {
        IEnumerable<TaskStep> source = job.Steps.OfType<TaskStep>().Where<TaskStep>((Func<TaskStep, bool>) (ts => ts.Reference.Name == "DownloadSecureFile"));
        if (source.Count<TaskStep>() <= 0)
          return;
        IEnumerable<Guid> values = source.Select<TaskStep, Guid>((Func<TaskStep, Guid>) (dsft => Guid.Parse(dsft.Inputs["secureFile"])));
        requestContext.TraceAlways(10016164, TraceLevel.Verbose, "DistributedTask", "TaskHub", "This JobRequest contains DownloadSecureFile task(s) with the following File Id(s): " + string.Join<Guid>(",", values));
        HashSet<Guid> hashSet = jobResources.SecureFiles.Select<SecureFile, Guid>((Func<SecureFile, Guid>) (sf => sf.Id)).ToHashSet<Guid>();
        foreach (Guid secureFileId in values)
        {
          if (!hashSet.Contains(secureFileId))
            requestContext.TraceSecureFileEvent(TraceLevel.Warning, scopeIdentifier, secureFileId, "JobRequest has a Task to download a SF with this Id but such SF does not exist in the Job Resources! The Task is going to fail. The SF has likely been deleted since the plan was created.");
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceAlways(10016164, TraceLevel.Verbose, "DistributedTask", "TaskHub", string.Format("Encountered {0} while logging info about DownloadSecureFiles Tasks", (object) ex.GetType()));
      }
    }
  }
}

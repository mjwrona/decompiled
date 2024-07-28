// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities.QueueJobUtility
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Xml;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities
{
  public static class QueueJobUtility
  {
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to continue on error")]
    public static void QueueUpdateRetainBuildJob(
      IVssRequestContext requestContext,
      UpdateRetainBuildData updateRetainBuildData)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      try
      {
        XmlNode xml = TeamFoundationSerializationUtility.SerializeToXml((object) updateRetainBuildData);
        ITeamFoundationJobService jobService = ReleaseOperationHelper.GetJobService(requestContext.Elevate());
        string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Queuing UpdateRetainBuildJob with following data : {0}", (object) xml);
        requestContext.Trace(1971051, TraceLevel.Info, "ReleaseManagementService", "Service", message);
        IVssRequestContext requestContext1 = requestContext;
        XmlNode jobData = xml;
        jobService.QueueOneTimeJob(requestContext1, "UpdateRetainBuild", "Microsoft.VisualStudio.Services.ReleaseManagement.Server.Plugins.JobExtension.UpdateRetainBuildJob", jobData, JobPriorityLevel.BelowNormal);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1971051, "ReleaseManagementService", "JobLayer", ex);
      }
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "This is logged for diagnosability")]
    public static void DeleteJobDefinition(
      IVssRequestContext requestContext,
      Guid jobId,
      int releaseId)
    {
      if (jobId.Equals(Guid.Empty))
        return;
      try
      {
        requestContext.GetService<ITeamFoundationJobService>().DeleteJobDefinitions(requestContext, (IEnumerable<Guid>) new List<Guid>()
        {
          jobId
        });
      }
      catch (Exception ex)
      {
        requestContext.Trace(1960102, TraceLevel.Error, "ReleaseManagementService", "Pipeline", "QueueJobUtility: DeleteJobDefinition: Throwed exception {0}, jobId: {1}, Release Id {2}", (object) ex, (object) jobId, (object) releaseId);
      }
    }
  }
}

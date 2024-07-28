// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.CVS.CVSJobHelper
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Microsoft.VisualStudio.Services.Gallery.Server.CVS
{
  public static class CVSJobHelper
  {
    private const string c_scanStatusJobNamespace = "Microsoft.VisualStudio.Services.Gallery.Extensions.JobAgentPlugins.CVS.ScanStatusJob";
    private const string c_submitScanJobNamespace = "Microsoft.VisualStudio.Services.Gallery.Extensions.JobAgentPlugins.CVS.SubmitScanJob";

    public static Guid ScheduleSubmitScanJob(
      IVssRequestContext requestContext,
      string extensionIds,
      string id)
    {
      Guid guid = Guid.Empty;
      if (!string.IsNullOrWhiteSpace(extensionIds))
      {
        CVSExtensionsIteratorRunnerJobData objectToSerialize = new CVSExtensionsIteratorRunnerJobData();
        objectToSerialize.ExtensionGuidsString = extensionIds;
        objectToSerialize.Id = id;
        XmlNode xml = TeamFoundationSerializationUtility.SerializeToXml((object) objectToSerialize);
        string jobName = "CVS SubmitScanJob: " + extensionIds;
        guid = requestContext.GetService<ITeamFoundationJobService>().QueueOneTimeJob(requestContext, jobName, "Microsoft.VisualStudio.Services.Gallery.Extensions.JobAgentPlugins.CVS.SubmitScanJob", xml, false);
      }
      return guid;
    }

    public static Guid ScheduleScanStatusJob(
      IVssRequestContext requestContext,
      List<ExtensionScanInfo> extensionsScanInfo,
      string JobName)
    {
      Guid guid = Guid.Empty;
      if (extensionsScanInfo != null)
      {
        XmlNode xml = TeamFoundationSerializationUtility.SerializeToXml((object) extensionsScanInfo);
        ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
        string str = string.IsNullOrWhiteSpace(JobName) ? "CVS ScanStatusJob: " + DateTime.UtcNow.ToString() : JobName;
        TimeSpan recheckTimespan = requestContext.GetService<ICVSService>().GetRecheckTimespan(requestContext);
        IVssRequestContext requestContext1 = requestContext;
        string jobName = str;
        XmlNode jobData = xml;
        TimeSpan startOffset = recheckTimespan;
        guid = service.QueueOneTimeJob(requestContext1, jobName, "Microsoft.VisualStudio.Services.Gallery.Extensions.JobAgentPlugins.CVS.ScanStatusJob", jobData, startOffset);
      }
      return guid;
    }

    public static void LogScanCompleteTelemetry(
      IVssRequestContext requestContext,
      ExtensionScanInfo extensionScanInfo,
      bool isTimeout,
      long timeTaken,
      string scanResult,
      string feature,
      bool isPassed = false)
    {
      if (extensionScanInfo == null)
        return;
      ClientTraceService service = requestContext.GetService<ClientTraceService>();
      ClientTraceData clientTraceData = new ClientTraceData();
      clientTraceData.Add("ExtensionId", (object) extensionScanInfo.ExtensionId);
      clientTraceData.Add("PublisherName", (object) extensionScanInfo.PublisherName);
      clientTraceData.Add("ExtensionName", (object) extensionScanInfo.ExtensionName);
      clientTraceData.Add("Version", (object) extensionScanInfo.Version);
      clientTraceData.Add("FileCount", (object) extensionScanInfo.FileCount);
      clientTraceData.Add("ScanId", (object) extensionScanInfo.ScanId);
      clientTraceData.Add("TimeTaken", (object) timeTaken);
      clientTraceData.Add("IsScanTimeOut", (object) isTimeout);
      if (!isTimeout)
        clientTraceData.Add("ScanStatus", isPassed ? (object) "Pass" : (object) "Fail");
      clientTraceData.Add("ErrorInfo", (object) scanResult);
      IVssRequestContext requestContext1 = requestContext;
      string feature1 = feature;
      ClientTraceData properties = clientTraceData;
      service.Publish(requestContext1, "Microsoft.VisualStudio.Services.Gallery", feature1, properties);
    }
  }
}

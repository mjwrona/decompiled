// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.ESRP.ESRPJobHelper
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using MS.Ess.EsrpClient.Contracts.Common;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Microsoft.VisualStudio.Services.Gallery.Server.ESRP
{
  public static class ESRPJobHelper
  {
    private const string c_scanStatusJobNamespace = "Microsoft.VisualStudio.Services.Gallery.Extensions.JobAgentPlugins.ESRP.ScanStatusJob";

    public static ScanStatusJobData PrepareScanStatusJobData(
      DateTime scanSubmissionTime,
      ICollection<ExtensionScanInfo> extensionsScanInfo)
    {
      List<ExtensionScanInfo> extensionScanInfoList = new List<ExtensionScanInfo>();
      if (extensionsScanInfo != null)
      {
        foreach (ExtensionScanInfo extensionScanInfo in (IEnumerable<ExtensionScanInfo>) extensionsScanInfo)
        {
          if (extensionScanInfo.OperationId != Guid.Empty)
            extensionScanInfoList.Add(extensionScanInfo);
        }
      }
      return new ScanStatusJobData()
      {
        StartTime = scanSubmissionTime,
        ExtensionsScanInfo = extensionScanInfoList
      };
    }

    public static Guid ScheduleScanStatusJob(
      IVssRequestContext requestContext,
      ScanStatusJobData scanStatusJobData,
      StatusCode currentStatus)
    {
      Guid guid = Guid.Empty;
      if (scanStatusJobData != null)
      {
        XmlNode xml = TeamFoundationSerializationUtility.SerializeToXml((object) scanStatusJobData);
        ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
        string str = "ScanStatusJob: " + scanStatusJobData.StartTime.ToString();
        TimeSpan recheckTimespan = requestContext.GetService<IESRPService>().GetRecheckTimespan(requestContext, currentStatus);
        IVssRequestContext requestContext1 = requestContext;
        string jobName = str;
        XmlNode jobData = xml;
        TimeSpan startOffset = recheckTimespan;
        guid = service.QueueOneTimeJob(requestContext1, jobName, "Microsoft.VisualStudio.Services.Gallery.Extensions.JobAgentPlugins.ESRP.ScanStatusJob", jobData, startOffset);
      }
      return guid;
    }

    public static void LogScanCompleteTelemetry(
      IVssRequestContext requestContext,
      ExtensionScanInfo extensionScanInfo,
      bool isTimeout,
      long timeTaken,
      ResponseBase extensionScanResponse,
      string feature,
      bool isRetry)
    {
      if (extensionScanInfo == null)
        return;
      CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
      CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
      intelligenceData.Add("PublisherName", extensionScanInfo.PublisherName);
      intelligenceData.Add("ExtensionName", extensionScanInfo.ExtensionName);
      intelligenceData.Add("Version", extensionScanInfo.Version);
      intelligenceData.Add("ExtensionSize", (double) extensionScanInfo.FileSize);
      intelligenceData.Add("OperationId", (object) extensionScanInfo.OperationId);
      intelligenceData.Add("TimeTaken", (double) timeTaken);
      intelligenceData.Add("IsScanTimeOut", isTimeout);
      intelligenceData.Add("IsRetry", isRetry);
      if (!isTimeout && extensionScanResponse != null)
      {
        intelligenceData.Add("ScanStatus", (object) extensionScanResponse.StatusCode);
        intelligenceData.Add("ErrorInfo", (object) extensionScanResponse.ErrorInfo);
      }
      IVssRequestContext requestContext1 = requestContext;
      string feature1 = feature;
      CustomerIntelligenceData properties = intelligenceData;
      service.Publish(requestContext1, "Microsoft.VisualStudio.Services.Gallery", feature1, properties);
    }

    public static void LogSignCompleteTelemetry(
      IVssRequestContext requestContext,
      RepositorySigningInfo signingInfo,
      bool isTimeout,
      long timeTaken,
      EsrpGetOperationStatusResponse extensionScanResponse,
      string feature)
    {
      if (signingInfo == null)
        return;
      CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
      CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
      intelligenceData.Add("PublisherName", signingInfo.PublisherName);
      intelligenceData.Add("ExtensionName", signingInfo.ExtensionName);
      intelligenceData.Add("Version", signingInfo.Version);
      intelligenceData.Add("ExtensionSize", (double) signingInfo.FileSize);
      intelligenceData.Add("OperationId", signingInfo.OperationId);
      intelligenceData.Add("TimeTaken", (double) timeTaken);
      intelligenceData.Add("IsSignTimeOut", isTimeout);
      intelligenceData.Add("IsRetry", signingInfo.IsRetry);
      intelligenceData.Add("RetryCount", (double) signingInfo.RetryCount);
      if (!isTimeout && extensionScanResponse != null)
      {
        intelligenceData.Add("SignStatus", (object) extensionScanResponse.OperationStatus);
        intelligenceData.Add("EsrpOperationId", extensionScanResponse.OperationId);
        intelligenceData.Add("ErrorInfo", extensionScanResponse.ErrorDetail);
      }
      IVssRequestContext requestContext1 = requestContext;
      string feature1 = feature;
      CustomerIntelligenceData properties = intelligenceData;
      service.Publish(requestContext1, "Microsoft.VisualStudio.Services.Gallery", feature1, properties);
    }
  }
}

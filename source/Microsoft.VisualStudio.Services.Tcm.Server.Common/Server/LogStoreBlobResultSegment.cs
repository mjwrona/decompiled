// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.LogStoreBlobResultSegment
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.Azure.Storage.Blob;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class LogStoreBlobResultSegment : ILogStoreBlobResultSegment
  {
    private IVssRequestContext _requestContext;
    private BlobResultSegment _blobResultSegment;
    private LogStoreBlobContinuationToken _logStoreBlobContinuationToken;

    public LogStoreBlobResultSegment(
      BlobResultSegment blobResultSegment,
      IVssRequestContext requestContext)
    {
      this._requestContext = requestContext;
      this._blobResultSegment = blobResultSegment;
      this._logStoreBlobContinuationToken = new LogStoreBlobContinuationToken(this._blobResultSegment?.ContinuationToken);
    }

    public List<AttachmentTestLog> GetAttachmentTestLogs(AttachmentTestLogReference testLogReference)
    {
      List<AttachmentTestLog> attachmentTestLogs = new List<AttachmentTestLog>();
      if (this._blobResultSegment == null || this._blobResultSegment.Results == null)
        return attachmentTestLogs;
      foreach (IListBlobItem result in this._blobResultSegment.Results)
      {
        if (result.GetType() == typeof (CloudBlockBlob))
        {
          AttachmentTestLog attachmentTestLog = this.GetAttachmentTestLog(testLogReference, (CloudBlockBlob) result);
          if (attachmentTestLog.AttachmentTestLogReference == null || attachmentTestLog.AttachmentTestLogReference.AttachmentId == 0)
            this._requestContext.Trace(0, TraceLevel.Error, "TestManagement", "LogStorage", "GetAttachmentTestLogs: Invalid attachment Id filePath: {0}", (object) ((CloudBlob) result).Name);
          else
            attachmentTestLogs.Add(attachmentTestLog);
        }
        else
          this._requestContext.Trace(0, TraceLevel.Error, "TestManagement", "LogStorage", "GetAttachmentTestLogs: Invalid blob item type {0}", (object) result.GetType());
      }
      return attachmentTestLogs;
    }

    public LogStoreBlobContinuationToken GetLogStoreBlobContinuationToken() => this._logStoreBlobContinuationToken;

    public List<TestLog> GetTestLogList(TestLogScope scope, int containerId)
    {
      List<TestLog> testLogList = new List<TestLog>();
      if (this._blobResultSegment == null)
        return testLogList;
      foreach (IListBlobItem result in this._blobResultSegment.Results)
      {
        if (result.GetType() == typeof (CloudBlockBlob))
        {
          TestLog testLog = this.GetTestLog((CloudBlockBlob) result, scope, containerId);
          if (string.IsNullOrEmpty(testLog.LogReference.FilePath))
            this._requestContext.Trace(1015681, TraceLevel.Info, "TestManagement", "LogStorage", "Couldn't convert the blobitem to TestLog reference, please check : " + (string.Format("RunId:{0},", (object) testLog.LogReference.RunId) + string.Format("ResultId:{0},", (object) testLog.LogReference.ResultId) + string.Format("SubResultId:{0},", (object) testLog.LogReference.SubResultId) + string.Format("BuildId:{0},", (object) testLog.LogReference.BuildId) + string.Format("ReleaseId:{0},", (object) testLog.LogReference.ReleaseId) + string.Format("ReleaseEnvId:{0},", (object) testLog.LogReference.ReleaseEnvId) + "ActualBlobName:" + ((CloudBlob) result).Name));
          else
            testLogList.Add(testLog);
        }
        else
          this._requestContext.Trace(0, TraceLevel.Error, "TestManagement", "LogStorage", "GetTestLogList: Invalid blob item type {0}", (object) result.GetType());
      }
      return testLogList;
    }

    public List<ILogStoreCloudBlockBlob> GetRawBlobsForCvs()
    {
      List<ILogStoreCloudBlockBlob> rawBlobsForCvs = new List<ILogStoreCloudBlockBlob>();
      if (this._blobResultSegment == null)
        return rawBlobsForCvs;
      foreach (IListBlobItem result in this._blobResultSegment.Results)
      {
        if (result.GetType() == typeof (CloudBlockBlob))
        {
          LogStoreCloudBlockBlob storeCloudBlockBlob = new LogStoreCloudBlockBlob((CloudBlockBlob) result);
          rawBlobsForCvs.Add((ILogStoreCloudBlockBlob) storeCloudBlockBlob);
        }
        else
          this._requestContext.Trace(0, TraceLevel.Error, "TestManagement", "LogStorage", "GetRawBlobs: Invalid blob item type {0}", (object) result.GetType());
      }
      return rawBlobsForCvs;
    }

    public List<string> GetDirectoryPrefixList()
    {
      List<string> directoryPrefixList = new List<string>();
      if (this._blobResultSegment == null)
        return directoryPrefixList;
      foreach (IListBlobItem result in this._blobResultSegment.Results)
      {
        if (result.GetType() == typeof (CloudBlobDirectory))
        {
          CloudBlobDirectory cloudBlobDirectory = (CloudBlobDirectory) result;
          directoryPrefixList.Add(cloudBlobDirectory.Prefix);
        }
      }
      return directoryPrefixList;
    }

    public List<string> GetBlobNameList()
    {
      List<string> blobNameList = new List<string>();
      if (this._blobResultSegment == null)
        return blobNameList;
      foreach (IListBlobItem result in this._blobResultSegment.Results)
      {
        if (result.GetType() == typeof (CloudBlockBlob))
        {
          CloudBlockBlob cloudBlockBlob = (CloudBlockBlob) result;
          blobNameList.Add(cloudBlockBlob.Name);
        }
      }
      return blobNameList;
    }

    private TestLog GetTestLog(CloudBlockBlob blobItem, TestLogScope scope, int containerId)
    {
      TestLog testLog = new TestLog();
      LogStorePathFormatter storePathFormatter = new LogStorePathFormatter();
      testLog.LogReference = storePathFormatter.GetTestLogReferenceFromBlobName(scope, containerId, blobItem.Name);
      if (blobItem.Properties.LastModified.HasValue)
        testLog.ModifiedOn = blobItem.Properties.LastModified.Value.UtcDateTime;
      testLog.Size = blobItem.Properties.Length;
      testLog.MetaData = blobItem.Metadata != null ? new Dictionary<string, string>(blobItem.Metadata, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) : (Dictionary<string, string>) null;
      return testLog;
    }

    private AttachmentTestLog GetAttachmentTestLog(
      AttachmentTestLogReference attachmentTestLogReference,
      CloudBlockBlob blobItem)
    {
      AttachmentTestLog attachmentTestLog1 = new AttachmentTestLog();
      LogStorePathFormatter storePathFormatter = new LogStorePathFormatter();
      string str;
      string s;
      if (blobItem.Metadata == null || !blobItem.Metadata.TryGetValue("type", out str) || !blobItem.Metadata.TryGetValue("size", out s))
        return attachmentTestLog1;
      attachmentTestLog1.AttachmentTestLogReference = storePathFormatter.GetAttachmentTestLogReferenceFromBlobName(attachmentTestLogReference.RunId, blobItem.Name);
      DateTimeOffset? lastModified = blobItem.Properties.LastModified;
      if (lastModified.HasValue)
      {
        AttachmentTestLog attachmentTestLog2 = attachmentTestLog1;
        lastModified = blobItem.Properties.LastModified;
        DateTime utcDateTime = lastModified.Value.UtcDateTime;
        attachmentTestLog2.ModifiedOn = utcDateTime;
      }
      attachmentTestLog1.Size = blobItem.Properties.Length;
      attachmentTestLog1.MetaData = new Dictionary<string, string>(blobItem.Metadata, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      attachmentTestLog1.Size = long.Parse(s);
      attachmentTestLog1.AttachmentTestLogReference.Type = (TestLogType) Enum.Parse(typeof (TestLogType), str);
      return attachmentTestLog1;
    }
  }
}

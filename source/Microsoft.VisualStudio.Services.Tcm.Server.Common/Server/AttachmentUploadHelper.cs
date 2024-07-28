// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.AttachmentUploadHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Common.Internal;
using Microsoft.TeamFoundation.TestManagement.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Web;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class AttachmentUploadHelper
  {
    private bool setTimeToFirstPage = true;

    public void WakeUpCoverAnJobIfRequired(
      TestManagementRequestContext tmContext,
      bool coverageChanged)
    {
      if (!coverageChanged)
        return;
      tmContext.TestManagementHost.SignalTfsJobService(tmContext, tmContext.JobMappings["TestManagement.Jobs.CoverageAnalyzerJob"].ToString(), JobPriorityLevel.BelowNormal);
    }

    public void WakeUpNewCoverageJobIfRequired(
      TestManagementRequestContext tmContext,
      TeamProjectReference projRef,
      int runId,
      bool coverageChanged,
      string attachmentType)
    {
      try
      {
        using (new SimpleTimer(tmContext.RequestContext, nameof (WakeUpNewCoverageJobIfRequired)))
        {
          if (tmContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment || !coverageChanged || !string.Equals("CodeCoverage", attachmentType, StringComparison.OrdinalIgnoreCase))
            return;
          TestRun testRunById = tmContext.RequestContext.GetService<ITeamFoundationTestManagementRunService>().GetTestRunById(tmContext, runId, projRef);
          if (testRunById.BuildReference.BuildId <= 0 || testRunById.ReleaseReference != null && testRunById.ReleaseReference.ReleaseId > 0)
            return;
          new CodeCoverageHelper(tmContext).QueueInvokerJobForPatchSummaryRequest(projRef.Id, testRunById.BuildReference.BuildId);
        }
      }
      catch (Exception ex)
      {
        tmContext.Logger.Error(1015145, string.Format("AttachmentUploadHelper: WakeUpNewCoverageJobIfRequired exception for RunId: {0} {1}", (object) runId, (object) ex));
      }
    }

    public void ProcessUpload(TestManagementRequestContext context, HttpRequestBase request) => this.ProcessUpload(context, request.Params.ToDictionary(), request.Files.ToHttpPostedAttachments());

    public void ProcessUpload(
      TestManagementRequestContext context,
      Dictionary<string, string> requestParams,
      List<HttpPostedTcmAttachment> files)
    {
      string projectName = this.ReadStringField(context, requestParams, "ProjectName");
      int testRunId = AttachmentUploadHelper.ReadIntField(context, requestParams, "TestRunId");
      int sessionId = AttachmentUploadHelper.ReadIntField(requestParams, "SessionId", 0);
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      if (!context.FeedbackHelper.IsFeedBackSession(context, sessionId, projectName))
        context.SecurityManager.CheckPublishTestResultsPermission(context, projectFromName.String);
      int num = AttachmentUploadHelper.ReadIntField(requestParams, "MultiFilesUpload", 0);
      context.TraceVerbose("AttachmentHandler", "AttachmentUploadHandler.ProcessUpload - {0}, {1}, {2}, {3}", (object) projectName, (object) testRunId, (object) sessionId, (object) num);
      bool coverageChanged = num == 1 ? this.ProcessMultiFilesUpload(context, requestParams, files, projectFromName.GuidId, testRunId, sessionId) : this.ProcessSingleFileUpload(context, requestParams, files, projectFromName.GuidId, testRunId, sessionId);
      this.WakeUpCoverAnJobIfRequired(context, coverageChanged);
    }

    public void ProcessUploadInLogStore(
      TestManagementRequestContext context,
      Dictionary<string, string> requestParams,
      List<HttpPostedTcmAttachment> files)
    {
      string projectName = this.ReadStringField(context, requestParams, "ProjectName");
      int testRunId = AttachmentUploadHelper.ReadIntField(context, requestParams, "TestRunId");
      int sessionId = AttachmentUploadHelper.ReadIntField(requestParams, "SessionId", 0);
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      if (!context.FeedbackHelper.IsFeedBackSession(context, sessionId, projectName))
        context.SecurityManager.CheckPublishTestResultsPermission(context, projectFromName.String);
      int num = AttachmentUploadHelper.ReadIntField(requestParams, "MultiFilesUpload", 0);
      context.TraceVerbose("AttachmentHandler", "AttachmentUploadHandler.ProcessUploadInlogStore - {0}, {1}, {2}, {3}", (object) projectName, (object) testRunId, (object) sessionId, (object) num);
      bool coverageChanged = num != 0 ? this.ProcessMultiFilesUploadInLogStore(context, requestParams, files, projectFromName.GuidId, testRunId, sessionId) : this.ProcessSingleFileUploadInLogStore(context, requestParams, files, projectFromName.GuidId, testRunId, sessionId);
      this.WakeUpCoverAnJobIfRequired(context, coverageChanged);
    }

    private bool ProcessSingleFileUploadInLogStore(
      TestManagementRequestContext context,
      Dictionary<string, string> requestParams,
      List<HttpPostedTcmAttachment> files,
      Guid projectId,
      int testRunId,
      int sessionId)
    {
      int resultId = 0;
      int attachmentId = 0;
      CompressionType compressionType = CompressionType.None;
      long uncompressedLength = 0;
      HttpRange httpRange = (HttpRange) null;
      HttpPostedTcmAttachment attachment = (HttpPostedTcmAttachment) null;
      bool flag = false;
      try
      {
        resultId = AttachmentUploadHelper.ReadIntField(context, requestParams, "TestResultId");
        httpRange = this.ReadRangeField(context, requestParams, "Range");
        attachmentId = AttachmentUploadHelper.ReadIntField(context, requestParams, "AttachmentId");
        uncompressedLength = this.ReadLongField(context, requestParams, "UncompressedLength");
        compressionType = this.ReadCompressionType(requestParams);
        this.ReadHashValue(requestParams);
        attachment = files != null && files.Count != 0 ? files[0] : throw new InvalidDataException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidAttachmentFile, (object) attachmentId));
        context.TraceVerbose("AttachmentHandler", "AttachmentUploadHandler.ProcessSingleFileUploadInLogStore - ResultId:{0} File:{1} AttachmentId:{2} Length:{3}", (object) resultId, (object) attachment.FileName, (object) attachmentId, (object) uncompressedLength);
        int num = context.RequestContext.GetService<ITeamFoundationTestManagementAttachmentsService>().UploadAttachmentToLogStore(context, attachment, projectId, testRunId, resultId, sessionId, attachmentId, uncompressedLength) ? 1 : 0;
        flag = true;
        return num != 0;
      }
      catch (Exception ex)
      {
        string str1 = httpRange != null ? httpRange.ToString() : string.Empty;
        string str2 = attachment != null ? string.Format("FileName:{0} ContentLength:{1} ContentType:{2}", (object) attachment.FileName, (object) attachment.ContentLength, (object) attachment.ContentType) : string.Empty;
        context.RequestContext.TraceAlways(1015164, TraceLevel.Error, "TestManagement", "AttachmentHandler", "AttachmentUploadHelper.ProcessSingleFileUploadInLogStore: Upload attachment to LogStore threw exception. ProjectId:{0}, RunId:{1}, ResultId:{2}, SessionId:{3}, AttachmentId:{4}, Length:{5}, CompressionType:{6}, RangeAsString:{7}, FileInfo:{8}, Exception:{9}", (object) projectId, (object) testRunId, (object) resultId, (object) sessionId, (object) attachmentId, (object) uncompressedLength, (object) compressionType.ToString(), (object) str1, (object) str2, (object) ex.Message);
        flag = false;
        throw;
      }
      finally
      {
        CustomerIntelligenceData cid = new CustomerIntelligenceData((IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            "ProjectId",
            (object) projectId.ToString()
          },
          {
            "TestRunId",
            (object) testRunId.ToString()
          },
          {
            "TestResultId",
            (object) resultId.ToString()
          },
          {
            "AttachmentId",
            (object) attachmentId.ToString()
          },
          {
            "UncompressedLength",
            (object) uncompressedLength.ToString()
          },
          {
            "FileName",
            attachment == null ? (object) string.Empty : (object) attachment.FileName
          },
          {
            "IsSuccessful",
            (object) flag.ToString()
          }
        });
        TelemetryLogger.Instance.PublishData(context.RequestContext, nameof (ProcessSingleFileUploadInLogStore), cid);
      }
    }

    private bool ProcessMultiFilesUploadInLogStore(
      TestManagementRequestContext context,
      Dictionary<string, string> requestParams,
      List<HttpPostedTcmAttachment> files,
      Guid projectId,
      int testRunId,
      int sessionId)
    {
      int[] numArray1 = this.ReadIntArrayField(context, requestParams, "TestResultIds");
      int[] numArray2 = this.ReadIntArrayField(context, requestParams, "AttachmentIds");
      if (files == null || files.Count == 0)
        throw new InvalidDataException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidAttachmentFile, (object) numArray2[0]));
      int[] numArray3 = this.ReadIntArrayField(context, requestParams, "UncompressedLengths");
      int[] numArray4 = this.ReadIntArrayField(context, requestParams, "StreamLengths");
      string[] strArray = this.ReadStringArrayField(requestParams, "CompressionTypes");
      context.TraceAndDebugAssert("AttachmentHandler", files.Count == numArray1.Length, "invalid value for testResultIds");
      context.TraceAndDebugAssert("AttachmentHandler", files.Count == numArray2.Length, "invalid value for attachmentIds");
      context.TraceAndDebugAssert("AttachmentHandler", files.Count == numArray3.Length, "invalid value for uncompressedLengths");
      context.TraceAndDebugAssert("AttachmentHandler", files.Count == numArray4.Length, "invalid value for compressedLengths");
      context.TraceAndDebugAssert("AttachmentHandler", files.Count == strArray.Length, "invalid value for compressionTypes");
      ITeamFoundationTestManagementAttachmentsService service = context.RequestContext.GetService<ITeamFoundationTestManagementAttachmentsService>();
      bool flag1 = false;
      for (int index = 0; index < numArray1.Length; ++index)
      {
        context.TraceAndDebugAssert("AttachmentHandler", numArray4[index] > 0 || numArray3[index] == 0, "invalid value for streamLengths");
        bool flag2 = false;
        try
        {
          flag1 |= service.UploadAttachmentToLogStore(context, files[index], projectId, testRunId, numArray1[index], sessionId, numArray2[index], (long) numArray3[index]);
          flag2 = true;
        }
        catch (Exception ex)
        {
          CompressionType compressionType = this.ToCompressionType(strArray[index]);
          context.TraceError("AttachmentHandler", "AttachmentUploadHandler.ProcessMultiFilesUploadInLogStore - RunId:{0} ResultId:{1} SessionId:{2} AttachmentId:{3} Length:{4} CompressionType:{5}, Exception:{6}", (object) testRunId, (object) numArray1[index], (object) sessionId, (object) numArray2[index], (object) numArray3[index], (object) compressionType.ToString(), (object) ex.Message);
          flag2 = false;
          throw;
        }
        finally
        {
          CustomerIntelligenceData cid = new CustomerIntelligenceData((IDictionary<string, object>) new Dictionary<string, object>()
          {
            {
              "ProjectId",
              (object) projectId.ToString()
            },
            {
              "TestRunId",
              (object) testRunId.ToString()
            },
            {
              "TestResultId",
              (object) numArray1[index].ToString()
            },
            {
              "AttachmentId",
              (object) numArray2[index].ToString()
            },
            {
              "UncompressedLength",
              (object) numArray3[index].ToString()
            },
            {
              "FileName",
              files[index] == null ? (object) string.Empty : (object) files[index].FileName
            },
            {
              "IsSuccessful",
              (object) flag2.ToString()
            }
          });
          TelemetryLogger.Instance.PublishData(context.RequestContext, nameof (ProcessMultiFilesUploadInLogStore), cid);
        }
      }
      return flag1;
    }

    private bool ProcessSingleFileUpload(
      TestManagementRequestContext context,
      Dictionary<string, string> requestParams,
      List<HttpPostedTcmAttachment> files,
      Guid projectId,
      int testRunId,
      int sessionId)
    {
      int testResultId = 0;
      int attachmentId = 0;
      int defaultAfnStripFlag = 0;
      CompressionType compressionType = CompressionType.None;
      long uncompressedLength = 0;
      HttpRange httpRange = (HttpRange) null;
      HttpPostedTcmAttachment postedTcmAttachment = (HttpPostedTcmAttachment) null;
      try
      {
        testResultId = AttachmentUploadHelper.ReadIntField(context, requestParams, "TestResultId");
        httpRange = this.ReadRangeField(context, requestParams, "Range");
        attachmentId = AttachmentUploadHelper.ReadIntField(context, requestParams, "AttachmentId");
        uncompressedLength = this.ReadLongField(context, requestParams, "UncompressedLength");
        compressionType = this.ReadCompressionType(requestParams);
        byte[] hashValue = this.ReadHashValue(requestParams);
        defaultAfnStripFlag = AttachmentUploadHelper.ReadIntField(context, requestParams, "DefaultAfnStripFlag");
        postedTcmAttachment = files != null && files.Count != 0 ? files[0] : throw new InvalidDataException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidAttachmentFile, (object) attachmentId));
        context.TraceVerbose("AttachmentHandler", "AttachmentUploadHandler.ProcessSingleFileUpload - ResultId:{0} File:{1} AttachmentId:{2} Length:{3} AFN:{4}", (object) testResultId, (object) postedTcmAttachment.FileName, (object) attachmentId, (object) uncompressedLength, (object) defaultAfnStripFlag);
        using (MemoryStream fileStream = new MemoryStream(Convert.FromBase64String(postedTcmAttachment.AttachmentContent)))
          return this.SaveFileData(context, projectId, testRunId, sessionId, testResultId, attachmentId, uncompressedLength, httpRange.Total, hashValue, compressionType, httpRange.Start, httpRange.End, defaultAfnStripFlag, postedTcmAttachment.FileName, (Stream) fileStream);
      }
      catch (Exception ex)
      {
        string str1 = httpRange != null ? httpRange.ToString() : string.Empty;
        string str2 = postedTcmAttachment != null ? string.Format("FileName:{0} ContentLength:{1} ContentType:{2}", (object) postedTcmAttachment.FileName, (object) postedTcmAttachment.ContentLength, (object) postedTcmAttachment.ContentType) : string.Empty;
        context.TraceError("AttachmentHandler", "AttachmentUploadHandler.ProcessSingleFileUpload - RunId:{0} ResultId:{1} SessionId:{2} AttachmentId:{3} Length:{4} AFN:{5} CompressionType:{6}, RangeAsString:{7}, FileInfo:{8}", (object) testRunId, (object) testResultId, (object) sessionId, (object) attachmentId, (object) uncompressedLength, (object) defaultAfnStripFlag, (object) compressionType.ToString(), (object) str1, (object) str2);
        throw;
      }
    }

    private bool ProcessMultiFilesUpload(
      TestManagementRequestContext context,
      Dictionary<string, string> requestParams,
      List<HttpPostedTcmAttachment> files,
      Guid projectId,
      int testRunId,
      int sessionId)
    {
      int[] numArray1 = this.ReadIntArrayField(context, requestParams, "TestResultIds");
      int[] numArray2 = this.ReadIntArrayField(context, requestParams, "AttachmentIds");
      if (files == null || files.Count == 0)
        throw new InvalidDataException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidAttachmentFile, (object) numArray2[0]));
      int[] numArray3 = this.ReadIntArrayField(context, requestParams, "UncompressedLengths");
      int[] numArray4 = this.ReadIntArrayField(context, requestParams, "StreamLengths");
      string[] strArray1 = this.ReadStringArrayField(requestParams, "HashValues");
      string[] strArray2 = this.ReadStringArrayField(requestParams, "CompressionTypes");
      int[] numArray5 = this.ReadIntArrayField(context, requestParams, "DefaultAfnStripFlags");
      context.TraceAndDebugAssert("AttachmentHandler", files.Count == numArray1.Length, "invalid value for testResultIds");
      context.TraceAndDebugAssert("AttachmentHandler", files.Count == numArray2.Length, "invalid value for attachmentIds");
      context.TraceAndDebugAssert("AttachmentHandler", files.Count == numArray3.Length, "invalid value for uncompressedLengths");
      context.TraceAndDebugAssert("AttachmentHandler", files.Count == numArray4.Length, "invalid value for compressedLengths");
      context.TraceAndDebugAssert("AttachmentHandler", files.Count == strArray1.Length, "invalid value for hashValues");
      context.TraceAndDebugAssert("AttachmentHandler", files.Count == strArray2.Length, "invalid value for compressionTypes");
      context.TraceAndDebugAssert("AttachmentHandler", files.Count == numArray5.Length, "invalid value for defaultAfnStripFlags");
      bool flag = false;
      for (int index = 0; index < numArray1.Length; ++index)
      {
        byte[] hashValue = this.ToHashValue(strArray1[index]);
        CompressionType compressionType = this.ToCompressionType(strArray2[index]);
        context.TraceAndDebugAssert("AttachmentHandler", numArray4[index] > 0 || numArray3[index] == 0, "invalid value for streamLengths");
        using (MemoryStream fileStream = new MemoryStream(Convert.FromBase64String(files[index].AttachmentContent)))
          flag |= this.SaveFileData(context, projectId, testRunId, sessionId, numArray1[index], numArray2[index], (long) numArray3[index], (long) numArray4[index], hashValue, compressionType, 0L, (long) numArray4[index], numArray5[index], files[index].FileName, (Stream) fileStream);
      }
      return flag;
    }

    private bool SaveFileData(
      TestManagementRequestContext context,
      Guid projectId,
      int testRunId,
      int sessionId,
      int testResultId,
      int attachmentId,
      long uncompressedLength,
      long streamLength,
      byte[] hashValue,
      CompressionType compressionType,
      long offsetFrom,
      long offsetTo,
      int defaultAfnStripFlag,
      string fileName,
      Stream fileStream)
    {
      context.TraceVerbose("AttachmentHandler", "AttachmentUploadHandler.ProcessUpload - {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}", (object) testResultId, (object) attachmentId, (object) fileName, (object) defaultAfnStripFlag, (object) uncompressedLength, (object) streamLength, (object) offsetFrom, (object) offsetTo);
      context.TraceVerbose("AttachmentHandler", "Compression Type: {0}", (object) compressionType);
      bool flag = false;
      using (Stream stream = fileStream)
      {
        using (ByteArray byteArray = new ByteArray((int) Math.Min(stream.Length, 1048576L)))
        {
          if (!context.RequestContext.IsFeatureEnabled("TestManagement.Server.AllowNegativeAttachmentIds"))
            context.TraceAndDebugAssert("AttachmentHandler", attachmentId > 0, "attachmentId cannot be < 0");
          context.TraceAndDebugAssert("AttachmentHandler", offsetTo - offsetFrom == stream.Length, "attachment size mismatch");
          do
          {
            int count = (int) Math.Min(stream.Length - stream.Position, 1048576L);
            int contentLength = stream.Read(byteArray.Bytes, 0, count);
            flag |= TestResultAttachment.AppendAttachment(context, projectId, testRunId, testResultId, attachmentId, sessionId, uncompressedLength, streamLength, hashValue, compressionType, offsetFrom, byteArray.Bytes, contentLength, defaultAfnStripFlag);
            offsetFrom += (long) contentLength;
            if (this.setTimeToFirstPage)
            {
              context.RequestContext.UpdateTimeToFirstPage();
              this.setTimeToFirstPage = false;
            }
          }
          while (stream.Position < stream.Length);
        }
      }
      return flag;
    }

    private CompressionType ToCompressionType(string compressionTypeValue) => !string.IsNullOrEmpty(compressionTypeValue) && compressionTypeValue.Equals("gzip", StringComparison.OrdinalIgnoreCase) ? CompressionType.GZip : CompressionType.None;

    private byte[] ToHashValue(string hashValueString) => !string.IsNullOrEmpty(hashValueString) && !string.Equals(hashValueString, "NULL", StringComparison.OrdinalIgnoreCase) ? Convert.FromBase64String(hashValueString) : (byte[]) null;

    private CompressionType ReadCompressionType(Dictionary<string, string> requestParams)
    {
      string compressionTypeValue;
      requestParams.TryGetValue("CompressionType", out compressionTypeValue);
      return this.ToCompressionType(compressionTypeValue);
    }

    private byte[] ReadHashValue(Dictionary<string, string> requestParams)
    {
      string hashValueString;
      requestParams.TryGetValue("HashValue", out hashValueString);
      return this.ToHashValue(hashValueString);
    }

    public static int ReadIntField(
      TestManagementRequestContext context,
      Dictionary<string, string> requestParams,
      string fieldName)
    {
      string s;
      requestParams.TryGetValue(fieldName, out s);
      int result;
      if (string.IsNullOrEmpty(s) || !int.TryParse(s, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidFieldValue, (object) fieldName)).Expected(context.RequestContext.ServiceName);
      return result;
    }

    public static int ReadIntField(
      Dictionary<string, string> requestParams,
      string fieldName,
      int defaultValueIfFieldNotFound)
    {
      string s;
      requestParams.TryGetValue(fieldName, out s);
      int result;
      return string.IsNullOrEmpty(s) || !int.TryParse(s, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result) ? defaultValueIfFieldNotFound : result;
    }

    private string ReadStringField(
      TestManagementRequestContext context,
      Dictionary<string, string> requestParams,
      string fieldName)
    {
      string stringVar;
      requestParams.TryGetValue(fieldName, out stringVar);
      ArgumentUtility.CheckStringForNullOrEmpty(stringVar, fieldName, context.RequestContext.ServiceName);
      return stringVar;
    }

    private long ReadLongField(
      TestManagementRequestContext context,
      Dictionary<string, string> requestParams,
      string fieldName)
    {
      string s;
      requestParams.TryGetValue(fieldName, out s);
      long result;
      if (string.IsNullOrEmpty(s) || !long.TryParse(s, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidFieldValue, (object) fieldName)).Expected(context.RequestContext.ServiceName);
      return result;
    }

    private HttpRange ReadRangeField(
      TestManagementRequestContext context,
      Dictionary<string, string> requestParams,
      string fieldName)
    {
      string range;
      requestParams.TryGetValue(fieldName, out range);
      HttpRange httpRange;
      if (string.IsNullOrEmpty(range) || !HttpRange.TryParse(range, out httpRange))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidFieldValue, (object) fieldName)).Expected(context.RequestContext.ServiceName);
      return httpRange;
    }

    private int[] ReadIntArrayField(
      TestManagementRequestContext context,
      Dictionary<string, string> requestParams,
      string fieldName)
    {
      string str;
      requestParams.TryGetValue(fieldName, out str);
      int[] numArray = (int[]) null;
      if (!string.IsNullOrEmpty(str))
      {
        string[] strArray = str.Split(new string[1]{ "," }, StringSplitOptions.RemoveEmptyEntries);
        numArray = new int[strArray.Length];
        for (int index = 0; index < strArray.Length; ++index)
        {
          if (!int.TryParse(strArray[index], NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out numArray[index]))
            throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidFieldValue, (object) fieldName)).Expected(context.RequestContext.ServiceName);
        }
      }
      return numArray;
    }

    private string[] ReadStringArrayField(NameValueCollection requestParams, string fieldName) => this.ReadStringArrayField(requestParams.ToDictionary(), fieldName);

    private string[] ReadStringArrayField(
      Dictionary<string, string> requestParams,
      string fieldName)
    {
      string str;
      requestParams.TryGetValue(fieldName, out str);
      if (string.IsNullOrEmpty(str))
        return (string[]) null;
      return str.Split(new string[1]{ "," }, StringSplitOptions.RemoveEmptyEntries);
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.AttachmentDownloadHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.Common.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class AttachmentDownloadHelper
  {
    private const int DownloadBufferSize = 65536;

    public void ProcessDownload(
      TestManagementRequestContext context,
      HttpRequest request,
      HttpResponse response)
    {
      int[] array = this.StringToArray(request, AttachmentDownloadFields.AttachmentId, -1);
      long[] longArray = this.StringToLongArray(request, AttachmentDownloadFields.Length, array.Length == 1, array.Length);
      if (context.IsTracing(TraceLevel.Verbose, "AttachmentHandler"))
      {
        context.TraceVerbose("AttachmentHandler", "AttachmentDownloadHandler.ProcessRequest.attachmentIds - {0}", (object) AttachmentDownloadHelper.GetStringFromArray<int>(array));
        context.TraceVerbose("AttachmentHandler", "AttachmentDownloadHandler.ProcessRequest.lengths - {0}", (object) AttachmentDownloadHelper.GetStringFromArray<long>(longArray));
      }
      List<(int, Guid)> attachmentToProjectMap = this.GetAttachmentToProjectMap(context, array);
      AttachmentDownloadHelper.ProcessMultiDownload(context, attachmentToProjectMap, longArray, response);
    }

    public (Stream contentStream, string contentType, string fileName, long contentLength) ProcessDownload(
      TestManagementRequestContext context,
      int[] attachmentIds,
      long[] lengths,
      out List<(int attachmentId, Guid projectId)> attachmentProjectMap)
    {
      context.RequestContext.Trace(1015791, TraceLevel.Info, "TestManagement", "DownloadAttachment", string.Format("AttachmentId Is Null:{0}, AttachmentId length:{1} And Lengths Is Null:{2}, Lengths length:{3}", (object) (attachmentIds == null), (object) attachmentIds?.Length, (object) (lengths == null), (object) lengths?.Length));
      attachmentProjectMap = this.GetAttachmentToProjectMap(context, attachmentIds);
      if (context.IsTracing(TraceLevel.Verbose, "AttachmentHandler"))
      {
        context.TraceVerbose("AttachmentHandler", "AttachmentDownloadHandler.ProcessRequest.attachmentIds - {0}", (object) AttachmentDownloadHelper.GetStringFromArray<int>(attachmentIds));
        context.TraceVerbose("AttachmentHandler", "AttachmentDownloadHandler.ProcessRequest.lengths - {0}", (object) AttachmentDownloadHelper.GetStringFromArray<long>(lengths));
      }
      return AttachmentDownloadHelper.ProcessMultiDownload(context, attachmentProjectMap, lengths);
    }

    public (int[] AttachmentIds, long[] Lengths) ExtractAttachmentsInfo(HttpRequest request)
    {
      int[] array = this.StringToArray(request, AttachmentDownloadFields.AttachmentId, -1);
      long[] longArray = this.StringToLongArray(request, AttachmentDownloadFields.Length, array.Length == 1, array.Length);
      return (array, longArray);
    }

    private List<(int attachmentId, Guid projectId)> GetAttachmentToProjectMap(
      TestManagementRequestContext context,
      int[] attachmentIds)
    {
      bool? nullable1 = new bool?();
      Dictionary<Guid, bool> dictionary = new Dictionary<Guid, bool>();
      List<(int, Guid)> attachmentToProjectMap = new List<(int, Guid)>();
      if (attachmentIds == null)
        return attachmentToProjectMap;
      foreach (int attachmentId in attachmentIds)
      {
        Guid? nullable2 = new Guid?();
        ProjectInfo projectInfo = new ProjectInfo()
        {
          Uri = string.Empty,
          Name = string.Empty
        };
        int sessionId = 0;
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          nullable2 = managementDatabase.GetProjectForAttachment(attachmentId, out int _, out sessionId);
        if (nullable2.HasValue && !dictionary.ContainsKey(nullable2.Value))
        {
          projectInfo = context.ProjectServiceHelper.GetProjectFromGuid(nullable2.Value);
          dictionary.Add(nullable2.Value, context.SecurityManager.HasViewTestResultsPermission(context, projectInfo.Uri));
        }
        if (nullable2.HasValue && !dictionary[nullable2.Value])
        {
          if (!nullable1.HasValue)
            nullable1 = new bool?(CommonLicenseCheckHelper.IsStakeholder(context.RequestContext));
          if (!nullable1.Value || sessionId == 0)
          {
            context.TraceWarning("WebService", "AttachmentDownloadHandler.ProcessRequest: User {0} doesn't have permissions for project {1}. So Attachment {2} can't be served", (object) (context.UserSID ?? ""), (object) projectInfo.Name, (object) attachmentId);
            throw new AccessDeniedException(ServerResources.CannotReadProject);
          }
        }
        if (nullable2.HasValue)
          attachmentToProjectMap.Add((attachmentId, nullable2.Value));
      }
      return attachmentToProjectMap;
    }

    private static string GetStringFromArray<T>(T[] array) => array != null ? string.Join<T>(",", (IEnumerable<T>) array) : string.Empty;

    private int[] StringToArray(HttpRequest request, string parameterName, int requiredLength)
    {
      bool flag = false;
      int[] array = (int[]) null;
      string str = request.Params[parameterName];
      if (str != null)
      {
        string[] strArray = str.Split(',');
        if (requiredLength < 1 || strArray.Length == requiredLength)
        {
          array = new int[strArray.Length];
          flag = true;
          for (int index = 0; index < strArray.Length; ++index)
          {
            if (!int.TryParse(strArray[index], out array[index]))
            {
              flag = false;
              break;
            }
          }
        }
      }
      if (flag)
        return array;
      throw new TestManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidFieldValue, (object) parameterName));
    }

    private long[] StringToLongArray(
      HttpRequest request,
      string parameterName,
      bool allowNull,
      int requiredLength)
    {
      bool flag = false;
      long[] longArray = (long[]) null;
      string str = request.Params[parameterName];
      if (str != null)
      {
        string[] strArray = str.Split(',');
        if (requiredLength < 1 || strArray.Length == requiredLength)
        {
          longArray = new long[strArray.Length];
          flag = true;
          for (int index = 0; index < strArray.Length; ++index)
          {
            if (!long.TryParse(strArray[index], out longArray[index]))
            {
              flag = false;
              break;
            }
          }
        }
      }
      else
        flag = allowNull;
      if (flag)
        return longArray;
      throw new TestManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidFieldValue, (object) parameterName));
    }

    internal static (Stream contentStream, string contentType, string fileName, long contentLength) ProcessMultiDownload(
      TestManagementRequestContext context,
      List<(int attachmentId, Guid projectId)> attachments,
      long[] lengths)
    {
      string attachmentName = string.Empty;
      string str1 = string.Empty;
      string str2 = string.Empty;
      long num = 0;
      context.RequestContext.UpdateTimeToFirstPage();
      MemoryStream memoryStream = (MemoryStream) null;
      using (ByteArray byteArray = new ByteArray(65536))
      {
        if (attachments != null)
        {
          if (attachments.Any<(int, Guid)>())
          {
            memoryStream = new MemoryStream();
            for (int index = 0; index < attachments.Count; ++index)
            {
              CompressionType compressionType;
              using (Stream attachmentStream = TestResultAttachment.GetAttachmentStream(context, attachments[index].projectId, attachments[index].attachmentId, out attachmentName, out compressionType))
              {
                if (attachmentStream == null)
                  return ((Stream) null, (string) null, (string) null, 0L);
                context.TraceVerbose("AttachmentHandler", "ProcessMultiDownload: Attachment Name: {0}", (object) attachmentName);
                context.TraceVerbose("AttachmentHandler", "ProcessMultiDownload: Attachment compression type: {0}", (object) compressionType);
                if (index == 0)
                {
                  str1 = AttachmentDownloadHelper.GetContentType(attachmentName);
                  context.TraceVerbose("AttachmentHandler", "ProcessMultiDownload: Attachment content type: {0}", (object) str1);
                  if (attachments.Count == 1)
                  {
                    str2 = AttachmentDownloadHelper.GetAttachmentName(attachmentName);
                    num = attachmentStream.Length;
                  }
                }
                if (lengths != null && attachmentStream.Length != lengths[index])
                  return ((Stream) null, (string) null, (string) null, 0L);
                int count;
                while ((count = attachmentStream.Read(byteArray.Bytes, 0, byteArray.Bytes.Length)) > 0)
                  memoryStream.Write(byteArray.Bytes, 0, count);
              }
            }
          }
        }
      }
      memoryStream?.Seek(0L, SeekOrigin.Begin);
      return ((Stream) memoryStream, str1, str2, num);
    }

    internal static void ProcessMultiDownload(
      TestManagementRequestContext context,
      List<(int attachmentId, Guid projectId)> attachments,
      long[] lengths,
      HttpResponse response)
    {
      string attachmentName = string.Empty;
      if (response == null)
        response = HttpContext.Current.Response;
      context.RequestContext.UpdateTimeToFirstPage();
      using (ByteArray byteArray = new ByteArray(65536))
      {
        if (attachments != null)
        {
          for (int index = 0; index < attachments.Count; ++index)
          {
            CompressionType compressionType;
            using (Stream attachmentStream = TestResultAttachment.GetAttachmentStream(context, attachments[index].projectId, attachments[index].attachmentId, out attachmentName, out compressionType))
            {
              if (attachmentStream == null)
              {
                response.StatusCode = 404;
                return;
              }
              context.TraceVerbose("AttachmentHandler", "ProcessMultiDownload: Attachment Name: {0}", (object) attachmentName);
              context.TraceVerbose("AttachmentHandler", "ProcessMultiDownload: Attachment compression type: {0}", (object) compressionType);
              if (index == 0)
              {
                response.BufferOutput = false;
                response.ContentType = AttachmentDownloadHelper.GetContentType(attachmentName);
                context.TraceVerbose("AttachmentHandler", "ProcessMultiDownload: Attachment content type: {0}", (object) response.ContentType);
                if (attachments.Count == 1)
                {
                  response.AppendHeader("Content-Disposition", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "attachment; filename={0}", (object) AttachmentDownloadHelper.GetAttachmentName(attachmentName)));
                  response.AppendHeader("content-length", attachmentStream.Length.ToString((IFormatProvider) CultureInfo.InvariantCulture));
                }
              }
              if (lengths != null && attachmentStream.Length != lengths[index])
                return;
              while (response.IsClientConnected)
              {
                int count;
                if ((count = attachmentStream.Read(byteArray.Bytes, 0, byteArray.Bytes.Length)) > 0)
                  response.OutputStream.Write(byteArray.Bytes, 0, count);
                else
                  break;
              }
            }
          }
        }
      }
      response.Flush();
    }

    internal static void ProcessDownload(
      TestManagementRequestContext context,
      int testRunId,
      int testResultId,
      int attachmentId,
      int sessionId,
      HttpResponse response)
    {
      Guid? nullable = new Guid?();
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        nullable = managementDatabase.GetProjectForAttachment(attachmentId, out testRunId, out sessionId);
      if (!nullable.HasValue)
        throw new TestObjectNotFoundException(context.RequestContext, attachmentId, ObjectTypes.Attachment);
      ProjectInfo projectFromGuid = context.ProjectServiceHelper.GetProjectFromGuid(nullable.Value);
      context.SecurityManager.CheckViewTestResultsPermission(context, projectFromGuid.Uri);
      TestManagementRequestContext context1 = context;
      List<(int, Guid)> attachments = new List<(int, Guid)>();
      attachments.Add((attachmentId, nullable.Value));
      HttpResponse response1 = response;
      AttachmentDownloadHelper.ProcessMultiDownload(context1, attachments, (long[]) null, response1);
    }

    public static string GetContentType(string fileName)
    {
      string extension = Path.GetExtension(fileName);
      return string.Equals(extension, ".jpeg", StringComparison.OrdinalIgnoreCase) || string.Equals(extension, ".jpg", StringComparison.OrdinalIgnoreCase) || string.Equals(extension, ".jpe", StringComparison.OrdinalIgnoreCase) ? "image/jpeg" : (string.Equals(extension, ".tiff", StringComparison.OrdinalIgnoreCase) || string.Equals(extension, ".tif", StringComparison.OrdinalIgnoreCase) ? "image/tiff" : (!string.Equals(extension, ".png", StringComparison.OrdinalIgnoreCase) ? (!string.Equals(extension, ".gif", StringComparison.OrdinalIgnoreCase) ? (!string.Equals(extension, ".bmp", StringComparison.OrdinalIgnoreCase) ? (!string.Equals(extension, ".jfif", StringComparison.OrdinalIgnoreCase) ? (!string.Equals(extension, ".ief", StringComparison.OrdinalIgnoreCase) ? (!string.Equals(extension, ".ico", StringComparison.OrdinalIgnoreCase) ? "application/octet-stream" : "image/x-icon") : "image/ief") : "image/pipeg") : "image/bmp") : "image/gif") : "image/png"));
    }

    internal static string GetAttachmentName(string filePath)
    {
      if (string.IsNullOrEmpty(filePath) || HttpContext.Current == null)
        return string.Empty;
      if (!string.Equals("IE", HttpContext.Current.Request.Browser.Browser, StringComparison.OrdinalIgnoreCase))
        return filePath;
      int num = filePath.LastIndexOf('.');
      string str1;
      string str2;
      if (num >= 0)
      {
        str1 = filePath.Substring(0, num);
        str2 = filePath.Substring(num);
      }
      else
      {
        str1 = filePath;
        str2 = "";
      }
      return HttpContext.Current.Server.UrlPathEncode(str1.Replace(".", "%2e")) + str2;
    }

    private int[] GetSessionIds(HttpRequest request, int requiredLength)
    {
      try
      {
        return this.StringToArray(request, AttachmentDownloadFields.SessionId, requiredLength);
      }
      catch (TestManagementServiceException ex)
      {
        return this.GetDefaultValueIntArray(requiredLength);
      }
    }

    private int[] GetDefaultValueIntArray(int requiredLength)
    {
      if (requiredLength <= 0)
        return (int[]) null;
      int[] defaultValueIntArray = new int[requiredLength];
      for (int index = 0; index < requiredLength; ++index)
        defaultValueIntArray[index] = 0;
      return defaultValueIntArray;
    }
  }
}

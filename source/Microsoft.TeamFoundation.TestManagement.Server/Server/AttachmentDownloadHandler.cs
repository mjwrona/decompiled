// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.AttachmentDownloadHandler
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Server.Legacy;
using Microsoft.VisualStudio.Services.Tcm.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class AttachmentDownloadHandler : TeamFoundationHttpHandler
  {
    private const int DownloadBufferSize = 65536;

    protected override bool AllowSimplePostRequests => true;

    protected override void ProcessRequestImpl(HttpContext context)
    {
      HttpRequest request = context.Request;
      HttpResponse response = context.Response;
      TfsTestManagementRequestContext context1 = new TfsTestManagementRequestContext(this.RequestContext);
      this.EnterMethod(new MethodInformation("TCMAttachmentDownloadHandler", MethodType.Normal, EstimatedMethodCost.Low, true, true));
      try
      {
        AttachmentDownloadHelper attachmentDownloadHelper = new AttachmentDownloadHelper();
        (int[] AttachmentIds, long[] Lengths) attachmentsInfo = attachmentDownloadHelper.ExtractAttachmentsInfo(request);
        ILegacyTCMServiceHelper tcmServiceHelper = context1.LegacyTcmServiceHelper;
        TfsTestManagementRequestContext requestContext = context1;
        List<int> list1 = ((IEnumerable<int>) attachmentsInfo.AttachmentIds).ToList<int>();
        long[] lengths = attachmentsInfo.Lengths;
        List<long> list2 = lengths != null ? ((IEnumerable<long>) lengths).ToList<long>() : (List<long>) null;
        TcmAttachment tcmAttachment;
        ref TcmAttachment local = ref tcmAttachment;
        if (tcmServiceHelper.TryDownloadAttachments(requestContext, list1, list2, out local))
        {
          using (ByteArray byteArray = new ByteArray(65536))
          {
            response.ContentType = tcmAttachment.ContentType;
            Stream stream = tcmAttachment.Stream;
            int count;
            while ((count = stream.Read(byteArray.Bytes, 0, byteArray.Bytes.Length)) > 0)
              response.OutputStream.Write(byteArray.Bytes, 0, count);
            if (attachmentsInfo.AttachmentIds.Length == 1)
            {
              response.AppendHeader("Content-Disposition", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "attachment; filename={0}", (object) tcmAttachment.FileName));
              response.AppendHeader("content-length", tcmAttachment.ContentLength.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            }
          }
          response.Flush();
        }
        else
          attachmentDownloadHelper.ProcessDownload((TestManagementRequestContext) context1, request, response);
      }
      catch (Exception ex)
      {
        this.HandleException(ex, "X-TestManagement-Exception", 500, !response.BufferOutput);
      }
      finally
      {
        this.LeaveMethod();
      }
    }
  }
}

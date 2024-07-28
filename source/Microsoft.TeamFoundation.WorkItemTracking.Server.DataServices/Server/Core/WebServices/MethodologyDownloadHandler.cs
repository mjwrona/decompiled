// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.WebServices.MethodologyDownloadHandler
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 929F0284-16B2-4277-9F4A-B615689A77D1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Web;

namespace Microsoft.TeamFoundation.Server.Core.WebServices
{
  public class MethodologyDownloadHandler : FrameworkHttpHandler
  {
    protected override void Execute()
    {
      MethodInformation methodInformation = new MethodInformation("MethodologyDownload", MethodType.Normal, EstimatedMethodCost.Moderate);
      HttpRequest request = HttpContext.Current.Request;
      HttpResponse response = HttpContext.Current.Response;
      try
      {
        ITeamFoundationProcessService service = this.RequestContext.GetService<ITeamFoundationProcessService>();
        int result;
        Guid parameterValue = !int.TryParse(request.Params["methodologyIndex"], NumberStyles.None, (IFormatProvider) CultureInfo.InvariantCulture, out result) ? Guid.Parse(request.Params["templateId"]) : service.GetSpecificProcessDescriptorIdByIntegerId(this.RequestContext, result);
        methodInformation.AddParameter("TemplateId", (object) parameterValue);
        this.EnterMethod(methodInformation);
        if (result < 0)
          throw new ArgumentOutOfRangeException("context", "templateIntegerId");
        Guid descriptorIdByIntegerId = service.GetSpecificProcessDescriptorIdByIntegerId(this.RequestContext, result);
        MethodologyDownloadHandler.TransferTemplateFileContent(this.RequestContext, service.GetSpecificProcessDescriptor(this.RequestContext, descriptorIdByIntegerId), response);
      }
      catch (Exception ex)
      {
        TeamFoundationEventLog.Default.LogException(ex.Message, ex, TeamFoundationEventId.TemplateDownloadException, EventLogEntryType.Warning);
        this.HandleException(ex, "X-TFS-Exception", 500, false);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    private static void TransferTemplateFileContent(
      IVssRequestContext requestContext,
      ProcessDescriptor descriptor,
      HttpResponse response)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<HttpResponse>(response, nameof (response));
      if (descriptor.IsDerived)
        throw new ProcessTemplateInvalidDownloadOnInheritedException();
      requestContext.GetService<TeamFoundationFileCacheService>();
      using (Stream transformedWebLayout = ProcessPackageFormTransformer.GetProcessPackageContentWithTransformedWebLayout(requestContext, descriptor))
      {
        FileInformation fileInformation = new FileInformation(requestContext.ServiceHost.InstanceId, descriptor.FileId, (byte[]) null);
        fileInformation.UncompressedLength = transformedWebLayout.Length;
        fileInformation.Length = transformedWebLayout.Length;
        MethodologyDownloadHandler.SetHeaders(response, fileInformation);
        transformedWebLayout.CopyTo(response.OutputStream);
        response.OutputStream.Flush();
      }
    }

    private static void SetHeaders(HttpResponse response, FileInformation fileInformation)
    {
      if (!response.IsClientConnected)
        return;
      response.BufferOutput = true;
      response.ContentType = fileInformation.ContentType;
      response.CacheControl = "no-cache";
      response.AppendHeader("Content-Length", fileInformation.Length.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    }
  }
}

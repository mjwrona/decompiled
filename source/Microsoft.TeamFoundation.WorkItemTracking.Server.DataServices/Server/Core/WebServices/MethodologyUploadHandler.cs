// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.WebServices.MethodologyUploadHandler
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 929F0284-16B2-4277-9F4A-B615689A77D1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using System;
using System.Text;
using System.Web;

namespace Microsoft.TeamFoundation.Server.Core.WebServices
{
  public class MethodologyUploadHandler : FrameworkHttpHandler
  {
    public MethodologyUploadHandler()
    {
    }

    public MethodologyUploadHandler(HttpContextBase context)
      : base(context)
    {
    }

    protected override bool AllowSimplePostRequests => true;

    protected override void Execute() => this.ExecuteImpl((HttpRequestBase) new HttpRequestWrapper(HttpContext.Current.Request), (HttpResponseBase) new HttpResponseWrapper(HttpContext.Current.Response));

    internal void ExecuteImpl(HttpRequestBase request, HttpResponseBase response)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation("MethodologyUpload", MethodType.Admin, EstimatedMethodCost.High);
        response.BufferOutput = false;
        ApiStackRouter.ValidateSimplePostRequestOrigin((TeamFoundationHttpHandler) this, this.RequestContext, request);
        HttpPostedFileBase file;
        this.ParseRequestParameters(request, response, methodInformation, out string _, out string _, out string _, out file);
        this.EnterMethod(methodInformation);
        if (file == null)
          return;
        this.RequestContext.GetService<ITeamFoundationProcessService>().CreateOrUpdateLegacyProcess(this.RequestContext, file.InputStream);
      }
      catch (ProcessServiceException ex)
      {
        response.StatusCode = ex is ProcessServicePermissionException ? 403 : 400;
        response.StatusDescription = ex.Message;
        response.End();
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    private void ParseRequestParameters(
      HttpRequestBase request,
      HttpResponseBase response,
      MethodInformation methodInformation,
      out string name,
      out string description,
      out string metadata,
      out HttpPostedFileBase file)
    {
      name = this.ExtractParameterValue(request.Params["templateName"]);
      description = this.ExtractParameterValue(request.Params[nameof (description)]);
      metadata = this.ExtractParameterValue(request.Params[nameof (metadata)]);
      file = request.Files["content"];
      methodInformation.AddParameter(nameof (name), (object) name);
      methodInformation.AddParameter(nameof (description), (object) description);
      methodInformation.AddParameter(nameof (metadata), (object) metadata);
      if (file == null)
      {
        if (request.HttpMethod == "POST")
          throw new IncompleteUploadException(name);
        response.StatusCode = 400;
      }
      else
      {
        if (string.IsNullOrEmpty(name))
          throw new ArgumentException(FrameworkResources.ParameterFormatException((object) nameof (name)));
        try
        {
          TeamFoundationSerializationUtility.SerializeToDocument(metadata);
        }
        catch
        {
          throw new ArgumentException(FrameworkResources.ParameterFormatException((object) nameof (metadata)));
        }
      }
    }

    private string ExtractParameterValue(string parameter)
    {
      if (string.IsNullOrEmpty(parameter))
        return string.Empty;
      try
      {
        return Encoding.UTF8.GetString(Convert.FromBase64String(parameter));
      }
      catch (FormatException ex)
      {
        TeamFoundationTrace.TraceException((Exception) ex);
      }
      return HttpUtility.HtmlDecode(parameter);
    }
  }
}

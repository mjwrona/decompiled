// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GvfsHttpHandler
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Net;
using System.Web;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal abstract class GvfsHttpHandler : GitHttpHandler
  {
    public GvfsHttpHandler()
    {
    }

    public GvfsHttpHandler(HttpContextBase context)
      : base(context)
    {
    }

    protected abstract TimeSpan Timeout { get; }

    protected bool ResponseStarted { get; set; }

    internal virtual void ProcessGet(RepoNameKey nameKey) => this.WriteCurrentMethodUnsupported();

    internal virtual void ProcessPost(RepoNameKey nameKey) => this.WriteCurrentMethodUnsupported();

    internal virtual void ProcessPut(RepoNameKey nameKey) => this.WriteCurrentMethodUnsupported();

    internal override sealed void Execute()
    {
      this.HandlerHttpContext.OverrideRequestTimeoutSeconds((int) this.Timeout.TotalSeconds);
      this.HandlerHttpContext.Response.BufferOutput = false;
      HttpRequestBase request = this.HandlerHttpContext.Request;
      RepoNameKey nameKey = RepoNameKey.FromRequest(request);
      try
      {
        this.EnterMethod(nameKey);
        switch (request.HttpMethod)
        {
          case "GET":
            this.ProcessGet(nameKey);
            break;
          case "POST":
            this.ProcessPost(nameKey);
            break;
          case "PUT":
            this.ProcessPut(nameKey);
            break;
          default:
            this.WriteCurrentMethodUnsupported();
            break;
        }
      }
      catch (Exception ex)
      {
        if (this.HandleException(ex, this.ResponseStarted))
          return;
        throw;
      }
    }

    protected void WriteCurrentMethodUnsupported() => this.WriteTextResponse(HttpStatusCode.MethodNotAllowed, Resources.Format("UnsupportedHttpMethod", (object) this.HandlerHttpContext.Request.HttpMethod));

    protected void WriteTextResponse(HttpStatusCode statusCode, string message)
    {
      HttpResponseBase response = this.HandlerHttpContext.Response;
      response.StatusCode = (int) statusCode;
      response.ContentType = "text/plain";
      response.Write(message);
    }

    private void EnterMethod(RepoNameKey nameKey)
    {
      MethodInformation methodInformation = new MethodInformation(this.Layer, MethodType.Normal, EstimatedMethodCost.Moderate, this.Timeout);
      methodInformation.AddParameter("ProjectName", (object) nameKey.ProjectName);
      methodInformation.AddParameter("RepositoryName", (object) nameKey.RepositoryName);
      this.EnterMethod(methodInformation);
    }
  }
}

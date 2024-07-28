// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GvfsGetObjectHandler
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Net;
using System.Web;

namespace Microsoft.TeamFoundation.Git.Server
{
  [GvfsPublicProjectRequestRestrictions]
  internal class GvfsGetObjectHandler : GvfsHttpHandler
  {
    private const string c_layer = "GvfsGetObjectHandler";

    public GvfsGetObjectHandler()
    {
    }

    public GvfsGetObjectHandler(HttpContextBase context)
      : base(context)
    {
    }

    protected override TimeSpan Timeout => TimeSpan.FromHours(24.0);

    protected override string Layer => nameof (GvfsGetObjectHandler);

    internal override void ProcessGet(RepoNameKey nameKey)
    {
      Sha1Id id;
      if (!Sha1Id.TryParse(this.HandlerHttpContext.Request.RequestContext.RouteData.Values["oid"].ToString(), out id))
      {
        this.WriteTextResponse(HttpStatusCode.BadRequest, "An invalid ObjectId was supplied in the request URI.");
      }
      else
      {
        using (ITfsGitRepository repositoryByName = this.RequestContext.GetService<ITeamFoundationGitRepositoryService>().FindRepositoryByName(this.RequestContext, nameKey.ProjectName, nameKey.RepositoryName))
        {
          HttpResponseBase response = this.HandlerHttpContext.Response;
          response.StatusCode = 200;
          response.ContentType = GvfsServerConstants.LooseObjectResponseContentType;
          GitLooseObjectWriter looseObjectWriter = new GitLooseObjectWriter(this.RequestContext.RequestTracer, repositoryByName);
          try
          {
            looseObjectWriter.Write(response.OutputStream, id, (Action) (() =>
            {
              this.ResponseStarted = true;
              this.RequestContext.UpdateTimeToFirstPage();
            }));
          }
          catch (GitObjectDoesNotExistException ex)
          {
            this.RequestContext.TraceCatch(1013567, GitServerUtils.TraceArea, nameof (GvfsGetObjectHandler), (Exception) ex);
            this.WriteTextResponse(HttpStatusCode.NotFound, ex.Message);
          }
        }
      }
    }
  }
}

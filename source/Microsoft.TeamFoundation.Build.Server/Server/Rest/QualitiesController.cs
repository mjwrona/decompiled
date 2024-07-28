// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Rest.QualitiesController
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build.Server.Rest
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "Build", ResourceName = "Qualities")]
  public class QualitiesController : BuildApiController
  {
    private static Dictionary<Type, HttpStatusCode> s_httpExceptions = new Dictionary<Type, HttpStatusCode>();

    static QualitiesController()
    {
      QualitiesController.s_httpExceptions.Add(typeof (UriFormatException), HttpStatusCode.BadRequest);
      QualitiesController.s_httpExceptions.Add(typeof (IndexOutOfRangeException), HttpStatusCode.BadRequest);
      QualitiesController.s_httpExceptions.Add(typeof (InvalidPathException), HttpStatusCode.BadRequest);
      QualitiesController.s_httpExceptions.Add(typeof (FormatException), HttpStatusCode.BadRequest);
      QualitiesController.s_httpExceptions.Add(typeof (ArgumentOutOfRangeException), HttpStatusCode.BadRequest);
      QualitiesController.s_httpExceptions.Add(typeof (ArgumentNullException), HttpStatusCode.BadRequest);
    }

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) QualitiesController.s_httpExceptions;

    [HttpGet]
    [ClientResponseType(typeof (IList<InformationNode>), null, null)]
    public HttpResponseMessage GetQualities(string projectId = null)
    {
      if (this.ProjectId != Guid.Empty)
        projectId = this.ProjectId.ToString("D");
      ArgumentUtility.CheckStringForNullOrEmpty(projectId, nameof (projectId));
      return this.GenerateResponse<string>((IEnumerable<string>) this.TfsRequestContext.GetService<TeamFoundationBuildService>().GetBuildQualities(this.TfsRequestContext, projectId));
    }

    [HttpPut]
    public void CreateQuality(string quality, string projectId = null)
    {
      if (this.ProjectId != Guid.Empty)
        projectId = this.ProjectId.ToString("D");
      ArgumentUtility.CheckStringForNullOrEmpty(projectId, nameof (projectId));
      ArgumentUtility.CheckStringForNullOrEmpty(quality, nameof (quality));
      this.TfsRequestContext.GetService<TeamFoundationBuildService>().AddBuildQualities(this.TfsRequestContext, projectId, (IList<string>) new string[1]
      {
        quality
      });
    }

    [HttpDelete]
    public void DeleteQuality(string quality, string projectId = null)
    {
      if (this.ProjectId != Guid.Empty)
        projectId = this.ProjectId.ToString("D");
      ArgumentUtility.CheckStringForNullOrEmpty(projectId, nameof (projectId));
      ArgumentUtility.CheckStringForNullOrEmpty(quality, nameof (quality));
      this.TfsRequestContext.GetService<TeamFoundationBuildService>().DeleteBuildQualities(this.TfsRequestContext, projectId, (IList<string>) new string[1]
      {
        quality
      });
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Rest.DetailsController
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build.Server.Rest
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "Build", ResourceName = "Details")]
  public class DetailsController : BuildApiController
  {
    private static Dictionary<Type, HttpStatusCode> s_httpExceptions = new Dictionary<Type, HttpStatusCode>();

    static DetailsController()
    {
      DetailsController.s_httpExceptions.Add(typeof (UriFormatException), HttpStatusCode.BadRequest);
      DetailsController.s_httpExceptions.Add(typeof (BuildNotFoundException), HttpStatusCode.NotFound);
      DetailsController.s_httpExceptions.Add(typeof (Microsoft.TeamFoundation.Build.WebApi.AccessDeniedException), HttpStatusCode.Forbidden);
      DetailsController.s_httpExceptions.Add(typeof (InvalidPathException), HttpStatusCode.BadRequest);
      DetailsController.s_httpExceptions.Add(typeof (FormatException), HttpStatusCode.BadRequest);
      DetailsController.s_httpExceptions.Add(typeof (ArgumentOutOfRangeException), HttpStatusCode.BadRequest);
      DetailsController.s_httpExceptions.Add(typeof (IndexOutOfRangeException), HttpStatusCode.BadRequest);
    }

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) DetailsController.s_httpExceptions;

    [HttpGet]
    [ClientResponseType(typeof (IList<InformationNode>), null, null)]
    public HttpResponseMessage GetDetails(int buildId, [FromUri] string[] types, [FromUri(Name = "$top")] int top = 2147483647, [FromUri(Name = "$skip")] int skip = 0)
    {
      try
      {
        ArgumentUtility.CheckBoundsInclusive(buildId, 1, int.MaxValue, nameof (buildId));
        ArgumentUtility.CheckBoundsInclusive(top, 1, int.MaxValue, "$top");
        ArgumentUtility.CheckBoundsInclusive(skip, 0, int.MaxValue, nameof (skip));
      }
      catch (ArgumentOutOfRangeException ex)
      {
        return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
      }
      ITeamFoundationBuildService service = this.TfsRequestContext.GetService<ITeamFoundationBuildService>();
      ArtifactId artifactId = new ArtifactId("Build", "Build", buildId.ToString());
      if (types != null)
        types = ((IEnumerable<string>) types).Where<string>((Func<string, bool>) (s => !string.IsNullOrEmpty(s))).Distinct<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).ToArray<string>();
      if (types == null || types.Length == 0)
        types = new string[1]{ BuildConstants.Star };
      ITeamFoundationBuildService foundationBuildService = service;
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      List<string> uris = new List<string>();
      uris.Add(LinkingUtilities.EncodeUri(artifactId));
      string[] informationTypes = types;
      Guid projectId = this.ProjectId;
      using (TeamFoundationDataReader foundationDataReader = foundationBuildService.QueryBuildsByUri(tfsRequestContext, (IList<string>) uris, (IList<string>) informationTypes, QueryOptions.None, QueryDeletedOption.IncludeDeleted, projectId))
      {
        BuildDetail buildDetail = foundationDataReader.Current<BuildQueryResult>().Builds.ToList<BuildDetail>().ElementAt<BuildDetail>(0);
        if (buildDetail != null)
          return this.GenerateResponse<InformationNode>(buildDetail.Information.Skip<BuildInformationNode>(skip).Take<BuildInformationNode>(top).Select<BuildInformationNode, InformationNode>((Func<BuildInformationNode, InformationNode>) (x => this.ConvertInformationNodesToDataContract(x))));
      }
      QueuedBuild queuedBuild = (QueuedBuild) null;
      List<int> intList;
      if (service.GetQueueIdsByBuildIds(this.TfsRequestContext, this.ProjectId, (IList<int>) new int[1]
      {
        buildId
      }).TryGetValue(buildId, out intList) && intList.Count > 0)
      {
        TeamFoundationDataReader foundationDataReader;
        try
        {
          foundationDataReader = service.QueryQueuedBuildsById(this.TfsRequestContext, (IList<int>) new int[1]
          {
            intList[0]
          }, (IList<string>) null, QueryOptions.None, false, this.ProjectId);
        }
        catch (ProjectDoesNotExistWithNameException ex)
        {
          throw new BuildNotFoundException(ResourceStrings.BuildNotFound((object) buildId.ToString()));
        }
        using (foundationDataReader)
          queuedBuild = foundationDataReader.Current<BuildQueueQueryResult>().QueuedBuilds.FirstOrDefault<QueuedBuild>();
      }
      if (queuedBuild != null)
        return this.GenerateResponse<InformationNode>(Enumerable.Empty<InformationNode>());
      throw new BuildNotFoundException(ResourceStrings.BuildNotFound((object) buildId.ToString()));
    }

    private InformationNode ConvertInformationNodesToDataContract(BuildInformationNode infoNode)
    {
      InformationNode dataContract = new InformationNode();
      dataContract.LastModifiedBy = infoNode.LastModifiedBy;
      dataContract.LastModifiedDate = infoNode.LastModifiedDate;
      dataContract.NodeId = infoNode.NodeId;
      dataContract.ParentId = infoNode.ParentId;
      dataContract.Type = infoNode.Type;
      foreach (InformationField field in infoNode.Fields)
        dataContract.Fields.Add(field.Name, field.Value);
      return dataContract;
    }
  }
}

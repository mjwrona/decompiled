// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Rest.QueuesController
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.WebApis;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build.Server.Rest
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "Build", ResourceName = "Queues")]
  public class QueuesController : BuildApiController
  {
    private static Dictionary<Type, HttpStatusCode> s_httpExceptions = new Dictionary<Type, HttpStatusCode>();

    static QueuesController()
    {
      QueuesController.s_httpExceptions.Add(typeof (UriFormatException), HttpStatusCode.BadRequest);
      QueuesController.s_httpExceptions.Add(typeof (Microsoft.TeamFoundation.Build.Server.AccessDeniedException), HttpStatusCode.NotFound);
      QueuesController.s_httpExceptions.Add(typeof (Microsoft.TeamFoundation.Build.WebApi.BuildControllerNotFoundException), HttpStatusCode.NotFound);
      QueuesController.s_httpExceptions.Add(typeof (Microsoft.TeamFoundation.Build.WebApi.BuildControllerDeletionException), HttpStatusCode.BadRequest);
      QueuesController.s_httpExceptions.Add(typeof (IndexOutOfRangeException), HttpStatusCode.BadRequest);
      QueuesController.s_httpExceptions.Add(typeof (InvalidPathException), HttpStatusCode.BadRequest);
      QueuesController.s_httpExceptions.Add(typeof (FormatException), HttpStatusCode.BadRequest);
      QueuesController.s_httpExceptions.Add(typeof (ArgumentOutOfRangeException), HttpStatusCode.BadRequest);
      QueuesController.s_httpExceptions.Add(typeof (Microsoft.TeamFoundation.Build.WebApi.BuildControllerUpdateException), HttpStatusCode.BadRequest);
      QueuesController.s_httpExceptions.Add(typeof (Microsoft.TeamFoundation.Build.WebApi.AccessDeniedException), HttpStatusCode.Forbidden);
      QueuesController.s_httpExceptions.Add(typeof (ArgumentNullException), HttpStatusCode.BadRequest);
    }

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) QueuesController.s_httpExceptions;

    [StreamingCollectionResponse]
    [HttpGet]
    public IEnumerable<Microsoft.TeamFoundation.Build.WebApi.BuildController> GetQueues(
      string controllerName = "*",
      string serviceHost = "*")
    {
      QueuesController queuesController = this;
      if (string.IsNullOrEmpty(controllerName))
        ArgumentUtility.CheckStringForNullOrEmpty(controllerName, nameof (controllerName));
      if (string.IsNullOrEmpty(serviceHost))
        ArgumentUtility.CheckStringForNullOrEmpty(serviceHost, nameof (serviceHost));
      TeamFoundationBuildResourceService service = queuesController.TfsRequestContext.GetService<TeamFoundationBuildResourceService>();
      BuildControllerSpec buildControllerSpec = new BuildControllerSpec();
      buildControllerSpec.IncludeAgents = false;
      buildControllerSpec.ServiceHostName = serviceHost;
      buildControllerSpec.Name = controllerName;
      IVssRequestContext tfsRequestContext = queuesController.TfsRequestContext;
      BuildControllerSpec controllerSpec = buildControllerSpec;
      foreach (Microsoft.TeamFoundation.Build.Server.BuildController controller in service.QueryBuildControllers(tfsRequestContext, controllerSpec).Controllers)
        yield return queuesController.ConvertControllerToContract(controller);
    }

    [HttpGet]
    public Microsoft.TeamFoundation.Build.WebApi.BuildController GetQueue(int controllerId)
    {
      TeamFoundationBuildResourceService service = this.TfsRequestContext.GetService<TeamFoundationBuildResourceService>();
      ArtifactId artifactId = new ArtifactId("Build", "Controller", controllerId.ToString());
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      return this.ConvertControllerToContract(service.QueryBuildControllersByUri(tfsRequestContext, (IList<string>) new List<string>()
      {
        LinkingUtilities.EncodeUri(artifactId)
      }, (IList<string>) new List<string>() { "*" }, false).Controllers.First<Microsoft.TeamFoundation.Build.Server.BuildController>() ?? throw new Microsoft.TeamFoundation.Build.WebApi.BuildControllerNotFoundException(ResourceStrings.BuildControllerNotFound((object) controllerId)));
    }

    private Microsoft.TeamFoundation.Build.WebApi.BuildController ConvertControllerToContract(
      Microsoft.TeamFoundation.Build.Server.BuildController controller)
    {
      Microsoft.TeamFoundation.Build.WebApi.BuildController contract = new Microsoft.TeamFoundation.Build.WebApi.BuildController();
      contract.CreatedDate = controller.DateCreated;
      contract.UpdatedDate = controller.DateUpdated;
      contract.Description = controller.Description;
      contract.Enabled = controller.Enabled;
      contract.Name = controller.Name;
      contract.Status = (Microsoft.TeamFoundation.Build.WebApi.ControllerStatus) controller.Status;
      contract.Uri = controller.Uri;
      contract.Id = int.Parse(LinkingUtilities.DecodeUri(controller.Uri).ToolSpecificId);
      contract.Url = this.Url.RestLink(this.TfsRequestContext, BuildResourceIds.Queues, (object) new
      {
        controllerId = contract.Id
      });
      return contract;
    }
  }
}

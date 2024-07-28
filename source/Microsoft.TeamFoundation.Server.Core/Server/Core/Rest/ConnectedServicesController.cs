// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.Rest.ConnectedServicesController
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Server.Core.Rest
{
  [ControllerApiVersion(1.0)]
  [ClientTemporarySwaggerExclusion]
  [VersionedApiControllerCustomName(Area = "core", ResourceName = "connectedServices")]
  public class ConnectedServicesController : TfsApiController
  {
    private static readonly Dictionary<Type, HttpStatusCode> s_httpExceptions = new Dictionary<Type, HttpStatusCode>();
    private const string ServiceManagementUrlAzure = "https://management.core.windows.net";

    static ConnectedServicesController()
    {
      ConnectedServicesController.s_httpExceptions.Add(typeof (ArgumentNullException), HttpStatusCode.BadRequest);
      ConnectedServicesController.s_httpExceptions.Add(typeof (UriFormatException), HttpStatusCode.BadRequest);
      ConnectedServicesController.s_httpExceptions.Add(typeof (HttpRequestException), HttpStatusCode.BadRequest);
    }

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) ConnectedServicesController.s_httpExceptions;

    public override string TraceArea => "ConnectedServicesService";

    public override string ActivityLogArea => "Framework";

    [HttpGet]
    public IEnumerable<WebApiConnectedService> GetConnectedServices(
      string projectId,
      [ClientParameterType(typeof (Microsoft.TeamFoundation.Core.WebApi.ConnectedServiceKind), false)] Microsoft.TeamFoundation.Server.Core.ConnectedServiceKind kind = Microsoft.TeamFoundation.Server.Core.ConnectedServiceKind.Custom)
    {
      TeamFoundationConnectedServicesService service = this.TfsRequestContext.GetService<TeamFoundationConnectedServicesService>();
      return kind == Microsoft.TeamFoundation.Server.Core.ConnectedServiceKind.Custom ? service.QueryConnectedServices(this.TfsRequestContext, projectId).Select<ConnectedServiceMetadata, WebApiConnectedService>(new Func<ConnectedServiceMetadata, WebApiConnectedService>(this.ConvertToWebApiConnectedService)) : service.QueryConnectedServices(this.TfsRequestContext, projectId).Where<ConnectedServiceMetadata>((Func<ConnectedServiceMetadata, bool>) (connectedService => connectedService.Kind.Equals((object) kind))).Select<ConnectedServiceMetadata, WebApiConnectedService>(new Func<ConnectedServiceMetadata, WebApiConnectedService>(this.ConvertToWebApiConnectedService));
    }

    [HttpGet]
    public WebApiConnectedServiceDetails GetConnectedServiceDetails(string projectId, string name)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      return this.ConvertToWebApiConnectedServiceDetails(this.TfsRequestContext.GetService<TeamFoundationConnectedServicesService>().GetConnectedService(this.TfsRequestContext, name, projectId));
    }

    [HttpPost]
    public WebApiConnectedService CreateConnectedService(
      string projectId,
      WebApiConnectedServiceDetails connectedServiceCreationData)
    {
      ArgumentUtility.CheckForNull<WebApiConnectedServiceDetails>(connectedServiceCreationData, nameof (connectedServiceCreationData));
      ArgumentUtility.CheckStringForNullOrEmpty(connectedServiceCreationData.ConnectedServiceMetaData.Name, "connectedServiceCreationData.ServiceMetadata.Name");
      ArgumentUtility.CheckForNull<WebApiConnectedService>(connectedServiceCreationData.ConnectedServiceMetaData, "connectedServiceCreationData.ConnectedServiceMetaData");
      ArgumentUtility.CheckStringForNullOrEmpty(connectedServiceCreationData.CredentialsXml, "connectedServiceCreationData.CredentialsXml");
      TeamFoundationConnectedServicesService service = this.TfsRequestContext.GetService<TeamFoundationConnectedServicesService>();
      Microsoft.TeamFoundation.Server.Core.ConnectedServiceKind kind = (Microsoft.TeamFoundation.Server.Core.ConnectedServiceKind) System.Enum.Parse(typeof (Microsoft.TeamFoundation.Server.Core.ConnectedServiceKind), connectedServiceCreationData.ConnectedServiceMetaData.Kind);
      WebApiConnectedService connectedServiceMetaData = connectedServiceCreationData.ConnectedServiceMetaData;
      string str = connectedServiceCreationData.EndPoint;
      if (str == null && kind.Equals((object) Microsoft.TeamFoundation.Server.Core.ConnectedServiceKind.AzureSubscription))
        str = "https://management.core.windows.net";
      ConnectedServiceCreationData connectedServiceCreationData1 = new ConnectedServiceCreationData(connectedServiceMetaData.Name, projectId, kind, connectedServiceMetaData.FriendlyName, connectedServiceMetaData.Description, str, str, connectedServiceCreationData.CredentialsXml);
      if (service.DoesConnectedServiceExist(this.TfsRequestContext, connectedServiceMetaData.Name, projectId))
        service.DeleteConnectedService(this.TfsRequestContext, connectedServiceMetaData.Name, projectId);
      if (this.IsFriendlyNameInUse(connectedServiceCreationData1.ServiceMetadata.FriendlyName, projectId))
        throw new TeamFoundationServiceException(Resources.ServiceFriendlyNameAlreadyInUse());
      return this.ConvertToWebApiConnectedService(service.CreateConnectedService(this.TfsRequestContext, connectedServiceCreationData1));
    }

    private WebApiConnectedServiceDetails ConvertToWebApiConnectedServiceDetails(
      ConnectedService connectedService)
    {
      if (connectedService == null)
        return (WebApiConnectedServiceDetails) null;
      return new WebApiConnectedServiceDetails()
      {
        ConnectedServiceMetaData = this.ConvertToWebApiConnectedService(connectedService.ServiceMetadata),
        EndPoint = connectedService.GetEndpoint(this.TfsRequestContext),
        CredentialsXml = connectedService.GetCredentialsXml(this.TfsRequestContext)
      };
    }

    private WebApiConnectedService ConvertToWebApiConnectedService(
      ConnectedServiceMetadata connectedService)
    {
      if (connectedService == null)
        return (WebApiConnectedService) null;
      WebApiConnectedService connectedService1 = new WebApiConnectedService();
      connectedService1.Id = connectedService.Name;
      connectedService1.Name = connectedService.Name;
      connectedService1.FriendlyName = connectedService.FriendlyName;
      connectedService1.Kind = connectedService.Kind.ToString();
      TeamProjectReference projectReference = TeamFoundationConnectedServicesService.GetProjectInfo(this.TfsRequestContext, connectedService.TeamProject, true).ToTeamProjectReference(this.TfsRequestContext);
      connectedService1.Project = projectReference;
      connectedService1.Description = connectedService.Description;
      connectedService1.ServiceUri = connectedService.ServiceUri;
      TeamFoundationIdentity[] foundationIdentityArray = this.TfsRequestContext.GetService<TeamFoundationIdentityService>().ReadIdentities(this.TfsRequestContext, new Guid[1]
      {
        connectedService.AuthenticatedBy
      }, MembershipQuery.None, ReadIdentityOptions.None, (IEnumerable<string>) null);
      if (foundationIdentityArray.Length == 1 && foundationIdentityArray[0] != null)
        connectedService1.AuthenticatedBy = foundationIdentityArray[0].ToIdentityRef(this.TfsRequestContext);
      connectedService1.Url = this.Url.RestLink(this.TfsRequestContext, CoreConstants.ConnectedServicesId, (object) new
      {
        projectId = projectReference.Id,
        name = connectedService.Name
      });
      return connectedService1;
    }

    private bool IsFriendlyNameInUse(string friendlyName, string projectId) => !string.IsNullOrEmpty(friendlyName) && this.TfsRequestContext.GetService<TeamFoundationConnectedServicesService>().QueryConnectedServices(this.TfsRequestContext, projectId).Select<ConnectedServiceMetadata, WebApiConnectedService>(new Func<ConnectedServiceMetadata, WebApiConnectedService>(this.ConvertToWebApiConnectedService)).Any<WebApiConnectedService>((Func<WebApiConnectedService, bool>) (connectedService => connectedService.FriendlyName.Equals(friendlyName.Trim(), StringComparison.CurrentCultureIgnoreCase)));
  }
}

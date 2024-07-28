// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Rest.AzureDeploymentDefinitionsController
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Integration.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Security.Cryptography.X509Certificates;
using System.Web.Http;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.Build.Server.Rest
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "Build", ResourceName = "DeploymentDefinitions")]
  public class AzureDeploymentDefinitionsController : BuildApiController
  {
    private static Dictionary<Type, HttpStatusCode> s_httpExceptions = new Dictionary<Type, HttpStatusCode>();

    static AzureDeploymentDefinitionsController()
    {
      AzureDeploymentDefinitionsController.s_httpExceptions.Add(typeof (ArgumentNullException), HttpStatusCode.BadRequest);
      AzureDeploymentDefinitionsController.s_httpExceptions.Add(typeof (UriFormatException), HttpStatusCode.BadRequest);
      AzureDeploymentDefinitionsController.s_httpExceptions.Add(typeof (HttpRequestException), HttpStatusCode.BadRequest);
    }

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) AzureDeploymentDefinitionsController.s_httpExceptions;

    [HttpPost]
    public ContinuousDeploymentDefinition CreateContinuousDeploymentDefinition(
      ContinuousDeploymentDefinition continuousDeploymentDefinition)
    {
      ArgumentUtility.CheckForNull<ContinuousDeploymentDefinition>(continuousDeploymentDefinition, nameof (continuousDeploymentDefinition));
      ArgumentUtility.CheckStringForNullOrEmpty(continuousDeploymentDefinition.SubscriptionId, "continuousDeploymentDefinition.SubscriptionId");
      ArgumentUtility.CheckForNull<TeamProjectReference>(continuousDeploymentDefinition.Project, "continuousDeploymentDefinition.Project");
      if (!string.IsNullOrEmpty(continuousDeploymentDefinition.HostedServiceName) && !string.IsNullOrEmpty(continuousDeploymentDefinition.Website))
        throw new ArgumentException(ResourceStrings.OnlyHostedServiceOrWebsiteAllowed());
      if (!Guid.TryParse(continuousDeploymentDefinition.SubscriptionId, out Guid _))
        throw new ArgumentException(ResourceStrings.SubscriptionIdNotAGuid((object) continuousDeploymentDefinition.SubscriptionId));
      string str = this.TfsRequestContext.GetService<CachedRegistryService>().GetValue<string>(this.TfsRequestContext, (RegistryQuery) "/Configuration/Azure/ServiceManagementUrl", true, string.Empty);
      Uri uri = new Uri(str);
      X509Certificate2 certificate = CertHelper.CreateCertificate(CertHelper.CreateContinuousDeploymentDistinguishedName(), CertHelper.CreateContinuousDeploymentCertificateFriendlyName());
      this.UploadCertToAzure(certificate, str, continuousDeploymentDefinition.SubscriptionId);
      string profile = this.CreateProfile(str, continuousDeploymentDefinition.SubscriptionId, certificate);
      CommonStructureService service1 = this.TfsRequestContext.GetService<CommonStructureService>();
      CommonStructureProjectInfo structureProjectInfo;
      if (continuousDeploymentDefinition.Project.Id != Guid.Empty)
      {
        string projectUri = LinkingUtilities.EncodeUri(new ArtifactId("Classification", "TeamProject", continuousDeploymentDefinition.Project.Id.ToString("D")));
        structureProjectInfo = service1.GetProject(this.TfsRequestContext, projectUri);
      }
      else
        structureProjectInfo = service1.GetProjectFromName(this.TfsRequestContext, continuousDeploymentDefinition.Project.Name);
      continuousDeploymentDefinition.Project = structureProjectInfo.ToProjectInfo().ToTeamProjectReference(this.TfsRequestContext);
      this.TfsRequestContext.GetService<IServiceEndpointService2>();
      TeamFoundationDeploymentService service2 = this.TfsRequestContext.GetService<TeamFoundationDeploymentService>();
      ConnectedServiceCreationData serviceData = new ConnectedServiceCreationData(continuousDeploymentDefinition.SubscriptionId, structureProjectInfo.Name, Microsoft.TeamFoundation.Server.Core.ConnectedServiceKind.AzureSubscription, ResourceStrings.AzureConnectedServiceFriendlyName((object) continuousDeploymentDefinition.SubscriptionId), ResourceStrings.AzureConnectedServiceDescription(), str, str, profile);
      service2.CreateServiceEndpoint(this.TfsRequestContext, serviceData, continuousDeploymentDefinition.Project.Id);
      continuousDeploymentDefinition.ConnectedService = new WebApiConnectedServiceRef();
      continuousDeploymentDefinition.ConnectedService.Id = continuousDeploymentDefinition.SubscriptionId;
      continuousDeploymentDefinition.ConnectedService.Url = this.Url.RestLink(this.TfsRequestContext, CoreConstants.ConnectedServicesId, (object) new
      {
        projectId = continuousDeploymentDefinition.Project.Id,
        name = continuousDeploymentDefinition.SubscriptionId
      });
      ShallowReference shallowReference;
      if (!string.IsNullOrEmpty(continuousDeploymentDefinition.HostedServiceName))
      {
        shallowReference = service2.ConnectAzureCloudAppWithNewBuildDefinition(this.TfsRequestContext, structureProjectInfo.Name, continuousDeploymentDefinition.SubscriptionId, continuousDeploymentDefinition.HostedServiceName, continuousDeploymentDefinition.StorageAccountName, continuousDeploymentDefinition.HostedServiceName);
        continuousDeploymentDefinition.Website = (string) null;
        continuousDeploymentDefinition.Webspace = (string) null;
      }
      else
      {
        shallowReference = service2.ConnectAzureWebsiteWithNewBuildDefinition(this.TfsRequestContext, structureProjectInfo.Name, continuousDeploymentDefinition.SubscriptionId, continuousDeploymentDefinition.Webspace, continuousDeploymentDefinition.Website, continuousDeploymentDefinition.RepositoryId, continuousDeploymentDefinition.GitBranch);
        continuousDeploymentDefinition.StorageAccountName = (string) null;
      }
      if (shallowReference != null)
      {
        shallowReference.Url = this.Url.RestLink(this.TfsRequestContext, BuildResourceIds.Definitions, (object) new
        {
          definitionId = shallowReference.Id
        });
        continuousDeploymentDefinition.Definition = shallowReference;
      }
      return continuousDeploymentDefinition;
    }

    [HttpDelete]
    public HttpResponseMessage DeleteContinuousDeploymentDefinition(
      ContinuousDeploymentDefinition continuousDeploymentDefinition)
    {
      ArgumentUtility.CheckForNull<ContinuousDeploymentDefinition>(continuousDeploymentDefinition, nameof (continuousDeploymentDefinition));
      ArgumentUtility.CheckStringForNullOrEmpty(continuousDeploymentDefinition.SubscriptionId, "continuousDeploymentDefinition.SubscriptionId");
      ArgumentUtility.CheckForNull<TeamProjectReference>(continuousDeploymentDefinition.Project, "continuousDeploymentDefinition.Project");
      if (!string.IsNullOrEmpty(continuousDeploymentDefinition.HostedServiceName) && !string.IsNullOrEmpty(continuousDeploymentDefinition.Website))
        throw new ArgumentException(ResourceStrings.OnlyHostedServiceOrWebsiteAllowed());
      TeamFoundationDeploymentService service = this.TfsRequestContext.GetService<TeamFoundationDeploymentService>();
      try
      {
        if (!string.IsNullOrEmpty(continuousDeploymentDefinition.HostedServiceName))
          service.DisconnectAzureCloudAppNewBuildDefinition(this.TfsRequestContext, continuousDeploymentDefinition.Project.Name, continuousDeploymentDefinition.HostedServiceName);
        else
          service.DisconnectAzureWebsiteNewBuildDefinition(this.TfsRequestContext, continuousDeploymentDefinition.Project.Name, continuousDeploymentDefinition.Website);
        return this.Request.CreateResponse(HttpStatusCode.NoContent);
      }
      catch (Microsoft.TeamFoundation.Build.Server.AccessDeniedException ex)
      {
        throw new Microsoft.TeamFoundation.Build.WebApi.AccessDeniedException(ex.Message);
      }
    }

    private string CreateProfile(
      string managementUrl,
      string subscriptionId,
      X509Certificate2 managementCertificate)
    {
      return new XDocument(new object[1]
      {
        (object) new XElement((XName) "PublishData", (object) new XElement((XName) "PublishProfile", new object[3]
        {
          (object) new XAttribute((XName) "SchemaVersion", (object) "2.0"),
          (object) new XAttribute((XName) "PublishMethod", (object) "AzureServiceManagementAPI"),
          (object) new XElement((XName) "Subscription", new object[4]
          {
            (object) new XAttribute((XName) "ServiceManagementUrl", (object) managementUrl),
            (object) new XAttribute((XName) "Id", (object) subscriptionId),
            (object) new XAttribute((XName) "Name", (object) "UNKOWN"),
            (object) new XAttribute((XName) "ManagementCertificate", (object) Convert.ToBase64String(managementCertificate.Export(X509ContentType.Pfx)))
          })
        }))
      }).ToString();
    }

    private void UploadCertToAzure(
      X509Certificate2 managementCertificate,
      string serviceManagementUrl,
      string subscriptionId)
    {
      SubscriptionCertificate subscriptionCertificate = new SubscriptionCertificate()
      {
        SubscriptionCertificateData = Convert.ToBase64String(managementCertificate.Export(X509ContentType.Cert)),
        SubscriptionCertificatePublicKey = Convert.ToBase64String(managementCertificate.GetPublicKey()),
        SubscriptionCertificateThumbprint = managementCertificate.Thumbprint
      };
      HttpClient httpClient = new HttpClient();
      httpClient.BaseAddress = new Uri(serviceManagementUrl);
      HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, string.Format("{0}/certificates", (object) subscriptionId));
      request.Headers.Authorization = this.Request.Headers.Authorization;
      XmlMediaTypeFormatter formatter = new XmlMediaTypeFormatter()
      {
        UseXmlSerializer = false
      };
      request.Headers.Add("x-ms-version", "2013-08-01");
      request.Content = (HttpContent) new ObjectContent(subscriptionCertificate.GetType(), (object) subscriptionCertificate, (MediaTypeFormatter) formatter);
      httpClient.SendAsync(request).Result.EnsureSuccessStatusCode();
    }

    private class PublishProfileConstants
    {
      internal const string PublishData = "PublishData";
      internal const string PublishMethod = "PublishMethod";
      internal const string PublishProfile = "PublishProfile";
      internal const string ServiceManagementUrl = "ServiceManagementUrl";
      internal const string Id = "Id";
      internal const string Name = "Name";
      internal const string ManagementCert = "ManagementCertificate";
      internal const string Subscription = "Subscription";
      internal const string SchemaVersion = "SchemaVersion";
      internal const string Version2 = "2.0";
      internal const string AzureServiceManagementApi = "AzureServiceManagementAPI";
    }
  }
}

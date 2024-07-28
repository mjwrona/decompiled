// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.AppDescriptorController
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  [ControllerApiVersion(5.1)]
  [VersionedApiControllerCustomName(Area = "pipelines", ResourceName = "appdescriptor")]
  public class AppDescriptorController : TfsApiController
  {
    [HttpGet]
    [ClientIgnore]
    public HttpResponseMessage QueryAppDescriptor([FromUri] string appType)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(appType, nameof (appType));
      if (!string.Equals(appType, "jira", StringComparison.OrdinalIgnoreCase))
        throw new ArgumentException(nameof (appType));
      string s = "";
      Contribution contribution = this.TfsRequestContext.GetService<IContributionService>().QueryContribution(this.TfsRequestContext, "ms.vss-jira-pipelines-app-descriptor.jira-app-descriptor");
      if (contribution == null)
        throw new AppDescriptorNotFoundException(PipelinesResources.JiraAppDescriptorNotFound());
      if (contribution.Properties != null)
      {
        JToken jtoken;
        contribution.Properties.TryGetValue("descriptor", StringComparison.OrdinalIgnoreCase, out jtoken);
        if (jtoken != null)
        {
          if (jtoken[(object) "baseUrl"] != null)
          {
            string vsassetsAccessMapping = PipelineConstants.VsassetsAccessMapping;
            AccessMapping accessMapping = this.TfsRequestContext.GetService<ILocationService>().GetAccessMapping(this.TfsRequestContext, vsassetsAccessMapping);
            if (accessMapping != null)
              jtoken[(object) "baseUrl"] = (JToken) accessMapping.AccessPoint;
          }
          s = jtoken.ToString(Formatting.None);
        }
      }
      if (string.IsNullOrEmpty(s))
        throw new AppDescriptorEmptyException(PipelinesResources.JiraAppDescriptorShouldNotBeEmpty());
      HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
      response.Content = (HttpContent) new StreamContent((Stream) new MemoryStream(Encoding.UTF8.GetBytes(s)));
      response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
      return response;
    }

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<ArgumentException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<AppDescriptorNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<AppDescriptorEmptyException>(HttpStatusCode.NotFound);
    }
  }
}

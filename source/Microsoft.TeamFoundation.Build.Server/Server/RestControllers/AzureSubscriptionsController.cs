// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.RestControllers.AzureSubscriptionsController
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build.Server.RestControllers
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "Build", ResourceName = "AzureSubscriptions", ResourceVersion = 1)]
  public sealed class AzureSubscriptionsController : TfsProjectApiController
  {
    private static Dictionary<Type, HttpStatusCode> s_httpExceptions = new Dictionary<Type, HttpStatusCode>();

    static AzureSubscriptionsController() => AzureSubscriptionsController.s_httpExceptions.Add(typeof (ConnectedServiceNotFoundException), HttpStatusCode.NotFound);

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) AzureSubscriptionsController.s_httpExceptions;

    [HttpGet]
    [ClientIgnore]
    public IEnumerable<ConnectedServiceMetadata> GetConnectedServices() => this.TfsRequestContext.GetService<TeamFoundationConnectedServicesService>().QueryConnectedServices(this.TfsRequestContext, this.ProjectInfo.Name).Where<ConnectedServiceMetadata>((Func<ConnectedServiceMetadata, bool>) (connectedService => connectedService.Kind.Equals((object) ConnectedServiceKind.AzureSubscription)));

    [HttpPut]
    [ClientIgnore]
    public ConnectedServiceMetadata UpdateSubscription(
      ConnectedServiceMetadata connectedServiceMetadata)
    {
      ArgumentUtility.CheckForNull<ConnectedServiceMetadata>(connectedServiceMetadata, nameof (connectedServiceMetadata));
      ArgumentUtility.CheckStringForNullOrEmpty(connectedServiceMetadata.TeamProject, "connectedServiceMetadata.TeamProject");
      ArgumentUtility.CheckStringForNullOrEmpty(connectedServiceMetadata.FriendlyName, "connectedServiceMetadata.FriendlyName");
      ArgumentUtility.CheckStringForNullOrEmpty(connectedServiceMetadata.Name, "connectedServiceMetadata.Name");
      return this.TfsRequestContext.GetService<TeamFoundationDeploymentService>().UpdateSubscription(this.TfsRequestContext, connectedServiceMetadata);
    }
  }
}

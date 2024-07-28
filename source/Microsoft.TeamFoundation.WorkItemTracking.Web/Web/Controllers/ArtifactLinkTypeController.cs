// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.ArtifactLinkTypeController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.Linking;
using Microsoft.Azure.Boards.WebApi.Common.Converters.ArtifactLinkFactories;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [VersionedApiControllerCustomName(Area = "wit", ResourceName = "artifactLinkTypes", ResourceVersion = 1)]
  public class ArtifactLinkTypeController : WorkItemTrackingApiController
  {
    private static IEnumerable<WorkArtifactLink> s_artifactTypesFromNewService;
    private const int TraceRange = 5990000;

    [HttpGet]
    [TraceFilter(5990000, 5990010)]
    [ClientResponseType(typeof (IEnumerable<WorkArtifactLink>), null, null)]
    [ClientExample("GET_wit_artifactlinktypes.json", null, null, null)]
    public HttpResponseMessage GetWorkArtifactLinkTypes()
    {
      if (ArtifactLinkTypeController.s_artifactTypesFromNewService == null)
        ArtifactLinkTypeController.s_artifactTypesFromNewService = (IEnumerable<WorkArtifactLink>) this.TfsRequestContext.GetService<IArtifactLinkTypesService>().GetArtifactLinkTypes(this.TfsRequestContext, "WorkItemTracking").Where<RegistrationArtifactType>((Func<RegistrationArtifactType, bool>) (artifactType => VssStringComparer.ArtifactType.Equals(artifactType.Name, "WorkItem"))).SelectMany<RegistrationArtifactType, OutboundLinkType>((Func<RegistrationArtifactType, IEnumerable<OutboundLinkType>>) (type => (IEnumerable<OutboundLinkType>) type.OutboundLinkTypes)).Select<OutboundLinkType, WorkArtifactLink>((Func<OutboundLinkType, WorkArtifactLink>) (outboundLink => WorkArtifactLinkFactory.Create(outboundLink))).ToList<WorkArtifactLink>();
      return this.Request.CreateResponse<IEnumerable<WorkArtifactLink>>(HttpStatusCode.OK, ArtifactLinkTypeController.s_artifactTypesFromNewService);
    }

    public override string TraceArea => "artifactLinkTypes";
  }
}

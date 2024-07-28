// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.GraphDescriptorsController
// Assembly: Microsoft.VisualStudio.Services.Graph, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00390AA0-D8BB-45EB-AEF5-70DC8BFC765D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Graph.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph.Client;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Graph
{
  [ControllerApiVersion(3.1)]
  [VersionedApiControllerCustomName(Area = "Graph", ResourceName = "Descriptors")]
  public class GraphDescriptorsController : GraphControllerBase
  {
    private static readonly Dictionary<Type, HttpStatusCode> s_httpExceptions = new Dictionary<Type, HttpStatusCode>()
    {
      {
        typeof (SubjectDescriptorNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (InvalidGetDescriptorRequestException),
        HttpStatusCode.BadRequest
      }
    };

    [HttpGet]
    [TraceFilter(6307300, 6307309)]
    [ClientExample("GetDescriptorById.json", null, null, null)]
    public GraphDescriptorResult GetDescriptor(Guid storageKey)
    {
      this.TfsRequestContext.CheckPermissionToReadPublicIdentityInfo();
      SubjectDescriptor subjectDescriptor = !storageKey.Equals(Guid.Empty) ? this.TfsRequestContext.GetService<IGraphIdentifierConversionService>().GetDescriptorByStorageKey(this.TfsRequestContext, storageKey) : throw new InvalidGetDescriptorRequestException(storageKey);
      if (subjectDescriptor == new SubjectDescriptor())
        throw new SubjectDescriptorNotFoundException(storageKey);
      return GraphResultExtensions.GetGraphDescriptorResult(this.TfsRequestContext, storageKey, subjectDescriptor) ?? throw new SubjectDescriptorNotFoundException(storageKey);
    }

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) GraphDescriptorsController.s_httpExceptions;
  }
}

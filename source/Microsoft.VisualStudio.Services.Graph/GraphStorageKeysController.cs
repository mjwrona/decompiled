// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.GraphStorageKeysController
// Assembly: Microsoft.VisualStudio.Services.Graph, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00390AA0-D8BB-45EB-AEF5-70DC8BFC765D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Graph.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph.Client;
using System;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Graph
{
  [ControllerApiVersion(3.2)]
  [VersionedApiControllerCustomName(Area = "Graph", ResourceName = "StorageKeys")]
  public class GraphStorageKeysController : GraphControllerBase
  {
    private const string TraceLayer = "GraphStorageKeysController";

    [HttpGet]
    [TraceFilter(10006087, 10006089)]
    [ClientExample("GetStorageKeyBySubjectDescriptor.json", null, null, null)]
    public GraphStorageKeyResult GetStorageKey([ClientParameterType(typeof (string), false)] SubjectDescriptor subjectDescriptor)
    {
      Guid storageKeyByDescriptor = this.TfsRequestContext.GetService<IGraphIdentifierConversionService>().GetStorageKeyByDescriptor(this.TfsRequestContext, subjectDescriptor);
      if (storageKeyByDescriptor == new Guid())
        throw new StorageKeyNotFoundException(subjectDescriptor);
      return GraphResultExtensions.GetGraphStorageKeyResult(this.TfsRequestContext, subjectDescriptor, storageKeyByDescriptor) ?? throw new StorageKeyNotFoundException(subjectDescriptor);
    }

    public override string TraceArea => "Graph";

    public override string ActivityLogArea => "Graph";
  }
}

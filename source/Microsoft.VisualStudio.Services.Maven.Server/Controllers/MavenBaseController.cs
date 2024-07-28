// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Controllers.MavenBaseController
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Maven.Server.Contracts;
using Microsoft.VisualStudio.Services.Maven.Server.Exceptions.Helpers;
using Microsoft.VisualStudio.Services.Maven.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.Maven.Server.Controllers
{
  [FeatureEnabled("Packaging.Maven.Service")]
  public abstract class MavenBaseController : PackagingApiController
  {
    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) MavenExceptionMappings.HttpExceptionMapping;

    public override string ActivityLogArea => "maven";

    public override string TraceArea => "maven";

    protected override IProtocol GetProtocol() => (IProtocol) Protocol.Maven;

    protected override string GetClientSessionIdFrom(HttpRequestMessage requestMessage) => (string) null;

    protected IMavenPackageVersionService MavenPackageVersionService => this.TfsRequestContext.GetService<IMavenPackageVersionService>();

    protected void ValidateBatchOperationType(
      MavenPackagesBatchRequest batchRequest,
      params IBatchOperationType[] acceptableOperationTypes)
    {
      ArgumentUtility.CheckForNull<MavenPackagesBatchRequest>(batchRequest, nameof (batchRequest));
      ArgumentUtility.CheckForNull<IBatchOperationType[]>(acceptableOperationTypes, nameof (acceptableOperationTypes));
      IBatchOperationType operationType = batchRequest.GetOperationType();
      if (!((IEnumerable<IBatchOperationType>) acceptableOperationTypes).Contains<IBatchOperationType>(operationType))
        throw new ArgumentException(Microsoft.VisualStudio.Services.Maven.Server.Resources.Error_InvalidBatchOperation((object) operationType));
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.ValidationController
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupUtility;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupValidation;
using Microsoft.VisualStudio.Services.BlobStore.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "dedup", ResourceName = "validations", ResourceVersion = 1)]
  [FeatureEnabled("BlobStore.Features.DedupStoreDebug")]
  [ClientIgnore]
  public sealed class ValidationController : BlobControllerBase
  {
    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) DedupExceptionMapping.ServerErrorMap;

    [HttpGet]
    [ControllerMethodTraceFilter(5707200)]
    public async Task<DedupValidationResult> ValidateDedupAsync(
      string dedupId,
      bool cache = true,
      bool diag = false,
      int? dp = null,
      int? pp = null,
      int? queue = null)
    {
      ValidationController validationController = this;
      ArgumentUtility.CheckForNull<string>(dedupId, nameof (dedupId));
      IDedupStore service = validationController.TfsRequestContext.GetService<IDedupStore>();
      DedupIdentifier dedupIdentifier = DedupIdentifier.Create(dedupId);
      DedupTraversalConfig dedupTraversalConfig1 = new DedupTraversalConfig();
      dedupTraversalConfig1.NoCache = !cache;
      dedupTraversalConfig1.EnableDiagnostics = diag;
      dedupTraversalConfig1.DispatchingParallelism = dp.GetValueOrDefault();
      dedupTraversalConfig1.ProcessingParallelism = pp.GetValueOrDefault();
      dedupTraversalConfig1.DispatchingAndProcessingCapacity = queue.GetValueOrDefault();
      DedupTraversalConfig dedupTraversalConfig2 = dedupTraversalConfig1;
      IVssRequestContext tfsRequestContext = validationController.TfsRequestContext;
      IDomainId defaultDomainId = WellKnownDomainIds.DefaultDomainId;
      DedupIdentifier root = dedupIdentifier;
      DedupTraversalConfig config = dedupTraversalConfig2;
      return await service.VerifyDedupAsync(tfsRequestContext, defaultDomainId, root, config);
    }

    [HttpGet]
    [ControllerMethodTraceFilter(5707210)]
    public async Task<RootsValidationResult> ValidateRootsAsync(
      string start = "",
      string end = "",
      bool scanonly = false,
      string scope = null,
      int? parallelism = 0)
    {
      ValidationController validationController = this;
      IDedupStore service = validationController.TfsRequestContext.GetService<IDedupStore>();
      RootsValidationConfig validationConfig = new RootsValidationConfig()
      {
        Scope = scope,
        ScanOnly = scanonly
      };
      DateTimeOffset? nullable1 = new DateTimeOffset?();
      DateTimeOffset? nullable2 = new DateTimeOffset?();
      DateTimeOffset? nullable3 = DedupTimeRangeUtil.FromISO8601UTC(start);
      DateTimeOffset? nullable4 = DedupTimeRangeUtil.FromISO8601UTC(end);
      if (nullable4.HasValue && nullable3.HasValue && nullable4.GetValueOrDefault() <= nullable3.GetValueOrDefault())
        throw new ArgumentException("Invalid time range: end <= start");
      validationConfig.Start = nullable3;
      validationConfig.End = nullable4;
      validationConfig.RootLevelParallelism = parallelism.GetValueOrDefault();
      IVssRequestContext tfsRequestContext = validationController.TfsRequestContext;
      IDomainId defaultDomainId = WellKnownDomainIds.DefaultDomainId;
      RootsValidationConfig config = validationConfig;
      return await service.VerifyRootsAsync(tfsRequestContext, defaultDomainId, config);
    }
  }
}

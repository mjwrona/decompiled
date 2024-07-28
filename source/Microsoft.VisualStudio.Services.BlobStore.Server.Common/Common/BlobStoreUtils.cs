// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.BlobStoreUtils
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.WebApi;
using Microsoft.VisualStudio.Services.Commerce;
using Microsoft.VisualStudio.Services.Content.Common.Tracing;
using Microsoft.VisualStudio.Services.Content.Server.Common.JobFramework;
using Microsoft.VisualStudio.Services.Organization;
using Polly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common
{
  public static class BlobStoreUtils
  {
    private const double BytesPerGibibyte = 1073741824.0;
    public static readonly string[] PrefixX1 = Enumerable.Range(0, 16).Select<int, string>((Func<int, string>) (i => i.ToString("X1"))).ToArray<string>();
    public static readonly string[] PrefixX2 = Enumerable.Range(0, 256).Select<int, string>((Func<int, string>) (i => i.ToString("X2"))).ToArray<string>();
    public static readonly string[] PrefixX3 = Enumerable.Range(0, 4096).Select<int, string>((Func<int, string>) (i => i.ToString("X3"))).ToArray<string>();
    private static readonly Lazy<HttpClient> httpClient = new Lazy<HttpClient>((Func<HttpClient>) (() => new HttpClient()
    {
      Timeout = TimeSpan.FromMinutes(30.0)
    }));

    public static string GeneratePrefix(int totalPartitions, int partitionId)
    {
      switch (totalPartitions)
      {
        case 1:
          return "";
        case 16:
          return partitionId.ToString("X1");
        case 256:
          return partitionId.ToString("X2");
        case 4096:
          return partitionId.ToString("X3");
        default:
          throw new InvalidPartitionSizeException("TotalPartitions should be one of: [1, 16, 256, 4096]");
      }
    }

    public static IEnumerable<string> GeneratePrefixRange(int totalPartitions, int partitionId)
    {
      if (totalPartitions <= 0)
        throw new InvalidPartitionSizeException("totalPartitions must be greater than 0.");
      if (partitionId >= totalPartitions)
        throw new InvalidPartitionException(string.Format("partitionId ({0}) must be less than totalPartitions ({1}).", (object) partitionId, (object) totalPartitions));
      if (partitionId < 0)
        throw new InvalidPartitionException(string.Format("partitionId ({0}) must be greater than or equal to 0.", (object) partitionId));
      if (totalPartitions != 1)
        return BlobStoreUtils.GeneratePrefixRange(totalPartitions, partitionId, BlobStoreUtils.GetPrecalculatedPrefixList(totalPartitions));
      return (IEnumerable<string>) new List<string>() { "" };
    }

    public static string[] GetPrecalculatedPrefixList(int totalPartitions)
    {
      if (totalPartitions <= 16)
        return BlobStoreUtils.PrefixX1;
      return totalPartitions <= 256 ? BlobStoreUtils.PrefixX2 : BlobStoreUtils.PrefixX3;
    }

    private static IEnumerable<string> GeneratePrefixRange(
      int totalPartitions,
      int partitionId,
      string[] precalculatedPrefixes)
    {
      if (totalPartitions > precalculatedPrefixes.Length)
        throw new InvalidPartitionSizeException(string.Format("totalPartitions must be less than or equal to {0}.", (object) precalculatedPrefixes.Length));
      int count1 = precalculatedPrefixes.Length / totalPartitions;
      int num = precalculatedPrefixes.Length % totalPartitions;
      int count2;
      if (partitionId < num)
      {
        ++count1;
        count2 = count1 * partitionId;
      }
      else
        count2 = num * (count1 + 1) + (partitionId - num) * count1;
      return ((IEnumerable<string>) precalculatedPrefixes).Skip<string>(count2).Take<string>(count1);
    }

    public static Task<bool> EvaluateQuotaCapAsync(
      IVssRequestContext requestContext,
      ResourceName resourceName,
      string scope)
    {
      return !requestContext.IsFeatureEnabled("BlobStore.Features.StorageVolumeQuotaCap") || requestContext.ExecutionEnvironment.IsOnPremisesDeployment ? Task.FromResult<bool>(false) : requestContext.GetService<IQuotaService>().ValidateQuotaAsync(requestContext, scope, resourceName);
    }

    public static bool UseHttpClientForStorageOperations(IVssRequestContext requestContext) => !requestContext.ExecutionEnvironment.IsOnPremisesDeployment;

    public static bool AllowMultiDomainOperations(
      this IVssRequestContext requestContext,
      IDomainId domainId)
    {
      return !requestContext.ExecutionEnvironment.IsOnPremisesDeployment;
    }

    public static bool AllowHostDomainAdminOperations(this IVssRequestContext requestContext) => !requestContext.ExecutionEnvironment.IsOnPremisesDeployment;

    public static bool AllowProjectDomainsForAdminOperations(this IVssRequestContext requestContext) => !requestContext.ExecutionEnvironment.IsOnPremisesDeployment && requestContext.IsFeatureEnabled("Blobstore.Features.ProjectDomains");

    public static bool IsInternalHost(IVssRequestContext requestContext)
    {
      IVssRequestContext organizationContext = requestContext.To(TeamFoundationHostType.Application);
      IOrganizationPolicyService orgPolicyService = organizationContext.GetService<IOrganizationPolicyService>();
      return ((Policy) RetrySyntax.WaitAndRetry(Policy.Handle<Exception>(), 3, (Func<int, TimeSpan>) (_ => TimeSpan.FromSeconds(2.0)))).Execute<Policy<bool>>((Func<Policy<bool>>) (() => orgPolicyService.GetPolicy<bool>(organizationContext.Elevate(), "Policy.IsInternal", false))).EffectiveValue;
    }

    public static double ToBillableGiB(double totalBytes) => totalBytes / 1073741824.0;

    public static async Task<Stream> GetStreamAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      DedupIdentifier dedupId)
    {
      DedupDownloadInfo dedupDownloadInfo = await requestContext.GetService<IDedupStore>().GetDownloadInfoAsync(requestContext, domainId, dedupId, true) ?? throw new DedupNotFoundException(string.Format("The requested manifest ID {0} does not exist.", (object) dedupId));
      Uri[] uriArray;
      if (dedupDownloadInfo.Chunks == null)
        uriArray = new Uri[1]{ dedupDownloadInfo.Url };
      else
        uriArray = ((IEnumerable<ChunkDedupDownloadInfo>) dedupDownloadInfo.Chunks).Select<ChunkDedupDownloadInfo, Uri>((Func<ChunkDedupDownloadInfo, Uri>) (c => c.Url)).ToArray<Uri>();
      IEnumerable<Uri> uris = (IEnumerable<Uri>) uriArray;
      return (Stream) Microsoft.VisualStudio.Services.BlobStore.Server.BlobStitcher.Common.BlobStitcher.GetTransitiveContent(dedupDownloadInfo.Size, uris, BlobStoreUtils.httpClient.Value, (IAppTraceSource) NoopAppTraceSource.Instance, requestContext.CancellationToken);
    }

    public enum HostAccountType
    {
      Undefined,
      Internal,
      External,
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata.TerrapinMetadataValidator`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion.CentralFeedServices;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache.V2;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata
{
  public class TerrapinMetadataValidator<TPackageName, TPackageVersion> : 
    ITerrapinMetadataValidator<TPackageName, TPackageVersion>
    where TPackageName : IPackageName
    where TPackageVersion : class, IPackageVersion
  {
    private ITerrapinService terrapinService;
    private ITracerService tracerService;
    private IIdentityResolver identityResolver;
    private IRegistryService registryService;
    private ITimeProvider timeProvider;

    public TerrapinMetadataValidator(
      ITerrapinService terrapinService,
      ITracerService tracerService,
      IIdentityResolver identityResolver,
      IRegistryService registryService,
      ITimeProvider timeProvider)
    {
      this.terrapinService = terrapinService;
      this.tracerService = tracerService;
      this.identityResolver = identityResolver;
      this.registryService = registryService;
      this.timeProvider = timeProvider;
    }

    public async Task<Dictionary<TPackageVersion, TerrapinIngestionValidationReason>> GetTerrapinData(
      TPackageName packageName,
      IEnumerable<UpstreamVersionInstance<TPackageVersion>> upstreamVersions)
    {
      TerrapinMetadataValidator<TPackageName, TPackageVersion> sendInTheThisObject = this;
      Dictionary<TPackageVersion, TerrapinIngestionValidationReason> terrapinData;
      Dictionary<Task<TerrapinIngestionValidationResult>, TPackageVersion> tpinTasks;
      IEnumerator<UpstreamVersionInstance<TPackageVersion>> upstreamVersionsEnumerator;
      Dictionary<TPackageVersion, TerrapinIngestionValidationReason> terrapinData1;
      using (ITracerBlock traceBlock = sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (GetTerrapinData)))
      {
        terrapinData = new Dictionary<TPackageVersion, TerrapinIngestionValidationReason>((IEqualityComparer<TPackageVersion>) PackageVersionComparer.NormalizedVersion);
        int count = sendInTheThisObject.registryService.GetValue<int>((RegistryQuery) "/Configuration/Packaging/UpstreamMetadataCache/MaxNumVersionsPerPkgToSendToTpin", -1);
        if (count > -1)
          upstreamVersions = upstreamVersions.Take<UpstreamVersionInstance<TPackageVersion>>(count);
        int num1 = sendInTheThisObject.registryService.GetValue<int>((RegistryQuery) "/Configuration/Packaging/UpstreamMetadataCache/NumParallelRequestsToTpin", 1);
        tpinTasks = new Dictionary<Task<TerrapinIngestionValidationResult>, TPackageVersion>();
        upstreamVersionsEnumerator = upstreamVersions.GetEnumerator();
        try
        {
          DateTime timeToStop = DateTime.MaxValue;
          int num2 = sendInTheThisObject.registryService.GetValue<int>((RegistryQuery) "/Configuration/Packaging/UpstreamMetadataCache/MaxTimeToSpendOnRequestsToTpin", -1);
          if (num2 > -1)
            timeToStop = sendInTheThisObject.timeProvider.Now.AddSeconds((double) num2);
          while (tpinTasks.Count < num1 && upstreamVersionsEnumerator.MoveNext())
          {
            UpstreamVersionInstance<TPackageVersion> current = upstreamVersionsEnumerator.Current;
            tpinTasks[sendInTheThisObject.GetTerrapinValidationResult(packageName, current)] = current.Version;
          }
          while (tpinTasks.Count<KeyValuePair<Task<TerrapinIngestionValidationResult>, TPackageVersion>>() > 0)
          {
            Task<TerrapinIngestionValidationResult> key = await Task.WhenAny<TerrapinIngestionValidationResult>((IEnumerable<Task<TerrapinIngestionValidationResult>>) tpinTasks.Keys);
            TPackageVersion completedVersion = tpinTasks[key];
            tpinTasks.Remove(key);
            if (upstreamVersionsEnumerator.MoveNext() && sendInTheThisObject.timeProvider.Now < timeToStop)
            {
              UpstreamVersionInstance<TPackageVersion> current = upstreamVersionsEnumerator.Current;
              tpinTasks[sendInTheThisObject.GetTerrapinValidationResult(packageName, current)] = current.Version;
            }
            TerrapinIngestionValidationResult validationResult = await key;
            TerrapinIngestionValidationReason validationReason1;
            if (validationResult.OverallResult == TerrapinIngestionValidationOverallResult.Denied)
            {
              if (validationResult.Reasons.Any<TerrapinIngestionValidationReason>((Func<TerrapinIngestionValidationReason, bool>) (r => TerrapinConstants.BlockedCodes.Contains(r.Code))))
              {
                TerrapinIngestionValidationReason validationReason2 = validationResult.Reasons.Find((Predicate<TerrapinIngestionValidationReason>) (r => TerrapinConstants.BlockedCodes.Contains(r.Code)));
                validationReason1 = new TerrapinIngestionValidationReason(validationReason2.Code, validationReason2.Message);
              }
              else if (validationResult.Reasons.Any<TerrapinIngestionValidationReason>((Func<TerrapinIngestionValidationReason, bool>) (r => r.Code == "Quarantine")))
              {
                TerrapinIngestionValidationReason validationReason3 = validationResult.Reasons.Find((Predicate<TerrapinIngestionValidationReason>) (r => r.Code == "Quarantine"));
                validationReason1 = (TerrapinIngestionValidationReason) new TerrapinIngestionValidationReasonQuarantine(validationReason3.Message, ((TerrapinIngestionValidationReasonQuarantine) validationReason3).QuarantinedUntilUtc);
              }
              else if (validationResult.Reasons.Any<TerrapinIngestionValidationReason>((Func<TerrapinIngestionValidationReason, bool>) (r => r.Code == "QuarantinedPendingAssessment")))
              {
                validationReason1 = (TerrapinIngestionValidationReason) new TerrapinIngestionValidationReasonQuarantinePending(validationResult.Reasons.Find((Predicate<TerrapinIngestionValidationReason>) (r => r.Code == "QuarantinedPendingAssessment")).Message);
              }
              else
              {
                TerrapinIngestionValidationReason validationReason4 = validationResult.Reasons.First<TerrapinIngestionValidationReason>();
                validationReason1 = new TerrapinIngestionValidationReason(validationReason4.Code, validationReason4.Message);
                traceBlock.TraceError("Received new code from terrapin '" + validationReason4.Code + "' with message '" + validationReason4.Message + "'");
              }
            }
            else
              validationReason1 = (TerrapinIngestionValidationReason) null;
            terrapinData.Add(completedVersion, validationReason1);
            completedVersion = default (TPackageVersion);
          }
          terrapinData1 = terrapinData;
        }
        finally
        {
          upstreamVersionsEnumerator?.Dispose();
        }
      }
      terrapinData = (Dictionary<TPackageVersion, TerrapinIngestionValidationReason>) null;
      tpinTasks = (Dictionary<Task<TerrapinIngestionValidationResult>, TPackageVersion>) null;
      upstreamVersionsEnumerator = (IEnumerator<UpstreamVersionInstance<TPackageVersion>>) null;
      return terrapinData1;
    }

    private async Task<TerrapinIngestionValidationResult> GetTerrapinValidationResult(
      TPackageName packageName,
      UpstreamVersionInstance<TPackageVersion> version)
    {
      IEnumerable<UpstreamSourceInfo> sourceChain = version.ImmediateSource != null ? version.SourceChain.Prepend<UpstreamSourceInfo>(version.ImmediateSource.ToUpstreamSourceInfo()) : (IEnumerable<UpstreamSourceInfo>) version.SourceChain;
      return await this.terrapinService.GetValidationStatusAsync(this.identityResolver.GetPackageUrlIdentity(this.identityResolver.FusePackageIdentity((IPackageName) packageName, (IPackageVersion) version.Version), (IPackageFileName) null), sourceChain, false);
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion.CentralFeedServices.TerrapinIngestionValidator
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion.CentralFeedServices
{
  public class TerrapinIngestionValidator : ITerrapinIngestionValidator
  {
    private readonly ITerrapinService terrapinService;
    private readonly IProblemPackagesRecorder? problemPackagesRecorder;
    private readonly IOrgLevelPackagingSetting<bool> shouldEnforceResultsSetting;
    private readonly IIdentityResolver identityResolver;

    public TerrapinIngestionValidator(
      ITerrapinService terrapinService,
      IProblemPackagesRecorder? problemPackagesRecorder,
      IOrgLevelPackagingSetting<bool> shouldEnforceResultsSetting,
      IIdentityResolver identityResolver)
    {
      this.terrapinService = terrapinService;
      this.problemPackagesRecorder = problemPackagesRecorder;
      this.shouldEnforceResultsSetting = shouldEnforceResultsSetting;
      this.identityResolver = identityResolver;
    }

    public async Task ValidateAsync(
      IPackageRequest request,
      IPackageFileName packageFileName,
      IEnumerable<UpstreamSourceInfo> sourceChain)
    {
      TerrapinIngestionValidationResult status = await this.terrapinService.GetValidationStatusAsync(this.identityResolver.GetPackageUrlIdentity(request.PackageId, packageFileName), sourceChain, true);
      if (status.OverallResult != TerrapinIngestionValidationOverallResult.Denied)
        status = (TerrapinIngestionValidationResult) null;
      else if (!this.shouldEnforceResultsSetting.Get())
      {
        status = (TerrapinIngestionValidationResult) null;
      }
      else
      {
        if (this.problemPackagesRecorder != null && status.Reasons.Any<TerrapinIngestionValidationReason>((Func<TerrapinIngestionValidationReason, bool>) (x => x.Code != "Quarantine")))
          await this.problemPackagesRecorder.RecordProblemPackageAsync(request, sourceChain.Last<UpstreamSourceInfo>(), status);
        throw TerrapinIngestionValidator.GetExceptionForDenial(request.PackageId, status);
      }
    }

    public static IngestionProhibitedException GetExceptionForDenial(
      IPackageIdentity packageIdentity,
      TerrapinIngestionValidationResult status)
    {
      string str = status.Reasons.IsEmpty ? "(no reasons provided)" : string.Join("; ", status.Reasons.Select<TerrapinIngestionValidationReason, string>((Func<TerrapinIngestionValidationReason, string>) (x => "[" + x.Code + "] " + x.Message)));
      return new IngestionProhibitedException("Your organization's policies prohibit the ingestion of " + packageIdentity.DisplayStringForMessages + " into your feed: " + str);
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache.V2.ShouldIncludeExternalVersionsHelper
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache.V2
{
  public class ShouldIncludeExternalVersionsHelper : IShouldIncludeExternalVersionsHelper
  {
    private readonly ITracerService tracerService;
    private readonly IFeatureFlagService featureFlagService;
    private readonly IRegistryService registryService;
    private readonly IPackageUpstreamBehaviorFacade upstreamBehaviorService;
    private readonly IExecutionEnvironment executionEnvironment;
    private readonly IOrganizationPolicies organizationPolicies;

    public ShouldIncludeExternalVersionsHelper(
      ITracerService tracerService,
      IFeatureFlagService featureFlagService,
      IRegistryService registryService,
      IPackageUpstreamBehaviorFacade upstreamBehaviorService,
      IOrganizationPolicies organizationPolicies,
      IExecutionEnvironment executionEnvironment)
    {
      this.tracerService = tracerService;
      this.featureFlagService = featureFlagService;
      this.registryService = registryService;
      this.upstreamBehaviorService = upstreamBehaviorService;
      this.executionEnvironment = executionEnvironment;
      this.organizationPolicies = organizationPolicies;
    }

    public bool ShouldIncludeExternalUpstreamVersions(
      FeedCore feed,
      IPackageName packageName,
      IReadOnlyCollection<IUpstreamVersionInstance> allVersionInstances)
    {
      using (ITracerBlock tracerBlock = this.tracerService.Enter((object) this, nameof (ShouldIncludeExternalUpstreamVersions)))
      {
        bool haveAnyLocalVersions = allVersionInstances.Any<IUpstreamVersionInstance>((Func<IUpstreamVersionInstance, bool>) (y => y.IsLocal));
        bool haveAnyLocalInternalVersions = allVersionInstances.Any<IUpstreamVersionInstance>((Func<IUpstreamVersionInstance, bool>) (y => y.IsLocal && y.Origin == PackageOrigin.Internal));
        bool haveAnyUpstreamInternalVersions = allVersionInstances.Any<IUpstreamVersionInstance>((Func<IUpstreamVersionInstance, bool>) (y => !y.IsLocal && y.Origin == PackageOrigin.Internal));
        bool haveAnyVersionsWithUnknownOrigin = allVersionInstances.Any<IUpstreamVersionInstance>((Func<IUpstreamVersionInstance, bool>) (y => y.Origin == PackageOrigin.Unknown));
        bool policyValue = this.organizationPolicies.GetPolicyValue<bool>("Policy.ArtifactsExternalPackageProtectionToken", true);
        UpstreamVersionVisibility externalUpstreams = this.upstreamBehaviorService.GetBehavior(feed, packageName).VersionsFromExternalUpstreams;
        bool overrideFlagEnabled = externalUpstreams == UpstreamVersionVisibility.AllowExternalVersions;
        bool flag = ShouldIncludeExternalVersionsHelper.ShouldIncludeExternalUpstreamVersionsCore(policyValue, overrideFlagEnabled, haveAnyVersionsWithUnknownOrigin, haveAnyLocalVersions, haveAnyLocalInternalVersions, haveAnyUpstreamInternalVersions);
        tracerBlock.TraceInfo(string.Format("Should include external versions: {0}\r\nNew upstream merge behavior enabled: {1}.\r\n    Policy enabled: {2},\r\nOverride flag enabled: {3}\r\n    Version visibility value: {4}\r\nHave any versions with unknown origin: {5}\r\nHave any local versions: {6}\r\nHave any local internal versions: {7}\r\nHave any upstream internal versions: {8}", (object) flag, (object) policyValue, (object) policyValue, (object) overrideFlagEnabled, (object) externalUpstreams, (object) haveAnyVersionsWithUnknownOrigin, (object) haveAnyLocalVersions, (object) haveAnyLocalInternalVersions, (object) haveAnyUpstreamInternalVersions));
        return flag;
      }
    }

    private static bool ShouldIncludeExternalUpstreamVersionsCore(
      bool policyEnabled,
      bool overrideFlagEnabled,
      bool haveAnyVersionsWithUnknownOrigin,
      bool haveAnyLocalVersions,
      bool haveAnyLocalInternalVersions,
      bool haveAnyUpstreamInternalVersions)
    {
      if (!policyEnabled || overrideFlagEnabled || haveAnyVersionsWithUnknownOrigin)
        return true;
      if (haveAnyLocalVersions)
      {
        if (haveAnyLocalInternalVersions)
          return false;
        int num = haveAnyUpstreamInternalVersions ? 1 : 0;
        return true;
      }
      return !haveAnyUpstreamInternalVersions;
    }
  }
}

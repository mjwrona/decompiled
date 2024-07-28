// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.UrlHostResolutionConstants
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class UrlHostResolutionConstants
  {
    public const string DefaultCollectionName = "DefaultCollection";
    internal const string DefaultCollectionVirtualPath = "/DefaultCollection/";
    private const string BaseRegPath = "/Configuration/UrlHostResolution";
    private const string VSUsesLegacyDefaultCollectionRouting = "/Configuration/UrlHostResolution/VSUsesLegacyDefaultCollectionRouting";
    public static readonly RegistryQuery VsUsesLegacyDefaultCollectionRoutingQuery = new RegistryQuery("/Configuration/UrlHostResolution/VSUsesLegacyDefaultCollectionRouting", false);
    private const string DomainEndpointMapping = "/Configuration/UrlHostResolution/DomainEndpointMapping";
    public static readonly RegistryQuery DomainEndpointMappingQuery = (RegistryQuery) "/Configuration/UrlHostResolution/DomainEndpointMapping";
    internal const string DomainEndpointMappingEntries = "/Configuration/UrlHostResolution/DomainEndpointMapping/Entries";
    public static readonly RegistryQuery DomainEndpointMappingEntriesQuery = (RegistryQuery) "/Configuration/UrlHostResolution/DomainEndpointMapping/Entries/*";
    public const string AppVsspsShardIncludedAPIsKey = "/Configuration/UrlHostResolution/AppVsspsShardedApis";
    public const string AppVsspsShardExcludedAPIsKey = "/Configuration/UrlHostResolution/AppVsspsShardExcludeApis";
    public const string AppVsspsOnlyServeARRCalls = "/Configuration/UrlHostResolution/AppVsspsOnlyServeARRCalls";
    internal const string PathRouting = "/Configuration/UrlHostResolution/PathRouting";
    internal const string PathRoutingResolvesCollectionsQuery = "/Configuration/UrlHostResolution/PathRouting/ResolvesCollections";
    internal const string PathRoutingResolvesOrganizationsQuery = "/Configuration/UrlHostResolution/PathRouting/ResolvesOrganizations";
    public const string OnPremResolverStrategy = "OnPrem";
    public const string HostedResolverStrategy = "Hosted";
    public const string ServiceDeploymentsPathSegment = "serviceDeployments";
    public const string ServiceHostsPathSegment = "serviceHosts";
    public const string OrganizationPathSegment = "e";
    public const string UseDevOpsDomainForHostCreation = "VisualStudio.Services.HostResolution.UseCodexDomainForHostCreation";
    public const string ClearCacheOnHostUpdated = "VisualStudio.Services.HostResolution.ClearCacheOnHostUpdated";
    public const string MultiInstanceAffinityCookie = "X-VSS-DeploymentAffinity";
  }
}

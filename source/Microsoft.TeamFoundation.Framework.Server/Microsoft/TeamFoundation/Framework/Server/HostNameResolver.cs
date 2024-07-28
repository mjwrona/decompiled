// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HostNameResolver
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.NameResolution;
using Microsoft.VisualStudio.Services.NameResolution.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class HostNameResolver
  {
    public static bool TryGetCollectionServiceHostId(
      IVssRequestContext deploymentContext,
      string collectionName,
      out Guid collectionId)
    {
      deploymentContext.CheckDeploymentRequestContext();
      ArgumentUtility.CheckStringForNullOrWhiteSpace(collectionName, nameof (collectionName));
      NameResolutionEntry nameResolutionEntry = HostNameResolver.ResolveCollectionServiceHost(deploymentContext, collectionName);
      if (nameResolutionEntry != null)
      {
        collectionId = nameResolutionEntry.Value;
        return true;
      }
      collectionId = new Guid();
      return false;
    }

    public static bool TryGetOrganizationServiceHostId(
      IVssRequestContext deploymentContext,
      string organizationName,
      out Guid organizationId)
    {
      deploymentContext.CheckDeploymentRequestContext();
      ArgumentUtility.CheckStringForNullOrWhiteSpace(organizationName, nameof (organizationName));
      NameResolutionEntry nameResolutionEntry = HostNameResolver.ResolveOrganizationServiceHost(deploymentContext, organizationName);
      if (nameResolutionEntry != null)
      {
        organizationId = nameResolutionEntry.Value;
        return true;
      }
      organizationId = new Guid();
      return false;
    }

    public static NameResolutionEntry ResolveCollectionServiceHost(
      IVssRequestContext deploymentContext,
      string collectionName)
    {
      deploymentContext.CheckDeploymentRequestContext();
      ArgumentUtility.CheckStringForNullOrWhiteSpace(collectionName, nameof (collectionName));
      IInternalNameResolutionService service = deploymentContext.GetService<IInternalNameResolutionService>();
      string collectionEntryName = HostNameResolver.GetCollectionEntryName(deploymentContext, collectionName);
      IVssRequestContext requestContext = deploymentContext;
      string[] namespaces = new string[2]
      {
        "Collection",
        "GlobalCollection"
      };
      string name = collectionEntryName;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      Predicate<NameResolutionEntry> filter = HostNameResolver.\u003C\u003EO.\u003C0\u003E__IsEnabled ?? (HostNameResolver.\u003C\u003EO.\u003C0\u003E__IsEnabled = new Predicate<NameResolutionEntry>(HostNameResolver.IsEnabled));
      return service.QueryFirstEntry(requestContext, (IReadOnlyList<string>) namespaces, name, filter);
    }

    public static NameResolutionEntry ResolveOrganizationServiceHost(
      IVssRequestContext deploymentContext,
      string organizationName)
    {
      deploymentContext.CheckDeploymentRequestContext();
      ArgumentUtility.CheckStringForNullOrWhiteSpace(organizationName, nameof (organizationName));
      NameResolutionEntry entry = deploymentContext.GetService<INameResolutionService>().QueryEntry(deploymentContext, "Organization", organizationName);
      return HostNameResolver.IsEnabled(entry) ? entry : (NameResolutionEntry) null;
    }

    public static NameResolutionEntry ResolveServiceHost(
      IVssRequestContext deploymentContext,
      Guid hostId)
    {
      deploymentContext.CheckDeploymentRequestContext();
      ArgumentUtility.CheckForEmptyGuid(hostId, nameof (hostId));
      NameResolutionEntry primaryEntryForValue = deploymentContext.GetService<INameResolutionService>().GetPrimaryEntryForValue(deploymentContext, hostId);
      return HostNameResolver.IsEnabled(primaryEntryForValue) ? primaryEntryForValue : (NameResolutionEntry) null;
    }

    public static Uri GetCollectionUrl(
      IVssRequestContext deploymentContext,
      string collectionName,
      Guid serviceIdentifier = default (Guid))
    {
      deploymentContext.CheckDeploymentRequestContext();
      ArgumentUtility.CheckStringForNullOrWhiteSpace(collectionName, nameof (collectionName));
      return HostNameResolver.GetCollectionUriData(deploymentContext, collectionName).BuildUri(deploymentContext, serviceIdentifier);
    }

    private static IHostUriData GetCollectionUriData(
      IVssRequestContext deploymentContext,
      string collectionName)
    {
      Guid collectionId;
      if (HostNameResolver.TryGetCollectionServiceHostId(deploymentContext, collectionName, out collectionId))
        return deploymentContext.GetService<IInternalUrlHostResolutionService>().ResolveUriData(deploymentContext, collectionId);
      return deploymentContext.UseDevOpsDomainUrls() ? (IHostUriData) new DevOpsCollectionHostUriData(collectionName) : (IHostUriData) new RootMappingHostUriData(collectionName);
    }

    internal static string GetCollectionEntryName(
      IVssRequestContext deploymentContext,
      string collectionName)
    {
      return !deploymentContext.ExecutionEnvironment.IsOnPremisesDeployment ? collectionName : "/" + collectionName + "/";
    }

    private static bool IsEnabled(NameResolutionEntry entry) => entry != null && (entry.IsEnabled || entry.IsPrimary && entry.Namespace == "Organization");
  }
}

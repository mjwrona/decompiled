// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.DirectorySearcherHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  internal class DirectorySearcherHelper
  {
    private static DirectoryEntry s_globalCatalogEntry;
    private static readonly SortOption DisplayNameSortOption = new SortOption("DisplayName", SortDirection.Ascending);

    private static DirectoryEntry GlobalCatalogEntry
    {
      get
      {
        DirectoryEntry globalCatalogEntry = DirectorySearcherHelper.s_globalCatalogEntry;
        if (globalCatalogEntry == null)
          DirectorySearcherHelper.s_globalCatalogEntry = globalCatalogEntry = DirectorySearcherHelper.CreateGlobalCatalogEntry();
        return globalCatalogEntry;
      }
      set
      {
        if (DirectorySearcherHelper.s_globalCatalogEntry == value)
          return;
        DirectoryEntry globalCatalogEntry = DirectorySearcherHelper.s_globalCatalogEntry;
        DirectorySearcherHelper.s_globalCatalogEntry = value;
        globalCatalogEntry?.Dispose();
      }
    }

    private static DirectoryEntry GetDirectoryEntryForDomain(string friendlyDomainName)
    {
      try
      {
        using (Domain domain = Domain.GetDomain(new DirectoryContext(DirectoryContextType.Domain, friendlyDomainName)))
        {
          DirectoryEntry directoryEntry = domain.GetDirectoryEntry();
          directoryEntry.AuthenticationType |= AuthenticationTypes.ReadonlyServer;
          return directoryEntry;
        }
      }
      catch (Exception ex)
      {
        throw new DirectorySearchException(FrameworkResources.UnableToResolveDomainName((object) friendlyDomainName), ex);
      }
    }

    private static DirectoryEntry CreateGlobalCatalogEntry()
    {
      using (DirectoryEntry directoryEntry = DirectoryEntryFactory.CreateDirectoryEntry("GC:"))
      {
        IEnumerator enumerator = directoryEntry.Children.GetEnumerator();
        return (enumerator.MoveNext() ? (DirectoryEntry) enumerator.Current : throw new Exception(FrameworkResources.UnableToFindGlobalCatalog())) ?? throw new Exception(FrameworkResources.UnableToFindGlobalCatalog());
      }
    }

    internal static IEnumerable<SearchResult> FindAll(
      IVssRequestContext context,
      string filter,
      int sizeLimit,
      HashSet<string> propertiesToLoad,
      string domainName = null)
    {
      string[] propertiesToLoadArray = (propertiesToLoad != null ? propertiesToLoad.ToArray<string>() : (string[]) null) ?? Array.Empty<string>();
      return string.IsNullOrWhiteSpace(domainName) ? DirectorySearcherHelper.FindAllFromGlobalCatalog(context, filter, sizeLimit, propertiesToLoadArray) : DirectorySearcherHelper.FindAllFromDomain(context, filter, sizeLimit, domainName, propertiesToLoadArray);
    }

    private static IEnumerable<SearchResult> FindAllFromGlobalCatalog(
      IVssRequestContext context,
      string filter,
      int sizeLimit,
      string[] propertiesToLoadArray)
    {
      DirectoryEntry rootEntry = (DirectoryEntry) null;
      try
      {
        rootEntry = DirectoryEntryFactory.CreateDirectoryEntry(DirectorySearcherHelper.GlobalCatalogEntry.Path);
        rootEntry.AuthenticationType |= AuthenticationTypes.ReadonlyServer;
        IEnumerable<SearchResult> all = DirectorySearcherHelper.FindAll(context, filter, sizeLimit, propertiesToLoadArray, rootEntry);
        DirectorySearcherHelper.GlobalCatalogEntry = rootEntry;
        return all;
      }
      catch (Exception ex)
      {
        context.TraceException(15005008, "DirectoryDiscovery", "ActiveDirectory", ex);
        DirectorySearcherHelper.GlobalCatalogEntry = (DirectoryEntry) null;
        rootEntry?.Dispose();
        throw;
      }
    }

    private static IEnumerable<SearchResult> FindAllFromDomain(
      IVssRequestContext context,
      string filter,
      int sizeLimit,
      string domainName,
      string[] propertiesToLoadArray)
    {
      try
      {
        using (DirectoryEntry directoryEntryForDomain = DirectorySearcherHelper.GetDirectoryEntryForDomain(domainName))
          return DirectorySearcherHelper.FindAll(context, filter, sizeLimit, propertiesToLoadArray, directoryEntryForDomain);
      }
      catch (Exception ex)
      {
        context.TraceException(15005008, "DirectoryDiscovery", "ActiveDirectory", ex);
        throw;
      }
    }

    private static IEnumerable<SearchResult> FindAll(
      IVssRequestContext context,
      string filter,
      int sizeLimit,
      string[] propertiesToLoad,
      DirectoryEntry rootEntry)
    {
      ActiveDirectorySettingsService service = context.To(TeamFoundationHostType.Deployment).GetService<ActiveDirectorySettingsService>();
      context.Trace(11335900, TraceLevel.Verbose, "DirectoryDiscovery", "ActiveDirectory", "ldap query=" + filter);
      List<SearchResult> all1 = new List<SearchResult>();
      using (DirectorySearcher directorySearcher = new DirectorySearcher(rootEntry, filter, propertiesToLoad))
      {
        directorySearcher.SearchScope = SearchScope.Subtree;
        directorySearcher.SizeLimit = Math.Min(sizeLimit, service.DirectorySearcherMaxSizeLimit);
        directorySearcher.ClientTimeout = service.DirectorySearcherClientTimeoutInSecs;
        directorySearcher.Sort = DirectorySearcherHelper.DisplayNameSortOption;
        using (SearchResultCollection all2 = directorySearcher.FindAll())
        {
          foreach (SearchResult searchResult in all2)
            all1.Add(searchResult);
        }
      }
      return (IEnumerable<SearchResult>) all1;
    }
  }
}

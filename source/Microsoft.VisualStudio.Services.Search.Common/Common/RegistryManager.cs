// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.RegistryManager
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public class RegistryManager
  {
    private const string TraceArea = "Indexing Pipeline";
    private readonly string m_traceLayer;
    private readonly string m_accountName;

    protected IVssRequestContext RequestContext { get; }

    protected Guid CollectionId { get; }

    public RegistryManager()
    {
    }

    public RegistryManager(IVssRequestContext requestContext, string traceLayer)
    {
      this.RequestContext = requestContext ?? throw new ArgumentNullException(nameof (requestContext));
      this.m_traceLayer = traceLayer;
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        return;
      this.CollectionId = requestContext.GetCollectionID();
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        this.m_accountName = requestContext.GetOrganizationName();
      else
        this.m_accountName = requestContext.GetCollectionName();
    }

    public virtual void AddOrUpdateRegistryValue(
      string featureName,
      string childPath,
      string value)
    {
      IVssRegistryService service = this.RequestContext.GetService<IVssRegistryService>();
      string registryPath = this.GetRegistryPath(featureName, childPath);
      RegistryEntry registryEntry = new RegistryEntry()
      {
        Path = registryPath,
        Value = value
      };
      IVssRequestContext requestContext = this.RequestContext;
      service.UpdateOrDeleteEntries(requestContext, (IEnumerable<RegistryEntry>) new List<RegistryEntry>()
      {
        registryEntry
      });
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1080472, "Indexing Pipeline", this.m_traceLayer, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Added registry entry with path {0} and value {1}", (object) registryPath, (object) value));
    }

    public virtual void AddOrUpdateRegistryValues(List<RegistryEntry> registryEntries)
    {
      if (registryEntries == null)
        throw new ArgumentNullException(nameof (registryEntries));
      this.RequestContext.GetService<IVssRegistryService>().UpdateOrDeleteEntries(this.RequestContext, (IEnumerable<RegistryEntry>) registryEntries);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1080472, "Indexing Pipeline", this.m_traceLayer, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Added/Updated {0} registry entries", (object) registryEntries.Count));
    }

    public virtual List<RegistryEntry> GetRegistryEntriesWithPathPattern(
      string registryEntryPathPattern)
    {
      RegistryEntryCollection registryEntryCollection = this.RequestContext.GetService<IVssRegistryService>().ReadEntries(this.RequestContext, (RegistryQuery) registryEntryPathPattern);
      List<RegistryEntry> entriesWithPathPattern = new List<RegistryEntry>();
      foreach (RegistryEntry registryEntry in registryEntryCollection)
        entriesWithPathPattern.Add(registryEntry);
      return entriesWithPathPattern;
    }

    public virtual RegistryEntry GetRegistryEntry(string featureName, string childPath) => this.GetRegistryEntriesWithPathPattern(this.GetRegistryPathPattern(featureName)).Find((Predicate<RegistryEntry>) (r => r.Path.Equals(this.GetRegistryPath(featureName, childPath), StringComparison.OrdinalIgnoreCase)));

    public virtual void RemoveRegistryEntry(RegistryEntry registryEntry)
    {
      if (registryEntry == null)
        throw new ArgumentNullException(nameof (registryEntry));
      CachedRegistryService service = this.RequestContext.GetService<CachedRegistryService>();
      registryEntry.Value = (string) null;
      IVssRequestContext requestContext = this.RequestContext;
      service.UpdateOrDeleteEntries(requestContext, (IEnumerable<RegistryEntry>) new List<RegistryEntry>()
      {
        registryEntry
      });
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1080472, "Indexing Pipeline", this.m_traceLayer, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Removed registry entry with path {0}", (object) registryEntry.Path));
    }

    public virtual bool RemoveRegistryEntry(string featureName, string childPath)
    {
      bool flag = false;
      string registryPathPattern = this.GetRegistryPathPattern(featureName);
      RegistryEntryCollection registryEntryCollection = this.RequestContext.GetService<IVssRegistryService>().ReadEntries(this.RequestContext, (RegistryQuery) registryPathPattern);
      RegistryEntry registryEntry = (RegistryEntry) null;
      string registryPath = this.GetRegistryPath(featureName, childPath);
      ref RegistryEntry local = ref registryEntry;
      registryEntryCollection.TryGetValue(registryPath, out local);
      if (registryEntry != null)
      {
        this.RemoveRegistryEntry(registryEntry);
        flag = true;
      }
      return flag;
    }

    public virtual string GetRegistryPath(string featureName, string entityPathId)
    {
      if (!this.RequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/{0}/{1}", (object) featureName, (object) entityPathId);
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/{0}/{1}/{2}/{3}", (object) featureName, (object) this.m_accountName, (object) this.CollectionId, (object) entityPathId);
    }

    public virtual string GetRegistryPathPattern(string featureName) => this.RequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/{0}/{1}/{2}/*", (object) featureName, (object) this.m_accountName, (object) this.CollectionId) : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/{0}/*", (object) featureName);
  }
}

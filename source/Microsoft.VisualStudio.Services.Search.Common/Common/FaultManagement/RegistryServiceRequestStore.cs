// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.FaultManagement.RegistryServiceRequestStore
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Common.FaultManagement
{
  public class RegistryServiceRequestStore : IRequestStore
  {
    private const string TraceArea = "Indexing Pipeline";
    private const string TraceLayer = "FaultManagement";
    private RegistryManager m_registryManager;
    private readonly IVssRequestContext m_requestContext;

    internal RegistryManager RegistryManager
    {
      get
      {
        if (this.m_registryManager == null)
          this.m_registryManager = new RegistryManager(this.m_requestContext, "FaultManagement");
        return this.m_registryManager;
      }
      set => this.m_registryManager = value;
    }

    public RegistryServiceRequestStore(IVssRequestContext requestContext)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) || requestContext.ServiceHost.Is(TeamFoundationHostType.Application))
        this.m_requestContext = requestContext.To(TeamFoundationHostType.Deployment);
      else
        this.m_requestContext = requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) ? requestContext : throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Unsupported TeamFoundationHostType '{0}' for RegistryServiceRequestStore.", (object) requestContext.ServiceHost.HostType)));
    }

    public bool ResetRequestCounter(string request)
    {
      bool flag = false;
      try
      {
        if (string.IsNullOrEmpty(request))
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1080652, "Indexing Pipeline", "FaultManagement", "RegistryServiceRequestStore:ResetRequestCounter() - request passed is null");
        }
        else
        {
          List<RegistryEntry> registryEntries = this.BuildRegistryEntries(request);
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1080653, "Indexing Pipeline", "FaultManagement", FormattableString.Invariant(FormattableStringFactory.Create("ResetRequestCounter with {0}", (object) RegistryServiceRequestStore.GetRegistryEntries(registryEntries))));
          this.RegistryManager.AddOrUpdateRegistryValues(registryEntries);
          flag = true;
        }
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1080651, "Indexing Pipeline", "FaultManagement", FormattableString.Invariant(FormattableStringFactory.Create("RegistryServiceRequestStore:ResetRequestCounter() failed: {0}", (object) ex)));
      }
      return flag;
    }

    public bool IncrementRequestCounter(string request, int count)
    {
      bool flag = false;
      try
      {
        if (string.IsNullOrEmpty(request))
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1080652, "Indexing Pipeline", "FaultManagement", "RegistryServiceRequestStore:IncrementRequestCounter() - request passed is null");
        }
        else
        {
          this.RegistryManager.AddOrUpdateRegistryValues(this.BuildRegistryEntries(request, new DateTime?(), new int?(count)));
          flag = true;
        }
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1080651, "Indexing Pipeline", "FaultManagement", FormattableString.Invariant(FormattableStringFactory.Create("RegistryServiceRequestStore:IncrementRequestCounter() failed: {0}", (object) ex)));
      }
      return flag;
    }

    public bool TryGetRequestCounter(string request, out RequestCounter requestCounter)
    {
      requestCounter = (RequestCounter) null;
      bool requestCounter1 = false;
      List<RegistryEntry> entriesWithPathPattern = this.RegistryManager.GetRegistryEntriesWithPathPattern(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/*", (object) request));
      if (entriesWithPathPattern.Count != 0)
      {
        RegistryEntry registryEntry1 = entriesWithPathPattern.Find((Predicate<RegistryEntry>) (reg => reg.Path.Equals(RegistryServiceRequestStore.GetRegistryPath(request, "WindowStartTimeStamp"))));
        RegistryEntry registryEntry2 = entriesWithPathPattern.Find((Predicate<RegistryEntry>) (reg => reg.Path.Equals(RegistryServiceRequestStore.GetRegistryPath(request, "RequestCount"))));
        if (registryEntry1 != null && registryEntry2 != null)
        {
          requestCounter = new RequestCounter(Convert.ToDateTime(registryEntry1.Value, (IFormatProvider) CultureInfo.InvariantCulture), Convert.ToInt32(registryEntry2.Value, (IFormatProvider) CultureInfo.InvariantCulture));
          requestCounter1 = true;
        }
        else
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1080652, "Indexing Pipeline", "FaultManagement", FormattableString.Invariant(FormattableStringFactory.Create("RegistryServiceRequestStore:TryGetRequestCounter() Invalid entries: {0}", (object) RegistryServiceRequestStore.GetRegistryEntries(entriesWithPathPattern))));
      }
      return requestCounter1;
    }

    private List<RegistryEntry> BuildRegistryEntries(string request) => this.BuildRegistryEntries(request, new DateTime?(DateTime.UtcNow), new int?(0));

    private List<RegistryEntry> BuildRegistryEntries(
      string request,
      DateTime? dateTime,
      int? count)
    {
      List<RegistryEntry> registryEntryList = new List<RegistryEntry>();
      if (dateTime.HasValue)
        registryEntryList.Add(new RegistryEntry()
        {
          Path = RegistryServiceRequestStore.GetRegistryPath(request, "WindowStartTimeStamp"),
          Value = dateTime.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture)
        });
      if (count.HasValue)
        registryEntryList.Add(new RegistryEntry()
        {
          Path = RegistryServiceRequestStore.GetRegistryPath(request, "RequestCount"),
          Value = count.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture)
        });
      return registryEntryList;
    }

    private static string GetRegistryPath(string featureName, string path) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) featureName, (object) path);

    private static string GetRegistryEntries(List<RegistryEntry> registryEntries)
    {
      string empty = string.Empty;
      if (registryEntries != null)
      {
        foreach (RegistryEntry registryEntry in registryEntries)
          empty += FormattableString.Invariant(FormattableStringFactory.Create("[Path:{0} Value:{1}]", (object) registryEntry.Path, (object) registryEntry.Value));
      }
      return empty;
    }
  }
}

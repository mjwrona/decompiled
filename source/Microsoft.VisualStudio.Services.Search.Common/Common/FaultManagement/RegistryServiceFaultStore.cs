// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.FaultManagement.RegistryServiceFaultStore
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement.Definitions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Common.FaultManagement
{
  public class RegistryServiceFaultStore : IFaultStore
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

    public RegistryServiceFaultStore(IVssRequestContext requestContext)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) || requestContext.ServiceHost.Is(TeamFoundationHostType.Application))
        this.m_requestContext = requestContext.To(TeamFoundationHostType.Deployment);
      else
        this.m_requestContext = requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) ? requestContext : throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Unsupported TeamFoundationHostType '{0}' for RegistryServiceFaultStore.", (object) requestContext.ServiceHost.HostType)));
    }

    public bool TryAddOrUpdate(IndexerFaultStatus faultStatus, TransientFaultState state)
    {
      bool flag = false;
      try
      {
        if (faultStatus == null || state == null)
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1080652, "Indexing Pipeline", "FaultManagement", "RegistryServiceFaultStore:TryAddOrUpdate() - FaultStatus or State passed is null");
        }
        else
        {
          List<RegistryEntry> registryEntries = this.BuildRegistryEntries(faultStatus);
          registryEntries.AddRange((IEnumerable<RegistryEntry>) this.BuildRegistryEntries(state));
          this.RegistryManager.AddOrUpdateRegistryValues(registryEntries);
          flag = true;
        }
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1080651, "Indexing Pipeline", "FaultManagement", FormattableString.Invariant(FormattableStringFactory.Create("RegistryServiceFaultStore:TryAddOrUpdate() failed: {0}", (object) ex)));
      }
      return flag;
    }

    public bool TryGetFaultStatus(out IndexerFaultStatus status)
    {
      status = (IndexerFaultStatus) null;
      List<RegistryEntry> entriesWithPathPattern = this.RegistryManager.GetRegistryEntriesWithPathPattern(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/{0}/*", (object) "FaultManagement_FaultManager"));
      if (entriesWithPathPattern.Count == 0)
      {
        status = new IndexerFaultStatus();
        return true;
      }
      if (entriesWithPathPattern.Count != typeof (IndexerFaultStatus).GetProperties().Length)
        return false;
      status = new IndexerFaultStatus((IndexerFaultSeverity) Enum.Parse(typeof (IndexerFaultSeverity), entriesWithPathPattern.Find((Predicate<RegistryEntry>) (reg => reg.Path.Equals(RegistryServiceFaultStore.GetRegistryPath("FaultManagement_FaultManager", "Severity")))).Value), Convert.ToBoolean(entriesWithPathPattern.Find((Predicate<RegistryEntry>) (reg => reg.Path.Equals(RegistryServiceFaultStore.GetRegistryPath("FaultManagement_FaultManager", "IsActive")))).Value, (IFormatProvider) CultureInfo.InvariantCulture));
      return true;
    }

    public bool TryGetTransientFaultState(out TransientFaultState state)
    {
      state = (TransientFaultState) null;
      List<RegistryEntry> entriesWithPathPattern = this.RegistryManager.GetRegistryEntriesWithPathPattern(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/{0}/*", (object) "FaultManagement_ServiceFaultState"));
      if (entriesWithPathPattern.Count == 0)
      {
        state = new TransientFaultState();
        return true;
      }
      if (entriesWithPathPattern.Count != typeof (TransientFaultState).GetProperties().Length)
        return false;
      state = new TransientFaultState();
      state.StateSeverity = (IndexerFaultSeverity) Enum.Parse(typeof (IndexerFaultSeverity), entriesWithPathPattern.Find((Predicate<RegistryEntry>) (reg => reg.Path.Equals(RegistryServiceFaultStore.GetRegistryPath("FaultManagement_ServiceFaultState", "Severity")))).Value);
      state.ServiceFaultState = (ServiceFaultState) Enum.Parse(typeof (ServiceFaultState), entriesWithPathPattern.Find((Predicate<RegistryEntry>) (reg => reg.Path.Equals(RegistryServiceFaultStore.GetRegistryPath("FaultManagement_ServiceFaultState", "ServiceState")))).Value);
      state.FailureCount = Convert.ToInt32(entriesWithPathPattern.Find((Predicate<RegistryEntry>) (reg => reg.Path.Equals(RegistryServiceFaultStore.GetRegistryPath("FaultManagement_ServiceFaultState", "FailureCounts")))).Value, (IFormatProvider) CultureInfo.InvariantCulture);
      state.SuccessCount = Convert.ToInt32(entriesWithPathPattern.Find((Predicate<RegistryEntry>) (reg => reg.Path.Equals(RegistryServiceFaultStore.GetRegistryPath("FaultManagement_ServiceFaultState", "SuccessCounts")))).Value, (IFormatProvider) CultureInfo.InvariantCulture);
      state.StateTimeStamp = Convert.ToDateTime(entriesWithPathPattern.Find((Predicate<RegistryEntry>) (reg => reg.Path.Equals(RegistryServiceFaultStore.GetRegistryPath("FaultManagement_ServiceFaultState", "ServiceStateTimeStamp")))).Value, (IFormatProvider) CultureInfo.InvariantCulture);
      return true;
    }

    private List<RegistryEntry> BuildRegistryEntries(TransientFaultState state) => new List<RegistryEntry>()
    {
      new RegistryEntry()
      {
        Path = RegistryServiceFaultStore.GetRegistryPath("FaultManagement_ServiceFaultState", "Severity"),
        Value = state.StateSeverity.ToString()
      },
      new RegistryEntry()
      {
        Path = RegistryServiceFaultStore.GetRegistryPath("FaultManagement_ServiceFaultState", "ServiceState"),
        Value = state.ServiceFaultState.ToString()
      },
      new RegistryEntry()
      {
        Path = RegistryServiceFaultStore.GetRegistryPath("FaultManagement_ServiceFaultState", "FailureCounts"),
        Value = state.FailureCount.ToString((IFormatProvider) CultureInfo.InvariantCulture)
      },
      new RegistryEntry()
      {
        Path = RegistryServiceFaultStore.GetRegistryPath("FaultManagement_ServiceFaultState", "SuccessCounts"),
        Value = state.SuccessCount.ToString((IFormatProvider) CultureInfo.InvariantCulture)
      },
      new RegistryEntry()
      {
        Path = RegistryServiceFaultStore.GetRegistryPath("FaultManagement_ServiceFaultState", "ServiceStateTimeStamp"),
        Value = state.StateTimeStamp.ToString((IFormatProvider) CultureInfo.InvariantCulture)
      }
    };

    private List<RegistryEntry> BuildRegistryEntries(IndexerFaultStatus indexerFaultStatus) => new List<RegistryEntry>()
    {
      new RegistryEntry(RegistryServiceFaultStore.GetRegistryPath("FaultManagement_FaultManager", "Severity"), indexerFaultStatus.Severity.ToString()),
      new RegistryEntry(RegistryServiceFaultStore.GetRegistryPath("FaultManagement_FaultManager", "IsActive"), indexerFaultStatus.IsActive.ToString())
    };

    private static string GetRegistryPath(string featureName, string path) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/{0}/{1}", (object) featureName, (object) path);
  }
}

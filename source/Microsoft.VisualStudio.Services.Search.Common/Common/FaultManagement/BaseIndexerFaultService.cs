// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.FaultManagement.BaseIndexerFaultService
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement.Maps;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Common.FaultManagement
{
  public abstract class BaseIndexerFaultService : 
    IIndexerFaultService,
    IIndexerRequestThrottleService
  {
    protected const string TraceArea = "Indexing Pipeline";
    protected const string TraceLayer = "FaultManagement";

    protected BaseIndexerFaultService()
    {
    }

    protected BaseIndexerFaultService(IVssRequestContext requestContext, IFaultStore faultStore)
    {
      this.RequestContext = requestContext;
      this.FaultStore = faultStore;
      this.Settings = new FaultManagementSettings(this.RequestContext);
    }

    protected BaseIndexerFaultService(
      IVssRequestContext requestContext,
      IFaultStore faultStore,
      IRequestStore requestStore)
    {
      this.RequestContext = requestContext;
      this.FaultStore = faultStore;
      this.RequestStore = requestStore;
      this.Settings = new FaultManagementSettings(this.RequestContext);
    }

    public abstract IndexerFaultStatus GetFaultStatus();

    public bool ShouldRetryOnError(Exception ex)
    {
      foreach (FaultMapper faultMapper in IndexFaultMapManager.FaultMappers)
      {
        if (faultMapper.IsMatch(ex) && !faultMapper.Retriable)
          return false;
      }
      return true;
    }

    public FaultMapper GetFaultMapper(Exception ex)
    {
      foreach (FaultMapper faultMapper in IndexFaultMapManager.FaultMappers)
      {
        if (faultMapper.IsMatch(ex))
          return faultMapper;
      }
      return (FaultMapper) null;
    }

    public bool SetError(Exception ex)
    {
      if (!this.RequestContext.IsFeatureEnabled("Search.Server.FaultManagement"))
        return true;
      try
      {
        foreach (FaultMapper faultMapper in IndexFaultMapManager.FaultMappers)
        {
          if (faultMapper.IsMatch(ex) && faultMapper.LogFault)
          {
            this.SetFault(new IndexerFault()
            {
              Severity = faultMapper.Severity
            });
            return true;
          }
        }
      }
      catch (Exception ex1)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1080652, "Indexing Pipeline", "FaultManagement", FormattableString.Invariant(FormattableStringFactory.Create("SetError() failed - {0}", (object) ex1)));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpi("FaultManagementStateOpError", "Indexing Pipeline", 1.0);
      }
      return false;
    }

    public bool SetError(FaultMapper faultMap)
    {
      if (faultMap == null)
        throw new ArgumentNullException(nameof (faultMap));
      if (!this.RequestContext.IsFeatureEnabled("Search.Server.FaultManagement"))
        return true;
      try
      {
        if (faultMap.LogFault)
        {
          this.SetFault(new IndexerFault()
          {
            Severity = faultMap.Severity
          });
          return true;
        }
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1080652, "Indexing Pipeline", "FaultManagement", FormattableString.Invariant(FormattableStringFactory.Create("SetError() failed - {0}", (object) ex)));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpi("FaultManagementStateOpError", "Indexing Pipeline", 1.0);
      }
      return false;
    }

    public void ResetError(Exception ex)
    {
      if (!this.RequestContext.IsFeatureEnabled("Search.Server.FaultManagement"))
        return;
      try
      {
        if (ex != null)
        {
          foreach (FaultMapper faultMapper in IndexFaultMapManager.FaultMappers)
          {
            if (faultMapper.IsMatch(ex))
            {
              this.ResetFault(new IndexerFault()
              {
                Severity = faultMapper.Severity
              });
              break;
            }
          }
        }
        else
          this.ResetFaults();
      }
      catch (Exception ex1)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1080652, "Indexing Pipeline", "FaultManagement", FormattableString.Invariant(FormattableStringFactory.Create("ResetError() failed - {0}", (object) ex1)));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpi("FaultManagementStateOpError", "Indexing Pipeline", 1.0);
      }
    }

    public abstract void SetFault(IndexerFault fault);

    public abstract void ResetFault(IndexerFault fault);

    public abstract void ResetFaults();

    public virtual bool Request(string feature, IndexerFaultSeverity severity, out TimeSpan delay)
    {
      delay = new TimeSpan(0L);
      return true;
    }

    protected IVssRequestContext RequestContext { get; }

    protected IFaultStore FaultStore { get; }

    protected IRequestStore RequestStore { get; }

    protected FaultManagementSettings Settings { get; }
  }
}

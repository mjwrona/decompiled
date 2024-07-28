// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.HostMigration.MigrationTracer
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud.Utils;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Cloud.HostMigration
{
  public class MigrationTracer
  {
    private readonly int[] m_enabledTracepoints;
    private readonly bool m_enabled;
    private readonly TraceLevel m_traceLevel;
    private readonly string m_header;
    private readonly string m_area;
    private readonly string m_layer;
    private readonly Throttle m_traceThrottle;

    public MigrationTracer(
      IVssRequestContext requestContext,
      IMigrationData data,
      string tag,
      string area,
      string layer)
    {
      ArgumentUtility.CheckForNull<IMigrationData>(data, nameof (data));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(area, nameof (area));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(layer, nameof (layer));
      this.m_enabled = requestContext.GetPerHostRegistry<bool>(data.HostId, FrameworkServerConstants.ParallelMigrationRoot, FrameworkServerConstants.ParallelMigration_BlobCopyTracing, true, false);
      this.m_traceLevel = requestContext.GetPerHostRegistry<TraceLevel>(data.HostId, FrameworkServerConstants.ParallelMigrationRoot, FrameworkServerConstants.ParallelMigration_BlobCopyTracingLevel, true, TraceLevel.Info);
      int perHostRegistry = requestContext.GetPerHostRegistry<int>(data.HostId, FrameworkServerConstants.ParallelMigrationRoot, FrameworkServerConstants.ParallelMigration_BlobCopyTracingPerMinute, true, 100);
      this.m_traceThrottle = new Throttle(TimeSpan.FromMinutes(1.0), perHostRegistry);
      this.m_enabledTracepoints = ((IEnumerable<string>) requestContext.GetPerHostRegistry<string>(data.HostId, FrameworkServerConstants.ParallelMigrationRoot, FrameworkServerConstants.ParallelMigration_BlobCopyTracingTracepoints, true, "").Split(new char[1]
      {
        ';'
      }, StringSplitOptions.RemoveEmptyEntries)).Select<string, int>((Func<string, int>) (o => int.Parse(o))).ToArray<int>();
      string str1 = data.HostName != null ? "N=" + data.HostName : "ID=" + MigrationTracer.GetGuidHead(data.HostId) + "*";
      string str2 = "M=" + MigrationTracer.GetGuidHead(data.MigrationId) + "*";
      string str3 = data.StorageOnly ? "P" : "D";
      string str4 = data.StorageType.ToString();
      this.m_header = "[" + str1 + "|" + str3 + "|" + str2 + "|" + str4 + "|" + (string.IsNullOrWhiteSpace(tag) ? "None" : tag) + "]";
      this.m_area = area;
      this.m_layer = layer;
    }

    private MigrationTracer(
      bool enabled,
      string header,
      string area,
      string layer,
      int[] enabledTracepoints,
      TraceLevel traceLevel,
      Throttle traceThrottle)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(header, nameof (header));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(area, nameof (area));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(layer, nameof (layer));
      this.m_enabled = enabled;
      this.m_header = header;
      this.m_area = area;
      this.m_layer = layer;
      this.m_enabledTracepoints = enabledTracepoints;
      this.m_traceLevel = traceLevel;
      this.m_traceThrottle = traceThrottle;
    }

    private static string GetGuidHead(Guid guid) => guid.ToString("n").Substring(0, 8);

    public MigrationTracer Enter(string area, string layer) => new MigrationTracer(this.m_enabled, this.m_header, area, layer, this.m_enabledTracepoints, this.m_traceLevel, this.m_traceThrottle);

    public void Verbose(IVssRequestContext requestContext, int tracepoint, Func<string> message) => this.Trace(requestContext, tracepoint, TraceLevel.Verbose, message);

    public void Verbose(
      IVssRequestContext requestContext,
      int tracepoint,
      string message,
      params object[] args)
    {
      this.Trace(requestContext, tracepoint, TraceLevel.Verbose, message, args);
    }

    public void Info(IVssRequestContext requestContext, int tracepoint, Func<string> message) => this.Trace(requestContext, tracepoint, TraceLevel.Info, message);

    public void Info(
      IVssRequestContext requestContext,
      int tracepoint,
      string message,
      params object[] args)
    {
      this.Trace(requestContext, tracepoint, TraceLevel.Info, message, args);
    }

    public void Warn(IVssRequestContext requestContext, int tracepoint, Func<string> message) => this.Trace(requestContext, tracepoint, TraceLevel.Warning, message);

    public void Warn(
      IVssRequestContext requestContext,
      int tracepoint,
      string message,
      params object[] args)
    {
      this.Trace(requestContext, tracepoint, TraceLevel.Warning, message, args);
    }

    public void Error(IVssRequestContext requestContext, int tracepoint, Func<string> message) => this.Trace(requestContext, tracepoint, TraceLevel.Error, message);

    public void Error(
      IVssRequestContext requestContext,
      int tracepoint,
      string message,
      params object[] args)
    {
      this.Trace(requestContext, tracepoint, TraceLevel.Error, message, args);
    }

    public void Error(IVssRequestContext requestContext, int tracepoint, Exception ex) => this.Trace(requestContext, tracepoint, TraceLevel.Error, ex.Message);

    private bool ShouldTrace(IVssRequestContext requestContext, int tracepoint, TraceLevel level) => this.m_enabled && this.m_traceLevel >= level && ((IEnumerable<int>) this.m_enabledTracepoints).Contains<int>(tracepoint) && this.m_traceThrottle.ShouldAllowAction(requestContext);

    private void Trace(
      IVssRequestContext requestContext,
      int tracepoint,
      TraceLevel level,
      Func<string> message)
    {
      if (!this.ShouldTrace(requestContext, tracepoint, level))
        return;
      string format = this.m_header + " " + message();
      requestContext.TraceAlways(tracepoint, level, this.m_area, this.m_layer, format);
    }

    private void Trace(
      IVssRequestContext requestContext,
      int tracepoint,
      TraceLevel level,
      string message,
      params object[] args)
    {
      if (!this.ShouldTrace(requestContext, tracepoint, level))
        return;
      if (args.Length != 0)
      {
        try
        {
          message = string.Format(message, args);
        }
        catch (Exception ex)
        {
          message = message + " (" + string.Join(", ", args) + ")";
        }
      }
      string format = this.m_header + " " + message;
      requestContext.TraceAlways(tracepoint, level, this.m_area, this.m_layer, format);
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades.TracerFacade
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Utils;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades
{
  public class TracerFacade : ITracerService
  {
    private CircularBuffer<TracerFacade.StoredTrace>? storedTraces;
    private readonly IVssRequestContext requestContext;
    private readonly IOrgLevelPackagingSetting<bool> allowStoredTracesSetting;
    private readonly IOrgLevelPackagingSetting<int> maxStoredTracesSetting;
    private readonly IHasher<uint> hasher = (IHasher<uint>) FNVHasher32.Instance;
    public const int EnterLeaveTracepoint = 20999999;

    public TracerFacade(
      IVssRequestContext requestContext,
      IOrgLevelPackagingSetting<bool> allowStoredTracesSetting,
      IOrgLevelPackagingSetting<int> maxStoredTracesSetting)
    {
      this.requestContext = requestContext;
      this.allowStoredTracesSetting = allowStoredTracesSetting;
      this.maxStoredTracesSetting = maxStoredTracesSetting;
    }

    public void EnableStoredTraces()
    {
      if (this.storedTraces != null || !this.allowStoredTracesSetting.Get())
        return;
      this.storedTraces = new CircularBuffer<TracerFacade.StoredTrace>(this.maxStoredTracesSetting.Get());
    }

    public ITracerBlock Enter(object? sendInTheThisObject, [CallerMemberName] string? methodName = null)
    {
      IProtocol requestContextItems1 = TracerFacade.GetValueOrDefaultFromRequestContextItems<IProtocol>(this.requestContext, "Packaging.Protocol");
      string area = requestContextItems1?.CorrectlyCasedName ?? "NoProtocol";
      FeedCore requestContextItems2 = TracerFacade.GetValueOrDefaultFromRequestContextItems<FeedCore>(this.requestContext, "Packaging.Feed");
      string str1 = requestContextItems2?.Id.ToString() ?? TracerFacade.GetValueOrDefaultFromRequestContextItems<string>(this.requestContext, "Packaging.FeedId") ?? "NoFeed";
      string str2 = string.Format("Packaging.FeedTracePoint.{0}.{1}", (object) area, (object) str1);
      int tracePoint = TracerFacade.GetValueOrDefaultFromRequestContextItems<int>(this.requestContext, str2);
      if (tracePoint == 0)
      {
        tracePoint = TracingUtils.GetTracePointFor(requestContextItems2?.Id, requestContextItems1, this.hasher);
        this.requestContext.Items[str2] = (object) tracePoint;
      }
      string str3;
      if (sendInTheThisObject == null)
      {
        str3 = (string) null;
      }
      else
      {
        Type type = sendInTheThisObject.GetType();
        if ((object) type == null)
          str3 = (string) null;
        else
          str3 = type.Name.Split('`')[0];
      }
      if (str3 == null)
        str3 = "NoLayer";
      string layer = str3;
      return (ITracerBlock) new TracerFacade.TracerBlock(this.requestContext, area, layer, tracePoint, 20999999, methodName ?? "NoMethod", this.storedTraces);
    }

    private static T? GetValueOrDefaultFromRequestContextItems<T>(
      IVssRequestContext requestContext,
      string name)
    {
      IDictionary<string, object> items = requestContext.Items;
      return (items != null ? items.GetValueOrDefault<string, object>(name) : (object) null) is T valueOrDefault ? valueOrDefault : default (T);
    }

    private enum StoredTraceType
    {
      Enter,
      Leave,
      Exception,
      Marker,
    }

    private record struct StoredTrace(
      TracerFacade.StoredTraceType Type,
      string Area,
      string Layer,
      string Method,
      string? MarkerId1,
      string? MarkerId2)
    ;

    private class TracerBlock : ITracerBlock, IDisposable
    {
      private readonly IVssRequestContext requestContext;
      private readonly string area;
      private readonly string layer;
      private readonly int tracePoint;
      private readonly int tracePointForEnterLeave;
      private readonly string methodName;
      private readonly CircularBuffer<TracerFacade.StoredTrace>? storedTraces;

      public TracerBlock(
        IVssRequestContext requestContext,
        string area,
        string layer,
        int tracePoint,
        int tracePointForEnterLeave,
        string methodName,
        CircularBuffer<TracerFacade.StoredTrace>? storedTraces)
      {
        this.requestContext = requestContext;
        this.area = area;
        this.layer = layer;
        this.tracePoint = tracePoint;
        this.tracePointForEnterLeave = tracePointForEnterLeave;
        this.methodName = methodName;
        this.storedTraces = storedTraces;
        requestContext.TraceEnter(tracePointForEnterLeave, area, layer, methodName);
        storedTraces?.Add(new TracerFacade.StoredTrace(TracerFacade.StoredTraceType.Enter, area, layer, methodName, (string) null, (string) null));
      }

      public void Dispose()
      {
        this.requestContext.TraceLeave(this.tracePointForEnterLeave, this.area, this.layer, this.methodName);
        if (this.storedTraces == null)
          return;
        this.storedTraces.Add(new TracerFacade.StoredTrace(TracerFacade.StoredTraceType.Leave, this.area, this.layer, this.methodName, (string) null, (string) null));
      }

      public void TraceInfo(string message) => this.requestContext.Trace(this.tracePoint, TraceLevel.Info, this.area, this.layer, message);

      public void TraceInfo(string[] tags, string message) => this.requestContext.Trace(this.tracePoint, TraceLevel.Info, this.area, this.layer, tags, message);

      public void TraceVerbose(string message) => this.requestContext.Trace(this.tracePoint, TraceLevel.Verbose, this.area, this.layer, message);

      public void TraceError(string message) => this.requestContext.Trace(this.tracePoint, TraceLevel.Error, this.area, this.layer, message);

      public void TraceException(Exception exception)
      {
        this.requestContext.TraceException(this.tracePoint, this.area, this.layer, exception);
        this.StoreException(exception);
      }

      public void TraceException(Exception exception, string format, params object[] args)
      {
        this.requestContext.TraceException(this.tracePoint, TraceLevel.Error, this.area, this.layer, exception, format, args);
        this.StoreException(exception);
      }

      private void StoreException(Exception exception) => this.storedTraces?.Add(new TracerFacade.StoredTrace(TracerFacade.StoredTraceType.Exception, this.area, this.layer, this.methodName, exception.GetType().FullName, (string) null));

      public void TraceConditionally(Func<string> messageFunc) => this.requestContext.TraceConditionally(this.tracePoint, TraceLevel.Info, this.area, this.layer, messageFunc);

      public void TraceConditionally(string[] tags, Func<string> messageFunc) => this.requestContext.TraceConditionally(this.tracePoint, TraceLevel.Info, this.area, this.layer, tags, messageFunc);

      public void TraceInfoAlways(string message) => this.requestContext.TraceAlways(this.tracePoint, TraceLevel.Info, this.area, this.layer, message);

      public void TraceInfoAlways(string[] tags, string message) => this.requestContext.TraceAlways(this.tracePoint, TraceLevel.Info, this.area, this.layer, tags, message);

      public IDisposable CreateTimeToFirstPageExclusionBlock() => this.requestContext.CreateTimeToFirstPageExclusionBlock();

      public void TraceMarker(string markerId1, string? markerId2 = null)
      {
        if (this.storedTraces == null)
          return;
        this.storedTraces.Add(new TracerFacade.StoredTrace(TracerFacade.StoredTraceType.Marker, this.area, this.layer, this.methodName, markerId1, markerId2));
      }
    }
  }
}

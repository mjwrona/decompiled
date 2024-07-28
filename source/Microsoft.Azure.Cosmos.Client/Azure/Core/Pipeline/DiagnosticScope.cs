// Decompiled with JetBrains decompiler
// Type: Azure.Core.Pipeline.DiagnosticScope
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;


#nullable enable
namespace Azure.Core.Pipeline
{
  internal readonly struct DiagnosticScope : IDisposable
  {
    private static readonly ConcurrentDictionary<string, object?> ActivitySources = new ConcurrentDictionary<string, object>();
    private readonly DiagnosticScope.ActivityAdapter? activityAdapter;

    internal DiagnosticScope(
      string ns,
      string scopeName,
      DiagnosticListener source,
      DiagnosticScope.ActivityKind kind)
    {
      object activitySource = DiagnosticScope.GetActivitySource(ns, scopeName);
      this.IsEnabled = source.IsEnabled() || ActivityExtensions.ActivitySourceHasListeners(activitySource);
      this.activityAdapter = this.IsEnabled ? new DiagnosticScope.ActivityAdapter(activitySource, (DiagnosticSource) source, scopeName, kind, (object) null) : (DiagnosticScope.ActivityAdapter) null;
    }

    internal DiagnosticScope(
      string scopeName,
      DiagnosticListener source,
      object? diagnosticSourceArgs,
      object? activitySource,
      DiagnosticScope.ActivityKind kind)
    {
      this.IsEnabled = source.IsEnabled() || ActivityExtensions.ActivitySourceHasListeners(activitySource);
      this.activityAdapter = this.IsEnabled ? new DiagnosticScope.ActivityAdapter(activitySource, (DiagnosticSource) source, scopeName, kind, diagnosticSourceArgs) : (DiagnosticScope.ActivityAdapter) null;
    }

    public bool IsEnabled { get; }

    private static object? GetActivitySource(string ns, string name)
    {
      if (!ActivityExtensions.SupportsActivitySource())
        return (object) null;
      int length = name.IndexOf(".", StringComparison.OrdinalIgnoreCase);
      if (length == -1)
        return (object) null;
      string key = ns + "." + name.Substring(0, length);
      return DiagnosticScope.ActivitySources.GetOrAdd(key, (Func<string, object>) (n => ActivityExtensions.CreateActivitySource(n)));
    }

    public void AddAttribute(string name, string? value) => this.activityAdapter?.AddTag(name, value);

    public void AddAttribute<T>(string name, T value) => this.AddAttribute<T>(name, value, (Func<T, string>) (v => Convert.ToString((object) v, (IFormatProvider) CultureInfo.InvariantCulture) ?? string.Empty));

    public void AddAttribute<T>(string name, T value, Func<T, string> format)
    {
      if (this.activityAdapter == null)
        return;
      string str = format(value);
      this.activityAdapter.AddTag(name, str);
    }

    public void AddLink(
      string traceparent,
      string tracestate,
      IDictionary<string, string>? attributes = null)
    {
      this.activityAdapter?.AddLink(traceparent, tracestate, attributes);
    }

    public void Start() => this.activityAdapter?.Start();

    public void SetStartTime(DateTime dateTime) => this.activityAdapter?.SetStartTime(dateTime);

    public void Dispose() => this.activityAdapter?.Dispose();

    public void Failed(Exception e) => this.activityAdapter?.MarkFailed(e);

    public enum ActivityKind
    {
      Internal,
      Server,
      Client,
      Producer,
      Consumer,
    }

    private class DiagnosticActivity : Activity
    {
      public IEnumerable<Activity> Links { get; set; } = (IEnumerable<Activity>) Array.Empty<Activity>();

      public DiagnosticActivity(string operationName)
        : base(operationName)
      {
      }
    }

    private class ActivityAdapter : IDisposable
    {
      private readonly DiagnosticSource diagnosticSource;
      private readonly object? activitySource;
      private readonly string activityName;
      private readonly DiagnosticScope.ActivityKind kind;
      private object? diagnosticSourceArgs;
      private Activity? currentActivity;
      private ICollection<KeyValuePair<string, object>>? tagCollection;
      private DateTimeOffset startTime;
      private List<Activity>? links;

      public ActivityAdapter(
        object? activitySource,
        DiagnosticSource diagnosticSource,
        string activityName,
        DiagnosticScope.ActivityKind kind,
        object? diagnosticSourceArgs)
      {
        this.activitySource = activitySource;
        this.diagnosticSource = diagnosticSource;
        this.activityName = activityName;
        this.kind = kind;
        this.diagnosticSourceArgs = diagnosticSourceArgs;
        switch (this.kind)
        {
          case DiagnosticScope.ActivityKind.Internal:
            this.AddTag(nameof (kind), "internal");
            break;
          case DiagnosticScope.ActivityKind.Server:
            this.AddTag(nameof (kind), "server");
            break;
          case DiagnosticScope.ActivityKind.Client:
            this.AddTag(nameof (kind), "client");
            break;
          case DiagnosticScope.ActivityKind.Producer:
            this.AddTag(nameof (kind), "producer");
            break;
          case DiagnosticScope.ActivityKind.Consumer:
            this.AddTag(nameof (kind), "consumer");
            break;
        }
      }

      public void AddTag(string name, string? value)
      {
        if (this.currentActivity == null)
        {
          if (this.tagCollection == null)
            this.tagCollection = ActivityExtensions.CreateTagsCollection() ?? (ICollection<KeyValuePair<string, object>>) new List<KeyValuePair<string, object>>();
          this.tagCollection?.Add(new KeyValuePair<string, object>(name, (object) value));
        }
        else
          this.currentActivity?.AddTag(name, value);
      }

      private IList? GetActivitySourceLinkCollection()
      {
        if (this.links == null)
          return (IList) null;
        IList linkCollection = ActivityExtensions.CreateLinkCollection();
        if (linkCollection == null)
          return (IList) null;
        foreach (Activity link in this.links)
        {
          ICollection<KeyValuePair<string, object>> tagsCollection = ActivityExtensions.CreateTagsCollection();
          if (tagsCollection != null)
          {
            foreach (KeyValuePair<string, string> tag in link.Tags)
              tagsCollection.Add(new KeyValuePair<string, object>(tag.Key, (object) tag.Value));
          }
          object activityLink = ActivityExtensions.CreateActivityLink(link.ParentId, link.TraceStateString, tagsCollection);
          if (activityLink != null)
            linkCollection.Add(activityLink);
        }
        return linkCollection;
      }

      public void AddLink(
        string traceparent,
        string tracestate,
        IDictionary<string, string>? attributes)
      {
        Activity activity = new Activity("LinkedActivity");
        activity.SetW3CFormat();
        activity.SetParentId(traceparent);
        activity.TraceStateString = tracestate;
        if (attributes != null)
        {
          foreach (KeyValuePair<string, string> attribute in (IEnumerable<KeyValuePair<string, string>>) attributes)
            activity.AddTag(attribute.Key, attribute.Value);
        }
        if (this.links == null)
          this.links = new List<Activity>();
        this.links.Add(activity);
      }

      public void Start()
      {
        this.currentActivity = this.StartActivitySourceActivity();
        if (this.currentActivity == null)
        {
          if (!this.diagnosticSource.IsEnabled(this.activityName, this.diagnosticSourceArgs))
            return;
          this.currentActivity = (Activity) new DiagnosticScope.DiagnosticActivity(this.activityName)
          {
            Links = (IEnumerable<Activity>) ((object) this.links ?? (object) Array.Empty<Activity>())
          };
          this.currentActivity.SetW3CFormat();
          if (this.startTime != new DateTimeOffset())
            this.currentActivity.SetStartTime(this.startTime.DateTime);
          if (this.tagCollection != null)
          {
            foreach (KeyValuePair<string, object> tag in (IEnumerable<KeyValuePair<string, object>>) this.tagCollection)
              this.currentActivity.AddTag(tag.Key, (string) tag.Value);
          }
          this.currentActivity.Start();
        }
        this.diagnosticSource.Write(this.activityName + ".Start", this.diagnosticSourceArgs ?? (object) this.currentActivity);
      }

      private Activity? StartActivitySourceActivity() => ActivityExtensions.ActivitySourceStartActivity(this.activitySource, this.activityName, (int) this.kind, this.startTime, this.tagCollection, this.GetActivitySourceLinkCollection());

      public void SetStartTime(DateTime startTime)
      {
        this.startTime = (DateTimeOffset) startTime;
        this.currentActivity?.SetStartTime(startTime);
      }

      public void MarkFailed(Exception exception) => this.diagnosticSource?.Write(this.activityName + ".Exception", (object) exception);

      public void Dispose()
      {
        if (this.currentActivity == null)
          return;
        if (this.currentActivity.Duration == TimeSpan.Zero)
          this.currentActivity.SetEndTime(DateTime.UtcNow);
        this.diagnosticSource.Write(this.activityName + ".Stop", this.diagnosticSourceArgs);
        if (this.currentActivity.TryDispose())
          return;
        this.currentActivity.Stop();
      }
    }
  }
}

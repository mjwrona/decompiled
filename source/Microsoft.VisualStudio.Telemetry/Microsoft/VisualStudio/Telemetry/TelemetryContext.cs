// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetryContext
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Telemetry
{
  public class TelemetryContext : TelemetryDisposableObject
  {
    private const int SchedulerDelay = 15;
    private const string ContextPropertyPrefix = "Context.";
    private const string ContextEventPrefix = "Context/";
    private const string ContextEventCreate = "Create";
    private const string ContextEventClose = "Close";
    private const string ContextEventPostProperty = "PostProperty";
    private readonly TelemetryPropertyBags.PrefixedConcurrent<object> sharedProperties;
    private readonly TelemetryPropertyBags.PrefixedConcurrent<Func<object>> realtimeSharedProperties;
    private readonly ITelemetrySessionInternal telemetrySessionInternal;
    private readonly bool overrideInit;
    private readonly object disposeLocker = new object();
    private readonly ITelemetryScheduler scheduler;
    private readonly ConcurrentQueue<TelemetryContext.PostPropertyEntry> postedProperties = new ConcurrentQueue<TelemetryContext.PostPropertyEntry>();
    private readonly DateTime contextStart = DateTime.UtcNow;
    private bool disposedContextPart;

    public IDictionary<string, object> SharedProperties => (IDictionary<string, object>) this.sharedProperties;

    public IDictionary<string, Func<object>> RealtimeSharedProperties => (IDictionary<string, Func<object>>) this.realtimeSharedProperties;

    public bool HasSharedProperties => this.sharedProperties.HasProperties<object>();

    public string ContextName { get; private set; }

    public void PostProperty(string propertyName, object propertyValue) => this.PostProperty(propertyName, propertyValue, false);

    internal void PostProperty(string propertyName, object propertyValue, bool isReserved)
    {
      if (this.IsDisposed)
        return;
      propertyName.RequiresArgumentNotNullAndNotWhiteSpace(nameof (propertyName));
      propertyValue.RequiresArgumentNotNull<object>(nameof (propertyValue));
      this.postedProperties.Enqueue(new TelemetryContext.PostPropertyEntry(propertyName, propertyValue, isReserved));
      this.scheduler.ScheduleTimed(new Action(this.FlushPostedProperties));
    }

    internal void FlushPostedProperties()
    {
      this.RequiresNotDisposed();
      if (this.postedProperties.Count == 0 || !this.scheduler.CanEnterTimedDelegate())
        return;
      TelemetryEvent telemetryEvent = this.CreateTelemetryEvent("PostProperty");
      TelemetryContext.PostPropertyEntry result;
      while (this.postedProperties.TryDequeue(out result))
      {
        if (result.IsReserved)
          telemetryEvent.ReservedProperties[result.Key] = result.Value;
        else
          telemetryEvent.Properties[result.Key] = result.Value;
      }
      TelemetrySession.ValidateEvent(telemetryEvent);
      TelemetryContext.ValidateEventProperties(telemetryEvent);
      this.AddReservedPropertiesToTheEvent(telemetryEvent);
      this.telemetrySessionInternal.PostValidatedEvent(telemetryEvent);
      this.scheduler.ExitTimedDelegate();
    }

    internal TelemetryContext(
      string contextName,
      ITelemetrySessionInternal telemetrySessionInternal,
      ITelemetryScheduler theScheduler = null,
      bool theOverrideInit = false,
      Action<TelemetryContext> initializationAction = null)
    {
      if (!TelemetryContext.IsContextNameValid(contextName))
        throw new ArgumentException("contextName is invalid, contextName must contain alphanumeric characters only");
      telemetrySessionInternal.RequiresArgumentNotNull<ITelemetrySessionInternal>(nameof (telemetrySessionInternal));
      if (theScheduler == null)
      {
        theScheduler = (ITelemetryScheduler) new TelemetryScheduler();
        theScheduler.InitializeTimed(TimeSpan.FromSeconds(15.0));
      }
      this.ContextName = contextName;
      this.telemetrySessionInternal = telemetrySessionInternal;
      this.scheduler = theScheduler;
      this.overrideInit = theOverrideInit;
      string prefix = "Context." + this.ContextName + ".";
      this.sharedProperties = new TelemetryPropertyBags.PrefixedConcurrent<object>(prefix);
      this.realtimeSharedProperties = new TelemetryPropertyBags.PrefixedConcurrent<Func<object>>(prefix);
      this.telemetrySessionInternal.AddContext(this);
      if (initializationAction != null)
        initializationAction(this);
      if (this.overrideInit)
        return;
      this.telemetrySessionInternal.PostValidatedEvent(this.BuildStartEvent());
    }

    internal static bool IsContextNameValid(string contextName) => !string.IsNullOrEmpty(contextName) && contextName.All<char>(new Func<char, bool>(char.IsLetterOrDigit));

    internal void ProcessEvent(TelemetryEvent telemetryEvent, bool overwriteExisting = true)
    {
      foreach (KeyValuePair<string, object> prefixed in this.sharedProperties.PrefixedEnumerable)
      {
        if (overwriteExisting || !telemetryEvent.Properties.ContainsKey(prefixed.Key))
          telemetryEvent.Properties[prefixed.Key] = prefixed.Value;
      }
    }

    internal void ProcessEventRealtime(TelemetryEvent telemetryEvent)
    {
      foreach (KeyValuePair<string, Func<object>> prefixed in this.realtimeSharedProperties.PrefixedEnumerable)
      {
        object obj = prefixed.Value();
        if (obj != null)
          telemetryEvent.Properties[prefixed.Key] = obj;
      }
    }

    internal static void ValidateEvent(TelemetryEvent telemetryEvent)
    {
      telemetryEvent.RequiresArgumentNotNull<TelemetryEvent>(nameof (telemetryEvent));
      TelemetryContext.ValidateEventName(telemetryEvent);
      TelemetryContext.ValidateEventProperties(telemetryEvent);
    }

    internal static void ValidatePropertyName(string propertyName)
    {
      propertyName.RequiresArgumentNotNullAndNotWhiteSpace(nameof (propertyName));
      if (TelemetryContext.IsPropertyNameReserved(propertyName))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "property '{0}' has reserved prefix '{1}'", new object[2]
        {
          (object) propertyName,
          (object) "Context."
        }));
    }

    internal static bool IsPropertyNameReserved(string propertyName) => propertyName.StartsWith("Context.", StringComparison.Ordinal);

    internal static bool IsEventNameContextPostProperty(string eventName) => eventName.Equals(TelemetryContext.BuildEventName("PostProperty"), StringComparison.OrdinalIgnoreCase);

    protected override void DisposeManagedResources()
    {
      if (this.disposedContextPart)
        return;
      lock (this.disposeLocker)
      {
        if (this.disposedContextPart)
          return;
        this.scheduler.CancelTimed(true);
        this.FlushPostedProperties();
        if (!this.overrideInit)
          this.telemetrySessionInternal.PostValidatedEvent(this.BuildCloseEvent());
        this.telemetrySessionInternal.RemoveContext(this);
        this.disposedContextPart = true;
      }
    }

    private static void ValidateEventName(TelemetryEvent telemetryEvent)
    {
      if (telemetryEvent.Name.StartsWith("Context/", StringComparison.OrdinalIgnoreCase))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "event '{0}' has reserved prefix '{1}'", new object[2]
        {
          (object) telemetryEvent.Name,
          (object) "Context/"
        }));
    }

    private static void ValidateEventProperties(TelemetryEvent telemetryEvent)
    {
      foreach (KeyValuePair<string, object> property in (IEnumerable<KeyValuePair<string, object>>) telemetryEvent.Properties)
        TelemetryContext.ValidatePropertyName(property.Key);
    }

    private static string BuildEventName(string eventName) => "Context/" + eventName;

    private TelemetryEvent BuildStartEvent()
    {
      TelemetryEvent telemetryEvent = this.CreateTelemetryEvent("Create");
      this.AddReservedPropertiesToTheEvent(telemetryEvent);
      return telemetryEvent;
    }

    private TelemetryEvent BuildCloseEvent()
    {
      TelemetryEvent telemetryEvent = this.CreateTelemetryEvent("Close");
      this.AddReservedPropertiesToTheEvent(telemetryEvent);
      telemetryEvent.ReservedProperties.AddPrefixed("Reserved.ContextDurationInMs", (object) Math.Round(DateTime.UtcNow.Subtract(this.contextStart).TotalMilliseconds));
      return telemetryEvent;
    }

    private void AddReservedPropertiesToTheEvent(TelemetryEvent telemetryEvent) => telemetryEvent.ReservedProperties.AddPrefixed("Reserved.ContextName", (object) this.ContextName);

    private TelemetryEvent CreateTelemetryEvent(string eventName) => new TelemetryEvent(TelemetryContext.BuildEventName(eventName));

    private class PostPropertyEntry
    {
      public string Key { get; }

      public object Value { get; }

      public bool IsReserved { get; }

      public PostPropertyEntry(string key, object value, bool isReserved)
      {
        this.Key = key;
        this.Value = value;
        this.IsReserved = isReserved;
      }
    }
  }
}

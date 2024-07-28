// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Common.PerformanceDescriptor
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C3F75541-7C8A-4AF6-A47E-709CEEE7550D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Common
{
  public sealed class PerformanceDescriptor : IDisposable
  {
    private readonly IVssRequestContext requestContext;
    private readonly string layerName;
    private readonly int level;
    private readonly Action<IVssRequestContext, string, string, CustomerIntelligenceData> publishEventDelegate;
    private readonly Action<Guid> disposeHandler;
    private IDictionary<string, object> additionalProperties;
    private long inclusiveTimeInMilliSeconds;
    private string actionName;

    public PerformanceDescriptor(
      IVssRequestContext context,
      string layerName,
      string actionName,
      int level,
      Action<IVssRequestContext, string, string, CustomerIntelligenceData> publishEventDelegate,
      Action<Guid> descriptionDisposeHandler)
    {
      this.requestContext = context;
      this.layerName = layerName;
      this.actionName = actionName;
      this.level = level;
      this.publishEventDelegate = publishEventDelegate;
      this.disposeHandler = descriptionDisposeHandler;
      this.additionalProperties = (IDictionary<string, object>) new Dictionary<string, object>();
    }

    [StaticSafe]
    public static PerformanceDescriptor Empty => new PerformanceDescriptor((IVssRequestContext) null, string.Empty, string.Empty, 0, (Action<IVssRequestContext, string, string, CustomerIntelligenceData>) null, (Action<Guid>) null);

    public void SetElapsedTime(long elapsedTime) => this.inclusiveTimeInMilliSeconds = elapsedTime;

    public void SetAdditionalData<TValue>(string key, TValue value) where TValue : IConvertible
    {
      if (this.additionalProperties.ContainsKey(key))
        this.additionalProperties[key] = (object) value;
      else
        this.additionalProperties.Add(key, (object) value);
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "All exceptions must be caught inorder to prevent them from being thrown")]
    public void Dispose()
    {
      try
      {
        if (this.requestContext == null)
          return;
        CustomerIntelligenceData intelligenceData = this.GetCustomerIntelligenceData(this.layerName, this.actionName, this.level);
        if (this.disposeHandler != null)
          this.disposeHandler(this.requestContext.ActivityId);
        if (this.publishEventDelegate == null)
          return;
        this.publishEventDelegate(this.requestContext, "Performance", "Layer", intelligenceData);
      }
      catch (Exception ex)
      {
        if (this.requestContext == null)
          return;
        this.requestContext.TraceException(1973200, TraceLevel.Warning, "ReleaseManagementService", "PerformanceManager", ex);
      }
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "level", Justification = "level is the right name to use")]
    private CustomerIntelligenceData GetCustomerIntelligenceData(
      string layer,
      string action,
      int level)
    {
      CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
      intelligenceData.Add("Layer", layer);
      intelligenceData.Add("ActionName", action);
      intelligenceData.Add("Level", (double) level);
      intelligenceData.Add("ActivityId", (object) this.requestContext.ActivityId);
      intelligenceData.Add("UniqueIdentifier", (object) this.requestContext.UniqueIdentifier);
      intelligenceData.Add("InclusiveTimeMs", (double) this.inclusiveTimeInMilliSeconds);
      foreach (KeyValuePair<string, object> additionalProperty in (IEnumerable<KeyValuePair<string, object>>) this.additionalProperties)
        intelligenceData.Add(additionalProperty.Key, additionalProperty.Value);
      return intelligenceData;
    }
  }
}

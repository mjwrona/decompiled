// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.ComplexPropertyAction
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Telemetry
{
  internal sealed class ComplexPropertyAction : IEventProcessorAction
  {
    internal const int MaxSerializedLength = 61440;
    internal const string FailedToSerializePropertyName = "Reserved.ComplexProperty.FailedToSerialize";
    internal const string TruncatedPropertyName = "Reserved.ComplexProperty.Truncated";
    private readonly IComplexObjectSerializerFactory serializerFactory;
    private readonly IPiiPropertyProcessor piiProcessor;
    private readonly Func<object, string> converterToHashValue;
    private readonly Func<object, string> converterToRawValue;
    private IComplexObjectSerializer serializer;

    public int Priority => 250;

    public ComplexPropertyAction(
      IComplexObjectSerializerFactory serializerFactory,
      IPiiPropertyProcessor piiProcessor)
    {
      serializerFactory.RequiresArgumentNotNull<IComplexObjectSerializerFactory>(nameof (serializerFactory));
      piiProcessor.RequiresArgumentNotNull<IPiiPropertyProcessor>(nameof (piiProcessor));
      this.serializerFactory = serializerFactory;
      this.piiProcessor = piiProcessor;
      this.converterToHashValue = (Func<object, string>) (value => this.piiProcessor.ConvertToHashedValue(value));
      this.converterToRawValue = (Func<object, string>) (value => this.piiProcessor.ConvertToRawValue(value).ToString());
    }

    public bool Execute(IEventProcessorContext eventProcessorContext)
    {
      eventProcessorContext.RequiresArgumentNotNull<IEventProcessorContext>(nameof (eventProcessorContext));
      this.EnsureSerializerIsInitialized();
      TelemetryEvent telemetryEvent = eventProcessorContext.TelemetryEvent;
      List<KeyValuePair<string, string>> propertiesToModify = new List<KeyValuePair<string, string>>();
      Dictionary<string, string> val1 = new Dictionary<string, string>();
      foreach (KeyValuePair<string, object> property in (IEnumerable<KeyValuePair<string, object>>) telemetryEvent.Properties)
      {
        if (property.Value is TelemetryComplexProperty)
        {
          try
          {
            this.SerializeProperty(property.Key, property.Value as TelemetryComplexProperty, eventProcessorContext, propertiesToModify);
          }
          catch (ComplexObjectSerializerException ex)
          {
            val1.Add(property.Key, ex.Message);
          }
        }
      }
      if (val1.Count > 0)
      {
        try
        {
          this.SerializeProperty("Reserved.ComplexProperty.FailedToSerialize", new TelemetryComplexProperty((object) val1), eventProcessorContext, propertiesToModify);
        }
        catch
        {
        }
      }
      foreach (KeyValuePair<string, string> keyValuePair in val1)
        telemetryEvent.Properties.Remove(keyValuePair.Key);
      HashSet<string> val2 = new HashSet<string>();
      foreach (KeyValuePair<string, string> keyValuePair in propertiesToModify)
      {
        if (keyValuePair.Value.Length <= 61440)
        {
          telemetryEvent.Properties[keyValuePair.Key] = (object) keyValuePair.Value;
        }
        else
        {
          telemetryEvent.Properties[keyValuePair.Key] = (object) (keyValuePair.Value.Substring(0, 61437) + "...");
          val2.Add(keyValuePair.Key);
        }
      }
      if (val2.Count > 0)
      {
        propertiesToModify.Clear();
        try
        {
          this.SerializeProperty("Reserved.ComplexProperty.Truncated", new TelemetryComplexProperty((object) val2), eventProcessorContext, propertiesToModify);
        }
        catch
        {
        }
        foreach (KeyValuePair<string, string> keyValuePair in propertiesToModify)
          telemetryEvent.Properties[keyValuePair.Key] = (object) keyValuePair.Value;
      }
      return true;
    }

    private void EnsureSerializerIsInitialized()
    {
      if (this.serializer != null)
        return;
      this.serializer = this.serializerFactory.Instance();
    }

    private void SerializeProperty(
      string propertyName,
      TelemetryComplexProperty propertyValue,
      IEventProcessorContext eventProcessorContext,
      List<KeyValuePair<string, string>> propertiesToModify)
    {
      this.serializer.SetTypeConverter(this.piiProcessor.TypeOfPiiProperty(), this.converterToHashValue);
      propertiesToModify.Add(new KeyValuePair<string, string>(propertyName, this.serializer.Serialize(propertyValue.Value)));
      if (!this.serializer.WasConverterUsedForType(this.piiProcessor.TypeOfPiiProperty()) || !this.piiProcessor.CanAddRawValue(eventProcessorContext))
        return;
      this.serializer.SetTypeConverter(this.piiProcessor.TypeOfPiiProperty(), this.converterToRawValue);
      propertiesToModify.Add(new KeyValuePair<string, string>(this.piiProcessor.BuildRawPropertyName(propertyName), this.serializer.Serialize(propertyValue.Value)));
    }
  }
}

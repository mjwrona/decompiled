// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.JsonComplexObjectSerializer
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry.JsonHelpers;
using Microsoft.VisualStudio.Utilities.Internal;
using Newtonsoft.Json;
using System;

namespace Microsoft.VisualStudio.Telemetry
{
  internal sealed class JsonComplexObjectSerializer : IComplexObjectSerializer
  {
    private const string SerializationErrorLabel = "Serialization error: ";
    private readonly Lazy<CustomJsonConverter> jsonConverter = new Lazy<CustomJsonConverter>();

    public void SetTypeConverter(Type type, Func<object, string> converter)
    {
      converter.RequiresArgumentNotNull<Func<object, string>>(nameof (converter));
      this.jsonConverter.Value.AddConverter(type, converter);
    }

    public string Serialize(object obj)
    {
      string str;
      try
      {
        if (this.jsonConverter.IsValueCreated)
        {
          this.jsonConverter.Value.ResetUsageInformation();
          str = JsonConvert.SerializeObject(obj, (JsonConverter) this.jsonConverter.Value);
        }
        else
          str = JsonConvert.SerializeObject(obj);
      }
      catch (JsonSerializationException ex)
      {
        throw new ComplexObjectSerializerException(ex.Message, (Exception) ex);
      }
      return str;
    }

    public bool WasConverterUsedForType(Type type) => this.jsonConverter.IsValueCreated && this.jsonConverter.Value.WasConverterUsed(type);
  }
}

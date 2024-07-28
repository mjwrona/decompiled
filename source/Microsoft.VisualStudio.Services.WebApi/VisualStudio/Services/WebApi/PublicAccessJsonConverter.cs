// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.PublicAccessJsonConverter
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class PublicAccessJsonConverter : JsonConverter
  {
    public abstract object GetDefaultValue();

    public override bool CanRead => false;

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      throw new NotImplementedException();
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      if (value != null && PublicAccessJsonConverter.ShouldClearValueFunction != null && PublicAccessJsonConverter.ShouldClearValueFunction())
        serializer.Serialize(writer, this.GetDefaultValue());
      else
        serializer.Serialize(writer, value);
    }

    internal static Func<bool> ShouldClearValueFunction { get; set; }
  }
}

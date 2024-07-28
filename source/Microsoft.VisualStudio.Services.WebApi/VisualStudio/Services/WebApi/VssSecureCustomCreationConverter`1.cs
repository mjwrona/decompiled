// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.VssSecureCustomCreationConverter`1
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Microsoft.VisualStudio.Services.WebApi
{
  public abstract class VssSecureCustomCreationConverter<T> : CustomCreationConverter<T>
  {
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => this.Validate(value, serializer);

    private void Validate(object value, JsonSerializer serializer)
    {
      Action<object, JsonSerializer> validate = VssSecureJsonConverterHelper.Validate;
      if (validate == null)
        return;
      validate(value, serializer);
    }
  }
}

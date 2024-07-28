// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.PublicAccessJsonConverter`1
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.ComponentModel;
using System.Reflection;

namespace Microsoft.VisualStudio.Services.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class PublicAccessJsonConverter<T> : PublicAccessJsonConverter
  {
    public PublicAccessJsonConverter()
    {
      if (typeof (T) == typeof (bool))
        throw new ArgumentException("The PublicAccessJsonConverter does not support Boolean types, because the value can be inferred from the existance or non existance of the property in the json.");
    }

    public override bool CanConvert(Type objectType) => typeof (T).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());
  }
}

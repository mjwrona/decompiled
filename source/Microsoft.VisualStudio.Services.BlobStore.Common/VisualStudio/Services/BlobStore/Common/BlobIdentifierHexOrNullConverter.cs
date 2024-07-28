// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierHexOrNullConverter
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using Newtonsoft.Json;
using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  public class BlobIdentifierHexOrNullConverter : BlobIdentifierHexConverter
  {
    private static readonly Lazy<BlobIdentifierHexOrNullConverter> _instance = new Lazy<BlobIdentifierHexOrNullConverter>((Func<BlobIdentifierHexOrNullConverter>) (() => new BlobIdentifierHexOrNullConverter()));

    public static BlobIdentifierHexOrNullConverter Instance => BlobIdentifierHexOrNullConverter._instance.Value;

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      return reader.ValueType == (Type) null ? (object) null : base.ReadJson(reader, objectType, existingValue, serializer);
    }
  }
}

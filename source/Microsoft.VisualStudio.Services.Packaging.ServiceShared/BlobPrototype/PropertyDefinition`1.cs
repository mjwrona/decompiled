// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.PropertyDefinition`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Newtonsoft.Json;
using System;
using System.Linq.Expressions;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class PropertyDefinition<TMetadataEntry>
  {
    public PropertyDefinition(
      Expression<Func<TMetadataEntry, object>> mappedPropertyExpression,
      Action<JsonWriter, ICache<string, object>, TMetadataEntry, IInternalMetadataDocumentProperties> writeFunc,
      Action<TMetadataEntry, MetadataDocumentProperties, ICache<string, object>, object> applyFunc,
      Action<TMetadataEntry, ICache<string, object>, JsonReader> applyUsingReaderFunc = null,
      Func<TMetadataEntry, bool> skipSerializingCondition = null,
      Action<TMetadataEntry> applyDefaultFunc = null)
    {
      this.CodePropertyName = ExpressionUtils.NameOf<TMetadataEntry>(mappedPropertyExpression);
      this.WriteFunc = writeFunc;
      this.ApplyFunc = applyFunc;
      this.ApplyUsingReaderFunc = applyUsingReaderFunc;
      this.SkipSerializingCondition = skipSerializingCondition;
      this.ApplyDefaultFunc = applyDefaultFunc;
    }

    public string CodePropertyName { get; }

    public Action<JsonWriter, ICache<string, object>, TMetadataEntry, IInternalMetadataDocumentProperties> WriteFunc { get; }

    public Action<TMetadataEntry, MetadataDocumentProperties, ICache<string, object>, object> ApplyFunc { get; }

    public Action<TMetadataEntry, ICache<string, object>, JsonReader> ApplyUsingReaderFunc { get; }

    public Func<TMetadataEntry, bool> SkipSerializingCondition { get; }

    public Action<TMetadataEntry> ApplyDefaultFunc { get; }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Formatter.Deserialization.ODataCollectionDeserializer
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.OData;
using Microsoft.OData.Edm;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Microsoft.AspNet.OData.Formatter.Deserialization
{
  public class ODataCollectionDeserializer : ODataEdmTypeDeserializer
  {
    private static readonly MethodInfo _castMethodInfo = typeof (Enumerable).GetMethod("Cast");

    public ODataCollectionDeserializer(ODataDeserializerProvider deserializerProvider)
      : base(ODataPayloadKind.Collection, deserializerProvider)
    {
    }

    public override object Read(
      ODataMessageReader messageReader,
      Type type,
      ODataDeserializerContext readContext)
    {
      if (messageReader == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (messageReader));
      IEdmTypeReference edmType = readContext.GetEdmType(type);
      IEdmTypeReference expectedItemTypeReference = edmType.IsCollection() ? edmType.AsCollection().ElementType() : throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (type), SRResources.ArgumentMustBeOfType, (object) EdmTypeKind.Collection);
      return this.ReadInline((object) ODataCollectionDeserializer.ReadCollection(messageReader.CreateODataCollectionReader(expectedItemTypeReference)), edmType, readContext);
    }

    public override sealed object ReadInline(
      object item,
      IEdmTypeReference edmType,
      ODataDeserializerContext readContext)
    {
      if (item == null)
        return (object) null;
      if (edmType == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (edmType));
      IEdmCollectionTypeReference collectionTypeReference = edmType.IsCollection() ? edmType.AsCollection() : throw new SerializationException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.TypeCannotBeDeserialized, (object) edmType.ToTraceString()));
      IEdmTypeReference edmTypeReference = collectionTypeReference.ElementType();
      if (!(item is ODataCollectionValue collectionValue))
        throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (item), SRResources.ArgumentMustBeOfType, (object) typeof (ODataCollectionValue).Name);
      RuntimeHelpers.EnsureSufficientExecutionStack();
      IEnumerable enumerable = this.ReadCollectionValue(collectionValue, edmTypeReference, readContext);
      if (enumerable == null)
        return (object) null;
      if (readContext.IsUntyped && edmTypeReference.IsEnum())
        return (object) enumerable.ConvertToEdmObject(collectionTypeReference);
      Type clrType = EdmLibHelpers.GetClrType(edmTypeReference, readContext.Model);
      return (object) (ODataCollectionDeserializer._castMethodInfo.MakeGenericMethod(clrType).Invoke((object) null, new object[1]
      {
        (object) enumerable
      }) as IEnumerable);
    }

    public virtual IEnumerable ReadCollectionValue(
      ODataCollectionValue collectionValue,
      IEdmTypeReference elementType,
      ODataDeserializerContext readContext)
    {
      ODataCollectionDeserializer collectionDeserializer = this;
      if (collectionValue == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (collectionValue));
      if (elementType == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (elementType));
      ODataEdmTypeDeserializer deserializer = collectionDeserializer.DeserializerProvider.GetEdmTypeDeserializer(elementType);
      if (deserializer == null)
        throw new SerializationException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.TypeCannotBeDeserialized, (object) elementType.FullName()));
      foreach (object obj in collectionValue.Items)
      {
        if (elementType.IsPrimitive())
          yield return obj;
        else
          yield return deserializer.ReadInline(obj, elementType, readContext);
      }
    }

    internal static ODataCollectionValue ReadCollection(ODataCollectionReader reader)
    {
      ArrayList source = new ArrayList();
      string str = (string) null;
      while (reader.Read())
      {
        if (ODataCollectionReaderState.Value == reader.State)
          source.Add(reader.Item);
        else if (ODataCollectionReaderState.CollectionStart == reader.State)
          str = reader.Item.ToString();
      }
      return new ODataCollectionValue()
      {
        Items = source.Cast<object>(),
        TypeName = str
      };
    }
  }
}

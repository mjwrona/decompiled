// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Formatter.Deserialization.ODataResourceSetDeserializer
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.OData;
using Microsoft.OData.Edm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Microsoft.AspNet.OData.Formatter.Deserialization
{
  public class ODataResourceSetDeserializer : ODataEdmTypeDeserializer
  {
    private static readonly MethodInfo CastMethodInfo = typeof (Enumerable).GetMethod("Cast");

    public ODataResourceSetDeserializer(ODataDeserializerProvider deserializerProvider)
      : base(ODataPayloadKind.ResourceSet, deserializerProvider)
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
      if (!edmType.IsCollection() || !edmType.AsCollection().ElementType().IsStructured())
      {
        string argumentMustBeOfType = SRResources.ArgumentMustBeOfType;
        object[] objArray = new object[1];
        EdmTypeKind edmTypeKind = EdmTypeKind.Complex;
        string str1 = edmTypeKind.ToString();
        edmTypeKind = EdmTypeKind.Entity;
        string str2 = edmTypeKind.ToString();
        objArray[0] = (object) (str1 + " or " + str2);
        throw Microsoft.AspNet.OData.Common.Error.Argument("edmType", argumentMustBeOfType, objArray);
      }
      return this.ReadInline((object) messageReader.CreateODataResourceSetReader().ReadResourceOrResourceSet(), edmType, readContext);
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
      if (!edmType.IsCollection() || !edmType.AsCollection().ElementType().IsStructured())
        throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (edmType), SRResources.TypeMustBeResourceSet, (object) edmType.ToTraceString());
      if (!(item is ODataResourceSetWrapper resourceSet))
        throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (item), SRResources.ArgumentMustBeOfType, (object) typeof (ODataResourceSetWrapper).Name);
      RuntimeHelpers.EnsureSufficientExecutionStack();
      IEdmStructuredTypeReference structuredTypeReference = edmType.AsCollection().ElementType().AsStructured();
      IEnumerable enumerable = this.ReadResourceSet(resourceSet, structuredTypeReference, readContext);
      if (enumerable == null || !structuredTypeReference.IsComplex())
        return (object) enumerable;
      if (readContext.IsUntyped)
      {
        EdmComplexObjectCollection objectCollection = new EdmComplexObjectCollection(edmType.AsCollection());
        foreach (EdmComplexObject edmComplexObject in enumerable)
          objectCollection.Add((IEdmComplexObject) edmComplexObject);
        return (object) objectCollection;
      }
      Type clrType = EdmLibHelpers.GetClrType((IEdmTypeReference) structuredTypeReference, readContext.Model);
      return (object) (ODataResourceSetDeserializer.CastMethodInfo.MakeGenericMethod(clrType).Invoke((object) null, new object[1]
      {
        (object) enumerable
      }) as IEnumerable);
    }

    public virtual IEnumerable ReadResourceSet(
      ODataResourceSetWrapper resourceSet,
      IEdmStructuredTypeReference elementType,
      ODataDeserializerContext readContext)
    {
      ODataEdmTypeDeserializer deserializer = this.DeserializerProvider.GetEdmTypeDeserializer((IEdmTypeReference) elementType);
      if (deserializer == null)
        throw new SerializationException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.TypeCannotBeDeserialized, (object) elementType.FullName()));
      foreach (object resource in (IEnumerable<ODataResourceWrapper>) resourceSet.Resources)
        yield return deserializer.ReadInline(resource, (IEdmTypeReference) elementType, readContext);
    }
  }
}

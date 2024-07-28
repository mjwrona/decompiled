// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.KeyBinder
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.OData.UriParser
{
  internal sealed class KeyBinder
  {
    private readonly MetadataBinder.QueryTokenVisitor keyValueBindMethod;

    internal KeyBinder(
      MetadataBinder.QueryTokenVisitor keyValueBindMethod)
    {
      this.keyValueBindMethod = keyValueBindMethod;
    }

    internal QueryNode BindKeyValues(
      CollectionResourceNode collectionNode,
      IEnumerable<NamedValue> namedValues,
      IEdmModel model)
    {
      IEdmEntityType collectionItemEntityType = (collectionNode.ItemStructuredType as IEdmEntityTypeReference).EntityDefinition();
      QueryNode keyLookupNode;
      if (this.TryBindToDeclaredKey(collectionNode, namedValues, model, collectionItemEntityType, out keyLookupNode) || this.TryBindToDeclaredAlternateKey(collectionNode, namedValues, model, collectionItemEntityType, out keyLookupNode))
        return keyLookupNode;
      throw new ODataException(Microsoft.OData.Strings.MetadataBinder_NotAllKeyPropertiesSpecifiedInKeyValues((object) collectionNode.ItemStructuredType.FullName()));
    }

    private bool TryBindToDeclaredAlternateKey(
      CollectionResourceNode collectionNode,
      IEnumerable<NamedValue> namedValues,
      IEdmModel model,
      IEdmEntityType collectionItemEntityType,
      out QueryNode keyLookupNode)
    {
      foreach (IDictionary<string, IEdmProperty> keys in model.GetAlternateKeysAnnotation(collectionItemEntityType))
      {
        if (this.TryBindToKeys(collectionNode, namedValues, model, collectionItemEntityType, keys, out keyLookupNode))
          return true;
      }
      keyLookupNode = (QueryNode) null;
      return false;
    }

    private bool TryBindToDeclaredKey(
      CollectionResourceNode collectionNode,
      IEnumerable<NamedValue> namedValues,
      IEdmModel model,
      IEdmEntityType collectionItemEntityType,
      out QueryNode keyLookupNode)
    {
      Dictionary<string, IEdmProperty> keys = new Dictionary<string, IEdmProperty>((IEqualityComparer<string>) StringComparer.Ordinal);
      foreach (IEdmStructuralProperty structuralProperty in collectionItemEntityType.Key())
        keys[structuralProperty.Name] = (IEdmProperty) structuralProperty;
      return this.TryBindToKeys(collectionNode, namedValues, model, collectionItemEntityType, (IDictionary<string, IEdmProperty>) keys, out keyLookupNode);
    }

    private bool TryBindToKeys(
      CollectionResourceNode collectionNode,
      IEnumerable<NamedValue> namedValues,
      IEdmModel model,
      IEdmEntityType collectionItemEntityType,
      IDictionary<string, IEdmProperty> keys,
      out QueryNode keyLookupNode)
    {
      List<KeyPropertyValue> list = new List<KeyPropertyValue>();
      HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.Ordinal);
      foreach (NamedValue namedValue in namedValues)
      {
        KeyPropertyValue keyPropertyValue;
        if (!this.TryBindKeyPropertyValue(namedValue, collectionItemEntityType, keys, out keyPropertyValue))
        {
          keyLookupNode = (QueryNode) null;
          return false;
        }
        if (!stringSet.Add(keyPropertyValue.KeyProperty.Name))
          throw new ODataException(Microsoft.OData.Strings.MetadataBinder_DuplicitKeyPropertyInKeyValues((object) keyPropertyValue.KeyProperty.Name));
        list.Add(keyPropertyValue);
      }
      if (list.Count == 0)
      {
        keyLookupNode = (QueryNode) collectionNode;
        return true;
      }
      if (list.Count != collectionItemEntityType.Key().Count<IEdmStructuralProperty>())
      {
        keyLookupNode = (QueryNode) null;
        return false;
      }
      keyLookupNode = (QueryNode) new KeyLookupNode(collectionNode, (IEnumerable<KeyPropertyValue>) new ReadOnlyCollection<KeyPropertyValue>((IList<KeyPropertyValue>) list));
      return true;
    }

    private bool TryBindKeyPropertyValue(
      NamedValue namedValue,
      IEdmEntityType collectionItemEntityType,
      IDictionary<string, IEdmProperty> keys,
      out KeyPropertyValue keyPropertyValue)
    {
      ExceptionUtils.CheckArgumentNotNull<NamedValue>(namedValue, nameof (namedValue));
      ExceptionUtils.CheckArgumentNotNull<LiteralToken>(namedValue.Value, "namedValue.Value");
      IEdmProperty edmProperty1 = (IEdmProperty) null;
      if (namedValue.Name == null)
      {
        foreach (IEdmProperty edmProperty2 in (IEnumerable<IEdmProperty>) keys.Values)
          edmProperty1 = edmProperty1 == null ? edmProperty2 : throw new ODataException(Microsoft.OData.Strings.MetadataBinder_UnnamedKeyValueOnTypeWithMultipleKeyProperties((object) collectionItemEntityType.FullTypeName()));
      }
      else
      {
        edmProperty1 = keys.SingleOrDefault<KeyValuePair<string, IEdmProperty>>((Func<KeyValuePair<string, IEdmProperty>, bool>) (k => string.CompareOrdinal(k.Key, namedValue.Name) == 0)).Value;
        if (edmProperty1 == null)
        {
          keyPropertyValue = (KeyPropertyValue) null;
          return false;
        }
      }
      IEdmTypeReference type = edmProperty1.Type;
      SingleValueNode typeIfNeeded = MetadataBindingUtils.ConvertToTypeIfNeeded((SingleValueNode) this.keyValueBindMethod((QueryToken) namedValue.Value), type);
      keyPropertyValue = new KeyPropertyValue()
      {
        KeyProperty = edmProperty1,
        KeyValue = typeIfNeeded
      };
      return true;
    }
  }
}

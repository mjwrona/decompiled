// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.AlternateKeysODataUriResolver
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.UriParser
{
  public sealed class AlternateKeysODataUriResolver : ODataUriResolver
  {
    private readonly IEdmModel model;

    public AlternateKeysODataUriResolver(IEdmModel model) => this.model = model;

    public override IEnumerable<KeyValuePair<string, object>> ResolveKeys(
      IEdmEntityType type,
      IDictionary<string, string> namedValues,
      Func<IEdmTypeReference, string, object> convertFunc)
    {
      IEnumerable<KeyValuePair<string, object>> convertedPairs;
      try
      {
        convertedPairs = base.ResolveKeys(type, namedValues, convertFunc);
      }
      catch (ODataException ex)
      {
        if (!this.TryResolveAlternateKeys(type, namedValues, convertFunc, out convertedPairs))
          throw;
      }
      return convertedPairs;
    }

    private bool TryResolveAlternateKeys(
      IEdmEntityType type,
      IDictionary<string, string> namedValues,
      Func<IEdmTypeReference, string, object> convertFunc,
      out IEnumerable<KeyValuePair<string, object>> convertedPairs)
    {
      foreach (IDictionary<string, IEdmProperty> keyProperties in this.model.GetAlternateKeysAnnotation(type))
      {
        if (this.TryResolveKeys(type, namedValues, keyProperties, convertFunc, out convertedPairs))
          return true;
      }
      convertedPairs = (IEnumerable<KeyValuePair<string, object>>) null;
      return false;
    }

    private bool TryResolveKeys(
      IEdmEntityType type,
      IDictionary<string, string> namedValues,
      IDictionary<string, IEdmProperty> keyProperties,
      Func<IEdmTypeReference, string, object> convertFunc,
      out IEnumerable<KeyValuePair<string, object>> convertedPairs)
    {
      if (namedValues.Count != keyProperties.Count)
      {
        convertedPairs = (IEnumerable<KeyValuePair<string, object>>) null;
        return false;
      }
      Dictionary<string, object> dictionary = new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.Ordinal);
      foreach (KeyValuePair<string, IEdmProperty> keyProperty in (IEnumerable<KeyValuePair<string, IEdmProperty>>) keyProperties)
      {
        KeyValuePair<string, IEdmProperty> kvp = keyProperty;
        string namedValue;
        if (!namedValues.TryGetValue(kvp.Key, out namedValue) && !this.EnableCaseInsensitive)
        {
          convertedPairs = (IEnumerable<KeyValuePair<string, object>>) null;
          return false;
        }
        if (namedValue == null)
        {
          List<string> list = namedValues.Keys.Where<string>((Func<string, bool>) (key => string.Equals(kvp.Key, key, StringComparison.OrdinalIgnoreCase))).ToList<string>();
          if (list.Count > 1)
            throw new ODataException(Microsoft.OData.Strings.UriParserMetadata_MultipleMatchingKeysFound((object) kvp.Key));
          if (list.Count == 0)
          {
            convertedPairs = (IEnumerable<KeyValuePair<string, object>>) null;
            return false;
          }
          namedValue = namedValues[list.Single<string>()];
        }
        object obj = convertFunc(kvp.Value.Type, namedValue);
        if (obj == null)
        {
          convertedPairs = (IEnumerable<KeyValuePair<string, object>>) null;
          return false;
        }
        dictionary[kvp.Key] = obj;
      }
      convertedPairs = (IEnumerable<KeyValuePair<string, object>>) dictionary;
      return true;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Nest.FieldCapabilitiesFields
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;

namespace Nest
{
  [JsonFormatter(typeof (FieldCapabilitiesFields.Converter))]
  public class FieldCapabilitiesFields : ResolvableDictionaryProxy<Field, FieldTypes>
  {
    internal FieldCapabilitiesFields(
      IConnectionConfigurationValues c,
      IReadOnlyDictionary<Field, FieldTypes> b)
      : base(c, b)
    {
    }

    internal class Converter : 
      ResolvableDictionaryFormatterBase<FieldCapabilitiesFields, Field, FieldTypes>
    {
      protected override FieldCapabilitiesFields Create(
        IConnectionSettingsValues s,
        Dictionary<Field, FieldTypes> d)
      {
        return new FieldCapabilitiesFields((IConnectionConfigurationValues) s, (IReadOnlyDictionary<Field, FieldTypes>) d);
      }
    }
  }
}

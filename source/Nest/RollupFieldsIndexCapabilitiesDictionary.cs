// Decompiled with JetBrains decompiler
// Type: Nest.RollupFieldsIndexCapabilitiesDictionary
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Nest
{
  [JsonFormatter(typeof (RollupFieldsIndexCapabilitiesDictionary.Converter))]
  public class RollupFieldsIndexCapabilitiesDictionary : 
    ResolvableDictionaryProxy<Nest.Field, IReadOnlyCollection<RollupFieldsIndexCapabilities>>
  {
    internal RollupFieldsIndexCapabilitiesDictionary(
      IConnectionConfigurationValues c,
      IReadOnlyDictionary<Nest.Field, IReadOnlyCollection<RollupFieldsIndexCapabilities>> b)
      : base(c, b)
    {
    }

    public IReadOnlyCollection<RollupFieldsIndexCapabilities> Field<T>(
      Expression<Func<T, object>> selector)
    {
      return this[(Nest.Field) (Expression) selector];
    }

    public IReadOnlyCollection<RollupFieldsIndexCapabilities> Field<T, TValue>(
      Expression<Func<T, TValue>> selector)
    {
      return this[(Nest.Field) (Expression) selector];
    }

    internal class Converter : 
      ResolvableDictionaryFormatterBase<RollupFieldsIndexCapabilitiesDictionary, Nest.Field, IReadOnlyCollection<RollupFieldsIndexCapabilities>>
    {
      protected override RollupFieldsIndexCapabilitiesDictionary Create(
        IConnectionSettingsValues s,
        Dictionary<Nest.Field, IReadOnlyCollection<RollupFieldsIndexCapabilities>> d)
      {
        return new RollupFieldsIndexCapabilitiesDictionary((IConnectionConfigurationValues) s, (IReadOnlyDictionary<Nest.Field, IReadOnlyCollection<RollupFieldsIndexCapabilities>>) d);
      }
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Nest.RollupFieldsCapabilitiesDictionary
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
  [JsonFormatter(typeof (RollupFieldsCapabilitiesDictionary.Converter))]
  public class RollupFieldsCapabilitiesDictionary : 
    ResolvableDictionaryProxy<Nest.Field, IReadOnlyCollection<RollupFieldsCapabilities>>
  {
    internal RollupFieldsCapabilitiesDictionary(
      IConnectionConfigurationValues c,
      IReadOnlyDictionary<Nest.Field, IReadOnlyCollection<RollupFieldsCapabilities>> b)
      : base(c, b)
    {
    }

    public IReadOnlyCollection<RollupFieldsCapabilities> Field<T>(
      Expression<Func<T, object>> selector)
    {
      return this[(Nest.Field) (Expression) selector];
    }

    public IReadOnlyCollection<RollupFieldsCapabilities> Field<T, TValue>(
      Expression<Func<T, TValue>> selector)
    {
      return this[(Nest.Field) (Expression) selector];
    }

    internal class Converter : 
      ResolvableDictionaryFormatterBase<RollupFieldsCapabilitiesDictionary, Nest.Field, IReadOnlyCollection<RollupFieldsCapabilities>>
    {
      protected override RollupFieldsCapabilitiesDictionary Create(
        IConnectionSettingsValues s,
        Dictionary<Nest.Field, IReadOnlyCollection<RollupFieldsCapabilities>> d)
      {
        return new RollupFieldsCapabilitiesDictionary((IConnectionConfigurationValues) s, (IReadOnlyDictionary<Nest.Field, IReadOnlyCollection<RollupFieldsCapabilities>>) d);
      }
    }
  }
}

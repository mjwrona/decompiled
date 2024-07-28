// Decompiled with JetBrains decompiler
// Type: Nest.MultiGetResponse
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  [JsonFormatter(typeof (MultiGetResponseFormatter))]
  public class MultiGetResponse : ResponseBase
  {
    public IReadOnlyCollection<IMultiGetHit<object>> Hits => (IReadOnlyCollection<IMultiGetHit<object>>) this.InternalHits.ToList<IMultiGetHit<object>>().AsReadOnly();

    public override bool IsValid => base.IsValid && !this.InternalHits.HasAny<IMultiGetHit<object>>((Func<IMultiGetHit<object>, bool>) (d => d.Error != null));

    internal ICollection<IMultiGetHit<object>> InternalHits { get; set; } = (ICollection<IMultiGetHit<object>>) new List<IMultiGetHit<object>>();

    public MultiGetHit<T> Get<T>(string id) where T : class => this.Hits.OfType<MultiGetHit<T>>().FirstOrDefault<MultiGetHit<T>>((Func<MultiGetHit<T>, bool>) (m => m.Id == id));

    public MultiGetHit<T> Get<T>(long id) where T : class => this.Get<T>(id.ToString((IFormatProvider) CultureInfo.InvariantCulture));

    public FieldValues GetFieldSelection<T>(long id) where T : class => this.GetFieldValues<T>(id.ToString((IFormatProvider) CultureInfo.InvariantCulture));

    public FieldValues GetFieldValues<T>(string id) where T : class => this.Get<T>(id)?.Fields ?? FieldValues.Empty;

    public IEnumerable<IMultiGetHit<T>> GetMany<T>(IEnumerable<string> ids) where T : class
    {
      HashSet<string> seenIndices = (HashSet<string>) null;
      foreach (string id in ids.Distinct<string>())
      {
        if (seenIndices == null)
          seenIndices = new HashSet<string>();
        else
          seenIndices.Clear();
        foreach (IMultiGetHit<T> multiGetHit in this.Hits.OfType<IMultiGetHit<T>>())
        {
          if (string.Equals(multiGetHit.Id, id) && seenIndices.Add(multiGetHit.Index))
            yield return multiGetHit;
        }
      }
    }

    public IEnumerable<IMultiGetHit<T>> GetMany<T>(IEnumerable<long> ids) where T : class => this.GetMany<T>(ids.Select<long, string>((Func<long, string>) (i => i.ToString((IFormatProvider) CultureInfo.InvariantCulture))));

    public T Source<T>(string id) where T : class
    {
      MultiGetHit<T> multiGetHit = this.Get<T>(id);
      // ISSUE: explicit non-virtual call
      return multiGetHit == null ? default (T) : __nonvirtual (multiGetHit.Source);
    }

    public T Source<T>(long id) where T : class => this.Source<T>(id.ToString((IFormatProvider) CultureInfo.InvariantCulture));

    public IEnumerable<T> SourceMany<T>(IEnumerable<string> ids) where T : class => this.GetMany<T>(ids).Where<IMultiGetHit<T>>((Func<IMultiGetHit<T>, bool>) (hit => hit.Found)).Select<IMultiGetHit<T>, T>((Func<IMultiGetHit<T>, T>) (hit => hit.Source));

    public IEnumerable<T> SourceMany<T>(IEnumerable<long> ids) where T : class => this.SourceMany<T>(ids.Select<long, string>((Func<long, string>) (i => i.ToString((IFormatProvider) CultureInfo.InvariantCulture))));
  }
}

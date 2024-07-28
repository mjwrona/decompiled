// Decompiled with JetBrains decompiler
// Type: Nest.CombinedFieldsQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class CombinedFieldsQueryDescriptor<T> : 
    QueryDescriptorBase<CombinedFieldsQueryDescriptor<T>, ICombinedFieldsQuery>,
    ICombinedFieldsQuery,
    IQuery
    where T : class
  {
    protected override bool Conditionless => CombinedFieldsQuery.IsConditionless((ICombinedFieldsQuery) this);

    string ICombinedFieldsQuery.Query { get; set; }

    Nest.Fields ICombinedFieldsQuery.Fields { get; set; }

    Nest.MinimumShouldMatch ICombinedFieldsQuery.MinimumShouldMatch { get; set; }

    bool? ICombinedFieldsQuery.AutoGenerateSynonymsPhraseQuery { get; set; }

    Nest.Operator? ICombinedFieldsQuery.Operator { get; set; }

    Nest.ZeroTermsQuery? ICombinedFieldsQuery.ZeroTermsQuery { get; set; }

    public CombinedFieldsQueryDescriptor<T> Query(string query) => this.Assign<string>(query, (Action<ICombinedFieldsQuery, string>) ((a, v) => a.Query = v));

    public CombinedFieldsQueryDescriptor<T> Fields(
      Func<FieldsDescriptor<T>, IPromise<Nest.Fields>> fields)
    {
      return this.Assign<Func<FieldsDescriptor<T>, IPromise<Nest.Fields>>>(fields, (Action<ICombinedFieldsQuery, Func<FieldsDescriptor<T>, IPromise<Nest.Fields>>>) ((a, v) => a.Fields = v != null ? v(new FieldsDescriptor<T>())?.Value : (Nest.Fields) null));
    }

    public CombinedFieldsQueryDescriptor<T> Fields(Nest.Fields fields) => this.Assign<Nest.Fields>(fields, (Action<ICombinedFieldsQuery, Nest.Fields>) ((a, v) => a.Fields = v));

    public CombinedFieldsQueryDescriptor<T> MinimumShouldMatch(Nest.MinimumShouldMatch minimumShouldMatch) => this.Assign<Nest.MinimumShouldMatch>(minimumShouldMatch, (Action<ICombinedFieldsQuery, Nest.MinimumShouldMatch>) ((a, v) => a.MinimumShouldMatch = v));

    public CombinedFieldsQueryDescriptor<T> Operator(Nest.Operator? op) => this.Assign<Nest.Operator?>(op, (Action<ICombinedFieldsQuery, Nest.Operator?>) ((a, v) => a.Operator = v));

    public CombinedFieldsQueryDescriptor<T> ZeroTermsQuery(Nest.ZeroTermsQuery? zeroTermsQuery) => this.Assign<Nest.ZeroTermsQuery?>(zeroTermsQuery, (Action<ICombinedFieldsQuery, Nest.ZeroTermsQuery?>) ((a, v) => a.ZeroTermsQuery = v));

    public CombinedFieldsQueryDescriptor<T> AutoGenerateSynonymsPhraseQuery(
      bool? autoGenerateSynonymsPhraseQuery = true)
    {
      return this.Assign<bool?>(autoGenerateSynonymsPhraseQuery, (Action<ICombinedFieldsQuery, bool?>) ((a, v) => a.AutoGenerateSynonymsPhraseQuery = v));
    }
  }
}

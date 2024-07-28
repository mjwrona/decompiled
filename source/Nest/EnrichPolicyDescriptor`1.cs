// Decompiled with JetBrains decompiler
// Type: Nest.EnrichPolicyDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class EnrichPolicyDescriptor<TDocument> : 
    DescriptorBase<EnrichPolicyDescriptor<TDocument>, IEnrichPolicy>,
    IEnrichPolicy
    where TDocument : class
  {
    Nest.Indices IEnrichPolicy.Indices { get; set; }

    Field IEnrichPolicy.MatchField { get; set; }

    Fields IEnrichPolicy.EnrichFields { get; set; }

    string IEnrichPolicy.Query { get; set; }

    public EnrichPolicyDescriptor<TDocument> Indices(Nest.Indices indices) => this.Assign<Nest.Indices>(indices, (Action<IEnrichPolicy, Nest.Indices>) ((a, v) => a.Indices = v));

    public EnrichPolicyDescriptor<TDocument> MatchField(Field matchField) => this.Assign<Field>(matchField, (Action<IEnrichPolicy, Field>) ((a, v) => a.MatchField = v));

    public EnrichPolicyDescriptor<TDocument> MatchField<TValue>(
      Expression<Func<TDocument, TValue>> objectPath)
    {
      return this.Assign<Expression<Func<TDocument, TValue>>>(objectPath, (Action<IEnrichPolicy, Expression<Func<TDocument, TValue>>>) ((a, v) => a.MatchField = (Field) (Expression) v));
    }

    public EnrichPolicyDescriptor<TDocument> EnrichFields(Fields enrichFields) => this.Assign<Fields>(enrichFields, (Action<IEnrichPolicy, Fields>) ((a, v) => a.EnrichFields = v));

    public EnrichPolicyDescriptor<TDocument> EnrichFields(
      Func<FieldsDescriptor<TDocument>, IPromise<Fields>> fields)
    {
      return this.Assign<Func<FieldsDescriptor<TDocument>, IPromise<Fields>>>(fields, (Action<IEnrichPolicy, Func<FieldsDescriptor<TDocument>, IPromise<Fields>>>) ((a, v) => a.EnrichFields = v != null ? v(new FieldsDescriptor<TDocument>())?.Value : (Fields) null));
    }

    public EnrichPolicyDescriptor<TDocument> Query(string query) => this.Assign<string>(query, (Action<IEnrichPolicy, string>) ((a, v) => a.Query = v));
  }
}

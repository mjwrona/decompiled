// Decompiled with JetBrains decompiler
// Type: Nest.TermQueryDescriptorBase`3
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public abstract class TermQueryDescriptorBase<TDescriptor, TInterface, T> : 
    FieldNameQueryDescriptorBase<TDescriptor, TInterface, T>,
    ITermQuery,
    IFieldNameQuery,
    IQuery
    where TDescriptor : TermQueryDescriptorBase<TDescriptor, TInterface, T>, TInterface
    where TInterface : class, ITermQuery
    where T : class
  {
    protected override bool Conditionless => TermQuery.IsConditionless((ITermQuery) this);

    object ITermQuery.Value { get; set; }

    bool? ITermQuery.CaseInsensitive { get; set; }

    public TDescriptor Value(object value)
    {
      this.Self.Value = value;
      return (TDescriptor) this;
    }

    public TDescriptor CaseInsensitive(bool? caseInsensitive = true)
    {
      this.Self.CaseInsensitive = caseInsensitive;
      return (TDescriptor) this;
    }
  }
}

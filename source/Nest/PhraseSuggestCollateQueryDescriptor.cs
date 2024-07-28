// Decompiled with JetBrains decompiler
// Type: Nest.PhraseSuggestCollateQueryDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class PhraseSuggestCollateQueryDescriptor : 
    DescriptorBase<PhraseSuggestCollateQueryDescriptor, IPhraseSuggestCollateQuery>,
    IPhraseSuggestCollateQuery
  {
    Nest.Id IPhraseSuggestCollateQuery.Id { get; set; }

    string IPhraseSuggestCollateQuery.Source { get; set; }

    public PhraseSuggestCollateQueryDescriptor Source(string source) => this.Assign<string>(source, (Action<IPhraseSuggestCollateQuery, string>) ((a, v) => a.Source = v));

    public PhraseSuggestCollateQueryDescriptor Id(Nest.Id id) => this.Assign<Nest.Id>(id, (Action<IPhraseSuggestCollateQuery, Nest.Id>) ((a, v) => a.Id = v));
  }
}

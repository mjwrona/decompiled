// Decompiled with JetBrains decompiler
// Type: Nest.PainlessContextSetupDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class PainlessContextSetupDescriptor : 
    DescriptorBase<PainlessContextSetupDescriptor, IPainlessContextSetup>,
    IPainlessContextSetup
  {
    object IPainlessContextSetup.Document { get; set; }

    IndexName IPainlessContextSetup.Index { get; set; }

    QueryContainer IPainlessContextSetup.Query { get; set; }

    public PainlessContextSetupDescriptor Document<T>(T document) => this.Assign<T>(document, (Action<IPainlessContextSetup, T>) ((a, v) => a.Document = (object) v));

    public PainlessContextSetupDescriptor Index(IndexName index) => this.Assign<IndexName>(index, (Action<IPainlessContextSetup, IndexName>) ((a, v) => a.Index = v));

    public PainlessContextSetupDescriptor Query<T>(
      Func<QueryContainerDescriptor<T>, QueryContainer> querySelector)
      where T : class
    {
      return this.Assign<Func<QueryContainerDescriptor<T>, QueryContainer>>(querySelector, (Action<IPainlessContextSetup, Func<QueryContainerDescriptor<T>, QueryContainer>>) ((a, v) => a.Query = v != null ? v(new QueryContainerDescriptor<T>()) : (QueryContainer) null));
    }
  }
}

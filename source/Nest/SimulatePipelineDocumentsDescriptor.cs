// Decompiled with JetBrains decompiler
// Type: Nest.SimulatePipelineDocumentsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class SimulatePipelineDocumentsDescriptor : 
    DescriptorPromiseBase<SimulatePipelineDocumentsDescriptor, IList<ISimulatePipelineDocument>>
  {
    public SimulatePipelineDocumentsDescriptor()
      : base((IList<ISimulatePipelineDocument>) new List<ISimulatePipelineDocument>())
    {
    }

    public SimulatePipelineDocumentsDescriptor Document(
      Func<SimulatePipelineDocumentDescriptor, ISimulatePipelineDocument> selector)
    {
      return this.Assign<Func<SimulatePipelineDocumentDescriptor, ISimulatePipelineDocument>>(selector, (Action<IList<ISimulatePipelineDocument>, Func<SimulatePipelineDocumentDescriptor, ISimulatePipelineDocument>>) ((a, v) => a.AddIfNotNull<ISimulatePipelineDocument>(v != null ? v(new SimulatePipelineDocumentDescriptor()) : (ISimulatePipelineDocument) null)));
    }
  }
}

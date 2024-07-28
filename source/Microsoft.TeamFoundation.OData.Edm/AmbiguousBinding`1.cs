// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.AmbiguousBinding`1
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Validation;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm
{
  internal class AmbiguousBinding<TElement> : BadElement where TElement : class, IEdmNamedElement
  {
    private readonly List<TElement> bindings = new List<TElement>();

    public AmbiguousBinding(TElement first, TElement second)
      : base((IEnumerable<EdmError>) new EdmError[1]
      {
        new EdmError((EdmLocation) null, EdmErrorCode.BadAmbiguousElementBinding, Strings.Bad_AmbiguousElementBinding((object) first.Name))
      })
    {
      this.AddBinding(first);
      this.AddBinding(second);
    }

    public IEnumerable<TElement> Bindings => (IEnumerable<TElement>) this.bindings;

    public string Name => this.bindings.First<TElement>().Name ?? string.Empty;

    public void AddBinding(TElement binding)
    {
      if (this.bindings.Contains(binding))
        return;
      this.bindings.Add(binding);
    }
  }
}

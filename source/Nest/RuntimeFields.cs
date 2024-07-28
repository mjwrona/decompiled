// Decompiled with JetBrains decompiler
// Type: Nest.RuntimeFields
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections;
using System.Collections.Generic;

namespace Nest
{
  public class RuntimeFields : 
    IsADictionaryBase<Field, IRuntimeField>,
    IRuntimeFields,
    IIsADictionary<Field, IRuntimeField>,
    IDictionary<Field, IRuntimeField>,
    ICollection<KeyValuePair<Field, IRuntimeField>>,
    IEnumerable<KeyValuePair<Field, IRuntimeField>>,
    IEnumerable,
    IIsADictionary
  {
    public RuntimeFields()
    {
    }

    public RuntimeFields(IDictionary<Field, IRuntimeField> container)
      : base(container)
    {
    }

    public RuntimeFields(Dictionary<Field, IRuntimeField> container)
      : base((IDictionary<Field, IRuntimeField>) container)
    {
    }

    public void Add(Field name, IRuntimeField runtimeField) => this.BackingDictionary.Add(name, runtimeField);
  }
}

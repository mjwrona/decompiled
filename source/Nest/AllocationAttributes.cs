// Decompiled with JetBrains decompiler
// Type: Nest.AllocationAttributes
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Nest
{
  public class AllocationAttributes : 
    IsADictionaryBase<string, IList<string>>,
    IAllocationAttributes,
    IIsADictionary<string, IList<string>>,
    IDictionary<string, IList<string>>,
    ICollection<KeyValuePair<string, IList<string>>>,
    IEnumerable<KeyValuePair<string, IList<string>>>,
    IEnumerable,
    IIsADictionary
  {
    IDictionary<string, IList<string>> IAllocationAttributes.Attributes => (IDictionary<string, IList<string>>) this.BackingDictionary;

    public void Add(string attribute, params string[] values) => this.BackingDictionary.Add(attribute, (IList<string>) ((IEnumerable<string>) values).ToList<string>());

    public void Add(string attribute, IEnumerable<string> values) => this.BackingDictionary.Add(attribute, (IList<string>) values.ToList<string>());
  }
}

// Decompiled with JetBrains decompiler
// Type: Nest.IAllocationAttributes
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections;
using System.Collections.Generic;

namespace Nest
{
  public interface IAllocationAttributes : 
    IIsADictionary<string, IList<string>>,
    IDictionary<string, IList<string>>,
    ICollection<KeyValuePair<string, IList<string>>>,
    IEnumerable<KeyValuePair<string, IList<string>>>,
    IEnumerable,
    IIsADictionary
  {
    IDictionary<string, IList<string>> Attributes { get; }
  }
}

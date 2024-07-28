// Decompiled with JetBrains decompiler
// Type: Nest.Privileges
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections;
using System.Collections.Generic;

namespace Nest
{
  public class Privileges : 
    IsADictionaryBase<string, IPrivilegesActions>,
    IPrivileges,
    IIsADictionary<string, IPrivilegesActions>,
    IDictionary<string, IPrivilegesActions>,
    ICollection<KeyValuePair<string, IPrivilegesActions>>,
    IEnumerable<KeyValuePair<string, IPrivilegesActions>>,
    IEnumerable,
    IIsADictionary
  {
    public Privileges()
    {
    }

    internal Privileges(
      IDictionary<string, IPrivilegesActions> backingDictionary)
      : base(backingDictionary)
    {
    }

    public void Add(string name, IPrivilegesActions actions) => this.BackingDictionary.Add(name, actions);
  }
}

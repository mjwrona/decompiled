// Decompiled with JetBrains decompiler
// Type: Nest.AppPrivileges
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections;
using System.Collections.Generic;

namespace Nest
{
  public class AppPrivileges : 
    IsADictionaryBase<string, IPrivileges>,
    IAppPrivileges,
    IIsADictionary<string, IPrivileges>,
    IDictionary<string, IPrivileges>,
    ICollection<KeyValuePair<string, IPrivileges>>,
    IEnumerable<KeyValuePair<string, IPrivileges>>,
    IEnumerable,
    IIsADictionary
  {
    public AppPrivileges()
    {
    }

    internal AppPrivileges(IDictionary<string, IPrivileges> backingDictionary)
      : base(backingDictionary)
    {
    }

    public void Add(string name, IPrivileges privileges) => this.BackingDictionary.Add(name, privileges);
  }
}

// Decompiled with JetBrains decompiler
// Type: Nest.PrivilegesActionsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class PrivilegesActionsDescriptor : 
    DescriptorBase<PrivilegesActionsDescriptor, IPrivilegesActions>,
    IPrivilegesActions
  {
    IEnumerable<string> IPrivilegesActions.Actions { get; set; }

    IDictionary<string, object> IPrivilegesActions.Metadata { get; set; }

    public PrivilegesActionsDescriptor Actions(params string[] actions) => this.Assign<string[]>(actions, (Action<IPrivilegesActions, string[]>) ((a, v) => a.Actions = (IEnumerable<string>) v));

    public PrivilegesActionsDescriptor Actions(IEnumerable<string> actions) => this.Assign<IEnumerable<string>>(actions, (Action<IPrivilegesActions, IEnumerable<string>>) ((a, v) => a.Actions = v));

    public PrivilegesActionsDescriptor Metadata(IDictionary<string, object> meta) => this.Assign<IDictionary<string, object>>(meta, (Action<IPrivilegesActions, IDictionary<string, object>>) ((a, v) => a.Metadata = v));

    public PrivilegesActionsDescriptor Metadata(
      Func<FluentDictionary<string, object>, FluentDictionary<string, object>> meta)
    {
      return this.Assign<Func<FluentDictionary<string, object>, FluentDictionary<string, object>>>(meta, (Action<IPrivilegesActions, Func<FluentDictionary<string, object>, FluentDictionary<string, object>>>) ((a, v) => a.Metadata = v != null ? (IDictionary<string, object>) v(new FluentDictionary<string, object>()) : (IDictionary<string, object>) null));
    }
  }
}

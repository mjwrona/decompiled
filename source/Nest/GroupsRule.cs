// Decompiled with JetBrains decompiler
// Type: Nest.GroupsRule
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  public class GroupsRule : FieldRuleBase
  {
    public GroupsRule(params string[] groups) => this.Groups = (IEnumerable<string>) groups;

    public GroupsRule(IEnumerable<string> groups) => this.Groups = groups;
  }
}

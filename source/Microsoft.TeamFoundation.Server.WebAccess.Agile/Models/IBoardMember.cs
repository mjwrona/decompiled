// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.IBoardMember
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.Models
{
  public interface IBoardMember
  {
    Guid Id { get; }

    string Title { get; }

    IEnumerable<string> Values { get; }

    IBoardNode ChildNode { get; }

    string Description { get; }

    bool HandlesNull { get; }

    LayoutOptions LayoutOptions { get; }

    FunctionReference ItemOrdering { get; }

    FunctionReference SortValue { get; }

    MemberLimit Limits { get; }

    IEnumerable<IItem> Items { get; }

    bool CanCreateNewItems { get; set; }

    Dictionary<string, string> Metadata { get; }

    JsObject ToJson();
  }
}

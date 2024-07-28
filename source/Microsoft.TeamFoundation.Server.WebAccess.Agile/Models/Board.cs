// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.Board
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.Models
{
  public class Board : IBoard
  {
    public Board(BoardNode node) => this.Node = (IBoardNode) node;

    public Guid Id { get; set; }

    public IBoardNode Node { get; private set; }

    public IItemSource ItemDataSource { get; set; }

    public FunctionReference Membership { get; set; }

    public Dictionary<string, string[]> FilterableFieldNamesByItemType { get; set; }

    public int PageSize { get; set; }

    public int FilterPageSize { get; set; }

    public JsObject ToJson(IVssRequestContext requestContext) => Microsoft.TeamFoundation.Server.WebAccess.Agile.JsonExtensions.ToJson(this, requestContext);

    public IDictionary<string, string> FieldTypes { get; set; }
  }
}

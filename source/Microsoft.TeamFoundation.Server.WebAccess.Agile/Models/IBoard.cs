// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.IBoard
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.Models
{
  public interface IBoard
  {
    Guid Id { get; }

    IBoardNode Node { get; }

    IItemSource ItemDataSource { get; set; }

    [DataMember(Name = "fields")]
    IDictionary<string, string> FieldTypes { get; }

    FunctionReference Membership { get; set; }

    Dictionary<string, string[]> FilterableFieldNamesByItemType { get; set; }

    int PageSize { get; set; }

    int FilterPageSize { get; set; }

    JsObject ToJson(IVssRequestContext requestContext);
  }
}

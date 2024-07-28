// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.BoardNode
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.Models
{
  public class BoardNode : IBoardNode
  {
    private IList<BoardMember> m_members;
    private IDictionary<string, BoardMember> m_memberMap = (IDictionary<string, BoardMember>) new Dictionary<string, BoardMember>();

    public BoardNode(string field, IList<BoardMember> members)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(field, nameof (field));
      ArgumentUtility.CheckForNull<IList<BoardMember>>(members, nameof (members));
      this.FieldName = field;
      this.IsItemDriven = false;
      this.m_members = members;
    }

    public string FieldName { get; private set; }

    public string LayoutStyle { get; set; }

    public IEnumerable<IBoardMember> Members => (IEnumerable<IBoardMember>) this.m_members;

    public bool IsItemDriven { get; set; }

    internal void AddItems(IEnumerable<IItem> items)
    {
      foreach (IItem obj in items)
      {
        string fieldValue = obj[this.FieldName];
        BoardMember boardMember;
        if (!this.m_memberMap.TryGetValue(fieldValue, out boardMember))
        {
          boardMember = this.m_members.FirstOrDefault<BoardMember>((Func<BoardMember, bool>) (m => m.IsMatch(fieldValue)));
          if (boardMember != null)
            this.m_memberMap[fieldValue] = boardMember;
          else
            continue;
        }
        boardMember.AddItem(obj);
      }
    }

    public JsObject ToJson() => Microsoft.TeamFoundation.Server.WebAccess.Agile.JsonExtensions.ToJson(this);
  }
}

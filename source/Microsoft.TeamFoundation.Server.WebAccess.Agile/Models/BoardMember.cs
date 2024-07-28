// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.BoardMember
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.Models
{
  public class BoardMember : IBoardMember
  {
    private string m_title;
    private LinkedList<IItem> m_items = new LinkedList<IItem>();

    public BoardMember(IEnumerable<string> values) => this.Values = (IEnumerable<string>) new List<string>(values);

    public BoardMember(string value)
      : this((IEnumerable<string>) new string[1]{ value })
    {
    }

    public Guid Id { get; set; }

    public string Title
    {
      get => this.m_title ?? Convert.ToString(this.Values.First<string>(), (IFormatProvider) CultureInfo.InvariantCulture);
      set => this.m_title = value;
    }

    public bool CanCreateNewItems { get; set; }

    public bool HandlesNull { get; set; }

    public IEnumerable<string> Values { get; private set; }

    public IEnumerable<IItem> Items => (IEnumerable<IItem>) this.m_items;

    public IBoardNode ChildNode { get; set; }

    public string Description { get; set; }

    public LayoutOptions LayoutOptions { get; set; }

    public FunctionReference ItemOrdering { get; set; }

    public FunctionReference SortValue { get; set; }

    public Dictionary<string, string> Metadata { get; set; }

    internal void AddItem(IItem item) => this.m_items.AddLast(item);

    internal bool IsMatch(string fieldValue) => this.Values.Contains<string>(fieldValue);

    public JsObject ToJson() => Microsoft.TeamFoundation.Server.WebAccess.Agile.JsonExtensions.ToJson(this);

    public MemberLimit Limits { get; set; }
  }
}

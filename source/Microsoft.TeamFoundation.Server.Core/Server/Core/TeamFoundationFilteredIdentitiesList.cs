// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.TeamFoundationFilteredIdentitiesList
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.VisualStudio.Services.Identity;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Server.Core
{
  public class TeamFoundationFilteredIdentitiesList
  {
    private int m_totalItems;

    public TeamFoundationIdentity[] Items { get; set; }

    [XmlAttribute("HasMoreItems")]
    public bool HasMoreItems { get; set; }

    [XmlAttribute("TotalItems")]
    public int TotalItems
    {
      get => this.HasMoreItems ? this.m_totalItems : this.Items.Length;
      set => this.m_totalItems = value;
    }

    [XmlAttribute("StartingIndex")]
    public int StartingIndex { get; set; }

    public static TeamFoundationFilteredIdentitiesList Convert(FilteredIdentitiesList filteredList) => new TeamFoundationFilteredIdentitiesList()
    {
      Items = IdentityUtil.Convert((IList<Microsoft.VisualStudio.Services.Identity.Identity>) filteredList.Items),
      HasMoreItems = filteredList.HasMoreItems,
      TotalItems = filteredList.TotalItems,
      StartingIndex = filteredList.StartingIndex
    };
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.FilteredIdentitiesList
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.ComponentModel;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  public sealed class FilteredIdentitiesList
  {
    private bool m_hasMoreItems;
    internal TeamFoundationIdentity[] m_items = Helper.ZeroLengthArrayOfTeamFoundationIdentity;
    private int m_startingIndex;
    private int m_totalItems;

    private FilteredIdentitiesList()
    {
    }

    public bool HasMoreItems
    {
      get => this.m_hasMoreItems;
      set => this.m_hasMoreItems = value;
    }

    public TeamFoundationIdentity[] Items
    {
      get => (TeamFoundationIdentity[]) this.m_items.Clone();
      set => this.m_items = value;
    }

    public int StartingIndex
    {
      get => this.m_startingIndex;
      set => this.m_startingIndex = value;
    }

    public int TotalItems
    {
      get => this.m_totalItems;
      set => this.m_totalItems = value;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static FilteredIdentitiesList FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      FilteredIdentitiesList filteredIdentitiesList = new FilteredIdentitiesList();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          switch (reader.Name)
          {
            case "HasMoreItems":
              filteredIdentitiesList.m_hasMoreItems = XmlUtility.BooleanFromXmlAttribute(reader);
              continue;
            case "StartingIndex":
              filteredIdentitiesList.m_startingIndex = XmlUtility.Int32FromXmlAttribute(reader);
              continue;
            case "TotalItems":
              filteredIdentitiesList.m_totalItems = XmlUtility.Int32FromXmlAttribute(reader);
              continue;
            default:
              continue;
          }
        }
      }
      reader.Read();
      if (!isEmptyElement)
      {
        while (reader.NodeType == XmlNodeType.Element)
        {
          if (reader.Name == "Items")
            filteredIdentitiesList.m_items = Helper.ArrayOfTeamFoundationIdentityFromXml(serviceProvider, reader, false);
          else
            reader.ReadOuterXml();
        }
        reader.ReadEndElement();
      }
      return filteredIdentitiesList;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("FilteredIdentitiesList instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  HasMoreItems: " + this.m_hasMoreItems.ToString());
      stringBuilder.AppendLine("  Items: " + Helper.ArrayToString<TeamFoundationIdentity>(this.m_items));
      stringBuilder.AppendLine("  StartingIndex: " + this.m_startingIndex.ToString());
      stringBuilder.AppendLine("  TotalItems: " + this.m_totalItems.ToString());
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_hasMoreItems)
        XmlUtility.ToXmlAttribute(writer, "HasMoreItems", this.m_hasMoreItems);
      if (this.m_startingIndex != 0)
        XmlUtility.ToXmlAttribute(writer, "StartingIndex", this.m_startingIndex);
      if (this.m_totalItems != 0)
        XmlUtility.ToXmlAttribute(writer, "TotalItems", this.m_totalItems);
      Helper.ToXml(writer, "Items", this.m_items, false, false);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, FilteredIdentitiesList obj) => obj.ToXml(writer, element);
  }
}

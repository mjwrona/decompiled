// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.SecurityNamespaceDescription
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  public sealed class SecurityNamespaceDescription
  {
    internal ActionDefinition[] m_actions = Helper.ZeroLengthArrayOfActionDefinition;
    private string m_databaseCategory;
    private string m_displayName;
    private int m_elementLength;
    private string m_name;
    private Guid m_namespaceId = Guid.Empty;
    private int m_readPermission;
    private char m_separatorValue;
    private int m_structureValue;
    private int m_writePermission;
    private List<ActionDefinition> m_localActions;

    private SecurityNamespaceDescription()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static SecurityNamespaceDescription FromXml(
      IServiceProvider serviceProvider,
      XmlReader reader)
    {
      SecurityNamespaceDescription namespaceDescription = new SecurityNamespaceDescription();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          string name = reader.Name;
          if (name != null)
          {
            switch (name.Length)
            {
              case 4:
                if (name == "name")
                {
                  namespaceDescription.m_name = XmlUtility.StringFromXmlAttribute(reader);
                  continue;
                }
                continue;
              case 9:
                switch (name[1])
                {
                  case 'e':
                    if (name == "separator")
                    {
                      namespaceDescription.m_separatorValue = XmlUtility.CharFromXmlAttribute(reader);
                      continue;
                    }
                    continue;
                  case 't':
                    if (name == "structure")
                    {
                      namespaceDescription.m_structureValue = XmlUtility.Int32FromXmlAttribute(reader);
                      continue;
                    }
                    continue;
                  default:
                    continue;
                }
              case 11:
                switch (name[0])
                {
                  case 'd':
                    if (name == "displayName")
                    {
                      namespaceDescription.m_displayName = XmlUtility.StringFromXmlAttribute(reader);
                      continue;
                    }
                    continue;
                  case 'n':
                    if (name == "namespaceId")
                    {
                      namespaceDescription.m_namespaceId = XmlUtility.GuidFromXmlAttribute(reader);
                      continue;
                    }
                    continue;
                  default:
                    continue;
                }
              case 13:
                if (name == "elementLength")
                {
                  namespaceDescription.m_elementLength = XmlUtility.Int32FromXmlAttribute(reader);
                  continue;
                }
                continue;
              case 14:
                if (name == "readPermission")
                {
                  namespaceDescription.m_readPermission = XmlUtility.Int32FromXmlAttribute(reader);
                  continue;
                }
                continue;
              case 15:
                if (name == "writePermission")
                {
                  namespaceDescription.m_writePermission = XmlUtility.Int32FromXmlAttribute(reader);
                  continue;
                }
                continue;
              case 16:
                if (name == "databaseCategory")
                {
                  namespaceDescription.m_databaseCategory = XmlUtility.StringFromXmlAttribute(reader);
                  continue;
                }
                continue;
              default:
                continue;
            }
          }
        }
      }
      reader.Read();
      if (!isEmptyElement)
      {
        while (reader.NodeType == XmlNodeType.Element)
        {
          if (reader.Name == "Actions")
            namespaceDescription.m_actions = Helper.ArrayOfActionDefinitionFromXml(serviceProvider, reader, false);
          else
            reader.ReadOuterXml();
        }
        reader.ReadEndElement();
      }
      namespaceDescription.InitializeFromWebServiceDeserialization();
      return namespaceDescription;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("SecurityNamespaceDescription instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  Actions: " + Helper.ArrayToString<ActionDefinition>(this.m_actions));
      stringBuilder.AppendLine("  DatabaseCategory: " + this.m_databaseCategory);
      stringBuilder.AppendLine("  DisplayName: " + this.m_displayName);
      stringBuilder.AppendLine("  ElementLength: " + this.m_elementLength.ToString());
      stringBuilder.AppendLine("  Name: " + this.m_name);
      stringBuilder.AppendLine("  NamespaceId: " + this.m_namespaceId.ToString());
      stringBuilder.AppendLine("  ReadPermission: " + this.m_readPermission.ToString());
      stringBuilder.AppendLine("  SeparatorValue: " + this.m_separatorValue.ToString());
      stringBuilder.AppendLine("  StructureValue: " + this.m_structureValue.ToString());
      stringBuilder.AppendLine("  WritePermission: " + this.m_writePermission.ToString());
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      this.PrepareForWebServiceSerialization();
      writer.WriteStartElement(element);
      if (this.m_databaseCategory != null)
        XmlUtility.ToXmlAttribute(writer, "databaseCategory", this.m_databaseCategory);
      if (this.m_displayName != null)
        XmlUtility.ToXmlAttribute(writer, "displayName", this.m_displayName);
      if (this.m_elementLength != 0)
        XmlUtility.ToXmlAttribute(writer, "elementLength", this.m_elementLength);
      if (this.m_name != null)
        XmlUtility.ToXmlAttribute(writer, "name", this.m_name);
      if (this.m_namespaceId != Guid.Empty)
        XmlUtility.ToXmlAttribute(writer, "namespaceId", this.m_namespaceId);
      if (this.m_readPermission != 0)
        XmlUtility.ToXmlAttribute(writer, "readPermission", this.m_readPermission);
      if (this.m_separatorValue != char.MinValue)
        XmlUtility.ToXmlAttribute(writer, "separator", this.m_separatorValue);
      if (this.m_structureValue != 0)
        XmlUtility.ToXmlAttribute(writer, "structure", this.m_structureValue);
      if (this.m_writePermission != 0)
        XmlUtility.ToXmlAttribute(writer, "writePermission", this.m_writePermission);
      Helper.ToXml(writer, "Actions", this.m_actions, false, false);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, SecurityNamespaceDescription obj) => obj.ToXml(writer, element);

    public SecurityNamespaceDescription(
      Guid namespaceId,
      string name,
      string displayName,
      string databaseCategory,
      char separatorValue,
      int elementLength,
      SecurityNamespaceStructure structure,
      int writePermission,
      int readPermission,
      IEnumerable<ActionDefinition> actions)
    {
      this.m_namespaceId = namespaceId;
      this.m_name = name;
      this.m_displayName = displayName;
      this.m_separatorValue = separatorValue;
      this.m_elementLength = elementLength;
      this.m_structureValue = (int) structure;
      this.m_readPermission = readPermission;
      this.m_writePermission = writePermission;
      this.m_databaseCategory = databaseCategory;
      if (actions != null)
        this.m_localActions = new List<ActionDefinition>(actions);
      else
        this.m_localActions = new List<ActionDefinition>();
    }

    public SecurityNamespaceStructure NamespaceStructure => (SecurityNamespaceStructure) this.m_structureValue;

    public string DisplayName => string.IsNullOrEmpty(this.m_displayName) ? this.Name : this.m_displayName;

    public Guid NamespaceId => this.m_namespaceId;

    public string Name => this.m_name;

    public char SeparatorValue => this.m_separatorValue;

    public int ElementLength => this.m_elementLength;

    public int WritePermission => this.m_writePermission;

    public int ReadPermission => this.m_readPermission;

    public string DatabaseCategory => this.m_databaseCategory;

    public ReadOnlyCollection<ActionDefinition> Actions => this.m_localActions.AsReadOnly();

    public int GetBitmaskForAction(string actionName)
    {
      foreach (ActionDefinition localAction in this.m_localActions)
      {
        if (StringComparer.OrdinalIgnoreCase.Equals(localAction.Name, actionName))
          return localAction.Bit;
      }
      return 0;
    }

    public string GetActionNameForBitmask(int bitmask)
    {
      foreach (ActionDefinition localAction in this.m_localActions)
      {
        if (localAction.Bit == bitmask)
          return localAction.Name;
      }
      return string.Empty;
    }

    public string GetActionDisplayNameForBitmask(int bitmask)
    {
      foreach (ActionDefinition localAction in this.m_localActions)
      {
        if (localAction.Bit == bitmask)
          return localAction.DisplayName;
      }
      return string.Empty;
    }

    internal SecurityNamespaceDescription Clone() => new SecurityNamespaceDescription(this.NamespaceId, this.Name, this.DisplayName, this.DatabaseCategory, this.SeparatorValue, this.ElementLength, this.NamespaceStructure, this.WritePermission, this.ReadPermission, this.Actions == null ? (IEnumerable<ActionDefinition>) new List<ActionDefinition>() : (IEnumerable<ActionDefinition>) new List<ActionDefinition>((IEnumerable<ActionDefinition>) this.Actions));

    private void PrepareForWebServiceSerialization() => this.m_actions = this.m_localActions.ToArray();

    private void InitializeFromWebServiceDeserialization() => this.m_localActions = this.m_actions != null ? new List<ActionDefinition>((IEnumerable<ActionDefinition>) this.m_actions) : new List<ActionDefinition>();
  }
}

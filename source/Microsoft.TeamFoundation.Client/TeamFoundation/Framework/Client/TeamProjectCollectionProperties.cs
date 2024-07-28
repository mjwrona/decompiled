// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.TeamProjectCollectionProperties
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  internal sealed class TeamProjectCollectionProperties
  {
    private string m_authority;
    internal KeyValueOfStringString[] m_databaseCategoryConnectionStringsValue = Helper.ZeroLengthArrayOfKeyValueOfStringString;
    private int m_databaseId;
    private string m_defaultConnectionString;
    private string m_description;
    private Guid m_id = Guid.Empty;
    private bool m_isDefault;
    private string m_name;
    private bool m_registered;
    internal ServicingJobDetail[] m_servicingDetails = Helper.ZeroLengthArrayOfServicingJobDetail;
    internal KeyValueOfStringString[] m_servicingTokensValue = Helper.ZeroLengthArrayOfKeyValueOfStringString;
    private int m_stateValue;
    private string m_virtualDirectory;

    public TeamProjectCollectionProperties(
      Guid id,
      string name,
      string description,
      bool isDefault,
      string virtualDirectory,
      TeamFoundationServiceHostStatus state,
      IDictionary<string, string> servicingTokens,
      string defaultConnectionString,
      IDictionary<string, string> databaseCategoryConnectionStrings)
    {
      this.m_id = id;
      this.m_name = name;
      this.m_description = description;
      this.m_isDefault = isDefault;
      this.m_virtualDirectory = virtualDirectory;
      this.m_stateValue = (int) state;
      this.m_servicingTokensValue = TeamProjectCollectionProperties.ConvertDictionaryToPairs(servicingTokens);
      this.m_defaultConnectionString = defaultConnectionString;
      this.m_databaseCategoryConnectionStringsValue = TeamProjectCollectionProperties.ConvertDictionaryToPairs(databaseCategoryConnectionStrings);
    }

    public TeamFoundationServiceHostStatus State
    {
      get => (TeamFoundationServiceHostStatus) this.m_stateValue;
      set => this.m_stateValue = (int) value;
    }

    private static KeyValueOfStringString[] ConvertDictionaryToPairs(IDictionary<string, string> map)
    {
      KeyValueOfStringString[] pairs;
      if (map == null)
      {
        pairs = Helper.ZeroLengthArrayOfKeyValueOfStringString;
      }
      else
      {
        pairs = new KeyValueOfStringString[map.Count];
        int num = 0;
        foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) map)
          pairs[num++] = new KeyValueOfStringString(keyValuePair);
      }
      return pairs;
    }

    internal void SetDatabaseCategoryConnectionStrings(IDictionary<string, string> connectionStrings)
    {
      if (connectionStrings == null)
      {
        this.m_databaseCategoryConnectionStringsValue = Helper.ZeroLengthArrayOfKeyValueOfStringString;
      }
      else
      {
        KeyValueOfStringString[] valueOfStringStringArray = new KeyValueOfStringString[connectionStrings.Count];
        int num = 0;
        foreach (KeyValuePair<string, string> connectionString in (IEnumerable<KeyValuePair<string, string>>) connectionStrings)
          valueOfStringStringArray[num++] = new KeyValueOfStringString(connectionString);
        this.m_databaseCategoryConnectionStringsValue = valueOfStringStringArray;
      }
    }

    private TeamProjectCollectionProperties()
    {
    }

    public string Authority
    {
      get => this.m_authority;
      set => this.m_authority = value;
    }

    public int DatabaseId
    {
      get => this.m_databaseId;
      set => this.m_databaseId = value;
    }

    public string DefaultConnectionString
    {
      get => this.m_defaultConnectionString;
      set => this.m_defaultConnectionString = value;
    }

    public string Description
    {
      get => this.m_description;
      set => this.m_description = value;
    }

    public Guid Id
    {
      get => this.m_id;
      set => this.m_id = value;
    }

    public bool IsDefault
    {
      get => this.m_isDefault;
      set => this.m_isDefault = value;
    }

    public string Name
    {
      get => this.m_name;
      set => this.m_name = value;
    }

    public bool Registered
    {
      get => this.m_registered;
      internal set => this.m_registered = value;
    }

    public ServicingJobDetail[] ServicingDetails => (ServicingJobDetail[]) this.m_servicingDetails.Clone();

    public int StateValue
    {
      get => this.m_stateValue;
      set => this.m_stateValue = value;
    }

    public string VirtualDirectory
    {
      get => this.m_virtualDirectory;
      set => this.m_virtualDirectory = value;
    }

    internal static TeamProjectCollectionProperties FromXml(
      IServiceProvider serviceProvider,
      XmlReader reader)
    {
      TeamProjectCollectionProperties collectionProperties = new TeamProjectCollectionProperties();
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
              case 2:
                if (name == "id")
                {
                  collectionProperties.m_id = XmlUtility.GuidFromXmlAttribute(reader);
                  continue;
                }
                continue;
              case 4:
                switch (name[0])
                {
                  case 'n':
                    if (name == "name")
                    {
                      collectionProperties.m_name = XmlUtility.StringFromXmlAttribute(reader);
                      continue;
                    }
                    continue;
                  case 'v':
                    if (name == "vdir")
                    {
                      collectionProperties.m_virtualDirectory = XmlUtility.StringFromXmlAttribute(reader);
                      continue;
                    }
                    continue;
                  default:
                    continue;
                }
              case 5:
                if (name == "state")
                {
                  collectionProperties.m_stateValue = XmlUtility.Int32FromXmlAttribute(reader);
                  continue;
                }
                continue;
              case 7:
                if (name == "default")
                {
                  collectionProperties.m_isDefault = XmlUtility.BooleanFromXmlAttribute(reader);
                  continue;
                }
                continue;
              case 9:
                if (name == "authority")
                {
                  collectionProperties.m_authority = XmlUtility.StringFromXmlAttribute(reader);
                  continue;
                }
                continue;
              case 10:
                switch (name[0])
                {
                  case 'D':
                    if (name == "DatabaseId")
                    {
                      collectionProperties.m_databaseId = XmlUtility.Int32FromXmlAttribute(reader);
                      continue;
                    }
                    continue;
                  case 'r':
                    if (name == "registered")
                    {
                      collectionProperties.m_registered = XmlUtility.BooleanFromXmlAttribute(reader);
                      continue;
                    }
                    continue;
                  default:
                    continue;
                }
              case 23:
                if (name == "DefaultConnectionString")
                {
                  collectionProperties.m_defaultConnectionString = XmlUtility.StringFromXmlAttribute(reader);
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
          switch (reader.Name)
          {
            case "DatabaseCategoryConnectionStrings":
              collectionProperties.m_databaseCategoryConnectionStringsValue = Helper.ArrayOfKeyValueOfStringStringFromXml(serviceProvider, reader, false);
              continue;
            case "Description":
              collectionProperties.m_description = XmlUtility.StringFromXmlElement(reader);
              continue;
            case "ServicingDetails":
              collectionProperties.m_servicingDetails = Helper.ArrayOfServicingJobDetailFromXml(serviceProvider, reader, false);
              continue;
            case "ServicingTokens":
              collectionProperties.m_servicingTokensValue = Helper.ArrayOfKeyValueOfStringStringFromXml(serviceProvider, reader, false);
              continue;
            default:
              reader.ReadOuterXml();
              continue;
          }
        }
        reader.ReadEndElement();
      }
      return collectionProperties;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("TeamProjectCollectionProperties instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  Authority: " + this.m_authority);
      stringBuilder.AppendLine("  DatabaseCategoryConnectionStringsValue: " + Helper.ArrayToString<KeyValueOfStringString>(this.m_databaseCategoryConnectionStringsValue));
      stringBuilder.AppendLine("  DatabaseId: " + this.m_databaseId.ToString());
      stringBuilder.AppendLine("  DefaultConnectionString: " + this.m_defaultConnectionString);
      stringBuilder.AppendLine("  Description: " + this.m_description);
      stringBuilder.AppendLine("  Id: " + this.m_id.ToString());
      stringBuilder.AppendLine("  IsDefault: " + this.m_isDefault.ToString());
      stringBuilder.AppendLine("  Name: " + this.m_name);
      stringBuilder.AppendLine("  Registered: " + this.m_registered.ToString());
      stringBuilder.AppendLine("  ServicingDetails: " + Helper.ArrayToString<ServicingJobDetail>(this.m_servicingDetails));
      stringBuilder.AppendLine("  ServicingTokensValue: " + Helper.ArrayToString<KeyValueOfStringString>(this.m_servicingTokensValue));
      stringBuilder.AppendLine("  StateValue: " + this.m_stateValue.ToString());
      stringBuilder.AppendLine("  VirtualDirectory: " + this.m_virtualDirectory);
      return stringBuilder.ToString();
    }

    internal void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_authority != null)
        XmlUtility.ToXmlAttribute(writer, "authority", this.m_authority);
      if (this.m_databaseId != 0)
        XmlUtility.ToXmlAttribute(writer, "DatabaseId", this.m_databaseId);
      if (this.m_defaultConnectionString != null)
        XmlUtility.ToXmlAttribute(writer, "DefaultConnectionString", this.m_defaultConnectionString);
      if (this.m_id != Guid.Empty)
        XmlUtility.ToXmlAttribute(writer, "id", this.m_id);
      if (this.m_isDefault)
        XmlUtility.ToXmlAttribute(writer, "default", this.m_isDefault);
      if (this.m_name != null)
        XmlUtility.ToXmlAttribute(writer, "name", this.m_name);
      if (this.m_registered)
        XmlUtility.ToXmlAttribute(writer, "registered", this.m_registered);
      if (this.m_stateValue != 0)
        XmlUtility.ToXmlAttribute(writer, "state", this.m_stateValue);
      if (this.m_virtualDirectory != null)
        XmlUtility.ToXmlAttribute(writer, "vdir", this.m_virtualDirectory);
      Helper.ToXml(writer, "DatabaseCategoryConnectionStrings", this.m_databaseCategoryConnectionStringsValue, false, false);
      if (this.m_description != null)
        XmlUtility.ToXmlElement(writer, "Description", this.m_description);
      Helper.ToXml(writer, "ServicingDetails", this.m_servicingDetails, false, false);
      Helper.ToXml(writer, "ServicingTokens", this.m_servicingTokensValue, false, false);
      writer.WriteEndElement();
    }

    internal static void ToXml(
      XmlWriter writer,
      string element,
      TeamProjectCollectionProperties obj)
    {
      obj.ToXml(writer, element);
    }
  }
}

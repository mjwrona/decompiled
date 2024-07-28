// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.RegistrationDatabase
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Server;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  internal sealed class RegistrationDatabase
  {
    private string m_connectionString;
    private string m_databaseName;
    private bool m_excludeFromBackup;
    private string m_name;
    private string m_sQLServerName;

    public string ConnectionString
    {
      get => this.m_connectionString;
      set => this.m_connectionString = value;
    }

    public string DatabaseName
    {
      get => this.m_databaseName;
      set => this.m_databaseName = value;
    }

    public bool ExcludeFromBackup
    {
      get => this.m_excludeFromBackup;
      set => this.m_excludeFromBackup = value;
    }

    public string Name
    {
      get => this.m_name;
      set => this.m_name = value;
    }

    public string SQLServerName
    {
      get => this.m_sQLServerName;
      set => this.m_sQLServerName = value;
    }

    internal static RegistrationDatabase FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      RegistrationDatabase registrationDatabase = new RegistrationDatabase();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          string name = reader.Name;
        }
      }
      reader.Read();
      if (!isEmptyElement)
      {
        while (reader.NodeType == XmlNodeType.Element)
        {
          switch (reader.Name)
          {
            case "ConnectionString":
              registrationDatabase.m_connectionString = XmlUtility.StringFromXmlElement(reader);
              continue;
            case "DatabaseName":
              registrationDatabase.m_databaseName = XmlUtility.StringFromXmlElement(reader);
              continue;
            case "ExcludeFromBackup":
              registrationDatabase.m_excludeFromBackup = XmlUtility.BooleanFromXmlElement(reader);
              continue;
            case "Name":
              registrationDatabase.m_name = XmlUtility.StringFromXmlElement(reader);
              continue;
            case "SQLServerName":
              registrationDatabase.m_sQLServerName = XmlUtility.StringFromXmlElement(reader);
              continue;
            default:
              reader.ReadOuterXml();
              continue;
          }
        }
        reader.ReadEndElement();
      }
      return registrationDatabase;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("RegistrationDatabase instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  ConnectionString: " + this.m_connectionString);
      stringBuilder.AppendLine("  DatabaseName: " + this.m_databaseName);
      stringBuilder.AppendLine("  ExcludeFromBackup: " + this.m_excludeFromBackup.ToString());
      stringBuilder.AppendLine("  Name: " + this.m_name);
      stringBuilder.AppendLine("  SQLServerName: " + this.m_sQLServerName);
      return stringBuilder.ToString();
    }

    internal void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_connectionString != null)
        XmlUtility.ToXmlElement(writer, "ConnectionString", this.m_connectionString);
      if (this.m_databaseName != null)
        XmlUtility.ToXmlElement(writer, "DatabaseName", this.m_databaseName);
      if (this.m_excludeFromBackup)
        XmlUtility.ToXmlElement(writer, "ExcludeFromBackup", this.m_excludeFromBackup);
      if (this.m_name != null)
        XmlUtility.ToXmlElement(writer, "Name", this.m_name);
      if (this.m_sQLServerName != null)
        XmlUtility.ToXmlElement(writer, "SQLServerName", this.m_sQLServerName);
      writer.WriteEndElement();
    }

    internal static void ToXml(XmlWriter writer, string element, RegistrationDatabase obj) => obj.ToXml(writer, element);

    internal static Database[] Convert(RegistrationDatabase[] databases)
    {
      if (databases == null || databases.Length == 0)
        return Array.Empty<Database>();
      Database[] databaseArray = new Database[databases.Length];
      for (int index = 0; index < databases.Length; ++index)
      {
        if (databases[index] != null)
          databaseArray[index] = databases[index].ToDatabase();
      }
      return databaseArray;
    }

    internal Database ToDatabase() => new Database(this.Name, this.DatabaseName, this.SQLServerName, this.ConnectionString, this.ExcludeFromBackup);
  }
}

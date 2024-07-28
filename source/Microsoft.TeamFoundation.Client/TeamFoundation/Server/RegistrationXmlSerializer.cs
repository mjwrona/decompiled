// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.RegistrationXmlSerializer
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client.Internal;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Globalization;
using System.Xml;

namespace Microsoft.TeamFoundation.Server
{
  public sealed class RegistrationXmlSerializer
  {
    internal const string DatabasesNode = "Databases";
    internal const string DatabaseNode = "Database";
    internal const string PropertyName = "Name";
    internal const string PropertyDatabaseName = "DatabaseName";
    internal const string PropertyDatabaseCategory = "DatabaseCategory";
    internal const string PropertySQLServerName = "SQLServerName";
    internal const string PropertyConnectionString = "ConnectionString";
    internal const string PropertyExcludeFromBackup = "ExcludeFromBackup";

    public static void Serialize(RegistrationEntry registrationEntry, XmlWriter xmlWriter)
    {
      ArgumentUtility.CheckForNull<RegistrationEntry>(registrationEntry, nameof (registrationEntry));
      ArgumentUtility.CheckForNull<XmlWriter>(xmlWriter, nameof (xmlWriter));
      xmlWriter.WriteStartElement("RegistrationEntry");
      xmlWriter.WriteElementString("Type", registrationEntry.Type);
      xmlWriter.WriteElementString("ChangeType", "NoChange");
      xmlWriter.WriteStartElement("ServiceInterfaces");
      foreach (ServiceInterface serviceInterface in registrationEntry.ServiceInterfaces)
        RegistrationXmlSerializer.Serialize(serviceInterface, xmlWriter);
      xmlWriter.WriteEndElement();
      xmlWriter.WriteStartElement("Databases");
      foreach (Database database in registrationEntry.Databases)
        RegistrationXmlSerializer.Serialize(database, xmlWriter);
      xmlWriter.WriteEndElement();
      xmlWriter.WriteStartElement("EventTypes");
      foreach (EventType eventType in registrationEntry.EventTypes)
        RegistrationXmlSerializer.Serialize(eventType, xmlWriter);
      xmlWriter.WriteEndElement();
      xmlWriter.WriteStartElement("RegistrationExtendedAttributes");
      foreach (RegistrationExtendedAttribute extendedAttribute in registrationEntry.RegistrationExtendedAttributes)
        RegistrationXmlSerializer.Serialize(extendedAttribute, xmlWriter);
      xmlWriter.WriteEndElement();
      xmlWriter.WriteStartElement("ArtifactTypes");
      foreach (ArtifactType artifactType in registrationEntry.ArtifactTypes)
        RegistrationXmlSerializer.Serialize(artifactType, xmlWriter);
      xmlWriter.WriteEndElement();
      xmlWriter.WriteEndElement();
    }

    public static object Deserialize(XmlNode root, Type type)
    {
      ArgumentUtility.CheckForNull<XmlNode>(root, nameof (root));
      ArgumentUtility.CheckForNull<Type>(type, nameof (type));
      if (type == typeof (ServiceInterface))
        return (object) new ServiceInterface()
        {
          Name = RegistrationXmlSerializer.GetTerminalNodeValue(root, "Name"),
          Url = RegistrationXmlSerializer.GetTerminalNodeValue(root, "Url")
        };
      if (type == typeof (Database))
      {
        Database database = new Database();
        database.Name = RegistrationXmlSerializer.GetTerminalNodeValue(root, "Name");
        database.DatabaseName = RegistrationXmlSerializer.GetTerminalNodeValue(root, "DatabaseName", false);
        if (database.DatabaseName == null)
          database.DatabaseName = RegistrationXmlSerializer.GetTerminalNodeValue(root, "DatabaseCategory");
        database.SQLServerName = RegistrationXmlSerializer.GetTerminalNodeValue(root, "SQLServerName", false);
        database.ConnectionString = RegistrationXmlSerializer.GetTerminalNodeValue(root, "ConnectionString", false);
        XmlNode xmlNode = root.SelectSingleNode("ExcludeFromBackup");
        bool result;
        database.ExcludeFromBackup = xmlNode != null && (!bool.TryParse(xmlNode.InnerText.Trim(), out result) || result);
        return (object) database;
      }
      if (type == typeof (EventType))
        return (object) new EventType()
        {
          Name = RegistrationXmlSerializer.GetTerminalNodeValue(root, "Name"),
          Schema = RegistrationXmlSerializer.GetTerminalNodeValue(root, "Schema")
        };
      if (type == typeof (RegistrationExtendedAttribute))
        return (object) new RegistrationExtendedAttribute()
        {
          Name = RegistrationXmlSerializer.GetTerminalNodeValue(root, "Name"),
          Value = RegistrationXmlSerializer.GetTerminalNodeValue(root, "Value")
        };
      if (type == typeof (OutboundLinkType))
        return (object) new OutboundLinkType()
        {
          Name = RegistrationXmlSerializer.GetTerminalNodeValue(root, "Name"),
          TargetArtifactTypeTool = RegistrationXmlSerializer.GetTerminalNodeValue(root, "TargetArtifactTypeTool"),
          TargetArtifactTypeName = RegistrationXmlSerializer.GetTerminalNodeValue(root, "TargetArtifactTypeName")
        };
      if (type == typeof (ArtifactType))
      {
        ArtifactType artifactType = new ArtifactType();
        artifactType.Name = RegistrationXmlSerializer.GetTerminalNodeValue(root, "Name");
        ArrayList arrayList = new ArrayList();
        foreach (XmlNode selectNode in root.SelectNodes("OutboundLinkTypes/OutboundLinkType"))
        {
          OutboundLinkType outboundLinkType = (OutboundLinkType) RegistrationXmlSerializer.Deserialize(selectNode, typeof (OutboundLinkType));
          arrayList.Add((object) outboundLinkType);
        }
        artifactType.OutboundLinkTypes = (OutboundLinkType[]) arrayList.ToArray(typeof (OutboundLinkType));
        return (object) artifactType;
      }
      if (!(type == typeof (RegistrationEntry)))
        throw new ArgumentException(ClientResources.TypeCannotSerializeMessage((object) type), nameof (type));
      RegistrationEntry registrationEntry = new RegistrationEntry();
      registrationEntry.Type = RegistrationXmlSerializer.GetTerminalNodeValue(root, "Type");
      ArrayList arrayList1 = new ArrayList();
      foreach (XmlNode selectNode in root.SelectNodes("ServiceInterfaces/ServiceInterface"))
      {
        ServiceInterface serviceInterface = (ServiceInterface) RegistrationXmlSerializer.Deserialize(selectNode, typeof (ServiceInterface));
        arrayList1.Add((object) serviceInterface);
      }
      registrationEntry.ServiceInterfaces = (ServiceInterface[]) arrayList1.ToArray(typeof (ServiceInterface));
      ArrayList arrayList2 = new ArrayList();
      foreach (XmlNode selectNode in root.SelectNodes("Databases/Database"))
        arrayList2.Add((object) (Database) RegistrationXmlSerializer.Deserialize(selectNode, typeof (Database)));
      registrationEntry.Databases = (Database[]) arrayList2.ToArray(typeof (Database));
      ArrayList arrayList3 = new ArrayList();
      foreach (XmlNode selectNode in root.SelectNodes("EventTypes/EventType"))
      {
        EventType eventType = (EventType) RegistrationXmlSerializer.Deserialize(selectNode, typeof (EventType));
        arrayList3.Add((object) eventType);
      }
      registrationEntry.EventTypes = (EventType[]) arrayList3.ToArray(typeof (EventType));
      ArrayList arrayList4 = new ArrayList();
      foreach (XmlNode selectNode in root.SelectNodes("RegistrationExtendedAttributes/RegistrationExtendedAttribute"))
      {
        RegistrationExtendedAttribute extendedAttribute = (RegistrationExtendedAttribute) RegistrationXmlSerializer.Deserialize(selectNode, typeof (RegistrationExtendedAttribute));
        arrayList4.Add((object) extendedAttribute);
      }
      registrationEntry.RegistrationExtendedAttributes = (RegistrationExtendedAttribute[]) arrayList4.ToArray(typeof (RegistrationExtendedAttribute));
      ArrayList arrayList5 = new ArrayList();
      foreach (XmlNode selectNode in root.SelectNodes("ArtifactTypes/ArtifactType"))
      {
        ArtifactType artifactType = (ArtifactType) RegistrationXmlSerializer.Deserialize(selectNode, typeof (ArtifactType));
        arrayList5.Add((object) artifactType);
      }
      registrationEntry.ArtifactTypes = (ArtifactType[]) arrayList5.ToArray(typeof (ArtifactType));
      return (object) registrationEntry;
    }

    internal static string GetTerminalNodeValue(XmlNode node, string tag) => RegistrationXmlSerializer.GetTerminalNodeValue(node, tag, true);

    internal static string GetTerminalNodeValue(XmlNode node, string tag, bool throwIfNotFound)
    {
      XmlNode xmlNode = node.SelectSingleNode(tag);
      if (xmlNode == null & throwIfNotFound)
        throw new ArgumentException(ClientResources.NodeDoesNotExistMessage((object) tag, (object) node.Name));
      return xmlNode?.InnerText.Trim();
    }

    private static void Serialize(ServiceInterface si, XmlWriter w)
    {
      w.WriteStartElement("ServiceInterface");
      w.WriteElementString("Name", si.Name);
      w.WriteElementString("Url", si.Url);
      w.WriteEndElement();
    }

    private static void Serialize(Database db, XmlWriter w)
    {
      w.WriteStartElement("Database");
      w.WriteElementString("Name", db.Name);
      w.WriteElementString("DatabaseName", db.DatabaseName);
      w.WriteElementString("SQLServerName", db.SQLServerName);
      w.WriteElementString("ConnectionString", db.ConnectionString);
      if (db.ExcludeFromBackup)
        w.WriteElementString("ExcludeFromBackup", db.ExcludeFromBackup.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      w.WriteEndElement();
    }

    private static void Serialize(EventType et, XmlWriter w)
    {
      w.WriteStartElement("EventType");
      w.WriteElementString("Name", et.Name);
      w.WriteElementString("Schema", et.Schema);
      w.WriteEndElement();
    }

    private static void Serialize(RegistrationExtendedAttribute rea, XmlWriter w)
    {
      w.WriteStartElement("RegistrationExtendedAttribute");
      w.WriteElementString("Name", rea.Name);
      w.WriteElementString("Value", rea.Value);
      w.WriteEndElement();
    }

    private static void Serialize(OutboundLinkType forwardLinkType, XmlWriter w)
    {
      w.WriteStartElement("OutboundLinkType");
      w.WriteElementString("Name", forwardLinkType.Name);
      w.WriteElementString("TargetArtifactTypeTool", forwardLinkType.TargetArtifactTypeTool);
      w.WriteElementString("TargetArtifactTypeName", forwardLinkType.TargetArtifactTypeName);
      w.WriteEndElement();
    }

    private static void Serialize(ArtifactType at, XmlWriter w)
    {
      w.WriteStartElement("ArtifactType");
      w.WriteElementString("Name", at.Name);
      w.WriteStartElement("OutboundLinkTypes");
      foreach (OutboundLinkType outboundLinkType in at.OutboundLinkTypes)
        RegistrationXmlSerializer.Serialize(outboundLinkType, w);
      w.WriteEndElement();
      w.WriteEndElement();
    }
  }
}

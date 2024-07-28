// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.FrameworkRegistrationXmlSerializer
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client.Internal;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Globalization;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  internal sealed class FrameworkRegistrationXmlSerializer
  {
    internal const string DatabasesNode = "Databases";
    internal const string DatabaseNode = "Database";
    internal const string PropertyName = "Name";
    internal const string PropertyDatabaseName = "DatabaseName";
    internal const string PropertyDatabaseCategory = "DatabaseCategory";
    internal const string PropertySQLServerName = "SQLServerName";
    internal const string PropertyConnectionString = "ConnectionString";
    internal const string PropertyExcludeFromBackup = "ExcludeFromBackup";

    public static void Serialize(FrameworkRegistrationEntry registrationEntry, XmlWriter xmlWriter)
    {
      ArgumentUtility.CheckForNull<FrameworkRegistrationEntry>(registrationEntry, nameof (registrationEntry));
      ArgumentUtility.CheckForNull<XmlWriter>(xmlWriter, nameof (xmlWriter));
      xmlWriter.WriteStartElement("RegistrationEntry");
      xmlWriter.WriteElementString("Type", registrationEntry.Type);
      xmlWriter.WriteElementString("ChangeType", "NoChange");
      xmlWriter.WriteStartElement("ServiceInterfaces");
      foreach (RegistrationServiceInterface serviceInterface in registrationEntry.ServiceInterfaces)
        FrameworkRegistrationXmlSerializer.Serialize(serviceInterface, xmlWriter);
      xmlWriter.WriteEndElement();
      xmlWriter.WriteStartElement("Databases");
      foreach (RegistrationDatabase database in registrationEntry.Databases)
        FrameworkRegistrationXmlSerializer.Serialize(database, xmlWriter);
      xmlWriter.WriteEndElement();
      xmlWriter.WriteStartElement("RegistrationExtendedAttributes");
      foreach (RegistrationExtendedAttribute2 extendedAttribute in registrationEntry.RegistrationExtendedAttributes)
        FrameworkRegistrationXmlSerializer.Serialize(extendedAttribute, xmlWriter);
      xmlWriter.WriteEndElement();
      xmlWriter.WriteStartElement("ArtifactTypes");
      foreach (RegistrationArtifactType artifactType in registrationEntry.ArtifactTypes)
        FrameworkRegistrationXmlSerializer.Serialize(artifactType, xmlWriter);
      xmlWriter.WriteEndElement();
      xmlWriter.WriteEndElement();
    }

    public static object Deserialize(XmlNode root, Type type)
    {
      ArgumentUtility.CheckForNull<XmlNode>(root, nameof (root));
      ArgumentUtility.CheckForNull<Type>(type, nameof (type));
      if (type == typeof (RegistrationServiceInterface))
        return (object) new RegistrationServiceInterface()
        {
          Name = FrameworkRegistrationXmlSerializer.GetTerminalNodeValue(root, "Name"),
          Url = FrameworkRegistrationXmlSerializer.GetTerminalNodeValue(root, "Url")
        };
      if (type == typeof (RegistrationDatabase))
      {
        RegistrationDatabase registrationDatabase = new RegistrationDatabase();
        registrationDatabase.Name = FrameworkRegistrationXmlSerializer.GetTerminalNodeValue(root, "Name");
        registrationDatabase.DatabaseName = FrameworkRegistrationXmlSerializer.GetTerminalNodeValue(root, "DatabaseName", false);
        if (registrationDatabase.DatabaseName == null)
          registrationDatabase.DatabaseName = FrameworkRegistrationXmlSerializer.GetTerminalNodeValue(root, "DatabaseCategory");
        registrationDatabase.SQLServerName = FrameworkRegistrationXmlSerializer.GetTerminalNodeValue(root, "SQLServerName", false);
        registrationDatabase.ConnectionString = FrameworkRegistrationXmlSerializer.GetTerminalNodeValue(root, "ConnectionString", false);
        XmlNode xmlNode = root.SelectSingleNode("ExcludeFromBackup");
        bool result;
        registrationDatabase.ExcludeFromBackup = xmlNode != null && (!bool.TryParse(xmlNode.InnerText.Trim(), out result) || result);
        return (object) registrationDatabase;
      }
      if (type == typeof (RegistrationExtendedAttribute2))
        return (object) new RegistrationExtendedAttribute2()
        {
          Name = FrameworkRegistrationXmlSerializer.GetTerminalNodeValue(root, "Name"),
          Value = FrameworkRegistrationXmlSerializer.GetTerminalNodeValue(root, "Value")
        };
      if (type == typeof (OutboundLinkType))
        return (object) new OutboundLinkType()
        {
          Name = FrameworkRegistrationXmlSerializer.GetTerminalNodeValue(root, "Name"),
          TargetArtifactTypeTool = FrameworkRegistrationXmlSerializer.GetTerminalNodeValue(root, "TargetArtifactTypeTool"),
          TargetArtifactTypeName = FrameworkRegistrationXmlSerializer.GetTerminalNodeValue(root, "TargetArtifactTypeName")
        };
      if (type == typeof (RegistrationArtifactType))
      {
        RegistrationArtifactType registrationArtifactType = new RegistrationArtifactType();
        registrationArtifactType.Name = FrameworkRegistrationXmlSerializer.GetTerminalNodeValue(root, "Name");
        ArrayList arrayList = new ArrayList();
        foreach (XmlNode selectNode in root.SelectNodes("OutboundLinkTypes/OutboundLinkType"))
        {
          OutboundLinkType outboundLinkType = (OutboundLinkType) FrameworkRegistrationXmlSerializer.Deserialize(selectNode, typeof (OutboundLinkType));
          arrayList.Add((object) outboundLinkType);
        }
        registrationArtifactType.OutboundLinkTypes = (OutboundLinkType[]) arrayList.ToArray(typeof (OutboundLinkType));
        return (object) registrationArtifactType;
      }
      if (!(type == typeof (FrameworkRegistrationEntry)))
        throw new ArgumentException(ClientResources.TypeCannotSerializeMessage((object) type), nameof (type));
      FrameworkRegistrationEntry registrationEntry = new FrameworkRegistrationEntry();
      registrationEntry.Type = FrameworkRegistrationXmlSerializer.GetTerminalNodeValue(root, "Type");
      ArrayList arrayList1 = new ArrayList();
      foreach (XmlNode selectNode in root.SelectNodes("ServiceInterfaces/ServiceInterface"))
      {
        RegistrationServiceInterface serviceInterface = (RegistrationServiceInterface) FrameworkRegistrationXmlSerializer.Deserialize(selectNode, typeof (RegistrationServiceInterface));
        arrayList1.Add((object) serviceInterface);
      }
      registrationEntry.ServiceInterfaces = (RegistrationServiceInterface[]) arrayList1.ToArray(typeof (RegistrationServiceInterface));
      ArrayList arrayList2 = new ArrayList();
      foreach (XmlNode selectNode in root.SelectNodes("Databases/Database"))
        arrayList2.Add((object) (RegistrationDatabase) FrameworkRegistrationXmlSerializer.Deserialize(selectNode, typeof (RegistrationDatabase)));
      registrationEntry.Databases = (RegistrationDatabase[]) arrayList2.ToArray(typeof (RegistrationDatabase));
      ArrayList arrayList3 = new ArrayList();
      foreach (XmlNode selectNode in root.SelectNodes("RegistrationExtendedAttributes/RegistrationExtendedAttribute"))
      {
        RegistrationExtendedAttribute2 extendedAttribute2 = (RegistrationExtendedAttribute2) FrameworkRegistrationXmlSerializer.Deserialize(selectNode, typeof (RegistrationExtendedAttribute2));
        arrayList3.Add((object) extendedAttribute2);
      }
      registrationEntry.RegistrationExtendedAttributes = (RegistrationExtendedAttribute2[]) arrayList3.ToArray(typeof (RegistrationExtendedAttribute2));
      ArrayList arrayList4 = new ArrayList();
      foreach (XmlNode selectNode in root.SelectNodes("ArtifactTypes/ArtifactType"))
      {
        RegistrationArtifactType registrationArtifactType = (RegistrationArtifactType) FrameworkRegistrationXmlSerializer.Deserialize(selectNode, typeof (RegistrationArtifactType));
        arrayList4.Add((object) registrationArtifactType);
      }
      registrationEntry.ArtifactTypes = (RegistrationArtifactType[]) arrayList4.ToArray(typeof (RegistrationArtifactType));
      return (object) registrationEntry;
    }

    internal static string GetTerminalNodeValue(XmlNode node, string tag) => FrameworkRegistrationXmlSerializer.GetTerminalNodeValue(node, tag, true);

    internal static string GetTerminalNodeValue(XmlNode node, string tag, bool throwIfNotFound)
    {
      XmlNode xmlNode = node.SelectSingleNode(tag);
      if (xmlNode == null & throwIfNotFound)
        throw new ArgumentException(ClientResources.NodeDoesNotExistMessage((object) tag, (object) node.Name));
      return xmlNode?.InnerText.Trim();
    }

    private static void Serialize(RegistrationServiceInterface si, XmlWriter w)
    {
      w.WriteStartElement("ServiceInterface");
      w.WriteElementString("Name", si.Name);
      w.WriteElementString("Url", si.Url);
      w.WriteEndElement();
    }

    private static void Serialize(RegistrationDatabase db, XmlWriter w)
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

    private static void Serialize(RegistrationExtendedAttribute2 rea, XmlWriter w)
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

    private static void Serialize(RegistrationArtifactType at, XmlWriter w)
    {
      w.WriteStartElement("ArtifactType");
      w.WriteElementString("Name", at.Name);
      w.WriteStartElement("OutboundLinkTypes");
      foreach (OutboundLinkType outboundLinkType in at.OutboundLinkTypes)
        FrameworkRegistrationXmlSerializer.Serialize(outboundLinkType, w);
      w.WriteEndElement();
      w.WriteEndElement();
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.FrameworkRegistrationEntry
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
  internal sealed class FrameworkRegistrationEntry
  {
    internal RegistrationArtifactType[] m_artifactTypes = Helper.ZeroLengthArrayOfRegistrationArtifactType;
    internal RegistrationDatabase[] m_databases = Helper.ZeroLengthArrayOfRegistrationDatabase;
    internal RegistrationExtendedAttribute2[] m_registrationExtendedAttributes = Helper.ZeroLengthArrayOfRegistrationExtendedAttribute2;
    internal RegistrationServiceInterface[] m_serviceInterfaces = Helper.ZeroLengthArrayOfRegistrationServiceInterface;
    private string m_type;

    public RegistrationArtifactType[] ArtifactTypes
    {
      get => (RegistrationArtifactType[]) this.m_artifactTypes.Clone();
      set => this.m_artifactTypes = value;
    }

    public RegistrationDatabase[] Databases
    {
      get => (RegistrationDatabase[]) this.m_databases.Clone();
      set => this.m_databases = value;
    }

    public RegistrationExtendedAttribute2[] RegistrationExtendedAttributes
    {
      get => (RegistrationExtendedAttribute2[]) this.m_registrationExtendedAttributes.Clone();
      set => this.m_registrationExtendedAttributes = value;
    }

    public RegistrationServiceInterface[] ServiceInterfaces
    {
      get => (RegistrationServiceInterface[]) this.m_serviceInterfaces.Clone();
      set => this.m_serviceInterfaces = value;
    }

    public string Type
    {
      get => this.m_type;
      set => this.m_type = value;
    }

    internal static FrameworkRegistrationEntry FromXml(
      IServiceProvider serviceProvider,
      XmlReader reader)
    {
      FrameworkRegistrationEntry registrationEntry = new FrameworkRegistrationEntry();
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
            case "ArtifactTypes":
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              registrationEntry.m_artifactTypes = XmlUtility.ArrayOfObjectFromXml<RegistrationArtifactType>(serviceProvider, reader, "ArtifactType", false, FrameworkRegistrationEntry.\u003C\u003EO.\u003C0\u003E__FromXml ?? (FrameworkRegistrationEntry.\u003C\u003EO.\u003C0\u003E__FromXml = new Func<IServiceProvider, XmlReader, RegistrationArtifactType>(RegistrationArtifactType.FromXml)));
              continue;
            case "Databases":
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              registrationEntry.m_databases = XmlUtility.ArrayOfObjectFromXml<RegistrationDatabase>(serviceProvider, reader, "Database", false, FrameworkRegistrationEntry.\u003C\u003EO.\u003C1\u003E__FromXml ?? (FrameworkRegistrationEntry.\u003C\u003EO.\u003C1\u003E__FromXml = new Func<IServiceProvider, XmlReader, RegistrationDatabase>(RegistrationDatabase.FromXml)));
              continue;
            case "RegistrationExtendedAttributes":
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              registrationEntry.m_registrationExtendedAttributes = XmlUtility.ArrayOfObjectFromXml<RegistrationExtendedAttribute2>(serviceProvider, reader, "RegistrationExtendedAttribute", false, FrameworkRegistrationEntry.\u003C\u003EO.\u003C2\u003E__FromXml ?? (FrameworkRegistrationEntry.\u003C\u003EO.\u003C2\u003E__FromXml = new Func<IServiceProvider, XmlReader, RegistrationExtendedAttribute2>(RegistrationExtendedAttribute2.FromXml)));
              continue;
            case "ServiceInterfaces":
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              registrationEntry.m_serviceInterfaces = XmlUtility.ArrayOfObjectFromXml<RegistrationServiceInterface>(serviceProvider, reader, "ServiceInterface", false, FrameworkRegistrationEntry.\u003C\u003EO.\u003C3\u003E__FromXml ?? (FrameworkRegistrationEntry.\u003C\u003EO.\u003C3\u003E__FromXml = new Func<IServiceProvider, XmlReader, RegistrationServiceInterface>(RegistrationServiceInterface.FromXml)));
              continue;
            case "Type":
              registrationEntry.m_type = XmlUtility.StringFromXmlElement(reader);
              continue;
            default:
              reader.ReadOuterXml();
              continue;
          }
        }
        reader.ReadEndElement();
      }
      return registrationEntry;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("FrameworkRegistrationEntry instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  ArtifactTypes: " + Helper.ArrayToString<RegistrationArtifactType>(this.m_artifactTypes));
      stringBuilder.AppendLine("  Databases: " + Helper.ArrayToString<RegistrationDatabase>(this.m_databases));
      stringBuilder.AppendLine("  RegistrationExtendedAttributes: " + Helper.ArrayToString<RegistrationExtendedAttribute2>(this.m_registrationExtendedAttributes));
      stringBuilder.AppendLine("  ServiceInterfaces: " + Helper.ArrayToString<RegistrationServiceInterface>(this.m_serviceInterfaces));
      stringBuilder.AppendLine("  Type: " + this.m_type);
      return stringBuilder.ToString();
    }

    internal void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<RegistrationArtifactType>(writer, this.m_artifactTypes, "ArtifactTypes", "ArtifactType", false, false, FrameworkRegistrationEntry.\u003C\u003EO.\u003C4\u003E__ToXml ?? (FrameworkRegistrationEntry.\u003C\u003EO.\u003C4\u003E__ToXml = new Action<XmlWriter, string, RegistrationArtifactType>(RegistrationArtifactType.ToXml)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<RegistrationDatabase>(writer, this.m_databases, "Databases", "Database", false, false, FrameworkRegistrationEntry.\u003C\u003EO.\u003C5\u003E__ToXml ?? (FrameworkRegistrationEntry.\u003C\u003EO.\u003C5\u003E__ToXml = new Action<XmlWriter, string, RegistrationDatabase>(RegistrationDatabase.ToXml)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<RegistrationExtendedAttribute2>(writer, this.m_registrationExtendedAttributes, "RegistrationExtendedAttributes", "RegistrationExtendedAttribute", false, false, FrameworkRegistrationEntry.\u003C\u003EO.\u003C6\u003E__ToXml ?? (FrameworkRegistrationEntry.\u003C\u003EO.\u003C6\u003E__ToXml = new Action<XmlWriter, string, RegistrationExtendedAttribute2>(RegistrationExtendedAttribute2.ToXml)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<RegistrationServiceInterface>(writer, this.m_serviceInterfaces, "ServiceInterfaces", "ServiceInterface", false, false, FrameworkRegistrationEntry.\u003C\u003EO.\u003C7\u003E__ToXml ?? (FrameworkRegistrationEntry.\u003C\u003EO.\u003C7\u003E__ToXml = new Action<XmlWriter, string, RegistrationServiceInterface>(RegistrationServiceInterface.ToXml)));
      if (this.m_type != null)
        XmlUtility.ToXmlElement(writer, "Type", this.m_type);
      writer.WriteEndElement();
    }

    internal static void ToXml(XmlWriter writer, string element, FrameworkRegistrationEntry obj) => obj.ToXml(writer, element);

    internal static RegistrationEntry[] Convert(FrameworkRegistrationEntry[] registryEntries)
    {
      if (registryEntries == null || registryEntries.Length == 0)
        return Array.Empty<RegistrationEntry>();
      RegistrationEntry[] registrationEntryArray = new RegistrationEntry[registryEntries.Length];
      for (int index = 0; index < registryEntries.Length; ++index)
      {
        if (registryEntries[index] != null)
          registrationEntryArray[index] = registryEntries[index].ToRegistrationEntry();
      }
      return registrationEntryArray;
    }

    internal RegistrationEntry ToRegistrationEntry() => new RegistrationEntry()
    {
      Type = this.Type,
      ServiceInterfaces = RegistrationServiceInterface.Convert(this.ServiceInterfaces),
      Databases = RegistrationDatabase.Convert(this.Databases),
      ArtifactTypes = RegistrationArtifactType.Convert(this.ArtifactTypes),
      RegistrationExtendedAttributes = RegistrationExtendedAttribute2.Convert(this.RegistrationExtendedAttributes)
    };
  }
}

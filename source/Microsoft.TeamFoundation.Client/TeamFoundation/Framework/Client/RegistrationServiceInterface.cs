// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.RegistrationServiceInterface
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
  internal sealed class RegistrationServiceInterface
  {
    private string m_name;
    private string m_url;

    public string Name
    {
      get => this.m_name;
      set => this.m_name = value;
    }

    public string Url
    {
      get => this.m_url;
      set => this.m_url = value;
    }

    internal static RegistrationServiceInterface FromXml(
      IServiceProvider serviceProvider,
      XmlReader reader)
    {
      RegistrationServiceInterface serviceInterface = new RegistrationServiceInterface();
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
            case "Name":
              serviceInterface.m_name = XmlUtility.StringFromXmlElement(reader);
              continue;
            case "Url":
              serviceInterface.m_url = XmlUtility.StringFromXmlElement(reader);
              continue;
            default:
              reader.ReadOuterXml();
              continue;
          }
        }
        reader.ReadEndElement();
      }
      return serviceInterface;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("RegistrationServiceInterface instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  Name: " + this.m_name);
      stringBuilder.AppendLine("  Url: " + this.m_url);
      return stringBuilder.ToString();
    }

    internal void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_name != null)
        XmlUtility.ToXmlElement(writer, "Name", this.m_name);
      if (this.m_url != null)
        XmlUtility.ToXmlElement(writer, "Url", this.m_url);
      writer.WriteEndElement();
    }

    internal static void ToXml(XmlWriter writer, string element, RegistrationServiceInterface obj) => obj.ToXml(writer, element);

    internal static ServiceInterface[] Convert(RegistrationServiceInterface[] serviceInterfaces)
    {
      if (serviceInterfaces == null || serviceInterfaces.Length == 0)
        return Array.Empty<ServiceInterface>();
      ServiceInterface[] serviceInterfaceArray = new ServiceInterface[serviceInterfaces.Length];
      for (int index = 0; index < serviceInterfaces.Length; ++index)
      {
        if (serviceInterfaces[index] != null)
          serviceInterfaceArray[index] = serviceInterfaces[index].ToServiceInterface();
      }
      return serviceInterfaceArray;
    }

    internal ServiceInterface ToServiceInterface() => new ServiceInterface(this.Name, this.Url);
  }
}

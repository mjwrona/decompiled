// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.ServiceIdentityInfo
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.ComponentModel;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  public sealed class ServiceIdentityInfo
  {
    private string m_name;
    private string m_password;

    public ServiceIdentityInfo(string name)
      : this(name, (string) null)
    {
    }

    public ServiceIdentityInfo(string name, string password)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      this.m_name = name;
      this.m_password = password;
    }

    private ServiceIdentityInfo()
    {
    }

    public string Name => this.m_name;

    public string Password => this.m_password;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static ServiceIdentityInfo FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      ServiceIdentityInfo serviceIdentityInfo = new ServiceIdentityInfo();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          switch (reader.Name)
          {
            case "Name":
              serviceIdentityInfo.m_name = XmlUtility.StringFromXmlAttribute(reader);
              continue;
            case "Password":
              serviceIdentityInfo.m_password = XmlUtility.StringFromXmlAttribute(reader);
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
          string name = reader.Name;
          reader.ReadOuterXml();
        }
        reader.ReadEndElement();
      }
      return serviceIdentityInfo;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("ServiceIdentityInfo instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  Name: " + this.m_name);
      stringBuilder.AppendLine("  Password: " + this.m_password);
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_name != null)
        XmlUtility.ToXmlAttribute(writer, "Name", this.m_name);
      if (this.m_password != null)
        XmlUtility.ToXmlAttribute(writer, "Password", this.m_password);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, ServiceIdentityInfo obj) => obj.ToXml(writer, element);
  }
}

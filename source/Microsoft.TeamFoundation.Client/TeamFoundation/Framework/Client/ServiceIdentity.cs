// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.ServiceIdentity
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.ComponentModel;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  public sealed class ServiceIdentity
  {
    private TeamFoundationIdentity m_identity;
    private ServiceIdentityInfo m_identityInfo;

    private ServiceIdentity()
    {
    }

    public TeamFoundationIdentity Identity => this.m_identity;

    public ServiceIdentityInfo IdentityInfo => this.m_identityInfo;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static ServiceIdentity FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      ServiceIdentity serviceIdentity = new ServiceIdentity();
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
            case "Identity":
              serviceIdentity.m_identity = TeamFoundationIdentity.FromXml(serviceProvider, reader);
              continue;
            case "IdentityInfo":
              serviceIdentity.m_identityInfo = ServiceIdentityInfo.FromXml(serviceProvider, reader);
              continue;
            default:
              reader.ReadOuterXml();
              continue;
          }
        }
        reader.ReadEndElement();
      }
      return serviceIdentity;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("ServiceIdentity instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  Identity: " + this.m_identity?.ToString());
      stringBuilder.AppendLine("  IdentityInfo: " + this.m_identityInfo?.ToString());
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, ServiceIdentity obj) => obj.ToXml(writer, element);
  }
}

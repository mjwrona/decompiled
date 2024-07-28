// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.FileRepositoryProperties
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.ComponentModel;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  public sealed class FileRepositoryProperties
  {
    private SigningInfo m_signingInfo;

    internal FileRepositoryProperties()
    {
    }

    public SigningInfo SigningInfo
    {
      get => this.m_signingInfo;
      internal set => this.m_signingInfo = value;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static FileRepositoryProperties FromXml(
      IServiceProvider serviceProvider,
      XmlReader reader)
    {
      FileRepositoryProperties repositoryProperties = new FileRepositoryProperties();
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
          if (reader.Name == "SigningInfo")
            repositoryProperties.m_signingInfo = SigningInfo.FromXml(serviceProvider, reader);
          else
            reader.ReadOuterXml();
        }
        reader.ReadEndElement();
      }
      return repositoryProperties;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("FileRepositoryProperties instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  SigningInfo: " + this.m_signingInfo?.ToString());
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_signingInfo != null)
        SigningInfo.ToXml(writer, "SigningInfo", this.m_signingInfo);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, FileRepositoryProperties obj) => obj.ToXml(writer, element);
  }
}

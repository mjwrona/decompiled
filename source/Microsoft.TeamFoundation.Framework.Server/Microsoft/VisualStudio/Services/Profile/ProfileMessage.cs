// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Profile.ProfileMessage
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.IO;
using System.Text;
using System.Xml;

namespace Microsoft.VisualStudio.Services.Profile
{
  public class ProfileMessage
  {
    private const string c_eventDataRoot = "data";
    private const string c_identifier = "identifier";
    private const string c_subjectDescriptor = "sdesc";
    private const string c_identityDescriptor = "idesc";
    private const string c_containerName = "containerName";
    private const string c_scope = "scope";

    public Guid IdentityId { get; set; }

    public SubjectDescriptor SubjectDescriptor { get; set; }

    public IdentityDescriptor IdentityDescriptor { get; set; }

    public AttributesScope Scope { get; set; }

    public string ContainerName { get; set; }

    public static ProfileMessage FromXml(string xml)
    {
      ProfileMessage profileMessage = new ProfileMessage();
      using (StringReader stringReader = new StringReader(xml))
      {
        StringReader input = stringReader;
        using (XmlReader xmlReader = XmlReader.Create((TextReader) input, new XmlReaderSettings()
        {
          DtdProcessing = DtdProcessing.Prohibit,
          XmlResolver = (XmlResolver) null
        }))
        {
          xmlReader.ReadToFollowing("data");
          if (xmlReader.NodeType == XmlNodeType.Element)
          {
            if (xmlReader.LocalName == "data")
            {
              Guid result1;
              if (Guid.TryParse(xmlReader.GetAttribute("identifier"), out result1))
                profileMessage.IdentityId = result1;
              AttributesScope result2;
              if (Enum.TryParse<AttributesScope>(xmlReader.GetAttribute("scope"), out result2))
                profileMessage.Scope = result2;
              profileMessage.ContainerName = xmlReader.GetAttribute("containerName");
              profileMessage.SubjectDescriptor = SubjectDescriptor.FromString(xmlReader.GetAttribute("sdesc"));
              profileMessage.IdentityDescriptor = IdentityDescriptor.FromString(xmlReader.GetAttribute("idesc"));
            }
          }
        }
      }
      return profileMessage;
    }

    public string ToXml()
    {
      StringBuilder output = new StringBuilder();
      using (XmlWriter xmlWriter = XmlWriter.Create(output))
      {
        xmlWriter.WriteStartElement("data");
        xmlWriter.WriteAttributeString("identifier", XmlConvert.ToString(this.IdentityId));
        xmlWriter.WriteAttributeString("containerName", this.ContainerName);
        xmlWriter.WriteAttributeString("scope", this.Scope.ToString());
        if (this.SubjectDescriptor != new SubjectDescriptor())
          xmlWriter.WriteAttributeString("sdesc", this.SubjectDescriptor.ToString());
        if (this.IdentityDescriptor != (IdentityDescriptor) null)
          xmlWriter.WriteAttributeString("idesc", this.IdentityDescriptor.ToString());
        xmlWriter.WriteEndElement();
        xmlWriter.Close();
      }
      return output.ToString();
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.ProcessTemplates.ProcessVersion
// Assembly: Microsoft.Azure.Boards.ProcessTemplates, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A94E8BA8-9851-4F5D-B619-9CF2FFF5B128
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.ProcessTemplates.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.Azure.Boards.ProcessTemplates
{
  [Serializable]
  public sealed class ProcessVersion
  {
    private ProcessVersion()
    {
    }

    public static ProcessVersion Create(int major, int minor) => new ProcessVersion()
    {
      Major = major,
      Minor = minor
    };

    public int Major { get; private set; }

    public int Minor { get; private set; }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Version {0}.{1}", (object) this.Major, (object) this.Minor);

    public override bool Equals(object obj) => obj is ProcessVersion processVersion && this.Major == processVersion.Major && this.Minor == processVersion.Minor;

    public override int GetHashCode() => (this.Major << 5) + this.Major ^ this.Minor;

    public static ProcessVersion ParseLegacyMetadataXml(XmlNode metadataNode) => ProcessVersion.ParseLegacyMetadataXml(metadataNode, out Guid _);

    public static ProcessVersion ParseLegacyMetadataXml(XmlNode metadataNode, out Guid typeId)
    {
      ArgumentUtility.CheckForNull<XmlNode>(metadataNode, nameof (metadataNode));
      try
      {
        return ProcessVersion.ParseLegacyMetadataXml(XElement.Parse(metadataNode.OuterXml), out typeId);
      }
      catch (XmlException ex)
      {
        throw new ProcessInvalidVersionException((Exception) ex);
      }
    }

    public static ProcessVersion ParseLegacyMetadataXml(XElement metadataElement) => ProcessVersion.ParseLegacyMetadataXml(metadataElement, out Guid _);

    public static ProcessVersion ParseLegacyMetadataXml(XElement metadataElement, out Guid typeId)
    {
      ArgumentUtility.CheckForNull<XElement>(metadataElement, nameof (metadataElement));
      try
      {
        XElement xelement = metadataElement.Element((XName) "version");
        if (xelement != null)
        {
          XAttribute xattribute1 = xelement.Attribute((XName) "type");
          if (xattribute1 != null)
          {
            if (Guid.TryParse(xattribute1.Value, out typeId))
            {
              XAttribute xattribute2 = xelement.Attribute((XName) "major");
              if (xattribute2 != null)
              {
                int result1;
                if (int.TryParse(xattribute2.Value, out result1))
                {
                  XAttribute xattribute3 = xelement.Attribute((XName) "minor");
                  if (xattribute3 != null)
                  {
                    int result2;
                    if (int.TryParse(xattribute3.Value, out result2))
                      return ProcessVersion.Create(result1, result2);
                  }
                }
              }
            }
          }
        }
      }
      catch (XmlException ex)
      {
        throw new ProcessInvalidVersionException((Exception) ex);
      }
      throw new ProcessInvalidVersionException();
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.TemplateVersion
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Xml;

namespace Microsoft.TeamFoundation.Server
{
  public class TemplateVersion
  {
    private const string c_versionNodeName = "version";
    private const string c_versionTypeAttributeName = "type";
    private const string c_minorVersionAttributeName = "minor";
    private const string c_majorVersionAttributeName = "major";
    private static readonly TemplateVersion DefaultUnspecifiedVersion = TemplateVersion.Create(Guid.Empty, -1, -1);

    public Guid TypeId { get; private set; }

    public int MajorVersion { get; private set; }

    public int MinorVersion { get; private set; }

    public static TemplateVersion Create(string metadataXml)
    {
      ArgumentUtility.CheckForNull<string>(metadataXml, nameof (metadataXml));
      try
      {
        return TemplateVersion.Create((XmlNode) XmlUtility.GetDocument(metadataXml));
      }
      catch (XmlException ex)
      {
        return TemplateVersion.DefaultUnspecifiedVersion;
      }
    }

    public static TemplateVersion Create(XmlNode xmlNode)
    {
      TemplateVersion unspecifiedVersion1;
      if (xmlNode != null)
      {
        try
        {
          XmlNode xmlNode1 = xmlNode.SelectSingleNode("//version");
          if (xmlNode1 == null)
          {
            TemplateVersion unspecifiedVersion2 = TemplateVersion.DefaultUnspecifiedVersion;
          }
          Guid typeId = new Guid(xmlNode1.Attributes["type"].Value);
          int num1 = int.Parse(xmlNode1.Attributes["major"].Value);
          int num2 = int.Parse(xmlNode1.Attributes["minor"].Value);
          int majorVersion = num1;
          int minorVersion = num2;
          return TemplateVersion.Create(typeId, majorVersion, minorVersion);
        }
        catch (Exception ex)
        {
          unspecifiedVersion1 = TemplateVersion.DefaultUnspecifiedVersion;
        }
      }
      else
        unspecifiedVersion1 = TemplateVersion.DefaultUnspecifiedVersion;
      return unspecifiedVersion1;
    }

    private static TemplateVersion Create(Guid typeId, int majorVersion, int minorVersion) => new TemplateVersion()
    {
      TypeId = typeId,
      MajorVersion = majorVersion,
      MinorVersion = minorVersion
    };

    public bool IsUnspecified => this.TypeId == Guid.Empty;

    public bool CouldOverride(TemplateVersion other)
    {
      if (this.IsUnspecified || !(this.TypeId == other.TypeId))
        return false;
      if (this.MajorVersion > other.MajorVersion)
        return true;
      return this.MajorVersion == other.MajorVersion && this.MinorVersion > other.MinorVersion;
    }
  }
}

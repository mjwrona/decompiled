// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Common.LegacyBuildTypeHelper
// Assembly: Microsoft.TeamFoundation.Build.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AD9C54FA-787C-49B8-AA73-C4A6EF8CE391
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.ComponentModel;
using System.IO;
using System.Xml;

namespace Microsoft.TeamFoundation.Build.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class LegacyBuildTypeHelper
  {
    public static BuildTypeInfo ParseBuildTypeFile(
      string buildTypeName,
      string fileName,
      bool allowZeroLengthFile,
      out string version)
    {
      string description = BuildTypeResource.Unknown();
      string buildMachine = BuildTypeResource.Unknown();
      string dropLocation = BuildTypeResource.Unknown();
      string buildDirectory = BuildTypeResource.Unknown();
      version = string.Empty;
      using (FileStream input = File.OpenRead(fileName))
      {
        try
        {
          FileInfo fileInfo = new FileInfo(fileName);
          if (allowZeroLengthFile)
          {
            if (fileInfo.Length <= 0L)
              goto label_23;
          }
          XmlReaderSettings settings = new XmlReaderSettings()
          {
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = (XmlResolver) null
          };
          using (XmlReader xmlReader = XmlReader.Create((Stream) input, settings))
          {
            while (xmlReader.Read())
            {
              if (VssStringComparer.XmlNodeName.Equals(xmlReader.LocalName, "Description"))
                description = xmlReader.ReadString();
              if (VssStringComparer.XmlNodeName.Equals(xmlReader.LocalName, "BuildMachine"))
                buildMachine = xmlReader.ReadString();
              if (VssStringComparer.XmlNodeName.Equals(xmlReader.LocalName, "DropLocation"))
                dropLocation = xmlReader.ReadString();
              if (VssStringComparer.XmlNodeName.Equals(xmlReader.LocalName, "BuildDirectoryPath"))
                buildDirectory = xmlReader.ReadString();
              if (VssStringComparer.XmlNodeName.Equals(xmlReader.LocalName, "ProjectFileVersion"))
                version = xmlReader.ReadString();
            }
          }
        }
        catch (Exception ex)
        {
          throw new IOException(BuildTypeResource.InvalidBuildTypeFile((object) buildTypeName, (object) ex.Message), ex);
        }
      }
label_23:
      return new BuildTypeInfo(buildTypeName, description, buildMachine, buildDirectory, dropLocation);
    }
  }
}

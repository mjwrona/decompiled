// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Health.BuildInfoReader
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.IO;
using System.Reflection;
using System.Web;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.Framework.Server.Health
{
  public static class BuildInfoReader
  {
    private static BuildInfo[] s_buildData;

    public static BuildInfo[] GetBuildInfo()
    {
      if (BuildInfoReader.s_buildData == null)
        BuildInfoReader.s_buildData = BuildInfoReader.GetBuildInfo(!(HttpRuntime.IISVersion != (Version) null) ? Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) : HttpRuntime.BinDirectory);
      return BuildInfoReader.s_buildData;
    }

    public static BuildInfo[] GetBuildInfo(string binDirectory)
    {
      if (!string.IsNullOrWhiteSpace(binDirectory))
      {
        string path = Path.Combine(binDirectory, "BuildInfo");
        if (Directory.Exists(path))
        {
          string[] files = Directory.GetFiles(path, "*.xml");
          BuildInfo[] buildInfo = new BuildInfo[files.Length];
          for (int index = 0; index < buildInfo.Length; ++index)
          {
            XDocument node = XDocument.Load(files[index]);
            buildInfo[index] = new BuildInfo()
            {
              Area = Path.GetFileNameWithoutExtension(files[index]),
              BuildNumber = node.XPathSelectElement("/BuildInfo/BuildNumber").Value,
              SourceVersion = node.XPathSelectElement("/BuildInfo/BuildSourceVersion")?.Value ?? string.Empty,
              SourceBranch = node.XPathSelectElement("/BuildInfo/BuildSourceBranch")?.Value ?? string.Empty
            };
          }
          return buildInfo;
        }
      }
      return (BuildInfo[]) null;
    }
  }
}

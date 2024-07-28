// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Implementations.Extensions.MavenPackageExtensions
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Maven.Server.Utilities;
using System;
using System.Collections.Generic;
using System.Web;

namespace Microsoft.VisualStudio.Services.Maven.Server.Implementations.Extensions
{
  public static class MavenPackageExtensions
  {
    private const string MimeZip = "application/zip";
    private const string MimeJavaArchive = "application/java-archive";
    private const string MimePlainText = "text/plain";
    private const string MimeXml = "text/xml";
    private const string MimeRar = "application/x-rar-compressed";
    private const string MimeOctetStream = "application/octet-stream";
    private const string MimeGZip = "application/gzip";
    private const string MimeGTar = "application/x-gtar";
    private const string MimeBzip2 = "application/x-bzip2";
    private static readonly Dictionary<string, string> ExtensionsMap = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        "zip",
        "application/zip"
      },
      {
        "jar",
        "application/java-archive"
      },
      {
        "xml",
        "text/xml"
      },
      {
        "md",
        "text/plain"
      },
      {
        "pom",
        "text/xml"
      },
      {
        "asc",
        "text/plain"
      },
      {
        "aar",
        "application/java-archive"
      },
      {
        "war",
        "application/java-archive"
      },
      {
        "ejb",
        "application/java-archive"
      },
      {
        "ejb3",
        "application/java-archive"
      },
      {
        "maven-plugin",
        "application/java-archive"
      },
      {
        "ear",
        "application/java-archive"
      },
      {
        "rar",
        "application/x-rar-compressed"
      },
      {
        "par",
        "application/octet-stream"
      },
      {
        "tar.bz2",
        "application/x-gtar"
      },
      {
        "tar.gz",
        "application/x-gtar"
      },
      {
        "tar.xz",
        "application/x-gtar"
      },
      {
        "tar.lzma",
        "application/x-gtar"
      },
      {
        "tgz",
        "application/x-gtar"
      },
      {
        "tz2",
        "application/x-gtar"
      },
      {
        "gz",
        "application/gzip"
      },
      {
        "bz2",
        "application/x-bzip2"
      },
      {
        "tbz",
        "application/zip"
      },
      {
        "so",
        "application/octet-stream"
      },
      {
        "har",
        "application/octet-stream"
      },
      {
        "apklib",
        "application/zip"
      }
    };

    public static string GetMimeType(string fileName)
    {
      bool isChecksum;
      string extension = MavenPackageExtensionsUtility.GetExtension(fileName, out isChecksum);
      if (isChecksum)
        return "text/plain";
      string str;
      return extension != null && MavenPackageExtensions.ExtensionsMap.TryGetValue(extension, out str) ? str : MimeMapping.GetMimeMapping(fileName);
    }
  }
}

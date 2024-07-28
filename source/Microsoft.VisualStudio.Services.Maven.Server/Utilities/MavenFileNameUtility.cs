// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Utilities.MavenFileNameUtility
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Maven.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.Maven.Server.Utilities
{
  public static class MavenFileNameUtility
  {
    public const string MD5FileExtension = ".md5";
    public const string SHA1FileExtension = ".sha1";
    public const string SHA256FileExtension = ".sha256";
    public const string SHA512FileExtension = ".sha512";
    public const string MetadataFileName = "maven-metadata.xml";
    public const string SnapshotTimestampFormat = "yyyyMMdd.HHmmss";
    private const string PomFileExtension = ".pom";
    public static readonly StringComparer FileNameStringComparer = StringComparer.OrdinalIgnoreCase;
    private static readonly ISet<string> checksumFileExtensions = (ISet<string>) new HashSet<string>((IEqualityComparer<string>) MavenFileNameUtility.FileNameStringComparer)
    {
      ".md5",
      ".sha1",
      ".sha256",
      ".sha512"
    };
    private static readonly Regex MetadataChecksumFileNamesPattern = new Regex("^maven-metadata\\.xml(\\.md5|\\.sha1|\\.sha256|\\.sha512)$", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled);

    public static IEnumerable<string> ChecksumFileExtensions => (IEnumerable<string>) MavenFileNameUtility.checksumFileExtensions;

    public static bool IsMetadataFile(string filePath)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(filePath, nameof (filePath));
      return MavenFileNameUtility.FileNameStringComparer.Equals(MavenFileNameUtility.GetFileName(filePath), "maven-metadata.xml");
    }

    public static bool IsChecksumFile(string filePath)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(filePath, nameof (filePath));
      return MavenFileNameUtility.checksumFileExtensions.Contains(MavenFileNameUtility.GetFileExtension(filePath));
    }

    public static (string BaseName, MavenHashAlgorithmInfo Algorithm) SplitChecksumFileName(
      string filePath)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(filePath, nameof (filePath));
      string fileExtension = MavenFileNameUtility.GetFileExtension(filePath);
      MavenHashAlgorithmInfo algorithm;
      return !MavenHashAlgorithmInfo.TryGet(fileExtension, out algorithm) ? (filePath, (MavenHashAlgorithmInfo) null) : (filePath.Substring(0, filePath.Length - fileExtension.Length), algorithm);
    }

    public static bool IsMetadataChecksum(string filePath)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(filePath, nameof (filePath));
      return MavenFileNameUtility.MetadataChecksumFileNamesPattern.IsMatch(MavenFileNameUtility.GetFileName(filePath));
    }

    public static bool IsMetadataOrMetadataChecksumFile(string filePath)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(filePath, nameof (filePath));
      return MavenFileNameUtility.IsMetadataFile(filePath) || MavenFileNameUtility.IsMetadataChecksum(filePath);
    }

    public static bool IsPomFile(string filePath)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(filePath, nameof (filePath));
      return MavenFileNameUtility.FileNameStringComparer.Equals(MavenFileNameUtility.GetFileExtension(filePath), ".pom");
    }

    public static bool IsSnapshotFileName(
      string fileName,
      bool includeChecksumFiles,
      out string artifactName,
      out DateTime? time,
      out int? build,
      out string classifier)
    {
      artifactName = (string) null;
      time = new DateTime?();
      build = new int?();
      classifier = (string) null;
      StringBuilder stringBuilder = new StringBuilder();
      if (!MavenFileNameUtility.IsChecksumFile(fileName) | includeChecksumFiles)
      {
        fileName = fileName.ToLowerInvariant();
        string[] strArray = fileName.Split("-".ToCharArray());
        for (int index = 0; index < strArray.Length; ++index)
        {
          string s1 = strArray[index];
          DateTime result1;
          if (DateTime.TryParseExact(s1, "yyyyMMdd.HHmmss", (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.None, out result1))
          {
            int startIndex = index + 1;
            if (startIndex < strArray.Length)
            {
              string s2 = ((IEnumerable<string>) strArray[startIndex].Split('.')).First<string>();
              int result2;
              if (int.TryParse(s2, out result2))
              {
                time = new DateTime?(result1);
                build = new int?(result2);
                string str = string.Join("-", strArray, startIndex, strArray.Length - startIndex).Substring(s2.Length);
                if (str.StartsWith("-"))
                  classifier = ((IEnumerable<string>) str.Remove(0, 1).Split('.')).FirstOrDefault<string>();
                artifactName = stringBuilder.ToString().TrimEnd("-".ToCharArray());
                return true;
              }
            }
          }
          else
            stringBuilder.Append(s1 + "-");
        }
      }
      return false;
    }

    private static string GetFileName(string filePath)
    {
      try
      {
        return Path.GetFileName(filePath);
      }
      catch (Exception ex)
      {
        throw new MavenInvalidFilePathException(Resources.Error_MavenInvalidFilePath((object) filePath), ex);
      }
    }

    private static string GetFileExtension(string filePath)
    {
      try
      {
        return Path.GetExtension(filePath);
      }
      catch (Exception ex)
      {
        throw new MavenInvalidFilePathException(Resources.Error_MavenInvalidFilePath((object) filePath), ex);
      }
    }
  }
}

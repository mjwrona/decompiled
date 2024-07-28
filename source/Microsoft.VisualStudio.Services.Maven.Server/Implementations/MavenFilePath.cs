// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Implementations.MavenFilePath
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Maven.Server.Implementations.Internal;
using Microsoft.VisualStudio.Services.Maven.Server.Utilities;
using Microsoft.VisualStudio.Services.Maven.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Maven.Server.Implementations
{
  public abstract class MavenFilePath : IMavenFilePath
  {
    protected static StringComparison FileNameStringComparison => StringComparison.OrdinalIgnoreCase;

    public string FileName { get; }

    public string Extension { get; }

    public abstract string FullName { get; }

    protected string OriginalPath { get; private set; }

    public MavenFilePath(string fileName)
    {
      ArgumentUtility.CheckStringForAnyWhiteSpace(fileName, nameof (fileName));
      this.FileName = fileName;
      this.Extension = Path.GetExtension(fileName);
    }

    public static IMavenFilePath Parse(string path, bool allowSnapshotLiteralInGroupIdAndArtifactId = false)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(path, nameof (path));
      string[] source = !path.Any<char>((Func<char, bool>) (c => char.IsWhiteSpace(c) || c == '\\')) ? path.Split('/') : throw new MavenGavException(Resources.Error_FilePathContainsInvalidCharacters((object) path));
      int length = source.Length;
      MavenFilePath.ThrowPathTooShortIf(path, length < 2);
      string fileName = source[length - 1];
      string str = source[length - 2];
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      if (((IEnumerable<string>) source).Take<string>(length - 1).Any<string>(MavenFilePath.\u003C\u003EO.\u003C0\u003E__IsNullOrEmpty ?? (MavenFilePath.\u003C\u003EO.\u003C0\u003E__IsNullOrEmpty = new Func<string, bool>(string.IsNullOrEmpty))))
        throw new MavenInvalidFilePathException(Resources.Error_FilePathHasEmptyComponents((object) path));
      if (string.IsNullOrEmpty(fileName))
        throw new MavenInvalidFilenameException(Resources.Error_MavenInvalidFilename((object) string.Empty));
      if (MavenFileNameUtility.IsMetadataOrMetadataChecksumFile(path))
      {
        if (MavenIdentityUtility.IsSnapshotVersion(str))
        {
          MavenFilePath.ThrowPathTooShortIf(path, length < 4);
          MavenVersionLevelMetadataFilePath metadataFilePath = new MavenVersionLevelMetadataFilePath(MavenFilePath.ValidateNotEndsWithSnapshot(MavenFilePath.GetGroupId(((IEnumerable<string>) source).Take<string>(length - 3)), allowSnapshotLiteralInGroupIdAndArtifactId), MavenFilePath.ValidateNotEndsWithSnapshot(source[length - 3], allowSnapshotLiteralInGroupIdAndArtifactId), str, fileName);
          metadataFilePath.OriginalPath = path;
          return (IMavenFilePath) metadataFilePath;
        }
        MavenGroupIdLevelMetadataFilePath metadataFilePath1 = new MavenGroupIdLevelMetadataFilePath(MavenFilePath.ValidateNotEndsWithSnapshot(MavenFilePath.GetGroupId(((IEnumerable<string>) source).Take<string>(length - 1)), allowSnapshotLiteralInGroupIdAndArtifactId), fileName);
        metadataFilePath1.OriginalPath = path;
        MavenGroupIdLevelMetadataFilePath groupIdLevelMetdataFilePath = metadataFilePath1;
        if (length == 2)
          return (IMavenFilePath) groupIdLevelMetdataFilePath;
        MavenArtifactIdLevelMetadataFilePath artifactIdLevelMetadataFilePath = new MavenArtifactIdLevelMetadataFilePath(MavenFilePath.ValidateNotEndsWithSnapshot(MavenFilePath.GetGroupId(((IEnumerable<string>) source).Take<string>(length - 2)), allowSnapshotLiteralInGroupIdAndArtifactId), MavenFilePath.ValidateNotEndsWithSnapshot(source[length - 2], allowSnapshotLiteralInGroupIdAndArtifactId), fileName);
        artifactIdLevelMetadataFilePath.OriginalPath = path;
        AmbiguousMavenMetadataFilePath metadataFilePath2 = new AmbiguousMavenMetadataFilePath((IMavenArtifactIdLevelMetadataFilePath) artifactIdLevelMetadataFilePath, (IMavenGroupIdLevelMetadataFilePath) groupIdLevelMetdataFilePath);
        metadataFilePath2.OriginalPath = path;
        return (IMavenFilePath) metadataFilePath2;
      }
      MavenFilePath.ThrowPathTooShortIf(path, length < 4);
      return (IMavenFilePath) new MavenArtifactFilePath(MavenFilePath.ValidateNotEndsWithSnapshot(MavenFilePath.GetGroupId(((IEnumerable<string>) source).Take<string>(length - 3)), allowSnapshotLiteralInGroupIdAndArtifactId), MavenFilePath.ValidateNotEndsWithSnapshot(source[length - 3], allowSnapshotLiteralInGroupIdAndArtifactId), str, fileName);
    }

    protected string BuildPath(string groupId, params string[] parts) => groupId.Replace('.', '/') + "/" + string.Join("/", parts);

    public static string GetOriginalGroupId(string groupId) => groupId.Replace('.', '/');

    private static string GetGroupId(IEnumerable<string> values) => string.Join(".", values);

    private static string ValidateNotEndsWithSnapshot(
      string s,
      bool allowSnapshotLiteralInGroupIdAndArtifactId)
    {
      if (!allowSnapshotLiteralInGroupIdAndArtifactId && s.EndsWith("snapshot", MavenFilePath.FileNameStringComparison))
        throw new MavenGavException(Resources.Error_SnapshotSuffixNotAllowedInGroupIdOrArtifactId((object) s));
      return s;
    }

    private static void ThrowPathTooShortIf(string path, bool condition)
    {
      if (condition)
        throw new MavenInvalidFilePathException(Resources.Error_FilePathHasTooFewComponents((object) path));
    }
  }
}

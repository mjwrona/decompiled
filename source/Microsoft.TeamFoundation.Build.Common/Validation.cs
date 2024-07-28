// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Common.Validation
// Assembly: Microsoft.TeamFoundation.Build.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AD9C54FA-787C-49B8-AA73-C4A6EF8CE391
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build.Common.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace Microsoft.TeamFoundation.Build.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class Validation
  {
    private static readonly char[] s_msbuildSpecialChars = new char[9]
    {
      '%',
      '*',
      '?',
      '@',
      '$',
      '(',
      ')',
      ';',
      '\''
    };
    private static readonly char[] s_invalidPlatformFlavorChars = new char[12]
    {
      '<',
      '>',
      '|',
      '*',
      ':',
      '&',
      '\\',
      '/',
      '"',
      '%',
      '?',
      '#'
    };
    private static readonly char[] s_definitionSpecialChars = new char[5]
    {
      '*',
      '?',
      '@',
      '$',
      '='
    };

    public static void CheckValidBuildDirectory(ref string path)
    {
      string error;
      if (!Validation.IsValidBuildDirectory(ref path, out error))
        throw new InvalidPathException(error);
    }

    public static void CheckValidBuildNumber(string buildNumber)
    {
      string error;
      if (!Validation.IsValidBuildNumber(buildNumber, out error))
        throw new ArgumentException(error);
    }

    public static void CheckValidBuildType(string buildType)
    {
      string error;
      if (!Validation.IsValidBuildType(buildType, out error))
        throw new ArgumentException(error);
    }

    public static void CheckValidConfigPlatform(string configPlatform)
    {
      string error;
      if (!Validation.IsValidConfigPlatform(configPlatform, out error))
        throw new ArgumentException(error);
    }

    public static void CheckValidAgentName(string name, bool allowWildcards)
    {
      string error;
      if (!Validation.IsValidAgentName(name, allowWildcards, out error))
        throw new ArgumentException(error);
    }

    public static void CheckValidControllerName(string name, bool allowWildcards)
    {
      string error;
      if (!Validation.IsValidControllerName(name, allowWildcards, out error))
        throw new ArgumentException(error);
    }

    public static void CheckValidDefinitionName(string name)
    {
      string error;
      if (!Validation.IsValidDefinitionName(name, out error))
        throw new ArgumentException(error);
    }

    public static void CheckValidDropLocation(string path) => Validation.CheckValidDropLocation(ref path);

    [CLSCompliant(false)]
    public static void CheckValidDropLocation(ref string path)
    {
      string error;
      if (!Validation.IsValidDropLocation(ref path, out error))
        throw new InvalidPathException(error);
    }

    public static void CheckValidServerPathDropLocation(ref string path)
    {
      string error;
      if (!Validation.IsValidServerPath(ref path, out error))
        throw new InvalidPathException(error);
    }

    public static void CheckValidUncDropLocation(ref string path)
    {
      string error;
      if (!Validation.IsValidUncPath(ref path, out error))
        throw new InvalidPathException(error);
    }

    public static void CheckValidBuildContainerDropLocation(ref string path)
    {
      string error;
      if (!BuildContainerPath.IsValidPath(ref path, out error))
        throw new InvalidPathException(error);
    }

    public static void CheckValidUncDropLocationNotServer(ref string path)
    {
      string str = path;
      string error;
      if (Validation.IsValidServerPath(ref path, out error))
        throw new InvalidPathException(BuildTypeResource.ServerPathNotSupported((object) str));
      if (Validation.IsValidBuildContainerPath(ref path, out error))
        throw new InvalidPathException(BuildTypeResource.BuildContainerPathNotSupported((object) str));
      if (!Validation.IsValidUncPath(ref path, out error))
        throw new InvalidPathException(error);
    }

    public static void CheckValidLogLocation(ref string path)
    {
      string error;
      if (!Validation.IsValidLogLocation(ref path, out error))
        throw new InvalidPathException(error);
    }

    public static void CheckValidItemPath(ref string path, bool allowWildcards)
    {
      string error;
      if (!BuildPath.IsValidPath(ref path, allowWildcards, out error))
        throw new InvalidPathException(error);
    }

    public static void CheckValidMachineName(string machineName)
    {
      string error;
      if (!Validation.IsValidMachineName(machineName, out error))
        throw new ArgumentException(error);
    }

    public static void CheckValidServiceHostName(string name, bool allowWildcards)
    {
      string error;
      if (!Validation.IsValidServiceHostName(name, allowWildcards, out error))
        throw new ArgumentException(error);
    }

    public static void CheckValidUri(string uri, string type)
    {
      string error;
      if (!Validation.IsValidUri(uri, type, out error))
        throw new ArgumentException(error);
    }

    public static bool IsValidDropLocation(string path, out string error) => Validation.IsValidDropLocation(ref path, out error);

    [CLSCompliant(false)]
    public static bool IsValidDropLocation(ref string path, out string error)
    {
      string str = path;
      if (Validation.IsValidUncPath(ref path, out error) || Validation.IsValidServerPath(ref path, out error) || Validation.IsValidDropLocationUri(path, out error) || Validation.IsValidBuildContainerPath(ref path, out error))
        return true;
      error = BuildTypeResource.InvalidDropLocation((object) str);
      return false;
    }

    internal static bool IsValidLogLocation(ref string path, out string error)
    {
      string str = path;
      if (Validation.IsValidUri(path, out error) || Validation.IsValidUncPath(ref path, out error) || Validation.IsValidServerPath(ref path, out error) || Validation.IsValidBuildContainerPath(ref path, out error))
        return true;
      error = BuildTypeResource.InvalidLogLocation((object) str);
      return false;
    }

    public static bool IsValidBuildDirectory(
      ref string path,
      out string error,
      bool allowVariables = true)
    {
      error = (string) null;
      if (string.IsNullOrEmpty(path))
      {
        error = TFCommonResources.InvalidPath((object) string.Empty);
        return false;
      }
      int num1 = path.IndexOfAny(new char[2]{ '\\', '/' });
      bool flag1 = path.Length > 2 && char.IsLetter(path[0]) && (int) path[1] == (int) Path.VolumeSeparatorChar;
      if (!flag1 && (num1 == 0 || !allowVariables || !BuildCommonUtil.IsEnvironmentVariable(path.Substring(0, num1 < 0 ? path.Length : num1))))
      {
        error = BuildTypeResource.InvalidPathVolumeRequired((object) path);
        return false;
      }
      int num2 = 0;
      if (flag1)
        num2 = num1 > 0 ? num1 : 2;
      string[] strArray = path.Substring(num2).Split(new char[2]
      {
        '\\',
        '/'
      }, StringSplitOptions.RemoveEmptyEntries);
      StringBuilder stringBuilder = new StringBuilder(flag1 ? path.Substring(0, num2) + "\\" : string.Empty);
      for (int index = 0; index < strArray.Length; ++index)
      {
        bool flag2 = ".".Equals(strArray[index], StringComparison.Ordinal) || "..".Equals(strArray[index], StringComparison.Ordinal) || Validation.IsValidFolderName(strArray[index], false, out error);
        if (!flag2 & allowVariables)
          flag2 = BuildCommonUtil.IsEnvironmentVariable(strArray[index]);
        if (!flag2)
          return false;
        stringBuilder.AppendFormat("{0}{1}", (object) strArray[index], index < strArray.Length - 1 ? (object) "\\" : (object) string.Empty);
      }
      path = stringBuilder.ToString();
      return true;
    }

    internal static bool IsValidBuildNumber(string buildNumber, out string error)
    {
      error = (string) null;
      if (buildNumber.EndsWith(" ", StringComparison.Ordinal) || buildNumber.EndsWith(".", StringComparison.Ordinal))
      {
        error = BuildTypeResource.BuildNumberInvalidTermination((object) buildNumber);
        return false;
      }
      if (LabelSpec.IsLegalName(buildNumber, false))
        return true;
      error = BuildTypeResource.BuildNumberInvalidChars((object) buildNumber);
      return false;
    }

    internal static bool IsValidMachineName(string machineName, out string error)
    {
      if (!string.IsNullOrEmpty(machineName))
      {
        int startIndex = machineName.IndexOf('.');
        if (startIndex >= 0)
          machineName = machineName.Remove(startIndex);
      }
      return TFCommonUtil.IsLegalComputerName(machineName, out error);
    }

    public static bool IsValidUncPath(string path, out string error) => Validation.IsValidUncPath(ref path, out error);

    [CLSCompliant(false)]
    public static bool IsValidUncPath(ref string path, out string error)
    {
      error = (string) null;
      if (string.IsNullOrEmpty(path))
      {
        error = TFCommonResources.InvalidPath((object) string.Empty);
        return false;
      }
      string fullPath;
      try
      {
        fullPath = FileSpec.GetFullPath(path, false);
      }
      catch (Exception ex)
      {
        error = !(ex is InvalidPathException) ? BuildTypeResource.InvalidUncPath((object) path) : ex.Message;
        return false;
      }
      if (!Path.IsPathRooted(path) || !VssStringComparer.FilePath.StartsWith(path, "\\\\"))
      {
        error = BuildTypeResource.InvalidUncPath((object) path);
        return false;
      }
      if (fullPath.Length > BuildConstants.MaxPathLength)
      {
        error = TFCommonResources.InvalidPathTooLongVariable((object) path, (object) BuildConstants.MaxPathLength);
        return false;
      }
      string[] strArray = fullPath.Split(new char[1]
      {
        Path.DirectorySeparatorChar
      }, StringSplitOptions.RemoveEmptyEntries);
      if (strArray.Length < 2)
      {
        error = BuildTypeResource.InvalidUncPath((object) path);
        return false;
      }
      if (!Validation.IsValidMachineName(strArray[0], out error))
        return false;
      for (int index = 1; index < strArray.Length; ++index)
      {
        if (!Validation.IsValidFolderName(strArray[index], false, out error))
          return false;
      }
      path = fullPath.TrimEnd('\\');
      return true;
    }

    internal static bool IsValidServerPath(string path, out string error) => Validation.IsValidServerPath(ref path, out error);

    internal static bool IsValidServerPath(ref string path, out string error)
    {
      error = (string) null;
      if (string.IsNullOrEmpty(path))
      {
        error = TFCommonResources.InvalidPath((object) string.Empty);
        return false;
      }
      if (!VersionControlPath.IsServerItem(path))
        return false;
      string fullPath;
      try
      {
        fullPath = VersionControlPath.GetFullPath(path);
        if (Wildcard.IsWildcard(fullPath))
          error = TFCommonResources.WildcardsNotAllowed();
      }
      catch (Exception ex)
      {
        if (ex is InvalidPathException)
          error = ex.Message;
        return false;
      }
      if (fullPath.Split(new char[1]{ '/' }, StringSplitOptions.RemoveEmptyEntries).Length < 3)
        return false;
      path = fullPath.TrimEnd('/');
      return true;
    }

    internal static bool IsValidBuildContainerPath(string path, out string error) => Validation.IsValidBuildContainerPath(ref path, out error);

    internal static bool IsValidBuildContainerPath(ref string path, out string error)
    {
      error = (string) null;
      if (string.IsNullOrEmpty(path))
      {
        error = TFCommonResources.InvalidPath((object) string.Empty);
        return false;
      }
      return BuildContainerPath.IsServerPath(path) && BuildContainerPath.IsValidPath(ref path, out error);
    }

    internal static bool IsValidDropLocationUri(string uri, out string error)
    {
      error = (string) null;
      if (string.IsNullOrEmpty(uri))
      {
        error = TFCommonResources.InvalidPath((object) string.Empty);
        return false;
      }
      if (!Validation.IsValidUri(uri, out error))
        return false;
      try
      {
        string serverPathForUrl = BuildCommonUtil.GetServerPathForUrl((string) null, uri);
        return Validation.IsValidServerPath(ref serverPathForUrl, out error);
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    internal static bool IsValidConfigPlatform(string str, out string error)
    {
      if (!Validation.IsValidFolderName(str, false, out error))
        return false;
      if (str.IndexOfAny(Validation.s_invalidPlatformFlavorChars) < 0)
        return true;
      error = BuildTypeResource.InvalidPlatformFlavorInvalidCharacters((object) str);
      return false;
    }

    internal static bool IsValidAgentName(string name, bool allowWildcards, out string error)
    {
      error = (string) null;
      if (string.IsNullOrEmpty(name))
      {
        error = BuildTypeResource.MissingAgentName();
        return false;
      }
      if (name.Length > BuildConstants.MaxPathNameLength)
      {
        error = BuildTypeResource.InvalidAgentNameTooLong((object) name, (object) BuildConstants.MaxPathNameLength);
        return false;
      }
      if (FileSpec.IsReservedName(name))
      {
        error = BuildTypeResource.InvalidAgentNameReservedName((object) name);
        return false;
      }
      switch (name[name.Length - 1])
      {
        case ' ':
        case '.':
          error = BuildTypeResource.InvalidAgentNameInvalidTermination((object) name);
          return false;
        default:
          foreach (char c in name)
          {
            if (!FileSpec.IsValidNtfsChar(c))
            {
              error = BuildTypeResource.InvalidAgentNameInvalidCharacters((object) name);
              return false;
            }
          }
          if (allowWildcards || !Wildcard.IsWildcard(name))
            return true;
          error = BuildTypeResource.InvalidAgentNameInvalidCharacters((object) name);
          return false;
      }
    }

    internal static bool IsValidControllerName(string name, bool allowWildcards, out string error)
    {
      error = (string) null;
      if (string.IsNullOrEmpty(name))
      {
        error = BuildTypeResource.MissingControllerName();
        return false;
      }
      if (name.Length > BuildConstants.MaxPathNameLength)
      {
        error = BuildTypeResource.InvalidControllerNameTooLong((object) name, (object) BuildConstants.MaxPathNameLength);
        return false;
      }
      if (FileSpec.IsReservedName(name))
      {
        error = BuildTypeResource.InvalidControllerNameReservedName((object) name);
        return false;
      }
      switch (name[name.Length - 1])
      {
        case ' ':
        case '.':
          error = BuildTypeResource.InvalidControllerNameInvalidTermination((object) name);
          return false;
        default:
          foreach (char c in name)
          {
            if (!FileSpec.IsValidNtfsChar(c))
            {
              error = BuildTypeResource.InvalidControllerNameInvalidCharacters((object) name);
              return false;
            }
          }
          if (allowWildcards || !Wildcard.IsWildcard(name))
            return true;
          error = BuildTypeResource.InvalidControllerNameInvalidCharacters((object) name);
          return false;
      }
    }

    internal static bool IsValidServiceHostName(string name, bool allowWildcards, out string error)
    {
      error = (string) null;
      if (string.IsNullOrEmpty(name))
      {
        error = BuildTypeResource.MissingServiceHostName();
        return false;
      }
      if (name.Length > BuildConstants.MaxPathNameLength)
      {
        error = BuildTypeResource.InvalidServiceHostNameTooLong((object) name, (object) BuildConstants.MaxPathNameLength);
        return false;
      }
      if (FileSpec.IsReservedName(name))
      {
        error = BuildTypeResource.InvalidServiceHostNameReservedName((object) name);
        return false;
      }
      switch (name[name.Length - 1])
      {
        case ' ':
        case '.':
          error = BuildTypeResource.InvalidServiceHostNameInvalidTermination((object) name);
          return false;
        default:
          foreach (char c in name)
          {
            if (!FileSpec.IsValidNtfsChar(c))
            {
              error = BuildTypeResource.InvalidServiceHostNameInvalidCharacters((object) name);
              return false;
            }
          }
          if (allowWildcards || !Wildcard.IsWildcard(name))
            return true;
          error = BuildTypeResource.InvalidServiceHostNameInvalidCharacters((object) name);
          return false;
      }
    }

    public static bool IsValidDefinitionName(string name, out string error)
    {
      error = (string) null;
      if (string.IsNullOrEmpty(name))
      {
        error = BuildTypeResource.MissingDefinitionName();
        return false;
      }
      if (name.Length > BuildConstants.MaxPathNameLength)
      {
        error = BuildTypeResource.InvalidDefinitionNameTooLong((object) name, (object) BuildConstants.MaxPathNameLength);
        return false;
      }
      if (FileSpec.IsReservedName(name))
      {
        error = BuildTypeResource.InvalidDefinitionNameReservedName((object) name);
        return false;
      }
      switch (name[name.Length - 1])
      {
        case ' ':
        case '.':
          error = BuildTypeResource.InvalidDefinitionNameInvalidTermination((object) name);
          return false;
        default:
          foreach (char c in name)
          {
            if (!FileSpec.IsValidNtfsChar(c))
            {
              error = BuildTypeResource.InvalidDefinitionNameInvalidCharacters((object) name);
              return false;
            }
          }
          if (name.IndexOfAny(Validation.s_definitionSpecialChars) < 0)
            return true;
          error = BuildTypeResource.InvalidDefinitionNameInvalidCharacters((object) name);
          return false;
      }
    }

    public static bool IsValidUri(string uri, out string error) => Validation.IsValidUri(uri, (string) null, out error);

    internal static bool IsValidUri(string uri, string type, out string error)
    {
      error = (string) null;
      try
      {
        Uri uri1 = new Uri(uri);
        if (!uri1.IsAbsoluteUri)
        {
          error = BuildTypeResource.InvalidUriNotAbsolute((object) uri);
          return false;
        }
        if (!string.IsNullOrEmpty(type))
        {
          ArtifactId artifactId = LinkingUtilities.DecodeUri(uri1.AbsoluteUri);
          if (!VssStringComparer.ArtifactType.Equals(type, artifactId.ArtifactType))
          {
            error = BuildTypeResource.ArtifactTypeMisMatch((object) type, (object) artifactId.ArtifactType);
            return false;
          }
          if (!VssStringComparer.ArtifactType.Equals("Build", artifactId.ArtifactType))
          {
            if (!int.TryParse(artifactId.ToolSpecificId, out int _))
            {
              error = BuildTypeResource.InvalidUriToolId((object) artifactId.ArtifactType, (object) artifactId.ToolSpecificId);
              return false;
            }
          }
        }
      }
      catch (Exception ex)
      {
        switch (ex)
        {
          case UriFormatException _:
          case ArgumentException _:
            error = ex.Message;
            return false;
          default:
            throw;
        }
      }
      return true;
    }

    internal static bool IsValidBuildType(string buildType, out string error) => Validation.IsValidFolderName(buildType, true, out error);

    internal static bool IsValidFolderName(string folder, bool requireVersionControl) => Validation.IsValidFolderName(folder, requireVersionControl, out string _);

    internal static bool IsValidFolderName(
      string folder,
      bool requireVersionControl,
      out string error)
    {
      error = (string) null;
      if (!FileSpec.IsLegalNtfsName(folder, BuildConstants.MaxPathNameLength, false, out error) || requireVersionControl && !VersionControlPath.IsValidFolderName(folder, out error))
        return false;
      if (!FileSpec.IsReservedName(folder))
        return true;
      error = BuildTypeResource.InvalidPathReservedName((object) folder);
      return false;
    }

    internal static bool IsValidSymbolStorePath(string path, out string error) => Validation.IsValidUncPath(path, out error);
  }
}

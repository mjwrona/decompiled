// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.PathHelper
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebServer;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.Wiki.Server
{
  public static class PathHelper
  {
    public static string GetPageReadablePath(string pageFilePath, string wikiRootPath)
    {
      if (string.IsNullOrEmpty(pageFilePath))
        return (string) null;
      if (wikiRootPath.Equals(pageFilePath))
        return "/";
      if (wikiRootPath != "/" && pageFilePath != "/")
        pageFilePath = pageFilePath.Substring(wikiRootPath.Length);
      pageFilePath = pageFilePath.Replace(SpecialChars.Hyphen, SpecialChars.Space);
      for (int index = 0; index < PathConstants.GitIllegalSpecialCharEscapes.Length; ++index)
        pageFilePath = pageFilePath.Replace(PathConstants.GitIllegalSpecialCharEscapes[index], PathConstants.GitIllegalSpecialChars[index]);
      return !pageFilePath.StartsWith(wikiRootPath) || wikiRootPath == "/" ? PathHelper.NormalizePath(pageFilePath) : pageFilePath;
    }

    public static string GetPageFilePath(string pageReadablePath, string wikiRootPath)
    {
      if (string.IsNullOrEmpty(pageReadablePath))
        return (string) null;
      for (int index = 0; index < PathConstants.GitIllegalSpecialChars.Length; ++index)
        pageReadablePath = pageReadablePath.Replace(PathConstants.GitIllegalSpecialChars[index], PathConstants.GitIllegalSpecialCharEscapes[index]);
      pageReadablePath = pageReadablePath.Replace(SpecialChars.Space, SpecialChars.Hyphen);
      return pageReadablePath.PrependPath(wikiRootPath);
    }

    public static string NormalizePath(string path)
    {
      if (string.IsNullOrEmpty(path))
        return (string) null;
      path = path.Trim();
      path = path.Replace('\\', '/');
      if (!path.StartsWith("/", StringComparison.Ordinal))
        path = "/" + path;
      return path;
    }

    internal static string GetDirectoryName(string path)
    {
      if (string.IsNullOrEmpty(path))
        return "";
      path = path.Replace('\\', '/');
      string[] source = path.Split(new string[1]{ "/" }, StringSplitOptions.None);
      string directoryName = string.Join("/", ((IEnumerable<string>) source).Take<string>(source.Length - 1).Where<string>((Func<string, bool>) (x => !string.IsNullOrEmpty(x))));
      if (path.StartsWith("/", StringComparison.Ordinal))
        directoryName = "/" + directoryName;
      return directoryName;
    }

    public static string GetPagePathWithExtension(this string path) => !string.IsNullOrEmpty(path) ? path + ".md" : (string) null;

    public static string GetOrderingFilePath(string folderPath) => !string.IsNullOrEmpty(folderPath) ? PathHelper.NormalizePath(Path.Combine(folderPath, ".order")) : (string) null;

    public static string GetAttachmentFilePath(string attachmentName) => !string.IsNullOrEmpty(attachmentName) ? PathHelper.NormalizePath(Path.Combine(".attachments", attachmentName)) : (string) null;

    public static string GetParentPath(string path) => !string.IsNullOrEmpty(path) ? PathHelper.NormalizePath(PathHelper.GetDirectoryName(path)) : (string) null;

    public static string GetCodeWikiAttachmentFilePath(string attachmentName, string wikiMappedPath) => !string.IsNullOrEmpty(wikiMappedPath) && !string.IsNullOrEmpty(attachmentName) ? PathHelper.NormalizePath(Path.Combine(wikiMappedPath, ".attachments", attachmentName)) : (string) null;

    public static string GetParentOrderFilePath(string pageFilePath)
    {
      string path1 = !(pageFilePath == "/") ? PathHelper.GetParentPath(pageFilePath) : "/";
      return !string.IsNullOrEmpty(path1) ? PathHelper.NormalizePath(Path.Combine(path1, ".order")) : (string) null;
    }

    public static bool IsValidPageName(string pageName)
    {
      if (string.IsNullOrEmpty(pageName) || pageName[0] == '.' || pageName[pageName.Length - 1] == '.')
        return false;
      foreach (string invalidCharacter in PathConstants.ResourceNameInvalidCharacters)
      {
        if (pageName.Contains(invalidCharacter))
          return false;
      }
      return true;
    }

    public static void ValidateAttachmentName(string name, bool allowSvg = false)
    {
      if (string.IsNullOrEmpty(name))
        throw new ArgumentNullException(nameof (name));
      foreach (string reservedCharacter in PathConstants.AttachmentNameReservedCharacters)
      {
        if (name.Contains(reservedCharacter))
          throw new InvalidArgumentValueException(nameof (name), string.Format(Resources.WikiAttachmentNameHasReservedCharacters, (object) string.Join(",", ((IEnumerable<string>) PathConstants.AttachmentNameReservedCharacters).Select<string, string>((Func<string, string>) (character => string.Format("'{0}'", (object) character))))));
      }
      string extension = Path.GetExtension(name);
      if (string.IsNullOrEmpty(extension) || !((IEnumerable<string>) PathConstants.AllowedAttachmentFileTypes(allowSvg)).Contains<string>(extension, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
        throw new InvalidArgumentValueException(nameof (name), string.Format(Resources.WikiAttachmentExtensionTypeNotSupported, (object) extension));
    }

    public static int GetPageDepth(string pagePath) => !string.IsNullOrEmpty(pagePath) ? pagePath.Split("/".ToArray<char>(), StringSplitOptions.RemoveEmptyEntries).Length : 0;

    public static string GetPageTitleFromReadablePath(string pagePath) => pagePath == "/" ? string.Empty : PathHelper.NormalizePath(PathHelper.GetPageReadablePath(Path.GetFileName(PathHelper.GetPageFilePath(pagePath, "/")), "/")).Replace("/", "");

    public static string GetPageTitleFromPath(string pagePath) => pagePath == "/" ? string.Empty : PathHelper.NormalizePath(PathHelper.GetPageReadablePath(Path.GetFileName(pagePath), "/")).Replace("/", "");

    public static string GetParentOfReadablePath(string pagePath) => pagePath == "/" ? string.Empty : PathHelper.NormalizePath(PathHelper.GetPageReadablePath(PathHelper.NormalizePath(Path.GetDirectoryName(PathHelper.GetPageFilePath(pagePath, "/"))), "/"));

    private static bool FileNameHasIllegalChars(string fileName)
    {
      foreach (string illegalSpecialChar in PathConstants.GitIllegalSpecialChars)
      {
        if (!illegalSpecialChar.Equals(SpecialChars.Hyphen) && fileName.Contains(illegalSpecialChar))
          return true;
      }
      return false;
    }

    public static bool IsPageFileNameNonConformant(string pageFilePath, bool allowTemplateFolder)
    {
      string fileName = Path.GetFileName(pageFilePath).Trim();
      int length = fileName.Length;
      if (length == 0)
        return false;
      return fileName[0] == '.' && (!allowTemplateFolder || !(fileName == ".templates")) || fileName[length - 1] == '.' || fileName.IndexOf('#') >= 0 || fileName.IndexOf(' ') >= 0 || PathHelper.FileNameHasIllegalChars(fileName);
    }

    public static string TrimTrailingSeparatorInPath(string path)
    {
      if (!string.IsNullOrEmpty(path) && !path.Equals("/") && path[path.Length - 1] == '/')
        path = path.TrimEnd(Constants.WikiPathSeparatorCharacters);
      return path;
    }

    public static bool IsPathUnder(string pageFilePath, string wikiRootPath) => !string.IsNullOrEmpty(pageFilePath) && !string.IsNullOrEmpty(wikiRootPath) && pageFilePath.StartsWith(wikiRootPath.TrimEnd(Constants.WikiPathSeparatorCharacters) + "/", StringComparison.Ordinal);

    public static string GetWikiPathFromGitFilePath(this string gitFilePath, string wikiRootPath)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(gitFilePath, nameof (gitFilePath));
      ArgumentUtility.CheckStringForNullOrEmpty(wikiRootPath, nameof (wikiRootPath));
      gitFilePath = gitFilePath.TrimEnd(Constants.WikiPathSeparatorCharacters);
      if (!PathHelper.IsPathUnder(gitFilePath, wikiRootPath))
        return (string) null;
      if (wikiRootPath.Equals(gitFilePath))
        return "/";
      if (wikiRootPath == "/")
        return gitFilePath;
      wikiRootPath = wikiRootPath.TrimEnd(Constants.WikiPathSeparatorCharacters);
      return gitFilePath.Remove(0, wikiRootPath.Length);
    }

    public static string PrependPath(this string path, string parentPath)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(path, nameof (path));
      ArgumentUtility.CheckStringForNullOrEmpty(parentPath, nameof (parentPath));
      path = path.TrimStart(Constants.WikiPathSeparatorCharacters);
      return PathHelper.NormalizePath(Path.Combine(parentPath, path));
    }

    public static bool IsMdFile(this string pageFilePath, IVssRequestContext requestContext = null)
    {
      string extension;
      try
      {
        extension = Path.GetExtension(pageFilePath);
      }
      catch (ArgumentException ex)
      {
        if (requestContext != null)
          requestContext.TraceException((Exception) ex, 15250602, nameof (IsMdFile));
        return false;
      }
      return ".md".Equals(extension, StringComparison.InvariantCultureIgnoreCase);
    }

    public static string GetPageReadablePathFromUnReadablePath(string unReadablePath) => PathHelper.GetPageReadablePath(unReadablePath, "/");
  }
}

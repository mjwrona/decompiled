// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities.FolderValidator
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities
{
  public static class FolderValidator
  {
    private const string PathSeparator = "\\";
    private static readonly List<char> IllegalNameChars = new List<char>()
    {
      '<',
      '>',
      '%',
      '&',
      ':',
      '?',
      '$',
      '@',
      '*'
    };

    [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "By Design")]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "By Design")]
    public static string ValidateAndSanitizePath(
      string path,
      bool allowWildcards = false,
      bool allowRootPath = true)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(path, nameof (path));
      path = FolderValidator.FixPath(path, allowWildcards, allowRootPath);
      if (!allowWildcards && path.Length > 400)
        throw new InvalidFolderException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.InvalidFolderPathLength, (object) path));
      return path;
    }

    public static void CheckForFolderNamesWithOnlyDigits(
      IVssRequestContext requestContext,
      string folderPath)
    {
      if (string.IsNullOrWhiteSpace(folderPath) || !requestContext.IsFeatureEnabled("AzureDevops.ReleaseManagement.BlockFolderNamesWithDigits"))
        return;
      string[] strArray = folderPath.Split(FolderService.PathDelimiters.ToArray<char>(), StringSplitOptions.RemoveEmptyEntries);
      if (strArray.Length == 0)
        return;
      foreach (string folderName in strArray)
      {
        if (FolderValidator.IsDigitsOnly(folderName))
          throw new InvalidFolderException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.FolderNameWithOnlyDigitsIsNotAllowed, (object) folderName));
      }
    }

    private static bool IsDigitsOnly(string folderName)
    {
      if (string.IsNullOrWhiteSpace(folderName))
        return false;
      foreach (char ch in folderName)
      {
        if (ch < '0' || ch > '9')
          return false;
      }
      return true;
    }

    private static string FixPath(string oldPath, bool allowWildcards, bool allowRootPath)
    {
      try
      {
        string fullPath = FolderValidator.GetFullPath(oldPath, allowRootPath);
        IEnumerable<char> source = FolderValidator.IllegalNameChars.Where<char>((Func<char, bool>) (n => !allowWildcards || n != '*'));
        ArgumentUtility.CheckStringForInvalidCharacters(fullPath, "path", source.ToArray<char>());
        return allowWildcards || !Wildcard.IsWildcard(fullPath) ? fullPath : throw new InvalidPathException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.InvalidPathContainsWildcards, (object) fullPath));
      }
      catch (Exception ex)
      {
        throw new InvalidPathException(ex.Message);
      }
    }

    private static string GetFullPath(string path, bool allowEmptyRootPath)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(path, nameof (path));
      StringBuilder stringBuilder = new StringBuilder();
      string[] strArray = path.Split(FolderService.PathDelimiters.ToArray<char>(), StringSplitOptions.RemoveEmptyEntries);
      if (strArray.Length == 0)
      {
        if (!allowEmptyRootPath)
          throw new InvalidPathException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.FolderPathNotSupported, (object) path));
        stringBuilder.Append("\\");
      }
      for (int index = 0; index < strArray.Length; ++index)
      {
        string error;
        if (!FileSpec.IsLegalNtfsName(strArray[index], 400, true, out error))
          throw new InvalidPathException(error);
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.CurrentCulture, "{0}{1}", (object) "\\", (object) strArray[index]);
      }
      return stringBuilder.ToString();
    }
  }
}

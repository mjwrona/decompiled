// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Common.VersionControlUtil
// Assembly: Microsoft.TeamFoundation.VersionControl.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 156CCB01-0A1F-468C-A332-06DB9F9B179E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Common.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.VersionControl.Common.Internal;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.VersionControl.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class VersionControlUtil
  {
    private static Regex s_8Dot3Checker = new Regex("[\\\\/]([^\\\\/]*~\\d+)(\\.[^.]{1,3})?([\\\\/]|$)");
    private static long[] s_subnetMasks = new long[33]
    {
      0L,
      128L,
      192L,
      224L,
      240L,
      248L,
      252L,
      254L,
      (long) byte.MaxValue,
      33023L,
      49407L,
      57599L,
      61695L,
      63743L,
      64767L,
      65279L,
      (long) ushort.MaxValue,
      8454143L,
      12648447L,
      14745599L,
      15794175L,
      16318463L,
      16580607L,
      16711679L,
      16777215L,
      2164260863L,
      3238002687L,
      3774873599L,
      4043309055L,
      4177526783L,
      4244635647L,
      4278190079L,
      (long) uint.MaxValue
    };
    private const string c_tfvcArea = "tfvc";

    public static string FormatWorkingFolderText(
      bool cloaked,
      string serverItem,
      string localFolder)
    {
      if (cloaked)
        return string.Format((IFormatProvider) CultureInfo.CurrentCulture, " {0} {1}:", (object) Resources.Get("CloakedIndicator"), (object) serverItem);
      if (localFolder.Length >= 2 && localFolder[1] == ':' && (int) localFolder[0] != (int) char.ToUpperInvariant(localFolder[0]))
        localFolder = new StringBuilder(localFolder)
        {
          [0] = char.ToUpperInvariant(localFolder[0])
        }.ToString();
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, " {0}: {1}", (object) serverItem, (object) localFolder);
    }

    public static string GetFullPath(string item) => VersionControlUtil.GetFullPath(item, PathLength.Length399);

    public static string GetFullPath(string item, PathLength maxServerPathLength)
    {
      item = !VersionControlPath.IsServerItem(item) ? FileSpec.GetFullPath(item) : VersionControlPath.GetFullPath(item, maxServerPathLength);
      return item;
    }

    public static void GetFullPaths(string[] items) => VersionControlUtil.GetFullPaths(items, PathLength.Length399);

    public static void GetFullPaths(string[] items, PathLength maxServerPathLength)
    {
      if (items == null)
        return;
      for (int index = 0; index < items.Length; ++index)
        items[index] = VersionControlUtil.GetFullPath(items[index], maxServerPathLength);
    }

    public static bool IsLegalComment(string comment, int maxLength)
    {
      if (comment == null)
        return true;
      return comment.Length <= maxLength && TFCommonUtil.IsLegalXmlString(comment);
    }

    public static RegistryKey GetOrCreateSubKey(RegistryKey parent, string keyName) => parent?.CreateSubKey(keyName, RegistryKeyPermissionCheck.ReadWriteSubTree);

    public static RegistryKey GetSubKey(RegistryKey parent, string keyName, bool writable) => parent?.OpenSubKey(keyName, writable);

    public static DateTime GetLastWriteTime(string localPath)
    {
      if (string.IsNullOrEmpty(localPath))
        return new DateTime();
      try
      {
        return System.IO.File.GetLastWriteTime(localPath);
      }
      catch
      {
        return new DateTime();
      }
    }

    public static string GetWorkstationSiteName()
    {
      try
      {
        using (ActiveDirectorySite computerSite = ActiveDirectorySite.GetComputerSite())
          return computerSite.Name;
      }
      catch
      {
      }
      return (string) null;
    }

    public static string GetSiteName(string hostOrIP)
    {
      string SiteName = (string) null;
      try
      {
        hostOrIP = Dns.GetHostEntry(hostOrIP).HostName;
        if (Microsoft.TeamFoundation.Common.Internal.NativeMethods.DsGetSiteName(hostOrIP, out SiteName) != 0U)
          SiteName = (string) null;
      }
      catch (Exception ex)
      {
      }
      if (!string.IsNullOrEmpty(SiteName))
        return SiteName;
      try
      {
        using (DirectoryEntry directoryEntry = new DirectoryEntry("LDAP://RootDSE"))
        {
          using (DirectoryEntry searchRoot = new DirectoryEntry("LDAP://cn=subnets,cn=Sites," + directoryEntry.Properties["configurationNamingContext"].Value?.ToString()))
          {
            using (DirectorySearcher directorySearcher = new DirectorySearcher(searchRoot))
            {
              foreach (IPAddress hostAddress in Dns.GetHostAddresses(hostOrIP))
              {
                if (AddressFamily.InterNetwork == hostAddress.AddressFamily)
                {
                  byte[] addressBytes = hostAddress.GetAddressBytes();
                  long num = (long) ((int) addressBytes[0] + ((int) addressBytes[1] << 8) + ((int) addressBytes[2] << 16) + ((int) addressBytes[3] << 24));
                  string[] strArray = new string[33];
                  for (int index = 0; index < 33; ++index)
                  {
                    string str = new IPAddress(num & VersionControlUtil.s_subnetMasks[index]).ToString();
                    strArray[index] = str + "/" + index.ToString((IFormatProvider) CultureInfo.InvariantCulture);
                  }
                  StringBuilder stringBuilder = new StringBuilder();
                  stringBuilder.Append("(|");
                  foreach (string str in strArray)
                    stringBuilder.Append("(cn=" + str + ")");
                  stringBuilder.Append(")");
                  directorySearcher.Filter = stringBuilder.ToString();
                  SearchResult one = directorySearcher.FindOne();
                  if (one != null)
                  {
                    using (ActiveDirectorySubnet byName = ActiveDirectorySubnet.FindByName(new DirectoryContext(DirectoryContextType.Forest), one.Properties["name"][0].ToString()))
                      return byName.Site.Name;
                  }
                }
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
      }
      return (string) null;
    }

    public static int TranslatePermission(Type enumClass, string[] array, int allPermissions)
    {
      try
      {
        return TFCommonUtil.TranslateEnum(enumClass, array, allPermissions);
      }
      catch (ArgumentException ex)
      {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (string name in System.Enum.GetNames(enumClass))
        {
          stringBuilder.Append(name);
          stringBuilder.Append(", ");
        }
        stringBuilder.Append("*");
        throw new ArgumentException(Resources.Format("InvalidPermissionValue", (object) ex.ParamName, (object) stringBuilder.ToString()), (Exception) ex).Expected("tfvc");
      }
    }

    public static ItemValidationError CheckItem(
      ref string item,
      string parameterName,
      bool allowNull,
      bool allowWildcards,
      bool allow8Dot3Paths,
      bool checkReservedCharacters)
    {
      return VersionControlUtil.CheckItem(ref item, parameterName, allowNull, allowWildcards, allow8Dot3Paths, checkReservedCharacters, PathLength.Length399);
    }

    public static ItemValidationError CheckItem(
      ref string item,
      string parameterName,
      bool allowNull,
      bool allowWildcards,
      bool allow8Dot3Paths,
      bool checkReservedCharacters,
      PathLength maxServerPathLength)
    {
      return item != null && VersionControlPath.IsServerItem(item) ? VersionControlUtil.CheckServerItem(ref item, parameterName, allowNull, allowWildcards, allow8Dot3Paths, checkReservedCharacters, maxServerPathLength) : VersionControlUtil.CheckLocalItem(item, parameterName, allowNull, allowWildcards, allow8Dot3Paths, checkReservedCharacters);
    }

    public static ItemValidationError CheckServerItem(
      ref string item,
      string parameterName,
      bool allowNull,
      bool allowWildcards,
      bool allow8Dot3Paths,
      bool checkReservedCharacters)
    {
      return VersionControlUtil.CheckServerItem(ref item, parameterName, allowNull, allowWildcards, allow8Dot3Paths, checkReservedCharacters, PathLength.Length399);
    }

    public static ItemValidationError CheckServerItem(
      ref string item,
      string parameterName,
      bool allowNull,
      bool allowWildcards,
      bool allow8Dot3Paths,
      bool checkReservedCharacters,
      PathLength maxServerPathLength)
    {
      if (item == null || item.Length == 0)
      {
        if (!allowNull)
          throw new ArgumentNullException(parameterName).Expected("tfvc");
      }
      else
      {
        if (!allowWildcards && Wildcard.IsWildcard(item))
          return ItemValidationError.WildcardNotAllowed;
        if (item.Length > VersionControlUtil.ConvertPathLengthToInt(maxServerPathLength))
          return ItemValidationError.RepositoryPathTooLong;
        item = VersionControlPath.GetFullPath(item, checkReservedCharacters, maxServerPathLength);
        char[] separator = new char[1]{ '/' };
        foreach (string str in item.Split(separator, StringSplitOptions.RemoveEmptyEntries))
        {
          if (str != null && str.Length > 259)
            return ItemValidationError.RepositoryPathTooLong;
        }
        if (!allow8Dot3Paths)
          VersionControlUtil.Check8Dot3Aliases(item);
      }
      return ItemValidationError.None;
    }

    public static ItemValidationError CheckLocalItem(
      string item,
      string parameterName,
      bool allowNull,
      bool allowWildcards,
      bool allow8Dot3Paths,
      bool checkReservedCharacters)
    {
      if (item == null || item.Length == 0)
      {
        if (!allowNull)
          throw new ArgumentNullException(parameterName).Expected("tfvc");
      }
      else
      {
        if (VersionControlPath.IsServerItem(item))
          return ItemValidationError.LocalItemRequired;
        if (!allowWildcards && Wildcard.IsWildcard(item))
          return ItemValidationError.WildcardNotAllowed;
        if (item.Length > 259)
          throw new PathTooLongException();
        if (!Path.IsPathRooted(item))
          throw new InvalidPathException(TFCommonResources.InvalidPath((object) item));
        if (item.StartsWith("\\", StringComparison.OrdinalIgnoreCase) && !item.StartsWith("\\\\", StringComparison.OrdinalIgnoreCase))
          throw new InvalidPathException(TFCommonResources.InvalidPath((object) item));
        if (!allow8Dot3Paths)
          VersionControlUtil.Check8Dot3Aliases(item);
        char c;
        if (checkReservedCharacters && FileSpec.HasVersionControlReservedCharacter(item, out c))
          throw new InvalidPathException(TFCommonResources.InvalidPathInvalidChar((object) item, (object) c));
      }
      return ItemValidationError.None;
    }

    public static int ConvertPathLengthToInt(PathLength pathLength)
    {
      if (pathLength <= PathLength.Length259WithGuid)
      {
        if (pathLength != PathLength.Length259 && pathLength != PathLength.Length259WithGuid)
          goto label_4;
      }
      else if (pathLength != PathLength.Length399 && pathLength != PathLength.MaxLengthWithGuid)
        goto label_4;
      return (int) pathLength;
label_4:
      throw new ArgumentOutOfRangeException(nameof (pathLength));
    }

    public static int GetMaxPathLengthComponent(PathLength pathLength) => VersionControlUtil.ConvertPathLengthToInt(pathLength) - 3;

    internal static void Check8Dot3Aliases(string item)
    {
      if (item.IndexOf('~') < 0)
        return;
      MatchCollection matchCollection = VersionControlUtil.s_8Dot3Checker.Matches(item);
      for (int i = 0; i < matchCollection.Count; ++i)
      {
        if (matchCollection[i].Groups[1].Value.Length <= 8)
          throw new InvalidPathException(Resources.Format("Invalid8Dot3Path", (object) item));
      }
    }
  }
}

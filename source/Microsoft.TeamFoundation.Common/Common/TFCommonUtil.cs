// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.TFCommonUtil
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Xml;

namespace Microsoft.TeamFoundation.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class TFCommonUtil
  {
    private static Process s_process;
    private static readonly char[] s_illegalComputerNameChars = new char[62]
    {
      char.MinValue,
      '\u0001',
      '\u0002',
      '\u0003',
      '\u0004',
      '\u0005',
      '\u0006',
      '\a',
      '\b',
      '\t',
      '\n',
      '\v',
      '\f',
      '\r',
      '\u000E',
      '\u000F',
      '\u0010',
      '\u0011',
      '\u0012',
      '\u0013',
      '\u0014',
      '\u0015',
      '\u0016',
      '\u0017',
      '\u0018',
      '\u0019',
      '\u001A',
      '\u001B',
      '\u001C',
      '\u001D',
      '\u001E',
      '\u001F',
      '`',
      '~',
      '!',
      '@',
      '#',
      '$',
      '%',
      ' ',
      '^',
      '&',
      '*',
      '(',
      ')',
      '=',
      '+',
      '[',
      ']',
      '{',
      '}',
      '\\',
      '|',
      ';',
      ':',
      '\'',
      '"',
      ',',
      '<',
      '>',
      '/',
      '?'
    };
    private static readonly char[] s_illegalIdentityNameChars = new char[47]
    {
      char.MinValue,
      '\u0001',
      '\u0002',
      '\u0003',
      '\u0004',
      '\u0005',
      '\u0006',
      '\a',
      '\b',
      '\t',
      '\n',
      '\v',
      '\f',
      '\r',
      '\u000E',
      '\u000F',
      '\u0010',
      '\u0011',
      '\u0012',
      '\u0013',
      '\u0014',
      '\u0015',
      '\u0016',
      '\u0017',
      '\u0018',
      '\u0019',
      '\u001A',
      '\u001B',
      '\u001C',
      '\u001D',
      '\u001E',
      '\u001F',
      '"',
      '/',
      '\\',
      '[',
      ']',
      ':',
      '|',
      '<',
      '>',
      '+',
      '=',
      ';',
      '?',
      '*',
      ','
    };
    private static readonly char[] s_trimChars = new char[2]
    {
      '/',
      '\\'
    };
    private static readonly string[] s_trimStrings = new string[2]
    {
      "/",
      "\\"
    };
    private const int MaxGroupNameLength = 256;
    private const int MaxGroupDescriptionLength = 256;

    public static IntPtr GetImageHandle(object obj)
    {
      switch (obj)
      {
        case IntPtr imageHandle:
          return imageHandle;
        case int num:
          return new IntPtr(num);
        default:
          return IntPtr.Zero;
      }
    }

    public static string GetExceptionMessage(Exception e) => e.Message;

    public static bool IsLonghornOrLater() => OSDetails.MajorVersion >= 6;

    public static string GetLocalTimeZoneOffset() => TFCommonUtil.GetLocalTimeZoneOffset(DateTime.Now);

    public static string GetLocalTimeZoneName(DateTime date) => TimeZone.CurrentTimeZone.IsDaylightSavingTime(date) ? TimeZone.CurrentTimeZone.DaylightName : TimeZone.CurrentTimeZone.StandardName;

    public static string GetLocalTimeZoneOffset(DateTime dateTime)
    {
      TimeSpan utcOffset = TimeZone.CurrentTimeZone.GetUtcOffset(dateTime);
      string localTimeZoneOffset = utcOffset.ToString();
      string positiveSign = NumberFormatInfo.CurrentInfo.PositiveSign;
      string negativeSign = NumberFormatInfo.CurrentInfo.NegativeSign;
      if (utcOffset > TimeSpan.Zero && !localTimeZoneOffset.StartsWith(positiveSign, StringComparison.Ordinal))
        localTimeZoneOffset = positiveSign + localTimeZoneOffset;
      else if (utcOffset < TimeSpan.Zero && !localTimeZoneOffset.StartsWith(negativeSign, StringComparison.Ordinal))
        localTimeZoneOffset = negativeSign + localTimeZoneOffset;
      return localTimeZoneOffset;
    }

    public static bool IsAlphaNumString(string strIn)
    {
      ArgumentUtility.CheckForNull<string>(strIn, nameof (strIn));
      foreach (char ch in strIn)
      {
        if ((ch < '0' || ch > '9') && (ch < 'a' || ch > 'z') && (ch < 'A' || ch > 'Z'))
          return false;
      }
      return true;
    }

    public static bool HasControlCharacters(string strIn) => TFCommonUtil.HasControlCharacters(strIn, false);

    public static bool HasControlCharacters(string strIn, bool allowCrLf)
    {
      for (int index = 0; index < strIn.Length; ++index)
      {
        char c = strIn[index];
        if ((!allowCrLf || c != '\n' && c != '\r') && (char.IsControl(c) || c == '\uFFFE' || c == char.MaxValue))
          return true;
      }
      return false;
    }

    [Obsolete("Use method in Microsft.VisualStudio.Services.Common.ArgumentUtility instead.")]
    public static bool HasSurrogates(string strIn) => ArgumentUtility.HasSurrogates(strIn);

    [Obsolete("Use method in Microsft.VisualStudio.Services.Common.ArgumentUtility instead.")]
    public static bool HasMismatchedSurrogates(string strIn) => TFCommonUtil.HasMismatchedSurrogates(strIn);

    [Obsolete("Use method in Microsft.VisualStudio.Services.Common.ArgumentUtility instead.")]
    public static bool IsInvalidString(string strIn) => TFCommonUtil.IsInvalidString(strIn);

    [Obsolete("Use method in Microsft.VisualStudio.Services.Common.ArgumentUtility instead.")]
    public static bool IsInvalidString(string strIn, bool allowCrLf) => TFCommonUtil.IsInvalidString(strIn, allowCrLf);

    public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
    {
      if (enumerable == null)
        return true;
      return !(enumerable is ICollection<T> objs) ? !enumerable.Any<T>() : objs.Count < 1;
    }

    public static bool IsSafeUrlProtocol(string url)
    {
      bool? nullable = new bool?();
      try
      {
        Uri uri = new Uri(url.Trim(), UriKind.Absolute);
        nullable = new bool?(UrlConstants.SafeUriSchemesSet.Contains(uri.Scheme));
      }
      catch (Exception ex)
      {
        nullable = new bool?(false);
      }
      return nullable.Value;
    }

    public static string RemoveInvalidCharacters(string strIn) => TFCommonUtil.ReplaceInvalidCharacters(strIn, (string) null);

    public static string ReplaceInvalidCharacters(string strIn, string replacement) => TFCommonUtil.ReplaceInvalidCharacters(strIn, replacement, false);

    public static string ReplaceInvalidCharacters(
      string strIn,
      string replacement,
      bool allowGB18030)
    {
      StringBuilder stringBuilder = new StringBuilder(strIn);
      for (int index = stringBuilder.Length - 1; index >= 0; --index)
      {
        char ch = stringBuilder[index];
        bool flag1 = false;
        if (ArgumentUtility.IsIllegalInputCharacter(ch, false, allowGB18030))
          flag1 = true;
        else if (char.IsHighSurrogate(ch))
          flag1 = true;
        else if (char.IsLowSurrogate(ch))
        {
          bool flag2 = false;
          if (index > 0 && char.IsSurrogatePair(stringBuilder[index - 1], ch))
            flag2 = true;
          if (flag2)
            --index;
          else
            flag1 = true;
        }
        if (flag1)
        {
          if (replacement != null && replacement.Length == 1)
          {
            stringBuilder[index] = replacement[0];
          }
          else
          {
            stringBuilder.Remove(index, 1);
            if (!string.IsNullOrEmpty(replacement))
              stringBuilder.Insert(index, replacement);
          }
        }
      }
      return stringBuilder.ToString();
    }

    public static void ValidateIdentityType(string identityType)
    {
      if (string.IsNullOrEmpty(identityType))
        throw new ArgumentNullException(nameof (identityType));
      if (identityType.Length > 128)
        throw new ArgumentOutOfRangeException(nameof (identityType));
    }

    public static void ValidateIdentifier(string identifier)
    {
      if (string.IsNullOrEmpty(identifier))
        throw new ArgumentNullException(nameof (identifier));
      if (identifier.Length > 256)
        throw new ArgumentOutOfRangeException(nameof (identifier));
    }

    public static string MakeDescriptorSearchFactor(string identityType, string identifier) => identityType + ";" + identifier;

    public static void ParseDescriptorSearchFactor(
      string descriptorValue,
      out string identityType,
      out string identifier)
    {
      string[] strArray = descriptorValue.Split(';');
      if (strArray.Length != 2)
      {
        identityType = (string) null;
        identifier = (string) null;
      }
      else
      {
        identityType = strArray[0];
        identifier = strArray[1];
      }
    }

    public static string GetIdentityDomainScope(Guid hostId) => LinkingUtilities.EncodeUri(new ArtifactId("Framework", "IdentityDomain", hostId.ToString()));

    public static void CheckApplicationGroupPropertyAndValue(
      GroupProperty property,
      ref string value)
    {
      if (property != GroupProperty.Name)
      {
        if (property != GroupProperty.Description)
          throw new ArgumentOutOfRangeException(nameof (property));
        TFCommonUtil.CheckGroupDescription(ref value);
      }
      else
        TFCommonUtil.CheckGroupName(ref value);
    }

    public static void CheckGroupName(ref string groupName)
    {
      if (!TFCommonUtil.IsValidGroupName(ref groupName))
        throw new ArgumentException(TFCommonResources.BAD_GROUP_NAME((object) groupName));
    }

    public static bool IsValidGroupName(ref string groupName)
    {
      if (groupName == null)
        throw new ArgumentNullException(nameof (groupName));
      groupName = groupName.Trim();
      return groupName.Length != 0 && groupName.Length <= 256 && groupName[groupName.Length - 1] != '.' && groupName.IndexOfAny(SidIdentityHelper.IllegalNameChars) < 0 && !ArgumentUtility.IsInvalidString(groupName, false) && !VssStringComparer.ReservedGroupName.Equals(groupName, PermissionNamespaces.Global) && !VssStringComparer.ReservedGroupName.Equals(groupName, GroupWellKnownShortNames.NamespaceAdministratorsGroup) && !VssStringComparer.ReservedGroupName.Equals(groupName, GroupWellKnownShortNames.ServiceUsersGroup) && !VssStringComparer.ReservedGroupName.Equals(groupName, GroupWellKnownShortNames.EveryoneGroup);
    }

    public static void CheckGroupDescription(ref string groupDescription)
    {
      if (groupDescription == null)
        groupDescription = string.Empty;
      groupDescription = groupDescription.Trim();
      if (groupDescription.Length > 256 || ArgumentUtility.IsInvalidString(groupDescription, true, true))
        throw new ArgumentException(TFCommonResources.BAD_GROUP_DESCRIPTION((object) groupDescription));
    }

    public static string TruncateGroupDescription(string groupDescription) => TFCommonUtil.TruncateString(groupDescription, 256);

    public static void CheckDisplayName(ref string name)
    {
      if (name == null)
        throw new ArgumentNullException(nameof (name));
      name = name.Trim();
      if (name.Length == 0 || ArgumentUtility.IsInvalidString(name, false, true))
        throw new ArgumentException(TFCommonResources.BAD_DISPLAY_NAME((object) name));
    }

    public static void CheckProjectUri(ref string projectUri, bool allowNullOrEmpty)
    {
      projectUri = projectUri != null ? projectUri.Trim() : string.Empty;
      if (string.IsNullOrEmpty(projectUri) && !allowNullOrEmpty)
        throw new ArgumentNullException(nameof (projectUri));
    }

    public static void CheckMembershipQuery(MembershipQuery queryMembership)
    {
      if (queryMembership != MembershipQuery.None && queryMembership != MembershipQuery.Direct && queryMembership != MembershipQuery.Expanded)
        throw new ArgumentOutOfRangeException(nameof (queryMembership));
    }

    public static void CheckSidNullEmpty(string sid, string paramName)
    {
      sid = sid != null ? sid.Trim() : throw new ArgumentNullException(paramName);
      if (sid.Length == 0)
        throw new ArgumentException(TFCommonResources.BAD_SID((object) sid, (object) paramName), paramName);
    }

    public static SecurityIdentifier CheckSid(string sid, string paramName)
    {
      TFCommonUtil.CheckSidNullEmpty(sid, paramName);
      return new SecurityIdentifier(sid);
    }

    public static void CanonicalizeDacl(NativeObjectSecurity objectSecurity)
    {
      ArgumentUtility.CheckForNull<NativeObjectSecurity>(objectSecurity, nameof (objectSecurity));
      if (objectSecurity.AreAccessRulesCanonical)
        return;
      RawSecurityDescriptor securityDescriptor = new RawSecurityDescriptor(objectSecurity.GetSecurityDescriptorSddlForm(AccessControlSections.Access));
      List<CommonAce> commonAceList1 = new List<CommonAce>();
      List<CommonAce> commonAceList2 = new List<CommonAce>();
      List<CommonAce> commonAceList3 = new List<CommonAce>();
      List<CommonAce> commonAceList4 = new List<CommonAce>();
      List<CommonAce> commonAceList5 = new List<CommonAce>();
      foreach (CommonAce commonAce in (GenericAcl) securityDescriptor.DiscretionaryAcl)
      {
        if ((commonAce.AceFlags & AceFlags.Inherited) == AceFlags.Inherited)
        {
          commonAceList3.Add(commonAce);
        }
        else
        {
          switch (commonAce.AceType)
          {
            case AceType.AccessAllowed:
              commonAceList4.Add(commonAce);
              continue;
            case AceType.AccessDenied:
              commonAceList1.Add(commonAce);
              continue;
            case AceType.AccessAllowedObject:
              commonAceList5.Add(commonAce);
              continue;
            case AceType.AccessDeniedObject:
              commonAceList2.Add(commonAce);
              continue;
            default:
              continue;
          }
        }
      }
      int aceIndex = 0;
      RawAcl newDacl = new RawAcl(securityDescriptor.DiscretionaryAcl.Revision, securityDescriptor.DiscretionaryAcl.Count);
      commonAceList1.ForEach((Action<CommonAce>) (x => newDacl.InsertAce(aceIndex++, (GenericAce) x)));
      commonAceList2.ForEach((Action<CommonAce>) (x => newDacl.InsertAce(aceIndex++, (GenericAce) x)));
      commonAceList4.ForEach((Action<CommonAce>) (x => newDacl.InsertAce(aceIndex++, (GenericAce) x)));
      commonAceList5.ForEach((Action<CommonAce>) (x => newDacl.InsertAce(aceIndex++, (GenericAce) x)));
      commonAceList3.ForEach((Action<CommonAce>) (x => newDacl.InsertAce(aceIndex++, (GenericAce) x)));
      if (aceIndex != securityDescriptor.DiscretionaryAcl.Count)
        return;
      securityDescriptor.DiscretionaryAcl = newDacl;
      objectSecurity.SetSecurityDescriptorSddlForm(securityDescriptor.GetSddlForm(AccessControlSections.Access));
    }

    public static string CombinePaths(string path1, string path2)
    {
      if (string.IsNullOrEmpty(path1))
        return path2;
      if (string.IsNullOrEmpty(path2))
        return path1;
      for (int index = 0; index < TFCommonUtil.s_trimChars.Length; ++index)
      {
        char trimChar = TFCommonUtil.s_trimChars[index];
        if ((int) path1[path1.Length - 1] == (int) trimChar)
          return path1.Length == 1 || path1[path1.Length - 2] != '/' && path1[path1.Length - 2] != '\\' ? path1 + path2.TrimStart(TFCommonUtil.s_trimChars) : path1.TrimEnd(TFCommonUtil.s_trimChars) + TFCommonUtil.s_trimStrings[index] + path2.TrimStart(TFCommonUtil.s_trimChars);
        if (path1.LastIndexOf(trimChar) >= 0 || index == 1)
        {
          char ch = path1[path1.Length - 1];
          return (int) path2[0] == (int) trimChar && (path2.Length == 1 || path2[1] != '/' && path2[1] != '\\') && ch != '/' && ch != '\\' ? path1 + path2 : path1.TrimEnd(TFCommonUtil.s_trimChars) + TFCommonUtil.s_trimStrings[index] + path2.TrimStart(TFCommonUtil.s_trimChars);
        }
      }
      throw new InvalidOperationException("We should never hit this line of code.");
    }

    public static string RemoveInvalidXmlChars(string strIn, bool allowCrLf = false)
    {
      StringBuilder stringBuilder = new StringBuilder(strIn.Length);
      for (int index = 0; index < strIn.Length; ++index)
      {
        char c = strIn[index];
        if (!ArgumentUtility.IsIllegalInputCharacter(c, allowCrLf) && !char.IsLowSurrogate(c))
        {
          if (char.IsHighSurrogate(c))
          {
            if (char.IsSurrogatePair(strIn, index))
            {
              stringBuilder.Append(c);
              c = strIn[++index];
            }
            else
              continue;
          }
          stringBuilder.Append(c);
        }
      }
      return stringBuilder.ToString();
    }

    [Obsolete("Use method in Microsft.VisualStudio.Services.Common.ArgumentUtility instead.")]
    public static void CheckForOutOfRange<T>(T var, string varName, T minimum) where T : IComparable<T> => ArgumentUtility.CheckForOutOfRange<T>(var, varName, minimum);

    [Obsolete("Use method in Microsft.VisualStudio.Services.Common.ArgumentUtility instead.")]
    public static void CheckForOutOfRange<T>(T var, string varName, T minimum, T maximum) where T : IComparable<T> => ArgumentUtility.CheckForOutOfRange<T>(var, varName, minimum, maximum);

    [Obsolete("Use method in Microsft.VisualStudio.Services.Common.ArgumentUtility instead.")]
    public static void CheckForOutOfRange(int var, string varName, int minimum) => ArgumentUtility.CheckForOutOfRange(var, varName, minimum);

    [Obsolete("Use method in Microsft.VisualStudio.Services.Common.ArgumentUtility instead.")]
    public static void CheckForOutOfRange(int var, string varName, int minimum, int maximum) => ArgumentUtility.CheckForOutOfRange(var, varName, minimum, maximum);

    [Obsolete("Use method in Microsft.VisualStudio.Services.Common.ArgumentUtility instead.")]
    public static void CheckForOutOfRange(long var, string varName, long minimum) => ArgumentUtility.CheckForOutOfRange(var, varName, minimum);

    [Obsolete("Use method in Microsft.VisualStudio.Services.Common.ArgumentUtility instead.")]
    public static void CheckForOutOfRange(long var, string varName, long minimum, long maximum) => ArgumentUtility.CheckForOutOfRange(var, varName, minimum, maximum);

    [Obsolete("Use method in Microsft.VisualStudio.Services.Common.ArgumentUtility instead.")]
    public static void CheckForDateTimeRange(
      DateTime var,
      string varName,
      DateTime minimum,
      DateTime maximum)
    {
      ArgumentUtility.CheckForDateTimeRange(var, varName, minimum, maximum);
    }

    [Obsolete("Use method in Microsft.VisualStudio.Services.Common.ArgumentUtility instead.")]
    public static void CheckGreaterThanOrEqualToZero(float value, string valueName) => ArgumentUtility.CheckGreaterThanOrEqualToZero(value, valueName);

    [Obsolete("Use method in Microsft.VisualStudio.Services.Common.ArgumentUtility instead.")]
    public static void CheckGreaterThanZero(float value, string valueName) => ArgumentUtility.CheckGreaterThanZero(value, valueName);

    [Obsolete("Use method in Microsft.VisualStudio.Services.Common.ArgumentUtility instead.")]
    public static void CheckForNull(object var, string varName) => ArgumentUtility.CheckForNull<object>(var, varName);

    [Obsolete("Use method in Microsft.VisualStudio.Services.Common.ArgumentUtility instead.")]
    public static void EnsureIsNull(object var, string varName) => ArgumentUtility.EnsureIsNull(var, varName);

    [Obsolete("Use method in Microsft.VisualStudio.Services.Common.ArgumentUtility instead.")]
    public static void CheckStringCasing(string stringVar, string varName, bool checkForLowercase = true) => ArgumentUtility.CheckStringCasing(stringVar, varName, checkForLowercase);

    [Obsolete("Use method in Microsft.VisualStudio.Services.Common.ArgumentUtility instead.")]
    public static void CheckForEmptyGuid(Guid guid, string varName) => ArgumentUtility.CheckForEmptyGuid(guid, varName);

    [Obsolete("Use CheckEnumerableForNullOrEmpty method in Microsft.VisualStudio.Services.Common.ArgumentUtility instead.")]
    public static void CheckArrayForNullOrEmpty(object[] array, string arrayName) => ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) array, arrayName);

    [Obsolete("Use CheckEnumerableForNullOrEmpty method in Microsft.VisualStudio.Services.Common.ArgumentUtility instead.")]
    public static void CheckArrayForNullOrEmpty(int[] array, string arrayName) => ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) array, arrayName);

    [Obsolete("Use CheckEnumerableForNullOrEmpty method in Microsft.VisualStudio.Services.Common.ArgumentUtility instead.")]
    public static void CheckArrayForNullOrEmpty<T>(T[] array, string arrayName) => ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) array, arrayName);

    [Obsolete("Use CheckEnumerableForNullOrEmpty method in Microsft.VisualStudio.Services.Common.ArgumentUtility instead.")]
    public static void CheckListForNullOrEmpty(IList list, string listName) => ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) list, listName);

    [Obsolete("Use method in Microsft.VisualStudio.Services.Common.ArgumentUtility instead.")]
    public static void CheckEnumerableForNullOrEmpty(IEnumerable enumerable, string enumerableName) => ArgumentUtility.CheckEnumerableForNullOrEmpty(enumerable, enumerableName);

    [Obsolete("Use method in Microsft.VisualStudio.Services.Common.ArgumentUtility instead.")]
    public static void CheckEnumerableForEmpty(IEnumerable enumerable, string enumerableName) => ArgumentUtility.CheckEnumerableForEmpty(enumerable, enumerableName);

    [Obsolete("Use method in Microsft.VisualStudio.Services.Common.ArgumentUtility instead.")]
    public static void CheckStringForNullOrEmpty(string stringVar, string stringVarName) => ArgumentUtility.CheckStringForNullOrEmpty(stringVar, stringVarName);

    [Obsolete("Use method in Microsft.VisualStudio.Services.Common.ArgumentUtility instead.")]
    public static void CheckStringForNullOrWhiteSpace(string stringVar, string stringVarName) => ArgumentUtility.CheckStringForNullOrWhiteSpace(stringVar, stringVarName);

    [Obsolete("Use method in Microsft.VisualStudio.Services.Common.ArgumentUtility instead.")]
    public static void CheckStringExactLength(string stringVar, int length, string stringVarName) => ArgumentUtility.CheckStringExactLength(stringVar, length, stringVarName);

    [Obsolete("Use method in Microsft.VisualStudio.Services.Common.ArgumentUtility instead.")]
    public static void CheckForBothStringsNullOrEmpty(
      string var1,
      string varName1,
      string var2,
      string varName2)
    {
      ArgumentUtility.CheckForBothStringsNullOrEmpty(var1, varName1, var2, varName2);
    }

    [Obsolete("Use method in Microsft.VisualStudio.Services.Common.ArgumentUtility instead.")]
    public static void CheckStringForAnyWhiteSpace(string stringVar, string stringVarName) => ArgumentUtility.CheckStringForAnyWhiteSpace(stringVar, stringVarName);

    [Obsolete("Use method in Microsft.VisualStudio.Services.Common.ArgumentUtility instead.")]
    public static void CheckType<T>(object var, string varName, string typeName) => ArgumentUtility.CheckType<T>(var, varName, typeName);

    [Obsolete("Use method in Microsft.VisualStudio.Services.Common.ArgumentUtility instead.")]
    public static void CheckForDefinedEnum<TEnum>(TEnum value, string enumVarName) where TEnum : struct => ArgumentUtility.CheckForDefinedEnum<TEnum>(value, enumVarName);

    public static bool IsLegalComputerName(string computerName) => TFCommonUtil.IsLegalComputerName(computerName, out string _);

    public static bool IsLegalComputerName(string computerName, out string error)
    {
      error = (string) null;
      if (string.IsNullOrEmpty(computerName))
      {
        error = TFCommonResources.InvalidComputerName((object) string.Empty);
        return false;
      }
      if (computerName.Length > 31)
      {
        error = TFCommonResources.InvalidComputerNameTooLong((object) computerName, (object) 31);
        return false;
      }
      if (computerName.IndexOfAny(TFCommonUtil.s_illegalComputerNameChars) < 0)
        return true;
      error = TFCommonResources.InvalidComputerNameInvalidCharacters((object) computerName);
      return false;
    }

    public static bool IsLegalIdentity(string identity)
    {
      int num = identity.IndexOf('\\');
      if (num >= 0 && num != identity.Length - 1)
        identity = identity.Substring(num + 1);
      int length = identity.IndexOf(':');
      if (length > 0 && length != identity.Length - 1)
      {
        for (int index = length + 1; index < identity.Length; ++index)
        {
          if (identity[index] < '0' || identity[index] > '9')
            return false;
        }
        identity = identity.Substring(0, length);
      }
      return !string.IsNullOrEmpty(identity) && identity.Length <= 256 && identity.IndexOfAny(TFCommonUtil.s_illegalIdentityNameChars) == -1;
    }

    public static bool IsLegalXmlString(string str)
    {
      foreach (char ch in str)
      {
        if (ch < ' ' && ch != '\t' && ch != '\n' && ch != '\r' || ch == '\uFFFE' || ch == char.MaxValue)
          return false;
      }
      return true;
    }

    public static void AddXmlAttribute(XmlNode node, string attrName, string value)
    {
      if (value == null)
        return;
      XmlAttribute attribute = node.OwnerDocument.CreateAttribute((string) null, attrName, (string) null);
      node.Attributes.Append(attribute);
      attribute.InnerText = value;
    }

    public static string EnumToString<T>(Enum value) => Enum.Format(typeof (T), (object) value, "G");

    public static T EnumFromString<T>(string s) => (T) Enum.Parse(typeof (T), s, true);

    public static void RunApp(
      string appExe,
      string arguments,
      EventHandler exitedHandler,
      bool wait)
    {
      TFCommonUtil.RunApp(appExe, arguments, exitedHandler, wait, true);
    }

    public static void RunApp(
      string appExe,
      string arguments,
      EventHandler exitedHandler,
      bool wait,
      bool defaultStartParameters)
    {
      TFCommonUtil.RunApp(appExe, arguments, exitedHandler, wait, defaultStartParameters, false);
    }

    public static void RunApp(
      string appExe,
      string arguments,
      EventHandler exitedHandler,
      bool wait,
      bool defaultStartParameters,
      bool ignoreExitCode)
    {
      int num = 0;
      try
      {
        Process process = TFCommonUtil.BeginRunApp(appExe, arguments, defaultStartParameters);
        if (process != null)
        {
          if (exitedHandler != null)
          {
            process.EnableRaisingEvents = true;
            process.Exited += exitedHandler;
          }
          if (wait)
            num = TFCommonUtil.EndRunApp(process);
        }
      }
      catch (Exception ex)
      {
        if (exitedHandler != null)
          exitedHandler((object) null, EventArgs.Empty);
        throw new TeamFoundationServerException(TFCommonResources.UnableToRunApp((object) appExe), ex);
      }
      if (num != 0 && !ignoreExitCode)
        throw new TeamFoundationServerException(TFCommonResources.CommandFailedWithExitCode((object) appExe, (object) num));
    }

    public static Process BeginRunApp(string appExe, string arguments) => TFCommonUtil.BeginRunApp(appExe, arguments, true);

    public static Process BeginRunApp(string appExe, string arguments, bool defaultStartParameters)
    {
      ProcessStartInfo startInfo = new ProcessStartInfo(appExe, arguments);
      if (!defaultStartParameters)
      {
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true;
      }
      return Process.Start(startInfo);
    }

    public static int EndRunApp(Process process)
    {
      using (process)
      {
        process.WaitForExit();
        return process.ExitCode;
      }
    }

    private static bool AreByteArraysEqual(byte[] a1, byte[] a2)
    {
      if (a1.Length != a2.Length)
        return false;
      return a1.Length == 0 || TFCommonUtil.AreByteArraysEquals(a1, a2, a1.Length);
    }

    private static unsafe bool AreByteArraysEquals(byte[] a1, byte[] a2, int length)
    {
      fixed (byte* numPtr1 = &a1[0])
        fixed (byte* numPtr2 = &a2[0])
        {
          byte* numPtr3 = numPtr1;
          byte* numPtr4 = numPtr2;
          for (int index = length >> 2; index > 0; --index)
          {
            if (*(int*) numPtr3 != *(int*) numPtr4)
              return false;
            numPtr3 += 4;
            numPtr4 += 4;
          }
          for (int index = length & 3; index > 0; --index)
          {
            if ((int) *numPtr3 != (int) *numPtr4)
              return false;
            ++numPtr3;
            ++numPtr4;
          }
        }
      return true;
    }

    public static void ConvertFileEncoding(
      string sourcePath,
      string destPath,
      Encoding sourceEncoding,
      Encoding destEncoding)
    {
      TFCommonUtil.ConvertFileEncoding(sourcePath, destPath, sourceEncoding, destEncoding, true);
    }

    public static void ConvertFileEncoding(
      string sourcePath,
      string destPath,
      Encoding sourceEncoding,
      Encoding destEncoding,
      bool includeBom)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(sourcePath, nameof (sourcePath));
      ArgumentUtility.CheckStringForNullOrEmpty(destPath, nameof (destPath));
      ArgumentUtility.CheckForNull<Encoding>(sourceEncoding, nameof (sourceEncoding));
      ArgumentUtility.CheckForNull<Encoding>(destEncoding, nameof (destEncoding));
      if (Directory.Exists(sourcePath))
        throw new TeamFoundationServerException(TFCommonResources.FoundDirectoryExpectedFilePath((object) sourcePath));
      if (Directory.Exists(destPath))
        throw new TeamFoundationServerException(TFCommonResources.FoundDirectoryExpectedFilePath((object) destPath));
      if (sourceEncoding == destEncoding)
      {
        if (FileSpec.Equals(sourcePath, destPath))
          return;
        FileSpec.CopyFile(sourcePath, destPath, true);
      }
      else
      {
        string str = (string) null;
        try
        {
          byte[] preamble1 = sourceEncoding.GetPreamble();
          byte[] preamble2 = destEncoding.GetPreamble();
          byte[] numArray = new byte[preamble1.Length];
          str = FileSpec.GetTempFileName();
          using (FileStream fileStream1 = new FileStream(sourcePath, FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read))
          {
            using (FileStream fileStream2 = new FileStream(str, FileMode.Create, System.IO.FileAccess.ReadWrite, System.IO.FileShare.None))
            {
              int num = fileStream1.Read(numArray, 0, numArray.Length);
              if (numArray.Length != 0 && num == numArray.Length && TFCommonUtil.AreByteArraysEqual(preamble1, numArray))
              {
                fileStream2.Write(preamble2, 0, preamble2.Length);
              }
              else
              {
                fileStream1.Seek(0L, SeekOrigin.Begin);
                if (includeBom && (destEncoding.CodePage == Encoding.Unicode.CodePage || destEncoding.CodePage == Encoding.UTF7.CodePage || destEncoding.CodePage == Encoding.UTF8.CodePage || destEncoding.CodePage == Encoding.UTF32.CodePage || destEncoding.CodePage == Encoding.BigEndianUnicode.CodePage) && preamble2.Length != 0)
                  fileStream2.Write(preamble2, 0, preamble2.Length);
              }
              using (StreamReader streamReader = new StreamReader((Stream) fileStream1, sourceEncoding, false))
              {
                char[] chArray = new char[131072];
                int count;
                while ((count = streamReader.ReadBlock(chArray, 0, chArray.Length)) > 0)
                {
                  byte[] bytes = destEncoding.GetBytes(chArray, 0, count);
                  fileStream2.Write(bytes, 0, bytes.Length);
                }
              }
            }
          }
          FileSpec.MoveFile(str, destPath, true);
        }
        finally
        {
          FileSpec.DeleteFile(str);
        }
      }
    }

    public static FileStream OpenFile(string path, System.IO.FileShare sharing, bool saveFile) => TFCommonUtil.OpenFileHelper(path, sharing, saveFile, false, out XmlDocument _);

    public static XmlDocument OpenXmlFile(
      out FileStream file,
      string path,
      System.IO.FileShare sharing,
      bool saveFile)
    {
      XmlDocument xmlDocument;
      file = TFCommonUtil.OpenFileHelper(path, sharing, saveFile, true, out xmlDocument);
      return xmlDocument;
    }

    private static FileStream OpenFileHelper(
      string path,
      System.IO.FileShare sharing,
      bool saveFile,
      bool loadAsXmlDocument,
      out XmlDocument xmlDocument)
    {
      FileStream input = (FileStream) null;
      xmlDocument = (XmlDocument) null;
      if (string.IsNullOrEmpty(path))
      {
        TeamFoundationTrace.Verbose("The cache file is unavailable.");
        return (FileStream) null;
      }
      TeamFoundationTrace.Verbose("Using cache file path " + path);
      if (!saveFile)
      {
        try
        {
          if (!File.Exists(path))
            return (FileStream) null;
        }
        catch (Exception ex)
        {
          return (FileStream) null;
        }
      }
      int num = 0;
      bool flag = true;
      Random random = (Random) null;
      while (flag)
      {
        if (num >= 9999)
          flag = false;
        try
        {
          System.IO.FileAccess access = System.IO.FileAccess.Read;
          if (saveFile && File.Exists(path))
          {
            File.SetAttributes(path, System.IO.FileAttributes.Normal);
            access = System.IO.FileAccess.ReadWrite;
          }
          input = new FileStream(path, FileMode.Open, access, sharing);
          if (loadAsXmlDocument)
          {
            XmlReaderSettings settings = new XmlReaderSettings()
            {
              DtdProcessing = DtdProcessing.Prohibit,
              XmlResolver = (XmlResolver) null
            };
            using (XmlReader reader = XmlReader.Create((Stream) input, settings))
            {
              xmlDocument = new XmlDocument();
              xmlDocument.Load(reader);
            }
          }
          return input;
        }
        catch (System.OperationCanceledException ex)
        {
          throw;
        }
        catch (Exception ex1)
        {
          switch (ex1)
          {
            case FileNotFoundException _:
            case DirectoryNotFoundException _:
            case XmlException _:
              if (input != null)
              {
                input.Close();
                input = (FileStream) null;
              }
              if (!saveFile)
                return (FileStream) null;
              string directoryName = FileSpec.GetDirectoryName(path);
              try
              {
                Directory.CreateDirectory(directoryName);
                xmlDocument = (XmlDocument) null;
                return new FileStream(path, FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None);
              }
              catch (IOException ex2)
              {
                flag = true;
                break;
              }
            default:
              if (!flag)
              {
                input?.Close();
                throw new TeamFoundationServerException(TFCommonResources.ErrorReadingFile((object) FileSpec.GetFileName(path), (object) ex1.Message), ex1);
              }
              break;
          }
        }
        if (random == null)
          random = new Random();
        int millisecondsTimeout = random.Next(1, 150);
        num += millisecondsTimeout;
        Thread.Sleep(millisecondsTimeout);
      }
      return (FileStream) null;
    }

    public static int GetAppSettingAsInt(string key, int defaultValue)
    {
      string appSetting = TFCommonUtil.GetAppSetting(key, (string) null);
      int result;
      if (appSetting == null || !int.TryParse(appSetting, out result))
        result = defaultValue;
      return result;
    }

    public static long GetAppSettingAsLong(string key, long defaultValue)
    {
      string appSetting = TFCommonUtil.GetAppSetting(key, (string) null);
      long result;
      if (appSetting == null || !long.TryParse(appSetting, out result))
        result = defaultValue;
      return result;
    }

    public static bool GetAppSettingAsBool(string key, bool defaultValue)
    {
      string appSetting = TFCommonUtil.GetAppSetting(key, (string) null);
      bool result;
      if (appSetting == null || !bool.TryParse(appSetting, out result))
        result = defaultValue;
      return result;
    }

    public static string GetAppSetting(string key, string defaultValue)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(key, nameof (key));
      string name = ConfigurationManager.AppSettings[key] ?? defaultValue;
      if (name != null)
        name = Environment.ExpandEnvironmentVariables(name);
      return name;
    }

    public static T[] CombineArrays<T>(T[] first, T[] second)
    {
      if (first == null)
        throw new ArgumentNullException(nameof (first));
      if (second == null)
        throw new ArgumentNullException(nameof (second));
      T[] objArray = new T[first.Length + second.Length];
      first.CopyTo((Array) objArray, 0);
      second.CopyTo((Array) objArray, first.Length);
      return objArray;
    }

    public static T[] CopyCollection<T>(ICollection<T> original)
    {
      T[] array = new T[original.Count];
      original.CopyTo(array, 0);
      return array;
    }

    public static T[] CopyCollection<T>(ICollection original)
    {
      T[] objArray = new T[original.Count];
      original.CopyTo((Array) objArray, 0);
      return objArray;
    }

    public static Process CurrentProcess
    {
      get
      {
        if (TFCommonUtil.s_process == null)
          TFCommonUtil.s_process = Process.GetCurrentProcess();
        return TFCommonUtil.s_process;
      }
    }

    public static void SafeRelease(IntPtr punk)
    {
      if (!(punk != IntPtr.Zero))
        return;
      Marshal.Release(punk);
    }

    public static int TranslateEnum(Type enumClass, string[] array, int allPermissions)
    {
      int num = 0;
      if (array != null)
      {
        for (int index = 0; index < array.Length; ++index)
        {
          try
          {
            if (string.Equals(array[index], "*", StringComparison.OrdinalIgnoreCase))
              return allPermissions;
            if (int.TryParse(array[index], out int _))
              throw new ArgumentException(TFCommonResources.InvalidEnumerationValue((object) array[index], (object) enumClass.Name), array[index]);
            num |= (int) Enum.Parse(enumClass, array[index], true);
          }
          catch (ArgumentException ex)
          {
            throw new ArgumentException(TFCommonResources.InvalidEnumerationValue((object) array[index], (object) enumClass.Name), array[index], (Exception) ex);
          }
        }
      }
      return num;
    }

    public static List<string> TranslateEnum(Type enumClass, int bits)
    {
      List<string> stringList = new List<string>(32);
      for (int index = 0; index < 32; ++index)
      {
        if ((bits & 1 << index) != 0)
        {
          string name = Enum.GetName(enumClass, (object) (1 << index));
          if (name != null)
            stringList.Add(name);
        }
      }
      return stringList;
    }

    internal static void SafeDispose(IDisposable instance) => instance?.Dispose();

    public static Uri StripEnd(Uri uri, string[] stringsToStrip)
    {
      string absoluteUri = uri.AbsoluteUri;
      string uriString = TFCommonUtil.StripEnd(absoluteUri, stringsToStrip);
      return absoluteUri != uriString ? new Uri(uriString) : uri;
    }

    public static string StripEnd(string stringToStrip, string[] stringsToStrip)
    {
      string str1 = stringToStrip.TrimEnd();
      foreach (string str2 in stringsToStrip)
      {
        if (!string.IsNullOrEmpty(str2) && str1.EndsWith(str2, StringComparison.OrdinalIgnoreCase))
          return str1.Substring(0, stringToStrip.Length - str2.Length);
      }
      return stringToStrip;
    }

    public static string EscapeString(string text, char escapeChar)
    {
      if (string.IsNullOrEmpty(text))
        return text;
      string oldValue = new string(escapeChar, 1);
      string newValue = new string(escapeChar, 2);
      return text.Replace(oldValue, newValue);
    }

    public static string ComputeFullDatabaseName(
      string databaseLabel,
      string databaseRootname,
      string databaseSuffix)
    {
      return TFCommonUtil.ComputeFullDatabaseName(databaseLabel, databaseRootname, 1, databaseSuffix);
    }

    public static string ComputeFullDatabaseName(
      string databaseLabel,
      string databaseRootname,
      int attemptNumber,
      string databaseSuffix)
    {
      string fullDatabaseName;
      if (attemptNumber == 1)
        fullDatabaseName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}{2}{3}", (object) "AzureDevOps_", (object) databaseLabel, (object) databaseRootname, (object) databaseSuffix);
      else
        fullDatabaseName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}{2}{3}{4}", (object) "AzureDevOps_", (object) databaseLabel, (object) databaseRootname, (object) attemptNumber, (object) databaseSuffix);
      return fullDatabaseName;
    }

    public static T ReadRegistryValue<T>(RegistryKey hiveRoot, string key, string valueName)
    {
      object obj1 = (object) null;
      using (RegistryKey registryKey = hiveRoot.OpenSubKey(key))
      {
        if (registryKey != null)
          obj1 = registryKey.GetValue(valueName);
      }
      return obj1 == null || !(obj1 is T obj2) ? default (T) : obj2;
    }

    internal static bool AreTypesEquivalent(Type a, Type b)
    {
      if (a == b)
        return true;
      return a.GetTypeInfo().IsAssignableFrom(b.GetTypeInfo()) && b.GetTypeInfo().IsAssignableFrom(a.GetTypeInfo());
    }

    public static byte[] GetSqlTimestamp(string data)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(data, nameof (data));
      if (data.Length != 8)
        throw new ArgumentException(TFCommonResources.InvalidSqlTimestampString((object) data));
      byte[] sqlTimestamp = new byte[8];
      for (int index = 0; index < 8; ++index)
        sqlTimestamp[index] = (byte) data[index];
      return sqlTimestamp;
    }

    public static void Retry(
      int count,
      int retryDelay,
      Action<int> action,
      Func<Exception, int, bool> exceptionAction = null)
    {
      ArgumentUtility.CheckForNull<Action<int>>(action, nameof (action));
      ArgumentUtility.CheckForOutOfRange(count, nameof (count), 1);
      ArgumentUtility.CheckForOutOfRange(retryDelay, nameof (retryDelay), 0);
      for (int index = 1; index <= count; ++index)
      {
        try
        {
          action(index);
          break;
        }
        catch (Exception ex)
        {
          bool flag = true;
          if (exceptionAction != null)
            flag = exceptionAction(ex, index);
          if (flag && index < count)
          {
            if (retryDelay > 0)
              Thread.Sleep(retryDelay);
          }
          else
            throw;
        }
      }
    }

    private static string TruncateString(string value, int limitingLength) => string.IsNullOrWhiteSpace(value) || value.Length < limitingLength ? value : value.Substring(0, limitingLength);
  }
}

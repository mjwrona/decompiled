// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Package.SortableVersionBuilder
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Package
{
  public class SortableVersionBuilder
  {
    public const int MaxSortableVersionLength = 127;
    public const char PrereleasePrefix = '-';
    public const char BuildPrefix = '+';
    public const char PartSeparator = '!';
    public const char TokenSeparator = '.';
    public const char EmptyPart = '~';
    private const int IntegerStringWidth = 10;
    private int m_numberVersionPartTokens;

    public SortableVersionBuilder(int numberVersionPartTokens) => this.m_numberVersionPartTokens = numberVersionPartTokens;

    public virtual Exception TryGetSortableVersion(
      string normalizedVersion,
      out string sortableVersion)
    {
      sortableVersion = (string) null;
      if (normalizedVersion == null)
        return (Exception) new ArgumentNullException(nameof (normalizedVersion));
      if (string.IsNullOrWhiteSpace(normalizedVersion))
        return (Exception) new ArgumentException(SearchSharedWebApiResources.EmptyVersionNotAllowed, nameof (normalizedVersion));
      Exception sortableVersion1 = this.TryValidateAllowedCharacters(normalizedVersion);
      if (sortableVersion1 != null)
        return sortableVersion1;
      Exception sortableVersion2 = this.TryValidateVersionPartContainsOnlyPositiveIntegers(normalizedVersion);
      if (sortableVersion2 != null)
        return sortableVersion2;
      string versionPart1;
      Exception versionPart2 = this.TryGetVersionPart(normalizedVersion, out versionPart1);
      if (versionPart2 != null)
        return versionPart2;
      string extendVersionPart;
      Exception expectedNumTokens = this.TryExtendVersionPartToExpectedNumTokens(versionPart1, out extendVersionPart);
      if (expectedNumTokens != null)
        return expectedNumTokens;
      string str = FormattableString.Invariant(FormattableStringFactory.Create("{0}{1}{2}{3}{4}", (object) this.PadIntegersInPart(extendVersionPart), (object) '!', (object) this.PadIntegersInPart(this.GetPrereleasePart(normalizedVersion)), (object) '!', (object) this.PadIntegersInPart(this.GetBuildPart(normalizedVersion))));
      if (str.Length > (int) sbyte.MaxValue)
        return (Exception) new InvalidVersionException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, SearchSharedWebApiResources.Error_PackageVersionExceedsMaximumLength, (object) normalizedVersion));
      sortableVersion = str;
      return (Exception) null;
    }

    public virtual string GetSortableVersion(string normalizedVersion)
    {
      string sortableVersion;
      SortableVersionBuilder.ThrowIfNotNull(this.TryGetSortableVersion(normalizedVersion, out sortableVersion));
      return sortableVersion;
    }

    protected Exception TryGetVersionPart(string versionString, out string versionPart)
    {
      versionPart = (string) null;
      string[] source = versionString.Split('-', '+');
      if (!((IEnumerable<string>) source).Any<string>())
        return (Exception) new InvalidVersionException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, SearchSharedWebApiResources.Error_InvalidVersion, (object) versionString));
      versionPart = source[0];
      return (Exception) null;
    }

    protected string GetPrereleasePart(string versionString)
    {
      int num1 = versionString.IndexOf('-');
      if (num1 == -1 || num1 == versionString.Length - 1)
        return '~'.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      int num2 = versionString.IndexOf('+');
      int startIndex = num1 + 1;
      int length = (num2 == -1 ? versionString.Length - 1 : num2 - 1) - startIndex + 1;
      return length <= 0 ? '~'.ToString((IFormatProvider) CultureInfo.InvariantCulture) : versionString.Substring(startIndex, length);
    }

    protected string GetBuildPart(string versionString)
    {
      int num = versionString.IndexOf('+');
      return num == -1 || num == versionString.Length - 1 ? '~'.ToString((IFormatProvider) CultureInfo.InvariantCulture) : versionString.Substring(num + 1);
    }

    protected Exception TryExtendVersionPartToExpectedNumTokens(
      string versionPart,
      out string extendVersionPart)
    {
      extendVersionPart = (string) null;
      IEnumerable<string> strings = ((IEnumerable<string>) versionPart.Split('.')).AsEnumerable<string>();
      if (!strings.Any<string>())
        return (Exception) new InvalidVersionException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, SearchSharedWebApiResources.Error_InvalidVersion, (object) versionPart));
      IEnumerable<string> values = strings.Concat<string>(Enumerable.Repeat<string>("0", this.m_numberVersionPartTokens)).Take<string>(this.m_numberVersionPartTokens);
      extendVersionPart = string.Join('.'.ToString((IFormatProvider) CultureInfo.InvariantCulture), values);
      return (Exception) null;
    }

    protected string PadIntegersInPart(string part)
    {
      string[] strArray = part.Split('.');
      List<string> values = new List<string>();
      foreach (string s in strArray)
      {
        int result;
        if (int.TryParse(s, out result) && result >= 0)
          values.Add(s.PadLeft(10, '0'));
        else
          values.Add(s);
      }
      return string.Join('.'.ToString((IFormatProvider) CultureInfo.InvariantCulture), (IEnumerable<string>) values);
    }

    protected Exception TryValidateAllowedCharacters(string version) => version.IndexOfAny(new char[2]
    {
      '!',
      '~'
    }) > -1 ? (Exception) new InvalidVersionException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, SearchSharedWebApiResources.Error_InvalidVersion, (object) version)) : (Exception) null;

    protected Exception TryValidateVersionPartContainsOnlyPositiveIntegers(string version)
    {
      string versionPart1;
      Exception versionPart2 = this.TryGetVersionPart(version, out versionPart1);
      if (versionPart2 != null)
        return versionPart2;
      string str = versionPart1;
      char[] chArray = new char[1]{ '.' };
      foreach (string s in str.Split(chArray))
      {
        int result;
        if (!int.TryParse(s, out result) || result < 0)
          return (Exception) new InvalidVersionException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, SearchSharedWebApiResources.Error_InvalidVersion, (object) version));
      }
      return (Exception) null;
    }

    private static void ThrowIfNotNull(Exception e)
    {
      if (e != null)
        throw e;
    }
  }
}

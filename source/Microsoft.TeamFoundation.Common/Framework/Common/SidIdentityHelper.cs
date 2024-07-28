// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.SidIdentityHelper
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.TeamFoundation.Common.Internal;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Security.Principal;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.Framework.Common
{
  public static class SidIdentityHelper
  {
    internal const uint s_identifierAuthorityNullAuthority = 0;
    internal const uint s_identifierAuthorityWorldAuthority = 1;
    internal const uint s_identifierAuthorityLocalAuthority = 2;
    internal const uint s_identifierAuthorityCreatorAuthority = 3;
    internal const uint s_identifierAuthorityNonUniqueAuthority = 4;
    internal const uint s_identifierAuthorityNTAuthority = 5;
    internal const uint s_identifierAuthoritySiteServerAuthority = 6;
    internal const uint s_identifierAuthorityInternetSiteAuthority = 7;
    internal const uint s_identifierAuthorityExchangeAuthority = 8;
    internal const uint s_identifierAuthorityResourceManagerAuthority = 9;
    private const uint s_subAuthority0 = 1551374245;
    private const uint s_subAuthority1 = 1204400969;
    private const uint s_subAuthority2 = 2402986413;
    private const uint s_subAuthority3 = 2179408616;
    private static readonly string s_teamFoundationSidPrefix = "S-1-9-" + 1551374245U.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "-";
    private static readonly string s_wellKnownDomainSid = SidIdentityHelper.s_teamFoundationSidPrefix + 1204400969U.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "-" + 2402986413U.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "-" + 2179408616U.ToString((IFormatProvider) CultureInfo.InvariantCulture);
    private static readonly Guid s_wellKnownDomainId = new Guid("A517785C-C947-49B3-8F3A-A9AD81E722E8");
    private const string s_wellKnownSidType = "-0-";
    private const string s_phantomSidType = "-2-";
    private const string s_aadSidType = "-3-";
    private const uint c_PhantomSidType = 2;
    private const uint c_AadSidType = 3;
    private static readonly string s_wellKnownSidPrefix = SidIdentityHelper.s_wellKnownDomainSid + "-0-";
    private static readonly string s_phantomSidPrefix = SidIdentityHelper.s_wellKnownDomainSid + "-2-";
    private static readonly string s_aadSidPrefix = SidIdentityHelper.s_wellKnownDomainSid + "-3-";
    private static readonly Regex WellKnownGroupPrefixRegex = new Regex("S-1-9-1551374245-[0-9]+-[0-9]+-[0-9]+-[0-9]+-0-0-0-");
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static readonly char[] IllegalNameChars = new char[49]
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
      ',',
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
      '\uFFFE',
      char.MaxValue
    };

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static SecurityIdentifier NewSid(Guid domainId)
    {
      uint[] subAuthorities = new uint[10];
      SidIdentityHelper.FillDomainSid(subAuthorities, domainId);
      subAuthorities[5] = 1U;
      byte[] byteArray = Guid.NewGuid().ToByteArray();
      for (int index = 0; index < 4; ++index)
        subAuthorities[index + 6] = SidIdentityHelper.ArrayToInt(byteArray, index * 4);
      return new SecurityIdentifier(SidIdentityHelper.EncodeSid(9UL, subAuthorities), 0);
    }

    public static string ConstructWellKnownSid(uint sidPoolId, uint specificSidNumber) => SidIdentityHelper.WellKnownSidPrefix + "0-0-" + sidPoolId.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "-" + specificSidNumber.ToString((IFormatProvider) CultureInfo.InvariantCulture);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static SecurityIdentifier ConstructPhantomSid(Guid phantomId) => SidIdentityHelper.ConstructExtendedSid(phantomId, 2U);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static SecurityIdentifier ConstructAadGroupSid(Guid aadGroupId) => SidIdentityHelper.ConstructExtendedSid(aadGroupId, 3U);

    private static SecurityIdentifier ConstructExtendedSid(Guid seed, uint sidType)
    {
      uint[] subAuthorities = new uint[9]
      {
        1551374245U,
        1204400969U,
        2402986413U,
        2179408616U,
        sidType,
        0U,
        0U,
        0U,
        0U
      };
      byte[] byteArray = seed.ToByteArray();
      for (int index = 0; index < 4; ++index)
        subAuthorities[index + 5] = SidIdentityHelper.ArrayToInt(byteArray, index * 4);
      return new SecurityIdentifier(SidIdentityHelper.EncodeSid(9UL, subAuthorities), 0);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Guid DecodePhantomSid(SecurityIdentifierInfo securityIdInfo) => SidIdentityHelper.DecodeExtendedSid(securityIdInfo);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Guid DecodeAadSid(SecurityIdentifierInfo securityIdInfo) => SidIdentityHelper.DecodeExtendedSid(securityIdInfo);

    private static Guid DecodeExtendedSid(SecurityIdentifierInfo securityIdInfo)
    {
      uint[] numArray1 = SidIdentityHelper.DecodeSid(securityIdInfo.GetBinaryForm(), out int _, out ulong _);
      byte[] numArray2 = new byte[16];
      SidIdentityHelper.IntToArray(numArray1[5], numArray2, 0);
      SidIdentityHelper.IntToArray(numArray1[6], numArray2, 4);
      SidIdentityHelper.IntToArray(numArray1[7], numArray2, 8);
      SidIdentityHelper.IntToArray(numArray1[8], numArray2, 12);
      return new Guid(numArray2);
    }

    public static SecurityIdentifier GetDomainSid(Guid domainId)
    {
      uint[] subAuthorities = new uint[5];
      SidIdentityHelper.FillDomainSid(subAuthorities, domainId);
      return new SecurityIdentifier(SidIdentityHelper.EncodeSid(9UL, subAuthorities), 0);
    }

    private static void FillDomainSid(uint[] subAuthorities, Guid domainId)
    {
      if (subAuthorities == null)
        throw new ArgumentNullException(nameof (subAuthorities), "subAuthorities array cannot be null");
      if (subAuthorities.Length < 5)
        throw new ArgumentException("subAuthorities array must be at least 5 entries in length", nameof (subAuthorities));
      subAuthorities[0] = 1551374245U;
      byte[] byteArray = domainId.ToByteArray();
      for (int index = 0; index < 4; ++index)
        subAuthorities[index + 1] = SidIdentityHelper.ArrayToInt(byteArray, index * 4);
    }

    public static uint[] DecodeSid(byte[] binarySid, out int revision, out ulong authority)
    {
      revision = (int) binarySid[0];
      int length = (int) binarySid[1];
      authority = (ulong) (((long) binarySid[2] << 40) + ((long) binarySid[3] << 32) + ((long) binarySid[4] << 24) + ((long) binarySid[5] << 16) + ((long) binarySid[6] << 8)) + (ulong) binarySid[7];
      uint[] numArray = new uint[length];
      for (byte index = 0; (int) index < length; ++index)
        numArray[(int) index] = (uint) ((int) binarySid[8 + 4 * (int) index] + ((int) binarySid[8 + 4 * (int) index + 1] << 8) + ((int) binarySid[8 + 4 * (int) index + 2] << 16) + ((int) binarySid[8 + 4 * (int) index + 3] << 24));
      return numArray;
    }

    public static byte[] EncodeSid(ulong identifierAuthority, uint[] subAuthorities)
    {
      byte[] numArray = subAuthorities != null ? new byte[8 + subAuthorities.Length * 4] : throw new ArgumentNullException(nameof (subAuthorities), "subAuthorities array cannot be null");
      numArray[0] = (byte) 1;
      numArray[1] = (byte) subAuthorities.Length;
      for (int index = 0; index < 6; ++index)
        numArray[2 + index] = (byte) (identifierAuthority >> (5 - index) * 8 & (ulong) byte.MaxValue);
      for (int index1 = 0; index1 < subAuthorities.Length; ++index1)
      {
        for (byte index2 = 0; index2 < (byte) 4; ++index2)
          numArray[8 + 4 * index1 + (int) index2] = (byte) (subAuthorities[index1] >> (int) index2 * 8 & (uint) byte.MaxValue);
      }
      return numArray;
    }

    public static uint GetSidRid(byte[] binarySid)
    {
      ulong authority;
      uint[] numArray = SidIdentityHelper.DecodeSid(binarySid, out int _, out authority);
      return authority != 5UL || numArray.Length != 5 ? 0U : numArray[4];
    }

    public static bool IsServiceAccount(byte[] sid)
    {
      ulong authority;
      uint[] numArray = SidIdentityHelper.DecodeSid(sid, out int _, out authority);
      return authority == 5UL && numArray.Length == 1 && (numArray[0] == 19U || numArray[0] == 20U);
    }

    public static bool IsNTAccount(byte[] sid)
    {
      ulong authority;
      uint[] numArray = SidIdentityHelper.DecodeSid(sid, out int _, out authority);
      return authority == 5UL && numArray.Length != 0 && numArray[0] == 21U;
    }

    public static bool IsBuiltInAccount(byte[] binarySid)
    {
      ulong authority;
      uint[] numArray = SidIdentityHelper.DecodeSid(binarySid, out int _, out authority);
      return authority == 5UL && numArray.Length != 0 && numArray[0] == 32U;
    }

    public static bool IsNTServiceAccount(byte[] binarySid)
    {
      ulong authority;
      uint[] numArray = SidIdentityHelper.DecodeSid(binarySid, out int _, out authority);
      return authority == 5UL && numArray.Length != 0 && numArray[0] == 80U;
    }

    public static bool IsAadGroupSid(byte[] sid)
    {
      uint[] numArray = SidIdentityHelper.DecodeSid(sid, out int _, out ulong _);
      return numArray.Length == 9 && numArray[0] == 1551374245U && numArray[1] == 1204400969U && numArray[2] == 2402986413U && numArray[3] == 2179408616U && numArray[4] == 3U;
    }

    public static bool IsWellKnownGroup(string sid, string targetGroupSid) => sid.StartsWith(SidIdentityHelper.TeamFoundationSidPrefix, StringComparison.OrdinalIgnoreCase) && sid.EndsWith(targetGroupSid.Substring(SidIdentityHelper.WellKnownDomainSid.Length), StringComparison.OrdinalIgnoreCase);

    public static bool IsWellKnownSid(string sid) => SidIdentityHelper.WellKnownGroupPrefixRegex.IsMatch(sid);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ResolveSid(
      SecurityIdentifierInfo securityIdInfo,
      out string domain,
      out string userName,
      out NativeMethods.AccountType type,
      out bool isDeleted)
    {
      bool migrated;
      SidIdentityHelper.ResolveSid(securityIdInfo, out domain, out userName, out type, out isDeleted, out migrated);
      isDeleted |= migrated;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ResolveSid(
      SecurityIdentifierInfo securityIdInfo,
      out string domain,
      out string userName,
      out NativeMethods.AccountType type,
      out bool isDeleted,
      out bool migrated)
    {
      string[] authentication = TraceKeywordSets.Authentication;
      migrated = false;
      try
      {
        userName = NativeMethods.SidToName(securityIdInfo.GetBinaryForm(), out domain, out type);
        SecurityIdentifier securityIdentifier = (SecurityIdentifier) new NTAccount(domain, userName).Translate(typeof (SecurityIdentifier));
        isDeleted = false;
        if (!string.Equals(securityIdInfo.SecurityId.Value, securityIdentifier.Value, StringComparison.OrdinalIgnoreCase))
        {
          domain = (string) null;
          userName = (string) null;
          migrated = true;
          if (!TeamFoundationTrace.IsTracing(authentication, TraceLevel.Info))
            return;
          TeamFoundationTrace.Info(authentication, "ResolveSid: Sid: {0}. {1}\\{2} migrated to {3}.", (object) securityIdInfo.SecurityId, (object) domain, (object) userName, (object) securityIdentifier);
        }
        else
        {
          try
          {
            if (!(securityIdInfo.SecurityId.AccountDomainSid != (SecurityIdentifier) null))
              return;
            string domainName;
            NativeMethods.SidToName(securityIdInfo.GetAccountDomainBinaryForm(), out domainName, out NativeMethods.AccountType _);
            if (string.Equals(domain, domainName, StringComparison.OrdinalIgnoreCase))
              return;
            domain = (string) null;
            userName = (string) null;
            migrated = true;
            if (!TeamFoundationTrace.IsTracing(authentication, TraceLevel.Warning))
              return;
            TeamFoundationTrace.Warning(authentication, "ResolveSid: Sid: {0}. Looks like identity in remote forest, {1}\\{2}, was migrated from {3} domain.", (object) securityIdInfo.SecurityId, (object) domain, (object) userName, (object) domainName);
          }
          catch (Exception ex)
          {
            if (TeamFoundationTrace.IsTracing(authentication, TraceLevel.Error))
              TeamFoundationTrace.Error(authentication, "ResolveSid: Sid: {0}. Exception resolving domain sid {1} to domain name. Exception: {2}", (object) securityIdInfo.SecurityId, (object) securityIdInfo.SecurityId.AccountDomainSid, (object) ex);
            throw;
          }
        }
      }
      catch (Win32Exception ex)
      {
        domain = (string) null;
        userName = (string) null;
        type = NativeMethods.AccountType.SidTypeUnknown;
        isDeleted = true;
        migrated = false;
        if (ex.NativeErrorCode != 1332)
          throw ex;
      }
      catch (IdentityNotMappedException ex)
      {
        domain = (string) null;
        userName = (string) null;
        type = NativeMethods.AccountType.SidTypeUnknown;
        isDeleted = true;
        migrated = false;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool IsTeamFoundationIdentifier(SecurityIdentifierInfo securityIdInfo)
    {
      ulong authority;
      uint[] numArray = SidIdentityHelper.DecodeSid(securityIdInfo.GetBinaryForm(), out int _, out authority);
      return authority == 9UL && numArray[0] == 1551374245U;
    }

    public static StringComparer WellKnownGroupSidComparer { get; } = (StringComparer) new SidIdentityHelper.WellKnownDomainSidComparer();

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string TeamFoundationSidPrefix => SidIdentityHelper.s_teamFoundationSidPrefix;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string WellKnownDomainSid => SidIdentityHelper.s_wellKnownDomainSid;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string WellKnownSidPrefix => SidIdentityHelper.s_wellKnownSidPrefix;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string PhantomSidPrefix => SidIdentityHelper.s_phantomSidPrefix;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string AadSidPrefix => SidIdentityHelper.s_aadSidPrefix;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string WellKnownSidType => "-0-";

    private static uint ArrayToInt(byte[] array, int index) => (uint) (0 | (int) array[index + 3] | (int) array[index + 2] << 8 | (int) array[index + 1] << 16 | (int) array[index] << 24);

    private static void IntToArray(uint subAuthority, byte[] array, int index)
    {
      array[index] = (byte) (subAuthority >> 24);
      array[index + 1] = (byte) (subAuthority >> 16);
      array[index + 2] = (byte) (subAuthority >> 8);
      array[index + 3] = (byte) subAuthority;
    }

    private sealed class WellKnownDomainSidComparer : StringComparer
    {
      private static string GetComparableSidParts(string descriptor)
      {
        if (string.IsNullOrEmpty(descriptor))
          return descriptor;
        int startIndex = descriptor.IndexOf("-0-0-0-", StringComparison.Ordinal);
        return startIndex >= 0 ? descriptor.Substring(0, SidIdentityHelper.TeamFoundationSidPrefix.Length) + descriptor.Substring(startIndex) : descriptor;
      }

      public override int Compare(string x, string y) => string.Compare(SidIdentityHelper.WellKnownDomainSidComparer.GetComparableSidParts(x), SidIdentityHelper.WellKnownDomainSidComparer.GetComparableSidParts(y), StringComparison.OrdinalIgnoreCase);

      public override bool Equals(string x, string y) => string.Equals(SidIdentityHelper.WellKnownDomainSidComparer.GetComparableSidParts(x), SidIdentityHelper.WellKnownDomainSidComparer.GetComparableSidParts(y), StringComparison.OrdinalIgnoreCase);

      public override int GetHashCode(string obj) => StringComparer.OrdinalIgnoreCase.GetHashCode(SidIdentityHelper.WellKnownDomainSidComparer.GetComparableSidParts(obj) ?? string.Empty);
    }
  }
}

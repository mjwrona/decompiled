// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.VectorSessionToken
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.Azure.Documents
{
  internal sealed class VectorSessionToken : ISessionToken, IEquatable<ISessionToken>
  {
    private static readonly IReadOnlyDictionary<uint, long> DefaultLocalLsnByRegion = (IReadOnlyDictionary<uint, long>) new Dictionary<uint, long>(0);
    private const char SegmentSeparator = '#';
    private const string SegmentSeparatorString = "#";
    private const char RegionProgressSeparator = '=';
    private readonly string sessionToken;
    private readonly long version;
    private readonly long globalLsn;
    private readonly IReadOnlyDictionary<uint, long> localLsnByRegion;

    private VectorSessionToken(
      long version,
      long globalLsn,
      IReadOnlyDictionary<uint, long> localLsnByRegion,
      string sessionToken = null)
    {
      this.version = version;
      this.globalLsn = globalLsn;
      this.localLsnByRegion = localLsnByRegion;
      this.sessionToken = sessionToken;
      if (this.sessionToken != null)
        return;
      string str = (string) null;
      if (localLsnByRegion.Any<KeyValuePair<uint, long>>())
        str = string.Join("#", localLsnByRegion.Select<KeyValuePair<uint, long>, string>((Func<KeyValuePair<uint, long>, string>) (kvp => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}{2}", (object) kvp.Key, (object) '=', (object) kvp.Value))));
      if (string.IsNullOrEmpty(str))
        this.sessionToken = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}{2}", (object) this.version, (object) "#", (object) this.globalLsn);
      else
        this.sessionToken = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}{2}{3}{4}", (object) this.version, (object) "#", (object) this.globalLsn, (object) "#", (object) str);
    }

    public VectorSessionToken(VectorSessionToken other, long globalLSN)
      : this(other.version, globalLSN, other.localLsnByRegion)
    {
    }

    public static bool TryCreate(string sessionToken, out ISessionToken parsedSessionToken)
    {
      parsedSessionToken = (ISessionToken) null;
      long version;
      long globalLsn;
      IReadOnlyDictionary<uint, long> localLsnByRegion;
      if (!VectorSessionToken.TryParseSessionToken(sessionToken, out version, out globalLsn, out localLsnByRegion))
        return false;
      parsedSessionToken = (ISessionToken) new VectorSessionToken(version, globalLsn, localLsnByRegion, sessionToken);
      return true;
    }

    public long LSN => this.globalLsn;

    public bool Equals(ISessionToken obj) => obj is VectorSessionToken vectorSessionToken && this.version == vectorSessionToken.version && this.globalLsn == vectorSessionToken.globalLsn && this.AreRegionProgressEqual(vectorSessionToken.localLsnByRegion);

    public bool IsValid(ISessionToken otherSessionToken)
    {
      if (!(otherSessionToken is VectorSessionToken vectorSessionToken))
        throw new ArgumentNullException(nameof (otherSessionToken));
      if (vectorSessionToken.version < this.version || vectorSessionToken.globalLsn < this.globalLsn)
        return false;
      if (vectorSessionToken.version == this.version && vectorSessionToken.localLsnByRegion.Count != this.localLsnByRegion.Count)
        throw new InternalServerErrorException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, RMResources.InvalidRegionsInSessionToken, (object) this.sessionToken, (object) vectorSessionToken.sessionToken));
      foreach (KeyValuePair<uint, long> keyValuePair in (IEnumerable<KeyValuePair<uint, long>>) vectorSessionToken.localLsnByRegion)
      {
        uint key = keyValuePair.Key;
        long num1 = keyValuePair.Value;
        long num2 = -1;
        if (!this.localLsnByRegion.TryGetValue(key, out num2))
        {
          if (this.version == vectorSessionToken.version)
            throw new InternalServerErrorException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, RMResources.InvalidRegionsInSessionToken, (object) this.sessionToken, (object) vectorSessionToken.sessionToken));
        }
        else if (num1 < num2)
          return false;
      }
      return true;
    }

    public ISessionToken Merge(ISessionToken obj)
    {
      if (!(obj is VectorSessionToken vectorSessionToken1))
        throw new ArgumentNullException(nameof (obj));
      if (this.version == vectorSessionToken1.version && this.localLsnByRegion.Count != vectorSessionToken1.localLsnByRegion.Count)
        throw new InternalServerErrorException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, RMResources.InvalidRegionsInSessionToken, (object) this.sessionToken, (object) vectorSessionToken1.sessionToken));
      if (this.version >= vectorSessionToken1.version && this.globalLsn > vectorSessionToken1.globalLsn)
      {
        if (VectorSessionToken.AreAllLocalLsnByRegionsGreaterThanOrEqual(this, vectorSessionToken1))
          return (ISessionToken) this;
      }
      else if (vectorSessionToken1.version >= this.version && vectorSessionToken1.globalLsn >= this.globalLsn && VectorSessionToken.AreAllLocalLsnByRegionsGreaterThanOrEqual(vectorSessionToken1, this))
        return (ISessionToken) vectorSessionToken1;
      VectorSessionToken vectorSessionToken2;
      VectorSessionToken vectorSessionToken3;
      if (this.version < vectorSessionToken1.version)
      {
        vectorSessionToken2 = this;
        vectorSessionToken3 = vectorSessionToken1;
      }
      else
      {
        vectorSessionToken2 = vectorSessionToken1;
        vectorSessionToken3 = this;
      }
      Dictionary<uint, long> localLsnByRegion = new Dictionary<uint, long>(vectorSessionToken3.localLsnByRegion.Count);
      foreach (KeyValuePair<uint, long> keyValuePair in (IEnumerable<KeyValuePair<uint, long>>) vectorSessionToken3.localLsnByRegion)
      {
        uint key = keyValuePair.Key;
        long val1 = keyValuePair.Value;
        long val2 = -1;
        if (vectorSessionToken2.localLsnByRegion.TryGetValue(key, out val2))
        {
          localLsnByRegion[key] = Math.Max(val1, val2);
        }
        else
        {
          if (this.version == vectorSessionToken1.version)
            throw new InternalServerErrorException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, RMResources.InvalidRegionsInSessionToken, (object) this.sessionToken, (object) vectorSessionToken1.sessionToken));
          localLsnByRegion[key] = val1;
        }
      }
      return (ISessionToken) new VectorSessionToken(Math.Max(this.version, vectorSessionToken1.version), Math.Max(this.globalLsn, vectorSessionToken1.globalLsn), (IReadOnlyDictionary<uint, long>) localLsnByRegion);
    }

    string ISessionToken.ConvertToString() => this.sessionToken;

    private bool AreRegionProgressEqual(IReadOnlyDictionary<uint, long> other)
    {
      if (this.localLsnByRegion.Count != other.Count)
        return false;
      foreach (KeyValuePair<uint, long> keyValuePair in (IEnumerable<KeyValuePair<uint, long>>) this.localLsnByRegion)
      {
        uint key = keyValuePair.Key;
        long num1 = keyValuePair.Value;
        long num2;
        if (other.TryGetValue(key, out num2) && num1 != num2)
          return false;
      }
      return true;
    }

    private static bool AreAllLocalLsnByRegionsGreaterThanOrEqual(
      VectorSessionToken higherToken,
      VectorSessionToken lowerToken)
    {
      if (higherToken.localLsnByRegion.Count != lowerToken.localLsnByRegion.Count)
        return false;
      if (!higherToken.localLsnByRegion.Any<KeyValuePair<uint, long>>())
        return true;
      foreach (KeyValuePair<uint, long> keyValuePair in (IEnumerable<KeyValuePair<uint, long>>) higherToken.localLsnByRegion)
      {
        uint key = keyValuePair.Key;
        long num1 = keyValuePair.Value;
        long num2;
        if (!lowerToken.localLsnByRegion.TryGetValue(key, out num2) || num2 > num1)
          return false;
      }
      return true;
    }

    private static bool TryParseSessionToken(
      string sessionToken,
      out long version,
      out long globalLsn,
      out IReadOnlyDictionary<uint, long> localLsnByRegion)
    {
      version = 0L;
      localLsnByRegion = (IReadOnlyDictionary<uint, long>) null;
      globalLsn = -1L;
      if (string.IsNullOrEmpty(sessionToken))
      {
        DefaultTrace.TraceWarning("Session token is empty");
        return false;
      }
      int index = 0;
      if (!VectorSessionToken.TryParseLongSegment(sessionToken, ref index, out version))
      {
        DefaultTrace.TraceWarning("Unexpected session token version number from token: " + sessionToken + " .");
        return false;
      }
      if (index >= sessionToken.Length)
        return false;
      if (!VectorSessionToken.TryParseLongSegment(sessionToken, ref index, out globalLsn))
      {
        DefaultTrace.TraceWarning("Unexpected session token global lsn from token: " + sessionToken + " .");
        return false;
      }
      if (index >= sessionToken.Length)
      {
        localLsnByRegion = VectorSessionToken.DefaultLocalLsnByRegion;
        return true;
      }
      Dictionary<uint, long> dictionary = new Dictionary<uint, long>();
      while (index < sessionToken.Length)
      {
        uint key;
        if (!VectorSessionToken.TryParseUintTillRegionProgressSeparator(sessionToken, ref index, out key))
        {
          DefaultTrace.TraceWarning("Unexpected region progress segment in session token: " + sessionToken + ".");
          return false;
        }
        long num;
        if (!VectorSessionToken.TryParseLongSegment(sessionToken, ref index, out num))
        {
          DefaultTrace.TraceWarning("Unexpected local lsn for region id " + key.ToString((IFormatProvider) CultureInfo.InvariantCulture) + " for segment in session token: " + sessionToken + ".");
          return false;
        }
        dictionary[key] = num;
      }
      localLsnByRegion = (IReadOnlyDictionary<uint, long>) dictionary;
      return true;
    }

    private static bool TryParseUintTillRegionProgressSeparator(
      string input,
      ref int index,
      out uint value)
    {
      value = 0U;
      if (index >= input.Length)
        return false;
      long num = 0;
      while (index < input.Length)
      {
        char ch = input[index];
        if (ch >= '0' && ch <= '9')
        {
          num = num * 10L + (long) ((int) ch - 48);
          ++index;
        }
        else
        {
          if (ch != '=')
            return false;
          ++index;
          break;
        }
      }
      if (num > (long) uint.MaxValue || num < 0L)
        return false;
      value = (uint) num;
      return true;
    }

    private static bool TryParseLongSegment(string input, ref int index, out long value)
    {
      value = 0L;
      if (index >= input.Length)
        return false;
      bool flag = false;
      if (input[index] == '-')
      {
        ++index;
        flag = true;
      }
      while (index < input.Length)
      {
        char ch = input[index];
        if (ch >= '0' && ch <= '9')
        {
          value = value * 10L + (long) ((int) ch - 48);
          ++index;
        }
        else
        {
          if (ch != '#')
            return false;
          ++index;
          break;
        }
      }
      if (flag)
        value *= -1L;
      return true;
    }
  }
}

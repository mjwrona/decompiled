// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.VectorSessionToken
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.Azure.Documents
{
  internal sealed class VectorSessionToken : ISessionToken, IEquatable<ISessionToken>
  {
    private const char SegmentSeparator = '#';
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
      string str = string.Join('#'.ToString(), localLsnByRegion.Select<KeyValuePair<uint, long>, string>((Func<KeyValuePair<uint, long>, string>) (kvp => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}{2}", (object) kvp.Key, (object) '=', (object) kvp.Value))));
      if (string.IsNullOrEmpty(str))
        this.sessionToken = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}{2}", (object) this.version, (object) '#', (object) this.globalLsn);
      else
        this.sessionToken = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}{2}{3}{4}", (object) this.version, (object) '#', (object) this.globalLsn, (object) '#', (object) str);
    }

    public VectorSessionToken(VectorSessionToken other, long globalLSN)
      : this(other.version, globalLSN, (IReadOnlyDictionary<uint, long>) other.localLsnByRegion.ToDictionary<KeyValuePair<uint, long>, uint, long>((Func<KeyValuePair<uint, long>, uint>) (kvp => kvp.Key), (Func<KeyValuePair<uint, long>, long>) (kvp => kvp.Value)))
    {
    }

    public static bool TryCreate(string sessionToken, out ISessionToken parsedSessionToken)
    {
      parsedSessionToken = (ISessionToken) null;
      long version = -1;
      long globalLsn = -1;
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
      Dictionary<uint, long> localLsnByRegion = new Dictionary<uint, long>();
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
        long num2 = -1;
        if (other.TryGetValue(key, out num2) && num1 != num2)
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
        DefaultTrace.TraceCritical("Session token is empty");
        return false;
      }
      string[] source = sessionToken.Split('#');
      if (source.Length < 2)
        return false;
      if (!long.TryParse(source[0], NumberStyles.Number, (IFormatProvider) CultureInfo.InvariantCulture, out version) || !long.TryParse(source[1], NumberStyles.Number, (IFormatProvider) CultureInfo.InvariantCulture, out globalLsn))
      {
        DefaultTrace.TraceCritical("Unexpected session token version number '{0}' OR global lsn '{1}'.", (object) source[0], (object) source[1]);
        return false;
      }
      Dictionary<uint, long> dictionary = new Dictionary<uint, long>();
      foreach (string str in ((IEnumerable<string>) source).Skip<string>(2))
      {
        string[] strArray = str.Split('=');
        if (strArray.Length != 2)
        {
          DefaultTrace.TraceCritical("Unexpected region progress segment length '{0}' in session token.", (object) strArray.Length);
          return false;
        }
        uint result1 = 0;
        long result2 = -1;
        if (!uint.TryParse(strArray[0], NumberStyles.Number, (IFormatProvider) CultureInfo.InvariantCulture, out result1) || !long.TryParse(strArray[1], NumberStyles.Number, (IFormatProvider) CultureInfo.InvariantCulture, out result2))
        {
          DefaultTrace.TraceCritical("Unexpected region progress '{0}' for region '{1}' in session token.", (object) strArray[0], (object) strArray[1]);
          return false;
        }
        dictionary[result1] = result2;
      }
      localLsnByRegion = (IReadOnlyDictionary<uint, long>) dictionary;
      return true;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.SimpleSessionToken
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Globalization;

namespace Microsoft.Azure.Documents
{
  internal sealed class SimpleSessionToken : ISessionToken, IEquatable<ISessionToken>
  {
    private readonly long globalLsn;

    public SimpleSessionToken(long globalLsn) => this.globalLsn = globalLsn;

    public static bool TryCreate(string globalLsn, out ISessionToken parsedSessionToken)
    {
      parsedSessionToken = (ISessionToken) null;
      long result = -1;
      if (!long.TryParse(globalLsn, out result))
        return false;
      parsedSessionToken = (ISessionToken) new SimpleSessionToken(result);
      return true;
    }

    public long LSN => this.globalLsn;

    public bool Equals(ISessionToken obj) => obj is SimpleSessionToken simpleSessionToken && this.globalLsn.Equals(simpleSessionToken.globalLsn);

    public ISessionToken Merge(ISessionToken obj) => obj is SimpleSessionToken simpleSessionToken ? (ISessionToken) new SimpleSessionToken(Math.Max(this.globalLsn, simpleSessionToken.globalLsn)) : throw new ArgumentNullException(nameof (obj));

    public bool IsValid(ISessionToken otherSessionToken)
    {
      if (!(otherSessionToken is SimpleSessionToken simpleSessionToken))
        throw new ArgumentNullException(nameof (otherSessionToken));
      return simpleSessionToken.globalLsn >= this.globalLsn;
    }

    string ISessionToken.ConvertToString() => this.globalLsn.ToString((IFormatProvider) CultureInfo.InvariantCulture);
  }
}

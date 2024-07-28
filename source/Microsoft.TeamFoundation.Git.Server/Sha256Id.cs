// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Sha256Id
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  [JsonConverter(typeof (Sha256IdJsonSerializer))]
  [DebuggerDisplay("{ToString()}")]
  public class Sha256Id : IEquatable<Sha256Id>
  {
    private string m_id;
    private byte[] m_bytes;
    public const int Length = 32;
    public const string EmptyString = "0000000000000000000000000000000000000000000000000000000000000000";
    public static readonly Sha256Id Empty = new Sha256Id("0000000000000000000000000000000000000000000000000000000000000000");

    public Sha256Id(string sha256IdString)
    {
      if (sha256IdString == null)
        throw new ArgumentNullException(nameof (sha256IdString));
      byte[] lfsObjectIdBytes;
      this.m_id = Sha256Id.IsValidSha256String(sha256IdString, out lfsObjectIdBytes) ? sha256IdString.ToLower() : throw new ArgumentException(Resources.Format("InvalidSha256Id", (object) sha256IdString));
      this.m_bytes = lfsObjectIdBytes;
    }

    public Sha256Id(byte[] data)
    {
      if (data == null)
        throw new ArgumentNullException(nameof (data));
      this.m_id = data.Length == 32 ? GitUtils.StringFromByteArray(data) : throw new ArgumentOutOfRangeException(nameof (data));
      this.m_bytes = ((IEnumerable<byte>) data).ToArray<byte>();
    }

    public byte[] ToByteArray() => ((IEnumerable<byte>) this.m_bytes).ToArray<byte>();

    public bool Equals(Sha256Id other) => !(other == (Sha256Id) null) && ((IEnumerable<byte>) this.m_bytes).SequenceEqual<byte>((IEnumerable<byte>) other.m_bytes);

    public static bool operator ==(Sha256Id sha256Id1, Sha256Id sha256Id2)
    {
      if ((object) sha256Id1 == null && (object) sha256Id2 == null)
        return true;
      return (object) sha256Id1 != null && (object) sha256Id2 != null && sha256Id1.Equals(sha256Id2);
    }

    public static bool operator !=(Sha256Id sha256Id1, Sha256Id sha256Id2) => !(sha256Id1 == sha256Id2);

    public override bool Equals(object value) => this.Equals(value as Sha256Id);

    public override int GetHashCode() => BitConverter.ToInt32(this.m_bytes, 0);

    public override string ToString() => this.m_id;

    public static bool IsValidSha256String(string sha256Id) => Sha256Id.IsValidSha256String(sha256Id, out byte[] _);

    public static bool TryParse(string sha256IdString, out Sha256Id result)
    {
      if (Sha256Id.IsValidSha256String(sha256IdString))
      {
        result = new Sha256Id(sha256IdString);
        return true;
      }
      result = (Sha256Id) null;
      return false;
    }

    private static bool IsValidSha256String(string sha256Id, out byte[] lfsObjectIdBytes)
    {
      lfsObjectIdBytes = (byte[]) null;
      return !string.IsNullOrWhiteSpace(sha256Id) && GitUtils.TryGetByteArrayFromString(sha256Id, 64, out lfsObjectIdBytes);
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Security.Server.TokenStoreSequenceId
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Text;

namespace Microsoft.VisualStudio.Services.Security.Server
{
  public struct TokenStoreSequenceId : IEquatable<TokenStoreSequenceId>
  {
    private readonly long[] Values;
    public static readonly TokenStoreSequenceId NoWorkPerformed = new TokenStoreSequenceId(0L);
    public static readonly TokenStoreSequenceId DropCache = new TokenStoreSequenceId(-2L);
    public static readonly TokenStoreSequenceId UnconditionalRefresh = new TokenStoreSequenceId(-3L);

    public TokenStoreSequenceId(long value) => this.Values = -1L != value ? new long[1]
    {
      value
    } : throw new InvalidOperationException();

    public TokenStoreSequenceId(params long[] values) => this.Values = values;

    public static implicit operator TokenStoreSequenceId(long sequenceId) => new TokenStoreSequenceId(sequenceId);

    public override bool Equals(object obj) => obj is TokenStoreSequenceId other && this.Equals(other);

    public override int GetHashCode()
    {
      int hashCode = 0;
      if (this.Values != null)
      {
        for (int index = 0; index < this.Values.Length; ++index)
          hashCode ^= (int) this.Values[index];
      }
      return hashCode;
    }

    public bool Equals(TokenStoreSequenceId other)
    {
      if (this.Values == other.Values)
        return true;
      if (this.Values == null || other.Values == null || this.Values.Length != other.Values.Length)
        return false;
      for (int index = 0; index < this.Values.Length; ++index)
      {
        if (this.Values[index] != other.Values[index])
          return false;
      }
      return true;
    }

    public static bool operator ==(TokenStoreSequenceId lhs, TokenStoreSequenceId rhs) => lhs.Equals(rhs);

    public static bool operator !=(TokenStoreSequenceId lhs, TokenStoreSequenceId rhs) => !lhs.Equals(rhs);

    public override string ToString()
    {
      if (this.IsEmpty)
        return "{ }";
      if (1 == this.Values.Length)
        return string.Format("{{ {0} }}", (object) this.Values[0]);
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append('{');
      foreach (long num in this.Values)
      {
        stringBuilder.Append(' ');
        stringBuilder.Append(num);
      }
      stringBuilder.Append(" }");
      return stringBuilder.ToString();
    }

    public bool IsEmpty => this.Values == null || this.Values.Length == 0;

    public long ToScalarForDelta() => this.Values != null && 1 == this.Values.Length && this.Values[0] >= 0L ? this.Values[0] : -1L;

    public long ToScalarForInvalidation() => this.Values != null && 1 == this.Values.Length && this.Values[0] >= 0L ? this.Values[0] : -3L;

    public long ToScalarForRestReply() => this.Values != null && 1 == this.Values.Length ? this.Values[this.Values.Length - 1] : throw new InvalidOperationException();

    public long[] ToArrayForRestReply() => this.Values == null ? Array.Empty<long>() : this.Values;

    public bool IsSupersededBy(
      TokenStoreSequenceId newer,
      bool allowInvalidationValues = false,
      bool returnValueOnLengthMismatch = true)
    {
      if (newer.IsEmpty)
        throw new InvalidOperationException();
      int num = this.Values != null ? this.Values.Length : 0;
      for (int index = 0; index < newer.Values.Length; ++index)
      {
        if (newer.Values[index] < 0L)
        {
          if (!allowInvalidationValues || 1 != newer.Values.Length)
            throw new InvalidOperationException();
          if (num == 0)
            return true;
          if (num > 1)
            num = 1;
        }
      }
      if (newer.Values.Length == num)
      {
        for (int index = 0; index < num; ++index)
        {
          if (this.Values[index] < 0L && (!allowInvalidationValues || 1 != this.Values.Length))
            throw new InvalidOperationException();
          if (!TokenStoreSequenceId.IsSequenceIdGreaterThanOrEqual(this.Values[index], newer.Values[index]))
            return true;
        }
        return false;
      }
      return num == 0 || returnValueOnLengthMismatch;
    }

    private static bool IsSequenceIdGreaterThanOrEqual(long a, long b)
    {
      if (a == -2L)
        return true;
      if (b == -2L)
        return false;
      if (a == -3L)
        return true;
      return b != -3L && a >= b;
    }

    public bool ImmediatelyPrecedes(TokenStoreSequenceId newer)
    {
      if (this.Values == null)
        return false;
      for (int index = 0; index < this.Values.Length; ++index)
      {
        if (this.Values[index] < 0L)
          throw new InvalidOperationException();
      }
      if (newer.Values == null || this.Values.Length != newer.Values.Length)
        return false;
      bool flag = false;
      for (int index = 0; index < newer.Values.Length; ++index)
      {
        if (newer.Values[index] < 0L)
          return false;
        if (newer.Values[index] - 1L == this.Values[index])
        {
          if (flag)
            return false;
          flag = true;
        }
      }
      return flag;
    }
  }
}

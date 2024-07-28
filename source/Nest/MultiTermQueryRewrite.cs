// Decompiled with JetBrains decompiler
// Type: Nest.MultiTermQueryRewrite
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  [JsonFormatter(typeof (MultiTermQueryRewriteFormatter))]
  public class MultiTermQueryRewrite : IEquatable<MultiTermQueryRewrite>
  {
    private static readonly char[] DigitCharacters = new char[10]
    {
      '0',
      '1',
      '2',
      '3',
      '4',
      '5',
      '6',
      '7',
      '8',
      '9'
    };
    private readonly string _value;

    internal MultiTermQueryRewrite(RewriteMultiTerm rewrite, int? size = null)
    {
      switch (rewrite)
      {
        case RewriteMultiTerm.ConstantScore:
        case RewriteMultiTerm.ScoringBoolean:
        case RewriteMultiTerm.ConstantScoreBoolean:
          this._value = rewrite.ToEnumValue<RewriteMultiTerm>();
          break;
        case RewriteMultiTerm.TopTermsN:
        case RewriteMultiTerm.TopTermsBoostN:
        case RewriteMultiTerm.TopTermsBlendedFreqsN:
          if (!size.HasValue)
            throw new ArgumentException(string.Format("{0} must be specified with {1}.{2}", (object) nameof (size), (object) "RewriteMultiTerm", (object) rewrite));
          string enumValue = rewrite.ToEnumValue<RewriteMultiTerm>();
          this._value = enumValue.Substring(0, enumValue.Length - 1) + size.ToString();
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof (rewrite));
      }
      this.Rewrite = rewrite;
      this.Size = size;
    }

    public static MultiTermQueryRewrite ConstantScore { get; } = new MultiTermQueryRewrite(RewriteMultiTerm.ConstantScore);

    public static MultiTermQueryRewrite ConstantScoreBoolean { get; } = new MultiTermQueryRewrite(RewriteMultiTerm.ConstantScoreBoolean);

    public RewriteMultiTerm Rewrite { get; }

    public static MultiTermQueryRewrite ScoringBoolean { get; } = new MultiTermQueryRewrite(RewriteMultiTerm.ScoringBoolean);

    public int? Size { get; }

    public bool Equals(MultiTermQueryRewrite other)
    {
      if ((object) other == null)
        return false;
      if ((object) this == (object) other)
        return true;
      if (this.Rewrite != other.Rewrite)
        return false;
      int? size1 = this.Size;
      int? size2 = other.Size;
      return size1.GetValueOrDefault() == size2.GetValueOrDefault() & size1.HasValue == size2.HasValue;
    }

    public static MultiTermQueryRewrite TopTerms(int size) => new MultiTermQueryRewrite(RewriteMultiTerm.TopTermsN, new int?(size));

    public static MultiTermQueryRewrite TopTermsBoost(int size) => new MultiTermQueryRewrite(RewriteMultiTerm.TopTermsBoostN, new int?(size));

    public static MultiTermQueryRewrite TopTermsBlendedFreqs(int size) => new MultiTermQueryRewrite(RewriteMultiTerm.TopTermsBlendedFreqsN, new int?(size));

    internal static MultiTermQueryRewrite Create(string value)
    {
      string str = value != null ? value : throw new ArgumentNullException(nameof (value));
      int size = 0;
      int num = value.IndexOfAny(MultiTermQueryRewrite.DigitCharacters);
      if (num > -1)
      {
        str = value.Substring(0, num) + "N";
        size = int.Parse(value.Substring(num));
      }
      RewriteMultiTerm? nullable = str.ToEnum<RewriteMultiTerm>();
      if (!nullable.HasValue)
        throw new InvalidOperationException("Unsupported RewriteMultiTerm value: '" + str + "'");
      if (nullable.HasValue)
      {
        switch (nullable.GetValueOrDefault())
        {
          case RewriteMultiTerm.ConstantScore:
            return MultiTermQueryRewrite.ConstantScore;
          case RewriteMultiTerm.ScoringBoolean:
            return MultiTermQueryRewrite.ScoringBoolean;
          case RewriteMultiTerm.ConstantScoreBoolean:
            return MultiTermQueryRewrite.ConstantScoreBoolean;
          case RewriteMultiTerm.TopTermsN:
            return MultiTermQueryRewrite.TopTerms(size);
          case RewriteMultiTerm.TopTermsBoostN:
            return MultiTermQueryRewrite.TopTermsBoost(size);
          case RewriteMultiTerm.TopTermsBlendedFreqsN:
            return MultiTermQueryRewrite.TopTermsBlendedFreqs(size);
        }
      }
      throw new InvalidOperationException(string.Format("Unsupported {0} value: '{1}'", (object) "RewriteMultiTerm", (object) nullable));
    }

    public override string ToString() => this._value;

    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;
      if ((object) this == obj)
        return true;
      if (obj is string a)
        return string.Equals(a, this._value);
      return obj.GetType() == this.GetType() && this.Equals((MultiTermQueryRewrite) obj);
    }

    public override int GetHashCode() => (int) this.Rewrite * 397 ^ this.Size.GetHashCode();

    public static bool operator ==(MultiTermQueryRewrite left, MultiTermQueryRewrite right) => object.Equals((object) left, (object) right);

    public static bool operator !=(MultiTermQueryRewrite left, MultiTermQueryRewrite right) => !object.Equals((object) left, (object) right);
  }
}

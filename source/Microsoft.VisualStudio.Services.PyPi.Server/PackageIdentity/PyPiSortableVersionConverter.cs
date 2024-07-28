// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity.PyPiSortableVersionConverter
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity.VersionDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity
{
  public class PyPiSortableVersionConverter : 
    IConverter<PyPiPackageVersion, string>,
    IHaveInputType<PyPiPackageVersion>,
    IHaveOutputType<string>
  {
    public const char EpochSuffix = '!';
    public const char DevPrefix = '#';
    public const char AlphaPrefix = '$';
    public const char BetaPrefix = '%';
    public const char ReleaseCandidatePrefix = '&';
    public const char EmptyIndicator = '(';
    public const char PostPrefix = ')';
    public const char ReleasePartSeparator = '.';
    public const char LocalPrefix = '*';
    public const string StringSegmentPrefix = "+";
    public const string NumericSegmentPrefix = "-";
    private readonly IConverter<ulong, string> numberConverter = (IConverter<ulong, string>) new VersionUInt64ToPaddedStringConverter((IConverter<ArraySegment<byte>, string>) new LexicalBase64Converter(), 6);
    private readonly IConverter<BigInteger, string> base64Converter;

    public PyPiSortableVersionConverter(IConverter<BigInteger, string> base64Converter) => this.base64Converter = base64Converter;

    public PyPiSortableVersionConverter() => this.base64Converter = (IConverter<BigInteger, string>) new VersionBigIntegerToPaddedStringConverter((IConverter<ArraySegment<byte>, string>) new LexicalBase64Converter());

    public string Convert(PyPiPackageVersion version)
    {
      PyPiSortableVersionConverter.ValidateNormalizedVersion(version);
      StringBuilder stringBuilder1 = new StringBuilder();
      stringBuilder1.Append(this.base64Converter.Convert(version.Epoch)).Append('!');
      IEnumerable<BigInteger> source = PyPiPackageVersion.RemoveTrailingZeroes((IEnumerable<BigInteger>) version.Release);
      string str1 = string.Join('.'.ToString(), source.Select<BigInteger, string>((Func<BigInteger, string>) (x => this.base64Converter.Convert(x))));
      stringBuilder1.Append(str1);
      BigInteger? nullable;
      if (version.PreLabel != null)
      {
        nullable = version.PreNumber;
        if (nullable.HasValue)
        {
          if (version.PreLabel.Equals("a"))
            stringBuilder1.Append('$');
          else if (version.PreLabel.Equals("b"))
            stringBuilder1.Append('%');
          else if (version.PreLabel.Equals("rc"))
            stringBuilder1.Append('&');
          StringBuilder stringBuilder2 = stringBuilder1;
          IConverter<BigInteger, string> base64Converter = this.base64Converter;
          nullable = version.PreNumber;
          BigInteger input = nullable.Value;
          string str2 = base64Converter.Convert(input);
          stringBuilder2.Append(str2);
        }
      }
      if (version.PostLabel != null)
      {
        nullable = version.PostNumber;
        if (nullable.HasValue)
        {
          StringBuilder stringBuilder3 = stringBuilder1.Append(')');
          IConverter<BigInteger, string> base64Converter = this.base64Converter;
          nullable = version.PostNumber;
          BigInteger input = nullable.Value;
          string str3 = base64Converter.Convert(input);
          stringBuilder3.Append(str3);
        }
      }
      if (version.DevLabel != null)
      {
        nullable = version.DevNumber;
        if (nullable.HasValue)
        {
          StringBuilder stringBuilder4 = stringBuilder1.Append('#');
          IConverter<BigInteger, string> base64Converter = this.base64Converter;
          nullable = version.DevNumber;
          BigInteger input = nullable.Value;
          string str4 = base64Converter.Convert(input);
          stringBuilder4.Append(str4);
        }
      }
      stringBuilder1.Append('(');
      if ((object) version.Local != null)
        stringBuilder1.Append('*').Append(this.GenerateLocalSortableVersion(version.Local));
      string str5 = stringBuilder1.ToString();
      return str5.Length <= (int) sbyte.MaxValue ? str5 : throw new InvalidVersionException(Resources.Error_SortableVersionExceedsMaximumLength((object) version.NormalizedVersion));
    }

    private string GenerateLocalSortableVersion(PyPiLocalVersionLabel label) => label == null ? "" : this.FormatLabel(label);

    private string FormatLabel(PyPiLocalVersionLabel label) => string.Concat(label.Segments.Select<IVersionLabelSegment, string>((Func<IVersionLabelSegment, string>) (x => this.FormatSegment(x))));

    private string FormatSegment(IVersionLabelSegment segment)
    {
      switch (segment)
      {
        case NumericVersionLabelSegment versionLabelSegment1:
          return "-" + this.numberConverter.Convert(versionLabelSegment1.Value);
        case StringVersionLabelSegment versionLabelSegment2:
          return "+" + versionLabelSegment2.StringValue.ToLowerInvariant();
        default:
          throw new ArgumentOutOfRangeException(nameof (segment), (object) segment, (string) null);
      }
    }

    private static void ValidateNormalizedVersion(PyPiPackageVersion version)
    {
      if (((IEnumerable<BigInteger>) version.Release).IsNullOrEmpty<BigInteger>())
        throw new InvalidOperationException("Encountered version without release parts: " + version.DisplayVersion);
      BigInteger? nullable;
      if (version.PreLabel != null)
      {
        nullable = version.PreNumber;
        if (!nullable.HasValue)
          throw new InvalidOperationException("Encountered pre-release label " + version.PreLabel + " without pre-release number");
        if (!version.PreLabel.Equals("a") && !version.PreLabel.Equals("b") && !version.PreLabel.Equals("rc"))
          throw new InvalidOperationException("Encountered non-normalized pre-release label: " + version.PreLabel);
      }
      if (version.PostLabel != null)
      {
        nullable = version.PostNumber;
        if (!nullable.HasValue)
          throw new InvalidOperationException("Encountered post-release label " + version.PostLabel + " without post-release number");
        if (!version.PostLabel.Equals("post"))
          throw new InvalidOperationException("Encountered non-normalized post-release label " + version.PostLabel);
      }
      if (version.DevLabel == null)
        return;
      nullable = version.DevNumber;
      if (!nullable.HasValue)
        throw new InvalidOperationException("Encountered dev-release label " + version.DevLabel + " without dev-release number");
      if (!version.DevLabel.Equals("dev"))
        throw new InvalidOperationException("Encountered non-normalized dev-release label " + version.DevLabel);
    }
  }
}

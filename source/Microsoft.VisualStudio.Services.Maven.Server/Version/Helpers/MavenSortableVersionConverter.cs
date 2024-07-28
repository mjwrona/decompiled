// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Version.Helpers.MavenSortableVersionConverter
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Maven.Server.Version.Items;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Microsoft.VisualStudio.Services.Maven.Server.Version.Helpers
{
  public class MavenSortableVersionConverter : 
    IConverter<MavenVersionParser, string>,
    IHaveInputType<MavenVersionParser>,
    IHaveOutputType<string>
  {
    private const char ENC_DOT_INT = '}';
    private const char ENC_DASH_INT = '|';
    private const char ENC_DASH_STRING = '$';
    private const char ENC_DOT_STRING = '!';
    private readonly IConverter<BigInteger, string> bigIntegerToBase64Converter;
    private readonly IConverter<ArraySegment<byte>, string> base64Converter;

    public MavenSortableVersionConverter(
      IConverter<BigInteger, string> bigIntegerToBase64Converter,
      IConverter<ArraySegment<byte>, string> base64Converter)
    {
      this.bigIntegerToBase64Converter = bigIntegerToBase64Converter;
      this.base64Converter = base64Converter;
    }

    public string Convert(MavenVersionParser versionParser)
    {
      string lowerInvariant = versionParser.RawVersion.ToLowerInvariant();
      if (!versionParser.IsGuid && !versionParser.IsHash)
        return this.Convert(versionParser.VersionList, lowerInvariant, false);
      MavenVersionList versionList = new MavenVersionList();
      versionList.Add((IMavenVersionItem) new MavenVersionString(lowerInvariant, false));
      return this.Convert(versionList, lowerInvariant, false);
    }

    public string Convert(MavenVersionList versionList, string normalizedVersion, bool sublist)
    {
      StringBuilder stringBuilder = new StringBuilder();
      bool flag = true;
      foreach (IMavenVersionItem version in (List<IMavenVersionItem>) versionList)
      {
        switch (version)
        {
          case MavenVersionString mavenVersionString:
            stringBuilder.Append(flag ? '$' : '!');
            string str1 = this.base64Converter.Convert(Encoding.UTF8.GetBytes(mavenVersionString.ComparableValue).AsArraySegment());
            stringBuilder.Append(str1);
            break;
          case MavenVersionInteger mavenVersionInteger:
            stringBuilder.Append(flag ? '|' : '}');
            string str2 = this.bigIntegerToBase64Converter.Convert(mavenVersionInteger.Value);
            stringBuilder.Append(str2);
            break;
          case MavenVersionList versionList1:
            stringBuilder.Append(this.Convert(versionList1, normalizedVersion, true));
            break;
          default:
            throw new InvalidOperationException("Internal Error: MavenVersionList contained an unexpected item type.");
        }
        flag = false;
      }
      if (!sublist)
      {
        stringBuilder.Append('$');
        stringBuilder.Append(this.base64Converter.Convert(Encoding.UTF8.GetBytes(MavenVersionString.ReleaseVersionIndex.ToString()).AsArraySegment()));
        stringBuilder.Append(normalizedVersion);
      }
      return stringBuilder.ToString();
    }
  }
}

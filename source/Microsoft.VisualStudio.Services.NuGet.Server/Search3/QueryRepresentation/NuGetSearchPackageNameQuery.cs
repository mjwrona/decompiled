// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation.NuGetSearchPackageNameQuery
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation
{
  public class NuGetSearchPackageNameQuery
  {
    public static readonly NuGetSearchPackageNameQuery AllNames = new NuGetSearchPackageNameQuery(string.Empty, StringComparison.OrdinalIgnoreCase, NameMatchType.AllNames);

    public string Value { get; }

    public StringComparison StringComparison { get; }

    public NameMatchType MatchType { get; }

    public NuGetSearchPackageNameQuery(
      string value,
      StringComparison stringComparison,
      NameMatchType matchType)
    {
      this.Value = value ?? throw new ArgumentNullException(nameof (value));
      this.StringComparison = stringComparison;
      this.MatchType = matchType;
    }
  }
}

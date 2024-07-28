// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation.PackageNameSelector
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation
{
  public class PackageNameSelector
  {
    public string NameString { get; }

    public bool CaseInsensitive { get; }

    public NameMatchType MatchType { get; }

    public NameMatchFields MatchFields { get; }

    public PackageNameSelector(
      string nameString,
      NameMatchFields matchFields,
      NameMatchType matchType,
      bool caseInsensitive)
    {
      this.NameString = nameString;
      this.CaseInsensitive = caseInsensitive;
      this.MatchType = matchType;
      this.MatchFields = matchFields;
      switch (this.MatchType)
      {
        case NameMatchType.Exact:
        case NameMatchType.Substring:
        case NameMatchType.Prefix:
          if (!string.IsNullOrWhiteSpace(nameString))
            break;
          throw new ArgumentException(string.Format("{0} name match requires a name string", (object) this.MatchType));
        case NameMatchType.AllNames:
          if (string.IsNullOrWhiteSpace(nameString))
            break;
          throw new ArgumentException(string.Format("{0} match cannot be combined with a name string", (object) this.MatchType));
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    public static PackageNameSelector AllPackageNames { get; } = new PackageNameSelector((string) null, NameMatchFields.Id, NameMatchType.AllNames, true);
  }
}

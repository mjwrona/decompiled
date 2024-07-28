// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Converters.MavenPackageFilesNewToLegacyConverter
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Maven.Server.Metadata;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Maven.Server.Utilities;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Maven.Server.Converters
{
  public class MavenPackageFilesNewToLegacyConverter : 
    IConverter<IEnumerable<MavenPackageFileNew>, IReadOnlyList<MavenPackageFile>>,
    IHaveInputType<IEnumerable<MavenPackageFileNew>>,
    IHaveOutputType<IReadOnlyList<MavenPackageFile>>
  {
    public IReadOnlyList<MavenPackageFile> Convert(IEnumerable<MavenPackageFileNew> input)
    {
      ImmutableList<MavenPackageFileNew> immutableList = input.ToImmutableList<MavenPackageFileNew>();
      Dictionary<string, MavenPackageFileNew> dictionary = new Dictionary<string, MavenPackageFileNew>((IEqualityComparer<string>) MavenFileNameUtility.FileNameStringComparer);
      foreach (MavenPackageFileNew mavenPackageFileNew in immutableList)
      {
        if (!dictionary.ContainsKey(mavenPackageFileNew.Path))
          dictionary[mavenPackageFileNew.Path] = mavenPackageFileNew;
      }
      foreach (MavenPackageFileNew mavenPackageFileNew in immutableList)
      {
        foreach (HashAndType hashAndType in (IEnumerable<HashAndType>) mavenPackageFileNew.Hashes ?? Enumerable.Empty<HashAndType>())
        {
          string str = mavenPackageFileNew.Path + "." + hashAndType.HashType.ToString().ToLower();
          if (!dictionary.ContainsKey(str))
            dictionary[str] = new MavenPackageFileNew(str, (IStorageId) new LiteralStringStorageId(hashAndType.Value), (long) hashAndType.Value.Length, mavenPackageFileNew.DateAdded);
        }
      }
      return (IReadOnlyList<MavenPackageFile>) dictionary.Values.Select<MavenPackageFileNew, MavenPackageFile>((Func<MavenPackageFileNew, MavenPackageFile>) (x =>
      {
        string str1 = (string) null;
        string str2 = (string) null;
        if (x.StorageId is LiteralStringStorageId storageId2)
          str1 = storageId2.Value;
        else
          str2 = x.StorageId?.ValueString;
        return new MavenPackageFile()
        {
          Name = x.Path,
          Content = str1,
          DateAdded = x.DateAdded,
          Size = x.SizeInBytes,
          StorageId = str2
        };
      })).ToImmutableList<MavenPackageFile>();
    }
  }
}

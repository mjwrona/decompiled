// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Converters.MavenPackageFilesLegacyToNewConverter
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
  public class MavenPackageFilesLegacyToNewConverter : 
    IConverter<IEnumerable<MavenPackageFile>, IReadOnlyList<MavenPackageFileNew>>,
    IHaveInputType<IEnumerable<MavenPackageFile>>,
    IHaveOutputType<IReadOnlyList<MavenPackageFileNew>>
  {
    public IReadOnlyList<MavenPackageFileNew> Convert(IEnumerable<MavenPackageFile> input)
    {
      Dictionary<string, MavenPackageFilesLegacyToNewConverter.Group> dictionary = new Dictionary<string, MavenPackageFilesLegacyToNewConverter.Group>((IEqualityComparer<string>) MavenFileNameUtility.FileNameStringComparer);
      foreach (MavenPackageFile mavenPackageFile in input)
      {
        (string str, MavenHashAlgorithmInfo Algorithm) = MavenFileNameUtility.SplitChecksumFileName(mavenPackageFile.Name);
        MavenPackageFilesLegacyToNewConverter.Group orAddValue = dictionary.GetOrAddValue<string, MavenPackageFilesLegacyToNewConverter.Group>(str, (Func<MavenPackageFilesLegacyToNewConverter.Group>) (() => new MavenPackageFilesLegacyToNewConverter.Group()
        {
          BaseName = str
        }));
        if (Algorithm == null)
          orAddValue.BaseFile = mavenPackageFile;
        else if (!string.IsNullOrWhiteSpace(mavenPackageFile.Content))
          orAddValue.Hashes.Add((new HashAndType()
          {
            HashType = Algorithm.HashType,
            Value = mavenPackageFile.Content
          }, mavenPackageFile));
        else
          orAddValue.LooseFiles.Add(mavenPackageFile);
      }
      return (IReadOnlyList<MavenPackageFileNew>) dictionary.Values.SelectMany<MavenPackageFilesLegacyToNewConverter.Group, MavenPackageFileNew>(new Func<MavenPackageFilesLegacyToNewConverter.Group, IEnumerable<MavenPackageFileNew>>(GroupToFiles)).ToImmutableList<MavenPackageFileNew>();

      static IEnumerable<MavenPackageFileNew> GroupToFiles(
        MavenPackageFilesLegacyToNewConverter.Group group)
      {
        foreach (MavenPackageFile looseFile in group.LooseFiles)
          yield return ConvertSingle(looseFile, Enumerable.Empty<HashAndType>());
        if (group.BaseFile != null)
        {
          yield return ConvertSingle(group.BaseFile, group.Hashes.Select<(HashAndType, MavenPackageFile), HashAndType>((Func<(HashAndType, MavenPackageFile), HashAndType>) (x => x.Hash)));
        }
        else
        {
          foreach (MavenPackageFile x in group.Hashes.Select<(HashAndType, MavenPackageFile), MavenPackageFile>((Func<(HashAndType, MavenPackageFile), MavenPackageFile>) (x => x.File)))
            yield return ConvertSingle(x, Enumerable.Empty<HashAndType>());
        }
      }

      static MavenPackageFileNew ConvertSingle(MavenPackageFile x, IEnumerable<HashAndType> hashes)
      {
        IStorageId storageId = (IStorageId) null;
        if (!string.IsNullOrWhiteSpace(x.Content))
          storageId = (IStorageId) new LiteralStringStorageId(x.Content);
        else if (x.StorageId != null)
          storageId = StorageId.Parse(x.StorageId);
        return new MavenPackageFileNew(x.Name, storageId, hashes, x.Size, x.DateAdded);
      }
    }

    private class Group
    {
      public string BaseName;
      public MavenPackageFile BaseFile;
      public readonly List<(HashAndType Hash, MavenPackageFile File)> Hashes = new List<(HashAndType, MavenPackageFile)>();
      public readonly List<MavenPackageFile> LooseFiles = new List<MavenPackageFile>();
    }
  }
}

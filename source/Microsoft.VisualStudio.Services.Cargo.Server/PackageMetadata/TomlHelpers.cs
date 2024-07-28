// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.PackageMetadata.TomlHelpers
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Tomlyn.Model;
using Tomlyn.Syntax;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.PackageMetadata
{
  public static class TomlHelpers
  {
    public static ImmutableArray<string>? ExtractStringArrayOrDefault(
      TomlTable tomlTable,
      string key)
    {
      TomlArray castedValueOrDefault = tomlTable.GetCastedValueOrDefault<string, TomlArray>(key);
      return castedValueOrDefault == null ? new ImmutableArray<string>?() : new ImmutableArray<string>?(castedValueOrDefault.OfType<string>().Where<string>((Func<string, bool>) (x => x != null)).ToImmutableArray<string>());
    }

    public static T? GetCastedValueThrowIfWrongTypeNullIfMissing<T>(
      this TomlTable tomlTable,
      ImmutableList<string> tablePath,
      string key)
    {
      object obj1;
      if (!tomlTable.TryGetValue(key, out obj1))
        return default (T);
      return obj1 is T obj2 ? obj2 : throw new InvalidPackageException(Resources.Error_CargoTomlWrongTypeElement((object) TomlHelpers.GetKeyString((IEnumerable<string>) tablePath.Add(key))));
    }

    public static T GetCastedValue<T>(
      this TomlTable tomlTable,
      ImmutableList<string> tablePath,
      string key)
    {
      object obj1;
      if (!tomlTable.TryGetValue(key, out obj1))
        throw new InvalidPackageException(Resources.Error_CargoTomlMissingElement((object) TomlHelpers.GetKeyString((IEnumerable<string>) tablePath.Add(key))));
      return obj1 is T obj2 ? obj2 : throw new InvalidPackageException(Resources.Error_CargoTomlWrongTypeElement((object) TomlHelpers.GetKeyString((IEnumerable<string>) tablePath.Add(key))));
    }

    public static string GetKeyString(IEnumerable<string> path)
    {
      using (IEnumerator<string> enumerator = path.GetEnumerator())
      {
        KeySyntax keySyntax = enumerator.MoveNext() ? new KeySyntax(enumerator.Current) : throw new ArgumentException("path must not be empty", nameof (path));
        while (enumerator.MoveNext())
          keySyntax.DotKeys.Add(new DottedKeyItemSyntax(enumerator.Current));
        return keySyntax.ToString();
      }
    }
  }
}

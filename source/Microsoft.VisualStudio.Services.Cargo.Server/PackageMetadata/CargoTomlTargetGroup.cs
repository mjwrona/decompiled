// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.PackageMetadata.CargoTomlTargetGroup
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Tomlyn.Model;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.PackageMetadata
{
  public record CargoTomlTargetGroup(
    ImmutableDictionary<string, CargoTomlTarget> Entries)
  {
    public static CargoTomlTargetGroup Empty { get; } = new CargoTomlTargetGroup(ImmutableDictionary<string, CargoTomlTarget>.Empty);

    public static CargoTomlTargetGroup FromTomlOrEmpty(
      TomlTable? tomlTable,
      ImmutableList<string> tomlTablePath)
    {
      return tomlTable == null ? CargoTomlTargetGroup.Empty : new CargoTomlTargetGroup(tomlTable.ToImmutableDictionary<KeyValuePair<string, object>, string, CargoTomlTarget>((Func<KeyValuePair<string, object>, string>) (x => x.Key), (Func<KeyValuePair<string, object>, CargoTomlTarget>) (x =>
      {
        ImmutableList<string> immutableList = tomlTablePath.Add(x.Key);
        if (x.Value is TomlTable tomlTable2)
          return CargoTomlTarget.FromToml(tomlTable2, immutableList);
        throw new InvalidPackageException(Resources.Error_CargoTomlWrongTypeElement((object) TomlHelpers.GetKeyString((IEnumerable<string>) immutableList)));
      })));
    }
  }
}

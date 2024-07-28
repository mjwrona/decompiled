// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity.NameDetails.CargoPackageNameValidator
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using System;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity.NameDetails
{
  public static class CargoPackageNameValidator
  {
    public const int CratesIoMaxNameLength = 64;
    public static readonly int MaxNameLength = Math.Min(64, (int) byte.MaxValue);

    public static void Validate(CargoPackageName name, bool enforceAsciiOnlyNames)
    {
      if (name.NormalizedName.Length > CargoPackageNameValidator.MaxNameLength)
        throw CargoPackageNameValidator.GetInvalidPackageException(name.DisplayName, CargoPackageNameValidator.MaxNameLength, Resources.Error_CrateNameHasTooManyChars((object) CargoPackageNameValidator.MaxNameLength));
      if (!char.IsLetter(name.DisplayName[0]))
        throw CargoPackageNameValidator.GetInvalidPackageException(name.DisplayName, 0, Resources.Error_CrateNameHasNonLetterFirstCharacter());
      int pos = 0;
      foreach (char ch in name.DisplayName)
      {
        if (enforceAsciiOnlyNames && !IsAscii(ch))
          throw CargoPackageNameValidator.GetInvalidPackageException(name.DisplayName, pos, Resources.Error_CrateNameHasNonAsciiCharacter());
        if (!char.IsLetterOrDigit(ch) && ch != '-' && ch != '_')
          throw CargoPackageNameValidator.GetInvalidPackageException(name.DisplayName, pos, Resources.Error_CrateNameHasInvalidCharacter());
        ++pos;
      }

      static bool IsAscii(char ch) => ch < '\u0080';
    }

    private static InvalidPackageException GetInvalidPackageException(
      string fullNameString,
      int pos,
      string specificMessage)
    {
      return new InvalidPackageException(Resources.Error_InvalidCrateNameAtPosition((object) fullNameString, (object) pos, (object) specificMessage));
    }
  }
}

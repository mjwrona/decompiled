// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity.NameDetails.CargoPackageNameParser
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.VisualStudio.Services.Common;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity.NameDetails
{
  public class CargoPackageNameParser
  {
    public static CargoPackageName Parse(string displayName)
    {
      CargoPackageName withoutValidating = CargoPackageNameParser.ParseWithoutValidating(displayName);
      CargoPackageNameValidator.Validate(withoutValidating, false);
      return withoutValidating;
    }

    public static CargoPackageName ParseWithoutValidating(string displayName)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(displayName, nameof (displayName));
      return new CargoPackageName(displayName, displayName.ToLowerInvariant().Replace('-', '_'));
    }
  }
}

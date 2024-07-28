// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Upload.CargoBlobRefGenerator
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity;
using System;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Upload
{
  public static class CargoBlobRefGenerator
  {
    public static IdBlobReference ForCrate(Guid feedId, CargoPackageIdentity id) => new IdBlobReference(string.Format("feed/{0}/{1}/{2}/crate/{3}", (object) feedId, (object) id.Name.NormalizedName, (object) id.Version.NormalizedVersion, (object) id.GetCanonicalCrateFileName()), Protocol.Cargo.LowercasedName);

    public static IdBlobReference ForCrateInnerFile(
      Guid feedId,
      CargoPackageIdentity id,
      string path)
    {
      return new IdBlobReference(string.Format("feed/{0}/{1}/{2}/crate/{3}/innerFiles/{4}", (object) feedId, (object) id.Name.NormalizedName, (object) id.Version.NormalizedVersion, (object) id.GetCanonicalCrateFileName(), (object) path), Protocol.Cargo.LowercasedName);
    }
  }
}

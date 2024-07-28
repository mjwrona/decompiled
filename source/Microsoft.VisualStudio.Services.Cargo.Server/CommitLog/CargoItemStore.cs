// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.CommitLog.CargoItemStore
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ItemStore;
using System.Diagnostics.CodeAnalysis;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.CommitLog
{
  [ExcludeFromCodeCoverage]
  public class CargoItemStore : PackagingItemStoreBase
  {
    public override bool CanUseTimeBasedReferences => false;

    protected override string GetExperienceName() => CargoItemStore.ExperienceName;

    public static string ExperienceName => Protocol.Cargo.LowercasedName;
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Controllers.Cargo.Index.CargoIndexDependency
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.VisualStudio.Services.Cargo.Server.Ingestion;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System.Collections.Generic;
using System.Runtime.Serialization;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Controllers.Cargo.Index
{
  [DataContract]
  public class CargoIndexDependency : PackagingSecuredObject
  {
    [DataMember(Name = "name")]
    public string DeclaredName { get; init; }

    [DataMember(Name = "req")]
    public string VersionRequirement { get; init; }

    [DataMember(Name = "features")]
    public IEnumerable<string> Features { get; init; }

    [DataMember(Name = "optional")]
    public bool Optional { get; init; }

    [DataMember(Name = "default_features")]
    public bool DefaultFeaturesEnabled { get; init; }

    [DataMember(Name = "target")]
    public string? TargetPlatform { get; init; }

    [DataMember(Name = "kind")]
    public string? Kind { get; init; }

    [DataMember(Name = "registry")]
    public string? Registry { get; init; }

    [DataMember(Name = "package")]
    public string? ActualPackageName { get; init; }

    public static CargoIndexDependency FromMetadataDependency(CargoPackageMetadataDependency x) => new CargoIndexDependency()
    {
      ActualPackageName = x.IsRenamed ? x.ActualPackageName.DisplayName : (string) null,
      DeclaredName = x.IsRenamed ? x.DeclaredName : x.ActualPackageName.DisplayName,
      DefaultFeaturesEnabled = x.DefaultFeaturesEnabled,
      Features = (IEnumerable<string>) x.Features,
      Kind = x.Kind,
      Optional = x.Optional,
      Registry = x.RegistryIndex,
      TargetPlatform = x.TargetPlatform,
      VersionRequirement = x.VersionRequirement
    };
  }
}

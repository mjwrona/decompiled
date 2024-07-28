// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Controllers.Cargo.IndexConfigJson.CargoIndexConfigJson
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System.Runtime.Serialization;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Controllers.Cargo.IndexConfigJson
{
  [DataContract]
  public class CargoIndexConfigJson : PackagingSecuredObject
  {
    [DataMember(Name = "dl")]
    public string DownloadLink { get; set; }

    [DataMember(Name = "api", EmitDefaultValue = false)]
    public string? ApiUrl { get; set; }

    [DataMember(Name = "auth-required", EmitDefaultValue = false)]
    public bool? AuthRequired { get; set; }
  }
}

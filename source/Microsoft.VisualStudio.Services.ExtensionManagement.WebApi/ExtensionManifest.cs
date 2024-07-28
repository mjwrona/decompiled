// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionManifest
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4FCC2C3-B106-43A6-A409-E4BF8CFC545C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.WebApi
{
  [DataContract]
  public class ExtensionManifest
  {
    public ExtensionManifest()
    {
    }

    public ExtensionManifest(ExtensionManifest sourceManifest)
    {
      this.BaseUri = sourceManifest.BaseUri;
      this.FallbackBaseUri = sourceManifest.FallbackBaseUri;
      this.Constraints = sourceManifest.Constraints;
      this.Contributions = sourceManifest.Contributions;
      this.ContributionTypes = sourceManifest.ContributionTypes;
      this.EventCallbacks = sourceManifest.EventCallbacks;
      this.Language = sourceManifest.Language;
      this.ManifestVersion = sourceManifest.ManifestVersion;
      this.Scopes = sourceManifest.Scopes;
      this.ServiceInstanceType = sourceManifest.ServiceInstanceType;
      this.Demands = sourceManifest.Demands;
      this.Licensing = sourceManifest.Licensing;
    }

    [DataMember(EmitDefaultValue = false, Order = 60)]
    public string Language { get; set; }

    [DataMember(EmitDefaultValue = false, Order = 70)]
    public double ManifestVersion { get; set; }

    [DataMember(EmitDefaultValue = false, Order = 80)]
    public string BaseUri { get; set; }

    [DataMember(EmitDefaultValue = false, Order = 85)]
    public string FallbackBaseUri { get; set; }

    [DataMember(EmitDefaultValue = false, Order = 90)]
    public ExtensionEventCallbackCollection EventCallbacks { get; set; }

    [DataMember(EmitDefaultValue = false, Order = 100)]
    public List<string> Scopes { get; set; }

    [DataMember(EmitDefaultValue = false, Order = 110)]
    public IEnumerable<Contribution> Contributions { get; set; }

    [DataMember(EmitDefaultValue = false, Order = 120)]
    public IEnumerable<ContributionType> ContributionTypes { get; set; }

    [DataMember(EmitDefaultValue = false, Order = 125)]
    public IEnumerable<ContributionConstraint> Constraints { get; set; }

    [DataMember(EmitDefaultValue = false, Order = 130)]
    public Guid? ServiceInstanceType { get; set; }

    [DataMember(EmitDefaultValue = false, Order = 140)]
    public List<string> Demands { get; set; }

    [DataMember(EmitDefaultValue = false, Order = 150)]
    public ExtensionLicensing Licensing { get; set; }

    [DataMember(EmitDefaultValue = false, Order = 160)]
    public IEnumerable<string> RestrictedTo { get; set; }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.ExtensionSource
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Licensing
{
  [DataContract]
  public class ExtensionSource : IEquatable<ExtensionSource>
  {
    [DataMember]
    public string ExtensionGalleryId { get; set; }

    [DataMember]
    public LicensingSource LicensingSource { get; set; }

    [DataMember]
    public AssignmentSource AssignmentSource { get; set; }

    public bool Equals(ExtensionSource other)
    {
      if (other == null)
        return false;
      return this == other || string.Equals(this.ExtensionGalleryId, other.ExtensionGalleryId);
    }

    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;
      if (this == obj)
        return true;
      return !(obj.GetType() != this.GetType()) && this.Equals((ExtensionSource) obj);
    }

    public override int GetHashCode() => this.ExtensionGalleryId.GetHashCode();
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.Internal.ClientRightBase
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Licensing.Internal
{
  public abstract class ClientRightBase : IClientRight, IUsageRight, IComparable
  {
    public abstract Dictionary<string, object> Attributes { get; }

    public virtual string AuthorizedVSEdition { get; set; }

    public virtual Version ClientVersion { get; set; }

    public virtual DateTimeOffset ExpirationDate { get; set; }

    public virtual string LicenseDescriptionId { get; set; }

    public virtual string LicenseFallbackDescription { get; set; }

    public virtual string LicenseUrl { get; set; }

    public virtual string LicenseSourceName { get; set; }

    public abstract string Name { get; }

    public virtual Version Version { get; set; }

    public override bool Equals(object obj)
    {
      ClientRightBase right = obj as ClientRightBase;
      return obj != null && right != null && this.Equals(right);
    }

    public bool Equals(ClientRightBase right) => right != null && string.Compare(this.AuthorizedVSEdition, right.AuthorizedVSEdition) == 0 && this.ClientVersion.Equals(right.ClientVersion) && this.ExpirationDate.Equals(right.ExpirationDate) && string.Compare(this.LicenseDescriptionId, right.LicenseDescriptionId) == 0 && string.Compare(this.LicenseFallbackDescription, right.LicenseFallbackDescription) == 0 && string.Compare(this.LicenseUrl, right.LicenseUrl) == 0 && string.Compare(this.LicenseSourceName, right.LicenseSourceName) == 0 && ClientRightBase.AttributesEqual(this.Attributes, right.Attributes);

    public override int GetHashCode() => 21 * (21 * (21 * (21 * (21 * (21 * (21 * (this.AuthorizedVSEdition == null ? 0 : this.AuthorizedVSEdition.GetHashCode()) + this.ClientVersion.GetHashCode()) + this.ExpirationDate.GetHashCode()) + (this.LicenseDescriptionId == null ? 0 : this.LicenseDescriptionId.GetHashCode())) + (this.LicenseFallbackDescription == null ? 0 : this.LicenseFallbackDescription.GetHashCode())) + (this.LicenseUrl == null ? 0 : this.LicenseUrl.GetHashCode())) + (this.LicenseSourceName == null ? 0 : this.LicenseSourceName.GetHashCode())) + (this.Attributes == null ? 0 : this.Attributes.GetHashCode());

    public abstract int CompareTo(object obj);

    private static bool AttributesEqual(
      Dictionary<string, object> attributes1,
      Dictionary<string, object> attributes2)
    {
      if (attributes1 == null)
        return attributes2 == null;
      if (attributes1.Count != attributes2.Count)
        return false;
      foreach (KeyValuePair<string, object> keyValuePair in attributes1)
      {
        object obj;
        if (!attributes2.TryGetValue(keyValuePair.Key, out obj) || !keyValuePair.Value.Equals(obj))
          return false;
      }
      return true;
    }
  }
}

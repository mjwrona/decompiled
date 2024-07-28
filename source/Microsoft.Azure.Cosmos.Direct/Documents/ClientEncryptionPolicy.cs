// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ClientEncryptionPolicy
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Microsoft.Azure.Documents
{
  internal sealed class ClientEncryptionPolicy : JsonSerializable
  {
    private Collection<ClientEncryptionIncludedPath> includedPaths;

    [JsonProperty(PropertyName = "includedPaths")]
    public Collection<ClientEncryptionIncludedPath> IncludedPaths
    {
      get
      {
        if (this.includedPaths == null)
        {
          this.includedPaths = this.GetObjectCollection<ClientEncryptionIncludedPath>("includedPaths");
          if (this.includedPaths == null)
            this.includedPaths = new Collection<ClientEncryptionIncludedPath>();
        }
        return this.includedPaths;
      }
      set
      {
        this.includedPaths = value != null ? value : throw new ArgumentNullException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, RMResources.PropertyCannotBeNull, (object) nameof (IncludedPaths)));
        this.SetObjectCollection<ClientEncryptionIncludedPath>("includedPaths", this.includedPaths);
      }
    }

    internal override void OnSave()
    {
      if (this.includedPaths == null)
        return;
      this.SetObjectCollection<ClientEncryptionIncludedPath>("includedPaths", this.includedPaths);
    }
  }
}

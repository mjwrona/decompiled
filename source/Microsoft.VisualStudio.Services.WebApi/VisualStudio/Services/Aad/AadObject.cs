// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.AadObject
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Aad
{
  [DataContract]
  public abstract class AadObject
  {
    [DataMember]
    private Guid objectId;
    [DataMember]
    private string displayName;

    protected AadObject()
    {
    }

    public AadObject(Guid objectId, string displayName)
    {
      this.objectId = objectId;
      this.displayName = displayName;
    }

    public Guid ObjectId
    {
      get => this.objectId;
      set => this.objectId = value;
    }

    public string DisplayName
    {
      get => this.displayName;
      set => this.displayName = value;
    }

    public override bool Equals(object obj) => this == obj || this.Equals(obj as AadObject);

    private bool Equals(AadObject other) => other != null && object.Equals((object) this.ObjectId, (object) other.ObjectId) && string.Equals(this.DisplayName, other.DisplayName);

    public override int GetHashCode() => this.ObjectId.GetHashCode();
  }
}

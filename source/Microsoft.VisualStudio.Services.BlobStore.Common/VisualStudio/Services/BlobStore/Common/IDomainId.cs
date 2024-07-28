// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.IDomainId
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  public abstract class IDomainId : IEquatable<IDomainId>, ISerializable
  {
    public abstract string Serialize();

    public void GetObjectData(SerializationInfo info, StreamingContext context) => info.AddValue("Id", (object) this.Serialize());

    public static bool operator ==(IDomainId a, IDomainId b)
    {
      if ((object) a != null && a.Equals(b))
        return true;
      return (object) a == null && (object) b == null;
    }

    public static bool operator !=(IDomainId a, IDomainId b)
    {
      if ((object) a != null && !a.Equals(b))
        return true;
      return (object) a == null && b != null;
    }

    public override sealed bool Equals(object other)
    {
      IDomainId other1 = other as IDomainId;
      return (object) other1 != null && this.Equals(other1);
    }

    public abstract bool Equals(IDomainId other);

    public abstract override int GetHashCode();

    public override sealed string ToString() => this.Serialize();
  }
}

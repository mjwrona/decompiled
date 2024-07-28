// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Entities.EntityType
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Common.Entities
{
  [DataContract]
  public abstract class EntityType : IEntityType
  {
    [DataMember(Order = 0)]
    public abstract string Name { get; set; }

    [DataMember(Order = 1)]
    public abstract int ID { get; set; }

    public override bool Equals(object obj)
    {
      if (!(obj is IEntityType entityType))
        return false;
      if (entityType == this)
        return true;
      return entityType.ID == this.ID && string.Equals(entityType.Name, this.Name, StringComparison.OrdinalIgnoreCase);
    }

    public override int GetHashCode() => this.Name.GetHashCode() ^ this.ID.GetHashCode();

    public override string ToString() => this.Name;
  }
}

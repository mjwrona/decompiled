// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.MetadataModel
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.OData.Edm;
using System;
using System.Data.Entity.Infrastructure;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  public class MetadataModel
  {
    public MetadataModel(
      Type type,
      int version,
      IEdmModel odataModel,
      DbCompiledModel efModel,
      int level)
    {
      this.Type = type;
      this.Version = version;
      this.EdmModel = odataModel;
      this.EFModel = efModel;
      this.Level = level;
    }

    public Type Type { get; }

    public int Version { get; }

    public IEdmModel EdmModel { get; }

    public DbCompiledModel EFModel { get; }

    public int Level { get; }

    public override bool Equals(object obj) => this.Equals(obj as MetadataModel);

    public bool Equals(MetadataModel other) => other != null && this.Type == other.Type && this.Level == other.Level && this.Version == other.Version;

    public override int GetHashCode() => ((17 * 23 + (this.Type != (Type) null ? this.Type.GetHashCode() : 0)) * 23 + this.Version) * 23 + this.Level;
  }
}

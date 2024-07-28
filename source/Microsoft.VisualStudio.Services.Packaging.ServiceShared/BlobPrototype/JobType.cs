// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobType
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class JobType
  {
    public JobType(Guid id, string name)
    {
      this.Id = id;
      this.Name = name;
    }

    public Guid Id { get; }

    public string Name { get; }

    public override int GetHashCode() => this.Id.GetHashCode();

    public bool Equals(JobType other) => this.Id.Equals(other.Id);

    public override bool Equals(object obj) => obj != null && obj is JobType && this.Equals((JobType) obj);
  }
}

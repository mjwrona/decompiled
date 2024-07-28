// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PipelineCache.Common.CacheScope
// Assembly: Microsoft.VisualStudio.Services.PipelineCache.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E063C74A-FCE9-47BF-84C0-7143B7075032
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PipelineCache.Common.dll

using System;
using System.Text;

namespace Microsoft.VisualStudio.Services.PipelineCache.Common
{
  public class CacheScope : IEquatable<CacheScope>
  {
    private const string SubTokenSeparator = ";";

    public int BuildDefinitionId { get; set; }

    public Guid ProjectId { get; set; }

    public string Branch { get; set; }

    public string RepositoryId { get; set; }

    public string Serialize()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(this.BuildDefinitionId);
      stringBuilder.Append(";");
      stringBuilder.Append((object) this.ProjectId);
      stringBuilder.Append(";");
      stringBuilder.Append(this.Branch);
      stringBuilder.Append(";");
      stringBuilder.Append(this.RepositoryId);
      return stringBuilder.ToString();
    }

    public bool Equals(CacheScope other) => other != null && this.BuildDefinitionId == other.BuildDefinitionId && this.ProjectId == other.ProjectId && this.Branch.Equals(other.Branch, StringComparison.Ordinal) && this.RepositoryId.Equals(other.RepositoryId, StringComparison.Ordinal);

    public override bool Equals(object obj) => obj is CacheScope other && this.Equals(other);

    public override int GetHashCode() => this.BuildDefinitionId.GetHashCode() ^ this.ProjectId.GetHashCode() ^ this.Branch.GetHashCode() ^ this.RepositoryId.GetHashCode();

    public override string ToString() => this.Serialize();
  }
}

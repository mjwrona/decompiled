// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Tags.Server.TagDefinition
// Assembly: Microsoft.Azure.Devops.Tags.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10FDC8E3-D1DB-4668-B2F2-04DAA10A7143
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Devops.Tags.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Devops.Tags.Server
{
  public class TagDefinition
  {
    public TagDefinition(
      Guid tagId,
      string name,
      IEnumerable<Guid> kindIds,
      bool includesAllArtifactKinds,
      Guid scope,
      TagDefinitionStatus status,
      DateTime lastUpdated)
    {
      this.TagId = tagId;
      this.Name = name;
      this.ApplicableKindIds = kindIds;
      this.IncludesAllArtifactKinds = includesAllArtifactKinds;
      this.Scope = scope;
      this.Status = status;
      this.LastUpdated = lastUpdated;
    }

    public Guid TagId { get; private set; }

    public string Name { get; private set; }

    public IEnumerable<Guid> ApplicableKindIds { get; private set; }

    public bool IncludesAllArtifactKinds { get; private set; }

    public Guid Scope { get; private set; }

    public TagDefinitionStatus Status { get; private set; }

    public DateTime LastUpdated { get; private set; }

    public void Normalize() => this.Name = TaggingHelper.NormalizeUnicode(this.Name);

    public bool IsDeleted => this.Status == TagDefinitionStatus.Inactive;

    public bool IsGlobalScope => this.Scope == Guid.Empty;

    public static TagDefinition Create(Guid scope, string name, Guid? tagId = null) => new TagDefinition(tagId ?? Guid.NewGuid(), name, (IEnumerable<Guid>) null, true, scope, TagDefinitionStatus.Normal, DateTime.UtcNow);
  }
}

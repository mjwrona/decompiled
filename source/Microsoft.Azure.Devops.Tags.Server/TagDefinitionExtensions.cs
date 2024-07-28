// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Tags.Server.TagDefinitionExtensions
// Assembly: Microsoft.Azure.Devops.Tags.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10FDC8E3-D1DB-4668-B2F2-04DAA10A7143
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Devops.Tags.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Devops.Tags.Server
{
  public static class TagDefinitionExtensions
  {
    public static TagDefinition Clone(
      this TagDefinition tag,
      Guid? tagId = null,
      string name = null,
      IEnumerable<Guid> applicableKindIds = null,
      bool? includesAllArtifactKinds = null,
      Guid? scope = null,
      TagDefinitionStatus? status = null,
      DateTime? lastUpdated = null)
    {
      return new TagDefinition(tagId ?? tag.TagId, name ?? tag.Name, applicableKindIds ?? tag.ApplicableKindIds, ((int) includesAllArtifactKinds ?? (tag.IncludesAllArtifactKinds ? 1 : 0)) != 0, scope ?? tag.Scope, (TagDefinitionStatus) ((int) status ?? (int) tag.Status), lastUpdated ?? tag.LastUpdated);
    }

    public static WebApiTagDefinition ToApiTagDefinition(
      this TagDefinition tagDefinition,
      ISecuredObject securedObject = null)
    {
      return new WebApiTagDefinition(securedObject)
      {
        Id = tagDefinition.TagId,
        Name = tagDefinition.Name,
        Active = new bool?(!tagDefinition.IsDeleted)
      };
    }

    public static IEnumerable<WebApiTagDefinition> ToApiTagDefinitions(
      this IEnumerable<TagDefinition> tagDefinitions,
      ISecuredObject securedObject = null)
    {
      return tagDefinitions.Select<TagDefinition, WebApiTagDefinition>((Func<TagDefinition, WebApiTagDefinition>) (tagDefinition => tagDefinition.ToApiTagDefinition(securedObject)));
    }
  }
}

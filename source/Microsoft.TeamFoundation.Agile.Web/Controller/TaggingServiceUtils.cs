// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Controller.TaggingServiceUtils
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.Azure.Devops.Tags.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Agile.Web.Controller
{
  internal sealed class TaggingServiceUtils
  {
    internal static IEnumerable<TagDefinition> EnsureTagDefinitions_NoActivations(
      IVssRequestContext requestContext,
      IEnumerable<string> tagNames,
      Guid[] artifactKinds,
      Guid scope)
    {
      ITeamFoundationTaggingService service = requestContext.GetService<ITeamFoundationTaggingService>();
      HashSet<string> source = new HashSet<string>(tagNames, (IEqualityComparer<string>) VssStringComparer.TagName);
      IEnumerable<TagDefinition> tagDefinitions = service.QueryTagDefinitions(requestContext, (IEnumerable<Guid>) artifactKinds, scope);
      Dictionary<string, TagDefinition> scopeTagsMap = new Dictionary<string, TagDefinition>((IEqualityComparer<string>) VssStringComparer.TagName);
      foreach (TagDefinition tagDefinition in tagDefinitions)
      {
        if (!scopeTagsMap.ContainsKey(tagDefinition.Name))
          scopeTagsMap[tagDefinition.Name] = tagDefinition;
      }
      foreach (string str in source)
      {
        if (!scopeTagsMap.TryGetValue(str, out TagDefinition _))
        {
          TagDefinition tagDefinition;
          try
          {
            tagDefinition = service.CreateTagDefinition(requestContext, str, (IEnumerable<Guid>) artifactKinds, scope, TagDefinitionStatus.Inactive);
          }
          catch (DuplicateTagNameException ex)
          {
            tagDefinition = service.GetTagDefinition(requestContext, str, scope);
          }
          scopeTagsMap[str] = tagDefinition;
        }
      }
      TagDefinition tagDefinition1;
      return source.Select<string, TagDefinition>((Func<string, TagDefinition>) (tagName => scopeTagsMap.TryGetValue(tagName, out tagDefinition1) ? tagDefinition1 : (TagDefinition) null)).Where<TagDefinition>((Func<TagDefinition, bool>) (tagDef => tagDef != null));
    }
  }
}

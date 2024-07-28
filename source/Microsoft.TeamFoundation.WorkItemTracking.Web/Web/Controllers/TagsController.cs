// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.TagsController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Devops.Tags.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Factories;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [ControllerApiVersion(5.2)]
  [VersionedApiControllerCustomName(Area = "wit", ResourceName = "tags", ResourceVersion = 1)]
  public class TagsController : WorkItemTrackingApiController
  {
    private ITeamFoundationTaggingService m_taggingService;

    [HttpGet]
    [TraceFilter(560101, 560110)]
    [ClientExample("GET__list_of_wit_tags.json", "List of work item tags", null, null)]
    [ClientLocationId("BC15BC60-E7A8-43CB-AB01-2106BE3983A1")]
    public IEnumerable<WorkItemTagDefinition> GetTags()
    {
      bool excludeUrl = MediaTypeFormatUtility.GetExcludeUrlsAcceptHeaderOptionValue(this.Request);
      return (IEnumerable<WorkItemTagDefinition>) this.TaggingService.QueryTagDefinitions(this.TfsRequestContext, (IEnumerable<Guid>) new Guid[1]
      {
        WorkItemArtifactKinds.WorkItem
      }, this.ProjectInfo.Id).Select<TagDefinition, WorkItemTagDefinition>((Func<TagDefinition, WorkItemTagDefinition>) (t => WorkItemTagDefinitionFactory.Create(this.TfsRequestContext, t, excludeUrl))).ToList<WorkItemTagDefinition>();
    }

    [HttpGet]
    [TraceFilter(560111, 560120)]
    [ClientExample("GET__wit_tag.json", "Get a work item tag", null, null)]
    [ClientLocationId("BC15BC60-E7A8-43CB-AB01-2106BE3983A1")]
    public WorkItemTagDefinition GetTag(string tagIdOrName)
    {
      TagDefinition tagDefinition;
      if (!this.TryGetTagDefinition(tagIdOrName, out tagDefinition))
        throw new TagDefinitionNotFoundException(tagIdOrName);
      bool headerOptionValue = MediaTypeFormatUtility.GetExcludeUrlsAcceptHeaderOptionValue(this.Request);
      return WorkItemTagDefinitionFactory.Create(this.TfsRequestContext, tagDefinition, headerOptionValue);
    }

    [HttpPatch]
    [TraceFilter(560121, 560130)]
    [ClientResponseType(typeof (WorkItemTagDefinition), null, null)]
    [ClientExample("PATCH__wit_tag.json", "Update a work item tag", null, null)]
    public HttpResponseMessage UpdateTag(string tagIdOrName, [FromBody] WorkItemTagDefinition tagData)
    {
      ArgumentUtility.CheckForNull<WorkItemTagDefinition>(tagData, nameof (tagData));
      TagDefinition tagDefinition;
      if (!this.TryGetTagDefinition(tagIdOrName, out tagDefinition))
        throw new TagDefinitionNotFoundException(tagIdOrName);
      TagDefinition tag = tagDefinition;
      string str = tagData.Name != null ? tagData.Name : tagDefinition.Name;
      Guid? tagId = new Guid?();
      string name = str;
      bool? includesAllArtifactKinds = new bool?();
      Guid? scope = new Guid?();
      TagDefinitionStatus? status = new TagDefinitionStatus?();
      DateTime? lastUpdated = new DateTime?();
      return this.Request.CreateResponse<WorkItemTagDefinition>(HttpStatusCode.OK, WorkItemTagDefinitionFactory.Create(this.TfsRequestContext, this.TaggingService.UpdateTagDefinition(this.TfsRequestContext, tag.Clone(tagId, name, includesAllArtifactKinds: includesAllArtifactKinds, scope: scope, status: status, lastUpdated: lastUpdated)), MediaTypeFormatUtility.GetExcludeUrlsAcceptHeaderOptionValue(this.Request)));
    }

    [HttpDelete]
    [TraceFilter(560131, 560140)]
    [ClientResponseType(typeof (void), null, null)]
    [ClientExample("DELETE__wit_tag.json", "Delete a work item tag", null, null)]
    public HttpResponseMessage DeleteTag(string tagIdOrName)
    {
      TagDefinition tagDefinition;
      if (this.TryGetTagDefinition(tagIdOrName, out tagDefinition))
        this.TaggingService.DeleteTagDefinition(this.TfsRequestContext, tagDefinition.TagId);
      return this.Request.CreateResponse(HttpStatusCode.NoContent);
    }

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<TagDefinitionNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<DuplicateTagNameException>(HttpStatusCode.Conflict);
    }

    private bool TryGetTagDefinition(string tagIdOrName, out TagDefinition tagDefinition)
    {
      tagDefinition = (TagDefinition) null;
      Guid result;
      if (Guid.TryParse(tagIdOrName, out result))
      {
        TagDefinition tagDefinition1 = this.TaggingService.GetTagDefinition(this.TfsRequestContext, result);
        if (tagDefinition1 != null && tagDefinition1.Scope == this.ProjectInfo.Id)
          tagDefinition = tagDefinition1;
      }
      else
        tagDefinition = this.TaggingService.GetTagDefinition(this.TfsRequestContext, tagIdOrName, this.ProjectInfo.Id);
      return tagDefinition != null;
    }

    private ITeamFoundationTaggingService TaggingService
    {
      get
      {
        if (this.m_taggingService == null)
          this.m_taggingService = this.TfsRequestContext.GetService<ITeamFoundationTaggingService>();
        return this.m_taggingService;
      }
    }
  }
}

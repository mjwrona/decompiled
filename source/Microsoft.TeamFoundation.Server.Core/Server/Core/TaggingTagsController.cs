// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.TaggingTagsController
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.Azure.Devops.Tags.Server;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Server.Core
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [VersionedApiControllerCustomName("Tagging", "tags", 1)]
  public class TaggingTagsController : TfsApiController
  {
    private static readonly IDictionary<Type, HttpStatusCode> s_httpExceptions = (IDictionary<Type, HttpStatusCode>) new Dictionary<Type, HttpStatusCode>()
    {
      {
        typeof (TagDefinitionNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (InvalidTagNameException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (DuplicateTagNameException),
        HttpStatusCode.Conflict
      }
    };
    private static readonly Dictionary<Type, Type> s_exceptionMapping = new Dictionary<Type, Type>()
    {
      {
        typeof (TagDefinitionNotFoundException),
        typeof (TagNotFoundException)
      },
      {
        typeof (InvalidTagNameException),
        typeof (TagOperationFailed)
      },
      {
        typeof (DuplicateTagNameException),
        typeof (TagOperationFailed)
      },
      {
        typeof (AccessCheckException),
        typeof (TaggingTagsController.VssAccessCheckException)
      }
    };
    private Lazy<ITeamFoundationTaggingService> m_taggingService;

    public TaggingTagsController() => this.m_taggingService = new Lazy<ITeamFoundationTaggingService>((Func<ITeamFoundationTaggingService>) (() => this.TfsRequestContext.GetService<ITeamFoundationTaggingService>()), false);

    internal TaggingTagsController(ITeamFoundationTaggingService taggingService)
    {
      ArgumentUtility.CheckForNull<ITeamFoundationTaggingService>(taggingService, nameof (taggingService));
      this.m_taggingService = new Lazy<ITeamFoundationTaggingService>((Func<ITeamFoundationTaggingService>) (() => taggingService));
    }

    public override string TraceArea => "Tagging";

    public override string ActivityLogArea => "Tags";

    private ITeamFoundationTaggingService TaggingService => this.m_taggingService.Value;

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => TaggingTagsController.s_httpExceptions;

    public override IDictionary<Type, Type> TranslatedExceptions => (IDictionary<Type, Type>) TaggingTagsController.s_exceptionMapping;

    [TraceFilter(103100, 103101)]
    [HttpGet]
    public WebApiTagDefinition GetTagDefinition(Guid scopeId, string tagId)
    {
      ArgumentUtility.CheckForNull<string>(nameof (tagId), tagId);
      TagDefinition tagDefinition = (TagDefinition) null;
      Guid result;
      bool flag1 = Guid.TryParse(tagId, out result);
      bool flag2 = !flag1;
      if (flag1)
      {
        tagDefinition = this.TaggingService.GetTagDefinition(this.TfsRequestContext, result);
        if (tagDefinition == null)
          flag2 = true;
      }
      if (flag2)
        tagDefinition = this.TaggingService.GetTagDefinition(this.TfsRequestContext, tagId, scopeId);
      if (tagDefinition == null || tagDefinition.Scope != scopeId)
        throw flag1 ? new TagDefinitionNotFoundException(tagId) : new TagDefinitionNotFoundException(result);
      return this.CreateTagDefinitionResponse(tagDefinition);
    }

    [TraceFilter(103102, 103103)]
    [HttpGet]
    [ClientResponseType(typeof (List<WebApiTagDefinition>), null, null)]
    public List<WebApiTagDefinition> GetTagDefinitions(
      Guid scopeId,
      bool includeInactive = false,
      Guid? artifactKind = null)
    {
      IEnumerable<TagDefinition> source;
      if (!artifactKind.HasValue)
        source = this.TaggingService.GetTagDefinitions(this.TfsRequestContext, scopeId);
      else
        source = this.TaggingService.QueryTagDefinitions(this.TfsRequestContext, (IEnumerable<Guid>) new Guid[1]
        {
          artifactKind.GetValueOrDefault()
        }, scopeId);
      if (!includeInactive)
        source = source.Where<TagDefinition>((Func<TagDefinition, bool>) (tag => tag.Status == TagDefinitionStatus.Normal));
      bool excludeUrl = MediaTypeFormatUtility.GetExcludeUrlsAcceptHeaderOptionValue(this.Request);
      return source.Select<TagDefinition, WebApiTagDefinition>((Func<TagDefinition, WebApiTagDefinition>) (tag => this.CreateTagDefinitionResponse(tag, excludeUrl))).ToList<WebApiTagDefinition>();
    }

    [TraceFilter(103104, 103105)]
    [HttpPost]
    [ClientResponseType(typeof (WebApiTagDefinition), null, null)]
    public HttpResponseMessage CreateTagDefinition(
      Guid scopeId,
      [FromBody] WebApiCreateTagRequestData tagData,
      string tagId = null)
    {
      if (tagId != null)
        return this.Request.CreateResponse(HttpStatusCode.MethodNotAllowed);
      ArgumentUtility.CheckForNull<WebApiCreateTagRequestData>(tagData, nameof (tagData));
      WebApiTagDefinition definitionResponse = this.CreateTagDefinitionResponse(this.TaggingService.CreateTagDefinition(this.TfsRequestContext, tagData.Name, scopeId));
      HttpResponseMessage response = this.Request.CreateResponse<WebApiTagDefinition>(HttpStatusCode.Created, definitionResponse);
      response.Headers.Location = new Uri(definitionResponse.Url);
      return response;
    }

    [TraceFilter(103106, 103107)]
    [HttpPatch]
    [ClientResponseType(typeof (WebApiTagDefinition), null, null)]
    public HttpResponseMessage UpdateTagDefinition(
      Guid scopeId,
      Guid tagId,
      [FromBody] WebApiTagDefinition tagData)
    {
      ArgumentUtility.CheckForNull<WebApiTagDefinition>(tagData, nameof (tagData));
      TagDefinition tagDefinition = this.TaggingService.GetTagDefinition(this.TfsRequestContext.Elevate(), tagId);
      if (tagDefinition == null || tagDefinition.Scope != scopeId)
        throw new TagDefinitionNotFoundException(tagId);
      TagDefinition tag = tagDefinition;
      string str = tagData.Name != null ? tagData.Name : tagDefinition.Name;
      TagDefinitionStatus? nullable1;
      ref TagDefinitionStatus? local = ref nullable1;
      bool? nullable2 = tagData.Active;
      int num;
      if (!nullable2.HasValue)
      {
        num = (int) tagDefinition.Status;
      }
      else
      {
        nullable2 = tagData.Active;
        num = nullable2.Value ? 0 : 1;
      }
      local = new TagDefinitionStatus?((TagDefinitionStatus) num);
      Guid? tagId1 = new Guid?();
      string name = str;
      nullable2 = new bool?();
      bool? includesAllArtifactKinds = nullable2;
      Guid? scope = new Guid?();
      TagDefinitionStatus? status = nullable1;
      DateTime? lastUpdated = new DateTime?();
      return this.Request.CreateResponse<WebApiTagDefinition>(HttpStatusCode.OK, this.CreateTagDefinitionResponse(this.TaggingService.UpdateTagDefinition(this.TfsRequestContext, tag.Clone(tagId1, name, includesAllArtifactKinds: includesAllArtifactKinds, scope: scope, status: status, lastUpdated: lastUpdated))));
    }

    [TraceFilter(103108, 103109)]
    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage DeleteTagDefinition(Guid scopeId, Guid tagId)
    {
      TagDefinition tagDefinition = this.TaggingService.GetTagDefinition(this.TfsRequestContext.Elevate(), tagId);
      try
      {
        if (tagDefinition != null)
        {
          if (tagDefinition.Scope == scopeId)
            this.TaggingService.DeleteTagDefinition(this.TfsRequestContext, tagDefinition.TagId);
        }
      }
      catch (NotSupportedException ex)
      {
        return this.Request.CreateResponse(HttpStatusCode.MethodNotAllowed);
      }
      return this.Request.CreateResponse(HttpStatusCode.OK);
    }

    private WebApiTagDefinition CreateTagDefinitionResponse(
      TagDefinition tagDefinition,
      bool excludeUrl = false)
    {
      ArgumentUtility.CheckForNull<TagDefinition>(tagDefinition, nameof (tagDefinition));
      WebApiTagDefinition definitionResponse = new WebApiTagDefinition()
      {
        Id = tagDefinition.TagId,
        Name = tagDefinition.Name,
        Active = new bool?(!tagDefinition.IsDeleted)
      };
      if (!excludeUrl)
        definitionResponse.Url = this.GetTagResourceUrl(tagDefinition);
      return definitionResponse;
    }

    private string GetTagResourceUrl(TagDefinition tagDefinition)
    {
      var routeValues = new
      {
        scopeId = tagDefinition.Scope,
        tagId = tagDefinition.TagId
      };
      return this.TfsRequestContext.GetService<ILocationService>().GetResourceUri(this.TfsRequestContext, "Tagging", TaggingWebApiConstants.TagsLocationId, (object) routeValues).AbsoluteUri;
    }

    private class VssAccessCheckException : AccessCheckException
    {
      private static readonly Version s_backCompatExclusiveMaxVersion = new Version(3, 0);

      public VssAccessCheckException(string message)
        : base(message)
      {
      }

      public override void GetTypeNameAndKey(
        Version apiVersion,
        out string typeName,
        out string typeKey)
      {
        Type type = this.GetType();
        typeName = "Microsoft.VisualStudio.Services.Common." + type.Name + ", " + type.Assembly.FullName;
        typeName = typeName.Replace(Assembly.GetExecutingAssembly().GetName().Name, "Microsoft.VisualStudio.Services.Common");
        if (apiVersion != (Version) null && apiVersion < TaggingTagsController.VssAccessCheckException.s_backCompatExclusiveMaxVersion)
          typeName.Replace("19.0.0.0", "14.0.0.0");
        typeKey = type.Name;
      }
    }
  }
}

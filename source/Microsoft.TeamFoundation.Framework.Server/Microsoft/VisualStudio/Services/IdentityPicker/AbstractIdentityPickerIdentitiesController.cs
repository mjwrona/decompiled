// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.IdentityPicker.AbstractIdentityPickerIdentitiesController
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.IdentityPicker.Extensions;
using Microsoft.VisualStudio.Services.IdentityPicker.Operations;
using Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Microsoft.VisualStudio.Services.IdentityPicker
{
  public abstract class AbstractIdentityPickerIdentitiesController : TfsApiController
  {
    internal static readonly Dictionary<Type, HttpStatusCode> s_httpIdentityPickerServiceExceptions = new Dictionary<Type, HttpStatusCode>()
    {
      {
        typeof (IdentityPickerValidateException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (IdentityPickerForbiddenOperationException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (IdentityPickerAuthorizationException),
        HttpStatusCode.Unauthorized
      },
      {
        typeof (IdentityPickerProcessException),
        HttpStatusCode.InternalServerError
      },
      {
        typeof (IdentityPickerMultipleExtensionsException),
        HttpStatusCode.InternalServerError
      },
      {
        typeof (IdentityPickerServiceInitializationException),
        HttpStatusCode.InternalServerError
      },
      {
        typeof (IdentityPickerIdentityCreateException),
        HttpStatusCode.InternalServerError
      },
      {
        typeof (IdentityPickerImageRetrievalException),
        HttpStatusCode.InternalServerError
      },
      {
        typeof (IdentityPickerAdapterException),
        HttpStatusCode.InternalServerError
      },
      {
        typeof (IdentityPickerServiceException),
        HttpStatusCode.InternalServerError
      },
      {
        typeof (IdentityPickerExtensionException),
        HttpStatusCode.InternalServerError
      }
    };
    private static readonly TimeSpan defaultClientCacheControlMaxAge = new TimeSpan(0, 12, 0, 0);
    private const string ClientCacheControlMaxAgeRegistryPath = "/Configuration/Identity/IdentityPicker/ClientCacheControlMaxAge";

    public virtual HttpResponseMessage GetIdentities(IdentitiesSearchRequestModel identitiesRequest)
    {
      if (identitiesRequest == null)
        throw new IdentityPickerArgumentException(nameof (identitiesRequest));
      AbstractIdentityPickerIdentitiesController.CheckReadBasicPropertiesPermission(this.TfsRequestContext);
      try
      {
        IdentityPickerService service = this.TfsRequestContext.GetService<IdentityPickerService>();
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        Microsoft.VisualStudio.Services.IdentityPicker.Operations.SearchRequest request = new Microsoft.VisualStudio.Services.IdentityPicker.Operations.SearchRequest();
        request.Query = identitiesRequest.Query;
        request.QueryTypeHint = identitiesRequest.QueryTypeHint;
        request.IdentityTypes = identitiesRequest.IdentityTypes;
        request.FilterByAncestorEntityIds = identitiesRequest.FilterByAncestorEntityIds;
        request.FilterByEntityIds = identitiesRequest.FilterByEntityIds;
        request.OperationScopes = identitiesRequest.OperationScopes;
        request.ExtensionData = (IdentityPickerServiceExtensionData) identitiesRequest.Options;
        request.RequestedProperties = identitiesRequest.Properties;
        request.PagingToken = identitiesRequest.PagingToken;
        Microsoft.VisualStudio.Services.IdentityPicker.Operations.SearchResponse searchResponse = service.Search(tfsRequestContext, request);
        return this.Request.CreateResponse<IdentitiesSearchResponseModel>(new IdentitiesSearchResponseModel()
        {
          Results = searchResponse.Results
        });
      }
      catch (CircuitBreakerException ex)
      {
        return this.Request.CreateErrorResponse(HttpStatusCode.ServiceUnavailable, (Exception) ex);
      }
    }

    public virtual HttpResponseMessage GetAvatar(string objectId)
    {
      AbstractIdentityPickerIdentitiesController.CheckReadBasicPropertiesPermission(this.TfsRequestContext);
      IVssRequestContext vssRequestContext = this.TfsRequestContext.Elevate();
      Microsoft.VisualStudio.Services.IdentityPicker.Operations.GetAvatarResponse avatar = vssRequestContext.GetService<IdentityPickerService>().GetAvatar(vssRequestContext, new Microsoft.VisualStudio.Services.IdentityPicker.Operations.GetAvatarRequest()
      {
        ObjectId = objectId
      });
      HttpResponseMessage response;
      if (avatar != null && avatar.Avatar != null && avatar.Avatar.Image != null && avatar.Avatar.Image.Length != 0)
      {
        response = this.Request.CreateResponse(HttpStatusCode.OK);
        using (Stream stream = (Stream) new MemoryStream(avatar.Avatar.Image))
        {
          using (Image image = Image.FromStream(stream))
          {
            response.Content = (HttpContent) new VssServerByteArrayContent(avatar.Avatar.Image, (object) avatar.Avatar);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(image.GetMimeType());
          }
        }
      }
      else
        response = this.Request.CreateResponse(HttpStatusCode.NoContent);
      AbstractIdentityPickerIdentitiesController.AddCacheControlHeader(this.TfsRequestContext, response);
      return response;
    }

    public virtual HttpResponseMessage GetConnections(
      string objectId,
      IdentitiesGetConnectionsRequestModel getRequestParams)
    {
      Microsoft.VisualStudio.Services.IdentityPicker.Operations.GetConnectionsResponse connections = this.TfsRequestContext.GetService<IdentityPickerService>().GetConnections(this.TfsRequestContext, new Microsoft.VisualStudio.Services.IdentityPicker.Operations.GetConnectionsRequest()
      {
        IdentityTypes = getRequestParams.IdentityTypes,
        OperationScopes = getRequestParams.OperationScopes,
        ConnectionTypes = getRequestParams.ConnectionTypes,
        ObjectId = objectId,
        PagingToken = getRequestParams.PagingToken,
        Properties = new HashSet<string>((IEnumerable<string>) getRequestParams.Properties),
        Depth = getRequestParams.Depth
      });
      return this.Request.CreateResponse<IdentitiesGetConnectionsResponseModel>(new IdentitiesGetConnectionsResponseModel()
      {
        Connections = AbstractIdentityPickerIdentitiesController.GetJsonExtensibleDataFromDictionaryResult<IList<Identity>>(connections.Connections)
      });
    }

    public virtual HttpResponseMessage GetMru(
      string objectId,
      string featureId,
      IdentitiesGetMruRequestModel getRequestParams)
    {
      if (getRequestParams == null)
        throw new IdentityPickerArgumentException("Invalid get request parameters.");
      Microsoft.VisualStudio.Services.IdentityPicker.Operations.GetMruResponse mru = this.TfsRequestContext.GetService<IdentityPickerService>().GetMru(this.TfsRequestContext, new Microsoft.VisualStudio.Services.IdentityPicker.Operations.GetMruRequest()
      {
        RequestIdentityId = objectId,
        FeatureId = featureId,
        OperationScopes = getRequestParams.OperationScopes,
        Properties = getRequestParams.Properties,
        FilterByAncestorEntityIds = getRequestParams.FilterByAncestorEntityIds,
        FilterByEntityIds = getRequestParams.FilterByEntityIds,
        MaxItemsCount = getRequestParams.MaxItemsCount
      });
      return this.Request.CreateResponse<IdentitiesGetMruResponseModel>(new IdentitiesGetMruResponseModel()
      {
        MruIdentities = mru.MruIdentities
      });
    }

    public virtual HttpResponseMessage PatchMru(
      string objectId,
      string featureId,
      IList<IdentitiesPatchMruRequestModel> patchRequestBody)
    {
      if (patchRequestBody == null || patchRequestBody.Count == 0)
        throw new IdentityPickerArgumentException("Invalid patch request parameters.");
      if (patchRequestBody.Count > 1)
        return this.Request.CreateResponse(HttpStatusCode.Forbidden);
      bool flag = true;
      using (IEnumerator<IdentitiesPatchMruRequestModel> enumerator = patchRequestBody.GetEnumerator())
      {
        if (enumerator.MoveNext())
        {
          IdentitiesPatchMruRequestModel current = enumerator.Current;
          if (string.IsNullOrEmpty(current.Op) || current.Value == null || current.Value.Count == 0 || current.OperationScopes == null || current.OperationScopes.Count == 0)
            throw new IdentityPickerArgumentException("Invalid patch request parameters.");
          PatchOperationTypeEnum result;
          if (!System.Enum.TryParse<PatchOperationTypeEnum>(current.Op.Trim().ToLower(), true, out result))
            result = PatchOperationTypeEnum.None;
          if (!this.TfsRequestContext.GetService<IdentityPickerService>().PatchMru(this.TfsRequestContext, new Microsoft.VisualStudio.Services.IdentityPicker.Operations.PatchMruRequest()
          {
            RequestIdentityId = objectId,
            FeatureId = featureId,
            PatchOperationType = result,
            ObjectIds = current.Value,
            OperationScopes = current.OperationScopes
          }).Result)
            flag = false;
        }
      }
      return this.Request.CreateResponse<IdentitiesPatchMruResponseModel>(new IdentitiesPatchMruResponseModel()
      {
        Result = flag
      });
    }

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) AbstractIdentityPickerIdentitiesController.s_httpIdentityPickerServiceExceptions;

    private static void AddCacheControlHeader(
      IVssRequestContext requestContext,
      HttpResponseMessage response)
    {
      IdentityOperationHelper.ValidateRequestContext(requestContext);
      TimeSpan cacheControlMaxAge = AbstractIdentityPickerIdentitiesController.defaultClientCacheControlMaxAge;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      TimeSpan timeSpan = vssRequestContext.GetService<IVssRegistryService>().GetValue<TimeSpan>(vssRequestContext, (RegistryQuery) "/Configuration/Identity/IdentityPicker/ClientCacheControlMaxAge", cacheControlMaxAge);
      response.Headers.CacheControl = new CacheControlHeaderValue()
      {
        Private = true,
        Public = false,
        MaxStale = false,
        MaxAge = new TimeSpan?(timeSpan)
      };
    }

    private static void CheckReadBasicPropertiesPermission(IVssRequestContext context)
    {
      IVssSecurityNamespace securityNamespace = context.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(context, IdentityPickerSecurityConstants.NamespaceId);
      if (securityNamespace == null)
        return;
      securityNamespace.CheckPermission(context, IdentityPickerSecurityConstants.RootToken, 1);
    }

    private static IDictionary<string, object> GetJsonExtensibleDataFromDictionaryResult<T>(
      IDictionary<string, T> result)
    {
      return (IDictionary<string, object>) new Dictionary<string, object>((IDictionary<string, object>) result.Where<KeyValuePair<string, T>>((Func<KeyValuePair<string, T>, bool>) (kvp => !string.IsNullOrWhiteSpace(kvp.Key) && (object) kvp.Value != null)).ToDictionary<KeyValuePair<string, T>, string, object>((Func<KeyValuePair<string, T>, string>) (kvp => char.ToLowerInvariant(kvp.Key[0]).ToString() + kvp.Key.Substring(1)), (Func<KeyValuePair<string, T>, object>) (kvp => (object) kvp.Value)), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }
  }
}

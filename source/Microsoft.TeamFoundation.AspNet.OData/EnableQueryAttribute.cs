// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.EnableQueryAttribute
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Adapters;
using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.AspNet.OData.Interfaces;
using Microsoft.AspNet.OData.Query;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Microsoft.AspNet.OData
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
  public class EnableQueryAttribute : ActionFilterAttribute
  {
    private const char CommaSeparator = ',';
    private ODataValidationSettings _validationSettings;
    private string _allowedOrderByProperties;
    private ODataQuerySettings _querySettings;

    public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
    {
      HttpRequestMessage request = actionExecutedContext != null ? actionExecutedContext.Request : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (actionExecutedContext));
      if (request == null)
        throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (actionExecutedContext), SRResources.ActionExecutedContextMustHaveRequest);
      if (request.GetConfiguration() == null)
        throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (actionExecutedContext), SRResources.RequestMustContainConfiguration);
      if (actionExecutedContext.ActionContext == null)
        throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (actionExecutedContext), SRResources.ActionExecutedContextMustHaveActionContext);
      HttpActionDescriptor actionDescriptor = actionExecutedContext.ActionContext.ActionDescriptor;
      if (actionDescriptor == null)
        throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (actionExecutedContext), SRResources.ActionContextMustHaveDescriptor);
      HttpResponseMessage response = actionExecutedContext.Response;
      if (response == null || !response.IsSuccessStatusCode || response.Content == null)
        return;
      if (!(response.Content is ObjectContent content))
        throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (actionExecutedContext), SRResources.QueryingRequiresObjectContent, (object) response.Content.GetType().FullName);
      IQueryable singleResultCollection = (IQueryable) null;
      if (content.Value is SingleResult singleResult)
        singleResultCollection = ((IEnumerable<PropertyInfo>) content.Value.GetType().GetProperties()).OrderBy<PropertyInfo, int>((Func<PropertyInfo, int>) (p => ((IEnumerable<ParameterInfo>) p.GetIndexParameters()).Count<ParameterInfo>())).Where<PropertyInfo>((Func<PropertyInfo, bool>) (p => p.Name.Equals("Queryable"))).LastOrDefault<PropertyInfo>().GetValue((object) singleResult) as IQueryable;
      object obj = this.OnActionExecuted(content.Value, singleResultCollection, (IWebApiActionDescriptor) new WebApiActionDescriptor(actionDescriptor), (IWebApiRequestMessage) new WebApiRequestMessage(request), (Func<Type, IEdmModel>) (elementClrType => this.GetModel(elementClrType, request, actionDescriptor)), (Func<ODataQueryContext, ODataQueryOptions>) (queryContext => this.CreateAndValidateQueryOptions(request, queryContext)), (Action<HttpStatusCode>) (statusCode => actionExecutedContext.Response = request.CreateResponse(statusCode)), (Action<HttpStatusCode, string, Exception>) ((statusCode, message, exception) => actionExecutedContext.Response = request.CreateErrorResponse(statusCode, message, exception)));
      if (obj == null)
        return;
      content.Value = obj;
    }

    private ODataQueryOptions CreateAndValidateQueryOptions(
      HttpRequestMessage request,
      ODataQueryContext queryContext)
    {
      ODataQueryOptions queryOptions = new ODataQueryOptions(queryContext, request);
      this.ValidateQuery(request, queryOptions);
      return queryOptions;
    }

    public virtual void ValidateQuery(HttpRequestMessage request, ODataQueryOptions queryOptions)
    {
      if (request == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (request));
      if (queryOptions == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (queryOptions));
      foreach (KeyValuePair<string, string> queryNameValuePair in request.GetQueryNameValuePairs())
      {
        if (!queryOptions.IsSupportedQueryOption(queryNameValuePair.Key) && queryNameValuePair.Key.StartsWith("$", StringComparison.Ordinal))
          throw new HttpResponseException(request.CreateErrorResponse(HttpStatusCode.BadRequest, Microsoft.AspNet.OData.Common.Error.Format(SRResources.QueryParameterNotSupported, (object) queryNameValuePair.Key)));
      }
      queryOptions.Validate(this._validationSettings);
    }

    public virtual IEdmModel GetModel(
      Type elementClrType,
      HttpRequestMessage request,
      HttpActionDescriptor actionDescriptor)
    {
      IEdmModel edmModel = request.GetModel();
      if (edmModel == EdmCoreModel.Instance || edmModel.GetEdmType(elementClrType) == null)
        edmModel = actionDescriptor.GetEdmModel(elementClrType);
      return edmModel;
    }

    public EnableQueryAttribute()
    {
      this._validationSettings = new ODataValidationSettings();
      this._querySettings = new ODataQuerySettings();
    }

    public bool EnsureStableOrdering
    {
      get => this._querySettings.EnsureStableOrdering;
      set => this._querySettings.EnsureStableOrdering = value;
    }

    public HandleNullPropagationOption HandleNullPropagation
    {
      get => this._querySettings.HandleNullPropagation;
      set => this._querySettings.HandleNullPropagation = value;
    }

    public bool EnableConstantParameterization
    {
      get => this._querySettings.EnableConstantParameterization;
      set => this._querySettings.EnableConstantParameterization = value;
    }

    public bool EnableCorrelatedSubqueryBuffering
    {
      get => this._querySettings.EnableCorrelatedSubqueryBuffering;
      set => this._querySettings.EnableCorrelatedSubqueryBuffering = value;
    }

    public int MaxAnyAllExpressionDepth
    {
      get => this._validationSettings.MaxAnyAllExpressionDepth;
      set => this._validationSettings.MaxAnyAllExpressionDepth = value;
    }

    public int MaxNodeCount
    {
      get => this._validationSettings.MaxNodeCount;
      set => this._validationSettings.MaxNodeCount = value;
    }

    public int PageSize
    {
      get => this._querySettings.PageSize.GetValueOrDefault();
      set => this._querySettings.PageSize = new int?(value);
    }

    public bool HandleReferenceNavigationPropertyExpandFilter
    {
      get => this._querySettings.HandleReferenceNavigationPropertyExpandFilter;
      set => this._querySettings.HandleReferenceNavigationPropertyExpandFilter = value;
    }

    public AllowedQueryOptions AllowedQueryOptions
    {
      get => this._validationSettings.AllowedQueryOptions;
      set => this._validationSettings.AllowedQueryOptions = value;
    }

    public AllowedFunctions AllowedFunctions
    {
      get => this._validationSettings.AllowedFunctions;
      set => this._validationSettings.AllowedFunctions = value;
    }

    public AllowedArithmeticOperators AllowedArithmeticOperators
    {
      get => this._validationSettings.AllowedArithmeticOperators;
      set => this._validationSettings.AllowedArithmeticOperators = value;
    }

    public AllowedLogicalOperators AllowedLogicalOperators
    {
      get => this._validationSettings.AllowedLogicalOperators;
      set => this._validationSettings.AllowedLogicalOperators = value;
    }

    public string AllowedOrderByProperties
    {
      get => this._allowedOrderByProperties;
      set
      {
        this._allowedOrderByProperties = value;
        if (string.IsNullOrEmpty(value))
        {
          this._validationSettings.AllowedOrderByProperties.Clear();
        }
        else
        {
          string orderByProperties = this._allowedOrderByProperties;
          char[] chArray = new char[1]{ ',' };
          foreach (string str in orderByProperties.Split(chArray))
            this._validationSettings.AllowedOrderByProperties.Add(str.Trim());
        }
      }
    }

    public int MaxSkip
    {
      get => this._validationSettings.MaxSkip.GetValueOrDefault();
      set => this._validationSettings.MaxSkip = new int?(value);
    }

    public int MaxTop
    {
      get => this._validationSettings.MaxTop.GetValueOrDefault();
      set => this._validationSettings.MaxTop = new int?(value);
    }

    public int MaxExpansionDepth
    {
      get => this._validationSettings.MaxExpansionDepth;
      set => this._validationSettings.MaxExpansionDepth = value;
    }

    public int MaxOrderByNodeCount
    {
      get => this._validationSettings.MaxOrderByNodeCount;
      set => this._validationSettings.MaxOrderByNodeCount = value;
    }

    private object OnActionExecuted(
      object responseValue,
      IQueryable singleResultCollection,
      IWebApiActionDescriptor actionDescriptor,
      IWebApiRequestMessage request,
      Func<Type, IEdmModel> modelFunction,
      Func<ODataQueryContext, ODataQueryOptions> createQueryOptionFunction,
      Action<HttpStatusCode> createResponseAction,
      Action<HttpStatusCode, string, Exception> createErrorAction)
    {
      if (!this._querySettings.PageSize.HasValue && responseValue != null)
        this.GetModelBoundPageSize(responseValue, singleResultCollection, actionDescriptor, modelFunction, request.Context.Path, createErrorAction);
      int num = responseValue == null || !(request.RequestUri != (Uri) null) ? 0 : (!string.IsNullOrWhiteSpace(request.RequestUri.Query) || this._querySettings.PageSize.HasValue || this._querySettings.ModelBoundPageSize.HasValue || singleResultCollection != null || request.IsCountRequest() ? 1 : (EnableQueryAttribute.ContainsAutoSelectExpandProperty(responseValue, singleResultCollection, actionDescriptor, modelFunction, request.Context.Path) ? 1 : 0));
      object obj1 = (object) null;
      if (num != 0)
      {
        try
        {
          object obj2 = this.ExecuteQuery(responseValue, singleResultCollection, actionDescriptor, modelFunction, request, createQueryOptionFunction);
          if (obj2 == null && (request.Context.Path == null || singleResultCollection != null))
            createResponseAction(HttpStatusCode.NotFound);
          obj1 = obj2;
        }
        catch (ArgumentOutOfRangeException ex)
        {
          createErrorAction(HttpStatusCode.BadRequest, Microsoft.AspNet.OData.Common.Error.Format(SRResources.QueryParameterNotSupported, (object) ex.Message), (Exception) ex);
        }
        catch (NotImplementedException ex)
        {
          createErrorAction(HttpStatusCode.BadRequest, Microsoft.AspNet.OData.Common.Error.Format(SRResources.UriQueryStringInvalid, (object) ex.Message), (Exception) ex);
        }
        catch (NotSupportedException ex)
        {
          createErrorAction(HttpStatusCode.BadRequest, Microsoft.AspNet.OData.Common.Error.Format(SRResources.UriQueryStringInvalid, (object) ex.Message), (Exception) ex);
        }
        catch (InvalidOperationException ex)
        {
          createErrorAction(HttpStatusCode.BadRequest, Microsoft.AspNet.OData.Common.Error.Format(SRResources.UriQueryStringInvalid, (object) ex.Message), (Exception) ex);
        }
      }
      return obj1;
    }

    public virtual IQueryable ApplyQuery(IQueryable queryable, ODataQueryOptions queryOptions)
    {
      if (queryable == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (queryable));
      if (queryOptions == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (queryOptions));
      return queryOptions.ApplyTo(queryable, this._querySettings);
    }

    public virtual object ApplyQuery(object entity, ODataQueryOptions queryOptions)
    {
      if (entity == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (entity));
      if (queryOptions == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (queryOptions));
      return queryOptions.ApplyTo(entity, this._querySettings);
    }

    private static ODataQueryContext GetODataQueryContext(
      object responseValue,
      IQueryable singleResultCollection,
      IWebApiActionDescriptor actionDescriptor,
      Func<Type, IEdmModel> modelFunction,
      Microsoft.AspNet.OData.Routing.ODataPath path)
    {
      Type elementType = EnableQueryAttribute.GetElementType(responseValue, singleResultCollection, actionDescriptor);
      return new ODataQueryContext(modelFunction(elementType) ?? throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.QueryGetModelMustNotReturnNull), elementType, path);
    }

    private void GetModelBoundPageSize(
      object responseValue,
      IQueryable singleResultCollection,
      IWebApiActionDescriptor actionDescriptor,
      Func<Type, IEdmModel> modelFunction,
      Microsoft.AspNet.OData.Routing.ODataPath path,
      Action<HttpStatusCode, string, Exception> createErrorAction)
    {
      ODataQueryContext odataQueryContext;
      try
      {
        odataQueryContext = EnableQueryAttribute.GetODataQueryContext(responseValue, singleResultCollection, actionDescriptor, modelFunction, path);
      }
      catch (InvalidOperationException ex)
      {
        createErrorAction(HttpStatusCode.BadRequest, Microsoft.AspNet.OData.Common.Error.Format(SRResources.UriQueryStringInvalid, (object) ex.Message), (Exception) ex);
        return;
      }
      ModelBoundQuerySettings boundQuerySettings = EdmLibHelpers.GetModelBoundQuerySettings(odataQueryContext.TargetProperty, odataQueryContext.TargetStructuredType, odataQueryContext.Model);
      if (boundQuerySettings == null || !boundQuerySettings.PageSize.HasValue)
        return;
      this._querySettings.ModelBoundPageSize = boundQuerySettings.PageSize;
    }

    private object ExecuteQuery(
      object responseValue,
      IQueryable singleResultCollection,
      IWebApiActionDescriptor actionDescriptor,
      Func<Type, IEdmModel> modelFunction,
      IWebApiRequestMessage request,
      Func<ODataQueryContext, ODataQueryOptions> createQueryOptionFunction)
    {
      ODataQueryContext odataQueryContext = EnableQueryAttribute.GetODataQueryContext(responseValue, singleResultCollection, actionDescriptor, modelFunction, request.Context.Path);
      ODataQueryOptions queryOptions = createQueryOptionFunction(odataQueryContext);
      if (!(responseValue is IEnumerable source) || responseValue is string || responseValue is byte[])
      {
        EnableQueryAttribute.ValidateSelectExpandOnly(queryOptions);
        return singleResultCollection == null ? this.ApplyQuery(responseValue, queryOptions) : EnableQueryAttribute.SingleOrDefault(this.ApplyQuery(singleResultCollection, queryOptions), actionDescriptor);
      }
      if (!(source is IQueryable queryable1))
        queryable1 = source.AsQueryable();
      IQueryable queryable2 = this.ApplyQuery(queryable1, queryOptions);
      if (request.IsCountRequest())
      {
        long? totalCount = request.Context.TotalCount;
        if (totalCount.HasValue)
          return (object) totalCount.Value;
      }
      return (object) queryable2;
    }

    internal static Type GetElementType(
      object responseValue,
      IQueryable singleResultCollection,
      IWebApiActionDescriptor actionDescriptor)
    {
      if (!(responseValue is IEnumerable enumerable))
      {
        if (singleResultCollection == null)
          return responseValue.GetType();
        enumerable = (IEnumerable) singleResultCollection;
      }
      Type implementedIenumerableType = TypeHelper.GetImplementedIEnumerableType(enumerable.GetType());
      return !(implementedIenumerableType == (Type) null) ? implementedIenumerableType : throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.FailedToRetrieveTypeToBuildEdmModel, (object) typeof (EnableQueryAttribute).Name, (object) actionDescriptor.ActionName, (object) actionDescriptor.ControllerName, (object) responseValue.GetType().FullName);
    }

    internal static object SingleOrDefault(
      IQueryable queryable,
      IWebApiActionDescriptor actionDescriptor)
    {
      IEnumerator enumerator = queryable.GetEnumerator();
      try
      {
        object current = enumerator.MoveNext() ? enumerator.Current : (object) null;
        if (enumerator.MoveNext())
          throw new InvalidOperationException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.SingleResultHasMoreThanOneEntity, (object) actionDescriptor.ActionName, (object) actionDescriptor.ControllerName, (object) "SingleResult"));
        return current;
      }
      finally
      {
        if (enumerator is IDisposable disposable)
          disposable.Dispose();
      }
    }

    internal static void ValidateSelectExpandOnly(ODataQueryOptions queryOptions)
    {
      if (queryOptions.Filter != null || queryOptions.Count != null || queryOptions.OrderBy != null || queryOptions.Skip != null || queryOptions.Top != null)
        throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.NonSelectExpandOnSingleEntity));
    }

    private static bool ContainsAutoSelectExpandProperty(
      object responseValue,
      IQueryable singleResultCollection,
      IWebApiActionDescriptor actionDescriptor,
      Func<Type, IEdmModel> modelFunction,
      Microsoft.AspNet.OData.Routing.ODataPath path)
    {
      Type elementType = EnableQueryAttribute.GetElementType(responseValue, singleResultCollection, actionDescriptor);
      IEdmModel model = modelFunction(elementType);
      IEdmEntityType entityType1 = model != null ? model.GetEdmType(elementType) as IEdmEntityType : throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.QueryGetModelMustNotReturnNull);
      IEdmStructuredType structuredType = model.GetEdmType(elementType) as IEdmStructuredType;
      IEdmProperty property = (IEdmProperty) null;
      if (path != null)
        EdmLibHelpers.GetPropertyAndStructuredTypeFromPath((IEnumerable<ODataPathSegment>) path.Segments, out property, out structuredType, out string _);
      if (entityType1 != null)
      {
        List<IEdmEntityType> edmEntityTypeList = new List<IEdmEntityType>();
        edmEntityTypeList.Add(entityType1);
        edmEntityTypeList.AddRange(EdmLibHelpers.GetAllDerivedEntityTypes(entityType1, model));
        foreach (IEdmEntityType edmEntityType in edmEntityTypeList)
        {
          IEdmEntityType entityType = edmEntityType;
          IEnumerable<IEdmNavigationProperty> source = entityType == entityType1 ? entityType.NavigationProperties() : entityType.DeclaredNavigationProperties();
          if (source != null && source.Any<IEdmNavigationProperty>((Func<IEdmNavigationProperty, bool>) (navigationProperty => EdmLibHelpers.IsAutoExpand((IEdmProperty) navigationProperty, property, (IEdmStructuredType) entityType, model))))
            return true;
          IEnumerable<IEdmStructuralProperty> structuralProperties = entityType == entityType1 ? entityType.StructuralProperties() : entityType.DeclaredStructuralProperties();
          if (structuralProperties != null)
          {
            foreach (IEdmProperty property1 in structuralProperties)
            {
              if (EdmLibHelpers.IsAutoSelect(property1, property, (IEdmStructuredType) entityType, model))
                return true;
            }
          }
        }
      }
      else if (structuredType != null)
      {
        IEnumerable<IEdmStructuralProperty> structuralProperties = structuredType.StructuralProperties();
        if (structuralProperties != null)
        {
          foreach (IEdmProperty property2 in structuralProperties)
          {
            if (EdmLibHelpers.IsAutoSelect(property2, property, structuredType, model))
              return true;
          }
        }
      }
      return false;
    }
  }
}

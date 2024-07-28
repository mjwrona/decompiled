// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.PermissionsValidator
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.AspNet.OData.Query;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Microsoft.OData.UriParser.Aggregation;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.Model;
using Microsoft.VisualStudio.Services.Analytics.Model.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  public class PermissionsValidator
  {
    private const IEdmType IProjectScopedEDMType = null;
    private const string c_area = "Analytics";
    private const string c_layer = "PermissionValidator";
    private ODataQueryOptions _queryOptions;
    private Func<HashSet<Guid>> _accessibleProjectGuidsFunc;
    private Func<List<Project>> _allProjectsFunc;
    private ODataQuerySettings _odataSettings;
    private bool _inProjectScope;
    private static Dictionary<string, HashSet<string>> s_navigationToSkipValidation;
    private static readonly object s_navigationToSkipValidationLock = new object();
    private static Dictionary<string, HashSet<string>> s_visibleCrossProject;
    private static readonly object s_visibleCrossProjectLock = new object();
    private static IEdmModel s_maxODataModel;
    private static readonly object s_maxODataModelLock = new object();
    private IQueryable<PermissionsValidator.MockProjectScoped> _deniedProjects;

    public bool UserHasAccessToAllProjects { get; private set; }

    internal IList<Expression> CurrentProjectFilterExpressions { get; private set; }

    public PermissionsValidator(
      IVssRequestContext requestContext,
      ODataQueryOptions queryOptions,
      Func<HashSet<Guid>> accessibleProjectGuidsFunc,
      Func<List<Project>> allProjectsFunc,
      ODataQuerySettings odataSettings,
      bool inProject)
    {
      this._queryOptions = queryOptions;
      this._accessibleProjectGuidsFunc = accessibleProjectGuidsFunc;
      this._allProjectsFunc = allProjectsFunc;
      this._odataSettings = odataSettings;
      this._inProjectScope = inProject;
      PermissionsValidator.InitializeNavigationToSkipValidation(requestContext);
      PermissionsValidator.InitializeVisibleCrossProject(requestContext);
    }

    public void Validate(IVssRequestContext requestContext)
    {
      using (requestContext.TraceBlock(12012010, 12012011, "Analytics", "PermissionValidator", nameof (Validate)))
      {
        try
        {
          string name = this._queryOptions.Context.NavigationSource.Name;
          PermissionsValidator.QuerySecurityIndividualEntityValidator validator = new PermissionsValidator.QuerySecurityIndividualEntityValidator(this, this._queryOptions.Context.RequestContainer, name);
          string empty = string.Empty;
          if (!this._inProjectScope)
          {
            validator.CollectProjectScopeFilters(empty, this._queryOptions?.Filter?.FilterClause, this._queryOptions?.Apply?.ApplyClause);
            validator.ValidateProjectScopeFiltered(this._queryOptions.Context.ElementType, empty, (LevelsClause) null);
            this.CurrentProjectFilterExpressions = validator.CurrentProjectFilterExpressions;
          }
          if (this.UserHasAccessToAllProjects)
            return;
          this.CheckSelectExpandPermission(this._queryOptions.Context.Model, validator, this._queryOptions.SelectExpand?.SelectExpandClause, empty, name);
          foreach (string childPath in new ApplyPathCollector().Process(this._queryOptions?.Apply?.ApplyClause, PermissionsValidator.s_navigationToSkipValidation, PermissionsValidator.s_visibleCrossProject, name, this._inProjectScope))
          {
            validator.ValidateProjectScopeFiltered((IEdmType) null, childPath, (LevelsClause) null);
            if (this.UserHasAccessToAllProjects)
              break;
          }
        }
        catch (AnalyticsAccessCheckException ex) when (requestContext.IsTracing(12017006, TraceLevel.Info, "Analytics", "PermissionValidator"))
        {
          requestContext.Trace(12017006, TraceLevel.Info, "Analytics", "PermissionValidator", "Permission Validation threw AnalyticsAccessCheckException.");
          string str1 = string.Join<Guid>(",", this._allProjectsFunc().Select<Project, Guid>((Func<Project, Guid>) (p => p.ProjectId)));
          string str2 = string.Join<Guid>(",", (IEnumerable<Guid>) this._accessibleProjectGuidsFunc());
          string str3 = string.Join<Guid?>(",", (IEnumerable<Guid?>) this.DeniedProjects.Select<PermissionsValidator.MockProjectScoped, Guid?>((Expression<Func<PermissionsValidator.MockProjectScoped, Guid?>>) (p => p.ProjectSK)));
          requestContext.TraceLongTextAlways(12017006, TraceLevel.Info, "Analytics", "PermissionValidator", "All projects: " + str1);
          requestContext.TraceLongTextAlways(12017006, TraceLevel.Info, "Analytics", "PermissionValidator", "Accessible projects: " + str2);
          requestContext.TraceLongTextAlways(12017006, TraceLevel.Info, "Analytics", "PermissionValidator", "Denied projects: " + str3);
          throw ex;
        }
        catch (Exception ex) when (this.TraceException(requestContext, ex))
        {
          throw;
        }
      }
    }

    private static IEdmModel GetMaxModel()
    {
      if (PermissionsValidator.s_maxODataModel == null)
      {
        lock (PermissionsValidator.s_maxODataModelLock)
        {
          if (PermissionsValidator.s_maxODataModel == null)
            PermissionsValidator.s_maxODataModel = new AnalyticsModelBuilder().GetEdmModel(new ModelCreationProcessFields(new List<ProcessFieldInfo>()), odataModelVersion: ModelVersionExtension.GetMaxODataModelVersion());
        }
      }
      return PermissionsValidator.s_maxODataModel;
    }

    private static void InitializeNavigationToSkipValidation(IVssRequestContext requestContext)
    {
      if (PermissionsValidator.s_navigationToSkipValidation != null)
        return;
      lock (PermissionsValidator.s_navigationToSkipValidationLock)
      {
        if (PermissionsValidator.s_navigationToSkipValidation != null)
          return;
        using (requestContext.TraceBlock(12012008, 12012009, "Analytics", "PermissionValidator", nameof (InitializeNavigationToSkipValidation)))
        {
          PermissionsValidator.s_navigationToSkipValidation = new Dictionary<string, HashSet<string>>();
          foreach (var data in PermissionsValidator.GetMaxModel().EntityContainer.Elements.OfType<EdmEntitySet>().Select(e => new
          {
            ClrType = EdmTypeUtils.GetType(e.Type),
            EdmType = e
          }).Where(t => typeof (IProjectScoped).IsAssignableFrom(t.ClrType)))
          {
            HashSet<string> source = new HashSet<string>();
            foreach (PropertyInfo propertyInfo in ((IEnumerable<PropertyInfo>) data.ClrType.GetProperties(BindingFlags.Instance | BindingFlags.Public)).Where<PropertyInfo>((Func<PropertyInfo, bool>) (p => p.GetCustomAttribute<NavigationRequiresPermissionCheckAttribute>() == null)))
              source.Add(propertyInfo.Name);
            if (source.Any<string>())
              PermissionsValidator.s_navigationToSkipValidation[data.EdmType.Name] = source;
          }
        }
      }
    }

    private static void InitializeVisibleCrossProject(IVssRequestContext requestContext)
    {
      if (PermissionsValidator.s_visibleCrossProject != null)
        return;
      lock (PermissionsValidator.s_visibleCrossProjectLock)
      {
        if (PermissionsValidator.s_visibleCrossProject != null)
          return;
        using (requestContext.TraceBlock(12012014, 12012015, "Analytics", "PermissionValidator", nameof (InitializeVisibleCrossProject)))
        {
          PermissionsValidator.s_visibleCrossProject = new Dictionary<string, HashSet<string>>((IEqualityComparer<string>) StringComparer.Ordinal);
          foreach (var data in PermissionsValidator.GetMaxModel().EntityContainer.Elements.OfType<EdmEntitySet>().Select(e => new
          {
            ClrType = EdmTypeUtils.GetType(e.Type),
            EdmType = e.EntityType()
          }).Where(t => typeof (IProjectScoped).IsAssignableFrom(t.ClrType)).Where(t => t.ClrType.GetCustomAttributes<DisableProjectFilteringAttribute>().Any<DisableProjectFilteringAttribute>()))
          {
            HashSet<string> source = new HashSet<string>((IEqualityComparer<string>) StringComparer.Ordinal);
            foreach (PropertyInfo propertyInfo in ((IEnumerable<PropertyInfo>) data.ClrType.GetProperties(BindingFlags.Instance | BindingFlags.Public)).Where<PropertyInfo>((Func<PropertyInfo, bool>) (p => p.GetCustomAttribute<VisibleCrossProjectAttribute>() != null)))
              source.Add(propertyInfo.Name);
            if (source.Any<string>())
              PermissionsValidator.s_visibleCrossProject[data.EdmType.Name] = source;
          }
        }
      }
    }

    private bool TraceException(IVssRequestContext requestContext, Exception ex)
    {
      switch (ex)
      {
        case ModelNotReadyException _:
        case AnalyticsAccessCheckException _:
label_2:
          return false;
        default:
          requestContext.Trace(12012007, TraceLevel.Error, "Analytics", "PermissionValidator", ex.ToString());
          goto label_2;
      }
    }

    private void CheckSelectExpandPermission(
      IEdmModel model,
      PermissionsValidator.QuerySecurityIndividualEntityValidator validator,
      SelectExpandClause selectExpandClause,
      string childPath,
      string previousEntitySetName)
    {
      if (selectExpandClause == null)
        return;
      HashSet<string> stringSet = (HashSet<string>) null;
      PermissionsValidator.s_navigationToSkipValidation.TryGetValue(previousEntitySetName, out stringSet);
      foreach (ExpandedNavigationSelectItem navigationSelectItem in selectExpandClause.SelectedItems.OfType<ExpandedNavigationSelectItem>())
      {
        ODataPathSegment odataPathSegment = navigationSelectItem.PathToNavigationProperty.Single<ODataPathSegment>();
        string identifier = odataPathSegment.Identifier;
        string childPath1 = childPath + "/" + identifier;
        HashSet<string> visibleCrossProject = (HashSet<string>) null;
        PermissionsValidator.s_visibleCrossProject.TryGetValue(identifier, out visibleCrossProject);
        if (!this._inProjectScope || visibleCrossProject != null)
        {
          if (this._odataSettings.HandleReferenceNavigationPropertyExpandFilter || odataPathSegment.EdmType.TypeKind == EdmTypeKind.Collection)
            validator.CollectProjectScopeFilters(childPath1, navigationSelectItem.FilterOption, navigationSelectItem.ApplyOption);
          if (stringSet == null || !stringSet.Contains(identifier))
          {
            if (visibleCrossProject != null && navigationSelectItem.SelectAndExpand != null && navigationSelectItem.SelectAndExpand.SelectedItems.Any<SelectItem>() && navigationSelectItem.SelectAndExpand.SelectedItems.OfType<PathSelectItem>().All<PathSelectItem>((Func<PathSelectItem, bool>) (i => visibleCrossProject.Contains(i.SelectedPath.LastSegment.Identifier))))
              break;
            validator.ValidateProjectScopeFiltered(navigationSelectItem.NavigationSource.Type, childPath1, navigationSelectItem.LevelsOption);
            if (this.UserHasAccessToAllProjects)
              break;
          }
        }
        this.CheckSelectExpandPermission(model, validator, navigationSelectItem.SelectAndExpand, childPath1, navigationSelectItem.NavigationSource.Name);
      }
    }

    private IQueryable<PermissionsValidator.MockProjectScoped> DeniedProjects
    {
      get
      {
        if (this._deniedProjects == null)
        {
          List<Project> source = this._allProjectsFunc();
          if (source == null || source.Count == 0)
            throw new ModelNotReadyException(AnalyticsResources.MODEL_NOT_READY());
          HashSet<Guid> accessibleProjectGuids = this._accessibleProjectGuidsFunc();
          this._deniedProjects = source.Where<Project>((Func<Project, bool>) (p => !accessibleProjectGuids.Contains(p.ProjectSK))).Select<Project, PermissionsValidator.MockProjectScoped>((Func<Project, PermissionsValidator.MockProjectScoped>) (p => new PermissionsValidator.MockProjectScoped()
          {
            ProjectName = p.ProjectName,
            ProjectSK = new Guid?(p.ProjectSK),
            ProjectId = new Guid?(p.ProjectId)
          })).AsQueryable<PermissionsValidator.MockProjectScoped>();
        }
        return this._deniedProjects;
      }
    }

    internal class QuerySecurityIndividualEntityValidator
    {
      private IServiceProvider _requestContainer;
      private QueryNodeFilterCollector _queryNodeFilterCollector = new QueryNodeFilterCollector();
      private PermissionsValidator _mainValidator;
      private string _rootEntityName;

      internal IList<Expression> CurrentProjectFilterExpressions { get; private set; } = (IList<Expression>) new List<Expression>();

      public QuerySecurityIndividualEntityValidator(
        PermissionsValidator mainValidator,
        IServiceProvider requestContainer,
        string rootEntityName)
      {
        this._mainValidator = mainValidator;
        this._requestContainer = requestContainer;
        this._rootEntityName = rootEntityName;
      }

      internal void CollectProjectScopeFilters(
        string childPath,
        FilterClause filterClause,
        ApplyClause applyClause)
      {
        this._queryNodeFilterCollector.Process(childPath, filterClause, applyClause);
      }

      internal void ValidateProjectScopeFiltered(
        IEdmType type,
        string childPath,
        LevelsClause levelsClause)
      {
        if (type != null && !this.IsProjectScopedType(type))
          return;
        if (levelsClause != null)
        {
          if (this._mainValidator.DeniedProjects.Any<PermissionsValidator.MockProjectScoped>())
          {
            PermissionsValidator.QuerySecurityIndividualEntityValidator.ThrowLevelsPermissionException();
          }
          else
          {
            this._mainValidator.UserHasAccessToAllProjects = true;
            return;
          }
        }
        List<SingleValueNode> source1;
        if (!this._queryNodeFilterCollector.PathToExpression.TryGetValue(childPath, out source1))
        {
          if (this._mainValidator.DeniedProjects.Any<PermissionsValidator.MockProjectScoped>())
          {
            this.ThrowPermissionException(childPath);
          }
          else
          {
            this._mainValidator.UserHasAccessToAllProjects = true;
            return;
          }
        }
        IQueryable<PermissionsValidator.MockProjectScoped> source2 = this._mainValidator.DeniedProjects;
        Microsoft.AspNet.OData.Query.Expressions.FilterBinder filterBinder = new Microsoft.AspNet.OData.Query.Expressions.FilterBinder(this._requestContainer);
        IEnumerable<LambdaExpression> source3 = source1.Select<SingleValueNode, LambdaExpression>((Func<SingleValueNode, LambdaExpression>) (filter => filterBinder.BindExpression(filter, (RangeVariable) this._queryNodeFilterCollector.ResourceRangeVariableReferenceNode.RangeVariable, typeof (PermissionsValidator.MockProjectScoped))));
        this.CurrentProjectFilterExpressions = (IList<Expression>) source3.Select<LambdaExpression, Expression>((Func<LambdaExpression, Expression>) (expression => expression.Body)).ToList<Expression>();
        if (!this._mainValidator.DeniedProjects.Any<PermissionsValidator.MockProjectScoped>())
        {
          this._mainValidator.UserHasAccessToAllProjects = true;
        }
        else
        {
          foreach (LambdaExpression lambdaExpression in source3)
            source2 = ExpressionHelpers.QueryableWhereGeneric.MakeGenericMethod(typeof (PermissionsValidator.MockProjectScoped)).Invoke((object) null, new object[2]
            {
              (object) source2,
              (object) lambdaExpression
            }) as IQueryable<PermissionsValidator.MockProjectScoped>;
          if (!source2.Any<PermissionsValidator.MockProjectScoped>())
            return;
          this.ThrowPermissionException(childPath);
        }
      }

      private void ThrowPermissionException(string childPath) => throw new AnalyticsAccessCheckException(AnalyticsResources.QUERY_TOO_WIDE_EntityName((object) (this._rootEntityName + childPath)), (Exception) null);

      private static void ThrowLevelsPermissionException() => throw new AnalyticsAccessCheckException(AnalyticsResources.QUERY_LEVELS_NOT_ALLOWED() + " " + AnalyticsResources.QUERY_TOO_WIDE(), (Exception) null);

      private bool IsProjectScopedType(IEdmType type)
      {
        Type type1 = EdmTypeUtils.GetType(type);
        return type1 != (Type) null && typeof (IProjectScoped).IsAssignableFrom(type1);
      }
    }

    internal class MockProjectScoped : IProjectScopeByName, IProjectScoped
    {
      public Guid? ProjectSK { get; set; }

      public Guid? ProjectId { get; set; }

      public string ProjectName { get; set; }
    }
  }
}

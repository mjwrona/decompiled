// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.AnalyticsViewsService
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Analytics
{
  public class AnalyticsViewsService : IAnalyticsViewsService, IVssFrameworkService
  {
    private const string c_defaultViewsCreatedForViewScopeFormat = "/Configuration/Views/DefaultView/{0}/{1}";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public IEnumerable<AnalyticsView> GetViews(
      IVssRequestContext requestContext,
      AnalyticsViewScope viewScope)
    {
      ArgumentUtility.CheckForEmptyGuid(viewScope.Id, "Id");
      IList<AnalyticsView> views1 = (IList<AnalyticsView>) new List<AnalyticsView>();
      using (AnalyticsViewsSqlComponent component = requestContext.CreateComponent<AnalyticsViewsSqlComponent>())
        views1.AddRange<AnalyticsView, IList<AnalyticsView>>((IEnumerable<AnalyticsView>) component.GetViews(viewScope.Id));
      if (views1.Count == 0 && !this.IsDefaultViewsCreatedForScope(requestContext, viewScope))
      {
        List<AnalyticsView> viewsForViewScope = this.CreateDefaultViewsForViewScope(requestContext, viewScope);
        using (AnalyticsViewsSqlComponent component = requestContext.CreateComponent<AnalyticsViewsSqlComponent>())
          views1.AddRange<AnalyticsView, IList<AnalyticsView>>((IEnumerable<AnalyticsView>) component.CreateViews(viewScope.Id, viewsForViewScope));
        AnalyticsPermission.SetProjectDefaultPermissionsForViews(requestContext, viewScope.Id);
      }
      List<AnalyticsView> views2 = new List<AnalyticsView>();
      this.EnsureDefaultViewDefinitions(requestContext, viewScope, ref views1);
      foreach (AnalyticsView view in (IEnumerable<AnalyticsView>) views1)
      {
        if (this.GetAnalyticsAccessService(requestContext).HasReadViewPermission(requestContext, view.Visibility, viewScope.Id, view.Id))
        {
          view.Definition = (string) null;
          view.Query = (AnalyticsViewQuery) null;
          view.SetLastUpdatedBy(requestContext);
          view.AdjustToAccountTime(requestContext);
          views2.Add(view);
        }
      }
      this.AddLinksToViews(requestContext, viewScope, views2.ToArray());
      return (IEnumerable<AnalyticsView>) views2;
    }

    public AnalyticsView GetView(
      IVssRequestContext requestContext,
      AnalyticsViewScope viewScope,
      Guid viewId,
      AnalyticsViewExpandFlags expandOption)
    {
      ArgumentUtility.CheckForEmptyGuid(viewId, nameof (viewId));
      ArgumentUtility.CheckForEmptyGuid(viewScope.Id, "Id");
      AnalyticsView view;
      using (AnalyticsViewsSqlComponent component = requestContext.CreateComponent<AnalyticsViewsSqlComponent>())
        view = component.GetView(viewScope.Id, viewId);
      this.ThrowIfViewNotExists(view, viewId);
      this.GetAnalyticsAccessService(requestContext).CheckReadViewPermission(requestContext, view.Visibility, viewScope.Id, viewId);
      this.EnsureDefaultViewDefinition(requestContext, viewScope, ref view);
      if (!expandOption.HasFlag((Enum) AnalyticsViewExpandFlags.Query))
        view.Query = (AnalyticsViewQuery) null;
      if (view.Query != null)
        view.Query.ExpandODataProperties(requestContext, view, viewScope, false);
      if (!expandOption.HasFlag((Enum) AnalyticsViewExpandFlags.Definition))
        view.Definition = (string) null;
      view.AdjustToAccountTime(requestContext);
      this.AddLinksToViews(requestContext, viewScope, view);
      view.SetLastUpdatedBy(requestContext);
      return view;
    }

    public AnalyticsView CreateView(
      IVssRequestContext requestContext,
      AnalyticsViewScope viewScope,
      AnalyticsViewCreateParameters viewCreateParameters,
      bool preview)
    {
      ArgumentUtility.CheckForEmptyGuid(viewScope.Id, "Id");
      ArgumentUtility.CheckForNull<AnalyticsViewCreateParameters>(viewCreateParameters, nameof (viewCreateParameters));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(viewCreateParameters.Name, "Name");
      ArgumentUtility.CheckStringForNullOrWhiteSpace(viewCreateParameters.Definition, "Definition");
      this.checkViewType(viewCreateParameters.ViewType, "ViewType");
      this.checkViewVisibility(viewCreateParameters.Visibility, "Visibility");
      this.GetAnalyticsAccessService(requestContext).CheckCreateViewPermission(requestContext, viewCreateParameters.Visibility, viewScope.Id);
      AnalyticsView view = new AnalyticsView(viewCreateParameters);
      if (string.IsNullOrEmpty(view.Definition))
        throw new AnalyticsViewCreationFailedException();
      this.ComposeQueryTemplateFromView(requestContext, view, viewScope);
      if (preview)
      {
        view.Id = Guid.Empty;
        if (view.Query != null)
          view.Query.ExpandODataProperties(requestContext, view, viewScope, true);
      }
      else
      {
        view.CreatedBy = requestContext.GetUserIdentity().ToIdentityRef(requestContext);
        this.GetAnalyticsAccessService(requestContext).SetViewOwnerPermission(requestContext, viewCreateParameters.Visibility, viewScope.Id, view.Id);
        using (AnalyticsViewsSqlComponent component = requestContext.CreateComponent<AnalyticsViewsSqlComponent>())
          view = component.CreateView(viewScope.Id, view);
        this.AddLinksToViews(requestContext, viewScope, view);
      }
      return view;
    }

    public AnalyticsView ReplaceView(
      IVssRequestContext requestContext,
      AnalyticsViewScope viewScope,
      Guid viewId,
      AnalyticsViewReplaceParameters viewReplaceParameters)
    {
      ArgumentUtility.CheckForEmptyGuid(viewId, nameof (viewId));
      ArgumentUtility.CheckForEmptyGuid(viewScope.Id, "Id");
      ArgumentUtility.CheckForNull<AnalyticsViewReplaceParameters>(viewReplaceParameters, nameof (viewReplaceParameters));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(viewReplaceParameters.Name, "Name");
      ArgumentUtility.CheckStringForNullOrWhiteSpace(viewReplaceParameters.Definition, "Definition");
      ArgumentUtility.CheckForDefinedEnum<AnalyticsViewVisibility>(viewReplaceParameters.Visibility, "Visibility");
      this.checkViewVisibility(viewReplaceParameters.Visibility, "Visibility");
      AnalyticsView view;
      using (AnalyticsViewsSqlComponent component = requestContext.CreateComponent<AnalyticsViewsSqlComponent>())
        view = component.GetView(viewScope.Id, viewId);
      this.ThrowIfViewNotExists(view, viewId);
      this.GetAnalyticsAccessService(requestContext).CheckEditViewPermission(requestContext, view.Visibility, viewScope.Id, view.Id);
      if (view.Visibility == AnalyticsViewVisibility.Private && viewReplaceParameters.Visibility == AnalyticsViewVisibility.Shared)
        this.GetAnalyticsAccessService(requestContext).CheckEditViewPermission(requestContext, viewReplaceParameters.Visibility, viewScope.Id, view.Id);
      view.LastModifiedBy = requestContext.GetUserIdentity().ToIdentityRef(requestContext);
      view.Definition = viewReplaceParameters.Definition;
      view.Description = viewReplaceParameters.Description;
      view.Name = viewReplaceParameters.Name;
      AnalyticsViewVisibility visibility = view.Visibility;
      view.Visibility = viewReplaceParameters.Visibility;
      this.ComposeQueryTemplateFromView(requestContext, view, viewScope);
      using (AnalyticsViewsSqlComponent component = requestContext.CreateComponent<AnalyticsViewsSqlComponent>())
        view = component.UpdateView(viewScope.Id, view);
      if (viewReplaceParameters.Visibility != visibility)
      {
        this.GetAnalyticsAccessService(requestContext).SetViewOwnerPermission(requestContext, viewReplaceParameters.Visibility, viewScope.Id, view.Id);
        this.GetAnalyticsAccessService(requestContext).RemovePermissionsForVisibility(requestContext, visibility, viewScope.Id, new Guid?(view.Id));
      }
      this.AddLinksToViews(requestContext, viewScope, view);
      return view;
    }

    private void checkViewType(AnalyticsViewType viewType, string fieldName)
    {
      ArgumentUtility.CheckForDefinedEnum<AnalyticsViewType>(viewType, fieldName);
      if (viewType == AnalyticsViewType.Undefined)
        throw new AnalyticsViewsInvalidViewTypeException(AnalyticsResources.ExceptionUndefinedValueNotAllowed((object) fieldName));
    }

    private void checkViewVisibility(AnalyticsViewVisibility viewVisibility, string fieldName)
    {
      ArgumentUtility.CheckForDefinedEnum<AnalyticsViewVisibility>(viewVisibility, fieldName);
      if (viewVisibility == AnalyticsViewVisibility.Undefined)
        throw new AnalyticsViewsInvalidViewVisibilityException(AnalyticsResources.ExceptionUndefinedValueNotAllowed((object) fieldName));
    }

    public void DeleteView(
      IVssRequestContext requestContext,
      AnalyticsViewScope viewScope,
      Guid viewId)
    {
      ArgumentUtility.CheckForEmptyGuid(viewScope.Id, "Id");
      AnalyticsView view;
      using (AnalyticsViewsSqlComponent component = requestContext.CreateComponent<AnalyticsViewsSqlComponent>())
        view = component.GetView(viewScope.Id, viewId);
      this.ThrowIfViewNotExists(view, viewId);
      this.GetAnalyticsAccessService(requestContext).CheckDeleteViewPermission(requestContext, view.Visibility, viewScope.Id, viewId);
      using (AnalyticsViewsSqlComponent component = requestContext.CreateComponent<AnalyticsViewsSqlComponent>())
        component.DeleteView(viewScope.Id, view);
      List<AnalyticsView> analyticsViewList = new List<AnalyticsView>();
      using (AnalyticsViewsSqlComponent component = requestContext.CreateComponent<AnalyticsViewsSqlComponent>())
        analyticsViewList.AddRange((IEnumerable<AnalyticsView>) component.GetViews(viewScope.Id));
      if (analyticsViewList.Count != 0)
        return;
      this.WriteDefaultViewsCreatedForScope(requestContext, viewScope);
    }

    private void EnsureDefaultViewDefinitions(
      IVssRequestContext requestContext,
      AnalyticsViewScope viewScope,
      ref IList<AnalyticsView> views)
    {
      List<AnalyticsView> list1 = views.Where<AnalyticsView>((Func<AnalyticsView, bool>) (view => view.CreatedBy == null)).Where<AnalyticsView>((Func<AnalyticsView, bool>) (view => view.Definition == null)).ToList<AnalyticsView>();
      WorkItemsDefinitions.GenerateDefinitionStringForDefaultViews(requestContext, viewScope, ref list1);
      List<AnalyticsView> list2 = list1.Where<AnalyticsView>((Func<AnalyticsView, bool>) (view => view.Definition != null)).ToList<AnalyticsView>();
      if (list2.Count <= 0)
        return;
      foreach (AnalyticsView analyticsView in list2)
      {
        AnalyticsView view;
        using (AnalyticsViewsSqlComponent component = requestContext.CreateComponent<AnalyticsViewsSqlComponent>())
          view = component.GetView(viewScope.Id, analyticsView.Id);
        analyticsView.Query = view.Query;
      }
      using (AnalyticsViewsSqlComponent component = requestContext.CreateComponent<AnalyticsViewsSqlComponent>())
        component.UpdateViews(viewScope.Id, list2);
    }

    private void EnsureDefaultViewDefinition(
      IVssRequestContext requestContext,
      AnalyticsViewScope viewScope,
      ref AnalyticsView view)
    {
      if (view.CreatedBy != null || view.Definition != null)
        return;
      WorkItemsDefinitions.GenerateDefinitionStringForDefaultView(requestContext, viewScope, ref view);
      if (view.Definition == null)
        return;
      using (AnalyticsViewsSqlComponent component = requestContext.CreateComponent<AnalyticsViewsSqlComponent>())
        component.UpdateView(viewScope.Id, view);
    }

    private List<AnalyticsView> CreateDefaultViewsForViewScope(
      IVssRequestContext requestContext,
      AnalyticsViewScope viewScope)
    {
      FactoryOptions factoryOptions1 = new FactoryOptions();
      factoryOptions1.ViewType = AnalyticsViewType.WorkItems;
      factoryOptions1.ViewScope = viewScope;
      FactoryOptions factoryOptions = factoryOptions1;
      List<AnalyticsView> defaultViews = new List<AnalyticsView>();
      new DefaultViewsFactory().GetFactories(requestContext).ForEach((Action<IFactory>) (v => defaultViews.AddRange((IEnumerable<AnalyticsView>) v.GetViews(requestContext, factoryOptions).ToList<AnalyticsView>())));
      return defaultViews;
    }

    private void ComposeQueryTemplateFromView(
      IVssRequestContext requestContext,
      AnalyticsView view,
      AnalyticsViewScope viewScope)
    {
      if (view.Query == null)
        view.Query = new AnalyticsViewQuery()
        {
          Id = Guid.NewGuid()
        };
      AnalyticsViewWorkItemsDefinition definition = JsonConvert.DeserializeObject<AnalyticsViewWorkItemsDefinition>(view.Definition);
      AnalyticsViewQueryFragment query = WorkItemsDefinitionToQueryTemplate.GenerateQuery(requestContext, definition, viewScope);
      view.Query.EntitySet = query.EntitySet;
      view.Query.ODataTemplate = query.ODataTemplate;
    }

    private void ThrowIfViewNotExists(AnalyticsView view, Guid viewId)
    {
      if (view == null)
        throw new AnalyticsViewDoesNotExistException(viewId);
    }

    private void AddLinksToViews(
      IVssRequestContext requestContext,
      AnalyticsViewScope viewScope,
      params AnalyticsView[] views)
    {
      foreach (AnalyticsView view in views)
        view.AddLinks(requestContext, viewScope);
    }

    private bool IsDefaultViewsCreatedForScope(
      IVssRequestContext requestContext,
      AnalyticsViewScope viewScope)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      RegistryQuery registryQuery = new RegistryQuery(this.GetRegistryPathForDefaultViewEntryOnScope(viewScope));
      IVssRequestContext requestContext1 = requestContext;
      ref RegistryQuery local = ref registryQuery;
      return service.GetValue<bool>(requestContext1, in local);
    }

    private void WriteDefaultViewsCreatedForScope(
      IVssRequestContext requestContext,
      AnalyticsViewScope viewScope)
    {
      requestContext.GetService<IVssRegistryService>().SetValue<bool>(requestContext, this.GetRegistryPathForDefaultViewEntryOnScope(viewScope), true);
    }

    private string GetRegistryPathForDefaultViewEntryOnScope(AnalyticsViewScope viewScope) => string.Format("/Configuration/Views/DefaultView/{0}/{1}", (object) Enum.GetName(typeof (AnalyticsViewScopeType), (object) viewScope.Type), (object) viewScope.Id.ToString());

    private IAnalyticsViewsAccessService GetAnalyticsAccessService(IVssRequestContext requestContext) => requestContext.GetService<IAnalyticsViewsAccessService>();
  }
}

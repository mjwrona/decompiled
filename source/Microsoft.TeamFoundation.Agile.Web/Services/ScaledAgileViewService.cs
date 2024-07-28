// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Services.ScaledAgileViewService
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.TeamFoundation.Agile.Server;
using Microsoft.TeamFoundation.Agile.Web.Data;
using Microsoft.TeamFoundation.Agile.Web.Utilities;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.TeamFoundation.Work.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.Agile.Web.Services
{
  public class ScaledAgileViewService : IScaledAgileViewService, IVssFrameworkService
  {
    private const string TraceArea = "Plans";
    private const string TraceLayer = "PlansService";

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new InvalidOperationException(FrameworkResources.UnexpectedHostType((object) requestContext.ServiceHost.HostType.ToString()));
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public Plan GetView(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid viewId,
      bool includeCardSettings = true)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForEmptyGuid(viewId, nameof (viewId));
      return requestContext.TraceBlock<Plan>(290731, 290732, 290733, "Plans", "PlansService", nameof (GetView), (Func<Plan>) (() =>
      {
        PlanPermissionsBitFlags planPermissions;
        using (PerformanceTimer performanceTimer = PerformanceTimer.StartMeasure(requestContext, "GetPlanPermissions"))
        {
          performanceTimer.AddProperty("NumberRequested", (object) 1);
          planPermissions = this.GetPlanPermissions(requestContext, projectId, viewId);
        }
        if (!planPermissions.HasFlag((Enum) PlanPermissionsBitFlags.View))
          return (Plan) null;
        ScaledAgileView view;
        using (PerformanceTimer.StartMeasure(requestContext, nameof (GetView)))
        {
          using (ScaledAgileViewComponent component = ScaledAgileViewComponent.CreateComponent(requestContext))
            view = component.GetView(projectId, viewId, includeCardSettings);
        }
        if (view == null)
          return (Plan) null;
        IdentityRef createdByIdentity;
        IdentityRef modifiedByIdentity;
        this.GetIdentitiesForScaledAgileView(requestContext, view, out createdByIdentity, out modifiedByIdentity);
        return view.ToPlan(requestContext, projectId, includeCardSettings, planPermissions, createdByIdentity, modifiedByIdentity);
      }));
    }

    public IEnumerable<Plan> GetViewDefinitions(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid ownerId = default (Guid))
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      return (IEnumerable<Plan>) requestContext.TraceBlock<List<Plan>>(290734, 290735, 290736, "Plans", "PlansService", nameof (GetViewDefinitions), (Func<List<Plan>>) (() =>
      {
        List<ShallowScaledAgileViewRecord> list;
        using (PerformanceTimer performanceTimer = PerformanceTimer.StartMeasure(requestContext, nameof (GetViewDefinitions)))
        {
          using (ScaledAgileViewComponent component = ScaledAgileViewComponent.CreateComponent(requestContext))
            list = component.GetViewDefinitions(projectId, ownerId).ToList<ShallowScaledAgileViewRecord>();
          performanceTimer.AddProperty("ViewDefinitionCount", (object) list.Count);
        }
        return this.ConvertDataRecordsToPlans(requestContext, (IReadOnlyList<ShallowScaledAgileViewRecord>) list, projectId);
      }));
    }

    public IList<Plan> GetViewDefinitions(
      IVssRequestContext requestContext,
      List<Tuple<Guid, Guid>> planIdsWithProjectId)
    {
      return (IList<Plan>) requestContext.TraceBlock<List<Plan>>(290753, 290754, 290755, "Plans", "PlansService", nameof (GetViewDefinitions), (Func<List<Plan>>) (() =>
      {
        IReadOnlyList<ShallowScaledAgileViewRecord> viewDefinitions;
        using (PerformanceTimer performanceTimer = PerformanceTimer.StartMeasure(requestContext, nameof (GetViewDefinitions)))
        {
          using (ScaledAgileViewComponent component = ScaledAgileViewComponent.CreateComponent(requestContext))
          {
            List<Guid> list = planIdsWithProjectId.Select<Tuple<Guid, Guid>, Guid>((Func<Tuple<Guid, Guid>, Guid>) (x => x.Item1)).ToList<Guid>();
            viewDefinitions = component.GetViewDefinitions((IReadOnlyList<Guid>) list);
          }
          performanceTimer.AddProperty("RequestedDefinitionCount", (object) planIdsWithProjectId.Count);
        }
        return this.ConvertDataRecordsToPlans(requestContext, viewDefinitions, planIdsWithProjectId);
      }));
    }

    public Plan CreateView(IVssRequestContext requestContext, Guid projectId, CreatePlan plan)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      this.EnsureCreateViewArgumentIsValid(plan);
      ScaledAgileView view = plan.ToScaledAgileView(requestContext);
      return requestContext.TraceBlock<Plan>(290737, 290738, 290739, "Plans", "PlansService", nameof (CreateView), (Func<Plan>) (() =>
      {
        Guid teamFoundationId = this.GetTeamFoundationId(requestContext);
        using (PerformanceTimer performanceTimer = PerformanceTimer.StartMeasure(requestContext, nameof (CreateView)))
        {
          ScaledAgileView view1;
          using (ScaledAgileViewComponent component = ScaledAgileViewComponent.CreateComponent(requestContext))
          {
            PlanSettings planSettings = PlanSettings.Create(requestContext);
            int viewDefinitionCount = component.GetViewDefinitionCount(projectId);
            performanceTimer.AddProperty("viewCount", (object) viewDefinitionCount);
            performanceTimer.AddProperty("maxPlanCount", (object) planSettings.MaxPlanCountPerProject);
            if (viewDefinitionCount >= planSettings.MaxPlanCountPerProject)
              throw new PlanLimitExceededException();
            view1 = component.CreateView(projectId, view, teamFoundationId);
          }
          this.AddPlanPermissions(requestContext, projectId, view1.Id);
          PlanPermissionsBitFlags bitFlagPermissions = PlanPermissionsBitFlags.Edit | PlanPermissionsBitFlags.Delete;
          IdentityRef createdByIdentity;
          IdentityRef modifiedByIdentity;
          this.GetIdentitiesForScaledAgileView(requestContext, view1, out createdByIdentity, out modifiedByIdentity);
          return view1.ToPlan(requestContext, projectId, true, bitFlagPermissions, createdByIdentity, modifiedByIdentity);
        }
      }));
    }

    public void UpdateView(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid planId,
      UpdatePlan updatedPlan)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForEmptyGuid(planId, nameof (planId));
      ArgumentUtility.CheckForNull<UpdatePlan>(updatedPlan, nameof (updatedPlan));
      requestContext.TraceBlock(290749, 290750, 290751, "Plans", "PlansService", nameof (UpdateView), (Action) (() =>
      {
        using (PerformanceTimer performanceTimer = PerformanceTimer.StartMeasure(requestContext, "GetPlanPermissions"))
        {
          performanceTimer.AddProperty("NumberRequested", (object) 1);
          if (!this.HasPermission(requestContext, projectId, planId, PlanPermissionsBitFlags.Edit))
            throw new ScaledAgileInvalidPermissionException(Microsoft.TeamFoundation.Agile.Web.Resources.PlanPermissionEditMessage());
        }
        this.EnsureUpdateViewArgumentIsValid(requestContext, planId, updatedPlan);
        ScaledAgileView view = updatedPlan.ToScaledAgileView(requestContext, planId);
        Guid changedBy = this.GetTeamFoundationId(requestContext);
        this.UpdateCardRules(requestContext, projectId, view);
        this.ExecuteWrapingSqlExceptions((Action) (() =>
        {
          using (PerformanceTimer.StartMeasure(requestContext, nameof (UpdateView)))
          {
            using (ScaledAgileViewComponent component = ScaledAgileViewComponent.CreateComponent(requestContext))
              component.UpdateView(projectId, planId, changedBy, view);
          }
        }));
      }));
    }

    private void UpdateCardRules(
      IVssRequestContext requestContext,
      Guid projectId,
      ScaledAgileView plan)
    {
      if (plan.Configuration.CardStyles == null)
        return;
      requestContext.GetService<BoardService>().UpdateBoardCardRules(requestContext, projectId, plan.Configuration.CardStyles, new List<string>()
      {
        "fill",
        "tagStyle"
      });
    }

    public void DeleteView(IVssRequestContext requestContext, Guid projectId, int daysOld)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckGreaterThanZero((float) daysOld, nameof (daysOld));
      requestContext.TraceBlock(290740, 290741, 290742, "Plans", "PlansService", nameof (DeleteView), (Action) (() =>
      {
        IList<Guid> source;
        using (PerformanceTimer.StartMeasure(requestContext, nameof (DeleteView)))
        {
          using (ScaledAgileViewComponent component = ScaledAgileViewComponent.CreateComponent(requestContext))
            source = component.DeleteScaledAgileView(projectId, daysOld);
        }
        List<string> list = source.Select<Guid, string>((Func<Guid, string>) (planId => PlanPermissionHelper.GetToken(projectId, planId))).ToList<string>();
        using (PerformanceTimer.StartMeasure(requestContext, "RemoveAccessControlLists"))
          this.GetPermissionsSecurityNamespace(requestContext).RemoveAccessControlLists(requestContext, (IEnumerable<string>) list, true);
      }));
    }

    public void SoftDeleteView(IVssRequestContext requestContext, Guid projectId, Guid viewId)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForEmptyGuid(viewId, nameof (viewId));
      requestContext.TraceBlock(290743, 290744, 290745, "Plans", "PlansService", nameof (SoftDeleteView), (Action) (() =>
      {
        using (PerformanceTimer performanceTimer = PerformanceTimer.StartMeasure(requestContext, "GetPlanPermissions"))
        {
          performanceTimer.AddProperty("NumberRequested", (object) 1);
          if (!this.HasPermission(requestContext, projectId, viewId, PlanPermissionsBitFlags.Delete))
            throw new ScaledAgileInvalidPermissionException(Microsoft.TeamFoundation.Agile.Web.Resources.PlanPermissionDeleteMessage());
        }
        Guid teamFoundationId = this.GetTeamFoundationId(requestContext);
        using (PerformanceTimer.StartMeasure(requestContext, nameof (SoftDeleteView)))
        {
          using (ScaledAgileViewComponent component = ScaledAgileViewComponent.CreateComponent(requestContext))
            component.SoftDeleteScaledAgileView(projectId, viewId, teamFoundationId);
        }
      }));
    }

    public PlanViewData GetViewData(
      IVssRequestContext requestContext,
      Guid viewId,
      PlanType viewType,
      PlanViewFilter viewFilter)
    {
      ArgumentUtility.CheckForEmptyGuid(viewId, nameof (viewId));
      ArgumentUtility.CheckForNull<PlanViewFilter>(viewFilter, nameof (viewFilter));
      return requestContext.TraceBlock<PlanViewData>(290746, 290747, 290748, "Plans", "PlansService", nameof (GetViewData), (Func<PlanViewData>) (() =>
      {
        using (PerformanceTimer.StartMeasure(requestContext, nameof (GetViewData)))
          return PlanViewDataFactory.GetViewData(requestContext, viewId, viewType, viewFilter);
      }));
    }

    public PlanViewData GetNewViewData(
      IVssRequestContext requestContext,
      Guid viewId,
      PlanViewFilter viewFilter)
    {
      ArgumentUtility.CheckForEmptyGuid(viewId, nameof (viewId));
      ArgumentUtility.CheckForNull<PlanViewFilter>(viewFilter, nameof (viewFilter));
      return requestContext.TraceBlock<PlanViewData>(290746, 290747, 290748, "Plans", "PlansService", "GetViewData", (Func<PlanViewData>) (() =>
      {
        using (PerformanceTimer.StartMeasure(requestContext, "GetViewData"))
          return DeliveryTimelineDataProvider.GetNewPlanViewData(requestContext, viewId, viewFilter);
      }));
    }

    public void UpdateLastAccessed(IVssRequestContext requestContext, Guid projectId, Guid planId)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForEmptyGuid(planId, nameof (planId));
      requestContext.TraceBlock(290752, 290753, 290754, "Plans", "PlansService", nameof (UpdateLastAccessed), (Action) (() =>
      {
        using (PerformanceTimer performanceTimer = PerformanceTimer.StartMeasure(requestContext, "GetPlanPermissions"))
        {
          performanceTimer.AddProperty("NumberRequested", (object) 1);
          if (!this.HasPermission(requestContext, projectId, planId, PlanPermissionsBitFlags.View))
            return;
        }
        this.ExecuteWrapingSqlExceptions((Action) (() =>
        {
          using (PerformanceTimer.StartMeasure(requestContext, nameof (UpdateLastAccessed)))
          {
            using (ScaledAgileViewComponent component = ScaledAgileViewComponent.CreateComponent(requestContext))
              component.UpdateLastAccessed(projectId, planId);
          }
        }));
      }));
    }

    private void EnsureCreateViewArgumentIsValid(CreatePlan plan)
    {
      ArgumentUtility.CheckForNull<CreatePlan>(plan, nameof (plan));
      ArgumentUtility.CheckForNull<object>(plan.Properties, "Properties");
      this.ValidatePlanType(plan.Type);
      this.ValidatePlanName(plan.Name);
      this.ValidatePlanDescription(plan.Description);
    }

    private void EnsureUpdateViewArgumentIsValid(
      IVssRequestContext requestContext,
      Guid planId,
      UpdatePlan plan)
    {
      ArgumentUtility.CheckForNull<UpdatePlan>(plan, nameof (plan));
      ArgumentUtility.CheckForNull<object>(plan.Properties, "Properties");
      this.ValidatePlanType(plan.Type);
      this.ValidatePlanName(plan.Name);
      this.ValidatePlanDescription(plan.Description);
      this.ValidateCardSettings(requestContext, planId, plan.GetCardSettings());
    }

    private void ValidatePlanType(PlanType planType)
    {
      if (!Enum.IsDefined(typeof (PlanType), (object) planType))
        throw new ViewTypeDoesNotExistException();
    }

    private void ValidatePlanName(string name)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(name, nameof (name));
      if (name.Length > 128)
        throw new ScaledAgileViewDefinitionInvalidException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Microsoft.TeamFoundation.Agile.Web.Resources.PlanNameTooLongMessage((object) 128)));
      char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
      if (name.IndexOfAny(invalidFileNameChars) >= 0)
        throw new ScaledAgileViewDefinitionInvalidException(Microsoft.TeamFoundation.Agile.Web.Resources.PlanNameContainsInvalidCharacters());
    }

    private void ValidatePlanDescription(string description)
    {
      if (!string.IsNullOrEmpty(description) && description.Length > 256)
        throw new ScaledAgileViewDefinitionInvalidException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Microsoft.TeamFoundation.Agile.Web.Resources.PlanDescriptionTooLongMessage((object) 256)));
    }

    private Guid GetTeamFoundationId(IVssRequestContext requestContext) => requestContext.GetUserIdentity().Id;

    private void ValidateCardSettings(
      IVssRequestContext requestContext,
      Guid planId,
      CardSettings cardSettings)
    {
      ArgumentUtility.CheckForEmptyGuid(planId, nameof (planId));
      if (cardSettings == null || cardSettings.Fields == null || cardSettings.Fields.AdditionalFields == null)
        return;
      new ScaledAgileCardSettingsValidator(requestContext).ValidateAdditionalFields(cardSettings.Fields.AdditionalFields.Select<FieldInfo, string>((Func<FieldInfo, string>) (x => x.ReferenceName)));
    }

    public virtual IdentityDescriptor GetIdentityDescriptor(IVssRequestContext requestContext) => requestContext.GetAuthenticatedDescriptor();

    internal Dictionary<Guid, List<ShallowScaledAgileViewRecord>> SortPlansByProject(
      IReadOnlyList<ShallowScaledAgileViewRecord> views,
      List<Tuple<Guid, Guid>> planIdsWithProjIds)
    {
      Dictionary<Guid, List<ShallowScaledAgileViewRecord>> dictionary = new Dictionary<Guid, List<ShallowScaledAgileViewRecord>>();
      foreach (Tuple<Guid, Guid> planIdsWithProjId in planIdsWithProjIds)
      {
        Guid planId = planIdsWithProjId.Item1;
        Guid key = planIdsWithProjId.Item2;
        ShallowScaledAgileViewRecord scaledAgileViewRecord = views.FirstOrDefault<ShallowScaledAgileViewRecord>((Func<ShallowScaledAgileViewRecord, bool>) (vr => vr.Id == planId));
        if (scaledAgileViewRecord != null)
        {
          if (dictionary.ContainsKey(key))
            dictionary[key].Add(scaledAgileViewRecord);
          else
            dictionary[key] = new List<ShallowScaledAgileViewRecord>()
            {
              scaledAgileViewRecord
            };
        }
      }
      return dictionary;
    }

    public bool HasPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid planId,
      PlanPermissionsBitFlags requestedPermission)
    {
      return this.GetPlanPermissions(requestContext, projectId, planId).HasFlag((Enum) requestedPermission);
    }

    public virtual void AddPlanPermissions(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid planId)
    {
      IVssSecurityNamespace securityNamespace = this.GetPermissionsSecurityNamespace(requestContext);
      this.AddPlanPermissionForCurrentUser(requestContext, securityNamespace, projectId, planId);
      this.AddPlanPermissionForProjectAdministrators(requestContext, securityNamespace, projectId);
    }

    private void AddPlanPermissionForCurrentUser(
      IVssRequestContext requestContext,
      IVssSecurityNamespace projectSecurity,
      Guid projectId,
      Guid planId)
    {
      IdentityDescriptor authenticatedDescriptor = requestContext.GetAuthenticatedDescriptor();
      string token = PlanPermissionHelper.GetToken(projectId, planId);
      using (PerformanceTimer.StartMeasure(requestContext, nameof (AddPlanPermissionForCurrentUser)))
        projectSecurity.SetPermissions(requestContext, token, authenticatedDescriptor, 15, 0, false);
    }

    private void AddPlanPermissionForProjectAdministrators(
      IVssRequestContext requestContext,
      IVssSecurityNamespace projectSecurity,
      Guid projectId)
    {
      IdentityService service = requestContext.GetService<IdentityService>();
      Microsoft.VisualStudio.Services.Identity.Identity identity;
      using (PerformanceTimer.StartMeasure(requestContext, "ReadIdentities"))
        identity = service.ReadIdentities(requestContext, IdentitySearchFilter.AdministratorsGroup, projectId.ToString(), QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (identity == null)
        return;
      string projectAdminToken = PlanPermissionHelper.GetProjectAdminToken(projectId);
      using (PerformanceTimer.StartMeasure(requestContext, nameof (AddPlanPermissionForProjectAdministrators)))
      {
        bool merge = true;
        projectSecurity.SetPermissions(requestContext, projectAdminToken, identity.Descriptor, 15, 0, merge);
      }
    }

    public virtual IVssSecurityNamespace GetPermissionsSecurityNamespace(
      IVssRequestContext requestContext)
    {
      return requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(requestContext, PlanConstants.PlanPermissionNamespaceID);
    }

    public virtual PlanPermissionsBitFlags GetPlanPermissions(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid planId)
    {
      IVssSecurityNamespace securityNamespace = this.GetPermissionsSecurityNamespace(requestContext);
      IdentityDescriptor authenticatedDescriptor = requestContext.GetAuthenticatedDescriptor();
      return this.GetPlanPermissions(requestContext, projectId, planId, securityNamespace, authenticatedDescriptor);
    }

    public virtual PlanPermissionsBitFlags GetPlanPermissions(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid planId,
      IVssSecurityNamespace projectSecurity,
      IdentityDescriptor identity)
    {
      EvaluationPrincipal evaluationPrincipal = new EvaluationPrincipal(identity);
      string token = PlanPermissionHelper.GetToken(projectId, planId);
      return (PlanPermissionsBitFlags) projectSecurity.QueryEffectivePermissions(requestContext, token, evaluationPrincipal);
    }

    private void GetIdentitiesForScaledAgileView(
      IVssRequestContext requestContext,
      ScaledAgileView view,
      out IdentityRef createdByIdentity,
      out IdentityRef modifiedByIdentity)
    {
      List<Guid> identityIds = new List<Guid>()
      {
        view.CreatedBy,
        view.ModifiedBy
      };
      Dictionary<Guid, IdentityRef> identities = this.GetIdentities(requestContext, identityIds);
      identities.TryGetValue(view.CreatedBy, out createdByIdentity);
      identities.TryGetValue(view.ModifiedBy, out modifiedByIdentity);
    }

    private List<ScaledAgileViewService.PlanData> GetValidPlanDataWithPermissions(
      IVssRequestContext requestContext,
      IEnumerable<ShallowScaledAgileViewRecord> viewRecords,
      Guid projectId)
    {
      List<ScaledAgileViewService.PlanData> dataWithPermissions = new List<ScaledAgileViewService.PlanData>();
      IVssSecurityNamespace securityNamespace = this.GetPermissionsSecurityNamespace(requestContext);
      IdentityDescriptor identityDescriptor = this.GetIdentityDescriptor(requestContext);
      foreach (ShallowScaledAgileViewRecord viewRecord in viewRecords)
      {
        PlanPermissionsBitFlags planPermissions = this.GetPlanPermissions(requestContext, projectId, viewRecord.Id, securityNamespace, identityDescriptor);
        if (planPermissions.HasFlag((Enum) PlanPermissionsBitFlags.View))
          dataWithPermissions.Add(new ScaledAgileViewService.PlanData(viewRecord, planPermissions));
      }
      return dataWithPermissions;
    }

    private List<Plan> ConvertDataRecordsToPlans(
      IVssRequestContext requestContext,
      IReadOnlyList<ShallowScaledAgileViewRecord> records,
      Guid projectId)
    {
      List<ScaledAgileViewService.PlanData> dataWithPermissions;
      using (PerformanceTimer.StartMeasure(requestContext, "GetPlanPermissions"))
        dataWithPermissions = this.GetValidPlanDataWithPermissions(requestContext, (IEnumerable<ShallowScaledAgileViewRecord>) records, projectId);
      this.AddIdentitiesToPlanData(requestContext, (IList<ScaledAgileViewService.PlanData>) dataWithPermissions);
      return dataWithPermissions.Select<ScaledAgileViewService.PlanData, Plan>((Func<ScaledAgileViewService.PlanData, Plan>) (x => PlanUtils.ToShallowPlan(x.ViewRecord, x.Permissions, x.CreatedByIdentity, x.ModifiedByIdentity))).ToList<Plan>();
    }

    private List<Plan> ConvertDataRecordsToPlans(
      IVssRequestContext requestContext,
      IReadOnlyList<ShallowScaledAgileViewRecord> records,
      List<Tuple<Guid, Guid>> planIdsWithProjectId)
    {
      List<ScaledAgileViewService.PlanData> planDataList = new List<ScaledAgileViewService.PlanData>();
      using (PerformanceTimer performanceTimer = PerformanceTimer.StartMeasure(requestContext, "GetPlanPermissions"))
      {
        foreach (KeyValuePair<Guid, List<ShallowScaledAgileViewRecord>> keyValuePair in this.SortPlansByProject(records, planIdsWithProjectId))
          planDataList.AddRange((IEnumerable<ScaledAgileViewService.PlanData>) this.GetValidPlanDataWithPermissions(requestContext, (IEnumerable<ShallowScaledAgileViewRecord>) keyValuePair.Value, keyValuePair.Key));
        performanceTimer.AddProperty("NumberRequested", (object) planDataList.Count);
      }
      this.AddIdentitiesToPlanData(requestContext, (IList<ScaledAgileViewService.PlanData>) planDataList);
      return planDataList.Select<ScaledAgileViewService.PlanData, Plan>((Func<ScaledAgileViewService.PlanData, Plan>) (x => PlanUtils.ToShallowPlan(x.ViewRecord, x.Permissions, x.CreatedByIdentity, x.ModifiedByIdentity))).ToList<Plan>();
    }

    private void AddIdentitiesToPlanData(
      IVssRequestContext requestContext,
      IList<ScaledAgileViewService.PlanData> plans)
    {
      using (PerformanceTimer.StartMeasure(requestContext, "ResolveIdentities"))
      {
        List<Guid> list = plans.Select<ScaledAgileViewService.PlanData, Guid>((Func<ScaledAgileViewService.PlanData, Guid>) (x => x.ViewRecord.CreatedBy)).ToList<Guid>();
        list.AddRange(plans.Select<ScaledAgileViewService.PlanData, Guid>((Func<ScaledAgileViewService.PlanData, Guid>) (x => x.ViewRecord.ModifiedBy)));
        Dictionary<Guid, IdentityRef> identities = this.GetIdentities(requestContext, list);
        foreach (ScaledAgileViewService.PlanData plan in (IEnumerable<ScaledAgileViewService.PlanData>) plans)
        {
          IdentityRef identityRef;
          if (identities.TryGetValue(plan.ViewRecord.CreatedBy, out identityRef))
            plan.CreatedByIdentity = identityRef;
          if (identities.TryGetValue(plan.ViewRecord.ModifiedBy, out identityRef))
            plan.ModifiedByIdentity = identityRef;
        }
      }
    }

    public Dictionary<Guid, IdentityRef> GetIdentities(
      IVssRequestContext requestContext,
      List<Guid> identityIds)
    {
      if (identityIds == null)
        throw new ArgumentNullException(nameof (identityIds));
      bool flag = false;
      Dictionary<Guid, IdentityRef> identities = new Dictionary<Guid, IdentityRef>();
      foreach (Guid identityId in identityIds)
      {
        if (identityId == Guid.Empty)
          flag = true;
        else if (!identities.ContainsKey(identityId))
          identities.Add(identityId, (IdentityRef) null);
      }
      List<Guid> list = identities.Keys.ToList<Guid>();
      Guid guid;
      if (list.Count != 0)
      {
        IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<Guid>) list, QueryMembership.None, (IEnumerable<string>) null);
        for (int index = 0; index < identityList.Count; ++index)
        {
          Microsoft.VisualStudio.Services.Identity.Identity identity = identityList[index];
          if (identity == null)
          {
            Guid key = list[index];
            identities[key] = new IdentityRef()
            {
              Id = key.ToString()
            };
          }
          else
          {
            Dictionary<Guid, IdentityRef> dictionary = identities;
            Guid id = identity.Id;
            IdentityRef identityRef = new IdentityRef();
            guid = identity.Id;
            identityRef.Id = guid.ToString();
            identityRef.DisplayName = identity.DisplayName;
            dictionary[id] = identityRef;
          }
        }
      }
      if (flag)
      {
        Dictionary<Guid, IdentityRef> dictionary = identities;
        Guid empty = Guid.Empty;
        IdentityRef identityRef = new IdentityRef();
        guid = Guid.Empty;
        identityRef.Id = guid.ToString();
        dictionary.Add(empty, identityRef);
      }
      return identities;
    }

    private void ExecuteWrapingSqlExceptions(Action method)
    {
      try
      {
        method();
      }
      catch (ViewNotFoundException ex)
      {
        throw new ViewNotFoundException((Exception) ex);
      }
      catch (ViewRevisionMismatchException ex)
      {
        throw new ViewRevisionMismatchException((Exception) ex);
      }
    }

    public (int plansCount, int maxPlans) GetPlansCount(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      int planCountPerProject = PlanSettings.Create(requestContext).MaxPlanCountPerProject;
      return (this.GetViewDefinitions(requestContext, projectId, new Guid()).Count<Plan>(), planCountPerProject);
    }

    private class PlanData
    {
      public PlanData(ShallowScaledAgileViewRecord record, PlanPermissionsBitFlags permissions)
      {
        this.ViewRecord = record;
        this.Permissions = permissions;
      }

      public ShallowScaledAgileViewRecord ViewRecord { get; private set; }

      public PlanPermissionsBitFlags Permissions { get; private set; }

      public IdentityRef CreatedByIdentity { get; set; }

      public IdentityRef ModifiedByIdentity { get; set; }
    }
  }
}

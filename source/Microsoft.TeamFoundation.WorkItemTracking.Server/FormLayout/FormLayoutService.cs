// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.FormLayoutService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Provision;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout
{
  public class FormLayoutService : IFormLayoutService, IVssFrameworkService
  {
    private ILayoutOperations m_layoutOperations;

    private int m_maxGroupsAllowed { get; set; }

    private int m_maxPagesAllowed { get; set; }

    private int m_maxControlsAllowed { get; set; }

    public FormLayoutService()
      : this((ILayoutOperations) new LayoutOperations())
    {
    }

    public FormLayoutService(ILayoutOperations layoutOperations) => this.m_layoutOperations = layoutOperations;

    public virtual void ServiceStart(IVssRequestContext systemRequestContext) => this.SetLayoutLimits(systemRequestContext);

    public virtual void ServiceEnd(IVssRequestContext systemRequestContext) => systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));

    public LayoutInfo GetLayout(
      IVssRequestContext requestContext,
      Guid processId,
      string witRefName)
    {
      ComposedWorkItemType workItemType = requestContext.GetService<IProcessWorkItemTypeService>().GetWorkItemType(requestContext, processId, witRefName, true);
      if (!workItemType.IsDerived && !workItemType.IsCustomType)
        throw new FormLayoutInfoNotAvailableException(witRefName, processId.ToString());
      return workItemType.GetFormLayoutInfo(requestContext);
    }

    public Layout CombineLayouts(
      IVssRequestContext requestContext,
      Layout baseLayout,
      Layout deltaLayout)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(910001, "Services", nameof (FormLayoutService), "CombineLayout");
      try
      {
        return this.m_layoutOperations.BuildCombinedLayout(requestContext, baseLayout, deltaLayout);
      }
      catch (Exception ex) when (FormLayoutService.TraceCombineLayoutsException(requestContext, ex))
      {
        throw;
      }
      finally
      {
        requestContext.TraceLeave(910002, "Services", nameof (FormLayoutService), "CombineLayout");
      }
    }

    public Layout SetFieldControlInGroup(
      IVssRequestContext requestContext,
      Guid processId,
      string witRefName,
      string groupId,
      Control control,
      int? order,
      bool isEdit = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<string>(witRefName, nameof (witRefName));
      ArgumentUtility.CheckForNull<Control>(control, nameof (control));
      ArgumentUtility.CheckStringForNullOrEmpty(groupId, nameof (groupId));
      requestContext.TraceEnter(910004, "Services", nameof (FormLayoutService), nameof (SetFieldControlInGroup));
      try
      {
        bool flag = false;
        IProcessWorkItemTypeService service = requestContext.GetService<IProcessWorkItemTypeService>();
        ComposedWorkItemType workItemType = service.GetWorkItemType(requestContext, processId, witRefName, true);
        LayoutInfo layout1 = this.GetLayout(requestContext, processId, witRefName);
        if (string.IsNullOrEmpty(control.Id))
          control.Id = Guid.NewGuid().ToString();
        Layout composedLayout = layout1.ComposedLayout;
        Layout deltaLayout = layout1.DeltaLayout;
        Layout baseLayout = layout1.BaseLayout;
        Group descendant1 = composedLayout.FindDescendant<Group>(groupId);
        if (descendant1 == null)
          throw new FormLayoutGroupDoesNotExistException(witRefName, groupId);
        if (descendant1.IsContribution)
          throw new FormLayoutAddControlToContributedGroupException();
        Section ancestorOf1 = composedLayout.FindAncestorOf<Group, Section>(groupId);
        string controlType = FormLayoutService.GetControlType(workItemType.GetLegacyFields(requestContext).Where<ProcessFieldResult>((Func<ProcessFieldResult, bool>) (f => TFStringComparer.WorkItemFieldReferenceName.Equals(f.ReferenceName, control.Id))).FirstOrDefault<ProcessFieldResult>(), control);
        if (LayoutConstants.WideControls.Contains(controlType) && FormLayoutService.IsSealedGroup(descendant1) && string.Equals(descendant1.Children[0].Id, control.Id, StringComparison.OrdinalIgnoreCase))
        {
          descendant1.Label = control.Label;
          flag = true;
        }
        else
        {
          if (LayoutConstants.WideControls.Contains(controlType))
            throw new FormLayoutAddHTMLControlToGroupException();
          if (FormLayoutService.IsSealedGroup(descendant1))
            throw new FormLayoutHtmlGroupInvalidControlException();
        }
        Page ancestorOf2 = composedLayout.FindAncestorOf<Section, Page>(ancestorOf1.Id);
        FormLayoutService.ValidateControls(requestContext, (IEnumerable<Control>) new Control[1]
        {
          control
        }, workItemType);
        if (!order.HasValue)
        {
          Control descendant2 = deltaLayout.FindDescendant<Control>(control.Id);
          if (descendant2 != null)
            control.Rank = descendant2.Rank;
        }
        this.CheckControlsLimit(control.Id, groupId, composedLayout);
        if (!isEdit)
          this.CheckControlAlreadyExists(control.Id, groupId, composedLayout);
        Layout layout2 = this.m_layoutOperations.PutControlInExistingGroup(witRefName, composedLayout, deltaLayout, groupId, control, order);
        if (flag)
          layout2 = this.m_layoutOperations.EditGroup(witRefName, composedLayout, baseLayout, layout2, ancestorOf2.Id, ancestorOf1.Id, descendant1, new int?());
        service.UpdateCustomForm(requestContext, processId, witRefName, layout2);
        this.RecordCIForFieldAddOrMove(requestContext, witRefName, layout1, "fieldAddedToGroup", (string) null, groupId, control.Id, order);
        return layout2;
      }
      finally
      {
        requestContext.TraceLeave(910005, "Services", nameof (FormLayoutService), nameof (SetFieldControlInGroup));
      }
    }

    public Layout RemoveFieldControlFromGroup(
      IVssRequestContext requestContext,
      Guid processId,
      string witRefName,
      string groupId,
      string fieldRefName)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<string>(witRefName, nameof (witRefName));
      ArgumentUtility.CheckStringForNullOrEmpty(groupId, nameof (groupId));
      ArgumentUtility.CheckStringForNullOrEmpty(fieldRefName, nameof (fieldRefName));
      requestContext.TraceEnter(910006, "Services", nameof (FormLayoutService), nameof (RemoveFieldControlFromGroup));
      try
      {
        LayoutInfo layout1 = this.GetLayout(requestContext, processId, witRefName);
        Layout layout2 = this.m_layoutOperations.RemoveControlFromGroup(witRefName, layout1.ComposedLayout, layout1.DeltaLayout, groupId, fieldRefName);
        requestContext.GetService<IProcessWorkItemTypeService>().UpdateCustomForm(requestContext, processId, witRefName, layout2);
        return layout2;
      }
      finally
      {
        requestContext.TraceLeave(910007, "Services", nameof (FormLayoutService), nameof (RemoveFieldControlFromGroup));
      }
    }

    public Layout MoveFieldControlToGroup(
      IVssRequestContext requestContext,
      Guid processId,
      string witRefName,
      string sourceGroupId,
      string targetGroupId,
      Control control,
      int? order)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<string>(witRefName, nameof (witRefName));
      ArgumentUtility.CheckForNull<Control>(control, nameof (control));
      ArgumentUtility.CheckStringForNullOrEmpty(sourceGroupId, nameof (sourceGroupId));
      ArgumentUtility.CheckStringForNullOrEmpty(targetGroupId, nameof (targetGroupId));
      ArgumentUtility.CheckStringForNullOrEmpty(control.Id, "Id");
      requestContext.TraceEnter(910010, "Services", nameof (FormLayoutService), nameof (MoveFieldControlToGroup));
      try
      {
        IProcessWorkItemTypeService service = requestContext.GetService<IProcessWorkItemTypeService>();
        ComposedWorkItemType workItemType = service.GetWorkItemType(requestContext, processId, witRefName, true);
        LayoutInfo layout1 = this.GetLayout(requestContext, processId, witRefName);
        Layout composedLayout1 = layout1.ComposedLayout;
        ProcessFieldResult field = workItemType.GetLegacyFields(requestContext).Where<ProcessFieldResult>((Func<ProcessFieldResult, bool>) (f => TFStringComparer.WorkItemFieldReferenceName.Equals(f.ReferenceName, control.Id))).FirstOrDefault<ProcessFieldResult>();
        Group descendant = composedLayout1.FindDescendant<Group>(targetGroupId);
        if (descendant != null)
        {
          if (LayoutConstants.WideControls.Contains(FormLayoutService.GetControlType(field, control)))
            throw new FormLayoutAddHTMLControlToGroupException();
          if (FormLayoutService.IsSealedGroup(descendant))
            throw new FormLayoutHtmlGroupInvalidControlException();
          if (descendant.IsContribution)
            throw new FormLayoutAddControlToContributedGroupException();
        }
        FormLayoutService.ValidateControls(requestContext, (IEnumerable<Control>) new Control[1]
        {
          control
        }, workItemType);
        if (!string.Equals(sourceGroupId, targetGroupId, StringComparison.OrdinalIgnoreCase))
        {
          this.CheckControlsLimit(control.Id, targetGroupId, layout1.ComposedLayout);
          this.CheckControlAlreadyExists(control.Id, targetGroupId, layout1.ComposedLayout);
          control.Rank = new int?();
        }
        Layout deltaLayout = this.m_layoutOperations.RemoveControlFromGroup(witRefName, composedLayout1, layout1.DeltaLayout, sourceGroupId, control.Id);
        Layout composedLayout2 = deltaLayout.Clone();
        if (layout1.BaseLayout != null)
          composedLayout2 = this.m_layoutOperations.BuildCombinedLayout(requestContext, layout1.BaseLayout, deltaLayout);
        Layout layout2 = this.m_layoutOperations.PutControlInExistingGroup(witRefName, composedLayout2, deltaLayout, targetGroupId, control, order);
        service.UpdateCustomForm(requestContext, processId, witRefName, layout2);
        this.RecordCIForFieldAddOrMove(requestContext, witRefName, layout1, "moveFieldToGroup", sourceGroupId, targetGroupId, control.Id, order);
        return layout2;
      }
      finally
      {
        requestContext.TraceLeave(910011, "Services", nameof (FormLayoutService), nameof (MoveFieldControlToGroup));
      }
    }

    public Layout RemoveFieldFromLayout(
      IVssRequestContext requestContext,
      Guid processId,
      string witRefName,
      string fieldRefName,
      bool suppressPermissionCheck = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<string>(witRefName, nameof (witRefName));
      ArgumentUtility.CheckStringForNullOrEmpty(fieldRefName, nameof (fieldRefName));
      requestContext.TraceEnter(910008, "Services", nameof (FormLayoutService), nameof (RemoveFieldFromLayout));
      try
      {
        LayoutInfo layout1 = this.GetLayout(requestContext, processId, witRefName);
        Layout layout2 = this.m_layoutOperations.RemoveControlFromLayout(witRefName, layout1.ComposedLayout, layout1.DeltaLayout, fieldRefName);
        requestContext.GetService<IProcessWorkItemTypeService>().UpdateCustomForm(requestContext, processId, witRefName, layout2, suppressPermissionCheck);
        return layout2;
      }
      finally
      {
        requestContext.TraceLeave(910009, "Services", nameof (FormLayoutService), nameof (RemoveFieldFromLayout));
      }
    }

    public Layout AddGroup(
      IVssRequestContext requestContext,
      Guid processId,
      string witRefName,
      string pageId,
      string sectionId,
      Group group,
      int? order)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<string>(witRefName, nameof (witRefName));
      ArgumentUtility.CheckStringForNullOrEmpty(pageId, nameof (pageId));
      ArgumentUtility.CheckStringForNullOrEmpty(sectionId, nameof (sectionId));
      ArgumentUtility.CheckForNull<Group>(group, nameof (group));
      FormValidationUtils.CheckLabel(group.Label);
      FormValidationUtils.CheckRestrictedSections(sectionId);
      requestContext.TraceEnter(910012, "Services", nameof (FormLayoutService), nameof (AddGroup));
      try
      {
        LayoutInfo layout1 = this.GetLayout(requestContext, processId, witRefName);
        FormLayoutService.ValidateGroup(requestContext, layout1.ComposedLayout, processId, witRefName, pageId, sectionId, group);
        if (group.IsContribution)
          throw new FormLayoutAddContributedGroupException(group.Label ?? group.Id);
        this.CheckGroupsLimit(group.Label, pageId, sectionId, layout1.ComposedLayout);
        group.Id = Guid.NewGuid().ToString();
        Layout layout2 = this.m_layoutOperations.AddGroup(witRefName, layout1.ComposedLayout, layout1.BaseLayout, layout1.DeltaLayout, pageId, sectionId, group, order);
        requestContext.GetService<IProcessWorkItemTypeService>().UpdateCustomForm(requestContext, processId, witRefName, layout2);
        this.RecordCIForGroupAddOrEdit("addGroup", requestContext, witRefName, layout1, pageId, sectionId, group, order);
        return layout2;
      }
      finally
      {
        requestContext.TraceLeave(910013, "Services", nameof (FormLayoutService), nameof (AddGroup));
      }
    }

    public Layout EditGroup(
      IVssRequestContext requestContext,
      Guid processId,
      string witRefName,
      string pageId,
      string sectionId,
      Group group,
      int? order)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<string>(witRefName, nameof (witRefName));
      ArgumentUtility.CheckStringForNullOrEmpty(pageId, nameof (pageId));
      ArgumentUtility.CheckStringForNullOrEmpty(sectionId, nameof (sectionId));
      ArgumentUtility.CheckForNull<Group>(group, nameof (group));
      ArgumentUtility.CheckForNull<string>(group.Id, "Id");
      FormValidationUtils.CheckLabel(group.Label);
      requestContext.TraceEnter(910014, "Services", nameof (FormLayoutService), nameof (EditGroup));
      try
      {
        LayoutInfo layout1 = this.GetLayout(requestContext, processId, witRefName);
        FormLayoutService.ValidateGroup(requestContext, layout1.ComposedLayout, processId, witRefName, pageId, sectionId, group);
        Layout layout2 = this.m_layoutOperations.EditGroup(witRefName, layout1.ComposedLayout, layout1.BaseLayout, layout1.DeltaLayout, pageId, sectionId, group, order);
        requestContext.GetService<IProcessWorkItemTypeService>().UpdateCustomForm(requestContext, processId, witRefName, layout2);
        this.RecordCIForGroupAddOrEdit("editGroup", requestContext, witRefName, layout1, pageId, sectionId, group, order);
        return layout2;
      }
      finally
      {
        requestContext.TraceLeave(910015, "Services", nameof (FormLayoutService), nameof (EditGroup));
      }
    }

    public Layout RemoveGroup(
      IVssRequestContext requestContext,
      Guid processId,
      string witRefName,
      string pageId,
      string sectionId,
      string groupId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<string>(witRefName, nameof (witRefName));
      ArgumentUtility.CheckStringForNullOrEmpty(pageId, nameof (pageId));
      ArgumentUtility.CheckStringForNullOrEmpty(sectionId, nameof (sectionId));
      ArgumentUtility.CheckStringForNullOrEmpty(groupId, nameof (groupId));
      FormValidationUtils.CheckRestrictedSections(sectionId);
      requestContext.TraceEnter(910016, "Services", nameof (FormLayoutService), "EditGroup");
      try
      {
        LayoutInfo layout1 = this.GetLayout(requestContext, processId, witRefName);
        Group descendant = layout1.ComposedLayout.FindDescendant<Group>(groupId);
        if (descendant != null && descendant.IsContribution)
          throw new FormLayoutRemoveContributedGroupException();
        Layout layout2 = this.m_layoutOperations.RemoveGroup(witRefName, layout1.ComposedLayout, layout1.DeltaLayout, pageId, sectionId, groupId, true);
        requestContext.GetService<IProcessWorkItemTypeService>().UpdateCustomForm(requestContext, processId, witRefName, layout2);
        return layout2;
      }
      finally
      {
        requestContext.TraceLeave(910017, "Services", nameof (FormLayoutService), "EditGroup");
      }
    }

    public Layout SetGroupInSection(
      IVssRequestContext requestContext,
      Guid processId,
      string witRefName,
      string pageId,
      string sourceSectionId,
      string targetSectionId,
      Group group,
      int? order)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<string>(witRefName, nameof (witRefName));
      ArgumentUtility.CheckStringForNullOrEmpty(pageId, nameof (pageId));
      ArgumentUtility.CheckStringForNullOrEmpty(sourceSectionId, nameof (sourceSectionId));
      ArgumentUtility.CheckStringForNullOrEmpty(targetSectionId, nameof (targetSectionId));
      ArgumentUtility.CheckForNull<Group>(group, nameof (group));
      ArgumentUtility.CheckStringForNullOrEmpty(group.Id, "Id");
      FormValidationUtils.CheckRestrictedSections(sourceSectionId);
      FormValidationUtils.CheckRestrictedSections(targetSectionId);
      requestContext.TraceEnter(910010, "Services", nameof (FormLayoutService), "MoveFieldControlToGroup");
      try
      {
        return this.MoveGroup(requestContext, processId, witRefName, pageId, sourceSectionId, targetSectionId, group, order);
      }
      finally
      {
        requestContext.TraceLeave(910011, "Services", nameof (FormLayoutService), "MoveFieldControlToGroup");
      }
    }

    public Layout SetGroupInPage(
      IVssRequestContext requestContext,
      Guid processId,
      string witRefName,
      string sourcePageId,
      string targetPageId,
      string sourceSectionId,
      string targetSectionId,
      Group group,
      int? order)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<string>(witRefName, nameof (witRefName));
      ArgumentUtility.CheckStringForNullOrEmpty(sourcePageId, nameof (sourcePageId));
      ArgumentUtility.CheckStringForNullOrEmpty(targetPageId, nameof (targetPageId));
      ArgumentUtility.CheckStringForNullOrEmpty(sourceSectionId, nameof (sourceSectionId));
      ArgumentUtility.CheckStringForNullOrEmpty(targetSectionId, nameof (targetSectionId));
      ArgumentUtility.CheckForNull<Group>(group, nameof (group));
      ArgumentUtility.CheckStringForNullOrEmpty(group.Id, "Id");
      FormValidationUtils.CheckRestrictedSections(sourceSectionId);
      FormValidationUtils.CheckRestrictedSections(targetSectionId);
      requestContext.TraceEnter(910027, "Services", nameof (FormLayoutService), "MoveGroupToPage");
      try
      {
        return this.MoveGroup(requestContext, processId, witRefName, sourcePageId, sourceSectionId, targetSectionId, group, order, targetPageId);
      }
      finally
      {
        requestContext.TraceLeave(910028, "Services", nameof (FormLayoutService), "MoveGroupToPage");
      }
    }

    public Layout AddPage(
      IVssRequestContext requestContext,
      Guid processId,
      string witRefName,
      Page page,
      int? order)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<string>(witRefName, nameof (witRefName));
      ArgumentUtility.CheckForNull<Page>(page, nameof (page));
      FormValidationUtils.CheckLabel(page.Label);
      requestContext.TraceEnter(910019, "Services", nameof (FormLayoutService), nameof (AddPage));
      try
      {
        LayoutInfo layout1 = this.GetLayout(requestContext, processId, witRefName);
        this.ValidatePage(requestContext, layout1.ComposedLayout, processId, witRefName, page);
        if (page.IsContribution)
          throw new FormLayoutAddContributedPageException(page.Label ?? page.Id);
        this.CheckPagesLimit(page.Label, layout1.ComposedLayout);
        page.Id = Guid.NewGuid().ToString();
        Layout layout2 = this.m_layoutOperations.AddPage(witRefName, layout1.ComposedLayout, layout1.DeltaLayout, page, order);
        requestContext.GetService<IProcessWorkItemTypeService>().UpdateCustomForm(requestContext, processId, witRefName, layout2);
        this.RecordCIForPageOperation(requestContext, witRefName, layout1, page.Id, order, nameof (AddPage));
        return layout2;
      }
      finally
      {
        requestContext.TraceLeave(910020, "Services", nameof (FormLayoutService), nameof (AddPage));
      }
    }

    public Layout EditPage(
      IVssRequestContext requestContext,
      Guid processId,
      string witRefName,
      Page page,
      int? order)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<string>(witRefName, nameof (witRefName));
      ArgumentUtility.CheckForNull<Page>(page, nameof (page));
      ArgumentUtility.CheckForNull<string>(page.Id, "Id");
      FormValidationUtils.CheckLabel(page.Label);
      requestContext.TraceEnter(910021, "Services", nameof (FormLayoutService), nameof (EditPage));
      try
      {
        LayoutInfo layout1 = this.GetLayout(requestContext, processId, witRefName);
        this.ValidatePage(requestContext, layout1.ComposedLayout, processId, witRefName, page);
        Layout layout2 = this.m_layoutOperations.EditPage(witRefName, layout1.ComposedLayout, layout1.BaseLayout, layout1.DeltaLayout, page, order);
        requestContext.GetService<IProcessWorkItemTypeService>().UpdateCustomForm(requestContext, processId, witRefName, layout2);
        this.RecordCIForPageOperation(requestContext, witRefName, layout1, page.Id, order, nameof (EditPage));
        return layout2;
      }
      finally
      {
        requestContext.TraceLeave(910022, "Services", nameof (FormLayoutService), nameof (EditPage));
      }
    }

    public Layout RemovePage(
      IVssRequestContext requestContext,
      Guid processId,
      string witRefName,
      string pageId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<string>(witRefName, nameof (witRefName));
      ArgumentUtility.CheckStringForNullOrEmpty(pageId, nameof (pageId));
      requestContext.TraceEnter(910023, "Services", nameof (FormLayoutService), nameof (RemovePage));
      try
      {
        LayoutInfo layout1 = this.GetLayout(requestContext, processId, witRefName);
        Page descendant = layout1.ComposedLayout.FindDescendant<Page>(pageId);
        if (descendant != null && descendant.IsContribution)
          throw new FormLayoutRemoveContributedPageException(descendant.Label ?? descendant.Id);
        this.RecordCIForPageOperation(requestContext, witRefName, layout1, pageId, new int?(), "DeletePage");
        Layout layout2 = this.m_layoutOperations.RemovePage(witRefName, layout1.DeltaLayout, pageId);
        requestContext.GetService<IProcessWorkItemTypeService>().UpdateCustomForm(requestContext, processId, witRefName, layout2);
        return layout2;
      }
      finally
      {
        requestContext.TraceLeave(910024, "Services", nameof (FormLayoutService), nameof (RemovePage));
      }
    }

    public Layout AddorEditSystemControls(
      IVssRequestContext requestContext,
      Guid processId,
      string witRefName,
      ISet<Control> systemControlsToUpdate)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<string>(witRefName, nameof (witRefName));
      ArgumentUtility.CheckForNull<ISet<Control>>(systemControlsToUpdate, nameof (systemControlsToUpdate));
      IEnumerable<Control> source = systemControlsToUpdate.Where<Control>((Func<Control, bool>) (ctrl => !WebLayoutXmlHelper.ReplaceableOrHidableSystemControlFields.Contains(ctrl.Id)));
      if (source.Any<Control>())
        throw new FormLayoutInvalidSystemControlException(source.First<Control>().Id);
      requestContext.TraceEnter(910029, "Services", nameof (FormLayoutService), nameof (AddorEditSystemControls));
      try
      {
        LayoutInfo layout1 = this.GetLayout(requestContext, processId, witRefName);
        HashSet<Control> systemControls1 = layout1.DeltaLayout.SystemControls;
        HashSet<Control> systemControls2 = layout1.BaseLayout.SystemControls;
        HashSet<Control> updatedSystemControls = new HashSet<Control>();
        foreach (Control equalValue in (IEnumerable<Control>) systemControlsToUpdate)
        {
          Control control1 = equalValue.Clone();
          Control actualValue1;
          if (systemControls2.TryGetValue(equalValue, out actualValue1))
          {
            control1.Id = actualValue1.Id;
            control1.ReadOnly = actualValue1.ReadOnly;
            control1.ControlType = actualValue1.ControlType;
          }
          Control actualValue2;
          bool? visible;
          if (systemControls1.TryGetValue(equalValue, out actualValue2))
          {
            control1.Label = equalValue.Label ?? actualValue2.Label;
            Control control2 = control1;
            visible = equalValue.Visible;
            bool? nullable = visible ?? actualValue2.Visible;
            control2.Visible = nullable;
          }
          else
          {
            control1.Label = equalValue.Label ?? actualValue1.Label;
            Control control3 = control1;
            visible = equalValue.Visible;
            bool? nullable = visible ?? actualValue1.Visible;
            control3.Visible = nullable;
          }
          updatedSystemControls.Add(control1);
        }
        Layout layout2 = this.m_layoutOperations.AddOrEditSystemControls(layout1.DeltaLayout, (ISet<Control>) updatedSystemControls);
        requestContext.GetService<IProcessWorkItemTypeService>().UpdateCustomForm(requestContext, processId, witRefName, layout2);
        return layout2;
      }
      finally
      {
        requestContext.TraceLeave(910030, "Services", nameof (FormLayoutService), nameof (AddorEditSystemControls));
      }
    }

    public Layout RemoveSystemControls(
      IVssRequestContext requestContext,
      Guid processId,
      string witRefName,
      ISet<string> systemControlIds)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<string>(witRefName, nameof (witRefName));
      ArgumentUtility.CheckForNull<ISet<string>>(systemControlIds, nameof (systemControlIds));
      requestContext.TraceEnter(910031, "Services", nameof (FormLayoutService), nameof (RemoveSystemControls));
      try
      {
        LayoutInfo layout1 = this.GetLayout(requestContext, processId, witRefName);
        if (!systemControlIds.Any<string>())
          return layout1.DeltaLayout;
        Layout layout2 = this.m_layoutOperations.RemoveSystemControls(layout1.DeltaLayout, systemControlIds);
        requestContext.GetService<IProcessWorkItemTypeService>().UpdateCustomForm(requestContext, processId, witRefName, layout2);
        return layout2;
      }
      finally
      {
        requestContext.TraceLeave(910032, "Services", nameof (FormLayoutService), nameof (RemoveSystemControls));
      }
    }

    private Layout MoveGroup(
      IVssRequestContext requestContext,
      Guid processId,
      string witRefName,
      string sourcePageId,
      string sourceSectionId,
      string targetSectionId,
      Group group,
      int? order,
      string targetPageId = null)
    {
      if (string.IsNullOrEmpty(targetPageId))
        targetPageId = sourcePageId;
      LayoutInfo layout1 = this.GetLayout(requestContext, processId, witRefName);
      Group group1 = (layout1.DeltaLayout.FindDescendant<Group>(group.Id) ?? throw new FormLayoutGroupDoesNotExistException(witRefName, group.Label)).Clone();
      if (group.Label != null)
      {
        FormValidationUtils.CheckLabel(group.Label);
        group1.Label = group.Label;
      }
      FormLayoutService.ValidateGroup(requestContext, layout1.ComposedLayout, processId, witRefName, targetPageId, targetSectionId, group1);
      if (!string.Equals(sourceSectionId, targetSectionId, StringComparison.OrdinalIgnoreCase) || !string.Equals(sourcePageId, targetPageId, StringComparison.OrdinalIgnoreCase))
      {
        this.CheckGroupsLimit(group.Label, targetPageId, targetSectionId, layout1.ComposedLayout);
        group1.Rank = new int?();
      }
      Layout deltaLayout = this.m_layoutOperations.RemoveGroup(witRefName, layout1.ComposedLayout, layout1.DeltaLayout, sourcePageId, sourceSectionId, group.Id, true);
      Layout composedLayout = deltaLayout.Clone();
      if (layout1.BaseLayout != null)
        composedLayout = this.m_layoutOperations.BuildCombinedLayout(requestContext, layout1.BaseLayout, deltaLayout);
      Layout layout2 = this.m_layoutOperations.AddGroup(witRefName, composedLayout, layout1.BaseLayout, deltaLayout, targetPageId, targetSectionId, group1, order);
      requestContext.GetService<IProcessWorkItemTypeService>().UpdateCustomForm(requestContext, processId, witRefName, layout2);
      this.RecordCIForGroupAddOrEdit("moveGroup", requestContext, witRefName, layout1, targetSectionId, targetSectionId, group, order);
      return layout2;
    }

    private void ValidatePage(
      IVssRequestContext requestContext,
      Layout composedLayout,
      Guid processId,
      string witRefName,
      Page page)
    {
      if (page.GetDescendants<Group>().Any<Group>())
        throw new FormLayoutPageInvalidOperationException();
      page.Label = page.Label?.Trim();
      Page descendant = composedLayout.FindDescendant<Page>(page.Id);
      if ((descendant == null || !descendant.IsContribution) && page.Label != null && composedLayout.Children.Any<Page>((Func<Page, bool>) (p => p.Label != null && !string.Equals(p.Id, page.Id, StringComparison.OrdinalIgnoreCase) && string.Equals(p.Label.Trim(), page.Label, StringComparison.OrdinalIgnoreCase))))
        throw new FormLayoutPageAlreadyExistsException(page.Label);
    }

    private static void ValidateGroup(
      IVssRequestContext requestContext,
      Layout composedLayout,
      Guid processId,
      string witRefName,
      string pageId,
      string sectionId,
      Group group)
    {
      group.Label = group.Label?.Trim();
      ComposedWorkItemType workItemType = requestContext.GetService<IProcessWorkItemTypeService>().GetWorkItemType(requestContext, processId, witRefName, true);
      Page descendant = composedLayout.FindDescendant<Page>(pageId);
      IList<Control> children = group.Children;
      FormLayoutService.ValidateControls(requestContext, (IEnumerable<Control>) children, workItemType);
      if (descendant == null)
        return;
      if (descendant.IsContribution)
        throw new FormLayoutAddGroupToContributedPageException(group.Label ?? group.Id, descendant.Label ?? descendant.Id);
      if (FormLayoutService.IsSealedGroup(group))
        return;
      IEnumerable<Group> source = descendant.GetDescendants<Group>().Cast<Group>();
      if (group.Label != null && source.Any<Group>((Func<Group, bool>) (g => !g.IsContribution && g.Label != null && !FormLayoutService.IsSealedGroup(g) && !string.Equals(g.Id, group.Id, StringComparison.OrdinalIgnoreCase) && string.Equals(g.Label.Trim(), group.Label, StringComparison.OrdinalIgnoreCase))))
      {
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add("GroupLabel", group.Label);
        properties.Add("WITRefName", witRefName);
        if (workItemType != null)
          properties.Add("WITName", workItemType.Name);
        int num = 0;
        if (descendant.Children != null)
        {
          foreach (Section child1 in (IEnumerable<Section>) descendant.Children)
          {
            if (child1.Children != null)
            {
              string str = "";
              bool flag = true;
              foreach (Group child2 in (IEnumerable<Group>) child1.Children)
              {
                if (child2 != null)
                {
                  if (flag)
                    flag = false;
                  else
                    str += ", ";
                  str = str + "[" + child2.Label + ", FromInheritedLayout: " + child2.FromInheritedLayout.ToString() + "]";
                }
              }
              properties.Add("PageGroups" + num.ToString(), str);
            }
            ++num;
          }
        }
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, nameof (FormLayoutService), nameof (ValidateGroup), properties);
        throw new FormLayoutGroupAlreadyExistsException(group.Label, descendant.Label);
      }
    }

    private static void ValidateControls(
      IVssRequestContext requestContext,
      IEnumerable<Control> controls,
      ComposedWorkItemType workItemType)
    {
      int contributionInputLimit = requestContext.WitContext().ServerSettings.ControlContributionInputLimit;
      IReadOnlyCollection<ProcessFieldResult> fields = workItemType.GetLegacyFields(requestContext);
      foreach (Control control1 in controls)
      {
        Control control = control1;
        if (!string.IsNullOrWhiteSpace(control.Id) && !control.IsContribution)
        {
          ProcessFieldResult field = fields.Where<ProcessFieldResult>((Func<ProcessFieldResult, bool>) (f => TFStringComparer.WorkItemFieldReferenceName.Equals(f.ReferenceName, control.Id))).FirstOrDefault<ProcessFieldResult>();
          if (!LayoutConstants.NonFieldControls.Contains(control.ControlType) && field == null)
            throw new ProcessWorkItemTypeFieldDoesNotExistException(control.Id, workItemType.ReferenceName);
          if (field != null && LayoutConstants.NonCustomizableFields.Contains(field.ReferenceName))
            throw new FieldNotCustomizableException(control.Id);
          control.ControlType = FormLayoutService.GetControlType(field, control);
        }
        else if (control.IsContribution)
        {
          if (!string.IsNullOrWhiteSpace(control.ControlType))
            throw new ContributionControlCannotHaveControlTypeException();
          if (control.Contribution.Inputs != null && control.Contribution.Inputs.Count > contributionInputLimit)
            throw new FormLayoutControlInputsExceededException(control.Contribution.Inputs.Count, contributionInputLimit);
          WebLayoutXmlHelper.ValidateWebLayoutControlContributionInputs(requestContext.GetService<IContributionService>().QueryContribution(requestContext, control.Contribution.ContributionId) ?? throw new ControlContributionNotFoundException(control.Contribution.ContributionId), (Func<string, string[], bool>) ((fieldReferenceName, fieldTypes) =>
          {
            ProcessFieldResult processFieldResult = fields.Where<ProcessFieldResult>((Func<ProcessFieldResult, bool>) (f => TFStringComparer.WorkItemFieldReferenceName.Equals(f.ReferenceName, fieldReferenceName))).FirstOrDefault<ProcessFieldResult>();
            if (processFieldResult == null)
              return false;
            return fieldTypes == null || fieldTypes.Length == 0 || ((IEnumerable<string>) fieldTypes).Contains<string>(processFieldResult.Type.ToString(), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          }), control.Contribution.Inputs);
        }
        if (!LayoutConstants.WideControls.Contains(control.ControlType) && !string.IsNullOrEmpty(control.Label))
          FormValidationUtils.CheckLabel(control.Label);
      }
    }

    private static bool TraceCombineLayoutsException(
      IVssRequestContext requestContext,
      Exception ex)
    {
      requestContext.TraceException(910003, "Services", nameof (FormLayoutService), ex);
      return false;
    }

    private static bool IsInheritedNode(string nodeId) => string.Equals(nodeId, "$inherited", StringComparison.CurrentCultureIgnoreCase);

    private static string GetControlType(ProcessFieldResult field, Control control)
    {
      if (field == null)
        return control?.ControlType ?? string.Empty;
      switch (field.Type)
      {
        case InternalFieldType.DateTime:
          return WellKnownControlNames.DateControl;
        case InternalFieldType.Html:
          return WellKnownControlNames.HtmlControl;
        case InternalFieldType.TreePath:
          return WellKnownControlNames.ClassificationControl;
        default:
          return WellKnownControlNames.FieldControl;
      }
    }

    private void RecordCIForFieldAddOrMove(
      IVssRequestContext requestContext,
      string witRefName,
      LayoutInfo layoutInfo,
      string eventName,
      string sourceGroupId,
      string targetGroupId,
      string controlId,
      int? order)
    {
      try
      {
        Section ancestorOf = layoutInfo.ComposedLayout.FindAncestorOf<Group, Page>(targetGroupId)?.FindAncestorOf<Group, Section>(targetGroupId);
        Group descendant = ancestorOf?.FindDescendant<Group>(targetGroupId);
        int count = descendant != null ? descendant.Children.Count : 0;
        if (descendant != null && !descendant.Children.Contains(descendant.FindDescendant<Control>(controlId)))
          ++count;
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add(nameof (eventName), eventName);
        properties.Add("workItemType", string.Format("[nonemail:{0}]", (object) witRefName));
        properties.Add("section", ancestorOf != null ? ancestorOf.Id : string.Empty);
        properties.Add("controlsInGroup", (double) count);
        properties.Add(nameof (order), order.HasValue ? (double) order.Value : (double) (count - 1));
        properties.Add("isReorderOnly", sourceGroupId != null && sourceGroupId.Equals(targetGroupId, StringComparison.OrdinalIgnoreCase));
        properties.Add("isNewOnForm", sourceGroupId == null);
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "WorkItemService", "FormLayout", properties);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(910018, "Services", nameof (FormLayoutService), ex);
      }
    }

    private void RecordCIForGroupAddOrEdit(
      string action,
      IVssRequestContext requestContext,
      string witRefName,
      LayoutInfo layoutInfo,
      string pageId,
      string sectionId,
      Group group,
      int? order)
    {
      try
      {
        Section descendant = layoutInfo.ComposedLayout.FindDescendant<Page>(pageId)?.FindDescendant<Section>(sectionId);
        int num = descendant != null ? descendant.Children.Count + 1 : -1;
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add("eventName", action);
        properties.Add("workItemType", string.Format("[nonemail:{0}]", (object) witRefName));
        properties.Add("section", sectionId);
        properties.Add("groupsInSection", (double) num);
        properties.Add(nameof (order), order.HasValue ? (double) order.Value : (double) (num - 1));
        properties.Add("visible", (object) group.Visible);
        properties.Add("label", group.Label);
        properties.Add("contribution", group.IsContribution);
        if (group.IsContribution)
          properties.Add("contributionId", group.Contribution.ContributionId);
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "WorkItemService", "FormLayout", properties);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(910018, "Services", nameof (FormLayoutService), ex);
      }
    }

    private void RecordCIForPageOperation(
      IVssRequestContext requestContext,
      string witRefName,
      LayoutInfo layoutInfo,
      string pageId,
      int? order,
      string eventName)
    {
      try
      {
        Page descendant = layoutInfo.ComposedLayout.FindDescendant<Page>(pageId);
        int num = layoutInfo.ComposedLayout.GetDescendants<Page>().Count<Page>();
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add(nameof (eventName), eventName);
        properties.Add("workItemType", string.Format("[nonemail:{0}]", (object) witRefName));
        properties.Add(nameof (order), order.HasValue ? (double) order.Value : (double) (num - 1));
        if (descendant != null)
        {
          properties.Add("visible", (object) descendant.Visible);
          properties.Add("label", descendant.Label);
          properties.Add("contribution", descendant.IsContribution);
          if (descendant.IsContribution)
            properties.Add("contributionId", descendant.Contribution?.ContributionId);
        }
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "WorkItemService", "FormLayout", properties);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(910018, "Services", nameof (FormLayoutService), ex);
      }
    }

    private static bool IsSealedGroup(Group group) => group.Children.Count == 1 && LayoutConstants.WideControls.Contains(group.Children[0].ControlType);

    private void SetLayoutLimits(IVssRequestContext requestContext)
    {
      this.SetLimits(requestContext);
      requestContext.GetService<IVssRegistryService>().RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), "/Service/FormLayout/Settings/...");
    }

    private void OnRegistryChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      if (!changedEntries.Any<RegistryEntry>())
        return;
      this.SetLimits(requestContext);
    }

    private void SetLimits(IVssRequestContext requestContext)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      this.m_maxControlsAllowed = service.GetValue<int>(requestContext, (RegistryQuery) "/Service/FormLayout/Settings/MaxControlsAllowed", true, 32);
      this.m_maxGroupsAllowed = service.GetValue<int>(requestContext, (RegistryQuery) "/Service/FormLayout/Settings/MaxGroupsAllowed", true, 32);
      this.m_maxPagesAllowed = service.GetValue<int>(requestContext, (RegistryQuery) "/Service/FormLayout/Settings/MaxPagesAllowed", true, 16);
    }

    private void CheckControlsLimit(string controlId, string groupId, Layout combinedLayout)
    {
      Group descendant = combinedLayout.FindDescendant<Group>(groupId);
      if (descendant != null && descendant.Children.Count >= this.m_maxControlsAllowed && descendant.FindDescendant<Control>(controlId) == null)
        throw new FormLayoutControlsLimitExceededException(controlId, groupId, this.m_maxControlsAllowed);
    }

    private void CheckControlAlreadyExists(string controlId, string groupId, Layout combinedLayout)
    {
      Group descendant = combinedLayout.FindDescendant<Group>(groupId);
      if (descendant != null && descendant.FindDescendant<Control>(controlId) != null)
        throw new FormLayoutControlAlreadyExistInGroupException(controlId, descendant.Label);
    }

    private void CheckGroupsLimit(
      string groupLabel,
      string pageId,
      string sectionId,
      Layout combinedLayout)
    {
      Page descendant1 = combinedLayout.FindDescendant<Page>(pageId);
      if (descendant1 == null)
        return;
      Section descendant2 = descendant1.FindDescendant<Section>(sectionId);
      if (descendant2 != null && descendant2.Children.Count >= this.m_maxGroupsAllowed)
        throw new FormLayoutGroupsLimitExceededException(groupLabel, sectionId, this.m_maxGroupsAllowed);
    }

    private void CheckPagesLimit(string pageLabel, Layout combinedLayout)
    {
      if (combinedLayout.GetDescendants<Page>().Count<Page>() >= this.m_maxPagesAllowed)
        throw new FormLayoutPagesLimitExceededException(pageLabel, this.m_maxPagesAllowed);
    }
  }
}

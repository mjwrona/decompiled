// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.IFormLayoutService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout
{
  [DefaultServiceImplementation(typeof (FormLayoutService))]
  public interface IFormLayoutService : IVssFrameworkService
  {
    LayoutInfo GetLayout(IVssRequestContext requestContext, Guid processId, string witRefName);

    Layout CombineLayouts(IVssRequestContext requestContext, Layout baseLayout, Layout deltaLayout);

    Layout SetFieldControlInGroup(
      IVssRequestContext requestContext,
      Guid processId,
      string witRefName,
      string groupId,
      Control control,
      int? order,
      bool isEdit = false);

    Layout RemoveFieldControlFromGroup(
      IVssRequestContext requestContext,
      Guid processId,
      string witRefName,
      string groupId,
      string controlId);

    Layout MoveFieldControlToGroup(
      IVssRequestContext requestContext,
      Guid processId,
      string witRefName,
      string sourceGroupId,
      string targetGroupId,
      Control control,
      int? order);

    Layout RemoveFieldFromLayout(
      IVssRequestContext requestContext,
      Guid processId,
      string witRefName,
      string fieldRefName,
      bool suppressPermissionCheck = false);

    Layout AddGroup(
      IVssRequestContext requestContext,
      Guid processId,
      string witRefName,
      string pageId,
      string sectionId,
      Group group,
      int? order);

    Layout EditGroup(
      IVssRequestContext requestContext,
      Guid processId,
      string witRefName,
      string pageId,
      string sectionId,
      Group group,
      int? order);

    Layout RemoveGroup(
      IVssRequestContext requestContext,
      Guid processId,
      string witRefName,
      string pageId,
      string sectionId,
      string groupId);

    Layout SetGroupInSection(
      IVssRequestContext requestContext,
      Guid processId,
      string witRefName,
      string pageId,
      string sourceSectionId,
      string targetSectionId,
      Group group,
      int? order);

    Layout SetGroupInPage(
      IVssRequestContext requestContext,
      Guid processId,
      string witRefName,
      string sourcePageId,
      string targetPageId,
      string sourceSectionId,
      string targetSectionId,
      Group group,
      int? order);

    Layout AddPage(
      IVssRequestContext requestContext,
      Guid processId,
      string witRefName,
      Page page,
      int? order);

    Layout EditPage(
      IVssRequestContext requestContext,
      Guid processId,
      string witRefName,
      Page page,
      int? order);

    Layout RemovePage(
      IVssRequestContext requestContext,
      Guid processId,
      string witRefName,
      string pageId);

    Layout AddorEditSystemControls(
      IVssRequestContext requestContext,
      Guid processId,
      string witRefName,
      ISet<Control> systemControls);

    Layout RemoveSystemControls(
      IVssRequestContext requestContext,
      Guid processId,
      string witRefName,
      ISet<string> systemControlIds);
  }
}

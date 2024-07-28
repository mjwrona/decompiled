// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.ILayoutOperations
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout
{
  public interface ILayoutOperations
  {
    Layout BuildCombinedLayout(
      IVssRequestContext requestContext,
      Layout baseLayout,
      Layout deltaLayout);

    Layout PutControlInExistingGroup(
      string witRefName,
      Layout composedLayout,
      Layout deltaLayout,
      string groupId,
      Control control,
      int? order);

    Layout RemoveControlFromGroup(
      string witRefName,
      Layout composedLayout,
      Layout deltaLayout,
      string groupId,
      string fieldRefName);

    Layout RemoveControlFromLayout(
      string witRefName,
      Layout composedLayout,
      Layout deltaLayout,
      string fieldRefName);

    Layout AddOrEditSystemControls(Layout deltaLayout, ISet<Control> updatedSystemControls);

    Layout RemoveSystemControls(Layout deltaLayout, ISet<string> systemControlIds);

    Layout AddGroup(
      string witRefName,
      Layout composedLayout,
      Layout baseLayout,
      Layout deltaLayout,
      string pageId,
      string sectionId,
      Group group,
      int? order);

    Layout EditGroup(
      string witRefName,
      Layout composedLayout,
      Layout baseLayout,
      Layout deltaLayout,
      string pageId,
      string sectionId,
      Group group,
      int? order);

    Layout RemoveGroup(
      string witRefName,
      Layout composedLayout,
      Layout deltaLayout,
      string pageId,
      string sectionId,
      string groupId,
      bool ignoreChildren = false);

    Layout AddPage(
      string witRefName,
      Layout composedLayout,
      Layout deltaLayout,
      Page page,
      int? order);

    Layout EditPage(
      string witRefName,
      Layout composedLayout,
      Layout baseLayout,
      Layout deltaLayout,
      Page page,
      int? order);

    Layout RemovePage(string witRefName, Layout deltaLayout, string pageId);

    Section EnsureInheritSection(Layout layout);
  }
}

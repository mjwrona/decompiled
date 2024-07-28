// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata.IWorkItemRulesService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata
{
  [DefaultServiceImplementation(typeof (WorkItemRulesService))]
  public interface IWorkItemRulesService : IVssFrameworkService
  {
    bool TryGetOutOfBoxRulesAndHelpTexts(
      IVssRequestContext requestContext,
      ProcessDescriptor processDescriptor,
      string workItemTypeReferenceName,
      out IReadOnlyCollection<WorkItemFieldRule> rules,
      out IReadOnlyCollection<HelpTextDescriptor> helpTexts);

    bool TryGetOutOfBoxRules(
      IVssRequestContext requestContext,
      ProcessDescriptor processDescriptor,
      string workItemTypeReferenceName,
      out IReadOnlyCollection<WorkItemFieldRule> rules);

    WorkItemRulesAndHelpTextsDescriptor GetOutOfBoxRulesAndHelpTexts(
      IVssRequestContext requestContext,
      ProcessDescriptor processDescriptor,
      string workItemTypeReferenceName);

    IReadOnlyCollection<WorkItemFieldRule> GetOutOfBoxRules(
      IVssRequestContext requestContext,
      ProcessDescriptor processDescriptor,
      string workItemTypeReferenceName);

    bool TryGetOutOfBoxRulesForWorkItemTypeField(
      IVssRequestContext requestContext,
      ProcessDescriptor processDescriptor,
      string workItemTypeReferenceName,
      string fieldReferenceName,
      out WorkItemFieldRule fieldRule);
  }
}

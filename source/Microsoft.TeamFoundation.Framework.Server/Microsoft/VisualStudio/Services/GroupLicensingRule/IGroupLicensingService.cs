// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.GroupLicensingRule.IGroupLicensingService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Licensing;
using Microsoft.VisualStudio.Services.Operations;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.GroupLicensingRule
{
  [DefaultServiceImplementation(typeof (FrameworkGroupLicensingService))]
  public interface IGroupLicensingService : IVssFrameworkService
  {
    IEnumerable<Microsoft.VisualStudio.Services.GroupLicensingRule.GroupLicensingRule> GetGroupLicensingRules(
      IVssRequestContext requestContext,
      int top,
      int skip = 0);

    IEnumerable<Microsoft.VisualStudio.Services.GroupLicensingRule.GroupLicensingRule> GetGroupLicensingRules(
      IVssRequestContext requestContext,
      IEnumerable<SubjectDescriptor> subjectDescriptors);

    Microsoft.VisualStudio.Services.GroupLicensingRule.GroupLicensingRule GetGroupLicensingRule(
      IVssRequestContext requestContext,
      SubjectDescriptor subjectDescriptor);

    OperationReference AddGroupLicensingRule(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.GroupLicensingRule.GroupLicensingRule licensingRule,
      RuleOption? ruleOption = null);

    OperationReference UpdateGroupLicensingRule(
      IVssRequestContext requestContext,
      GroupLicensingRuleUpdate licensingRuleUpdate,
      RuleOption? ruleOption = null);

    OperationReference DeleteGroupLicensingRule(
      IVssRequestContext requestContext,
      SubjectDescriptor subjectDescriptor,
      RuleOption? ruleOption = null);

    AccountEntitlement ApplyGroupLicensingRulesToUser(IVssRequestContext requestContext);

    AccountEntitlement ApplyGroupLicensingRulesToUser(
      IVssRequestContext requestContext,
      SubjectDescriptor identityDescriptor);

    AccountEntitlement ApplyGroupLicensingRulesToUser(
      IVssRequestContext requestContext,
      Guid identityId);

    OperationReference ApplyGroupLicensingRulesToAllUsers(
      IVssRequestContext requestContext,
      RuleOption? ruleOption = null);

    ApplicationStatus GetApplicationStatus(IVssRequestContext requestContext, Guid? operationId = null);

    void RemoveDirectAssignment(
      IVssRequestContext requestContext,
      SubjectDescriptor identityDescriptor);

    void RemoveDirectAssignment(IVssRequestContext requestContext, Guid identityId);
  }
}

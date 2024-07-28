// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules.IRuleEvaluationContext
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Identity;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public interface IRuleEvaluationContext
  {
    IVssRequestContext RequestContext { get; }

    IWorkItemRuleFilter RuleFilter { get; }

    IServerDefaultValueTransformer ServerDefaultValueTransformer { get; }

    int? FirstInvalidFieldId { get; }

    int? FirstFieldRequiresPendingCheck { get; }

    IDictionary<int, FieldRuleEvalutionStatus> RuleEvaluationStatuses { get; }

    object GetCurrentFieldValue(int fieldId);

    object GetOriginalFieldValue(int fieldId);

    bool TryComputeFieldValue(int fieldId, int parentField, out object value);

    Microsoft.VisualStudio.Services.Identity.Identity GetCurrentIdentity();

    string GetCurrentUser();

    InternalFieldType GetFieldType(int fieldId);

    bool IsIdentityField(int fieldId);

    void SetFieldRuleEvalutionStatus(FieldRuleEvalutionStatus status);

    FieldStatusFlags GetFieldFlags(int fieldId);

    bool IsAreaPathValid(string path);

    bool IsIterationPathValid(string path);

    void ClearPendingListChecks(IEnumerable<int> exceptFieldIds);

    string ResolveIdentityValue(int fieldId, object value);

    bool IsRealIdentity(string value);

    bool IsIdentityMemberOfGroup(string value, ConstantSetReference setReference);

    void UpdateIdentityMap(IDictionary<string, Microsoft.VisualStudio.Services.Identity.Identity> identityMap);

    void SetResolvedIdentityNamesInfo(
      ResolvedIdentityNamesInfo resolvedIdentityNamesInfo);

    bool IsIdentityFieldValueAmbiguous(object value);

    Microsoft.VisualStudio.Services.Identity.Identity[] GetAmbiguousIdentities(object value);

    IdentityDisplayType GetIdentityDisplayType();
  }
}

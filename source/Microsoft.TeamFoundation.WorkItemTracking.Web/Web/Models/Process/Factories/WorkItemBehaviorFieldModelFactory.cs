// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Process.Factories.WorkItemBehaviorFieldModelFactory
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common.Converters;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Process.Factories
{
  internal static class WorkItemBehaviorFieldModelFactory
  {
    internal static WorkItemBehaviorField Create(
      IVssRequestContext requestContext,
      KeyValuePair<string, FieldEntry> behaviorField)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<FieldEntry>(behaviorField.Value, "Value");
      return new WorkItemBehaviorField()
      {
        BehaviorFieldId = behaviorField.Key,
        Id = behaviorField.Value.ReferenceName
      };
    }

    internal static WorkItemBehaviorField Create(
      IVssRequestContext requestContext,
      KeyValuePair<string, ProcessFieldDefinition> legacyBehaviorField)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ProcessFieldDefinition>(legacyBehaviorField.Value, "Value");
      return new WorkItemBehaviorField()
      {
        BehaviorFieldId = legacyBehaviorField.Key,
        Id = legacyBehaviorField.Value.ReferenceName,
        Url = WitUrlHelper.GetFieldUrl(requestContext, legacyBehaviorField.Value.ReferenceName)
      };
    }

    internal static IEnumerable<WorkItemBehaviorField> Create(
      IVssRequestContext requestContext,
      IReadOnlyDictionary<string, FieldEntry> behaviorFields)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IReadOnlyDictionary<string, FieldEntry>>(behaviorFields, nameof (behaviorFields));
      return behaviorFields.Select<KeyValuePair<string, FieldEntry>, WorkItemBehaviorField>((Func<KeyValuePair<string, FieldEntry>, WorkItemBehaviorField>) (f => WorkItemBehaviorFieldModelFactory.Create(requestContext, f)));
    }

    internal static IEnumerable<WorkItemBehaviorField> Create(
      IVssRequestContext requestContext,
      IReadOnlyDictionary<string, ProcessFieldDefinition> legacyBehaviorFields)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IReadOnlyDictionary<string, ProcessFieldDefinition>>(legacyBehaviorFields, nameof (legacyBehaviorFields));
      return legacyBehaviorFields.Select<KeyValuePair<string, ProcessFieldDefinition>, WorkItemBehaviorField>((Func<KeyValuePair<string, ProcessFieldDefinition>, WorkItemBehaviorField>) (f => WorkItemBehaviorFieldModelFactory.Create(requestContext, f)));
    }
  }
}

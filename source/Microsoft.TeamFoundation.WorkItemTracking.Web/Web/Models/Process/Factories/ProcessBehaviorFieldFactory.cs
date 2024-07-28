// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Process.Factories.ProcessBehaviorFieldFactory
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
  internal static class ProcessBehaviorFieldFactory
  {
    internal static ProcessBehaviorField Create(
      IVssRequestContext requestContext,
      KeyValuePair<string, FieldEntry> behaviorField)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<FieldEntry>(behaviorField.Value, "Value");
      return new ProcessBehaviorField()
      {
        Name = behaviorField.Value.Name,
        ReferenceName = behaviorField.Value.ReferenceName
      };
    }

    internal static ProcessBehaviorField Create(
      IVssRequestContext requestContext,
      KeyValuePair<string, ProcessFieldDefinition> legacyBehaviorField)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ProcessFieldDefinition>(legacyBehaviorField.Value, "Value");
      return new ProcessBehaviorField()
      {
        Name = legacyBehaviorField.Value.Name,
        ReferenceName = legacyBehaviorField.Value.ReferenceName,
        Url = WitUrlHelper.GetFieldUrl(requestContext, legacyBehaviorField.Value.ReferenceName)
      };
    }

    internal static IEnumerable<ProcessBehaviorField> Create(
      IVssRequestContext requestContext,
      IReadOnlyDictionary<string, FieldEntry> behaviorFields)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IReadOnlyDictionary<string, FieldEntry>>(behaviorFields, nameof (behaviorFields));
      return behaviorFields.Select<KeyValuePair<string, FieldEntry>, ProcessBehaviorField>((Func<KeyValuePair<string, FieldEntry>, ProcessBehaviorField>) (f => ProcessBehaviorFieldFactory.Create(requestContext, f)));
    }

    internal static IEnumerable<ProcessBehaviorField> Create(
      IVssRequestContext requestContext,
      IReadOnlyDictionary<string, ProcessFieldDefinition> legacyBehaviorFields)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IReadOnlyDictionary<string, ProcessFieldDefinition>>(legacyBehaviorFields, nameof (legacyBehaviorFields));
      return legacyBehaviorFields.Select<KeyValuePair<string, ProcessFieldDefinition>, ProcessBehaviorField>((Func<KeyValuePair<string, ProcessFieldDefinition>, ProcessBehaviorField>) (f => ProcessBehaviorFieldFactory.Create(requestContext, f)));
    }
  }
}

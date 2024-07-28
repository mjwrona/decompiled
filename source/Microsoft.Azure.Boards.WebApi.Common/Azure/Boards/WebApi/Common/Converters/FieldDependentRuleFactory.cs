// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WebApi.Common.Converters.FieldDependentRuleFactory
// Assembly: Microsoft.Azure.Boards.WebApi.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC99C479-6852-4E74-BCA4-2660760F9D83
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.WebApi.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Boards.WebApi.Common.Converters
{
  public static class FieldDependentRuleFactory
  {
    public static FieldDependentRule GetDependentFields(
      WorkItemTrackingRequestContext witRequestContext,
      Guid projectId,
      string type,
      string fieldName)
    {
      if (string.IsNullOrEmpty(fieldName))
        throw new VssPropertyValidationException(nameof (fieldName), ResourceStrings.NullOrEmptyParameter((object) nameof (fieldName)));
      if (string.IsNullOrEmpty(type))
        throw new VssPropertyValidationException(nameof (type), ResourceStrings.NullOrEmptyParameter((object) nameof (type)));
      FieldEntry field;
      if (!witRequestContext.FieldDictionary.TryGetField(fieldName, out field))
        throw new WorkItemTrackingFieldDefinitionNotFoundException(fieldName);
      Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType typeByReferenceName = witRequestContext.RequestContext.GetService<IWorkItemTypeService>().GetWorkItemTypeByReferenceName(witRequestContext.RequestContext, projectId, type);
      IEnumerable<WorkItemFieldReference> source = typeByReferenceName.GetAdditionalProperties(witRequestContext.RequestContext).GetDependentFieldRule(witRequestContext, field.FieldId).Select<FieldEntry, WorkItemFieldReference>((Func<FieldEntry, WorkItemFieldReference>) (fieldEntry => WorkItemFieldReferenceFactory.Create(witRequestContext, fieldEntry)));
      ReferenceLinks referenceLinks = new ReferenceLinks();
      referenceLinks.AddLink("parent", WitUrlHelper.GetWorkItemTypeUrl(witRequestContext, typeByReferenceName));
      referenceLinks.AddLink("self", WitUrlHelper.GetWorkItemTypeFieldUrl(witRequestContext, projectId, typeByReferenceName.Name, fieldName));
      FieldDependentRule dependentFields = new FieldDependentRule();
      dependentFields.DependentFields = source.Any<WorkItemFieldReference>() ? source : (IEnumerable<WorkItemFieldReference>) null;
      dependentFields.Links = referenceLinks;
      return dependentFields;
    }
  }
}

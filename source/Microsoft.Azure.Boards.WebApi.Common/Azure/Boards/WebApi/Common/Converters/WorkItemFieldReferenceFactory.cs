// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WebApi.Common.Converters.WorkItemFieldReferenceFactory
// Assembly: Microsoft.Azure.Boards.WebApi.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC99C479-6852-4E74-BCA4-2660760F9D83
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.WebApi.Common.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Boards.WebApi.Common.Converters
{
  public static class WorkItemFieldReferenceFactory
  {
    public static WorkItemFieldReference Create(
      WorkItemTrackingRequestContext witRequestContext,
      FieldEntry field,
      ISecuredObject securedObject = null,
      Guid? projectId = null,
      bool returnProjectScopedUrl = true,
      bool includeUrl = true)
    {
      return new WorkItemFieldReference(securedObject)
      {
        ReferenceName = field.ReferenceName,
        Name = field.Name,
        Url = includeUrl ? WitUrlHelper.GetFieldUrl(witRequestContext.RequestContext, field.ReferenceName, projectId, returnProjectScopedUrl) : (string) null
      };
    }

    public static WorkItemFieldReference Create(
      WorkItemTrackingRequestContext witRequestContext,
      string fieldName,
      ISecuredObject securedObject = null,
      Guid? projectId = null,
      bool returnProjectScopedUrl = true,
      bool includeUrls = true)
    {
      FieldEntry field;
      return witRequestContext.FieldDictionary.TryGetField(fieldName, out field) ? WorkItemFieldReferenceFactory.Create(witRequestContext, field, securedObject, projectId, returnProjectScopedUrl, includeUrls) : (WorkItemFieldReference) null;
    }

    public static IEnumerable<WorkItemFieldReference> Create(
      WorkItemTrackingRequestContext witRequestContext,
      IEnumerable<string> displayFields,
      ISecuredObject securedObject = null,
      Guid? projectId = null,
      bool returnProjectScopedUrl = true,
      bool includeUrls = true)
    {
      IEnumerable<WorkItemFieldReference> source = displayFields.Select<string, WorkItemFieldReference>((Func<string, WorkItemFieldReference>) (fld => WorkItemFieldReferenceFactory.Create(witRequestContext, fld, securedObject, projectId, returnProjectScopedUrl, includeUrls)));
      return source.Any<WorkItemFieldReference>() ? source : (IEnumerable<WorkItemFieldReference>) null;
    }
  }
}

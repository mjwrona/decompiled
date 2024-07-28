// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.IWorkItemType
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public interface IWorkItemType
  {
    string Description { get; }

    FieldDefinitionCollection GetFields(IVssRequestContext requestContext);

    AdditionalWorkItemTypeProperties GetExtendedProperties(IVssRequestContext requestContext);

    List<string> GetOrderedStates(IVssRequestContext requestContext);

    int? Id { get; }

    string Name { get; }

    string ReferenceName { get; }

    int ProjectId { get; }

    string Color { get; }

    IReadOnlyCollection<int> Fields { get; }
  }
}

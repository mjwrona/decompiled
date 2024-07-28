// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.IBehaviorService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  [DefaultServiceImplementation(typeof (BehaviorService))]
  public interface IBehaviorService : IVssFrameworkService
  {
    IReadOnlyCollection<Behavior> GetBehaviors(
      IVssRequestContext requestContext,
      Guid processId,
      bool includeParent = true,
      bool bypassCache = false);

    Behavior GetBehavior(
      IVssRequestContext requestContext,
      Guid processId,
      string behaviorReferenceName,
      bool includeParent = true,
      bool bypassCache = false);

    DeletedBehaviors GetBehaviorReferenceNamesDeletedSince(
      IVssRequestContext requestContext,
      DateTime deletedSince);

    bool TryGetBehavior(
      IVssRequestContext requestContext,
      Guid processId,
      string behaviorReferenceName,
      out Behavior behavior,
      bool includeParent = true,
      bool bypassCache = false);

    IReadOnlyCollection<Behavior> GetPortfolioBehaviors(
      IVssRequestContext requestContext,
      Guid processId);

    Behavior CreateBehavior(
      IVssRequestContext requestContext,
      Guid processId,
      string parentBehaviorRefName,
      string name,
      string color = null,
      string categoryRefrenceName = null);

    Behavior UpdateBehavior(
      IVssRequestContext requestContext,
      Guid processId,
      string behaviorReferenceName,
      string name,
      string color = null);

    void DeleteBehavior(
      IVssRequestContext requestContext,
      Guid processId,
      string behaviorReferenceName);

    IReadOnlyCollection<Behavior> GetBacklogBehaviors(
      IVssRequestContext requestContext,
      Guid processId);

    IDictionary<string, bool> IsSystemBehavior(
      IVssRequestContext requestContext,
      IReadOnlyCollection<string> behaviorReferenceNames);
  }
}

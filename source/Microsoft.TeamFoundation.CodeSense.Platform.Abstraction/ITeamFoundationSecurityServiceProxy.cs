// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Platform.Abstraction.ITeamFoundationSecurityServiceProxy
// Assembly: Microsoft.TeamFoundation.CodeSense.Platform.Abstraction, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7627AC5C-7AFD-416A-A79B-D03A392C9E3B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Platform.Abstraction.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.CodeSense.Platform.Abstraction
{
  [DefaultServiceImplementation(typeof (TeamFoundationSecurityServiceProxy))]
  public interface ITeamFoundationSecurityServiceProxy : IVssFrameworkService
  {
    void CheckPermissions(
      IVssRequestContext requestContext,
      Guid securityNamespaceGuid,
      string token,
      int permissions);

    bool HasPermissions(
      IVssRequestContext requestContext,
      Guid securityNamespaceGuid,
      string token,
      int permissions);

    Dictionary<string, bool> ObtainPermissions(
      IVssRequestContext requestContext,
      Guid securityNamespaceGuid,
      IEnumerable<string> tokens,
      int permissions);

    IDictionary<int, bool> GetWorkItemPermissions(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds);
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AuditLog.IAuditLogService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server.AuditLog
{
  [DefaultServiceImplementation(typeof (NullAuditLogService))]
  public interface IAuditLogService : IVssFrameworkService
  {
    void Log(
      IVssRequestContext requestContext,
      string actionId,
      IDictionary<string, object> data,
      Guid targetHostId = default (Guid),
      Guid projectId = default (Guid));

    void Log(
      IVssRequestContext requestContext,
      string actionId,
      IDictionary<string, object> data,
      AuditLogContextOverride contextOverride);

    void HandlePostLog(IVssRequestContext requestContext);
  }
}

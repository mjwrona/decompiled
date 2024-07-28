// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.UserReportedConcernService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Gallery.Server.Components;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public class UserReportedConcernService : IUserReportedConcernService, IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public IEnumerable<UserReportedConcern> GetUserReportedConcernByUserId(
      IVssRequestContext requestContext,
      Guid userId)
    {
      ArgumentUtility.CheckForEmptyGuid(userId, nameof (userId));
      using (UserReportedConcernComponent component = requestContext.CreateComponent<UserReportedConcernComponent>())
        return component.GetUserReportedConcernByUserId(userId);
    }

    public int AnonymizeUserReportedConcern(IVssRequestContext requestContext, Guid userId)
    {
      ArgumentUtility.CheckForEmptyGuid(userId, nameof (userId));
      using (UserReportedConcernComponent component = requestContext.CreateComponent<UserReportedConcernComponent>())
        return component.AnonymizeUserReportedConcern(userId);
    }
  }
}

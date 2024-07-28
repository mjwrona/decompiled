// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.ILicensingMitigationService
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Account;
using System;

namespace Microsoft.VisualStudio.Services.Licensing
{
  [DefaultServiceImplementation(typeof (LicensingMitigationService))]
  public interface ILicensingMitigationService : IVssFrameworkService
  {
    void DeleteUserLicense(IVssRequestContext requestContext, Guid userId);

    void UpdateUserLicense(
      IVssRequestContext requestContext,
      Guid userId,
      LicensingSource? source,
      LicensingOrigin? origin,
      int? license,
      AssignmentSource? assignmentSource,
      AccountUserStatus? status,
      DateTimeOffset? assignmentDate,
      DateTimeOffset? dateCreated,
      DateTimeOffset? lastUpdated,
      DateTimeOffset? lastAccessed);
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Users.Server.IUserIdentifierConversionCacheService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.Users.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [DefaultServiceImplementation(typeof (UserIdentifierConversionCacheService))]
  public interface IUserIdentifierConversionCacheService : IVssFrameworkService
  {
    Guid GetStorageKey(IVssRequestContext deploymentContext, SubjectDescriptor subjectDescriptor);

    SubjectDescriptor GetSubjectDescriptor(IVssRequestContext deploymentContext, Guid storageKey);

    void Set(
      IVssRequestContext deploymentContext,
      SubjectDescriptor subjectDescriptor,
      Guid storageKey);

    void Remove(
      IVssRequestContext deploymentContext,
      SubjectDescriptor subjectDescriptor = default (SubjectDescriptor),
      Guid storageKey = default (Guid));
  }
}

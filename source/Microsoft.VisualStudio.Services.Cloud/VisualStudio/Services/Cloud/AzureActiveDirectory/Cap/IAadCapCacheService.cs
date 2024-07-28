// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Cap.IAadCapCacheService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Cap
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [DefaultServiceImplementation(typeof (AadCapCacheService))]
  public interface IAadCapCacheService : IVssFrameworkService
  {
    AadCapResult CheckIsUserConditionAllowed(
      IVssRequestContext context,
      Guid tenantId,
      SubjectDescriptor subjectDescriptor,
      string ip);

    void Set(
      IVssRequestContext requestContext,
      SubjectDescriptor subjectDescriptor,
      string clientIp,
      AadCapResult capResult);
  }
}

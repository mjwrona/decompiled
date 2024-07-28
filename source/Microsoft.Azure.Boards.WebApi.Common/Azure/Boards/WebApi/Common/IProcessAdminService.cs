// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WebApi.Common.IProcessAdminService
// Assembly: Microsoft.Azure.Boards.WebApi.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC99C479-6852-4E74-BCA4-2660760F9D83
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.WebApi.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Azure.Boards.WebApi.Common
{
  [DefaultServiceImplementation(typeof (ProcessAdminService))]
  public interface IProcessAdminService : IVssFrameworkService
  {
    IEnumerable<ProcessDescriptorModel> GetProcesses(IVssRequestContext context);

    ProcessUpdateResultModel UpdateProcess(
      IVssRequestContext context,
      Stream content,
      bool confirmWarnings);

    void DeleteProcess(IVssRequestContext context, Guid templateTypeId);

    ProcessUpdateProgressModel MonitorUpdateProgress(IVssRequestContext context, Guid promoteJobId);

    ProcessFieldUsageInfo GetProcessFieldUsages(
      IVssRequestContext requestContext,
      Guid processTypeId);
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.ProcessTemplates.IProcessTemplateValidatorService
// Assembly: Microsoft.Azure.Boards.ProcessTemplates, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A94E8BA8-9851-4F5D-B619-9CF2FFF5B128
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.ProcessTemplates.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Azure.Boards.ProcessTemplates
{
  [DefaultServiceImplementation(typeof (ProcessTemplateValidatorService))]
  public interface IProcessTemplateValidatorService : IVssFrameworkService
  {
    void ValidateTemplateFileSizeLimit(IVssRequestContext requestContext, Stream zipContentStream);

    ProcessTemplateValidatorResult Validate(
      IVssRequestContext requestContext,
      Stream zipTemplateContent,
      bool isTfsMigratorValidationContext = false,
      Dictionary<string, object> optionalParameters = null);
  }
}

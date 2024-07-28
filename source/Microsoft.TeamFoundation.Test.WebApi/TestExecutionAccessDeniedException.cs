// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Test.WebApi.TestExecutionAccessDeniedException
// Assembly: Microsoft.TeamFoundation.Test.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17829F78-DAC0-47C1-AC4C-95D401C54448
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Test.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Test.WebApi
{
  [ExceptionMapping("0.0", "3.0", "TestExecutionAccessDeniedException", "Microsoft.TeamFoundation.Test.WebApi.TestExecutionAccessDeniedException, Microsoft.TeamFoundation.Test.WebApi, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public class TestExecutionAccessDeniedException : TestExecutionServiceException
  {
    public TestExecutionAccessDeniedException(string message)
      : base(message)
    {
    }
  }
}

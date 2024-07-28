// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.BuildControllerNotFoundException
// Assembly: Microsoft.TeamFoundation.Build.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 97B7A530-2EF1-42C1-8A2A-360BCF05C7EF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [ExceptionMapping("0.0", "3.0", "BuildControllerNotFoundException", "Microsoft.TeamFoundation.Build.WebApi.BuildControllerNotFoundException, Microsoft.TeamFoundation.Build.WebApi, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public class BuildControllerNotFoundException : VssServiceException
  {
    public BuildControllerNotFoundException(string message)
      : base(message)
    {
    }

    public BuildControllerNotFoundException(string message, Exception ex)
      : base(message, ex)
    {
    }
  }
}

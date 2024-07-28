// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.CannotUpdateTriggeredByBuildException
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [ExceptionMapping("0.0", "3.0", "CannotUpdateTriggeredByBuildException", "Microsoft.TeamFoundation.Build.WebApi.CannotUpdateTriggeredByBuildException, Microsoft.TeamFoundation.Build2.WebApi, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public class CannotUpdateTriggeredByBuildException : BuildException
  {
    public CannotUpdateTriggeredByBuildException(string message)
      : base(message)
    {
    }

    public CannotUpdateTriggeredByBuildException(string message, Exception ex)
      : base(message, ex)
    {
    }

    protected CannotUpdateTriggeredByBuildException(
      SerializationInfo info,
      StreamingContext context)
      : base(info, context)
    {
    }
  }
}

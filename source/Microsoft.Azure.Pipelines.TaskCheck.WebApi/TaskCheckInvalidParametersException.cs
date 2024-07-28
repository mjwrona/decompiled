// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.TaskCheck.WebApi.TaskCheckInvalidParametersException
// Assembly: Microsoft.Azure.Pipelines.TaskCheck.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7E88E420-FA63-4A56-A903-50B247686E79
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.TaskCheck.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.TaskCheck.WebApi
{
  [Serializable]
  public sealed class TaskCheckInvalidParametersException : VssServiceException
  {
    public TaskCheckInvalidParametersException(string message)
      : base(message)
    {
    }

    public TaskCheckInvalidParametersException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    private TaskCheckInvalidParametersException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}

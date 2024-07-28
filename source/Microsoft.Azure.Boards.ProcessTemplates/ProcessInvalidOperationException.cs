// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.ProcessTemplates.ProcessInvalidOperationException
// Assembly: Microsoft.Azure.Boards.ProcessTemplates, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A94E8BA8-9851-4F5D-B619-9CF2FFF5B128
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.ProcessTemplates.dll

using System;

namespace Microsoft.Azure.Boards.ProcessTemplates
{
  [Serializable]
  public class ProcessInvalidOperationException : ProcessServiceException
  {
    public ProcessInvalidOperationException(string message)
      : base(message, (Exception) null)
    {
    }

    public ProcessInvalidOperationException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}

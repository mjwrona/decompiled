// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.ProcessTemplates.ProcessTemplateValidatorException
// Assembly: Microsoft.Azure.Boards.ProcessTemplates, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A94E8BA8-9851-4F5D-B619-9CF2FFF5B128
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.ProcessTemplates.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.Azure.Boards.ProcessTemplates
{
  public abstract class ProcessTemplateValidatorException : TeamFoundationServiceException
  {
    public string TemplateFile { get; set; }

    public ProcessTemplateValidatorException(string message, string fileName, int errorCode)
      : this(message, fileName, errorCode, (Exception) null)
    {
    }

    public ProcessTemplateValidatorException(
      string message,
      string fileName,
      int errorCode,
      Exception innerException)
      : base(message, innerException)
    {
      this.ErrorCode = errorCode;
      this.TemplateFile = fileName;
    }
  }
}

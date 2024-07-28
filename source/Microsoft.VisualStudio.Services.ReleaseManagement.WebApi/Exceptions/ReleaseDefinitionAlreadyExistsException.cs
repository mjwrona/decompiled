// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Exceptions.ReleaseDefinitionAlreadyExistsException
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Exceptions
{
  [Serializable]
  public class ReleaseDefinitionAlreadyExistsException : VssServiceException
  {
    public ReleaseDefinitionAlreadyExistsException()
    {
    }

    public ReleaseDefinitionAlreadyExistsException(string message)
      : base(message)
    {
    }

    public ReleaseDefinitionAlreadyExistsException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected ReleaseDefinitionAlreadyExistsException(
      SerializationInfo info,
      StreamingContext context)
      : base(info, context)
    {
    }
  }
}

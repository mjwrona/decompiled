// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.MachineManagementException
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  [ExceptionMapping("0.0", "3.0", "MachineManagementException", "Microsoft.VisualStudio.Services.MachineManagement.WebApi.MachineManagementException, Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public class MachineManagementException : VssServiceException
  {
    public MachineManagementException(string message)
      : base(message)
    {
    }

    public MachineManagementException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected MachineManagementException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}

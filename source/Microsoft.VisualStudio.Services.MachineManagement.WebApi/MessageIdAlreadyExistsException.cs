// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.MessageIdAlreadyExistsException
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  [ExceptionMapping("0.0", "3.0", "MessageIdAlreadyExistsException", "Microsoft.VisualStudio.Services.MachineManagement.WebApi.MessageIdAlreadyExistsException, Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public sealed class MessageIdAlreadyExistsException : MachineManagementException
  {
    public MessageIdAlreadyExistsException(string message)
      : base(message)
    {
    }

    private MessageIdAlreadyExistsException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}

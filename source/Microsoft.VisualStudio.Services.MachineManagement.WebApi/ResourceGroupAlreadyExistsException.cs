// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.ResourceGroupAlreadyExistsException
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  [ExceptionMapping("0.0", "3.0", "ResourceGroupNotFoundException", "Microsoft.VisualStudio.Services.MachineManagement.WebApi.ResourceGroupAlreadyExistsException, Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public sealed class ResourceGroupAlreadyExistsException : MachineManagementException
  {
    public ResourceGroupAlreadyExistsException(string message)
      : base(message)
    {
    }

    private ResourceGroupAlreadyExistsException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}

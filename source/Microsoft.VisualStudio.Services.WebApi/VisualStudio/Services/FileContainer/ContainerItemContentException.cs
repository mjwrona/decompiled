// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.FileContainer.ContainerItemContentException
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.FileContainer
{
  [ExceptionMapping("0.0", "3.0", "ContainerItemContentException", "Microsoft.VisualStudio.Services.FileContainer.ContainerItemContentException, Microsoft.VisualStudio.Services.WebApi, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public sealed class ContainerItemContentException : FileContainerException
  {
    public ContainerItemContentException()
      : base(FileContainerResources.NoContentReturned())
    {
    }

    public ContainerItemContentException(string message)
      : base(message)
    {
    }

    public ContainerItemContentException(string message, Exception ex)
      : base(message, ex)
    {
    }
  }
}

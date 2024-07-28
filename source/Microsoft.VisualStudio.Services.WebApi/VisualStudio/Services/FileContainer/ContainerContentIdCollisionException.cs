// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.FileContainer.ContainerContentIdCollisionException
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.FileContainer
{
  [ExceptionMapping("0.0", "3.0", "ContainerContentIdCollisionException", "Microsoft.VisualStudio.Services.FileContainer.ContainerContentIdCollisionException, Microsoft.VisualStudio.Services.WebApi, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public sealed class ContainerContentIdCollisionException : FileContainerException
  {
    public ContainerContentIdCollisionException(
      string fileId1,
      string length1,
      string fileId2,
      string length2)
      : base(FileContainerResources.ContentIdCollision((object) fileId1, (object) length1, (object) fileId2, (object) length2))
    {
    }

    public ContainerContentIdCollisionException(string message)
      : base(message)
    {
    }

    public ContainerContentIdCollisionException(string message, Exception ex)
      : base(message, ex)
    {
    }
  }
}

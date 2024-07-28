// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.Common.ContainerIsDeletePendingException
// Assembly: Microsoft.VisualStudio.Services.ItemStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 44753C0C-D541-4975-AF3F-2B606DE6FF70
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ItemStore.Common
{
  [Serializable]
  public class ContainerIsDeletePendingException : VssServiceException
  {
    public ContainerIsDeletePendingException(string message, Exception inner)
      : base(message, inner)
    {
    }

    public ContainerIsDeletePendingException(string message)
      : this(message, (Exception) null)
    {
    }

    public ContainerIsDeletePendingException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public static ContainerIsDeletePendingException Create(string containerId) => new ContainerIsDeletePendingException(ContainerIsDeletePendingException.MakeMessage(containerId));

    private static string MakeMessage(string containerId) => Resources.DeletePendingContainer((object) containerId);
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.Common.DropManifestItemException
// Assembly: Microsoft.VisualStudio.Services.ItemStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 44753C0C-D541-4975-AF3F-2B606DE6FF70
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ItemStore.Common
{
  [Serializable]
  public class DropManifestItemException : VssServiceException
  {
    public DropManifestItemException(string message, Exception inner)
      : base(message, inner)
    {
    }

    public DropManifestItemException(string message)
      : this(message, (Exception) null)
    {
    }

    public DropManifestItemException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public static DropManifestItemException Create(
      string containerName,
      string path,
      string manifestBlobIdentifier)
    {
      return new DropManifestItemException(DropManifestItemException.MakeMessage(containerName, path, manifestBlobIdentifier));
    }

    private static string MakeMessage(
      string containerName,
      string path,
      string manifestBlobIdentifier)
    {
      return Resources.ManifestItemException((object) containerName, (object) path, (object) manifestBlobIdentifier);
    }
  }
}

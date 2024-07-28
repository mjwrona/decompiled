// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.InvalidResourceExtensionException
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Cloud
{
  [Serializable]
  public class InvalidResourceExtensionException : TeamFoundationServiceException
  {
    public InvalidResourceExtensionException()
    {
    }

    public InvalidResourceExtensionException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected InvalidResourceExtensionException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public InvalidResourceExtensionException(string resourceName)
      : base(HostingResources.ResourceExtensionDoesNotExist((object) resourceName))
    {
    }

    public InvalidResourceExtensionException(string resourceName, IEnumerable<Guid> namespaceIds)
      : base(HostingResources.DuplicateResourceExtensionNamespaceIds((object) resourceName, (object) string.Join<Guid>(", ", namespaceIds)))
    {
    }
  }
}

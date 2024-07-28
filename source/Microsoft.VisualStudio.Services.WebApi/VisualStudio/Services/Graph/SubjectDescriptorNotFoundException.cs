// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.SubjectDescriptorNotFoundException
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Graph
{
  [Serializable]
  public class SubjectDescriptorNotFoundException : GraphException
  {
    public SubjectDescriptorNotFoundException()
    {
    }

    public SubjectDescriptorNotFoundException(string message)
      : base(message)
    {
    }

    public SubjectDescriptorNotFoundException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected SubjectDescriptorNotFoundException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public SubjectDescriptorNotFoundException(Guid storageKey)
      : base(GraphResources.SubjectDescriptorNotFoundWithStorageKey((object) storageKey))
    {
    }

    public SubjectDescriptorNotFoundException(IdentityDescriptor identityDescriptor)
      : base(GraphResources.SubjectDescriptorNotFoundWithIdentityDescriptor((object) identityDescriptor))
    {
    }
  }
}

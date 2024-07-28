// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.GraphApiUnavailableException
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Graph
{
  [Serializable]
  public class GraphApiUnavailableException : GraphException
  {
    protected GraphApiUnavailableException()
    {
    }

    public GraphApiUnavailableException(string message)
      : base(message)
    {
    }

    public GraphApiUnavailableException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected GraphApiUnavailableException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public GraphApiUnavailableException(SubjectDescriptor subjectDescriptor)
      : base(IdentityResources.IdentityNotFoundWithDescriptor((object) subjectDescriptor.SubjectType, (object) subjectDescriptor.Identifier))
    {
    }

    public GraphApiUnavailableException(Guid id)
      : base(IdentityResources.IdentityNotFoundWithTfid((object) id))
    {
    }
  }
}

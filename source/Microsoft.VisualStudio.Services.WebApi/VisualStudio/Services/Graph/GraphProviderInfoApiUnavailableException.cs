// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.GraphProviderInfoApiUnavailableException
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
  public class GraphProviderInfoApiUnavailableException : GraphException
  {
    protected GraphProviderInfoApiUnavailableException()
    {
    }

    public GraphProviderInfoApiUnavailableException(string message)
      : base(message)
    {
    }

    public GraphProviderInfoApiUnavailableException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected GraphProviderInfoApiUnavailableException(
      SerializationInfo info,
      StreamingContext context)
      : base(info, context)
    {
    }

    public GraphProviderInfoApiUnavailableException(SubjectDescriptor subjectDescriptor)
      : base(IdentityResources.IdentityNotFoundWithDescriptor((object) subjectDescriptor.SubjectType, (object) subjectDescriptor.Identifier))
    {
    }

    public GraphProviderInfoApiUnavailableException(Guid id)
      : base(IdentityResources.IdentityNotFoundWithTfid((object) id))
    {
    }
  }
}

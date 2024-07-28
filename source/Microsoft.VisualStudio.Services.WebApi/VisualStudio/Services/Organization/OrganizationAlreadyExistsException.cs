// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.OrganizationAlreadyExistsException
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Organization
{
  [Serializable]
  public class OrganizationAlreadyExistsException : OrganizationException
  {
    public OrganizationAlreadyExistsException()
    {
    }

    public OrganizationAlreadyExistsException(string message)
      : base(message)
    {
    }

    public OrganizationAlreadyExistsException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected OrganizationAlreadyExistsException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}

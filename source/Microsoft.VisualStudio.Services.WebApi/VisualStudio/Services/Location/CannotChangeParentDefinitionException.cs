// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Location.CannotChangeParentDefinitionException
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Location
{
  [ExceptionMapping("0.0", "3.0", "CannotChangeParentDefinitionException", "Microsoft.VisualStudio.Services.Location.CannotChangeParentDefinitionException, Microsoft.VisualStudio.Services.WebApi, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public class CannotChangeParentDefinitionException : VssServiceException
  {
    public CannotChangeParentDefinitionException()
    {
    }

    public CannotChangeParentDefinitionException(string message)
      : base(CannotChangeParentDefinitionException.GetMessage(message))
    {
    }

    public CannotChangeParentDefinitionException(string message, Exception ex)
      : base(CannotChangeParentDefinitionException.GetMessage(message), ex)
    {
    }

    protected CannotChangeParentDefinitionException(
      SerializationInfo info,
      StreamingContext context)
      : base(info, context)
    {
    }

    private static string GetMessage(string message) => message;
  }
}

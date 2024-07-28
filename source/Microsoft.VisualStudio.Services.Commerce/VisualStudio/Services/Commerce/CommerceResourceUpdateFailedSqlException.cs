// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceResourceUpdateFailedSqlException
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [ExcludeFromCodeCoverage]
  public class CommerceResourceUpdateFailedSqlException : TeamFoundationServiceException
  {
    public CommerceResourceUpdateFailedSqlException()
    {
    }

    public CommerceResourceUpdateFailedSqlException(string message)
      : base(message)
    {
    }

    public CommerceResourceUpdateFailedSqlException(string message, Exception inner)
      : base(message, inner)
    {
    }

    protected CommerceResourceUpdateFailedSqlException(
      SerializationInfo info,
      StreamingContext context)
      : base(info, context)
    {
    }
  }
}

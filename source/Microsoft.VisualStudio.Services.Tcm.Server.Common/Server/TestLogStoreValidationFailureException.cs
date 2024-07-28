// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestLogStoreValidationFailureException
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestLogStoreValidationFailureException : TestManagementServiceException
  {
    public TestLogStoreValidationFailureException()
    {
    }

    public TestLogStoreValidationFailureException(string exceptionMessage)
      : base(exceptionMessage)
    {
    }

    public TestLogStoreValidationFailureException(string exceptionMessage, Exception innerException)
      : base(exceptionMessage, innerException)
    {
    }

    protected TestLogStoreValidationFailureException(
      SerializationInfo info,
      StreamingContext context)
      : base(info, context)
    {
    }
  }
}

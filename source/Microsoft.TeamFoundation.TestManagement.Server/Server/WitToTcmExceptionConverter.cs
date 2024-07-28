// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.WitToTcmExceptionConverter
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server;
using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal static class WitToTcmExceptionConverter
  {
    internal static TestManagementServiceException Convert(Exception e)
    {
      switch (e)
      {
        case null:
          return (TestManagementServiceException) null;
        case WorkItemException innerException1:
          return innerException1.ErrorCode == 600122 ? (TestManagementServiceException) new TestObjectUpdatedException(innerException1.Message, (Exception) innerException1) : (TestManagementServiceException) new TestManagementValidationException(innerException1.Message, (Exception) innerException1);
        case WorkItemUnauthorizedAccessException innerException2:
          return (TestManagementServiceException) new TestManagementValidationException(innerException2.Message, (Exception) innerException2);
        default:
          return new TestManagementServiceException(e.Message, e);
      }
    }
  }
}

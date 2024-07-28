// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemFieldInvalidException
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [Serializable]
  public class WorkItemFieldInvalidException : WorkItemException
  {
    public WorkItemFieldInvalidException(
      int workItemId,
      string referenceName,
      FieldStatusFlags status)
      : base(workItemId, WorkItemFieldInvalidException.GetMessage(workItemId, referenceName, status))
    {
      this.Status = status;
      this.ErrorCode = 600171;
    }

    public WorkItemFieldInvalidException(int workItemId, string message)
      : base(workItemId, message)
    {
      this.ErrorCode = 600171;
    }

    public WorkItemFieldInvalidException(
      int workItemId,
      string referenceName,
      string message,
      int errorCode = 600171)
      : base(workItemId, message)
    {
      this.ReferenceName = referenceName;
      this.ErrorCode = errorCode;
    }

    public FieldStatusFlags Status { get; private set; }

    [DataMember]
    public string ReferenceName { get; private set; }

    private static string GetMessage(int workItemId, string referenceName, FieldStatusFlags status)
    {
      string str = string.IsNullOrEmpty(referenceName) ? workItemId.ToString() : referenceName;
      switch (status)
      {
        case FieldStatusFlags.InvalidDate:
          return ServerResources.FieldInvalidDateTime((object) str);
        case FieldStatusFlags.InvalidTooLong:
          return ServerResources.FieldValueTooLong((object) str);
        case FieldStatusFlags.InvalidSpecialChars:
          return ServerResources.FieldValueInvalidCharacters((object) str);
        default:
          return ServerResources.InvalidFieldStatus((object) status, (object) str);
      }
    }
  }
}

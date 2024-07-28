// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTrackingLinkTypeException
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  [Serializable]
  public abstract class WorkItemTrackingLinkTypeException : WorkItemTrackingServiceException
  {
    protected WorkItemTrackingLinkTypeException(string message, int errorCode, int id)
      : this(message, errorCode, new int?(id), (string) null)
    {
    }

    protected WorkItemTrackingLinkTypeException(
      string message,
      int errorCode,
      string referenceName)
      : this(message, errorCode, new int?(), referenceName)
    {
    }

    protected WorkItemTrackingLinkTypeException(
      string message,
      int errorCode,
      int? id,
      string referenceName)
      : base(message)
    {
      this.Id = id;
      this.ErrorCode = errorCode;
      this.ReferenceName = referenceName;
    }

    public int? Id { get; private set; }

    public string ReferenceName { get; private set; }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ContainerAccessCondition
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.Azure.Storage;
using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class ContainerAccessCondition : IContainerAccessCondition
  {
    private AccessCondition _accessCondition;

    public ContainerAccessCondition() => this._accessCondition = new AccessCondition();

    public ContainerAccessCondition(
      string leaseId,
      string ifNoneMatchETag,
      string ifMatchETag,
      DateTimeOffset? ifModifiedSinceTime)
    {
      this._accessCondition = new AccessCondition()
      {
        LeaseId = leaseId,
        IfNoneMatchETag = ifNoneMatchETag,
        IfMatchETag = ifMatchETag,
        IfModifiedSinceTime = ifModifiedSinceTime
      };
    }

    public AccessCondition GetAccessCondition() => this._accessCondition;
  }
}

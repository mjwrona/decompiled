// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTrackingLinkTypeNotFoundException
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  [Serializable]
  public class WorkItemTrackingLinkTypeNotFoundException : WorkItemTrackingLinkTypeException
  {
    public WorkItemTrackingLinkTypeNotFoundException(string referenceName)
      : base(string.Format((IFormatProvider) CultureInfo.InvariantCulture, InternalsResourceStrings.Get("LinkTypeNotFound"), (object) referenceName), 600250, referenceName)
    {
    }

    public WorkItemTrackingLinkTypeNotFoundException(int id)
      : base(string.Format((IFormatProvider) CultureInfo.InvariantCulture, InternalsResourceStrings.Get("LinkTypeNotFound"), (object) id), 600250, id)
    {
    }
  }
}

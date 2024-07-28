// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.WorkItemUpdateResult
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model
{
  [DataContract]
  public class WorkItemUpdateResult : WorkItemReference
  {
    [DataMember(EmitDefaultValue = false)]
    public string UpdatesUrl { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int UpdateId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public TeamFoundationServiceException Exception { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<FieldValue> Fields { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<WorkItemLinkUpdateResult> Links { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<WorkItemResourceLinkUpdateResult> ResourceLinks { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<Guid> CurrentExtensions { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<Guid> AttachedExtensions { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<Guid> DetachedExtensions { get; set; }
  }
}

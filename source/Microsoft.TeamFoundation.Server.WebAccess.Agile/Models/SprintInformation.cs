// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.SprintInformation
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.Models
{
  [DataContract]
  public class SprintInformation
  {
    public SprintInformation(IVssRequestContext requestContext, TreeNode nodeInfo)
    {
      this.Name = nodeInfo.GetName(requestContext);
      this.IterationPath = nodeInfo.GetPath(requestContext);
      this.IterationId = nodeInfo.CssNodeId;
      this.StartDate = nodeInfo.StartDate;
      this.FinishDate = nodeInfo.FinishDate;
    }

    [DataMember(Name = "name")]
    public string Name { get; set; }

    [DataMember(Name = "path")]
    public string IterationPath { get; set; }

    [DataMember(Name = "id", EmitDefaultValue = false)]
    public Guid IterationId { get; set; }

    [DataMember(Name = "start", EmitDefaultValue = false)]
    public DateTime? StartDate { get; set; }

    [DataMember(Name = "finish", EmitDefaultValue = false)]
    public DateTime? FinishDate { get; set; }
  }
}

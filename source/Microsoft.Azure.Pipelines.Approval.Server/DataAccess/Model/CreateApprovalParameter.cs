// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Approval.Server.DataAccess.Model.CreateApprovalParameter
// Assembly: Microsoft.Azure.Pipelines.Approval.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03DD9218-2C79-49A0-9EA7-F497B1327A4F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Approval.Server.dll

using Microsoft.Azure.Pipelines.Approval.WebApi;
using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.Azure.Pipelines.Approval.Server.DataAccess.Model
{
  public class CreateApprovalParameter
  {
    public Guid ApprovalId { get; set; }

    public Guid TimeoutJobId { get; set; }

    public Guid CreatedBy { get; set; }

    public ApprovalConfig Config { get; set; }

    public ApprovalOwner Owner { get; set; }

    public JObject Context { get; set; }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Approval.Server.DataAccess.ApprovalStepBinder2
// Assembly: Microsoft.Azure.Pipelines.Approval.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03DD9218-2C79-49A0-9EA7-F497B1327A4F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Approval.Server.dll

using Microsoft.Azure.Pipelines.Approval.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.Azure.Pipelines.Approval.Server.DataAccess
{
  internal class ApprovalStepBinder2 : ApprovalStepBinder
  {
    private SqlColumnBinder m_deferredTo = new SqlColumnBinder("DeferredTo");

    protected override void AdditionalActions(ApprovalStep approvalStep) => approvalStep.DeferredTo = this.m_deferredTo.GetNullableDateTime((IDataReader) this.Reader);
  }
}

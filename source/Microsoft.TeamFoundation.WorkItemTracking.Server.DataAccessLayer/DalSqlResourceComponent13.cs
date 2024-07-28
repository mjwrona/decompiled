// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DalSqlResourceComponent13
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DalSqlResourceComponent13 : DalSqlResourceComponent12
  {
    private void InsertDomainAccountsSVRules()
    {
      this.PrepareStoredProcedure("prc_InsertDomainAccountsSuggestedValueRules");
      this.ExecuteNonQuery();
    }

    private void DeleteDomainAccountsSVRules()
    {
      this.PrepareStoredProcedure("prc_DeleteDomainAccountsSuggestedValueRules");
      this.ExecuteNonQuery();
    }

    private void UpdateCacheStampAllowedValueRules()
    {
      this.PrepareStoredProcedure("prc_UpdateCacheStampAllowedValueRules");
      this.ExecuteNonQuery();
    }

    public override void InsertDomainAccountsSVRulesAndUpdateCacheStamp()
    {
      this.BeginTransaction(IsolationLevel.ReadCommitted);
      try
      {
        this.InsertDomainAccountsSVRules();
        this.UpdateCacheStampAllowedValueRules();
        this.CommitTransaction();
      }
      catch
      {
        this.RollbackTransaction();
        throw;
      }
    }

    public override void DeleteDomainAccountsSVRulesAndUpdateCacheStamp()
    {
      this.BeginTransaction(IsolationLevel.ReadCommitted);
      try
      {
        this.DeleteDomainAccountsSVRules();
        this.UpdateCacheStampAllowedValueRules();
        this.CommitTransaction();
      }
      catch
      {
        this.RollbackTransaction();
        throw;
      }
    }
  }
}

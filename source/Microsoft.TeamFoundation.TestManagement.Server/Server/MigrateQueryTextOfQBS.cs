// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.MigrateQueryTextOfQBS
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class MigrateQueryTextOfQBS
  {
    private TfsTestManagementRequestContext m_requestContext;
    private MigrationLogger m_logger;

    public MigrateQueryTextOfQBS(
      TestManagementRequestContext context,
      IServicingContext servicingContext)
    {
      this.m_requestContext = new TfsTestManagementRequestContext(context.RequestContext);
      this.m_logger = new MigrationLogger(context, servicingContext);
    }

    public bool Migrate(string projectName)
    {
      try
      {
        this.m_logger.Log(TraceLevel.Info, "TestManagement::Migrate Query Based Suite Data Start.");
        bool flag = false;
        int num;
        do
        {
          List<int> migrate = this.FetchQBSIdsToMigrate();
          num = 0;
          if (migrate.Count == 0)
          {
            this.m_logger.Log(TraceLevel.Info, string.Format("Did not find any QBS which needs migration in the project {0}.", (object) projectName));
            return flag;
          }
          List<IdAndRev> idAndRevList = migrate.ConvertAll<IdAndRev>((Converter<int, IdAndRev>) (suiteId => new IdAndRev(suiteId, 0)));
          List<ServerTestSuite> serverTestSuiteList = ServerTestSuite.Fetch((TestManagementRequestContext) this.m_requestContext, projectName, idAndRevList.ToArray(), new List<int>());
          if (serverTestSuiteList.Count == 0)
          {
            this.m_logger.Log(TraceLevel.Info, string.Format("Found no more QBS in project {0} to be migrated.", (object) projectName));
            return flag;
          }
          foreach (ServerTestSuite qbsSuite in serverTestSuiteList)
          {
            try
            {
              this.UpdateQueryWithRetryWhenOutOfSync(qbsSuite);
            }
            catch (Exception ex)
            {
              flag = true;
              ++num;
              this.m_logger.Log(TraceLevel.Error, string.Format("Error Occurred During QBS Migration for suiteId {0} in project {1} : Exception {2}.", (object) qbsSuite.Id, (object) projectName, (object) ex.ToString()));
            }
          }
          if (num >= serverTestSuiteList.Count)
          {
            this.m_logger.Log(TraceLevel.Warning, "None of the suites in current iteration was updated. Please check the upgrade logs");
            return flag;
          }
        }
        while (num == 0);
        this.m_logger.Log(TraceLevel.Warning, "Some of the suites in current iteration were not updated. Please check the upgrade logs.");
        return flag;
      }
      finally
      {
        this.m_logger.Log(TraceLevel.Info, "TestManagement::Migrate Query Based Suite Data End.");
      }
    }

    public void Migrate()
    {
      int num = 0;
      try
      {
        this.m_logger.Log(TraceLevel.Info, "TestManagement::Migrate Query Based Suite Data Start.");
label_2:
        List<int> migrate = this.FetchQBSIdsToMigrate();
        if (migrate.Count == 0)
        {
          this.m_logger.Log(TraceLevel.Info, "Did not find any QBS which needs migration.");
        }
        else
        {
          using (List<int>.Enumerator enumerator = migrate.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              int current = enumerator.Current;
              try
              {
                this.UpdateQueryWithRetryWhenOutOfSync(current);
              }
              catch (Exception ex)
              {
                ++num;
                this.m_logger.Log(TraceLevel.Info, string.Format("Error Occurred During QBS Migration for suiteId {0} : Exception {1}.", (object) current, (object) ex.ToString()));
                using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create((TestManagementRequestContext) this.m_requestContext))
                  planningDatabase.UpdateQBSMigrationDetails((TestManagementRequestContext) this.m_requestContext, current, UpgradeMigrationState.Failed);
              }
            }
            goto label_2;
          }
        }
      }
      finally
      {
        if (num > 0)
          this.m_logger.Log(TraceLevel.Warning, ServerResources.MigrateQBSWarning);
        this.m_logger.Log(TraceLevel.Info, "TestManagement::Migrate Query Based Suite Data End.");
      }
    }

    private void UpdateQueryWithRetryWhenOutOfSync(int suiteId)
    {
      ServerTestSuite serverTestSuite = new ServerTestSuite()
      {
        Id = suiteId
      };
      this.m_logger.Log(TraceLevel.Info, string.Format("Starting Migration for Suite {0}.", (object) suiteId));
      this.m_logger.Log(TraceLevel.Info, string.Format("Fetch Suite {0} for Migration.", (object) suiteId));
      this.UpdateQueryWithRetryWhenOutOfSync(serverTestSuite.FetchTestSuite((TestManagementRequestContext) this.m_requestContext, new IdAndRev(suiteId, 0)));
    }

    private void UpdateQueryWithRetryWhenOutOfSync(ServerTestSuite qbsSuite)
    {
      int id = qbsSuite.Id;
      try
      {
        this.m_logger.Log(TraceLevel.Info, string.Format("Starting Migration for Suite {0}.", (object) id));
        this.UpdateQueryInQBS(qbsSuite);
      }
      catch (TestObjectUpdatedException ex)
      {
        this.m_logger.Log(TraceLevel.Info, string.Format("Syncing the suite {0} since it was out of sync as it threw : Exception {1}.", (object) id, (object) ex.Message));
        ServerTestSuite.SyncSuites((TestManagementRequestContext) this.m_requestContext, qbsSuite.ProjectName, (IEnumerable<IIdAndRevBase>) new List<IdAndRev>()
        {
          new IdAndRev(qbsSuite.Id, qbsSuite.Revision)
        });
        this.UpdateQueryInQBS(qbsSuite);
      }
    }

    private void UpdateQueryInQBS(ServerTestSuite qbsSuite)
    {
      try
      {
        this.m_logger.Log(TraceLevel.Info, string.Format("Forming the normalized queryText {1} of Suite {0} for Migration.", (object) qbsSuite.Id, (object) qbsSuite.QueryString));
        qbsSuite.ConvertedQueryString = WiqlTransformUtils.TransformNamesToIds(this.m_requestContext.RequestContext, qbsSuite.QueryString, true);
        qbsSuite.QueryMigrationState = UpgradeMigrationState.Completed;
        this.m_logger.Log(TraceLevel.Info, string.Format("Updating the suite with QueryText field to {1} for Suite {0} for Migration.", (object) qbsSuite.Id, (object) qbsSuite.ConvertedQueryString));
        qbsSuite.Update((TestManagementRequestContext) this.m_requestContext, qbsSuite.ProjectName, TestSuiteSource.Job, false, true);
      }
      catch (SyntaxException ex)
      {
        using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create((TestManagementRequestContext) this.m_requestContext))
          planningDatabase.UpdateQBSMigrationDetails((TestManagementRequestContext) this.m_requestContext, qbsSuite.Id, UpgradeMigrationState.Failed);
        this.m_logger.Log(TraceLevel.Info, string.Format("Error Occurred During QBS Migration: Exception {0}.", (object) ex.ToString()));
      }
    }

    private List<int> FetchQBSIdsToMigrate()
    {
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create((TestManagementRequestContext) this.m_requestContext))
        return planningDatabase.FetchQBSApplicableForMigration(this.m_requestContext);
    }
  }
}

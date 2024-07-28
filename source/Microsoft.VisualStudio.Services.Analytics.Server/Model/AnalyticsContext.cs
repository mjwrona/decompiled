// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.AnalyticsContext
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using EntityFramework.Functions;
using Microsoft.VisualStudio.Services.Analytics.Model.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.VisualStudio.Services.Analytics.Model
{
  internal class AnalyticsContext : DbContext, IComponentContext
  {
    static AnalyticsContext()
    {
      lock (ManyToManyJoinInterceptor.Instance)
      {
        DbInterception.Remove((IDbInterceptor) ManyToManyJoinInterceptor.Instance);
        DbInterception.Add((IDbInterceptor) ManyToManyJoinInterceptor.Instance);
        DbInterception.Remove((IDbInterceptor) ProjectScopeQueryInterceptor.Instance);
        DbInterception.Add((IDbInterceptor) ProjectScopeQueryInterceptor.Instance);
        DbInterception.Remove((IDbInterceptor) SqlResourceComponentInterceptor.Instance);
        DbInterception.Add((IDbInterceptor) SqlResourceComponentInterceptor.Instance);
      }
    }

    public int Version { get; }

    internal AnalyticsContext(DbCompiledModel model, int databaseServiceVersion)
      : base(model)
    {
      this.ConfigureContext();
      this.Version = databaseServiceVersion;
    }

    public AnalyticsContext(
      DbCompiledModel model,
      AnalyticsComponent parentComponent,
      DbConnection conn)
      : base(AnalyticsContext.CreateObjectContext(model, conn), true)
    {
      this.ConfigureContext();
      this.Component = parentComponent;
      this.Version = this.Component.Version;
    }

    private static ObjectContext CreateObjectContext(DbCompiledModel model, DbConnection conn)
    {
      ObjectContext objectContext = model.CreateObjectContext<ObjectContext>(conn);
      objectContext.ContextOptions.DisableFilterOverProjectionSimplificationForCustomFunctions = true;
      return objectContext;
    }

    public static IEnumerable<Type> GetEntityTypes() => ((IEnumerable<PropertyInfo>) typeof (AnalyticsContext).GetProperties()).Where<PropertyInfo>((Func<PropertyInfo, bool>) (p => p.PropertyType.IsGenericType)).Where<PropertyInfo>((Func<PropertyInfo, bool>) (p => p.PropertyType.GetGenericTypeDefinition() == typeof (DbSet<>))).Select<PropertyInfo, Type>((Func<PropertyInfo, Type>) (p => p.PropertyType.GenericTypeArguments[0]));

    private void ConfigureContext()
    {
      Database.SetInitializer<AnalyticsContext>((IDatabaseInitializer<AnalyticsContext>) null);
      this.Configuration.AutoDetectChangesEnabled = false;
      this.Configuration.ProxyCreationEnabled = false;
      this.Configuration.UseDatabaseNullSemantics = true;
      this.Configuration.LazyLoadingEnabled = false;
    }

    public AnalyticsComponent Component { get; private set; }

    public DbSet<Microsoft.VisualStudio.Services.Analytics.Model.WorkItemSnapshot> WorkItemSnapshot { get; set; }

    public DbSet<Microsoft.VisualStudio.Services.Analytics.Model.WorkItemBoardSnapshot> WorkItemBoardSnapshot { get; set; }

    public DbSet<WorkItemRevision> WorkItemRevisions { get; set; }

    public DbSet<WorkItem> WorkItems { get; set; }

    public DbSet<WorkItemLink> WorkItemLinks { get; set; }

    public DbSet<Project> Projects { get; set; }

    public DbSet<CalendarDate> Dates { get; set; }

    public DbSet<Iteration> Iterations { get; set; }

    public DbSet<Area> Areas { get; set; }

    public DbSet<Team> Teams { get; set; }

    public DbSet<Tag> Tags { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<BoardLocation> BoardLocations { get; set; }

    public DbSet<WorkItemTypeField> WorkItemTypeFields { get; set; }

    public DbSet<Test> Tests { get; set; }

    public DbSet<TestRun> TestRuns { get; set; }

    public DbSet<TestResult> TestResults { get; set; }

    public DbSet<TestResultDaily> TestResultsDaily { get; set; }

    public DbSet<TestConfiguration> TestConfigurations { get; set; }

    public DbSet<TestSuite> TestSuites { get; set; }

    public DbSet<TestPoint> TestPoints { get; set; }

    public DbSet<Microsoft.VisualStudio.Services.Analytics.Model.TestPointHistorySnapshot> TestPointHistorySnapshot { get; set; }

    public DbSet<PipelineRun> PipelineRuns { get; set; }

    public DbSet<Branch> Branches { get; set; }

    public DbSet<Process> Processes { get; set; }

    public DbSet<Pipeline> Pipelines { get; set; }

    public DbSet<PipelineTask> PipelineTasks { get; set; }

    public DbSet<PipelineRunActivityResult> PipelineRunActivityResults { get; set; }

    public DbSet<PipelineJob> PipelineJobs { get; set; }

    public DbSet<PipelineEnvironment> PipelineEnvironments { get; set; }

    public DbSet<TaskAgentPoolSizeSnapshot> TaskAgentPoolSizeSnapshots { get; set; }

    public DbSet<Microsoft.VisualStudio.Services.Analytics.Model.CommitToDeployment> CommitToDeployment { get; set; }

    public DbSet<GitHubCommit> GitHubCommits { get; set; }

    public DbSet<GitHubAccount> GitHubAccounts { get; set; }

    public DbSet<GitHubRepository> GitHubRepositories { get; set; }

    public DbSet<GitHubTeam> GitHubTeams { get; set; }

    public DbSet<GitHubUser> GitHubUsers { get; set; }

    public DbSet<Microsoft.VisualStudio.Services.Analytics.Model.GitHubPullRequest> GitHubPullRequest { get; set; }

    public DbSet<Microsoft.VisualStudio.Services.Analytics.Model.GitHubPullRequestSnapshot> GitHubPullRequestSnapshot { get; set; }

    public DbSet<Microsoft.VisualStudio.Services.Analytics.Model.GitHubBranch> GitHubBranch { get; set; }

    public DbSet<GitHubContributorActivity> GitHubContributorActivities { get; set; }

    public DbSet<GitHubPullRequestReviewerActivity> GitHubPullRequestReviewerActivities { get; set; }

    public DbSet<Microsoft.VisualStudio.Services.Analytics.Model.ParallelPipelineJobsSnapshot> ParallelPipelineJobsSnapshot { get; set; }

    public DbSet<TaskAgentRequestSnapshot> TaskAgentRequestSnapshots { get; set; }

    internal static DbCompiledModel CreateModel(int databaseServiceVersion)
    {
      Type type = typeof (AnalyticsContext);
      DbModelBuilder modelBuilder = new DbModelBuilder();
      modelBuilder.Conventions.Add((IConvention) new AnalyticsModelContainerConvention(nameof (AnalyticsContext)));
      modelBuilder.Conventions.Add((IConvention) new DatabaseRenamePropertyConvention(databaseServiceVersion));
      modelBuilder.Conventions.Add((IConvention) new DatabaseHidePropertyConvention(databaseServiceVersion));
      modelBuilder.Conventions.Add((IConvention) new DatabaseHideNavigationPropertyConvention(databaseServiceVersion));
      modelBuilder.Conventions.AddAfter<TableAttributeConvention>((IConvention) new DatabaseRenameEntityConvention(databaseServiceVersion));
      modelBuilder.Conventions.Add((IConvention) new FunctionConvention<AnalyticsContext>());
      foreach (var data in ((IEnumerable<PropertyInfo>) type.GetProperties()).Where<PropertyInfo>((Func<PropertyInfo, bool>) (p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof (DbSet<>))).Select(p => new
      {
        Type = ((IEnumerable<Type>) p.PropertyType.GetGenericArguments()).First<Type>(),
        Name = p.Name
      }).OrderBy(s => s.Name))
        modelBuilder.RegisterEntityType(data.Type);
      AnalyticsContext.PrepareModel(modelBuilder, databaseServiceVersion);
      DbProviderInfo providerInfo = new DbProviderInfo("System.Data.SqlClient", "2012");
      return modelBuilder.Build(providerInfo).Compile();
    }

    private static void PrepareModel(DbModelBuilder modelBuilder, int version)
    {
      AnalyticsContext.CurrentModelCrerating(modelBuilder, version);
      modelBuilder.Conventions.Add((IConvention) new AnalyticsContext.ClrTypeConvention());
      modelBuilder.Conventions.Add((IConvention) new ApproxDistinctFunction());
      modelBuilder.Conventions.Add((IConvention) new PredictFunctions());
      modelBuilder.Conventions.Add((IConvention) new WindowFunctions());
    }

    [TableValuedFunction("func_PredictTags", "AnalyticsContext", Schema = "AnalyticsModel")]
    public IQueryable<Tag> PredictTags(int partitionId, int workItemRevisionSK) => Enumerable.Range(1, 2).Select<int, Tag>((Func<int, Tag>) (i => new Tag()
    {
      TagId = new Guid?(Guid.NewGuid()),
      TagName = string.Format("Tag{0}", (object) i)
    })).AsQueryable<Tag>();

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);
      int version = this.Component != null ? this.Component.Version : int.MaxValue;
      AnalyticsContext.PrepareModel(modelBuilder, version);
    }

    protected static void CurrentModelCrerating(DbModelBuilder modelBuilder, int serviceVersion)
    {
      modelBuilder.Entity<CalendarDate>().HasKey(d => new
      {
        PartitionId = d.PartitionId,
        DateSK = d.DateSK
      });
      modelBuilder.Entity<Project>().HasKey(p => new
      {
        PartitionId = p.PartitionId,
        ProjectSK = p.ProjectSK
      });
      modelBuilder.Entity<WorkItemRevision>().HasKey(wit => new
      {
        PartitionId = wit.PartitionId,
        WorkItemRevisionSK = wit.WorkItemRevisionSK
      });
      modelBuilder.Entity<Microsoft.VisualStudio.Services.Analytics.Model.WorkItemSnapshot>().HasKey(wit => new
      {
        PartitionId = wit.PartitionId,
        WorkItemRevisionSK = wit.WorkItemRevisionSK,
        DateSK = wit.DateSK
      });
      modelBuilder.Entity<Microsoft.VisualStudio.Services.Analytics.Model.WorkItemBoardSnapshot>().HasKey(wit => new
      {
        PartitionId = wit.PartitionId,
        WorkItemRevisionSK = wit.WorkItemRevisionSK,
        DateSK = wit.DateSK,
        BoardLocationSK = wit.BoardLocationSK
      });
      modelBuilder.Entity<WorkItem>().HasKey(wit => new
      {
        PartitionId = wit.PartitionId,
        WorkItemId = wit.WorkItemId
      });
      modelBuilder.Entity<WorkItemLink>().HasKey(l => new
      {
        PartitionId = l.PartitionId,
        WorkItemLinkSK = l.WorkItemLinkSK
      });
      modelBuilder.Entity<Iteration>().HasKey(i => new
      {
        PartitionId = i.PartitionId,
        IterationSK = i.IterationSK
      });
      modelBuilder.Entity<Area>().HasKey(i => new
      {
        PartitionId = i.PartitionId,
        AreaSK = i.AreaSK
      });
      modelBuilder.Entity<Tag>().ToTable("vw_Tag", "AnalyticsModel");
      modelBuilder.Entity<Tag>().HasKey(i => new
      {
        PartitionId = i.PartitionId,
        TagSK = i.TagSK
      });
      modelBuilder.Entity<Team>().ToTable("vw_Team", "AnalyticsModel");
      modelBuilder.Entity<Team>().HasKey(i => new
      {
        PartitionId = i.PartitionId,
        TeamSK = i.TeamSK
      });
      modelBuilder.Entity<BoardLocation>().HasKey(i => new
      {
        PartitionId = i.PartitionId,
        BoardLocationSK = i.BoardLocationSK
      });
      modelBuilder.Entity<User>().ToTable("vw_User", "AnalyticsModel");
      modelBuilder.Entity<User>().HasKey(i => new
      {
        PartitionId = i.PartitionId,
        UserSK = i.UserSK
      });
      modelBuilder.Entity<WorkItemTypeField>().ToTable("vw_WorkItemTypeField", "AnalyticsModel");
      modelBuilder.Entity<WorkItemTypeField>().HasKey(i => new
      {
        PartitionId = i.PartitionId,
        ProjectSK = i.ProjectSK,
        FieldName = i.FieldName,
        WorkItemType = i.WorkItemType,
        WorkItemTypeCategory = i.WorkItemTypeCategory
      });
      modelBuilder.Entity<TestRun>().HasKey(i => new
      {
        PartitionId = i.PartitionId,
        TestRunSK = i.TestRunSK
      });
      modelBuilder.Entity<TestResult>().HasKey(i => new
      {
        PartitionId = i.PartitionId,
        TestResultSK = i.TestResultSK
      });
      modelBuilder.Entity<TestResultDaily>().HasKey(i => new
      {
        PartitionId = i.PartitionId,
        TestResultDailySK = i.TestResultDailySK
      });
      modelBuilder.Entity<TestConfiguration>().HasKey(i => new
      {
        PartitionId = i.PartitionId,
        TestConfigurationSK = i.TestConfigurationSK
      });
      modelBuilder.Entity<TestSuite>().HasKey(i => new
      {
        PartitionId = i.PartitionId,
        TestSuiteSK = i.TestSuiteSK
      });
      modelBuilder.Entity<TestPoint>().HasKey(i => new
      {
        PartitionId = i.PartitionId,
        TestPointSK = i.TestPointSK
      });
      modelBuilder.Entity<Microsoft.VisualStudio.Services.Analytics.Model.TestPointHistorySnapshot>().HasKey(i => new
      {
        PartitionId = i.PartitionId,
        TestPointSK = i.TestPointSK,
        DateSK = i.DateSK
      });
      modelBuilder.Entity<PipelineRun>().HasKey(i => new
      {
        PartitionId = i.PartitionId,
        PipelineRunSK = i.PipelineRunSK
      });
      modelBuilder.Entity<PipelineRunActivityResult>().HasKey(i => new
      {
        PartitionId = i.PartitionId,
        PipelineRunActivityResultSK = i.PipelineRunActivityResultSK
      });
      modelBuilder.Entity<Branch>().HasKey(i => new
      {
        PartitionId = i.PartitionId,
        BranchSK = i.BranchSK
      });
      modelBuilder.Entity<Test>().HasKey(i => new
      {
        PartitionId = i.PartitionId,
        TestSK = i.TestSK
      });
      modelBuilder.Entity<Microsoft.VisualStudio.Services.Analytics.Model.ParallelPipelineJobsSnapshot>().HasKey(i => new
      {
        PartitionId = i.PartitionId,
        SamplingDate = i.SamplingDate,
        ParallelismTag = i.ParallelismTag,
        IsHosted = i.IsHosted
      });
      modelBuilder.Entity<TaskAgentRequestSnapshot>().HasKey(i => new
      {
        PartitionId = i.PartitionId,
        SamplingDateSK = i.SamplingDateSK
      });
      modelBuilder.Entity<Process>().HasKey(i => new
      {
        PartitionId = i.PartitionId,
        ProcessSK = i.ProcessSK
      });
      modelBuilder.Entity<Pipeline>().HasKey(i => new
      {
        PartitionId = i.PartitionId,
        PipelineSK = i.PipelineSK
      });
      modelBuilder.Entity<PipelineTask>().HasKey(i => new
      {
        PartitionId = i.PartitionId,
        PipelineTaskSK = i.PipelineTaskSK
      });
      modelBuilder.Entity<PipelineJob>().HasKey(i => new
      {
        PartitionId = i.PartitionId,
        PipelineJobSK = i.PipelineJobSK
      });
      modelBuilder.Entity<PipelineEnvironment>().HasKey(i => new
      {
        PartitionId = i.PartitionId,
        EnvironmentSK = i.EnvironmentSK
      });
      modelBuilder.Entity<TaskAgentPoolSizeSnapshot>().HasKey(i => new
      {
        PartitionId = i.PartitionId,
        PoolId = i.PoolId,
        SamplingDate = i.SamplingDate
      });
      modelBuilder.Entity<Microsoft.VisualStudio.Services.Analytics.Model.CommitToDeployment>().HasKey(i => new
      {
        PartitionId = i.PartitionId,
        CommitToDeploymentSK = i.CommitToDeploymentSK
      });
      modelBuilder.Entity<Microsoft.VisualStudio.Services.Analytics.Model.GitHubPullRequest>().HasKey(i => new
      {
        PartitionId = i.PartitionId,
        PullRequestSK = i.PullRequestSK
      });
      modelBuilder.Entity<Microsoft.VisualStudio.Services.Analytics.Model.GitHubPullRequestSnapshot>().HasKey(i => new
      {
        PartitionId = i.PartitionId,
        PullRequestSK = i.PullRequestSK,
        DateSK = i.DateSK
      });
      modelBuilder.Entity<GitHubPullRequestReviewerActivity>().HasKey(i => new
      {
        PartitionId = i.PartitionId,
        PullRequestReviewerActivitySK = i.PullRequestReviewerActivitySK
      });
      modelBuilder.Entity<Microsoft.VisualStudio.Services.Analytics.Model.GitHubBranch>().HasKey(i => new
      {
        PartitionId = i.PartitionId,
        BranchSK = i.BranchSK
      });
      modelBuilder.Entity<GitHubAccount>().HasKey(i => new
      {
        PartitionId = i.PartitionId,
        AccountSK = i.AccountSK
      });
      modelBuilder.Entity<GitHubRepository>().HasKey(i => new
      {
        PartitionId = i.PartitionId,
        RepositorySK = i.RepositorySK
      });
      modelBuilder.Entity<GitHubTeam>().HasKey(i => new
      {
        PartitionId = i.PartitionId,
        TeamSK = i.TeamSK
      });
      modelBuilder.Entity<GitHubUser>().HasKey(i => new
      {
        PartitionId = i.PartitionId,
        UserSK = i.UserSK
      });
      modelBuilder.Entity<GitHubCommit>().HasKey(i => new
      {
        PartitionId = i.PartitionId,
        CommitSK = i.CommitSK
      });
      modelBuilder.Entity<GitHubUser>().HasMany<GitHubTeam>((Expression<Func<GitHubUser, ICollection<GitHubTeam>>>) (user => user.Teams)).WithMany().Map((Action<ManyToManyAssociationMappingConfiguration>) (m =>
      {
        m.ToTable("ef_GitHubTeamUser", "AnalyticsInternal");
        m.MapLeftKey("UserPartitionId", "UserSK");
        m.MapRightKey("TeamPartitionId", "TeamSK");
      }));
      modelBuilder.Entity<GitHubContributorActivity>().HasKey(i => new
      {
        PartitionId = i.PartitionId,
        ContributorActivitySK = i.ContributorActivitySK
      });
      modelBuilder.Entity<WorkItemRevision>().HasMany<BoardLocation>((Expression<Func<WorkItemRevision, ICollection<BoardLocation>>>) (wit => wit.BoardLocations)).WithMany().Map((Action<ManyToManyAssociationMappingConfiguration>) (m =>
      {
        m.ToTable("ef_WorkItemRevisionBoardLocation", "AnalyticsInternal");
        m.MapLeftKey("WorkItemRevisionPartitionId", "WorkItemRevisionSK");
        m.MapRightKey("BoardLocationPartitionId", "BoardLocationSK");
      }));
      modelBuilder.Entity<WorkItem>().HasMany<BoardLocation>((Expression<Func<WorkItem, ICollection<BoardLocation>>>) (wit => wit.BoardLocations)).WithMany((Expression<Func<BoardLocation, ICollection<WorkItem>>>) (bl => bl.WorkItems)).Map((Action<ManyToManyAssociationMappingConfiguration>) (m =>
      {
        m.ToTable("ef_WorkItemBoardLocation", "AnalyticsInternal");
        m.MapLeftKey("WorkItemPartitionId", "WorkItemRevisionSK");
        m.MapRightKey("BoardLocationPartitionId", "BoardLocationSK");
      }));
      modelBuilder.Entity<WorkItemRevision>().HasMany<Tag>((Expression<Func<WorkItemRevision, ICollection<Tag>>>) (wit => wit.Tags)).WithMany().Map((Action<ManyToManyAssociationMappingConfiguration>) (m =>
      {
        m.ToTable("ef_WorkItemRevisionTag", "AnalyticsInternal");
        m.MapLeftKey("WorkItemRevisionPartitionId", "WorkItemRevisionSK");
        m.MapRightKey("TagPartitionId", "TagSK");
      }));
      modelBuilder.Entity<WorkItem>().HasMany<Tag>((Expression<Func<WorkItem, ICollection<Tag>>>) (wit => wit.Tags)).WithMany().Map((Action<ManyToManyAssociationMappingConfiguration>) (m =>
      {
        m.ToTable("ef_WorkItemTag", "AnalyticsInternal");
        m.MapLeftKey("WorkItemPartitionId", "WorkItemRevisionSK");
        m.MapRightKey("TagPartitionId", "TagSK");
      }));
      modelBuilder.Entity<Microsoft.VisualStudio.Services.Analytics.Model.WorkItemBoardSnapshot>().HasMany<Tag>((Expression<Func<Microsoft.VisualStudio.Services.Analytics.Model.WorkItemBoardSnapshot, ICollection<Tag>>>) (wit => wit.Tags)).WithMany().Map((Action<ManyToManyAssociationMappingConfiguration>) (m =>
      {
        m.ToTable("ef_WorkItemBoardSnapshotTag", "AnalyticsInternal");
        m.MapLeftKey("WorkItemPartitionId", "WorkItemRevisionSK", "DateSK", "BoardLocationSK");
        m.MapRightKey("TagPartitionId", "TagSK");
      }));
      modelBuilder.Entity<Microsoft.VisualStudio.Services.Analytics.Model.WorkItemSnapshot>().HasMany<Tag>((Expression<Func<Microsoft.VisualStudio.Services.Analytics.Model.WorkItemSnapshot, ICollection<Tag>>>) (wit => wit.Tags)).WithMany().Map((Action<ManyToManyAssociationMappingConfiguration>) (m =>
      {
        m.ToTable("ef_WorkItemSnapshotTag", "AnalyticsInternal");
        m.MapLeftKey("WorkItemPartitionId", "WorkItemRevisionSK", "DateSK");
        m.MapRightKey("TagPartitionId", "TagSK");
      }));
      modelBuilder.Entity<Team>().HasMany<Area>((Expression<Func<Team, ICollection<Area>>>) (t => t.Areas)).WithMany((Expression<Func<Area, ICollection<Team>>>) (a => a.Teams)).Map((Action<ManyToManyAssociationMappingConfiguration>) (m =>
      {
        m.ToTable("ef_TeamArea", "AnalyticsInternal");
        m.MapLeftKey("TeamPartitionId", "TeamSK");
        m.MapRightKey("AreaPartitionId", "AreaSK");
      }));
      modelBuilder.Entity<Team>().HasMany<Iteration>((Expression<Func<Team, ICollection<Iteration>>>) (t => t.Iterations)).WithMany((Expression<Func<Iteration, ICollection<Team>>>) (i => i.Teams)).Map((Action<ManyToManyAssociationMappingConfiguration>) (m =>
      {
        m.ToTable("ef_TeamIteration", "AnalyticsInternal");
        m.MapLeftKey("TeamPartitionId", "TeamSK");
        m.MapRightKey("IterationPartitionId", "IterationSK");
      }));
      if (serviceVersion >= 41)
        modelBuilder.Entity<WorkItem>().HasMany<WorkItem>((Expression<Func<WorkItem, ICollection<WorkItem>>>) (wit => wit.Descendants)).WithMany().Map((Action<ManyToManyAssociationMappingConfiguration>) (m =>
        {
          m.ToTable("ef_WorkItemDescendant", "AnalyticsInternal");
          m.MapLeftKey("WorkItemPartitionId", "WorkItemId");
          m.MapRightKey("DescendantPartitionId", "DescendantWorkItemId");
        }));
      if (serviceVersion >= 34)
      {
        modelBuilder.Entity<WorkItem>().HasMany<Team>((Expression<Func<WorkItem, ICollection<Team>>>) (wit => wit.Teams)).WithMany().Map((Action<ManyToManyAssociationMappingConfiguration>) (m =>
        {
          m.ToTable("ef_WorkItemTeam", "AnalyticsInternal");
          m.MapLeftKey("WorkItemPartitionId", "TeamFieldSK");
          m.MapRightKey("TeamPartitionId", "TeamSK");
        }));
        modelBuilder.Entity<WorkItemRevision>().HasMany<Team>((Expression<Func<WorkItemRevision, ICollection<Team>>>) (wit => wit.Teams)).WithMany().Map((Action<ManyToManyAssociationMappingConfiguration>) (m =>
        {
          m.ToTable("ef_WorkItemRevisionTeam", "AnalyticsInternal");
          m.MapLeftKey("WorkItemRevisionPartitionId", "TeamFieldSK");
          m.MapRightKey("TeamPartitionId", "TeamSK");
        }));
        modelBuilder.Entity<Microsoft.VisualStudio.Services.Analytics.Model.WorkItemSnapshot>().HasMany<Team>((Expression<Func<Microsoft.VisualStudio.Services.Analytics.Model.WorkItemSnapshot, ICollection<Team>>>) (wit => wit.Teams)).WithMany().Map((Action<ManyToManyAssociationMappingConfiguration>) (m =>
        {
          m.ToTable("ef_WorkItemSnapshotTeam", "AnalyticsInternal");
          m.MapLeftKey("WorkItemPartitionId", "TeamFieldSK", "DateSK");
          m.MapRightKey("TeamPartitionId", "TeamSK");
        }));
      }
      else
      {
        modelBuilder.Entity<WorkItem>().HasMany<Team>((Expression<Func<WorkItem, ICollection<Team>>>) (wit => wit.Teams)).WithMany().Map((Action<ManyToManyAssociationMappingConfiguration>) (m =>
        {
          m.ToTable("ef_WorkItemTeam", "AnalyticsInternal");
          m.MapLeftKey("WorkItemPartitionId", "AreaSK");
          m.MapRightKey("TeamPartitionId", "TeamSK");
        }));
        modelBuilder.Entity<WorkItemRevision>().HasMany<Team>((Expression<Func<WorkItemRevision, ICollection<Team>>>) (wit => wit.Teams)).WithMany().Map((Action<ManyToManyAssociationMappingConfiguration>) (m =>
        {
          m.ToTable("ef_WorkItemRevisionTeam", "AnalyticsInternal");
          m.MapLeftKey("WorkItemRevisionPartitionId", "AreaSK");
          m.MapRightKey("TeamPartitionId", "TeamSK");
        }));
        modelBuilder.Entity<Microsoft.VisualStudio.Services.Analytics.Model.WorkItemSnapshot>().HasMany<Team>((Expression<Func<Microsoft.VisualStudio.Services.Analytics.Model.WorkItemSnapshot, ICollection<Team>>>) (wit => wit.Teams)).WithMany().Map((Action<ManyToManyAssociationMappingConfiguration>) (m =>
        {
          m.ToTable("ef_WorkItemSnapshotTeam", "AnalyticsInternal");
          m.MapLeftKey("WorkItemPartitionId", "AreaSK", "DateSK");
          m.MapRightKey("TeamPartitionId", "TeamSK");
        }));
      }
      modelBuilder.Entity<WorkItem>().HasMany<Process>((Expression<Func<WorkItem, ICollection<Process>>>) (wit => wit.Processes)).WithMany().Map((Action<ManyToManyAssociationMappingConfiguration>) (m =>
      {
        m.ToTable("ef_WorkItemProcess", "AnalyticsInternal");
        m.MapLeftKey("WorkItemPartitionId", "WorkItemRevisionSK");
        m.MapRightKey("ProcessPartitionId", "ProcessSK");
      }));
      modelBuilder.Entity<WorkItemRevision>().HasMany<Process>((Expression<Func<WorkItemRevision, ICollection<Process>>>) (wit => wit.Processes)).WithMany().Map((Action<ManyToManyAssociationMappingConfiguration>) (m =>
      {
        m.ToTable("ef_WorkItemRevisionProcess", "AnalyticsInternal");
        m.MapLeftKey("WorkItemRevisionPartitionId", "WorkItemRevisionSK");
        m.MapRightKey("ProcessPartitionId", "ProcessSK");
      }));
      modelBuilder.Entity<Microsoft.VisualStudio.Services.Analytics.Model.WorkItemSnapshot>().HasMany<Process>((Expression<Func<Microsoft.VisualStudio.Services.Analytics.Model.WorkItemSnapshot, ICollection<Process>>>) (wit => wit.Processes)).WithMany().Map((Action<ManyToManyAssociationMappingConfiguration>) (m =>
      {
        m.ToTable("ef_WorkItemSnapshotProcess", "AnalyticsInternal");
        m.MapLeftKey("WorkItemSnapshotPartitionId", "WorkItemRevisionSK", "DateSK");
        m.MapRightKey("ProcessPartitionId", "ProcessSK");
      }));
    }

    internal void SetComponent(AnalyticsComponent component) => this.Component = component;

    internal class ClrTypeConvention : Convention
    {
      public ClrTypeConvention() => this.Types().Configure((Action<ConventionTypeConfiguration>) (ctc => ctc.HasTableAnnotation("ClrTypeAnnotation", (object) new ClrTypeAnnotation()
      {
        ClrType = ctc.ClrType,
        ProjectFilterExclusion = System.Reflection.CustomAttributeExtensions.GetCustomAttributes<DisableProjectFilteringAttribute>(ctc.ClrType).Any<DisableProjectFilteringAttribute>()
      })));
    }
  }
}

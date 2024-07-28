// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.ProcessTemplates.ProcessTemplateComponent
// Assembly: Microsoft.Azure.Boards.ProcessTemplates, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A94E8BA8-9851-4F5D-B619-9CF2FFF5B128
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.ProcessTemplates.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.Azure.Boards.ProcessTemplates
{
  public class ProcessTemplateComponent : TeamFoundationSqlResourceComponent
  {
    private static readonly IDictionary<int, SqlExceptionFactory> s_sqlExceptionFactories;
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[11]
    {
      (IComponentCreator) new ComponentCreator<ProcessTemplateComponent>(1, true),
      (IComponentCreator) new ComponentCreator<ProcessTemplateComponent>(2),
      (IComponentCreator) new ComponentCreator<ProcessTemplateComponent>(3),
      (IComponentCreator) new ComponentCreator<ProcessTemplateComponent>(4),
      (IComponentCreator) new ComponentCreator<ProcessTemplateComponent>(5),
      (IComponentCreator) new ComponentCreator<ProcessTemplateComponent6>(6),
      (IComponentCreator) new ComponentCreator<ProcessTemplateComponent7>(7),
      (IComponentCreator) new ComponentCreator<ProcessTemplateComponent8>(8),
      (IComponentCreator) new ComponentCreator<ProcessTemplateComponent9>(9),
      (IComponentCreator) new ComponentCreator<ProcessTemplateComponent10>(10),
      (IComponentCreator) new ComponentCreator<ProcessTemplateComponent11>(11)
    }, "ProcessTemplate");

    static ProcessTemplateComponent()
    {
      ProcessTemplateComponent.s_sqlExceptionFactories = (IDictionary<int, SqlExceptionFactory>) new Dictionary<int, SqlExceptionFactory>();
      ProcessTemplateComponent.s_sqlExceptionFactories = (IDictionary<int, SqlExceptionFactory>) new Dictionary<int, SqlExceptionFactory>();
      ProcessTemplateComponent.s_sqlExceptionFactories[1300800] = new SqlExceptionFactory(typeof (ProcessNotFoundException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((_1, _2, _3, _4) => (Exception) new ProcessNotFoundException()));
      ProcessTemplateComponent.s_sqlExceptionFactories[1300801] = new SqlExceptionFactory(typeof (ProcessInvalidStateTransitionException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((_1, _2, _3, _4) => (Exception) new ProcessInvalidStateTransitionException()));
      ProcessTemplateComponent.s_sqlExceptionFactories[1300802] = new SqlExceptionFactory(typeof (ProcessLimitExceededException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((_1, _2, _3, _4) => (Exception) new ProcessLimitExceededException()));
      ProcessTemplateComponent.s_sqlExceptionFactories[1300803] = new SqlExceptionFactory(typeof (ProcessNameConflictException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((_1, _2, _3, _4) => (Exception) new ProcessNameConflictException()));
      ProcessTemplateComponent.s_sqlExceptionFactories[1300804] = new SqlExceptionFactory(typeof (ProcessRefNameConflictException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((_1, _2, _3, _4) => (Exception) new ProcessRefNameConflictException()));
    }

    public ProcessTemplateComponent() => this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => ProcessTemplateComponent.s_sqlExceptionFactories;

    protected ProcessScope Scope => this.RequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) ? ProcessScope.Collection : ProcessScope.Deployment;

    public Guid? NotificationAuthor { get; set; }

    protected int ConvertToDatabaseIntegerId(int integerId) => integerId - ProcessTemplateComponent.GetScopeIntegerId(this.Scope);

    protected virtual ObjectBinder<ProcessTemplateDescriptorEntry> GetProcessTemplateDescriptorRowBinder() => (ObjectBinder<ProcessTemplateDescriptorEntry>) new ProcessTemplateComponent.ProcessTemplateDescriptorRowBinder1(this.Scope);

    protected virtual ServiceLevel GetServiceLevel()
    {
      string serviceLevel = this.RequestContext.ServiceHost.ServiceHostInternal().ServiceLevel;
      if (serviceLevel == null)
        return (ServiceLevel) null;
      return new ServiceLevel(((IEnumerable<string>) serviceLevel.TrimEnd(';').Split(';')).Last<string>());
    }

    protected string GetServiceLevelString(ServiceLevel serviceLevel) => !(serviceLevel != (ServiceLevel) null) ? "" : serviceLevel.ToString();

    public virtual void UpdateProcessStatuses(IEnumerable<Guid> typeIds, ProcessStatus status) => throw new NotSupportedException();

    public virtual ProcessTemplateDescriptorEntry UpdateProcessTemplateDescriptor(
      ServiceLevel serviceLevel,
      string name,
      string description,
      int majorVersion,
      int minorVersion,
      Guid processTypeId,
      byte[] fileHashValue,
      int fileId,
      string referenceName,
      Guid? baseTypeId,
      Guid? changedBy)
    {
      this.PrepareStoredProcedure("prc_SaveProcessTemplateDescriptor");
      this.BindString("@name", name, 256, false, SqlDbType.NVarChar);
      this.BindString("@description", description, 1024, false, SqlDbType.NVarChar);
      this.BindInt("@majorVersion", majorVersion);
      this.BindInt("@minorVersion", minorVersion);
      this.BindGuid("@type", processTypeId);
      this.BindBinary("@fileHashValue", fileHashValue, 16, SqlDbType.Binary);
      this.BindString("@plugins", "", -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindInt("@fileId", fileId);
      this.ExecuteNonQuery();
      return this.GetAllProcessDescriptors((ServiceLevel) null).Where<ProcessTemplateDescriptorEntry>((System.Func<ProcessTemplateDescriptorEntry, bool>) (ptdr => ptdr.Name == name)).FirstOrDefault<ProcessTemplateDescriptorEntry>();
    }

    public virtual IReadOnlyCollection<ProcessTemplateDescriptorEntry> GetAllProcessDescriptors(
      ServiceLevel serviceLevel,
      bool onlyTipVersion = true)
    {
      string sqlStatement = "\r\nDECLARE @integerIdTemplates TABLE\r\n(\r\n    TemplateId UNIQUEIDENTIFIER PRIMARY KEY,\r\n    Name NVARCHAR(256),\r\n    Description NVARCHAR(MAX),\r\n    MajorVersion INT,\r\n    MinorVersion INT,\r\n    Type UNIQUEIDENTIFIER,\r\n    FileId INT,\r\n    FileHashValue BINARY(16),\r\n    Plugins NVARCHAR(MAX),\r\n    ServiceLevel VARCHAR(101),\r\n    OverriddenDate DATETIME,\r\n    IntegerId INT IDENTITY(1,1)\r\n)\r\nINSERT INTO @integerIdTemplates\r\nSELECT T.Id AS TemplateId,\r\n\tT.Name AS Name,\r\n\tT.Description AS Description,\r\n\tT.MajorVersion AS MajorVersion,\r\n\tT.MinorVersion AS MinorVersion,\r\n\tT.Type AS Type,\r\n\tT.FileId AS FileId,\r\n\tT.FileHashValue AS FileHashValue,\r\n\tT.Plugins AS Plugins,\r\n    '' AS ServiceLevel,\r\n    convert(datetime,'9999',126) AS OverriddenDate\r\nFROM tbl_ProcessTemplateDescriptor T\r\nWHERE T.PartitionId = @partitionId\r\nORDER BY T.Name\r\nOPTION (OPTIMIZE FOR (@partitionId UNKNOWN))\r\n\r\nSELECT *\r\nFROM @integerIdTemplates\r\n            ";
      this.PrepareSqlBatch(sqlStatement.Length, true);
      this.AddStatement(sqlStatement);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<ProcessTemplateDescriptorEntry>(this.GetProcessTemplateDescriptorRowBinder());
      List<ProcessTemplateDescriptorEntry> items = resultCollection.GetCurrent<ProcessTemplateDescriptorEntry>().Items;
      List<ProcessTemplateDescriptorEntry> entries = new List<ProcessTemplateDescriptorEntry>();
      foreach (IGrouping<Guid, ProcessTemplateDescriptorEntry> grouping in items.GroupBy<ProcessTemplateDescriptorEntry, Guid>((System.Func<ProcessTemplateDescriptorEntry, Guid>) (template => template.TypeId)))
      {
        if (grouping.Key == Guid.Empty)
        {
          entries.AddRange((IEnumerable<ProcessTemplateDescriptorEntry>) grouping);
        }
        else
        {
          ProcessTemplateDescriptorEntry templateDescriptorEntry = grouping.GroupBy<ProcessTemplateDescriptorEntry, int>((System.Func<ProcessTemplateDescriptorEntry, int>) (template => template.MajorVersion)).OrderByDescending<IGrouping<int, ProcessTemplateDescriptorEntry>, int>((System.Func<IGrouping<int, ProcessTemplateDescriptorEntry>, int>) (templateGroup => templateGroup.Key)).First<IGrouping<int, ProcessTemplateDescriptorEntry>>().OrderByDescending<ProcessTemplateDescriptorEntry, int>((System.Func<ProcessTemplateDescriptorEntry, int>) (template => template.MinorVersion)).First<ProcessTemplateDescriptorEntry>();
          entries.Add(templateDescriptorEntry);
        }
      }
      return ProcessTemplateComponent.FilterDescriptors((IEnumerable<ProcessTemplateDescriptorEntry>) entries, serviceLevel, onlyTipVersion);
    }

    public virtual Guid? GetProcessDescriptorSpecificId(int integerId)
    {
      string sqlStatement = "\r\nDECLARE @integerIdTemplates TABLE\r\n(\r\n\tTemplateId UNIQUEIDENTIFIER PRIMARY KEY,\r\n\tName NVARCHAR(256),\r\n\tDescription NVARCHAR(MAX),\r\n\tMajorVersion INT,\r\n\tMinorVersion INT,\r\n\tType UNIQUEIDENTIFIER,\r\n\tFileId INT,\r\n\tFileHashValue BINARY(16),\r\n\tPlugins NVARCHAR(MAX),\r\n    ServiceLevel VARCHAR(101),\r\n    OverriddenDate DATETIME,\r\n    IntegerId INT IDENTITY(1,1)\r\n)\r\nINSERT INTO @integerIdTemplates\r\nSELECT T.Id AS TemplateId,\r\n\tT.Name AS Name,\r\n\tT.Description AS Description,\r\n\tT.MajorVersion AS MajorVersion,\r\n\tT.MinorVersion AS MinorVersion,\r\n\tT.Type AS Type,\r\n\tT.FileId AS FileId,\r\n\tT.FileHashValue AS FileHashValue,\r\n\tT.Plugins AS Plugins,\r\n    '' AS ServiceLevel,\r\n    convert(datetime,'9999',126) AS OverriddenDate\r\nFROM tbl_ProcessTemplateDescriptor T\r\nWHERE T.PartitionId = @partitionId\r\nORDER BY T.Name\r\nOPTION (OPTIMIZE FOR (@partitionId UNKNOWN))\r\n\r\nSELECT TemplateId\r\nFROM @integerIdTemplates\r\nWHERE IntegerId = @integerId\r\n            ";
      this.PrepareSqlBatch(sqlStatement.Length, true);
      this.AddStatement(sqlStatement);
      this.BindInt("@integerId", this.ConvertToDatabaseIntegerId(integerId));
      SqlDataReader sqlDataReader = this.ExecuteReader();
      return sqlDataReader.Read() ? new Guid?(sqlDataReader.GetGuid(0)) : new Guid?();
    }

    public ProcessTemplateDescriptorEntry GetSpecificProcessDescriptor(Guid templateId) => this.GetSpecificProcessDescriptors((IEnumerable<Guid>) new Guid[1]
    {
      templateId
    }).FirstOrDefault<ProcessTemplateDescriptorEntry>();

    public virtual IReadOnlyCollection<ProcessTemplateDescriptorEntry> GetSpecificProcessDescriptors(
      IEnumerable<Guid> templateIds)
    {
      string sqlStatement = "\r\nDECLARE @integerIdTemplates TABLE\r\n(\r\n\tTemplateId UNIQUEIDENTIFIER PRIMARY KEY,\r\n\tName NVARCHAR(256),\r\n\tDescription NVARCHAR(MAX),\r\n\tMajorVersion INT,\r\n\tMinorVersion INT,\r\n\tType UNIQUEIDENTIFIER,\r\n\tFileId INT,\r\n\tFileHashValue BINARY(16),\r\n\tPlugins NVARCHAR(MAX),\r\n    ServiceLevel VARCHAR(101),\r\n    OverriddenDate DATETIME,\r\n    IntegerId INT IDENTITY(1,1)\r\n)\r\nINSERT INTO @integerIdTemplates\r\nSELECT T.Id AS TemplateId,\r\n\tT.Name AS Name,\r\n\tT.Description AS Description,\r\n\tT.MajorVersion AS MajorVersion,\r\n\tT.MinorVersion AS MinorVersion,\r\n\tT.Type AS Type,\r\n\tT.FileId AS FileId,\r\n\tT.FileHashValue AS FileHashValue,\r\n\tT.Plugins AS Plugins,\r\n    '' AS ServiceLevel,\r\n    convert(datetime,'9999',126) AS OverriddenDate\r\nFROM tbl_ProcessTemplateDescriptor T\r\nWHERE T.PartitionId = @partitionId\r\nORDER BY T.Name\r\nOPTION (OPTIMIZE FOR (@partitionId UNKNOWN))\r\n\r\nSELECT T.*\r\nFROM @integerIdTemplates T\r\nJOIN @templateIds I\r\n    ON I.Id = T.TemplateId\r\n            ";
      this.PrepareSqlBatch(sqlStatement.Length, true);
      this.AddStatement(sqlStatement);
      this.BindGuidTable("@templateIds", templateIds);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<ProcessTemplateDescriptorEntry>(this.GetProcessTemplateDescriptorRowBinder());
      return (IReadOnlyCollection<ProcessTemplateDescriptorEntry>) resultCollection.GetCurrent<ProcessTemplateDescriptorEntry>().Items;
    }

    protected static IReadOnlyCollection<ProcessTemplateDescriptorEntry> FilterDescriptors(
      IEnumerable<ProcessTemplateDescriptorEntry> entries,
      ServiceLevel serviceLevel,
      bool onlyTipVersion = true)
    {
      if (serviceLevel != (ServiceLevel) null)
        entries = entries.Where<ProcessTemplateDescriptorEntry>((System.Func<ProcessTemplateDescriptorEntry, bool>) (entry => string.IsNullOrEmpty(entry.ServiceLevel) || new ServiceLevel(entry.ServiceLevel) <= serviceLevel));
      if (onlyTipVersion)
        entries = entries.GroupBy<ProcessTemplateDescriptorEntry, Guid>((System.Func<ProcessTemplateDescriptorEntry, Guid>) (entry => entry.TypeId)).Select<IGrouping<Guid, ProcessTemplateDescriptorEntry>, ProcessTemplateDescriptorEntry>((System.Func<IGrouping<Guid, ProcessTemplateDescriptorEntry>, ProcessTemplateDescriptorEntry>) (group => group.OrderByDescending<ProcessTemplateDescriptorEntry, DateTime>((System.Func<ProcessTemplateDescriptorEntry, DateTime>) (entry => entry.RevisedDate)).First<ProcessTemplateDescriptorEntry>()));
      return (IReadOnlyCollection<ProcessTemplateDescriptorEntry>) entries.ToList<ProcessTemplateDescriptorEntry>();
    }

    public virtual void SetDefaultProcessType(Guid processTypeId, Guid changedBy)
    {
    }

    public virtual void EnableDisableProcess(Guid processTypeId, bool isEnabled, Guid changedBy) => this.UpdateProcessProperties(processTypeId, "IsEnabled", isEnabled ? bool.TrueString : bool.FalseString, changedBy);

    public virtual Guid? GetProcessDescriptorSpecificId(
      Guid typeId,
      int majorVersion,
      int minorVersion)
    {
      string sqlStatement = "\r\nDECLARE @integerIdTemplates TABLE\r\n(\r\n\tTemplateId UNIQUEIDENTIFIER PRIMARY KEY,\r\n\tName NVARCHAR(256),\r\n\tDescription NVARCHAR(MAX),\r\n\tMajorVersion INT,\r\n\tMinorVersion INT,\r\n\tType UNIQUEIDENTIFIER,\r\n\tFileId INT,\r\n\tFileHashValue BINARY(16),\r\n\tPlugins NVARCHAR(MAX),\r\n    ServiceLevel VARCHAR(101),\r\n    OverriddenDate DATETIME,\r\n    IntegerId INT IDENTITY(1,1)\r\n)\r\nINSERT INTO @integerIdTemplates\r\nSELECT T.Id AS TemplateId,\r\n\tT.Name AS Name,\r\n\tT.Description AS Description,\r\n\tT.MajorVersion AS MajorVersion,\r\n\tT.MinorVersion AS MinorVersion,\r\n\tT.Type AS Type,\r\n\tT.FileId AS FileId,\r\n\tT.FileHashValue AS FileHashValue,\r\n\tT.Plugins AS Plugins,\r\n    '' AS ServiceLevel,\r\n    convert(datetime,'9999',126) AS OverriddenDate\r\nFROM tbl_ProcessTemplateDescriptor T\r\nWHERE T.PartitionId = @partitionId\r\nORDER BY T.Name\r\nOPTION (OPTIMIZE FOR (@partitionId UNKNOWN))\r\n\r\nSELECT TemplateId\r\nFROM @integerIdTemplates\r\nWHERE Type = @typeId\r\n    AND MajorVersion = @majorVersion\r\n    AND MinorVersion = @minorVersion\r\n            ";
      this.PrepareSqlBatch(sqlStatement.Length, true);
      this.AddStatement(sqlStatement);
      this.BindGuid("@typeId", typeId);
      this.BindInt("@majorVersion", majorVersion);
      this.BindInt("@minorVersion", minorVersion);
      SqlDataReader sqlDataReader = this.ExecuteReader();
      return sqlDataReader.Read() ? new Guid?(sqlDataReader.GetGuid(0)) : new Guid?();
    }

    public virtual Guid GetDefaultProcessTypeId() => new Guid("6B724908-EF14-45CF-84F8-768B5384DA45");

    public virtual void DeleteProcess(Guid specificProcessId, Guid processTypeId) => this.DeleteProcessProperties(processTypeId);

    protected virtual void DeleteProcessProperties(Guid processTypeId)
    {
    }

    protected static int GetScopeIntegerId(ProcessScope scope)
    {
      if (scope == ProcessScope.Collection)
        return 0;
      if (scope == ProcessScope.Deployment)
        ;
      return 536870912;
    }

    public virtual ICollection<ProcessTemplateDescriptorEntry> GetProcessHistory(Guid typeId)
    {
      string sqlStatement = "\r\nSELECT T.Id AS TemplateId,\r\n    T.Name AS Name,\r\n    T.Description AS Description,\r\n    T.MajorVersion AS MajorVersion,\r\n    T.MinorVersion AS MinorVersion,\r\n    T.Type AS Type,\r\n    T.FileId AS FileId,\r\n    T.FileHashValue AS FileHashValue,\r\n    T.Plugins AS Plugins,\r\n    T.IntegerId AS IntegerId,\r\n    T.ServiceLevel AS ServiceLevel,\r\n    T.OverriddenDate AS OverriddenDate\r\nFROM [dbo].[tbl_ProcessTemplateDescriptor] T\r\nWHERE T.PartitionId = @partitionId\r\n    AND T.Type = @typeId\r\nOPTION (OPTIMIZE FOR (@partitionId UNKNOWN))\r\n            ";
      this.PrepareSqlBatch(sqlStatement.Length, true);
      this.AddStatement(sqlStatement);
      this.BindGuid("@typeId", typeId);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<ProcessTemplateDescriptorEntry>(this.GetProcessTemplateDescriptorRowBinder());
      return (ICollection<ProcessTemplateDescriptorEntry>) resultCollection.GetCurrent<ProcessTemplateDescriptorEntry>().Items;
    }

    protected virtual ObjectBinder<ProcessPropertyEntry> GetProcessPropertiesRowBinder() => (ObjectBinder<ProcessPropertyEntry>) new ProcessTemplateComponent.ProcessPropertiesRowBinder();

    protected virtual IReadOnlyCollection<ProcessPropertyEntry> GetProcessProperties(
      Guid templateTypeId)
    {
      return (IReadOnlyCollection<ProcessPropertyEntry>) Array.Empty<ProcessPropertyEntry>();
    }

    protected virtual void UpdateProcessProperties(
      Guid processTypeId,
      string propertyName,
      string propertyValue,
      Guid updatedBy)
    {
    }

    protected virtual void UpdateDefaultProcessProperty(
      Guid processTypeId,
      string propertyName,
      string propertyValue,
      Guid updatedBy)
    {
    }

    public virtual ProcessTemplateDescriptorEntry UpdateProcessNameAndDescription(
      ProcessDescriptor descriptor,
      string newName,
      string newDescription,
      Guid changedBy)
    {
      if (newName != null && !TFStringComparer.ProcessName.Equals(newName, descriptor.Name) && this.GetAllProcessDescriptors((ServiceLevel) null).Any<ProcessTemplateDescriptorEntry>((System.Func<ProcessTemplateDescriptorEntry, bool>) (t => t.TypeId != descriptor.TypeId && TFStringComparer.ProcessName.Equals(t.Name, newName))))
        throw new ProcessNameConflictException(newName);
      return this.UpdateProcessTemplateDescriptor(this.GetServiceLevel(), newName ?? descriptor.Name, newDescription ?? descriptor.Description, descriptor.Version.Major, descriptor.Version.Minor, descriptor.TypeId, descriptor.HashValue, descriptor.FileId, descriptor.ReferenceName, new Guid?(descriptor.Inherits), new Guid?(changedBy));
    }

    public virtual ProcessTemplateDescriptorEntry CreateInheritedProcess(
      ProcessDescriptor parentDescriptor,
      Guid processTypeId,
      string name,
      string referenceName,
      string description,
      Guid changedBy)
    {
      IReadOnlyCollection<ProcessTemplateDescriptorEntry> processDescriptors = this.GetAllProcessDescriptors((ServiceLevel) null);
      if (processDescriptors.Any<ProcessTemplateDescriptorEntry>((System.Func<ProcessTemplateDescriptorEntry, bool>) (t => TFStringComparer.ProcessName.Equals(t.Name, name))))
        throw new ProcessNameConflictException(name);
      if (processDescriptors.Any<ProcessTemplateDescriptorEntry>((System.Func<ProcessTemplateDescriptorEntry, bool>) (t => TFStringComparer.ProcessReferenceName.Equals(t.ReferenceName, referenceName))))
        throw new ProcessRefNameConflictException(referenceName);
      return this.UpdateProcessTemplateDescriptor(this.GetServiceLevel(), name, description, 1, 0, processTypeId, parentDescriptor.HashValue, parentDescriptor.FileId, referenceName, new Guid?(parentDescriptor.TypeId), new Guid?(changedBy));
    }

    public virtual ProcessTemplateDescriptorEntry CreateOrUpdateLegacyProcess(
      Guid processTypeId,
      string name,
      string description,
      ProcessVersion version,
      byte[] hashValue,
      int fileId,
      Guid changedBy)
    {
      if (this.GetAllProcessDescriptors((ServiceLevel) null).Any<ProcessTemplateDescriptorEntry>((System.Func<ProcessTemplateDescriptorEntry, bool>) (t => t.TypeId != processTypeId && TFStringComparer.ProcessName.Equals(t.Name, name))))
        throw new ProcessNameConflictException(name);
      return this.UpdateProcessTemplateDescriptor(this.GetServiceLevel(), name, description, version.Major, version.Minor, processTypeId, hashValue, fileId, (string) null, new Guid?(), new Guid?(changedBy));
    }

    public virtual IReadOnlyCollection<Guid> GetDisabledProcessTypeIds() => (IReadOnlyCollection<Guid>) this.GetProcessProperties(Guid.Empty).Where<ProcessPropertyEntry>((System.Func<ProcessPropertyEntry, bool>) (p => StringComparer.OrdinalIgnoreCase.Equals(p.PropertyName, "IsEnabled") && StringComparer.OrdinalIgnoreCase.Equals(p.PropertyValue, bool.FalseString))).Select<ProcessPropertyEntry, Guid>((System.Func<ProcessPropertyEntry, Guid>) (p => p.TemplateTypeId)).ToList<Guid>();

    public virtual ProcessTemplateDescriptorEntry RestoreProcess(Guid processTypeId) => (ProcessTemplateDescriptorEntry) null;

    protected class ProcessTemplateDescriptorRowBinder1 : 
      ObjectBinder<ProcessTemplateDescriptorEntry>
    {
      private SqlColumnBinder m_TemplateIdCol = new SqlColumnBinder("TemplateId");
      private SqlColumnBinder m_NameCol = new SqlColumnBinder("Name");
      private SqlColumnBinder m_DescriptionCol = new SqlColumnBinder("Description");
      private SqlColumnBinder m_MajorVersionCol = new SqlColumnBinder("MajorVersion");
      private SqlColumnBinder m_MinorVersionCol = new SqlColumnBinder("MinorVersion");
      private SqlColumnBinder m_TypeCol = new SqlColumnBinder("Type");
      private SqlColumnBinder m_FileIdCol = new SqlColumnBinder("FileId");
      private SqlColumnBinder m_HashValueCol = new SqlColumnBinder("FileHashValue");
      private SqlColumnBinder m_IntegerIdCol = new SqlColumnBinder("IntegerId");
      private SqlColumnBinder m_ServiceLevelCol = new SqlColumnBinder("ServiceLevel");
      private SqlColumnBinder m_OverriddenDateCol = new SqlColumnBinder("OverriddenDate");

      public ProcessTemplateDescriptorRowBinder1(ProcessScope scope) => this.Scope = scope;

      protected ProcessScope Scope { get; private set; }

      protected int ConvertFromDatabaseIntegerId(int databaseIntegerId) => databaseIntegerId + ProcessTemplateComponent.GetScopeIntegerId(this.Scope);

      protected override ProcessTemplateDescriptorEntry Bind() => new ProcessTemplateDescriptorEntry()
      {
        Id = this.m_TemplateIdCol.GetGuid((IDataReader) this.Reader),
        Name = this.m_NameCol.GetString((IDataReader) this.Reader, false),
        Description = this.m_DescriptionCol.GetString((IDataReader) this.Reader, true),
        Scope = this.Scope,
        MajorVersion = this.m_MajorVersionCol.GetInt32((IDataReader) this.Reader),
        MinorVersion = this.m_MinorVersionCol.GetInt32((IDataReader) this.Reader),
        TypeId = this.m_TypeCol.GetGuid((IDataReader) this.Reader, false),
        FileId = this.m_FileIdCol.GetInt32((IDataReader) this.Reader),
        HashValue = this.m_HashValueCol.GetBytes((IDataReader) this.Reader, false),
        IntegerId = this.ConvertFromDatabaseIntegerId(this.m_IntegerIdCol.GetInt32((IDataReader) this.Reader)),
        RevisedDate = this.m_OverriddenDateCol.GetDateTime((IDataReader) this.Reader),
        ServiceLevel = this.m_ServiceLevelCol.GetString((IDataReader) this.Reader, false),
        IsDeleted = false,
        ProcessStatus = ProcessStatus.Ready
      };
    }

    protected class ProcessTemplateDescriptorRowBinder2 : 
      ProcessTemplateComponent.ProcessTemplateDescriptorRowBinder1
    {
      private SqlColumnBinder m_IsDeletedCol = new SqlColumnBinder("IsDeleted");
      private SqlColumnBinder m_ProcessStatusCol = new SqlColumnBinder("ProcessStatus");

      public ProcessTemplateDescriptorRowBinder2(ProcessScope scope)
        : base(scope)
      {
      }

      protected override ProcessTemplateDescriptorEntry Bind()
      {
        ProcessTemplateDescriptorEntry templateDescriptorEntry = base.Bind();
        templateDescriptorEntry.IsDeleted = this.m_IsDeletedCol.GetBoolean((IDataReader) this.Reader);
        templateDescriptorEntry.ProcessStatus = (ProcessStatus) this.m_ProcessStatusCol.GetInt32((IDataReader) this.Reader);
        return templateDescriptorEntry;
      }
    }

    protected class ProcessTemplateDescriptorRowBinder3 : 
      ProcessTemplateComponent.ProcessTemplateDescriptorRowBinder2
    {
      private SqlColumnBinder m_ReferenceNameCol = new SqlColumnBinder("ReferenceName");
      private SqlColumnBinder m_ExtensionDataCol = new SqlColumnBinder("ExtensionData");

      public ProcessTemplateDescriptorRowBinder3(ProcessScope scope)
        : base(scope)
      {
      }

      protected override ProcessTemplateDescriptorEntry Bind()
      {
        ProcessTemplateDescriptorEntry templateDescriptorEntry = base.Bind();
        templateDescriptorEntry.ReferenceName = this.m_ReferenceNameCol.GetString((IDataReader) this.Reader, true);
        string serializedObject = this.m_ExtensionDataCol.GetString((IDataReader) this.Reader, true);
        if (string.IsNullOrEmpty(serializedObject))
        {
          templateDescriptorEntry.Inherits = Guid.Empty;
        }
        else
        {
          ProcessExtensionData processExtensionData = TeamFoundationSerializationUtility.Deserialize<ProcessExtensionData>(serializedObject);
          templateDescriptorEntry.Inherits = processExtensionData.Inherits;
        }
        return templateDescriptorEntry;
      }
    }

    protected class ProcessPropertiesRowBinder : ObjectBinder<ProcessPropertyEntry>
    {
      private SqlColumnBinder m_TemplateTypeIdCol = new SqlColumnBinder("TemplateTypeId");
      private SqlColumnBinder m_PropertyNameCol = new SqlColumnBinder("PropertyName");
      private SqlColumnBinder m_PropertyValueCol = new SqlColumnBinder("PropertyValue");
      private SqlColumnBinder m_UpdatedByCol = new SqlColumnBinder("UpdatedBy");
      private SqlColumnBinder m_UpdateDateCol = new SqlColumnBinder("UpdateDate");

      protected override ProcessPropertyEntry Bind() => new ProcessPropertyEntry()
      {
        TemplateTypeId = this.m_TemplateTypeIdCol.GetGuid((IDataReader) this.Reader, true),
        PropertyName = this.m_PropertyNameCol.GetString((IDataReader) this.Reader, true),
        PropertyValue = this.m_PropertyValueCol.GetString((IDataReader) this.Reader, true),
        UpdatedBy = this.m_UpdatedByCol.GetGuid((IDataReader) this.Reader, true),
        UpdateDate = this.m_UpdateDateCol.GetDateTime((IDataReader) this.Reader)
      };
    }
  }
}

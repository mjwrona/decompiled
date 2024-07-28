// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseObjectDefinitionComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class DatabaseObjectDefinitionComponent : TeamFoundationSqlResourceComponent
  {
    public List<DatabaseObjectDefinitionComponent.DatabaseObjectDefinition> GetStoredProcedures(
      string nameFilter = "%",
      bool includeContent = true)
    {
      return this.GetDatabaseResources("stmt_QuerySprocDefinitions.sql", DatabaseObjectDefinitionComponent.SqlObjectType.StoredProcedure, nameFilter, includeContent);
    }

    public List<DatabaseObjectDefinitionComponent.DatabaseObjectDefinition> GetViews(
      string nameFilter = "%",
      bool includeContent = true)
    {
      return this.GetDatabaseResources("stmt_QueryViewDefinitions.sql", DatabaseObjectDefinitionComponent.SqlObjectType.View, nameFilter, includeContent);
    }

    public List<DatabaseObjectDefinitionComponent.DatabaseObjectDefinition> GetFunctions(
      string nameFilter = "%",
      bool includeContent = true)
    {
      return this.GetDatabaseResources("stmt_QueryFunctionDefinitions.sql", DatabaseObjectDefinitionComponent.SqlObjectType.Function, nameFilter, includeContent);
    }

    public List<DatabaseObjectDefinitionComponent.DatabaseObjectDefinition> GetScalarFunctions(
      string nameFilter = "%",
      bool includeContent = true)
    {
      return this.GetDatabaseResources("stmt_QueryScalarFunctionDefinitions.sql", DatabaseObjectDefinitionComponent.SqlObjectType.Function, nameFilter, includeContent);
    }

    public List<DatabaseObjectDefinitionComponent.DatabaseObjectDefinition> GetTriggers(
      string nameFilter = "%",
      bool includeContent = true)
    {
      return this.GetDatabaseResources("stmt_QueryTriggerDefinitions.sql", DatabaseObjectDefinitionComponent.SqlObjectType.Trigger, nameFilter, includeContent);
    }

    public List<DatabaseObjectDefinitionComponent.DatabaseObjectDefinition> GetTableTypes(
      string nameFilter = "%",
      bool includeContent = false)
    {
      return this.GetDatabaseResources("stmt_QueryTableTypeDefinitions.sql", DatabaseObjectDefinitionComponent.SqlObjectType.TableType, nameFilter, includeContent);
    }

    public List<DatabaseObjectDefinitionComponent.DatabaseObjectDefinition> GetTables(
      string nameFilter = "%",
      bool includeContent = false)
    {
      return this.GetDatabaseResources("stmt_QueryTableDefinitions.sql", DatabaseObjectDefinitionComponent.SqlObjectType.Table, nameFilter, includeContent);
    }

    private List<DatabaseObjectDefinitionComponent.DatabaseObjectDefinition> GetDatabaseResources(
      string sqlStatementFileName,
      DatabaseObjectDefinitionComponent.SqlObjectType type,
      string nameFilter,
      bool includeContent)
    {
      this.Logger.Info("Getting all requested objects from database.");
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString(sqlStatementFileName);
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      this.BindString("@nameFilter", nameFilter, 256, false, SqlDbType.NVarChar);
      this.BindBoolean("@includeContent", includeContent);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), sqlStatementFileName, (IVssRequestContext) null);
      resultCollection.AddBinder<DatabaseObjectDefinitionComponent.DatabaseObjectDefinitionRaw>((ObjectBinder<DatabaseObjectDefinitionComponent.DatabaseObjectDefinitionRaw>) new DatabaseObjectDefinitionComponent.DatabaseObjectColumns());
      List<DatabaseObjectDefinitionComponent.DatabaseObjectDefinition> databaseResources = this.ConvertFromRawDefinitions(resultCollection.GetCurrent<DatabaseObjectDefinitionComponent.DatabaseObjectDefinitionRaw>().Items, type);
      this.Logger.Info("Found {0} matching records in database", (object) databaseResources.Count);
      return databaseResources;
    }

    private List<DatabaseObjectDefinitionComponent.DatabaseObjectDefinition> ConvertFromRawDefinitions(
      List<DatabaseObjectDefinitionComponent.DatabaseObjectDefinitionRaw> rawItems,
      DatabaseObjectDefinitionComponent.SqlObjectType type)
    {
      List<DatabaseObjectDefinitionComponent.DatabaseObjectDefinition> objectDefinitionList = new List<DatabaseObjectDefinitionComponent.DatabaseObjectDefinition>();
      string str = string.Empty;
      DatabaseObjectDefinitionComponent.DatabaseObjectDefinition objectDefinition = (DatabaseObjectDefinitionComponent.DatabaseObjectDefinition) null;
      StringBuilder stringBuilder = new StringBuilder();
      foreach (DatabaseObjectDefinitionComponent.DatabaseObjectDefinitionRaw rawItem in rawItems)
      {
        if (!rawItem.FullName.Equals(str))
        {
          if (objectDefinition != null)
          {
            objectDefinition.Content = stringBuilder.ToString();
            objectDefinitionList.Add(objectDefinition);
          }
          objectDefinition = new DatabaseObjectDefinitionComponent.DatabaseObjectDefinition()
          {
            Name = rawItem.Name,
            Schema = rawItem.Schema,
            Type = type
          };
          stringBuilder = new StringBuilder(rawItem.Text);
          str = objectDefinition.FullName;
        }
        else
          stringBuilder.Append(rawItem.Text);
      }
      if (objectDefinition != null)
      {
        objectDefinition.Content = stringBuilder.ToString();
        objectDefinitionList.Add(objectDefinition);
      }
      return objectDefinitionList;
    }

    public class DatabaseObjectDefinition
    {
      public string Name { get; set; }

      public string Schema { get; set; }

      public string Content { get; set; }

      public DatabaseObjectDefinitionComponent.SqlObjectType Type { get; set; }

      public string FullName => string.Format("{0}.{1}", (object) StringUtil.QuoteName(this.Schema), (object) StringUtil.QuoteName(this.Name));
    }

    internal class DatabaseObjectDefinitionRaw
    {
      public string Name { get; set; }

      public string Schema { get; set; }

      public int SectionId { get; set; }

      public string Text { get; set; }

      public string FullName => string.Format("{0}.{1}", (object) StringUtil.QuoteName(this.Schema), (object) StringUtil.QuoteName(this.Name));
    }

    public enum SqlObjectType
    {
      StoredProcedure,
      Function,
      View,
      Trigger,
      TableType,
      Table,
    }

    internal class DatabaseObjectColumns : 
      ObjectBinder<DatabaseObjectDefinitionComponent.DatabaseObjectDefinitionRaw>
    {
      private SqlColumnBinder nameColumn = new SqlColumnBinder("name");
      private SqlColumnBinder schemaColumn = new SqlColumnBinder("schemaName");
      private SqlColumnBinder textColumn = new SqlColumnBinder("text");
      private SqlColumnBinder sectionIdColumn = new SqlColumnBinder("sectionId");

      protected override DatabaseObjectDefinitionComponent.DatabaseObjectDefinitionRaw Bind() => new DatabaseObjectDefinitionComponent.DatabaseObjectDefinitionRaw()
      {
        Name = this.nameColumn.GetString((IDataReader) this.Reader, false),
        Schema = this.schemaColumn.GetString((IDataReader) this.Reader, false),
        Text = this.textColumn.GetString((IDataReader) this.Reader, true),
        SectionId = (int) this.sectionIdColumn.GetInt16((IDataReader) this.Reader)
      };
    }
  }
}

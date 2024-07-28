// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.PropertyComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [SupportedSqlAccessIntent(SqlAccessIntent.ReadWrite | SqlAccessIntent.ReadOnly, null)]
  internal class PropertyComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[14]
    {
      (IComponentCreator) new ComponentCreator<PropertyComponent>(1, true),
      (IComponentCreator) new ComponentCreator<PropertyComponent2>(2),
      (IComponentCreator) new ComponentCreator<PropertyComponent3>(3),
      (IComponentCreator) new ComponentCreator<PropertyComponent4>(4),
      (IComponentCreator) new ComponentCreator<PropertyComponent5>(5),
      (IComponentCreator) new ComponentCreator<PropertyComponent6>(6),
      (IComponentCreator) new ComponentCreator<PropertyComponent7>(7),
      (IComponentCreator) new ComponentCreator<PropertyComponent8>(8),
      (IComponentCreator) new ComponentCreator<PropertyComponent9>(9),
      (IComponentCreator) new ComponentCreator<PropertyComponent10>(10),
      (IComponentCreator) new ComponentCreator<PropertyComponent11>(11),
      (IComponentCreator) new ComponentCreator<PropertyComponent12>(12),
      (IComponentCreator) new ComponentCreator<PropertyComponent13>(13),
      (IComponentCreator) new ComponentCreator<PropertyComponent14>(14)
    }, "Property");
    private static Dictionary<int, SqlExceptionFactory> s_propertyExceptionFactories;

    internal virtual void CreateArtifactKind(ArtifactKind artifactKind)
    {
      this.PrepareStoredProcedure("prc_CreateArtifactKind");
      this.BindGuid("@kind", artifactKind.Kind);
      this.BindBoolean("@internal", artifactKind.IsInternalKind);
      this.BindString("@description", artifactKind.Description, 2000, true, SqlDbType.NVarChar);
      this.BindString("@databaseCategory", this.GetDataspaceCategory(artifactKind.DataspaceCategory), 256, false, SqlDbType.NVarChar);
      this.BindBoolean("@monikerBased", artifactKind.IsMonikerBased);
      this.ExecuteNonQuery();
    }

    internal virtual List<ArtifactKind> GetPropertyKinds()
    {
      this.PrepareStoredProcedure("prc_GetPropertyKinds");
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_GetPropertyKinds", this.RequestContext);
      resultCollection.AddBinder<ArtifactKind>((ObjectBinder<ArtifactKind>) this.CreateArtifactKindColumns());
      return resultCollection.GetCurrent<ArtifactKind>().Items;
    }

    internal virtual void SetArtifactKindFlags(
      Guid artifactKindId,
      ArtifactKindFlags onFlags,
      ArtifactKindFlags offFlags)
    {
      throw new NotImplementedException();
    }

    internal void DeleteArtifactKind(Guid kind)
    {
      this.PrepareStoredProcedure("prc_DeleteArtifactKind");
      this.BindGuid("@kind", kind);
      this.ExecuteNonQuery();
    }

    internal virtual ResultCollection GetPropertyDefinitions(IEnumerable<string> propertyNameFilters)
    {
      this.PrepareStoredProcedure("prc_GetPropertyDefinitions");
      List<string> rows = new List<string>();
      foreach (string propertyNameFilter in propertyNameFilters)
        rows.Add(ArtifactSpec.TranslateSqlWildcards(propertyNameFilter, out bool _, out bool _));
      this.BindStringTable("@propertyNameFilterList", (IEnumerable<string>) rows);
      ResultCollection propertyDefinitions = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_GetPropertyDefinitions", this.RequestContext);
      propertyDefinitions.AddBinder<PropertyDefinition>((ObjectBinder<PropertyDefinition>) this.GetPropertyDefinitionColumns());
      return propertyDefinitions;
    }

    protected virtual PropertyComponent.PropertyDefinitionColumns GetPropertyDefinitionColumns() => new PropertyComponent.PropertyDefinitionColumns();

    internal virtual ResultCollection GetPropertyValue(
      IEnumerable<ArtifactSpec> artifactSpecs,
      IEnumerable<string> propertyNameFilters,
      ArtifactKind artifactKind,
      PropertiesOptions options)
    {
      ArtifactSpecDbPagingManager specDbPagingManager = (ArtifactSpecDbPagingManager) null;
      try
      {
        if (artifactSpecs != null)
        {
          specDbPagingManager = new ArtifactSpecDbPagingManager(this.RequestContext, options, this);
          specDbPagingManager.EnqueueAll(artifactSpecs);
        }
        bool parameterValue = false;
        List<string> rows = new List<string>();
        foreach (string propertyNameFilter in propertyNameFilters)
        {
          bool containsWildcards;
          rows.Add(ArtifactSpec.TranslateSqlWildcards(propertyNameFilter, out bool _, out containsWildcards));
          if (containsWildcards)
            parameterValue = true;
        }
        this.PrepareStoredProcedure("prc_GetPropertyValue");
        this.BindGuid("@kind", artifactKind.Kind);
        this.BindBoolean("@keepInputOrder", true);
        this.BindBoolean("@containsPropertyWildcards", parameterValue);
        this.BindStringTable("@propertyNameFilterList", (IEnumerable<string>) rows, true);
        this.BindBoolean("@queryAllVersions", (options & PropertiesOptions.AllVersions) == PropertiesOptions.AllVersions);
        if (specDbPagingManager != null)
        {
          this.BindBoolean("@containsArtifactWildcards", specDbPagingManager.ContainsWildcards);
          if (specDbPagingManager.IsPaged)
            this.BindInt("@lobParam", specDbPagingManager.ParameterId);
          else
            this.BindString("@artifactSpecList", specDbPagingManager.ServerItemWriter.ToString(), -1, false, SqlDbType.NVarChar);
        }
        ResultCollection propertyValue = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_GetPropertyValue", this.RequestContext);
        propertyValue.AddBinder<DbArtifactPropertyValue>(this.CreatePropertyValueBinder());
        return propertyValue;
      }
      finally
      {
        specDbPagingManager?.Dispose();
      }
    }

    internal virtual ResultCollection GetPropertyValue(
      ArtifactKind artifactKind,
      IEnumerable<string> propertyNameFilters,
      Guid? dataspaceIdentifier)
    {
      throw new NotImplementedException();
    }

    protected virtual void BindDataspaces(Guid[] dataspaceIdentifiers)
    {
    }

    protected virtual string GetDataspaceCategory(string dataspaceCategory) => dataspaceCategory.Equals("Framework", StringComparison.OrdinalIgnoreCase) ? "Default" : dataspaceCategory;

    protected virtual PropertyComponent.ArtifactKindColumns CreateArtifactKindColumns() => new PropertyComponent.ArtifactKindColumns();

    protected virtual ObjectBinder<DbArtifactPropertyValue> CreatePropertyValueBinder() => (ObjectBinder<DbArtifactPropertyValue>) new PropertyComponent.DbArtifactPropertyValueColumns(this);

    internal virtual void CopyPropertyValues(
      IEnumerable<KeyValuePair<ArtifactSpec, ArtifactSpec>> specs,
      ArtifactKind kind)
    {
      throw new NotImplementedException();
    }

    internal ResultCollection GetArtifactsForQuery(
      Guid kind,
      string queryString,
      IEnumerable<SqlParameter> parameters)
    {
      this.PrepareSqlBatch(queryString.Length);
      this.AddStatement(queryString);
      foreach (SqlParameter parameter in parameters)
        this.Command.Parameters.Add(parameter);
      ResultCollection artifactsForQuery = new ResultCollection((IDataReader) this.ExecuteReader(), "GetArtifactsForQuery - BATCH", this.RequestContext);
      artifactsForQuery.AddBinder<ArtifactSpec>((ObjectBinder<ArtifactSpec>) this.BindColumns(kind));
      return artifactsForQuery;
    }

    protected virtual PropertyComponent.ArtifactSpecColumns BindColumns(Guid kind) => new PropertyComponent.ArtifactSpecColumns(kind, this);

    internal void SetPropertyValue(
      ArtifactPropertyValueDbPagingManager dbPagingManager)
    {
      this.PrepareStoredProcedure("prc_SetPropertyValue");
      this.BindGuid("@kind", dbPagingManager.PagedArtifactKind.Kind);
      this.BindGuid("@author", this.Author);
      if (dbPagingManager.IsPaged)
        this.BindInt("@lobParam", dbPagingManager.ParameterId);
      else
        this.BindString("@propertyValueList", dbPagingManager.ServerItemWriter.ToString(), -1, false, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    internal virtual bool SetPropertyValue(
      IEnumerable<ArtifactPropertyValue> artifactPropertyValues,
      ArtifactKind kind,
      DateTime? date,
      Guid? author)
    {
      if (date.HasValue || author.HasValue)
        throw new NotSupportedException();
      using (ArtifactPropertyValueDbPagingManager dbPagingManager = new ArtifactPropertyValueDbPagingManager(this.RequestContext, this))
      {
        dbPagingManager.EnqueueAll(artifactPropertyValues);
        if (dbPagingManager.TotalCount == 0)
          return false;
        this.SetPropertyValue(dbPagingManager);
        return true;
      }
    }

    internal virtual void DeleteArtifacts(
      IEnumerable<ArtifactSpec> artifactSpecs,
      ArtifactKind kind,
      PropertiesOptions options)
    {
      using (ArtifactSpecDbPagingManager specDbPagingManager = new ArtifactSpecDbPagingManager(this.RequestContext, PropertiesOptions.None, this))
      {
        specDbPagingManager.EnqueueAll(artifactSpecs);
        specDbPagingManager.Flush();
        if (specDbPagingManager.TotalCount == 0)
          return;
        this.PrepareStoredProcedure("prc_DeleteArtifacts");
        this.BindGuid("@kind", specDbPagingManager.PagedArtifactKind.Kind);
        this.BindGuid("@author", this.Author);
        if (specDbPagingManager.IsPaged)
          this.BindInt("@lobParam", specDbPagingManager.ParameterId);
        else
          this.BindString("@artifactSpecList", specDbPagingManager.ServerItemWriter.ToString(), -1, false, SqlDbType.NVarChar);
        this.ExecuteNonQuery();
      }
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions
    {
      get
      {
        if (PropertyComponent.s_propertyExceptionFactories == null)
          PropertyComponent.s_propertyExceptionFactories = new Dictionary<int, SqlExceptionFactory>()
          {
            {
              800011,
              new SqlExceptionFactory(typeof (PropertyServiceException))
            },
            {
              800017,
              new SqlExceptionFactory(typeof (PropertyServiceException))
            },
            {
              800012,
              new SqlExceptionFactory(typeof (PropertyServiceException))
            },
            {
              800013,
              new SqlExceptionFactory(typeof (PropertyServiceException))
            },
            {
              800014,
              new SqlExceptionFactory(typeof (PropertyServiceException))
            },
            {
              800015,
              new SqlExceptionFactory(typeof (PropertyServiceException))
            },
            {
              800044,
              new SqlExceptionFactory(typeof (ArtifactKindAlreadyExistsException))
            }
          };
        return (IDictionary<int, SqlExceptionFactory>) PropertyComponent.s_propertyExceptionFactories;
      }
    }

    internal virtual int DeleteProperties(
      Guid artifactKind,
      IEnumerable<string> propertyNames,
      int batchSize = 2000,
      int? maxPropertiesToDelete = null)
    {
      throw new NotSupportedException();
    }

    internal class DbArtifactPropertyValueColumns : ObjectBinder<DbArtifactPropertyValue>
    {
      private static readonly int[] s_persistedTypes = new int[5]
      {
        18,
        9,
        16,
        1,
        14
      };
      protected readonly PropertyComponent m_component;
      private SqlColumnBinder sidColumn = new SqlColumnBinder("SeqId");
      private SqlColumnBinder idColumn = new SqlColumnBinder("ArtifactId");
      private SqlColumnBinder monikerColumn = new SqlColumnBinder("Moniker");
      private SqlColumnBinder internalKindColumn = new SqlColumnBinder("InternalKindId");
      private SqlColumnBinder versionColumn = new SqlColumnBinder("Version");
      private SqlColumnBinder requestedVersion = new SqlColumnBinder("RequestedVersion");
      private SqlColumnBinder propertyNameColumn = new SqlColumnBinder("PropertyName");
      private SqlColumnBinder typeIdColumn = new SqlColumnBinder("TypeId");
      private PropertyComponent.DbArtifactPropertyValueColumns.PropertyValueBinder[] m_typeBinders;

      public DbArtifactPropertyValueColumns(PropertyComponent component) => this.m_component = component;

      protected override DbArtifactPropertyValue Bind()
      {
        DbArtifactPropertyValue artifactPropertyValue = new DbArtifactPropertyValue()
        {
          SequenceId = this.sidColumn.GetInt32((IDataReader) this.Reader),
          ArtifactId = this.idColumn.GetBytes((IDataReader) this.Reader, false),
          Moniker = this.monikerColumn.GetString((IDataReader) this.Reader, true),
          InternalKindId = this.internalKindColumn.GetInt32((IDataReader) this.Reader),
          Version = this.versionColumn.GetInt32((IDataReader) this.Reader),
          PropertyName = this.propertyNameColumn.GetString((IDataReader) this.Reader, false),
          TypeMatch = PropertyTypeMatch.Unspecified
        };
        if (this.requestedVersion.IsInitialized())
        {
          artifactPropertyValue.RequestedVersion = this.requestedVersion.GetInt32((IDataReader) this.Reader, 0);
        }
        else
        {
          try
          {
            artifactPropertyValue.RequestedVersion = this.requestedVersion.GetInt32((IDataReader) this.Reader, 0);
          }
          catch (IndexOutOfRangeException ex)
          {
            TeamFoundationTrace.Info("GetProperties missing RequestedVersion");
            artifactPropertyValue.RequestedVersion = artifactPropertyValue.Version;
          }
        }
        int int32 = this.typeIdColumn.GetInt32((IDataReader) this.Reader);
        PropertyComponent.DbArtifactPropertyValueColumns.PropertyValueBinder propertyValueBinder = (PropertyComponent.DbArtifactPropertyValueColumns.PropertyValueBinder) null;
        if (int32 != 0 && (propertyValueBinder = this.TypeBinders[int32]) != null)
        {
          if (propertyValueBinder.HasData(this.Reader))
          {
            artifactPropertyValue.TypeMatch = PropertyTypeMatch.ExplicitType;
          }
          else
          {
            propertyValueBinder = (PropertyComponent.DbArtifactPropertyValueColumns.PropertyValueBinder) null;
            artifactPropertyValue.TypeMatch = PropertyTypeMatch.MismatchedType;
          }
        }
        if (propertyValueBinder == null)
        {
          for (int index = 0; index < PropertyComponent.DbArtifactPropertyValueColumns.s_persistedTypes.Length; ++index)
          {
            propertyValueBinder = this.TypeBinders[PropertyComponent.DbArtifactPropertyValueColumns.s_persistedTypes[index]];
            if (!propertyValueBinder.HasData(this.Reader))
              propertyValueBinder = (PropertyComponent.DbArtifactPropertyValueColumns.PropertyValueBinder) null;
            else
              break;
          }
        }
        if (propertyValueBinder != null)
        {
          artifactPropertyValue.Value = propertyValueBinder.GetObject(this.Reader);
          if (propertyValueBinder.Converter != null)
          {
            try
            {
              artifactPropertyValue.Value = propertyValueBinder.Converter.ConvertFrom(artifactPropertyValue.Value);
            }
            catch (Exception ex)
            {
              artifactPropertyValue.TypeMatch = PropertyTypeMatch.MismatchedType;
              TeamFoundationTrace.Warning("Unable to convert property type from to '" + propertyValueBinder.ValueType.ToString() + "'. Exception: " + ex.Message);
            }
          }
        }
        return artifactPropertyValue;
      }

      private PropertyComponent.DbArtifactPropertyValueColumns.PropertyValueBinder[] TypeBinders
      {
        get
        {
          if (this.m_typeBinders == null)
            this.m_typeBinders = new PropertyComponent.DbArtifactPropertyValueColumns.PropertyValueBinder[19]
            {
              null,
              new PropertyComponent.DbArtifactPropertyValueColumns.PropertyValueBinder("BinaryValue")
              {
                ValueType = typeof (object)
              },
              null,
              new PropertyComponent.DbArtifactPropertyValueColumns.PropertyValueBinder("StringValue")
              {
                ValueType = typeof (bool),
                Converter = TypeDescriptor.GetConverter(typeof (bool))
              },
              new PropertyComponent.DbArtifactPropertyValueColumns.PropertyValueBinder("StringValue")
              {
                ValueType = typeof (char),
                Converter = TypeDescriptor.GetConverter(typeof (char))
              },
              new PropertyComponent.DbArtifactPropertyValueColumns.PropertyValueBinder("StringValue")
              {
                ValueType = typeof (sbyte),
                Converter = TypeDescriptor.GetConverter(typeof (sbyte))
              },
              new PropertyComponent.DbArtifactPropertyValueColumns.PropertyValueBinder("StringValue")
              {
                ValueType = typeof (byte),
                Converter = TypeDescriptor.GetConverter(typeof (byte))
              },
              new PropertyComponent.DbArtifactPropertyValueColumns.PropertyValueBinder("StringValue")
              {
                ValueType = typeof (short),
                Converter = TypeDescriptor.GetConverter(typeof (short))
              },
              new PropertyComponent.DbArtifactPropertyValueColumns.PropertyValueBinder("StringValue")
              {
                ValueType = typeof (ushort),
                Converter = TypeDescriptor.GetConverter(typeof (ushort))
              },
              new PropertyComponent.DbArtifactPropertyValueColumns.PropertyValueBinder("IntValue")
              {
                ValueType = typeof (int)
              },
              new PropertyComponent.DbArtifactPropertyValueColumns.PropertyValueBinder("StringValue")
              {
                ValueType = typeof (uint),
                Converter = TypeDescriptor.GetConverter(typeof (uint))
              },
              new PropertyComponent.DbArtifactPropertyValueColumns.PropertyValueBinder("StringValue")
              {
                ValueType = typeof (long),
                Converter = TypeDescriptor.GetConverter(typeof (long))
              },
              new PropertyComponent.DbArtifactPropertyValueColumns.PropertyValueBinder("StringValue")
              {
                ValueType = typeof (ulong),
                Converter = TypeDescriptor.GetConverter(typeof (ulong))
              },
              new PropertyComponent.DbArtifactPropertyValueColumns.PropertyValueBinder("StringValue")
              {
                ValueType = typeof (float),
                Converter = TypeDescriptor.GetConverter(typeof (float))
              },
              new PropertyComponent.DbArtifactPropertyValueColumns.PropertyValueBinder("DoubleValue")
              {
                ValueType = typeof (double)
              },
              new PropertyComponent.DbArtifactPropertyValueColumns.PropertyValueBinder("StringValue")
              {
                ValueType = typeof (Decimal),
                Converter = TypeDescriptor.GetConverter(typeof (Decimal))
              },
              new PropertyComponent.DbArtifactPropertyValueColumns.PropertyValueBinder("DateTimeValue")
              {
                ValueType = typeof (DateTime)
              },
              null,
              new PropertyComponent.DbArtifactPropertyValueColumns.PropertyValueBinder("StringValue")
              {
                ValueType = typeof (string)
              }
            };
          return this.m_typeBinders;
        }
      }

      private class PropertyValueBinder
      {
        private SqlColumnBinder m_columnBinder;

        public PropertyValueBinder(string columnName) => this.m_columnBinder = new SqlColumnBinder(columnName);

        public Type ValueType { get; set; }

        public TypeConverter Converter { get; set; }

        public bool HasData(SqlDataReader reader) => !this.m_columnBinder.IsNull((IDataReader) reader);

        public object GetObject(SqlDataReader reader)
        {
          if (this.ValueType == typeof (string))
            return (object) this.m_columnBinder.GetString((IDataReader) reader, false);
          if (this.ValueType == typeof (int))
            return (object) this.m_columnBinder.GetInt32((IDataReader) reader);
          if (this.ValueType == typeof (DateTime))
            return (object) DateTime.SpecifyKind(this.m_columnBinder.GetDateTime((IDataReader) reader), DateTimeKind.Utc);
          return this.ValueType == typeof (double) ? (object) reader.GetDouble(this.m_columnBinder.GetOrdinal((IDataReader) reader)) : this.m_columnBinder.GetObject((IDataReader) reader);
        }
      }
    }

    internal class DbArtifactPropertyValueColumns2 : PropertyComponent.DbArtifactPropertyValueColumns
    {
      private SqlColumnBinder changedDateColumn = new SqlColumnBinder("ChangedDate");
      private SqlColumnBinder changedByColumn = new SqlColumnBinder("ChangedBy");

      public DbArtifactPropertyValueColumns2(PropertyComponent component)
        : base(component)
      {
      }

      protected override DbArtifactPropertyValue Bind()
      {
        DbArtifactPropertyValue artifactPropertyValue = base.Bind();
        if (!this.changedDateColumn.IsNull((IDataReader) this.Reader))
          artifactPropertyValue.ChangedDate = new DateTime?(this.changedDateColumn.GetDateTime((IDataReader) this.Reader));
        if (!this.changedByColumn.IsNull((IDataReader) this.Reader))
          artifactPropertyValue.ChangedBy = new Guid?(this.changedByColumn.GetGuid((IDataReader) this.Reader));
        return artifactPropertyValue;
      }
    }

    internal class DbArtifactPropertyValueColumns3 : 
      PropertyComponent.DbArtifactPropertyValueColumns2
    {
      private SqlColumnBinder dataspaceId = new SqlColumnBinder("DataspaceId");

      public DbArtifactPropertyValueColumns3(PropertyComponent component)
        : base(component)
      {
      }

      protected override DbArtifactPropertyValue Bind()
      {
        DbArtifactPropertyValue artifactPropertyValue = base.Bind();
        artifactPropertyValue.DataspaceIdentifier = this.m_component.GetDataspaceIdentifier(this.dataspaceId.GetInt32((IDataReader) this.Reader));
        return artifactPropertyValue;
      }
    }

    internal class ArtifactKindColumns : ObjectBinder<ArtifactKind>
    {
      protected SqlColumnBinder kindIdColumn = new SqlColumnBinder("KindId");
      protected SqlColumnBinder databaseCategoryColumn = new SqlColumnBinder("DatabaseCategory");
      protected SqlColumnBinder internalKindIdColumn = new SqlColumnBinder("InternalKindId");
      protected SqlColumnBinder internalColumn = new SqlColumnBinder("Internal");
      protected SqlColumnBinder descriptionColumn = new SqlColumnBinder("Description");
      protected SqlColumnBinder monikerBasedColumn = new SqlColumnBinder("MonikerBased");

      protected override ArtifactKind Bind() => new ArtifactKind()
      {
        Kind = this.kindIdColumn.GetGuid((IDataReader) this.Reader),
        Description = this.descriptionColumn.GetString((IDataReader) this.Reader, true),
        DataspaceCategory = this.GetDataspaceCategory(this.databaseCategoryColumn.GetString((IDataReader) this.Reader, false)),
        CompactKindId = this.internalKindIdColumn.GetInt32((IDataReader) this.Reader),
        IsInternalKind = this.internalColumn.GetBoolean((IDataReader) this.Reader),
        IsMonikerBased = this.monikerBasedColumn.GetBoolean((IDataReader) this.Reader)
      };

      protected virtual string GetDataspaceCategory(string dataspaceCategory) => dataspaceCategory.Equals("Framework", StringComparison.OrdinalIgnoreCase) ? "Default" : dataspaceCategory;
    }

    internal class ArtifactKindColumns2 : PropertyComponent.ArtifactKindColumns
    {
      protected SqlColumnBinder flagsColumn = new SqlColumnBinder("Flags");

      protected override ArtifactKind Bind() => new ArtifactKind()
      {
        Kind = this.kindIdColumn.GetGuid((IDataReader) this.Reader),
        Description = this.descriptionColumn.GetString((IDataReader) this.Reader, true),
        DataspaceCategory = this.GetDataspaceCategory(this.databaseCategoryColumn.GetString((IDataReader) this.Reader, false)),
        CompactKindId = this.internalKindIdColumn.GetInt32((IDataReader) this.Reader),
        IsInternalKind = this.internalColumn.GetBoolean((IDataReader) this.Reader),
        IsMonikerBased = this.monikerBasedColumn.GetBoolean((IDataReader) this.Reader),
        Flags = (ArtifactKindFlags) this.flagsColumn.GetInt32((IDataReader) this.Reader)
      };
    }

    internal class ArtifactKindColumns3 : PropertyComponent.ArtifactKindColumns2
    {
      private SqlColumnBinder dataspaceCategoryColumn = new SqlColumnBinder("DataspaceCategory");

      protected override ArtifactKind Bind() => new ArtifactKind()
      {
        Kind = this.kindIdColumn.GetGuid((IDataReader) this.Reader),
        Description = this.descriptionColumn.GetString((IDataReader) this.Reader, true),
        DataspaceCategory = this.GetDataspaceCategory(this.dataspaceCategoryColumn.GetString((IDataReader) this.Reader, false)),
        CompactKindId = this.internalKindIdColumn.GetInt32((IDataReader) this.Reader),
        IsInternalKind = this.internalColumn.GetBoolean((IDataReader) this.Reader),
        IsMonikerBased = this.monikerBasedColumn.GetBoolean((IDataReader) this.Reader),
        Flags = (ArtifactKindFlags) this.flagsColumn.GetInt32((IDataReader) this.Reader)
      };
    }

    internal class ArtifactSpecColumns : ObjectBinder<ArtifactSpec>
    {
      private SqlColumnBinder artifactId = new SqlColumnBinder("ArtifactId");
      private Guid m_kind;
      protected PropertyComponent m_component;

      internal ArtifactSpecColumns(Guid kind, PropertyComponent component)
      {
        this.m_kind = kind;
        this.m_component = component;
      }

      protected override ArtifactSpec Bind() => new ArtifactSpec(this.m_kind, this.artifactId.GetBytes((IDataReader) this.Reader, false), 0);
    }

    internal class ArtifactSpecColumns2 : PropertyComponent.ArtifactSpecColumns
    {
      private SqlColumnBinder dataspaceId = new SqlColumnBinder("DataspaceId");

      internal ArtifactSpecColumns2(Guid kind, PropertyComponent component)
        : base(kind, component)
      {
      }

      protected override ArtifactSpec Bind()
      {
        ArtifactSpec artifactSpec = base.Bind();
        artifactSpec.DataspaceIdentifier = this.m_component.GetDataspaceIdentifier(this.dataspaceId.GetInt32((IDataReader) this.Reader));
        return artifactSpec;
      }
    }

    internal class PropertyDefinitionColumns : ObjectBinder<PropertyDefinition>
    {
      private SqlColumnBinder propertyIdColumn = new SqlColumnBinder("PropertyId");
      private SqlColumnBinder nameColumn = new SqlColumnBinder("Name");

      protected override PropertyDefinition Bind() => new PropertyDefinition()
      {
        PropertyId = this.propertyIdColumn.GetInt32((IDataReader) this.Reader),
        Name = this.nameColumn.GetString((IDataReader) this.Reader, false)
      };
    }

    internal class PropertyDefinitionColumns2 : PropertyComponent.PropertyDefinitionColumns
    {
      private SqlColumnBinder dataspaceIdColumn = new SqlColumnBinder("DataspaceId");
      protected PropertyComponent m_component;

      public PropertyDefinitionColumns2(PropertyComponent component) => this.m_component = component;

      protected override PropertyDefinition Bind()
      {
        PropertyDefinition propertyDefinition = base.Bind();
        propertyDefinition.DataspaceIdentifier = this.m_component.GetDataspaceIdentifier(this.dataspaceIdColumn.GetInt32((IDataReader) this.Reader));
        return propertyDefinition;
      }
    }
  }
}

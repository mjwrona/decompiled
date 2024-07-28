// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ConfigurationResourceComponent
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class ConfigurationResourceComponent : VersionControlSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[3]
    {
      (IComponentCreator) new ComponentCreator<ConfigurationResourceComponent>(1, true),
      (IComponentCreator) new ComponentCreator<ConfigurationResourceComponent2>(2),
      (IComponentCreator) new ComponentCreator<ConfigurationResourceComponent3>(3)
    }, "VCConfigurationResource");
    private static readonly SqlMetaData[] typ_FileType = new SqlMetaData[3]
    {
      new SqlMetaData("FileType", SqlDbType.NVarChar, 64L),
      new SqlMetaData("AllowMultipleCheckout", SqlDbType.Bit),
      new SqlMetaData("FileTypeId", SqlDbType.Int)
    };
    protected static readonly SqlMetaData[] typ_Extension = new SqlMetaData[2]
    {
      new SqlMetaData("FileType", SqlDbType.NVarChar, 64L),
      new SqlMetaData("Extension", SqlDbType.NVarChar, 260L)
    };

    protected virtual SqlParameter BindFileTypeTable(
      string parameterName,
      IEnumerable<FileType> rows)
    {
      rows = rows ?? Enumerable.Empty<FileType>();
      System.Func<FileType, SqlDataRecord> selector = (System.Func<FileType, SqlDataRecord>) (type =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ConfigurationResourceComponent.typ_FileType);
        sqlDataRecord.SetString(0, type.Name);
        sqlDataRecord.SetBoolean(1, type.AllowMultipleCheckout);
        sqlDataRecord.SetInt32(2, type.Id);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_FileType", rows.Select<FileType, SqlDataRecord>(selector));
    }

    protected virtual SqlParameter BindFileTypeExtensionTable(
      string parameterName,
      IEnumerable<FileTypeExtension> rows)
    {
      rows = rows ?? Enumerable.Empty<FileTypeExtension>();
      System.Func<FileTypeExtension, SqlDataRecord> selector = (System.Func<FileTypeExtension, SqlDataRecord>) (extension =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ConfigurationResourceComponent.typ_Extension);
        sqlDataRecord.SetString(0, extension.Name);
        sqlDataRecord.SetString(1, extension.Extension);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_Extension", rows.Select<FileTypeExtension, SqlDataRecord>(selector));
    }

    public List<FileType> QueryFileTypes()
    {
      try
      {
        this.PrepareStoredProcedure("prc_QueryFileTypes");
        SqlDataReader reader = this.ExecuteReader();
        int index = 0;
        List<FileType> fileTypeList = new List<FileType>();
        ConfigurationResourceComponent.QueryFileTypeColumns queryFileTypeColumns = new ConfigurationResourceComponent.QueryFileTypeColumns();
        while (reader.Read())
          fileTypeList.Add(queryFileTypeColumns.bind(reader));
        reader.NextResult();
        while (reader.Read())
        {
          FileTypeExtension fileTypeExtension = new ConfigurationResourceComponent.QueryExtensionColumns().bind(reader);
          FileType fileType = fileTypeList[index];
          while (fileType.Name != fileTypeExtension.Name)
            fileType = fileTypeList[++index];
          fileTypeList[index].Extensions.Add(fileTypeExtension.Extension);
        }
        return fileTypeList;
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
    }

    public void SetFileTypes(FileType[] fileTypes)
    {
      this.PrepareStoredProcedure("prc_SetFileTypes");
      this.BindFileTypeTable("@fileTypeList", (IEnumerable<FileType>) fileTypes);
      List<FileTypeExtension> rows = new List<FileTypeExtension>(fileTypes.Length);
      foreach (FileType fileType in fileTypes)
      {
        foreach (string extension in fileType.Extensions)
          rows.Add(new FileTypeExtension()
          {
            Extension = extension,
            Name = fileType.Name
          });
      }
      this.BindFileTypeExtensionTable("@extensionList", (IEnumerable<FileTypeExtension>) rows);
      this.BindGuid("@author", this.Author);
      this.ExecuteNonQuery();
    }

    internal class QueryFileTypeColumns
    {
      internal SqlColumnBinder allowMultipleCheckout = new SqlColumnBinder("AllowMultipleCheckout");
      internal SqlColumnBinder fileTypeName = new SqlColumnBinder("FileType");
      internal SqlColumnBinder fileTypeId = new SqlColumnBinder("FileTypeId");

      internal FileType bind(SqlDataReader reader) => new FileType()
      {
        AllowMultipleCheckout = this.allowMultipleCheckout.GetBoolean((IDataReader) reader),
        Name = this.fileTypeName.GetString((IDataReader) reader, false),
        Id = this.fileTypeId.GetInt32((IDataReader) reader)
      };
    }

    internal class QueryExtensionColumns
    {
      internal SqlColumnBinder extension = new SqlColumnBinder("Extension");
      internal SqlColumnBinder fileTypeName = new SqlColumnBinder("FileType");

      internal FileTypeExtension bind(SqlDataReader reader) => new FileTypeExtension()
      {
        Extension = this.extension.GetString((IDataReader) reader, false),
        Name = this.fileTypeName.GetString((IDataReader) reader, false)
      };
    }
  }
}

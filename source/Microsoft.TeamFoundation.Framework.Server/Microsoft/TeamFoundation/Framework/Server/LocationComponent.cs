// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.LocationComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class LocationComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[12]
    {
      (IComponentCreator) new ComponentCreator<LocationComponent>(1),
      (IComponentCreator) new ComponentCreator<LocationComponent2>(2),
      (IComponentCreator) new ComponentCreator<LocationComponent3>(3),
      (IComponentCreator) new ComponentCreator<LocationComponent4>(4),
      (IComponentCreator) new ComponentCreator<LocationComponent5>(5),
      (IComponentCreator) new ComponentCreator<LocationComponent6>(6),
      (IComponentCreator) new ComponentCreator<LocationComponent7>(7),
      (IComponentCreator) new ComponentCreator<LocationComponent8>(8),
      (IComponentCreator) new ComponentCreator<LocationComponent9>(9),
      (IComponentCreator) new ComponentCreator<LocationComponent10>(10),
      (IComponentCreator) new ComponentCreator<LocationComponent11>(11),
      (IComponentCreator) new ComponentCreator<LocationComponent12>(12)
    }, "Location");
    private static readonly SqlMetaData[] typ_LocationMappingTable = new SqlMetaData[4]
    {
      new SqlMetaData("ServiceType", SqlDbType.NVarChar, 256L),
      new SqlMetaData("ServiceIdentifier", SqlDbType.UniqueIdentifier),
      new SqlMetaData("AccessMappingMoniker", SqlDbType.NVarChar, 128L),
      new SqlMetaData("Location", SqlDbType.NVarChar, 1024L)
    };
    private static readonly SqlMetaData[] typ_ServiceDefinitionTable = new SqlMetaData[8]
    {
      new SqlMetaData("ServiceType", SqlDbType.NVarChar, 256L),
      new SqlMetaData("Identifier", SqlDbType.UniqueIdentifier),
      new SqlMetaData("DisplayName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("RelativeToSetting", SqlDbType.Int),
      new SqlMetaData("RelativePath", SqlDbType.NVarChar, 256L),
      new SqlMetaData("IsSingleton", SqlDbType.Bit),
      new SqlMetaData("Description", SqlDbType.NVarChar, -1L),
      new SqlMetaData("ToolType", SqlDbType.NVarChar, 256L)
    };
    private static Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();

    static LocationComponent()
    {
      LocationComponent.s_sqlExceptionFactories.Add(800002, new SqlExceptionFactory(typeof (DuplicateLocationMappingException)));
      LocationComponent.s_sqlExceptionFactories.Add(800003, new SqlExceptionFactory(typeof (InvalidAccessPointException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new InvalidAccessPointException(TFCommonResources.InvalidAccessMappingLocationServiceUrl()))));
      LocationComponent.s_sqlExceptionFactories.Add(800004, new SqlExceptionFactory(typeof (IllegalDeleteSelfReferenceServiceDefinitionException)));
      LocationComponent.s_sqlExceptionFactories.Add(800006, new SqlExceptionFactory(typeof (InvalidServiceDefinitionException)));
      LocationComponent.s_sqlExceptionFactories.Add(800008, new SqlExceptionFactory(typeof (LocationMappingDoesNotExistException)));
      LocationComponent.s_sqlExceptionFactories.Add(800033, new SqlExceptionFactory(typeof (RemoveAccessMappingException)));
      LocationComponent.s_sqlExceptionFactories.Add(800007, new SqlExceptionFactory(typeof (InvalidServiceDefinitionException)));
      LocationComponent.s_sqlExceptionFactories.Add(800041, new SqlExceptionFactory(typeof (AccessMappingNotRegisteredException)));
      LocationComponent.s_sqlExceptionFactories.Add(800045, new SqlExceptionFactory(typeof (InvalidAccessPointException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new InvalidAccessPointException(FrameworkResources.AccessPointConflicts((object) sqEr.ExtractString("accessPoint"), (object) sqEr.ExtractString("existingAccessPoint"), (object) sqEr.ExtractString("existingDisplayName"))))));
      LocationComponent.s_sqlExceptionFactories.Add(800097, new SqlExceptionFactory(typeof (CannotChangeParentDefinitionException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new CannotChangeParentDefinitionException(LocationResources.CannotChangeParentDefinition((object) sqEr.ExtractString("service type"), (object) sqEr.ExtractString("identifier"), (object) sqEr.ExtractString("parentservicetype"), (object) sqEr.ExtractString("parentidentifier"))))));
    }

    protected SqlParameter BindLocationMappingTable(
      string parameterName,
      IEnumerable<ServiceDefinition> rows)
    {
      rows = rows ?? Enumerable.Empty<ServiceDefinition>();
      return this.BindTable(parameterName, "typ_LocationMappingTable", LocationComponent.BindLocationMappingRow(rows));
    }

    internal static IEnumerable<SqlDataRecord> BindLocationMappingRow(
      IEnumerable<ServiceDefinition> rows)
    {
      foreach (ServiceDefinition definition in rows)
      {
        if (definition.LocationMappings != null)
        {
          foreach (LocationMapping locationMapping in definition.LocationMappings)
          {
            SqlDataRecord record = new SqlDataRecord(LocationComponent.typ_LocationMappingTable);
            record.SetString(0, definition.ServiceType);
            record.SetGuid(1, definition.Identifier);
            record.SetNullableStringAsEmpty(2, locationMapping.AccessMappingMoniker);
            record.SetNullableStringAsEmpty(3, locationMapping.Location);
            yield return record;
          }
        }
      }
    }

    protected virtual SqlParameter BindServiceDefinitionTable(
      string parameterName,
      IEnumerable<ServiceDefinition> rows,
      bool coreFieldsOnly = false)
    {
      rows = rows ?? Enumerable.Empty<ServiceDefinition>();
      System.Func<ServiceDefinition, SqlDataRecord> selector = (System.Func<ServiceDefinition, SqlDataRecord>) (serviceDefinition =>
      {
        SqlDataRecord record = new SqlDataRecord(LocationComponent.typ_ServiceDefinitionTable);
        record.SetString(0, serviceDefinition.ServiceType);
        record.SetGuid(1, serviceDefinition.Identifier);
        if (coreFieldsOnly)
        {
          record.SetDBNull(2);
          record.SetDBNull(3);
          record.SetDBNull(4);
          record.SetDBNull(5);
          record.SetDBNull(6);
          record.SetDBNull(7);
        }
        else
        {
          record.SetNullableString(2, serviceDefinition.DisplayName);
          record.SetInt32(3, (int) serviceDefinition.RelativeToSetting);
          record.SetNullableString(4, serviceDefinition.RelativePath);
          record.SetBoolean(5, false);
          record.SetNullableString(6, serviceDefinition.Description);
          record.SetNullableString(7, serviceDefinition.ToolId);
        }
        return record;
      });
      return this.BindTable(parameterName, "typ_ServiceDefinitionTable", rows.Select<ServiceDefinition, SqlDataRecord>(selector));
    }

    internal virtual Uri GetPublicAccessMapping()
    {
      string sqlStatement = "SELECT AccessPoint FROM tbl_AccessMapping WHERE Moniker = 'PublicAccessMapping' AND PartitionId = 1";
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      string uriString = (string) this.ExecuteScalar();
      return uriString != null ? new Uri(uriString) : new Uri("http://localhost");
    }

    public virtual void ConfigureAccessMapping(
      AccessMapping accessMapping,
      bool makeDefault,
      bool allowOverlapping)
    {
      this.PrepareStoredProcedure("prc_ConfigureAccessMapping");
      this.BindString("@moniker", accessMapping.Moniker, 128, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@displayName", accessMapping.DisplayName, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@accessPoint", accessMapping.AccessPoint, -1, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindBoolean("@makeDefault", makeDefault);
      this.BindGuid("@writerIdentifier", this.Author);
      this.ExecuteNonQuery();
    }

    public virtual void RemoveAccessMappings(IEnumerable<AccessMapping> accessMappings)
    {
      this.PrepareStoredProcedure("prc_RemoveAccessMappings");
      using (StringWriter w = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture))
      {
        XmlTextWriter xmlTextWriter = new XmlTextWriter((TextWriter) w);
        xmlTextWriter.WriteStartDocument();
        xmlTextWriter.WriteStartElement(nameof (accessMappings));
        foreach (AccessMapping accessMapping in accessMappings)
        {
          xmlTextWriter.WriteStartElement("a");
          xmlTextWriter.WriteAttributeString("moniker", accessMapping.Moniker);
          xmlTextWriter.WriteEndElement();
        }
        xmlTextWriter.WriteEndElement();
        xmlTextWriter.WriteEndDocument();
        xmlTextWriter.Flush();
        this.BindXml("@accessMappingXML", w.ToString());
      }
      this.BindBoolean("@inheritsMappings", this.RequestContext.ServiceHost.ParentServiceHost != null);
      this.BindGuid("@writerIdentifier", this.Author);
      this.ExecuteNonQuery();
    }

    public virtual bool SaveServiceDefinitions(IEnumerable<ServiceDefinition> serviceDefinitions)
    {
      using (StringWriter w1 = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture))
      {
        using (StringWriter w2 = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture))
        {
          XmlTextWriter xmlTextWriter = new XmlTextWriter((TextWriter) w1);
          xmlTextWriter.WriteStartDocument();
          xmlTextWriter.WriteStartElement("definition");
          XmlTextWriter xmlMappingWriter = new XmlTextWriter((TextWriter) w2);
          xmlMappingWriter.WriteStartDocument();
          xmlMappingWriter.WriteStartElement("locationMapping");
          foreach (ServiceDefinition serviceDefinition in serviceDefinitions)
          {
            xmlTextWriter.WriteStartElement("d");
            xmlTextWriter.WriteAttributeString("serviceType", serviceDefinition.ServiceType);
            xmlTextWriter.WriteAttributeString("identifier", serviceDefinition.Identifier.ToString());
            xmlTextWriter.WriteAttributeString("displayName", serviceDefinition.DisplayName);
            xmlTextWriter.WriteAttributeString("relativeToSetting", ((int) serviceDefinition.RelativeToSetting).ToString((IFormatProvider) CultureInfo.InvariantCulture));
            xmlTextWriter.WriteAttributeString("relativePath", serviceDefinition.RelativePath);
            xmlTextWriter.WriteAttributeString("description", serviceDefinition.Description);
            xmlTextWriter.WriteAttributeString("toolType", serviceDefinition.ToolId);
            xmlTextWriter.WriteEndElement();
            if (serviceDefinition.LocationMappings != null && serviceDefinition.RelativeToSetting == RelativeToSetting.FullyQualified)
            {
              foreach (LocationMapping locationMapping in serviceDefinition.LocationMappings)
                LocationComponent.WriteLocationMappingXMLNode(xmlMappingWriter, serviceDefinition.ServiceType, serviceDefinition.Identifier, locationMapping);
            }
          }
          xmlMappingWriter.WriteEndElement();
          xmlMappingWriter.WriteEndDocument();
          xmlMappingWriter.Flush();
          xmlTextWriter.WriteEndElement();
          xmlTextWriter.WriteEndDocument();
          xmlTextWriter.Flush();
          this.PrepareStoredProcedure("prc_SaveServiceDefinitions");
          this.BindXml("@definitionXML", w1.ToString());
          this.BindXml("@locationMappingXML", w2.ToString());
          this.BindGuid("@writerIdentifier", this.Author);
        }
      }
      this.ExecuteNonQuery();
      return true;
    }

    protected virtual void RemoveServiceDefinitions(IEnumerable<ServiceDefinition> definitions)
    {
      using (StringWriter w = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture))
      {
        XmlTextWriter xmlTextWriter = new XmlTextWriter((TextWriter) w);
        xmlTextWriter.WriteStartDocument();
        xmlTextWriter.WriteStartElement("definition");
        foreach (ServiceDefinition definition in definitions)
        {
          xmlTextWriter.WriteStartElement("d");
          xmlTextWriter.WriteAttributeString("serviceType", definition.ServiceType);
          xmlTextWriter.WriteAttributeString("identifier", definition.Identifier.ToString());
          xmlTextWriter.WriteEndElement();
        }
        xmlTextWriter.WriteEndElement();
        xmlTextWriter.WriteEndDocument();
        xmlTextWriter.Flush();
        this.PrepareStoredProcedure("prc_RemoveServiceDefinitions");
        this.BindXml("@definitionXML", w.ToString());
        this.BindGuid("@writerIdentifier", this.Author);
      }
      this.ExecuteNonQuery();
    }

    public virtual void RemoveServiceDefinitions(
      IEnumerable<ServiceDefinition> definitions,
      bool fullMatch)
    {
      if (fullMatch)
        return;
      this.RemoveServiceDefinitions(definitions);
    }

    public virtual ResultCollection QueryServiceData()
    {
      this.PrepareStoredProcedure("prc_QueryServiceData");
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QueryServiceData", this.RequestContext);
      resultCollection.AddBinder<ServiceDefinition>((ObjectBinder<ServiceDefinition>) new ServiceDefinitionDataColumns());
      resultCollection.AddBinder<LocationMappingData>((ObjectBinder<LocationMappingData>) new LocationMappingDataColumns());
      resultCollection.AddBinder<AccessMapping>((ObjectBinder<AccessMapping>) new AccessMappingColumns());
      resultCollection.AddBinder<string>((ObjectBinder<string>) new DefaultAccessMappingColumn());
      resultCollection.AddBinder<long>(this.CreateLastChangeIdBinder());
      return resultCollection;
    }

    public virtual void LogLocationChange(string eventData) => throw new NotImplementedException();

    public virtual ServiceDefinition QueryServiceDefinition(string serviceType, Guid identifier) => throw new NotImplementedException();

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) LocationComponent.s_sqlExceptionFactories;

    protected virtual ObjectBinder<long> CreateLastChangeIdBinder() => (ObjectBinder<long>) new LastChangeIdColumn32();

    private static void WriteLocationMappingXMLNode(
      XmlTextWriter xmlMappingWriter,
      string serviceType,
      Guid serviceIdentifier,
      LocationMapping mapping)
    {
      xmlMappingWriter.WriteStartElement("l");
      xmlMappingWriter.WriteAttributeString("location", mapping.Location);
      xmlMappingWriter.WriteAttributeString("accessMappingMoniker", mapping.AccessMappingMoniker);
      xmlMappingWriter.WriteAttributeString(nameof (serviceType), serviceType);
      xmlMappingWriter.WriteAttributeString(nameof (serviceIdentifier), serviceIdentifier.ToString());
      xmlMappingWriter.WriteEndElement();
    }
  }
}

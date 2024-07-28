// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.TeamFoundationTemporaryDataService
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Microsoft.TeamFoundation.Server.Core
{
  public class TeamFoundationTemporaryDataService : 
    ITeamFoundationTemporaryDataService,
    IVssFrameworkService
  {
    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext)
    {
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext)
    {
    }

    [TraceFilter(10003000, 10003001)]
    public ServiceTemporaryDataDTO CreateTemporaryData(
      IVssRequestContext requestContext,
      string data)
    {
      return this.CreateTemporaryData(requestContext, data, Guid.NewGuid(), "Web", 0);
    }

    [TraceFilter(10003004, 10003005)]
    public ServiceTemporaryDataDTO CreateTemporaryData(
      IVssRequestContext requestContext,
      string data,
      Guid id,
      string origin,
      int expirationSeconds)
    {
      ArgumentUtility.CheckForNull<string>(data, nameof (data));
      ArgumentUtility.CheckForEmptyGuid(id, nameof (id));
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (!this.IsTemporaryDataIdUnique(requestContext, id))
        throw new DuplicateTemporaryDataIdException(Resources.DuplicateTemporaryDataIdMessage((object) id.ToString(), (object) nameof (CreateTemporaryData)));
      byte[] bytes = Encoding.Unicode.GetBytes(data);
      int fileId = this.SaveDataUsingFileService(requestContext, bytes);
      DateTime utcNow = DateTime.UtcNow;
      int expirationTimeInSeconds = TeamFoundationTemporaryDataService.GetDefaultExpirationTimeInSeconds(requestContext);
      DateTime dateTime = expirationSeconds <= 0 || expirationSeconds > expirationTimeInSeconds ? utcNow.AddSeconds((double) expirationTimeInSeconds) : utcNow.AddSeconds((double) expirationSeconds);
      if (string.IsNullOrEmpty(origin))
        origin = "Web";
      ArtifactPropertyValue artifactProperties = TeamFoundationTemporaryDataService.GetArtifactProperties(id, dateTime, fileId, origin);
      this.SavePropertyValuesForTemporaryData(requestContext, fileId, utcNow, (IEnumerable<ArtifactPropertyValue>) new ArtifactPropertyValue[1]
      {
        artifactProperties
      });
      this.PublishCIForTemporaryData(requestContext, "TemporaryDataApiPost", id, origin, dateTime, new int?(fileId), bytes.Length, utcNow);
      return new ServiceTemporaryDataDTO()
      {
        Id = id,
        Value = data,
        ExpirationDate = dateTime,
        Origin = origin
      };
    }

    [TraceFilter(10003006, 10003007)]
    public ServiceTemporaryDataDTO CreateTemporaryDataWithPropertyService(
      IVssRequestContext requestContext,
      string data,
      string origin,
      int expirationSeconds = 0)
    {
      ArgumentUtility.CheckForNull<string>(data, nameof (data));
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      DateTime utcNow = DateTime.UtcNow;
      Guid guid = Guid.NewGuid();
      int expirationTimeInSeconds = TeamFoundationTemporaryDataService.GetDefaultQueryExpirationTimeInSeconds(requestContext);
      DateTime dateTime = expirationSeconds <= 0 || expirationSeconds > expirationTimeInSeconds ? utcNow.AddSeconds((double) expirationTimeInSeconds) : utcNow.AddSeconds((double) expirationSeconds);
      ArtifactPropertyValue artifactProperties = TeamFoundationTemporaryDataService.GetArtifactProperties(guid, dateTime, data, origin);
      try
      {
        requestContext.GetService<ITeamFoundationPropertyService>().SetProperties(requestContext, (IEnumerable<ArtifactPropertyValue>) new ArtifactPropertyValue[1]
        {
          artifactProperties
        }, new DateTime?(utcNow), new Guid?(this.GetUserIdentity(requestContext)));
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException("Fail to create new value to Property Service", "CreateTemporaryData", ex);
        throw;
      }
      this.PublishCIForTemporaryData(requestContext, "TemporaryDataApiPostPropertyService", guid, origin, dateTime, new int?(), data.Length, utcNow);
      return new ServiceTemporaryDataDTO()
      {
        Id = guid,
        Value = data,
        ExpirationDate = dateTime,
        Origin = origin
      };
    }

    [TraceFilter(10003002, 10003003)]
    public ServiceTemporaryDataDTO GetTemporaryData(IVssRequestContext requestContext, Guid id)
    {
      TemporaryDataInternalReference forTemporaryData = this.GetPropertiesForTemporaryData(requestContext, id);
      return new ServiceTemporaryDataDTO()
      {
        Id = id,
        Value = forTemporaryData.FileContent,
        ExpirationDate = forTemporaryData.ExpirationDate,
        Origin = forTemporaryData.Origin
      };
    }

    internal virtual Guid GetUserIdentity(IVssRequestContext requestContext) => requestContext.GetUserId();

    private void SavePropertyValuesForTemporaryData(
      IVssRequestContext requestContext,
      int fileId,
      DateTime changedDate,
      IEnumerable<ArtifactPropertyValue> artifactProperties)
    {
      ITeamFoundationFileService service1 = requestContext.GetService<ITeamFoundationFileService>();
      ITeamFoundationPropertyService service2 = requestContext.GetService<ITeamFoundationPropertyService>();
      try
      {
        service2.SetProperties(requestContext, artifactProperties, new DateTime?(changedDate), new Guid?(this.GetUserIdentity(requestContext)));
      }
      catch (Exception ex)
      {
        service1.DeleteFile(requestContext, (long) fileId);
        TeamFoundationTrace.TraceException("Fail to create new value to Property Service", "CreateTemporaryData", ex);
        throw;
      }
    }

    private int SaveDataUsingFileService(IVssRequestContext requestContext, byte[] dataBytes) => requestContext.TraceBlock<int>(10003009, 10003010, "ProcessTemplate", nameof (TeamFoundationTemporaryDataService), nameof (SaveDataUsingFileService), (Func<int>) (() =>
    {
      int num = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/TemporaryData/TemporaryDataMaxBytes", 131072);
      if (dataBytes.Length > num)
        throw new TemporaryDataTooLargeException(Resources.TemporaryDataTooLargeException((object) dataBytes.Length, (object) num));
      return requestContext.GetService<ITeamFoundationFileService>().UploadFile(requestContext, dataBytes);
    }));

    private static ArtifactPropertyValue GetArtifactProperties(
      Guid id,
      DateTime expirationDate,
      int fileId,
      string origin)
    {
      return new ArtifactPropertyValue(new ArtifactSpec(TemporaryDataConstants.TemporaryDataArtifactKind, id.ToByteArray(), 1), (IEnumerable<PropertyValue>) new PropertyValue[3]
      {
        new PropertyValue("Microsoft.TeamFoundation.Server.Core.TemporaryData.FileServiceId", (object) fileId),
        new PropertyValue("Microsoft.TeamFoundation.Server.Core.TemporaryData.ExpirationDate", (object) expirationDate),
        new PropertyValue("Microsoft.TeamFoundation.Server.Core.TemporaryData.Origin", (object) origin)
      });
    }

    private static ArtifactPropertyValue GetArtifactProperties(
      Guid id,
      DateTime expirationDate,
      string data,
      string origin)
    {
      return new ArtifactPropertyValue(new ArtifactSpec(TemporaryDataConstants.TemporaryDataArtifactKind, id.ToByteArray(), 1), (IEnumerable<PropertyValue>) new PropertyValue[3]
      {
        new PropertyValue("Microsoft.TeamFoundation.Server.Core.TemporaryData.Content", (object) data),
        new PropertyValue("Microsoft.TeamFoundation.Server.Core.TemporaryData.ExpirationDate", (object) expirationDate),
        new PropertyValue("Microsoft.TeamFoundation.Server.Core.TemporaryData.Origin", (object) origin)
      });
    }

    private string GetFileContentAsString(IVssRequestContext requestContext, int fileId)
    {
      using (Stream stream = requestContext.GetService<ITeamFoundationFileService>().RetrieveFile(requestContext, (long) fileId, false, out byte[] _, out long _, out CompressionType _))
      {
        using (StreamReader streamReader = new StreamReader(stream, Encoding.Unicode))
          return streamReader.ReadToEnd();
      }
    }

    private TemporaryDataInternalReference GetPropertiesForTemporaryData(
      IVssRequestContext requestContext,
      Guid temporaryDataId)
    {
      ITeamFoundationPropertyService service = requestContext.GetService<ITeamFoundationPropertyService>();
      ArtifactSpec artifactSpec1 = new ArtifactSpec(TemporaryDataConstants.TemporaryDataArtifactKind, temporaryDataId.ToByteArray(), 1);
      Dictionary<string, PropertyValue> propertyNameToValueMapping = new Dictionary<string, PropertyValue>()
      {
        {
          "Microsoft.TeamFoundation.Server.Core.TemporaryData.FileServiceId",
          (PropertyValue) null
        },
        {
          "Microsoft.TeamFoundation.Server.Core.TemporaryData.ExpirationDate",
          (PropertyValue) null
        },
        {
          "Microsoft.TeamFoundation.Server.Core.TemporaryData.Origin",
          (PropertyValue) null
        },
        {
          "Microsoft.TeamFoundation.Server.Core.TemporaryData.Content",
          (PropertyValue) null
        }
      };
      IVssRequestContext requestContext1 = requestContext;
      ArtifactSpec artifactSpec2 = artifactSpec1;
      Dictionary<string, PropertyValue>.KeyCollection keys = propertyNameToValueMapping.Keys;
      using (TeamFoundationDataReader properties = service.GetProperties(requestContext1, artifactSpec2, (IEnumerable<string>) keys))
      {
        foreach (ArtifactPropertyValue artifactPropertyValue in properties)
        {
          foreach (PropertyValue propertyValue in artifactPropertyValue.PropertyValues)
          {
            if (!propertyNameToValueMapping.ContainsKey(propertyValue.PropertyName))
              throw new TemporaryDataUnknownPropertyException(Resources.UnknownTemporaryDataPropertyMessage((object) propertyValue.PropertyName));
            propertyNameToValueMapping[propertyValue.PropertyName] = propertyValue;
          }
        }
      }
      int? fileId;
      string dataContent;
      DateTime changedDate;
      this.GetDataContent(requestContext, propertyNameToValueMapping, out fileId, out dataContent, out changedDate);
      string origin = "Web";
      if (propertyNameToValueMapping["Microsoft.TeamFoundation.Server.Core.TemporaryData.Origin"] != null)
        origin = (string) propertyNameToValueMapping["Microsoft.TeamFoundation.Server.Core.TemporaryData.Origin"].Value;
      DateTime expiration = propertyNameToValueMapping["Microsoft.TeamFoundation.Server.Core.TemporaryData.ExpirationDate"] != null ? (DateTime) propertyNameToValueMapping["Microsoft.TeamFoundation.Server.Core.TemporaryData.ExpirationDate"].Value : changedDate.AddSeconds((double) TeamFoundationTemporaryDataService.GetDefaultExpirationTimeInSeconds(requestContext));
      this.PublishCIForTemporaryData(requestContext, "TemporaryDataApiGet", temporaryDataId, origin, expiration, fileId, dataContent.Length, changedDate);
      return new TemporaryDataInternalReference()
      {
        ExpirationDate = expiration,
        FileContent = dataContent,
        Origin = origin
      };
    }

    private void GetDataContent(
      IVssRequestContext requestContext,
      Dictionary<string, PropertyValue> propertyNameToValueMapping,
      out int? fileId,
      out string dataContent,
      out DateTime changedDate)
    {
      if (propertyNameToValueMapping["Microsoft.TeamFoundation.Server.Core.TemporaryData.Content"] != null)
      {
        fileId = new int?();
        changedDate = propertyNameToValueMapping["Microsoft.TeamFoundation.Server.Core.TemporaryData.Content"].ChangedDate.GetValueOrDefault();
        dataContent = propertyNameToValueMapping["Microsoft.TeamFoundation.Server.Core.TemporaryData.Content"].Value.ToString();
      }
      else
      {
        fileId = propertyNameToValueMapping["Microsoft.TeamFoundation.Server.Core.TemporaryData.FileServiceId"] != null ? (int?) propertyNameToValueMapping["Microsoft.TeamFoundation.Server.Core.TemporaryData.FileServiceId"].Value : throw new TemporaryDataNotFoundException(Resources.TemporaryDataNotFoundException());
        changedDate = propertyNameToValueMapping["Microsoft.TeamFoundation.Server.Core.TemporaryData.FileServiceId"].ChangedDate.GetValueOrDefault();
        dataContent = this.GetFileContentAsString(requestContext, fileId.Value);
      }
    }

    internal static int GetDefaultExpirationTimeInSeconds(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) TemporaryDataConstants.TemporaryDataCutoffIntervalRegistryPath, true, TemporaryDataConstants.TemporaryData_defaultCutoffIntervalInSeconds);

    internal static int GetDefaultQueryExpirationTimeInSeconds(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) TemporaryDataConstants.TemporaryDataCutoffIntervalRegistryPath, true, TemporaryDataConstants.TemporaryQuery_defaultCutoffIntervalInSeconds);

    private void PublishCIForTemporaryData(
      IVssRequestContext requestContext,
      string action,
      Guid temporaryDataId,
      string origin,
      DateTime expiration,
      int? fileId,
      int fileLength,
      DateTime createdDate)
    {
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("Action", action);
      properties.Add("TemporaryDataId", (object) temporaryDataId);
      properties.Add("CreatedDate", (object) createdDate);
      properties.Add("Origin", origin);
      properties.Add("ExpirationDate", (object) expiration);
      properties.Add("FileId", (object) fileId);
      properties.Add("FileLengthBytes", (double) fileLength);
      properties.Add("AccessedDate", (object) DateTime.UtcNow);
      try
      {
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "TemporaryData", "RestApiTelemetry", properties);
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException("Fail to record CustomerIntelligence", nameof (PublishCIForTemporaryData), ex);
      }
    }

    private bool IsTemporaryDataIdUnique(IVssRequestContext requestContext, Guid temporaryDataId)
    {
      ITeamFoundationPropertyService service = requestContext.GetService<ITeamFoundationPropertyService>();
      ArtifactSpec artifactSpec1 = new ArtifactSpec(TemporaryDataConstants.TemporaryDataArtifactKind, temporaryDataId.ToByteArray(), 1);
      IVssRequestContext requestContext1 = requestContext;
      ArtifactSpec artifactSpec2 = artifactSpec1;
      string[] propertyNameFilters = new string[1]
      {
        "Microsoft.TeamFoundation.Server.Core.TemporaryData.FileServiceId"
      };
      using (TeamFoundationDataReader properties = service.GetProperties(requestContext1, artifactSpec2, (IEnumerable<string>) propertyNameFilters))
      {
        foreach (ArtifactPropertyValue artifactPropertyValue in properties)
        {
          if (!artifactPropertyValue.PropertyValues.IsNullOrEmpty<PropertyValue>())
            return false;
        }
      }
      return true;
    }
  }
}

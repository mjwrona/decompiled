// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.LicensingMitigationService
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Account;
using Microsoft.VisualStudio.Services.Licensing.CosmosDataAccess;
using Microsoft.VisualStudio.Services.Licensing.DataAccess;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Licensing
{
  public class LicensingMitigationService : ILicensingMitigationService, IVssFrameworkService
  {
    private const string _area = "Licensing";
    private const string _layer = "LicensingMitigationService";
    private ILicensingRepository m_licensingRepository;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckProjectCollectionRequestContext();
      this.m_licensingRepository = LicensingRepositoryFactory<ApplicationLicensingRepository>.Create(systemRequestContext);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void DeleteUserLicense(IVssRequestContext requestContext, Guid userId)
    {
      SecurityUtil.CheckMitigationPermision(requestContext);
      UserLicenseCosmosSerializableDocument userDocument = this.m_licensingRepository.GetUserDocument(requestContext, userId);
      if (userDocument == null)
      {
        requestContext.TraceAlways(1035540, TraceLevel.Warning, "Licensing", nameof (LicensingMitigationService), string.Format("No {0} found for userId {1}. Nothing to delete.", (object) "UserLicenseCosmosSerializableDocument", (object) userId));
      }
      else
      {
        Dictionary<string, object> ciData = new Dictionary<string, object>()
        {
          {
            nameof (userId),
            (object) userDocument.Document.UserId
          },
          {
            "license",
            (object) userDocument.Document.License
          },
          {
            "previousLicense",
            (object) userDocument.Document.PreviousLicense
          },
          {
            "extensionLicenses",
            (object) userDocument.Document.ExtensionLicenses
          }
        };
        this.PublishTelemetry(requestContext, "DeleteUserDocument", (IDictionary<string, object>) ciData);
        this.m_licensingRepository.DeleteUserDocument(requestContext, userDocument);
      }
    }

    public void UpdateUserLicense(
      IVssRequestContext requestContext,
      Guid userId,
      LicensingSource? source,
      LicensingOrigin? origin,
      int? license,
      AssignmentSource? assignmentSource,
      AccountUserStatus? status,
      DateTimeOffset? assignmentDate,
      DateTimeOffset? dateCreated,
      DateTimeOffset? lastUpdated,
      DateTimeOffset? lastAccessed)
    {
      SecurityUtil.CheckMitigationPermision(requestContext);
      UserLicenseCosmosSerializableDocument userDocument = this.m_licensingRepository.GetUserDocument(requestContext, userId);
      if (userDocument == null)
        throw new ArgumentException(string.Format("No {0} found for userId {1}", (object) "UserLicenseCosmosSerializableDocument", (object) userId), nameof (userId));
      if (userDocument.Document == null || userDocument.Document.License == null)
        throw new Exception(string.Format("No license found on {0} for userId {1}", (object) "UserLicenseCosmosSerializableDocument", (object) userId));
      UserLicense license1 = userDocument.Document.License;
      UserLicense userLicense = this.BuildUpdatedUserLicense(license1, source, origin, license, assignmentSource, status, assignmentDate, dateCreated, lastUpdated, lastAccessed);
      userDocument.Document.License = userLicense;
      Dictionary<string, object> ciData = new Dictionary<string, object>()
      {
        {
          nameof (userId),
          (object) userDocument.Document.UserId
        },
        {
          "oldLicense",
          (object) license1
        },
        {
          "newLicense",
          (object) userLicense
        },
        {
          "previousLicense",
          (object) userDocument.Document.PreviousLicense
        },
        {
          "extensionLicenses",
          (object) userDocument.Document.ExtensionLicenses
        }
      };
      this.PublishTelemetry(requestContext, "UpdateUserDocument", (IDictionary<string, object>) ciData);
      this.m_licensingRepository.UpdateUserDocument(requestContext, userDocument);
    }

    private UserLicense BuildUpdatedUserLicense(
      UserLicense userLicense,
      LicensingSource? source,
      LicensingOrigin? origin,
      int? license,
      AssignmentSource? assignmentSource,
      AccountUserStatus? status,
      DateTimeOffset? assignmentDate,
      DateTimeOffset? dateCreated,
      DateTimeOffset? lastUpdated,
      DateTimeOffset? lastAccessed)
    {
      UserLicense userLicense1 = (UserLicense) userLicense.Clone();
      userLicense1.Source = (LicensingSource) ((int) source ?? (int) userLicense1.Source);
      userLicense1.Origin = (LicensingOrigin) ((int) origin ?? (int) userLicense1.Origin);
      userLicense1.License = license ?? userLicense1.License;
      userLicense1.AssignmentSource = (AssignmentSource) ((int) assignmentSource ?? (int) userLicense1.AssignmentSource);
      userLicense1.Status = (AccountUserStatus) ((int) status ?? (int) userLicense1.Status);
      UserLicense userLicense2 = userLicense1;
      DateTimeOffset? nullable = assignmentDate;
      DateTimeOffset dateTimeOffset1 = nullable ?? userLicense1.AssignmentDate;
      userLicense2.AssignmentDate = dateTimeOffset1;
      UserLicense userLicense3 = userLicense1;
      nullable = dateCreated;
      DateTimeOffset dateTimeOffset2 = nullable ?? userLicense1.DateCreated;
      userLicense3.DateCreated = dateTimeOffset2;
      UserLicense userLicense4 = userLicense1;
      nullable = lastUpdated;
      DateTimeOffset dateTimeOffset3 = nullable ?? userLicense1.LastUpdated;
      userLicense4.LastUpdated = dateTimeOffset3;
      UserLicense userLicense5 = userLicense1;
      nullable = lastAccessed;
      DateTimeOffset dateTimeOffset4 = nullable ?? userLicense1.LastAccessed;
      userLicense5.LastAccessed = dateTimeOffset4;
      return userLicense1;
    }

    private void PublishTelemetry(
      IVssRequestContext requestContext,
      string featureName,
      IDictionary<string, object> ciData)
    {
      CustomerIntelligenceData properties = new CustomerIntelligenceData(ciData);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, requestContext.ServiceHost.InstanceId, CustomerIntelligenceArea.LicensingMitigation, featureName, properties);
    }

    private class CIConstants
    {
      public const string UserId = "userId";
      public const string License = "license";
      public const string PreviousLicense = "previousLicense";
      public const string ExtensionLicenses = "extensionLicenses";
      public const string OldLicense = "oldLicense";
      public const string NewLicense = "newLicense";
      public const string DeleteUserDocumentFeatureName = "DeleteUserDocument";
      public const string UpdateUserDocumentFeatureName = "UpdateUserDocument";
    }
  }
}

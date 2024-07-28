// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters.ReleaseStatusConverter
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters
{
  public static class ReleaseStatusConverter
  {
    private static readonly IDictionary<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseStatus, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseStatus> ModelToWebApiStatusMap = (IDictionary<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseStatus, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseStatus>) new Dictionary<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseStatus, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseStatus>()
    {
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseStatus.Undefined,
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseStatus.Undefined
      },
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseStatus.Abandoned,
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseStatus.Abandoned
      },
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseStatus.Draft,
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseStatus.Draft
      },
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseStatus.Active,
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseStatus.Active
      }
    };
    private static readonly IDictionary<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseStatus, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseStatus> WebApiToModelStatusMap = (IDictionary<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseStatus, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseStatus>) new Dictionary<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseStatus, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseStatus>()
    {
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseStatus.Undefined,
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseStatus.Undefined
      },
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseStatus.Abandoned,
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseStatus.Abandoned
      },
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseStatus.Draft,
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseStatus.Draft
      },
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseStatus.Active,
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseStatus.Active
      }
    };
    private static readonly IDictionary<ReleaseStatusOld, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseStatus> OldToNewStatusValuesMap = (IDictionary<ReleaseStatusOld, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseStatus>) new Dictionary<ReleaseStatusOld, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseStatus>()
    {
      {
        ReleaseStatusOld.Undefined,
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseStatus.Undefined
      },
      {
        ReleaseStatusOld.Abandoned,
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseStatus.Abandoned
      },
      {
        ReleaseStatusOld.Draft,
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseStatus.Draft
      },
      {
        ReleaseStatusOld.Active,
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseStatus.Active
      }
    };

    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseStatus ToWebApi(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseStatus status)
    {
      return ReleaseStatusConverter.ModelToWebApiStatusMap[status];
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseStatus FromWebApi(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseStatus status)
    {
      return ReleaseStatusConverter.WebApiToModelStatusMap[status];
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseStatus ConvertToNewReleaseStatusWithUpdatedValue(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseStatus status)
    {
      ReleaseStatusOld key = (ReleaseStatusOld) status;
      return ReleaseStatusConverter.OldToNewStatusValuesMap[key];
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseStatus ConvertToNewReleaseState(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseStatus status)
    {
      return status.ToWebApi().FromWebApi();
    }
  }
}

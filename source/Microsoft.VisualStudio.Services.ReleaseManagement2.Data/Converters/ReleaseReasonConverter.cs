// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters.ReleaseReasonConverter
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters
{
  public static class ReleaseReasonConverter
  {
    private static readonly IDictionary<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseReason, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseReason> ModelToWebApiMap = (IDictionary<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseReason, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseReason>) new Dictionary<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseReason, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseReason>()
    {
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseReason.None,
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseReason.None
      },
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseReason.Manual,
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseReason.Manual
      },
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseReason.ContinuousIntegration,
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseReason.ContinuousIntegration
      },
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseReason.Schedule,
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseReason.Schedule
      },
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseReason.PullRequest,
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseReason.PullRequest
      }
    };
    private static readonly IDictionary<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseReason, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseReason> WebApiToModelMap = (IDictionary<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseReason, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseReason>) new Dictionary<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseReason, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseReason>()
    {
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseReason.None,
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseReason.None
      },
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseReason.Manual,
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseReason.Manual
      },
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseReason.ContinuousIntegration,
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseReason.ContinuousIntegration
      },
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseReason.Schedule,
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseReason.Schedule
      },
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseReason.PullRequest,
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseReason.PullRequest
      }
    };

    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseReason ToWebApi(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseReason reason)
    {
      return ReleaseReasonConverter.ModelToWebApiMap[reason];
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseReason FromWebApi(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseReason reason)
    {
      return ReleaseReasonConverter.WebApiToModelMap[reason];
    }
  }
}

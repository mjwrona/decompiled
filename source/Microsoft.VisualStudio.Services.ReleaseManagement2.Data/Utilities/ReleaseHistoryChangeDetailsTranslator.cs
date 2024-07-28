// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ReleaseHistoryChangeDetailsTranslator
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities
{
  public static class ReleaseHistoryChangeDetailsTranslator
  {
    private static readonly Dictionary<ReleaseHistoryMessageId, Func<string, ReleaseHistoryMessageId, ReleaseRevisionChangeDetails>> ChangeDetailTranslatorMap = new Dictionary<ReleaseHistoryMessageId, Func<string, ReleaseHistoryMessageId, ReleaseRevisionChangeDetails>>()
    {
      {
        ReleaseHistoryMessageId.ReleaseStatusChange,
        new Func<string, ReleaseHistoryMessageId, ReleaseRevisionChangeDetails>(ReleaseHistoryChangeDetailsTranslator.TranslateReleaseStatusChangeDetail)
      },
      {
        ReleaseHistoryMessageId.EnvironmentStatusChange,
        new Func<string, ReleaseHistoryMessageId, ReleaseRevisionChangeDetails>(ReleaseHistoryChangeDetailsTranslator.TranslateEnvironmentStatusChangeDetail)
      },
      {
        ReleaseHistoryMessageId.EnvironmentStatusChangeByQueuingPolicy,
        new Func<string, ReleaseHistoryMessageId, ReleaseRevisionChangeDetails>(ReleaseHistoryChangeDetailsTranslator.TranslateEnvironmentStatusChangeDetail)
      },
      {
        ReleaseHistoryMessageId.EnvironmentStatusChangeByScheduleDeletion,
        new Func<string, ReleaseHistoryMessageId, ReleaseRevisionChangeDetails>(ReleaseHistoryChangeDetailsTranslator.TranslateEnvironmentStatusChangeDetail)
      },
      {
        ReleaseHistoryMessageId.EnvironmentStatusChangeByReleaseDeletion,
        new Func<string, ReleaseHistoryMessageId, ReleaseRevisionChangeDetails>(ReleaseHistoryChangeDetailsTranslator.TranslateEnvironmentStatusChangeDetail)
      },
      {
        ReleaseHistoryMessageId.EnvironmentStatusChangeByAbandonRelease,
        new Func<string, ReleaseHistoryMessageId, ReleaseRevisionChangeDetails>(ReleaseHistoryChangeDetailsTranslator.TranslateEnvironmentStatusChangeDetail)
      },
      {
        ReleaseHistoryMessageId.ApprovalStatusChange,
        new Func<string, ReleaseHistoryMessageId, ReleaseRevisionChangeDetails>(ReleaseHistoryChangeDetailsTranslator.TranslateApprovalStatusChangeDetail)
      },
      {
        ReleaseHistoryMessageId.ReassignedApprovalChange,
        new Func<string, ReleaseHistoryMessageId, ReleaseRevisionChangeDetails>(ReleaseHistoryChangeDetailsTranslator.TranslateReassignApprovalStatusChangeDetail)
      },
      {
        ReleaseHistoryMessageId.ReleaseCreateOrUpdateChange,
        new Func<string, ReleaseHistoryMessageId, ReleaseRevisionChangeDetails>(ReleaseHistoryChangeDetailsTranslator.TranslateReleaseCreateOrUpdateChangeDetail)
      },
      {
        ReleaseHistoryMessageId.EnvironmentStatusCanceledByQueuingPolicy,
        new Func<string, ReleaseHistoryMessageId, ReleaseRevisionChangeDetails>(ReleaseHistoryChangeDetailsTranslator.TranslateEnvironmentStatusChangeDetail)
      },
      {
        ReleaseHistoryMessageId.DeploymentGateChange,
        new Func<string, ReleaseHistoryMessageId, ReleaseRevisionChangeDetails>(ReleaseHistoryChangeDetailsTranslator.TranslateDeploymentGateChangeDetail)
      }
    };

    public static ReleaseRevisionChangeDetails Translate(string inputMessage)
    {
      if (inputMessage == null || inputMessage.IsNullOrEmpty<char>())
        return (ReleaseRevisionChangeDetails) new UnknownReleaseRevisionChangeDetails(inputMessage);
      string enumName = ReleaseHistoryChangeDetailsExtensions.ExtractEnumName(inputMessage, "messageId", typeof (ReleaseHistoryMessageId));
      string str = inputMessage.Substring(inputMessage.IndexOf(":", StringComparison.Ordinal) + 1);
      ReleaseHistoryMessageId key;
      ref ReleaseHistoryMessageId local = ref key;
      return Enum.TryParse<ReleaseHistoryMessageId>(enumName, out local) && ReleaseHistoryChangeDetailsTranslator.ChangeDetailTranslatorMap.ContainsKey(key) ? ReleaseHistoryChangeDetailsTranslator.ChangeDetailTranslatorMap[key](str, key) : (ReleaseRevisionChangeDetails) new UnknownReleaseRevisionChangeDetails(Resources.ReleaseHistoryChangeDetailsUnknownMessage);
    }

    private static ReleaseRevisionChangeDetails TranslateReassignApprovalStatusChangeDetail(
      string arg,
      ReleaseHistoryMessageId messageId)
    {
      string str1 = ReleaseHistoryChangeDetailsExtensions.ExtractString(arg, "environmentName");
      EnvironmentStepType result;
      if (!Enum.TryParse<EnvironmentStepType>(ReleaseHistoryChangeDetailsExtensions.ExtractEnumName(arg, "approvalType", typeof (EnvironmentStepType)), out result))
        return (ReleaseRevisionChangeDetails) new UnknownReleaseRevisionChangeDetails(Resources.ReleaseHistoryChangeDetailsUnknownApprovalStatus);
      string str2 = ReleaseHistoryChangeDetailsExtensions.ExtractString(arg, "approverId");
      ReassignedApprovalChangeDetails approvalChangeDetails = new ReassignedApprovalChangeDetails();
      approvalChangeDetails.Id = messageId;
      approvalChangeDetails.StepType = result;
      approvalChangeDetails.AssignedTo = new IdentityRef()
      {
        Id = str2
      };
      approvalChangeDetails.EnvironmentName = str1;
      return (ReleaseRevisionChangeDetails) approvalChangeDetails;
    }

    private static ReleaseRevisionChangeDetails TranslateReleaseStatusChangeDetail(
      string arg,
      ReleaseHistoryMessageId messageId)
    {
      string str = ReleaseHistoryChangeDetailsExtensions.ExtractString(arg, "releaseName");
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseStatus result;
      if (!Enum.TryParse<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseStatus>(ReleaseHistoryChangeDetailsExtensions.ExtractEnumName(arg, "releaseStatus", typeof (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseStatus)), out result))
        return (ReleaseRevisionChangeDetails) new UnknownReleaseRevisionChangeDetails(Resources.ReleaseHistoryChangeDetailsUnknownReleaseStatus);
      ReleaseStatusChangeDetails statusChangeDetails = new ReleaseStatusChangeDetails();
      statusChangeDetails.Id = messageId;
      statusChangeDetails.ReleaseStatus = result;
      statusChangeDetails.ReleaseName = str;
      return (ReleaseRevisionChangeDetails) statusChangeDetails;
    }

    private static ReleaseRevisionChangeDetails TranslateReleaseCreateOrUpdateChangeDetail(
      string arg,
      ReleaseHistoryMessageId messageId)
    {
      string str = ReleaseHistoryChangeDetailsExtensions.ExtractString(arg, "releaseName");
      ReleaseHistoryChangeTypes result;
      if (!Enum.TryParse<ReleaseHistoryChangeTypes>(ReleaseHistoryChangeDetailsExtensions.ExtractEnumName(arg, "changeType", typeof (ReleaseHistoryChangeTypes)), out result))
        return (ReleaseRevisionChangeDetails) new UnknownReleaseRevisionChangeDetails(Resources.ReleaseHistoryChangeDetailsUnknownReleaseChangeType);
      ReleaseCreateOrUpdateChangeDetails updateChangeDetail = new ReleaseCreateOrUpdateChangeDetails();
      updateChangeDetail.Id = messageId;
      updateChangeDetail.ChangeType = result;
      updateChangeDetail.ReleaseName = str;
      return (ReleaseRevisionChangeDetails) updateChangeDetail;
    }

    private static ReleaseRevisionChangeDetails TranslateEnvironmentStatusChangeDetail(
      string arg,
      ReleaseHistoryMessageId messageId)
    {
      string str = ReleaseHistoryChangeDetailsExtensions.ExtractString(arg, "environmentName");
      ReleaseEnvironmentStatus result;
      if (!Enum.TryParse<ReleaseEnvironmentStatus>(ReleaseHistoryChangeDetailsExtensions.ExtractEnumName(arg, "environmentStatus", typeof (ReleaseEnvironmentStatus)), out result))
        return (ReleaseRevisionChangeDetails) new UnknownReleaseRevisionChangeDetails(Resources.ReleaseHistoryChangeDetailsUnknownReleaseEnvironmentStatus);
      ReleaseEnvironmentStatusChangeDetails statusChangeDetails = new ReleaseEnvironmentStatusChangeDetails();
      statusChangeDetails.Id = messageId;
      statusChangeDetails.EnvironmentStatus = result;
      statusChangeDetails.EnvironmentName = str;
      return (ReleaseRevisionChangeDetails) statusChangeDetails;
    }

    private static ReleaseRevisionChangeDetails TranslateDeploymentGateChangeDetail(
      string arg,
      ReleaseHistoryMessageId messageId)
    {
      string str1 = ReleaseHistoryChangeDetailsExtensions.ExtractString(arg, "gateName");
      string enumName = ReleaseHistoryChangeDetailsExtensions.ExtractEnumName(arg, "gateType", typeof (EnvironmentStepType));
      string str2 = ReleaseHistoryChangeDetailsExtensions.ExtractString(arg, "environmentName");
      int num = ReleaseHistoryChangeDetailsExtensions.ExtractInt(arg, "isProcessing");
      EnvironmentStepType environmentStepType;
      ref EnvironmentStepType local = ref environmentStepType;
      if (!Enum.TryParse<EnvironmentStepType>(enumName, out local))
        return (ReleaseRevisionChangeDetails) new UnknownReleaseRevisionChangeDetails(Resources.ReleaseHistoryChangeDetailsUnknownReleaseEnvironmentStatus);
      return (ReleaseRevisionChangeDetails) new DeploymentGateUpdateChangeDetails()
      {
        GateName = str1,
        GateType = environmentStepType,
        EnvironmentName = str2,
        IsProcessing = (num == 0)
      };
    }

    private static ReleaseRevisionChangeDetails TranslateApprovalStatusChangeDetail(
      string arg,
      ReleaseHistoryMessageId messageId)
    {
      string str = ReleaseHistoryChangeDetailsExtensions.ExtractString(arg, "environmentName");
      ReleaseEnvironmentStepStatus result1;
      if (Enum.TryParse<ReleaseEnvironmentStepStatus>(ReleaseHistoryChangeDetailsExtensions.ExtractEnumName(arg, "approvalStatus", typeof (ReleaseEnvironmentStepStatus)), out result1))
      {
        ApprovalStatus webApi = result1.ToWebApi();
        EnvironmentStepType result2;
        if (Enum.TryParse<EnvironmentStepType>(ReleaseHistoryChangeDetailsExtensions.ExtractEnumName(arg, "approvalType", typeof (EnvironmentStepType)), out result2))
        {
          ReleaseApprovalStatusChangeDetails statusChangeDetails = new ReleaseApprovalStatusChangeDetails();
          statusChangeDetails.Id = messageId;
          statusChangeDetails.ApprovalStatus = webApi;
          statusChangeDetails.ApprovalType = result2;
          statusChangeDetails.EnvironmentName = str;
          return (ReleaseRevisionChangeDetails) statusChangeDetails;
        }
      }
      return (ReleaseRevisionChangeDetails) new UnknownReleaseRevisionChangeDetails(Resources.ReleaseHistoryChangeDetailsUnknownApprovalStatus);
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DataImport.FrameworkDataImportJobManager
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud;
using Microsoft.VisualStudio.Services.Cloud.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.DataImport
{
  public class FrameworkDataImportJobManager : DataImportJobManager<FrameworkDataImportRequest>
  {
    internal static readonly Type[] OrderedSteps = new Type[8]
    {
      typeof (CreateCollectionDataImportRequest),
      typeof (DatabaseDataImportRequest),
      typeof (HostUpgradeDataImportRequest),
      typeof (OnlinePostHostUpgradeDataImportRequest),
      typeof (StopHostAfterUpgradeDataImportRequest),
      typeof (ObtainDatabaseHoldDataImportRequest),
      typeof (HostMoveDataImportRequest),
      typeof (ActivateDataImportRequest)
    };
    private static readonly Type[] s_cleanupRequests = new Type[2]
    {
      typeof (DataImportDehydrateRequest),
      typeof (RemoveDataImportRequest)
    };
    private readonly Dictionary<Type, Type[]> m_validContinuations = new Dictionary<Type, Type[]>()
    {
      {
        typeof (CreateCollectionDataImportRequest),
        (Type[]) null
      },
      {
        typeof (DatabaseDataImportRequest),
        FrameworkDataImportJobManager.FollowingSteps(typeof (DatabaseDataImportRequest), (IEnumerable<Type>) FrameworkDataImportJobManager.s_cleanupRequests)
      },
      {
        typeof (HostUpgradeDataImportRequest),
        FrameworkDataImportJobManager.FollowingSteps(typeof (HostUpgradeDataImportRequest), (IEnumerable<Type>) FrameworkDataImportJobManager.s_cleanupRequests)
      },
      {
        typeof (OnlinePostHostUpgradeDataImportRequest),
        FrameworkDataImportJobManager.FollowingSteps(typeof (OnlinePostHostUpgradeDataImportRequest), (IEnumerable<Type>) FrameworkDataImportJobManager.s_cleanupRequests)
      },
      {
        typeof (StopHostAfterUpgradeDataImportRequest),
        FrameworkDataImportJobManager.FollowingSteps(typeof (StopHostAfterUpgradeDataImportRequest), (IEnumerable<Type>) FrameworkDataImportJobManager.s_cleanupRequests)
      },
      {
        typeof (ObtainDatabaseHoldDataImportRequest),
        FrameworkDataImportJobManager.FollowingSteps(typeof (ObtainDatabaseHoldDataImportRequest), (IEnumerable<Type>) FrameworkDataImportJobManager.s_cleanupRequests)
      },
      {
        typeof (HostMoveDataImportRequest),
        FrameworkDataImportJobManager.FollowingSteps(typeof (HostMoveDataImportRequest), (IEnumerable<Type>) FrameworkDataImportJobManager.s_cleanupRequests)
      },
      {
        typeof (DataImportDehydrateRequest),
        (Type[]) null
      },
      {
        typeof (RemoveDataImportRequest),
        Array.Empty<Type>()
      },
      {
        typeof (ActivateDataImportRequest),
        Array.Empty<Type>()
      }
    };

    protected override void ValidateRequest(
      IVssRequestContext requestContext,
      FrameworkDataImportRequest request,
      TeamFoundationJobDefinition jobDefinition)
    {
      base.ValidateRequest(requestContext, request, jobDefinition);
      request.Accept<bool>((IFrameworkDataImportRequestVisitor<bool>) new FrameworkDataImportJobValidator(requestContext, jobDefinition));
      if (jobDefinition == null)
        return;
      this.CheckContinuationPolicy(request, jobDefinition);
    }

    private void CheckContinuationPolicy(
      FrameworkDataImportRequest request,
      TeamFoundationJobDefinition jobDefinition)
    {
      List<FrameworkDataImportRequest> dataImportRequestList = ServicingOrchestrationJobManager<FrameworkDataImportRequest>.DeserializeJobData(jobDefinition.Data);
      for (int index = 0; index < dataImportRequestList.Count; ++index)
      {
        Type type = dataImportRequestList[index].GetType();
        Type[] source;
        if (!this.m_validContinuations.TryGetValue(type, out source))
          throw new InvalidOperationException(string.Format("No continuation policy defined for type {0}", (object) type));
        if (!(type == request.GetType()) && source != null && !((IEnumerable<Type>) source).Contains<Type>(request.GetType()) && (!(type == typeof (ActivateDataImportRequest)) || !(request.GetType() == typeof (RemoveDataImportRequest)) || !(request as RemoveDataImportRequest).IgnoreImportStatus))
        {
          string message = string.Format("Request {0} may not be executed after {1}", (object) request.GetType(), (object) type);
          if (source.Length == 0)
            throw new DataImportFinalizedException(message);
          throw new DataImportRequestOutOfOrderException(message);
        }
      }
    }

    internal static Type[] FollowingSteps(Type type, IEnumerable<Type> additionalRequests)
    {
      int num = Array.IndexOf<Type>(FrameworkDataImportJobManager.OrderedSteps, type);
      if (num < 0)
        throw new ArgumentException("Did not find " + type.Name + " in ordered steps");
      return ((IEnumerable<Type>) FrameworkDataImportJobManager.OrderedSteps).Skip<Type>(num + 1).Union<Type>(additionalRequests).ToArray<Type>();
    }
  }
}

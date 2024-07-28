// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.FrameworkReparentCollectionJobManager
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud;
using Microsoft.VisualStudio.Services.Cloud.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Organization
{
  public class FrameworkReparentCollectionJobManager : 
    ReparentCollectionJobManager<FrameworkReparentCollectionRequest>
  {
    private readonly Dictionary<Type, Type[]> m_validContinuations = new Dictionary<Type, Type[]>()
    {
      {
        typeof (InitializeReparentCollectionRequest),
        new Type[2]
        {
          typeof (PerformReparentCollectionRequest),
          typeof (RollbackReparentCollectionRequest)
        }
      },
      {
        typeof (PerformReparentCollectionRequest),
        new Type[2]
        {
          typeof (CompleteReparentCollectionRequest),
          typeof (RollbackReparentCollectionRequest)
        }
      },
      {
        typeof (CompleteReparentCollectionRequest),
        new Type[1]{ typeof (FinalizeReparentCollectionRequest) }
      },
      {
        typeof (RollbackReparentCollectionRequest),
        new Type[1]{ typeof (FinalizeReparentCollectionRequest) }
      },
      {
        typeof (FinalizeReparentCollectionRequest),
        Array.Empty<Type>()
      }
    };

    protected override void ValidateRequest(
      IVssRequestContext requestContext,
      FrameworkReparentCollectionRequest request,
      TeamFoundationJobDefinition jobDefinition)
    {
      base.ValidateRequest(requestContext, request, jobDefinition);
      if (jobDefinition == null)
        return;
      this.CheckContinuationPolicy(request, jobDefinition);
    }

    private void CheckContinuationPolicy(
      FrameworkReparentCollectionRequest request,
      TeamFoundationJobDefinition jobDefinition)
    {
      List<FrameworkReparentCollectionRequest> source1 = ServicingOrchestrationJobManager<FrameworkReparentCollectionRequest>.DeserializeJobData(jobDefinition.Data);
      if (source1.Count <= 0)
        return;
      Type type = source1.Last<FrameworkReparentCollectionRequest>().GetType();
      Type[] source2;
      if (!this.m_validContinuations.TryGetValue(type, out source2))
        throw new InvalidOperationException(string.Format("No continuation policy defined for type {0}", (object) type));
      if (type == request.GetType() || source2 == null || ((IEnumerable<Type>) source2).Contains<Type>(request.GetType()))
        return;
      string message = string.Format("ReparentCollection request {0} may not be executed after {1}", (object) request.GetType(), (object) type);
      if (source2.Length == 0)
        throw new ServicingOrchestrationFinalizedException(message);
      throw new ServicingOrchestrationRequestOutOfOrderException(message);
    }
  }
}

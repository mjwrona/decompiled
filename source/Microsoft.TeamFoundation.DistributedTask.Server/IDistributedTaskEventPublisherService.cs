// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.IDistributedTaskEventPublisherService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  [DefaultServiceImplementation(typeof (DistributedTaskEventPublisherService))]
  internal interface IDistributedTaskEventPublisherService : IVssFrameworkService
  {
    void NotifyAgentChangeEvent(
      IVssRequestContext requestContext,
      string eventType,
      TaskAgentPool pool,
      TaskAgent agent);

    void NotifyAgentPoolEvent(
      IVssRequestContext requestContext,
      string eventType,
      TaskAgentPool pool);

    void NotifyAgentQueueEvent(
      IVssRequestContext requestContext,
      string eventType,
      TaskAgentQueue queue);

    void NotifyAgentQueuesEvent(
      IVssRequestContext requestContext,
      string eventType,
      IEnumerable<TaskAgentQueue> queues);

    void NotifyElasticPoolResizedEvent(
      IVssRequestContext requestContext,
      TaskAgentPool pool,
      ElasticPool elasticPool,
      int previousSize,
      int newSize);

    void NotifyMachinesChangedEvent(
      IVssRequestContext requestContext,
      DeploymentGroup machineGroup,
      IList<DeploymentMachineChangedData> deploymentMachines);

    void NotifySecureFilesEvent(
      IVssRequestContext requestContext,
      string eventType,
      IEnumerable<SecureFile> secureFiles,
      Guid projectId);
  }
}

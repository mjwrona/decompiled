// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ServicingOrchestrationServiceProvider`2
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public abstract class ServicingOrchestrationServiceProvider<TRequest, TStatus>
    where TRequest : FrameworkServicingOrchestrationRequest
    where TStatus : ServicingOrchestrationRequestStatus
  {
    protected readonly IReadOnlyDictionary<Guid, string> m_services;
    protected readonly IReadOnlyDictionary<Guid, TRequest> m_requests;

    public virtual string Area => "ServicingOrchestration";

    public virtual string Layer => "ServiceProvider";

    public Guid RequestId { get; }

    public Guid ServicingJobId { get; }

    public ServicingOrchestrationServiceProvider(
      IDictionary<Guid, string> serviceInstances,
      TRequest request,
      ITFLogger logger = null)
    {
      ArgumentUtility.CheckForNull<IDictionary<Guid, string>>(serviceInstances, nameof (serviceInstances));
      ArgumentUtility.CheckForNull<TRequest>(request, nameof (request));
      this.Logger = logger ?? (ITFLogger) new NullLogger();
      this.m_services = (IReadOnlyDictionary<Guid, string>) new ReadOnlyDictionary<Guid, string>(serviceInstances);
      this.m_requests = (IReadOnlyDictionary<Guid, TRequest>) new ReadOnlyDictionary<Guid, TRequest>((IDictionary<Guid, TRequest>) serviceInstances.ToDictionary<KeyValuePair<Guid, string>, Guid, TRequest>((Func<KeyValuePair<Guid, string>, Guid>) (x => x.Key), (Func<KeyValuePair<Guid, string>, TRequest>) (y => request)));
      this.RequestId = request.RequestId;
      this.ServicingJobId = request.ServicingJobId;
      this.Logger.Info(string.Format("ServiceProvider: {0} for Request={1}, ServicingJob={2}", (object) this.m_requests.Count, (object) this.RequestId, (object) this.ServicingJobId));
    }

    public virtual IEnumerable<Guid> QueueRequests(
      IVssRequestContext requestContext,
      Type[] ignoredExceptions = null)
    {
      List<Guid> guidList = new List<Guid>();
      List<Exception> source = new List<Exception>();
      foreach (KeyValuePair<Guid, TRequest> request in (IEnumerable<KeyValuePair<Guid, TRequest>>) this.m_requests)
      {
        this.Logger.Info(string.Format("ServiceProvider: Calling {0} {1}", (object) request.Key, (object) request.Value));
        try
        {
          this.MakeSynchronousRestCall(requestContext, request.Key, request.Value);
          guidList.Add(request.Key);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(610006, this.Area, this.Layer, ex);
          this.Logger.Warning(string.Format("ServiceProvider: Failed to make Rest Call for service {0} {1} {2}: {3}", (object) request.Key, (object) request.Value, (object) ex.Message, (object) ex.StackTrace));
          source.Add(ex);
        }
      }
      Exception exception = source.FirstOrDefault<Exception>((Func<Exception, bool>) (ex => ignoredExceptions == null || !ServicingOrchestrationServiceProvider<TRequest, TStatus>.TypeOrSubTypeOf(ex.GetType(), ignoredExceptions)));
      if (exception != null)
        throw exception;
      return (IEnumerable<Guid>) guidList;
    }

    public IDictionary<Guid, TStatus> BlockForCompletion(
      IVssRequestContext requestContext,
      IServicingOrchestrationRetryPolicy timeoutPolicy)
    {
      HashSet<Guid> guidSet = new HashSet<Guid>(this.m_requests.Keys);
      Dictionary<Guid, TStatus> statuses = new Dictionary<Guid, TStatus>();
      int num = 0;
      Guid key;
      while (true)
      {
        foreach (Guid guid in guidSet)
          statuses[guid] = this.GetRequestStatus(requestContext, guid);
        key = statuses.Where<KeyValuePair<Guid, TStatus>>((Func<KeyValuePair<Guid, TStatus>, bool>) (x => x.Value.ServicingJobId != this.ServicingJobId)).FirstOrDefault<KeyValuePair<Guid, TStatus>>().Key;
        if (!(key != Guid.Empty))
        {
          guidSet.RemoveWhere((Predicate<Guid>) (x => statuses[x].Completed()));
          if (guidSet.Count != 0 && num++ < timeoutPolicy.RetryCount)
            Thread.Sleep(timeoutPolicy.Delay);
          else
            goto label_10;
        }
        else
          break;
      }
      throw new InvalidOperationException(string.Format("Unexpected servicing job {0} running at service {1}, expected {2}", (object) statuses[key].ServicingJobId, (object) key, (object) this.ServicingJobId));
label_10:
      return (IDictionary<Guid, TStatus>) statuses;
    }

    public IDictionary<Guid, TStatus> AwaitWithProgressLog(IVssRequestContext requestContext)
    {
      IDictionary<Guid, TStatus> dictionary;
      do
      {
        dictionary = this.BlockForCompletion(requestContext, (IServicingOrchestrationRetryPolicy) new SimpleRetryPolicy(3, TimeSpan.FromSeconds(10.0)));
        foreach (KeyValuePair<Guid, TStatus> keyValuePair in (IEnumerable<KeyValuePair<Guid, TStatus>>) dictionary)
          this.Logger.Info(string.Format("Service {0} status={1}", (object) keyValuePair.Key, (object) keyValuePair.Value));
      }
      while (dictionary.Values.Any<TStatus>((Func<TStatus, bool>) (x => x.Active())));
      this.Logger.Info("Finished awaiting Jobs for [" + string.Join(", ", dictionary.Keys.Select<Guid, string>((Func<Guid, string>) (service => this.m_services[service]))) + "] services");
      return dictionary;
    }

    public void AwaitWithProgressThrowOnFailure(IVssRequestContext requestContext)
    {
      IDictionary<Guid, TStatus> source = this.AwaitWithProgressLog(requestContext);
      List<ServiceRequestFailedException> list = source.Where<KeyValuePair<Guid, TStatus>>((Func<KeyValuePair<Guid, TStatus>, bool>) (x => !x.Value.Succeeded())).Select<KeyValuePair<Guid, TStatus>, ServiceRequestFailedException>((Func<KeyValuePair<Guid, TStatus>, ServiceRequestFailedException>) (failure => new ServiceRequestFailedException(failure.Key, (ServicingOrchestrationRequestStatus) failure.Value))).ToList<ServiceRequestFailedException>();
      foreach (KeyValuePair<Guid, TStatus> keyValuePair in source.Where<KeyValuePair<Guid, TStatus>>((Func<KeyValuePair<Guid, TStatus>, bool>) (x => x.Value.Properties != null && x.Value.Properties.Any<PropertyPair>())))
        this.ProcessResult(requestContext, keyValuePair.Key, keyValuePair.Value);
      if (list.Count > 0)
      {
        foreach (ServiceRequestFailedException serviceException in list)
          this.ProcessResult(requestContext, serviceException.ServiceId, serviceException);
        throw list[0];
      }
    }

    public abstract TStatus GetRequestStatus(IVssRequestContext requestContext, Guid serviceId);

    protected abstract void MakeSynchronousRestCall(
      IVssRequestContext requestContext,
      Guid serviceId,
      TRequest request);

    protected virtual void ProcessResult(
      IVssRequestContext requestContext,
      Guid serviceId,
      TStatus serviceResult)
    {
    }

    protected virtual void ProcessResult(
      IVssRequestContext requestContext,
      Guid serviceId,
      ServiceRequestFailedException serviceException)
    {
    }

    protected ITFLogger Logger { get; }

    protected static bool TypeOrSubTypeOf(Type t, Type[] types) => Array.Exists<Type>(types, (Predicate<Type>) (item => t == item || t.IsSubclassOf(item)));
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution.AggregationResolvingFactory`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution
{
  public class AggregationResolvingFactory<TBootstrapped> : 
    IFactory<IFeedRequest, Task<TBootstrapped>>
    where TBootstrapped : class
  {
    private readonly IAggregationAccessorFactory readMigrationAccessorFactory;
    private readonly IEnumerable<IRequireAggBootstrapper<TBootstrapped>> bootstrapperSequence;
    private readonly AggregationHandlerPolicy policy;
    private readonly ITracerService tracerService;

    public AggregationResolvingFactory(
      AggregationHandlerPolicy policy,
      ITracerService tracerService,
      IEnumerable<IRequireAggBootstrapper<TBootstrapped>> bootstrapperSequence,
      IAggregationAccessorFactory readMigrationAccessorFactory)
    {
      this.bootstrapperSequence = bootstrapperSequence;
      this.policy = policy;
      this.tracerService = tracerService;
      this.readMigrationAccessorFactory = readMigrationAccessorFactory;
    }

    public async Task<TBootstrapped> Get(IFeedRequest request)
    {
      AggregationResolvingFactory<TBootstrapped> sendInTheThisObject = this;
      IProtocol protocol;
      TBootstrapped bootstrapped;
      using (ITracerBlock tracer = sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (Get)))
      {
        protocol = request.Protocol;
        IReadOnlyList<IAggregationAccessor> aggregations = await sendInTheThisObject.readMigrationAccessorFactory.GetAccessorsFor(request);
        tracer.TraceInfo(string.Format("resolved aggregations for feed: {0} from protocol: {1} to resolved aggregations:{2}", (object) request.Feed.Id, (object) protocol, (object) string.Join(",", aggregations.Select<IAggregationAccessor, string>((Func<IAggregationAccessor, string>) (a => a.Aggregation.Definition.Name + "-" + a.Aggregation.VersionName)))));
        IList<TBootstrapped> list = (IList<TBootstrapped>) sendInTheThisObject.bootstrapperSequence.Select<IRequireAggBootstrapper<TBootstrapped>, TBootstrapped>((Func<IRequireAggBootstrapper<TBootstrapped>, TBootstrapped>) (possibleHandlerBootstrapper => possibleHandlerBootstrapper.Bootstrap((IReadOnlyCollection<IAggregationAccessor>) aggregations))).Where<TBootstrapped>((Func<TBootstrapped, bool>) (possibleHandler => (object) possibleHandler != null)).ToList<TBootstrapped>();
        tracer.TraceInfo(string.Format("chose handler: {0} for feed: {1} for protocol: {2}", (object) list.FirstOrDefault<TBootstrapped>(), (object) request.Feed.Id, (object) protocol));
        if (!list.Any<TBootstrapped>())
        {
          StringBuilder stringBuilder = new StringBuilder();
          stringBuilder.AppendLine("A handler could not be selected because all of the provided bootstrappers returned null. Most likely the bootstrappers demand at least one aggregation that is not available in the current migration.");
          stringBuilder.AppendLine();
          stringBuilder.AppendLine("Demands:");
          foreach (IRequireAggBootstrapper<TBootstrapped> requireAggBootstrapper in sendInTheThisObject.bootstrapperSequence)
          {
            stringBuilder.AppendLine("  - " + requireAggBootstrapper.GetType().GetPrettyName());
            foreach (string namesForDiagnostic in requireAggBootstrapper.GetAggregationNamesForDiagnostics())
              stringBuilder.AppendLine("      - " + namesForDiagnostic);
          }
          stringBuilder.AppendLine();
          stringBuilder.AppendLine("Migration contains these aggregations:");
          foreach (IAggregationAccessor aggregationAccessor in (IEnumerable<IAggregationAccessor>) aggregations)
          {
            stringBuilder.AppendLine("  - " + aggregationAccessor.Aggregation.Definition.Name + " " + aggregationAccessor.Aggregation.VersionName + ", implemented by " + aggregationAccessor.GetType().GetPrettyName() + ", provides:");
            foreach (Type type in aggregationAccessor.GetType().GetInterfaces())
              stringBuilder.AppendLine("      - " + type.GetPrettyName());
          }
          throw new InvalidHandlerException(stringBuilder.ToString());
        }
        if (sendInTheThisObject.policy == AggregationHandlerPolicy.RequireExactlyOneUsableHandler && list.Count<TBootstrapped>() > 1)
          throw new InvalidHandlerException(Resources.Error_MultipleHandlersMatching());
        bootstrapped = list.First<TBootstrapped>();
      }
      protocol = (IProtocol) null;
      return bootstrapped;
    }
  }
}

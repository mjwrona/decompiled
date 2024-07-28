// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityDescriptorChangeComponent
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.VisualStudio.Services.Identity
{
  public class IdentityDescriptorChangeComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<IdentityDescriptorChangeComponent>(1)
    }, "IdentityDescriptorChanges");

    public IdentityDescriptorChangeComponent() => this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;

    protected override void SetupComponentCircuitBreaker(
      IVssRequestContext requestContext,
      string databaseName,
      ApplicationIntent applicationIntent)
    {
      string key = "IdentityDescriptorChangeComponent-" + databaseName;
      this.ComponentLevelCommandSetter.AndCommandKey((Microsoft.VisualStudio.Services.CircuitBreaker.CommandKey) key);
      this.ComponentLevelCircuitBreakerProperties = (ICommandProperties) new CommandPropertiesRegistry(requestContext, (Microsoft.VisualStudio.Services.CircuitBreaker.CommandKey) key, this.ComponentLevelCommandSetter.CommandPropertiesDefaults);
    }

    internal virtual IList<IdentityDescriptorChange> GetDescriptorChanges(
      int fromSequenceId,
      int toSequenceId)
    {
      this.PrepareStoredProcedure("prc_GetDescriptorChanges");
      this.BindInt("@fromSequenceId", fromSequenceId);
      this.BindInt("@toSequenceId", toSequenceId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<IdentityDescriptorChange>((ObjectBinder<IdentityDescriptorChange>) new IdentityDescriptorChangeBinder());
        return (IList<IdentityDescriptorChange>) resultCollection.GetCurrent<IdentityDescriptorChange>().Items;
      }
    }
  }
}

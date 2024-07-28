// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.BlobStoreLogSettingsJob
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public class BlobStoreLogSettingsJob : VssAsyncJobExtension
  {
    private 
    #nullable disable
    BlobStoreLogSettingsJob.SettingPrefix[] prefixQueries = new BlobStoreLogSettingsJob.SettingPrefix[1]
    {
      new BlobStoreLogSettingsJob.SettingPrefix()
      {
        Prefix = "/Configuration/ClientSettings",
        Area = "Client"
      }
    };

    public override Task<VssJobResult> RunAsync(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime)
    {
      string str = requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) ? string.Empty : requestContext.ServiceHost.InstanceId.ToString();
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      foreach (BlobStoreLogSettingsJob.SettingPrefix prefixQuery in this.prefixQueries)
      {
        foreach (RegistryEntry readEntry in service.ReadEntries(requestContext, new RegistryQuery(prefixQuery.Prefix + "/**")))
        {
          BlobStoreLogSettingsJob.SettingValue settingValue = new BlobStoreLogSettingsJob.SettingValue()
          {
            Path = readEntry.Path.Substring(prefixQuery.Prefix.Length),
            Value = readEntry.Value,
            Area = prefixQuery.Area,
            HostId = str
          };
          requestContext.TraceAlways(ContentTracePoints.BlobStoreLogSettingsJob.SettingsInfo, JsonConvert.SerializeObject((object) settingValue));
        }
      }
      return Task.FromResult<VssJobResult>(new VssJobResult(TeamFoundationJobExecutionResult.Succeeded, ""));
    }

    private record SettingValue()
    {
      public string Value;
      public string Path;
      public string Area;
      public string HostId;

      [CompilerGenerated]
      protected virtual bool PrintMembers(
      #nullable enable
      StringBuilder builder)
      {
        RuntimeHelpers.EnsureSufficientExecutionStack();
        builder.Append("Value = ");
        builder.Append((object) this.Value);
        builder.Append(", Path = ");
        builder.Append((object) this.Path);
        builder.Append(", Area = ");
        builder.Append((object) this.Area);
        builder.Append(", HostId = ");
        builder.Append((object) this.HostId);
        return true;
      }

      [CompilerGenerated]
      public override int GetHashCode() => (((EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.Value)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.Path)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.Area)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.HostId);

      [CompilerGenerated]
      public virtual bool Equals(BlobStoreLogSettingsJob.SettingValue? other)
      {
        if ((object) this == (object) other)
          return true;
        return (object) other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<string>.Default.Equals(this.Value, other.Value) && EqualityComparer<string>.Default.Equals(this.Path, other.Path) && EqualityComparer<string>.Default.Equals(this.Area, other.Area) && EqualityComparer<string>.Default.Equals(this.HostId, other.HostId);
      }

      [CompilerGenerated]
      protected SettingValue(BlobStoreLogSettingsJob.SettingValue original)
      {
        this.Value = original.Value;
        this.Path = original.Path;
        this.Area = original.Area;
        this.HostId = original.HostId;
      }
    }

    private record SettingPrefix()
    {
      public 
      #nullable disable
      string Prefix;
      public string Area;

      [CompilerGenerated]
      protected virtual bool PrintMembers(
      #nullable enable
      StringBuilder builder)
      {
        RuntimeHelpers.EnsureSufficientExecutionStack();
        builder.Append("Prefix = ");
        builder.Append((object) this.Prefix);
        builder.Append(", Area = ");
        builder.Append((object) this.Area);
        return true;
      }

      [CompilerGenerated]
      public override int GetHashCode() => (EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.Prefix)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.Area);

      [CompilerGenerated]
      public virtual bool Equals(BlobStoreLogSettingsJob.SettingPrefix? other)
      {
        if ((object) this == (object) other)
          return true;
        return (object) other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<string>.Default.Equals(this.Prefix, other.Prefix) && EqualityComparer<string>.Default.Equals(this.Area, other.Area);
      }

      [CompilerGenerated]
      protected SettingPrefix(BlobStoreLogSettingsJob.SettingPrefix original)
      {
        this.Prefix = original.Prefix;
        this.Area = original.Area;
      }
    }
  }
}

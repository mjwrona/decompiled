// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.MdmSliConfiguration
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class MdmSliConfiguration
  {
    private object m_lock_object = new object();

    [JsonProperty]
    public string MdmMetric { get; set; }

    [JsonProperty]
    private List<MdmSliConfiguration.ApplicationAndCommand> Commands { get; set; }

    private Dictionary<string, HashSet<string>> ApplicationCommandWhitelist { get; set; }

    private void Initialize()
    {
      if (this.ApplicationCommandWhitelist != null)
        return;
      this.ApplicationCommandWhitelist = new Dictionary<string, HashSet<string>>();
      if (this.Commands == null)
        return;
      foreach (MdmSliConfiguration.ApplicationAndCommand command in this.Commands)
      {
        if (!this.ApplicationCommandWhitelist.ContainsKey(command.Application))
          this.ApplicationCommandWhitelist[command.Application] = new HashSet<string>();
        this.ApplicationCommandWhitelist[command.Application].Add(command.Command);
      }
    }

    public bool ShouldApplicationCommandPublishSli(string application, string command)
    {
      if (this.ApplicationCommandWhitelist == null)
      {
        lock (this.m_lock_object)
          this.Initialize();
      }
      if (this.ApplicationCommandWhitelist == null || this.ApplicationCommandWhitelist.Count == 0)
        return true;
      return this.ApplicationCommandWhitelist.ContainsKey(application) && this.ApplicationCommandWhitelist[application].Contains(command);
    }

    private class ApplicationAndCommand
    {
      public string Application { get; set; }

      public string Command { get; set; }
    }
  }
}

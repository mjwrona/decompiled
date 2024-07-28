// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.TeamConfigurationComponent6
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess
{
  internal class TeamConfigurationComponent6 : TeamConfigurationComponent5
  {
    protected override void BindDataspaceId(Guid dataspaceIdentifier) => this.BindInt("@dataspaceId", this.GetDataspaceId(dataspaceIdentifier));

    internal override void SetBacklogVisibilitiesProperty(
      Guid projectId,
      Guid teamId,
      IDictionary<string, bool> visibilities)
    {
      MemoryStream memoryStream = new MemoryStream();
      new DataContractSerializer(typeof (Dictionary<string, bool>)).WriteObject((Stream) memoryStream, (object) visibilities);
      memoryStream.Close();
      KeyValuePair<string, string> keyValuePair = new KeyValuePair<string, string>("BacklogVisibilities", Encoding.UTF8.GetString(memoryStream.ToArray()));
      this.SetTeamConfigurationProperties(projectId, teamId, (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[1]
      {
        keyValuePair
      });
    }
  }
}

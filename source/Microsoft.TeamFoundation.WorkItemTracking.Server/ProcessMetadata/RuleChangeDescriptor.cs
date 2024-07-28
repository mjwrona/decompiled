// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata.RuleChangeDescriptor
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata
{
  internal sealed class RuleChangeDescriptor
  {
    public RuleChangeDescriptor(IEnumerable<Guid> rulesToEnable, IEnumerable<Guid> rulesToDisable)
    {
      this.RulesToDisable = rulesToDisable != null ? new HashSet<Guid>(rulesToDisable.Where<Guid>((Func<Guid, bool>) (r => r != Guid.Empty)).Distinct<Guid>()) : new HashSet<Guid>();
      this.RulesToEnable = rulesToEnable != null ? new HashSet<Guid>(rulesToEnable.Where<Guid>((Func<Guid, bool>) (r => r != Guid.Empty)).Distinct<Guid>()) : new HashSet<Guid>();
    }

    public HashSet<Guid> RulesToEnable { get; private set; }

    public HashSet<Guid> RulesToDisable { get; private set; }
  }
}

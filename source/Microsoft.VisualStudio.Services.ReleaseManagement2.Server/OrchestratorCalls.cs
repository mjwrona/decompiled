// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.OrchestratorCalls
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server
{
  public class OrchestratorCalls
  {
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Replicates the signature of OSP")]
    public Func<IEnumerable<ReleaseEnvironmentStep>, IEnumerable<Release>, IEnumerable<ReleaseEnvironmentStep>> AcceptStep { get; set; }

    public Func<IEnumerable<ReleaseEnvironmentStep>, IList<Release>, IEnumerable<ReleaseEnvironmentStep>> RejectStep { get; set; }

    public Func<IEnumerable<ReleaseEnvironmentStep>, IEnumerable<ReleaseEnvironmentStep>> ReassignStep { get; set; }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs.RequirementParser
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Pegasus.Common.Tracing;
using System;
using System.Threading;

namespace Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs
{
  public static class RequirementParser
  {
    public static RequirementSpec ParseRequirement(
      string subject,
      TimeSpan timeout,
      string fileName = null,
      ITracer tracer = null)
    {
      using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(timeout))
        return new RequirementParserInternal()
        {
          Tracer = ((ITracer) new CancellingTracer(cancellationTokenSource.Token, tracer))
        }.ParseRequirement(subject, fileName);
    }

    public static VersionConstraintList ParseVersionConstraintList(
      string subject,
      TimeSpan timeout,
      string fileName = null,
      ITracer tracer = null)
    {
      using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(timeout))
        return new RequirementParserInternal()
        {
          Tracer = ((ITracer) new CancellingTracer(cancellationTokenSource.Token))
        }.ParseVersionConstraintList(subject, fileName);
    }
  }
}

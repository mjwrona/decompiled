// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.EmailSpecification.EmailParser
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs;
using Pegasus.Common.Tracing;
using System;
using System.Threading;

namespace Microsoft.VisualStudio.Services.PyPi.Server.EmailSpecification
{
  public static class EmailParser
  {
    public static EmailSpec ParseEmail(
      string subject,
      TimeSpan timeout,
      string fileName = null,
      ITracer tracer = null)
    {
      using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(timeout))
        return new EmailParserInternal()
        {
          Tracer = ((ITracer) new CancellingTracer(cancellationTokenSource.Token, tracer))
        }.ParseEmail(subject, fileName);
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TfsSubjectDescriptorImpersonationMessageHandler
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class TfsSubjectDescriptorImpersonationMessageHandler : DelegatingHandler
  {
    private const string c_area = "Identity";
    private const string c_layer = "TfsSubjectDescriptorImpersonationMessageHandler";
    private readonly SubjectDescriptor m_SubjectDescriptor;

    internal TfsSubjectDescriptorImpersonationMessageHandler(IVssRequestContext requestContext)
    {
      if (requestContext.IsSystemContext)
      {
        requestContext.Trace(639664896, TraceLevel.Verbose, "Identity", nameof (TfsSubjectDescriptorImpersonationMessageHandler), "Subject descriptor is not added to the header since this is a system context");
      }
      else
      {
        SubjectDescriptor? subjectDescriptor = requestContext.GetUserIdentity()?.SubjectDescriptor;
        this.m_SubjectDescriptor = SubjectDescriptor.FromString(subjectDescriptor.HasValue ? (string) subjectDescriptor.GetValueOrDefault() : (string) null);
        if (!(this.m_SubjectDescriptor != new SubjectDescriptor()))
          return;
        requestContext.Trace(639664898, TraceLevel.Verbose, "Identity", nameof (TfsSubjectDescriptorImpersonationMessageHandler), string.Format("Subject descriptor is found {0}", (object) this.m_SubjectDescriptor));
      }
    }

    protected override Task<HttpResponseMessage> SendAsync(
      HttpRequestMessage request,
      CancellationToken cancellationToken)
    {
      if (this.m_SubjectDescriptor != new SubjectDescriptor())
        request.Headers.Add("X-TFS-SubjectDescriptorImpersonate", (string) this.m_SubjectDescriptor);
      return base.SendAsync(request, cancellationToken);
    }
  }
}

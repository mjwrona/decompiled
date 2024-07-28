// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SubjectTemplate
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class SubjectTemplate
  {
    private const string c_area = "Security";
    private const string c_layer = "SubjectTemplate";

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("value")]
    public JToken Value { get; set; }

    public IEnumerable<IdentityDescriptor> GetSubjects(
      IVssRequestContext requestContext,
      object tokenContext)
    {
      ISecurityTemplateSubjectGeneratorExtension generatorExtension = requestContext.To(TeamFoundationHostType.Deployment).GetService<SecurityTemplateService>().GetSubjectGeneratorExtension(this.Type);
      if (generatorExtension != null)
        return (IEnumerable<IdentityDescriptor>) generatorExtension.GetSubjects(requestContext, this.Value, tokenContext).ToList<IdentityDescriptor>();
      requestContext.Trace(56290, TraceLevel.Error, "Security", nameof (SubjectTemplate), "Unable to load subject generator extension of type " + this.Type);
      return Enumerable.Empty<IdentityDescriptor>();
    }
  }
}

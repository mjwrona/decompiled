// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.Services.ArtifactProvenanceService
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.Extensions;
using Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Pipelines.Deployment.Services
{
  internal sealed class ArtifactProvenanceService : IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public IEnumerable<ArtifactProvenance> GetArtifactProvenances(
      IVssRequestContext requestContext,
      string resourceUris,
      string typeFilters)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) resourceUris, nameof (resourceUris));
      IEnumerable<string> strings = ((IEnumerable<string>) resourceUris.Split(new char[1]
      {
        ','
      }, StringSplitOptions.RemoveEmptyEntries)).Select<string, string>((Func<string, string>) (s => s.Trim()));
      if (!strings.Any<string>())
        throw new ArgumentException(nameof (resourceUris));
      List<Microsoft.Azure.Pipelines.Deployment.Model.NoteKind> source1 = new List<Microsoft.Azure.Pipelines.Deployment.Model.NoteKind>();
      if (typeFilters == null)
      {
        source1 = this.GetAllKinds();
      }
      else
      {
        IEnumerable<string> source2 = ((IEnumerable<string>) typeFilters.Split(new char[1]
        {
          ','
        }, StringSplitOptions.RemoveEmptyEntries)).Select<string, string>((Func<string, string>) (s => s.Trim()));
        if (!source2.Any<string>())
        {
          source1 = this.GetAllKinds();
        }
        else
        {
          foreach (string str in source2)
          {
            Microsoft.Azure.Pipelines.Deployment.Model.NoteKind result;
            if (!Enum.TryParse<Microsoft.Azure.Pipelines.Deployment.Model.NoteKind>(str, out result))
              throw new ArgumentException(nameof (typeFilters));
            source1.Add(result);
          }
        }
      }
      return requestContext.GetService<IArtifactMetadataService>().GetOccurrences(requestContext, strings.Distinct<string>(), source1.Distinct<Microsoft.Azure.Pipelines.Deployment.Model.NoteKind>()).ToArtifactProvenances(strings);
    }

    private List<Microsoft.Azure.Pipelines.Deployment.Model.NoteKind> GetAllKinds() => new List<Microsoft.Azure.Pipelines.Deployment.Model.NoteKind>()
    {
      Microsoft.Azure.Pipelines.Deployment.Model.NoteKind.Build,
      Microsoft.Azure.Pipelines.Deployment.Model.NoteKind.Image,
      Microsoft.Azure.Pipelines.Deployment.Model.NoteKind.Deployment,
      Microsoft.Azure.Pipelines.Deployment.Model.NoteKind.Attestation,
      Microsoft.Azure.Pipelines.Deployment.Model.NoteKind.Vulnerability
    };
  }
}

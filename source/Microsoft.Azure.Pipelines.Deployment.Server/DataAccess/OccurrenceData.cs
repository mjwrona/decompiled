// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.DataAccess.OccurrenceData
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.Model;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Pipelines.Deployment.DataAccess
{
  public class OccurrenceData
  {
    private IList<string> m_tags;

    public Guid ScopeId { get; set; }

    public string NoteName { get; set; }

    public string Name { get; set; }

    public NoteKind Kind { get; set; }

    public string ResourceId { get; set; }

    public int? FileId { get; set; }

    public IList<string> Tags
    {
      get
      {
        if (this.m_tags == null)
          this.m_tags = (IList<string>) new List<string>();
        return this.m_tags;
      }
    }
  }
}

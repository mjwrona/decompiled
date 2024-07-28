// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.Client.GraphSubjectLookupExtensions
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Graph.Client
{
  public static class GraphSubjectLookupExtensions
  {
    public static IEnumerable<SubjectDescriptor> ToSubjectDescriptors(
      this GraphSubjectLookup subjectLookup)
    {
      IEnumerable<GraphSubjectLookupKey> lookupKeys = subjectLookup.LookupKeys;
      return lookupKeys == null ? (IEnumerable<SubjectDescriptor>) null : (IEnumerable<SubjectDescriptor>) lookupKeys.Select<GraphSubjectLookupKey, SubjectDescriptor>((Func<GraphSubjectLookupKey, SubjectDescriptor>) (x => x.Descriptor)).ToList<SubjectDescriptor>();
    }
  }
}

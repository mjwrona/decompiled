// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.Commands.ICommentCommand
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExternalEvent;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Pipelines.Server.Commands
{
  public interface ICommentCommand
  {
    string CommandKeyword { get; }

    string ShortDescription { get; }

    string ExampleUsage { get; }

    bool IsValid(
      IVssRequestContext requestContext,
      string providerId,
      JObject authentication,
      ExternalPullRequestCommentEvent commentEvent,
      MergeJobStatus mergeJobStatus,
      out string responseMessage);

    bool TryExecute(
      IVssRequestContext requestContext,
      string providerId,
      JObject authentication,
      ExternalPullRequestCommentEvent commentEvent,
      MergeJobStatus mergeJobStatus,
      out string responseMessage,
      out List<Exception> exceptions);
  }
}

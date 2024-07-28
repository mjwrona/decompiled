// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.HooksPublisherInputValuesQueryController
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3575E571-FF3A-4E7B-A8CC-64FFB01E8C91
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.FormInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server
{
  [VersionedApiControllerCustomName(Area = "hooks", ResourceName = "PublisherInputValuesQuery")]
  [ClientGroupByResource("publishers")]
  public class HooksPublisherInputValuesQueryController : ServiceHooksPublisherControllerBase
  {
    [HttpPost]
    public InputValuesQuery QueryInputValues(InputValuesQuery inputValuesQuery, string publisherId)
    {
      ArgumentUtility.CheckForNull<InputValuesQuery>(inputValuesQuery, nameof (inputValuesQuery));
      ArgumentUtility.CheckForNull<IList<InputValues>>(inputValuesQuery.InputValues, "inputValuesQuery.InputValues");
      ServiceHooksPublisher publisher = this.FindPublisher(publisherId);
      inputValuesQuery.InputValues = publisher.GetInputValues(this.TfsRequestContext, inputValuesQuery.InputValues.Select<InputValues, string>((Func<InputValues, string>) (i => i.InputId)), inputValuesQuery.CurrentValues);
      return inputValuesQuery;
    }
  }
}

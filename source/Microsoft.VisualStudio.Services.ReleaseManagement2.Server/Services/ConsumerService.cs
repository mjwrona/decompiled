// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services.ConsumerService
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services
{
  public class ConsumerService : ReleaseManagement2ServiceBase
  {
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "As per VSSF service design this must be an instance member as it is a service API.")]
    public IEnumerable<string> QueryConsumers(IVssRequestContext tfsRequestContext)
    {
      if (tfsRequestContext == null)
        throw new ArgumentNullException(nameof (tfsRequestContext));
      IList<string> result = (IList<string>) new List<string>();
      Func<ConsumerSqlResourceComponent, IEnumerable<string>> action = (Func<ConsumerSqlResourceComponent, IEnumerable<string>>) (component =>
      {
        component.QueryConsumers();
        Assembly executingAssembly = Assembly.GetExecutingAssembly();
        string str = executingAssembly.GetName().Version.ToString();
        DateTime lastWriteTimeUtc = new FileInfo(executingAssembly.Location).LastWriteTimeUtc;
        result.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) "Version", (object) str));
        result.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) "LastModifiedTime", (object) lastWriteTimeUtc));
        return (IEnumerable<string>) result;
      });
      return tfsRequestContext.ExecuteWithinUsingWithComponent<ConsumerSqlResourceComponent, IEnumerable<string>>(action);
    }
  }
}

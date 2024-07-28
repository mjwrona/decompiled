// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ValidationHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal static class ValidationHelper
  {
    public static void ValidateQueueName(string paramName, string queueName, bool allowNull)
    {
      if (!allowNull)
        ArgumentUtility.CheckStringForNullOrEmpty(queueName, paramName);
      if (queueName.Length > 128 || Uri.CheckHostName(queueName) == UriHostNameType.Unknown)
        throw new ArgumentException(FrameworkResources.InvalidMessageQueueName((object) queueName), paramName);
    }

    public static bool ValidateUri(string uri) => Uri.TryCreate(uri, UriKind.Absolute, out Uri _);
  }
}

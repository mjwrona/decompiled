// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.OperationNotSupportedException
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Framework.Common
{
  [ExceptionMapping("0.0", "3.0", "OperationNotSupportedException", "Microsoft.TeamFoundation.Framework.Common.OperationNotSupportedException, Microsoft.TeamFoundation.Common, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public class OperationNotSupportedException : TeamFoundationServerException
  {
    public OperationNotSupportedException(string functionName, string serviceName)
      : base(TFCommonResources.OperationNotSupportedMessage((object) functionName, (object) serviceName))
    {
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.Patch.TestPatchOperationFailedException
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.WebApi.Patch
{
  [ExceptionMapping("0.0", "3.0", "TestPatchOperationFailedException", "Microsoft.VisualStudio.Services.WebApi.Patch.TestPatchOperationFailedException, Microsoft.VisualStudio.Services.WebApi, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public class TestPatchOperationFailedException : PatchOperationFailedException
  {
    public TestPatchOperationFailedException()
    {
    }

    public TestPatchOperationFailedException(string message)
      : base(message)
    {
    }

    public TestPatchOperationFailedException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected TestPatchOperationFailedException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}

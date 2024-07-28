// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NameResolution.NameResolutionEntryAlreadyExistsException
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.NameResolution
{
  [Serializable]
  public class NameResolutionEntryAlreadyExistsException : VssServiceException
  {
    public NameResolutionEntryAlreadyExistsException(string message)
      : base(message)
    {
    }

    public NameResolutionEntryAlreadyExistsException(string message, Exception ex)
      : base(message, ex)
    {
    }

    public NameResolutionEntryAlreadyExistsException(
      string name,
      string value,
      string conflictingValue)
      : base(NameResolutionResources.NameResolutionEntryAlreadyExistsError((object) name, (object) value, (object) conflictingValue))
    {
      this.Name = name;
      this.Value = value;
      this.ConflictingValue = conflictingValue;
    }

    protected NameResolutionEntryAlreadyExistsException(
      SerializationInfo info,
      StreamingContext context)
      : base(info, context)
    {
    }

    public string Name { get; set; }

    public string Value { get; set; }

    public string ConflictingValue { get; set; }
  }
}

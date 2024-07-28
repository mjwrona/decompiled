// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.BuildRequestValidationFailedException
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [ExceptionMapping("0.0", "3.0", "BuildRequestValidationFailedException", "Microsoft.TeamFoundation.Build.WebApi.BuildRequestValidationFailedException, Microsoft.TeamFoundation.Build2.WebApi, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public class BuildRequestValidationFailedException : BuildException
  {
    private List<BuildRequestValidationResult> m_validationResults;

    public BuildRequestValidationFailedException(string message)
      : base(message)
    {
    }

    public BuildRequestValidationFailedException(string message, Exception ex)
      : base(message, ex)
    {
    }

    public BuildRequestValidationFailedException(
      string message,
      List<BuildRequestValidationResult> validationResults)
      : base(message)
    {
      this.ValidationResults.AddRange((IEnumerable<BuildRequestValidationResult>) validationResults);
    }

    protected BuildRequestValidationFailedException(
      SerializationInfo info,
      StreamingContext context)
      : base(info, context)
    {
      this.m_validationResults = (List<BuildRequestValidationResult>) info.GetValue(nameof (ValidationResults), typeof (List<BuildRequestValidationResult>));
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);
      info.AddValue("ValidationResults", (object) this.ValidationResults);
    }

    [DataMember(Name = "ValidationResults", EmitDefaultValue = false)]
    public List<BuildRequestValidationResult> ValidationResults
    {
      get
      {
        if (this.m_validationResults == null)
          this.m_validationResults = new List<BuildRequestValidationResult>();
        return this.m_validationResults;
      }
      private set => this.m_validationResults = value;
    }
  }
}

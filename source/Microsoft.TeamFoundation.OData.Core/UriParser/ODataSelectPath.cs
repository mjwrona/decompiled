// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.ODataSelectPath
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.OData.UriParser
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "ODataSelectPathCollection just doesn't sound right")]
  public class ODataSelectPath : ODataPath
  {
    public ODataSelectPath(IEnumerable<ODataPathSegment> segments)
      : base(segments)
    {
      this.ValidatePath();
    }

    public ODataSelectPath(params ODataPathSegment[] segments)
      : base(segments)
    {
      this.ValidatePath();
    }

    private void ValidatePath()
    {
      int num = 0;
      if (this.Count == 1 && this.FirstSegment is TypeSegment)
        throw new ODataException(Strings.ODataSelectPath_CannotOnlyHaveTypeSegment);
      foreach (ODataPathSegment odataPathSegment in (ODataPath) this)
      {
        switch (odataPathSegment)
        {
          case NavigationPropertySegment _:
            if (num != this.Count - 1)
              throw new ODataException(Strings.ODataSelectPath_NavPropSegmentCanOnlyBeLastSegment);
            break;
          case OperationSegment _:
            if (num != this.Count - 1)
              throw new ODataException(Strings.ODataSelectPath_OperationSegmentCanOnlyBeLastSegment);
            break;
          case DynamicPathSegment _:
          case PropertySegment _:
          case TypeSegment _:
          case AnnotationSegment _:
            ++num;
            continue;
          default:
            throw new ODataException(Strings.ODataSelectPath_InvalidSelectPathSegmentType((object) odataPathSegment.GetType().Name));
        }
        ++num;
      }
    }
  }
}

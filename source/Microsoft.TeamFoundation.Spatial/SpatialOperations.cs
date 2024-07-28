// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.SpatialOperations
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System;

namespace Microsoft.Spatial
{
  public abstract class SpatialOperations
  {
    public virtual double Distance(Geometry operand1, Geometry operand2) => throw new NotImplementedException();

    public virtual double Distance(Geography operand1, Geography operand2) => throw new NotImplementedException();

    public virtual double Length(Geometry operand) => throw new NotImplementedException();

    public virtual double Length(Geography operand) => throw new NotImplementedException();

    public virtual bool Intersects(Geometry operand1, Geometry operand2) => throw new NotImplementedException();

    public virtual bool Intersects(Geography operand1, Geography operand2) => throw new NotImplementedException();
  }
}

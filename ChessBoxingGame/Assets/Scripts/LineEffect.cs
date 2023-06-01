using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable, VolumeComponentMenuForRenderPipeline
    ("Scripts/LineEffect", typeof(UniversalRenderPipeline))]
public class LineEffect : VolumeComponent, IPostProcessComponent
{
    public FloatParameter intensity = new FloatParameter(1);
    public bool IsActive() => true;
    public bool IsTileCompatible() => true;
}

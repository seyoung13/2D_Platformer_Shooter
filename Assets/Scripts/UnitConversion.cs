using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitConversion
{
    static private float PIXEL_PER_METER = 16;
    public float KMPH2PPS(float RUN_SPEED_KMPH)
    {
        float RUN_SPEED_MPM = (RUN_SPEED_KMPH * 1000 / 60);
        float RUN_SPEED_MPS = (RUN_SPEED_MPM / 60);
        float RUN_SPEED_PPS = RUN_SPEED_MPS * PIXEL_PER_METER;

        return RUN_SPEED_PPS;
    }

    public float RPM2RPS()
    {
        return 0;
    }

}

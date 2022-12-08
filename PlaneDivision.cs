using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;

namespace GeometriaComputacional;

public class PlaneDivision
{
    private DCEL dcel = null;
    private float sy;
    private float sx;
    private Bitmap img;

    public DCEL DCEL => dcel;

    private PlaneDivision(
        Bitmap img, DCEL dcel, float sx, float sy)
    {
        this.img = img;
        this.dcel = dcel;
        this.sx = sx;
        this.sy = sy;
    }

    public static PlaneDivision FromPolygon(
        float sx, float sy, Bitmap img,
        params Point[] pts)
    {
        return new PlaneDivision(img,
            DCEL.FromPoints(pts), sx, sy
        );
    }
}
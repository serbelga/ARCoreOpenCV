using OpenCvSharp;

public class ColorObjectCV
{
    private Scalar hsvUpperBound, hsvLowerBound;
    private Scalar contourColor;

    public ColorObjectCV(ColorCV color)
    {
        switch (color)
        {
            case ColorCV.Red:
            {
                hsvLowerBound = new Scalar(0, 70, 50);
                hsvUpperBound = new Scalar(10, 255, 255);
                contourColor = Scalar.Red;
                break;
            }
            case ColorCV.Yellow:
            {
                hsvLowerBound = new Scalar(20, 124, 123);
                hsvUpperBound = new Scalar(30, 256, 256);
                contourColor = Scalar.Orange;
                break;
            }
            case ColorCV.Blue:
            {
                hsvLowerBound = new Scalar(92, 0, 0);
                hsvUpperBound = new Scalar(124, 256, 256);
                contourColor = Scalar.Aqua;
                break;
            }
            case ColorCV.Green:
            {
                hsvLowerBound = new Scalar(34, 50, 50);
                hsvUpperBound = new Scalar(80, 220, 200);
                contourColor = Scalar.Blue;
                break;
            }
        }
    }

    public Scalar HsvUpperBound
    {
        get => hsvUpperBound;
        set => hsvUpperBound = value;
    }

    public Scalar HsvLowerBound
    {
        get => hsvLowerBound;
        set => hsvLowerBound = value;
    }

    public Scalar ContourColor
    {
        get => contourColor;
        set => contourColor = value;
    }
}
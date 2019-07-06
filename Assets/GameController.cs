using System;
using System.Collections.Generic;
using GoogleARCore;
using GoogleARCore.Examples.ComputerVision;
using OpenCvSharp;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    private TextureReader textureReader;
    private Texture2D texture;
    private int _imageWidth;
    private int _imageHeight;
    private byte[] _image;
    
    private DisplayUvCoords m_CameraImageToDisplayUvTransformation;
    private ScreenOrientation? m_CachedOrientation;
    private Vector2 m_CachedScreenDimensions = Vector2.zero;

    public RawImage imageWOProc, imageFormatCV;

    private Mat bgr, bin, hsv;
    private Texture2D colTexture;


    private ColorObjectCV red = new ColorObjectCV(ColorCV.Red);
    private ColorObjectCV yellow = new ColorObjectCV(ColorCV.Yellow);
    private ColorObjectCV blue = new ColorObjectCV(ColorCV.Blue);
    private ColorObjectCV green = new ColorObjectCV(ColorCV.Green);
    
    // Start is called before the first frame update
    void Start()
    {
    }

    /// <summary>
    /// On Awake
    /// </summary>
    private void Awake()
    {
        textureReader = GetComponent<TextureReader>();
    }

    /// <summary>
    /// On Enable
    /// </summary>
    private void OnEnable()
    {
        textureReader.OnImageAvailableCallback += OnImageAvailable;
    }

    /// <summary>
    /// On Disable
    /// </summary>
    private void OnDisable()
    {
        textureReader.OnImageAvailableCallback -= OnImageAvailable;
    }

    // Update is called once per frame
    void Update()
    {
    }

    /// <summary>
    /// Texture Reader callback
    /// </summary>
    /// <param name="format"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="pixelBuffer"></param>
    /// <param name="bufferSize"></param>
    public void OnImageAvailable(TextureReaderApi.ImageFormatType format, int width, int height, IntPtr pixelBuffer,
        int bufferSize)
    {
        if (texture == null || _image == null || _imageWidth != width || _imageHeight != height)
        {
            texture = new Texture2D(width, height, TextureFormat.RGBA32, false, false);
            _image = new byte[width * height * 4];
            _imageWidth = width; 
            _imageHeight = height;
        }
        
        System.Runtime.InteropServices.Marshal.Copy(pixelBuffer, _image, 0, bufferSize);

        // Update the rendering texture with the sampled image.
        texture.LoadRawTextureData(_image);
        texture.Apply();

        //Image without CV Processing and Shader to fit image
        imageWOProc.texture = texture;
        
        if (m_CachedOrientation != Screen.orientation ||
            m_CachedScreenDimensions.x != Screen.width ||
            m_CachedScreenDimensions.y != Screen.height)
        {
            m_CameraImageToDisplayUvTransformation = Frame.CameraImage.ImageDisplayUvs;
            m_CachedOrientation = Screen.orientation;
            m_CachedScreenDimensions = new Vector2(Screen.width, Screen.height);
        }
        
        ProcessingImage();
    }

    /// <summary>
    /// Processes the Current Frame
    /// </summary>
    private void ProcessingImage()
    {
        //Initialize hsv
        SetHSV(texture);
        
        Find(red);
        Find(yellow);
        Find(green);

        ShowImage();
        
        bgr.Release();
        bin.Release();
    }
    
    /// <summary>
    /// Initializes hsv mat to current 
    /// </summary>
    /// <param name="texture2D"></param>
    private void SetHSV(Texture2D texture2D)
    {
        // BGR Mat
        bgr = OpenCvSharp.Unity.TextureToMat(texture2D);
        // HSV Mat
        hsv = bgr.CvtColor(ColorConversionCodes.BGR2HSV);
    }

    private void ShowImage()
    {
        if (colTexture != null)
        { 
            DestroyImmediate(colTexture);
        }

        colTexture = OpenCvSharp.Unity.MatToTexture(bgr);
        
        if (m_CachedOrientation != Screen.orientation ||
            m_CachedScreenDimensions.x != Screen.width ||
            m_CachedScreenDimensions.y != Screen.height)
        {
            m_CameraImageToDisplayUvTransformation = Frame.CameraImage.ImageDisplayUvs;
            m_CachedOrientation = Screen.orientation;
            m_CachedScreenDimensions = new Vector2(Screen.width, Screen.height);
        }
        
        const string TOP_LEFT_RIGHT = "_UvTopLeftRight";
        const string BOTTOM_LEFT_RIGHT = "_UvBottomLeftRight";
        imageFormatCV.material.SetVector(TOP_LEFT_RIGHT, new Vector4(
            m_CameraImageToDisplayUvTransformation.TopLeft.x,
            m_CameraImageToDisplayUvTransformation.TopLeft.y,
            m_CameraImageToDisplayUvTransformation.TopRight.x,
            m_CameraImageToDisplayUvTransformation.TopRight.y));
        imageFormatCV.material.SetVector(BOTTOM_LEFT_RIGHT, new Vector4(
            m_CameraImageToDisplayUvTransformation.BottomLeft.x,
            m_CameraImageToDisplayUvTransformation.BottomLeft.y,
            m_CameraImageToDisplayUvTransformation.BottomRight.x,
            m_CameraImageToDisplayUvTransformation.BottomRight.y));
        
        imageFormatCV.material.SetTexture("_ImageTex", colTexture);
    }

    private void Find(ColorObjectCV colorObject)
    {
        // First binarize by color selection
        Binarize(colorObject);
        // Find the contours into binarize texture
        FindContoursBy(colorObject);
    }

    private void Binarize(ColorObjectCV colorObject)
    {
        bin = hsv.InRange(colorObject.HsvLowerBound, colorObject.HsvUpperBound);
    }
    
    private void FindContoursBy(ColorObjectCV colorObject)
    {
        List<Point[]> corners = new List<Point[]>();

        HierarchyIndex[] h;

        bin.FindContours(out var contours, out h, RetrievalModes.External, ContourApproximationModes.ApproxSimple);
        double maxArea = 0;

        for (int i = 0; i < contours.Length; i++)
        {
            double area = Cv2.ContourArea(contours[i]);
            if (area > maxArea)
            {
                maxArea = area;
                corners.Add(contours[i]);
            }
        }

        if (corners.Count > 0)
        {
            for (int i = 0; i < corners.Count; i++)
            {
                bgr.DrawContours(corners, i, colorObject.ContourColor, 5);
            }
        }
    }


}
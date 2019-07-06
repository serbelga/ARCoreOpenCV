# ARCoreOpenCV

<img src="https://gitlab.com/serbelga/arcoreopencv/raw/master/preview.gif" height="400" />

ARCore + OpenCV. TextureReader to retrieve camera frames and OpenCV to segmentate colors.

- Unity 2018.4.2f1
- [ARCore SDK for Unity v1.10.0](https://github.com/google-ar/arcore-unity-sdk/releases/tag/v1.10.0)
- [OpenCV plus Unity](https://assetstore.unity.com/packages/tools/integration/opencv-plus-unity-85928)

Setting

1. Build Settings: Switch Platform to Android
2. Player Settings:
- Disable Multithreading
- Define Package name
- Android min sdk and target sdk
- Enable ARCore
- Allow unsafe code
3. Import Package: arcore-unity-sdk
4. Import Package: opencv-plus-unity
using UnityEngine;
using UnityEditor;

namespace ResponsiveFixedScreenWidthForMobile
{
#if UNITY_EDITOR
  [CustomEditor(typeof(ResponsiveFixedScreenWidthForMobile))]
  public class ResponsiveFixedScreenWidthForMobileCustomWindow : Editor
  {
    public override void OnInspectorGUI()
    {
      base.OnInspectorGUI();

      EditorGUILayout.BeginVertical(GUI.skin.box);
      {
        EditorGUILayout.HelpBox(
        "*Be sure to set the Default values. These will be the values that apply to devices to which Fixed-Width is not applied. For example, if you are targeting only Portrait Smartphones, these values will apply to tablets. And of course, these values also apply to all landscape devices"
        + "\n\n"
        + "*The FieldOfView and OrthographicSize values in the Camera component will be overwritten."
        + "\n\n"
        + "*デフォルト値の設定は必ずしてください。これらは「横幅固定」を適用しないデバイスに適用される値になります。例えば、Portrait Smartphones のみを対象とした場合、タブレットの時はこの値が適用されます。加えて、これらの値はすべての横向きのデバイスにも適用されます"
        + "\n\n"
        + "*カメラコンポーネントの FieldOfView と OrthographicSize の値は上書きされます。"
        , MessageType.Info);
      }
      EditorGUILayout.EndVertical();
    }
  }
#endif

  [ExecuteInEditMode]
  [RequireComponent(typeof(Camera))]
  public class ResponsiveFixedScreenWidthForMobile : MonoBehaviour
  {

    //-CustomStart------------------------
    [Header("**1st CHECK target devices**")]
    public bool PortraitSmartphones = false;
    public bool PortraitTablets = false;
    //-------------------------


    [Header("**2nd SET Width value For Perspective")]
    [SerializeField]
    private float _targetFieldOfView = 60f;
    public float TargetFieldOfView
    {
      get { return _targetFieldOfView; }
      set
      {
        if (_targetFieldOfView != value)
        {
          _targetFieldOfView = value;
          RefreshCamera();
        }
      }
    }


    [Header("**2nd SET Width value For Orthographic")]
    [SerializeField]
    private float _targetOrthographicSize = 5f;
    public float TargetOrthographicSize
    {
      get { return _targetOrthographicSize; }
      set
      {
        if (_targetOrthographicSize != value)
        {
          _targetOrthographicSize = value;
          RefreshCamera();
        }
      }
    }

    [Header("**3rd SET Default value For Perspective")]
    public float defaultFieldOfView = 60f;

    [Header("**3rd SET Default value For Orthographic")]
    public float defaultOrthographicSize = 5f;


    private Camera _camera;
    private float lastAspect;


#if UNITY_EDITOR
    private void OnValidate()
    {
      RefreshCamera();
    }
#endif

    private void OnEnable()
    {
      RefreshCamera();
    }

    public void RefreshCamera()
    {
      if (!_camera)
        _camera = GetComponent<Camera>();
      AdjustCamera(_camera.aspect);
    }


    private void Update()
    {
      float currentAspect = _camera.aspect;
      if (currentAspect != lastAspect)
        AdjustCamera(currentAspect);
    }


    private void AdjustCamera(float aspect)
    {
      lastAspect = aspect;

      var deviceSize = DeviceSizeCheck.GetDeviceType(aspect);
      var doAdjust = false;

      // Debug.Log(deviceSize);

      switch (deviceSize)
      {
        case DeviceSizeCheck.DeviceType.PortraitVerticallyLongPhone:
          if (PortraitSmartphones) doAdjust = true;
          break;
        case DeviceSizeCheck.DeviceType.PortraitStandardPhone:
          if (PortraitSmartphones) doAdjust = true;
          break;
        case DeviceSizeCheck.DeviceType.PortraitTablet:
          if (PortraitTablets) doAdjust = true;
          break;
        case DeviceSizeCheck.DeviceType.Other:
          doAdjust = false;
          break;
        default:
          doAdjust = false;
          break;
      }


      if (doAdjust)
      {
        float _1OverAspect = 1f / aspect;
        _camera.fieldOfView = 2f * Mathf.Atan(Mathf.Tan(_targetFieldOfView * Mathf.Deg2Rad * 0.5f) * _1OverAspect) * Mathf.Rad2Deg;
        _camera.orthographicSize = _targetOrthographicSize * _1OverAspect;
      }
      else
      {
        _camera.fieldOfView = defaultFieldOfView;
        _camera.orthographicSize = defaultOrthographicSize;
      }

    }
  }



  public static class DeviceSizeCheck
  {

    public enum DeviceType
    {
      PortraitTablet,
      PortraitStandardPhone,
      PortraitVerticallyLongPhone,
      Other,
    }


    // ---- References ----


    // -- PortraitVerticallyLongPhone --

    // Qin2
    // 22.5:9  = 0.4

    // Xperia5
    // 21:9    = 0.428

    // iPhoneX, GooglePixel5
    // 9:19.5  = 0.461

    // GooglePixel3a:
    // 18.5:9  = 0.486

    // GooglePixel3aXL
    // 9:18    = 0.50


    // -- PortraitStandardPhone --

    // iPhone8
    // 9:16    = 0.5625

    // iPhone5
    // 40:71   = 0.563


    // -- PortraitTablet --

    // iPad mini Gen6
    // 3:4.6   = 0.652

    // iPad Pro
    // 139:199 = 0.69

    // iPad mini Air Pro
    // 3:4     = 0.75

    // ref: iPhone4s ignore (too old)
    // 2:3     = 0.66


    public static DeviceType GetDeviceType(float ratio)
    {

      if (0.39f < ratio && ratio <= 0.50f)
      {
        return DeviceType.PortraitVerticallyLongPhone;
      }

      if (0.50f < ratio && ratio <= 0.60f)
      {
        return DeviceType.PortraitStandardPhone;
      }

      if (0.60f < ratio && ratio <= 1.0f)
      {
        return DeviceType.PortraitTablet;
      }

      return DeviceType.Other;
    }
  }
}
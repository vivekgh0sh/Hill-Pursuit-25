using UnityEngine;

public static class HapticFeedback
{
    // A reference to the Android Vibrator service
#if UNITY_ANDROID && !UNITY_EDITOR
    private static AndroidJavaObject vibrator = new AndroidJavaClass("com.unity3d.player.UnityPlayer")
        .GetStatic<AndroidJavaObject>("currentActivity")
        .Call<AndroidJavaObject>("getSystemService", "vibrator");
#endif

    /// <summary>
    /// Triggers a haptic vibration with a specified duration and intensity.
    /// </summary>
    /// <param name="milliseconds">The duration of the vibration in milliseconds.</param>
    /// <param name="amplitude">The intensity of the vibration (1 to 255). 255 is max.</param>
    public static void Vibrate(long milliseconds, int amplitude)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        // Check if the device supports amplitude control
        if (vibrator.Call<bool>("hasAmplitudeControl"))
        {
            // Create a VibrationEffect with the specified settings
            AndroidJavaObject vibrationEffect = new AndroidJavaClass("android.os.VibrationEffect")
                .CallStatic<AndroidJavaObject>("createOneShot", milliseconds, amplitude);
            
            // Trigger the vibration
            vibrator.Call("vibrate", vibrationEffect);
        }
        else
        {
            // If no amplitude control, fall back to a simple vibration
            Handheld.Vibrate();
        }
#endif
    }
}
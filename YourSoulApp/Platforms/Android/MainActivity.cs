using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls.Maps;

namespace YourSoulApp;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    private const int RequestPermissionsCode = 123;
    private readonly string[] _requiredPermissions = new[]
    {
        Android.Manifest.Permission.ReadExternalStorage,
        Android.Manifest.Permission.WriteExternalStorage,
        Android.Manifest.Permission.Camera,
        Android.Manifest.Permission.AccessFineLocation,
        Android.Manifest.Permission.AccessCoarseLocation
    };

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // Initialize the Maps SDK
        // No initialization needed for Android Maps in MAUI 8

        RequestPermissions();
    }

    private void RequestPermissions()
    {
        // Filter permissions that need to be requested based on API level
        var permissionsToRequest = new List<string>();

        if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
        {
            // For Android 13+ (API 33+)
            permissionsToRequest.Add(Android.Manifest.Permission.ReadMediaImages);
            permissionsToRequest.Add(Android.Manifest.Permission.Camera);
            permissionsToRequest.Add(Android.Manifest.Permission.AccessFineLocation);
            permissionsToRequest.Add(Android.Manifest.Permission.AccessCoarseLocation);
        }
        else
        {
            // For Android 12 and below
            permissionsToRequest.Add(Android.Manifest.Permission.ReadExternalStorage);
            permissionsToRequest.Add(Android.Manifest.Permission.WriteExternalStorage);
            permissionsToRequest.Add(Android.Manifest.Permission.Camera);
            permissionsToRequest.Add(Android.Manifest.Permission.AccessFineLocation);
            permissionsToRequest.Add(Android.Manifest.Permission.AccessCoarseLocation);
        }

        RequestPermissions(permissionsToRequest.ToArray(), RequestPermissionsCode);
    }

    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
    {
        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

        if (requestCode == RequestPermissionsCode)
        {
            // Check if all permissions are granted
            bool allPermissionsGranted = grantResults.All(result => result == Permission.Granted);

            if (!allPermissionsGranted)
            {
                // Handle the case where permissions are denied
                // You might want to show a message to the user
            }
        }
    }
}

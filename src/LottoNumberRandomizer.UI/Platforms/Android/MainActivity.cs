using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;

namespace LottoNumberRandomizer.UI;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, 
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // Force respecting display cutouts
        if (OperatingSystem.IsAndroidVersionAtLeast(28))
        {
            Window?.Attributes?.LayoutInDisplayCutoutMode = LayoutInDisplayCutoutMode.Default;
        }
    }
}
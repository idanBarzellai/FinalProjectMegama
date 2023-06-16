using UnityEditor;
using UnityEngine;
using System.IO;
using System.IO.Compression;

public class BuildButtonWindow : EditorWindow
{
    private static BuildTarget previousPlatform;

    [MenuItem("Build/Build for PC or Android")]
    private static void BuildForPCAndAndroidMenuItem()
    {
        BuildOptionsWindow window = EditorWindow.GetWindow<BuildOptionsWindow>("Build Options");
        window.Show();
    }

    [MenuItem("Build/Switch to PC platform")]
    private static void SwitchToPCPlatformMenuItem()
    {
        bool confirmed = EditorUtility.DisplayDialog("Confirmation", "Switch to PC platform?", "Yes", "No");
        if (confirmed)
        {
            SwitchToPCPlatform();
        }
    }

    [MenuItem("Build/Switch to Android platform")]
    private static void SwitchToAndroidPlatformMenuItem()
    {
        bool confirmed = EditorUtility.DisplayDialog("Confirmation", "Switch to Android platform?", "Yes", "No");
        if (confirmed)
        {
            SwitchToAndroidPlatform();
        }
    }

    public static void BuildForPCAndAndroid()
    {
        bool confirmed = EditorUtility.DisplayDialog("Confirmation", "Build for PC and Android? (This might take a while...)", "Yes", "Not Now");
        if (confirmed)
        {
            BuildPlayerOptions pcBuildOptions = CreatePCBuildOptions();
            BuildPlayerOptions androidBuildOptions = CreateAndroidBuildOptions();

            BuildPipeline.BuildPlayer(pcBuildOptions);
            BuildPipeline.BuildPlayer(androidBuildOptions);

            // Return to PC platform if previous platform was PC
            if (previousPlatform == BuildTarget.StandaloneWindows)
            {
                SwitchToPCPlatform();
            }
        }
    }

    public static void BuildForAndroid()
    {
        bool confirmed = EditorUtility.DisplayDialog("Confirmation", "Build for Android? (This might take a while...)", "Yes", "Not Now");
        if (confirmed)
        {
            BuildPlayerOptions androidBuildOptions = CreateAndroidBuildOptions();
            BuildPipeline.BuildPlayer(androidBuildOptions);

            // Return to PC platform if previous platform was PC
            if (previousPlatform == BuildTarget.StandaloneWindows)
            {
                SwitchToPCPlatform();
            }
        }
    }

    public static void ZipBuildFolder(string buildFolderPath, string zipPath)
    {
        string directoryName = Path.GetDirectoryName(zipPath);
        if (!Directory.Exists(directoryName))
        {
            Directory.CreateDirectory(directoryName);
        }

        ZipFile.CreateFromDirectory(buildFolderPath, zipPath, System.IO.Compression.CompressionLevel.Optimal, false);
    }
    public static void BuildForPC()
    {
        bool confirmed = EditorUtility.DisplayDialog("Confirmation", "Build for PC? (This might take a while...)", "Yes", "Not Now");
        if (confirmed)
        {
            string gameName = PlayerSettings.productName;
            string version = PlayerSettings.bundleVersion;
            string pcBuildPath = $"Builds/PC/{gameName} {version}/{gameName}.exe";

            BuildPlayerOptions pcBuildOptions = CreatePCBuildOptions();
            BuildPipeline.BuildPlayer(pcBuildOptions);
            
            string zipPath = string.Format("Builds/PC/{0} {1}/{0} {1}.zip", gameName, version);
            ZipBuildFolder(pcBuildPath, zipPath);
        }
    }

    private static BuildPlayerOptions CreatePCBuildOptions()
    {
        string gameName = PlayerSettings.productName;
        string version = PlayerSettings.bundleVersion;
        string pcBuildPath = $"Builds/PC/{gameName} {version}/{gameName}.exe";

        BuildPlayerOptions pcBuildOptions = new BuildPlayerOptions();
        pcBuildOptions.scenes = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes);
        pcBuildOptions.locationPathName = pcBuildPath;
        pcBuildOptions.target = BuildTarget.StandaloneWindows;
        pcBuildOptions.options = BuildOptions.None;
        

        return pcBuildOptions;
    }

    private static BuildPlayerOptions CreateAndroidBuildOptions()
    {
        string gameName = PlayerSettings.productName;
        string version = PlayerSettings.bundleVersion;
        string androidBuildPath = $"Builds/Android/{gameName}.apk";

         BuildPlayerOptions androidBuildOptions = new BuildPlayerOptions();
        androidBuildOptions.scenes = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes);
        androidBuildOptions.locationPathName = androidBuildPath;
        androidBuildOptions.target = BuildTarget.Android;
        androidBuildOptions.options = BuildOptions.None;


        return androidBuildOptions;
    }

    private static void SwitchToPCPlatform()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows);
        previousPlatform = BuildTarget.StandaloneWindows;
    }

    private static void SwitchToAndroidPlatform()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
        previousPlatform = BuildTarget.Android;
    }
}

public class BuildOptionsWindow : EditorWindow
{
    private const int buttonHeight = 40;
    private const int buttonMargin = 10;

    private void OnGUI()
    {
        GUILayout.Label("Build Options", EditorStyles.boldLabel);

        if (GUILayout.Button("Build for PC and Android", GUILayout.Height(buttonHeight)))
        {
            BuildButtonWindow.BuildForPCAndAndroid();
            Close();
        }

        GUILayout.Space(buttonMargin);

        if (GUILayout.Button("Build for Android", GUILayout.Height(buttonHeight)))
        {
            BuildButtonWindow.BuildForAndroid();
            Close();
        }

        GUILayout.Space(buttonMargin);

        if (GUILayout.Button("Build for PC", GUILayout.Height(buttonHeight)))
        {
            BuildButtonWindow.BuildForPC();
            Close();
        }
    }
}

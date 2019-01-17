using UnityEditor;
using System.Diagnostics;

public class Builder
{
    [MenuItem("MyTools/Windows Build With Postprocess")]
    public static void BuildGame()
    {
        // Get filename.
        string path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "");
        string[] levels = new string[] { "Assets/online.unity"};

        // Build player.  PlayerSettings.SetPropertyInt("ScriptingBackend", (int)ScriptingImplementation.Mono2x, BuildTargetGroup.Standalone); // Ideal setting for Windows
        
        PlayerSettings.defaultScreenHeight = 768;
        PlayerSettings.defaultScreenWidth = 1024;
        PlayerSettings.runInBackground = true;
        PlayerSettings.displayResolutionDialog = ResolutionDialogSetting.Disabled;
        PlayerSettings.resizableWindow = true;
        BuildPipeline.BuildPlayer(levels, path + "/BuiltGame.exe", BuildTarget.StandaloneWindows, BuildOptions.Development);

        // Copy a file from the project folder to the build folder, alongside the built game.
      //  FileUtil.CopyFileOrDirectory("Assets/Templates/Readme.txt", path + "Readme.txt");

        // Run the game (Process class from System.Diagnostics).
        Process server1 = new Process();
        server1.StartInfo.FileName = path + "/BuiltGame.exe";
        server1.StartInfo.Arguments = "launchedFromMenu";
        server1.StartInfo.Arguments += " server";
        server1.Start();
        Process client1 = new Process();
        client1.StartInfo.FileName = path + "/BuiltGame.exe";
        client1.StartInfo.Arguments = "launchedFromMenu";
        client1.StartInfo.Arguments += " client";
        client1.Start();

    }

    [MenuItem("MyTools/Launch client and server")]
    public static void LaunchClients()
    {
        // Get filename.
        string path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "");

        // Run the game (Process class from System.Diagnostics).
        Process server1 = new Process();
        server1.StartInfo.FileName = path + "/BuiltGame.exe";
        server1.StartInfo.Arguments = "launchedFromMenu";
        server1.StartInfo.Arguments += " server";
        server1.Start();
        Process client1 = new Process();
        client1.StartInfo.FileName = path + "/BuiltGame.exe";
        client1.StartInfo.Arguments = "launchedFromMenu";
        client1.StartInfo.Arguments += " client";
        client1.Start();

    }
}


using Ookii.Dialogs;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AssetBundlePacker
{
    public static readonly string COMMON_BUILD_PATH_STORE = @"Assets\BuildLocation.txt";

    public static bool RequestBuild = false;

    [MenuItem("Sonic P-06/Save")]
    public static void Save()
    {
        if (string.IsNullOrEmpty(GetBuildPath()) && !PromptBuildPath())
            return;
        CheckAssetBundleLabels();
        OpenMetadataEditor();
    }

    [MenuItem("Sonic P-06/Save As...")]
    public static void SaveAs()
    {
        if (!PromptBuildPath())
            return;
        CheckAssetBundleLabels();
        OpenMetadataEditor();
    }
    public static void CheckAssetBundleLabels()
    {
        // Get all Scenes created & Saved to the build list
        int totalScenes = SceneManager.sceneCountInBuildSettings;

        //Temporarily set all asset bundle labels to empty
        for (int i = 0; i < totalScenes; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            string currentScene = SceneManager.GetActiveScene().name;

            if (currentScene == sceneName)
            {
                AssetImporter.GetAtPath(scenePath).assetBundleName = "~temp.ab";
            }
        }

    }
    public static string GetBuildPath()
    {
        if (File.Exists(COMMON_BUILD_PATH_STORE))
        {
            string location = File.ReadAllText(COMMON_BUILD_PATH_STORE);
            {
                if (Directory.Exists(location))
                    return location;
            }
        }

        return string.Empty;
    }

    public static bool PromptBuildPath()
    {
        var browser = new VistaFolderBrowserDialog()
        {
            Description = "Select your build directory...",
            UseDescriptionForTitle = true
        };

        if (browser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            File.WriteAllText(COMMON_BUILD_PATH_STORE, browser.SelectedPath);

            return true;
        }

        return false;
    }

    public static string GetMetadataPath()
    {
        if (string.IsNullOrEmpty(GetBuildPath()) && !PromptBuildPath())
            return string.Empty;

        return Path.Combine(GetBuildPath(), "mod.json");
    }

    private static void OpenMetadataEditor()
    {

        string metadata = GetMetadataPath();
        {
            if (File.Exists(metadata))
                MetadataEditor.Metadata = Metadata.Load(metadata);

            RequestBuild = true;

            MetadataEditor.ShowWindow();
        }
    }

    public static void BuildAssetBundles()
    {
        string buildPath = GetBuildPath();
        {
            // Write metadata.
            Metadata.Save(MetadataEditor.Metadata, Path.Combine(buildPath, "mod.json"));

            // Build asset bundle.
            BuildPipeline.BuildAssetBundles(buildPath, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);

            // Refresh assets after saving.
            AssetDatabase.Refresh();

            // Clean up the output directory.
            CleanUpBuildDirectory(buildPath);

        }
    }

    private static void CleanUpBuildDirectory(string directory)
    {
        string tempAssetBundle = Path.Combine(directory, "~temp.ab");
        {
            if (File.Exists(tempAssetBundle))
            {
                // Rename temporary asset bundle to final name.
                File.Copy
                (
                    tempAssetBundle,
                    Path.Combine(directory, MetadataEditor.Metadata.AssetBundle),
                    true
                );

                // Delete temporary asset bundle.
                File.Delete(tempAssetBundle);
            }
        }

        // Delete temporary manifest files.
        foreach (string manifest in Directory.GetFiles(directory, "*.manifest"))
            File.Delete(manifest);

        string temp = Path.Combine(directory, Path.GetFileName(directory));
        {
            // Delete erroneous asset bundle named after the directory.
            if (File.Exists(temp))
                File.Delete(temp);
        }
        // set current active scene ASB label to empty after build
        AssetImporter.GetAtPath(SceneManager.GetActiveScene().path).assetBundleName = string.Empty;
    }
}

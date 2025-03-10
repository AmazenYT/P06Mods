using System.IO;
using UnityEditor;
using UnityEngine;

public class MetadataEditor : EditorWindow
{
    public static Metadata Metadata = new Metadata();

    public static Vector2 ScrollPosition = Vector2.zero;

    public MetadataEditor()
    {
        // Set window parameters.
        minSize = new Vector2(450, 650);

        string metadata = AssetBundlePacker.GetMetadataPath();
        {
            // Set metadata content.
            if (File.Exists(metadata))
                Metadata = Metadata.Load(metadata);
        }
    }

    [MenuItem("Sonic P-06/Metadata Editor", false, 0)]
    public static void ShowWindow()
        => GetWindow(typeof(MetadataEditor), true, "Metadata Editor");

    void OnEnable()
    {
        // Set editor styles.
        EditorStyles.textField.wordWrap = true;
    }

    void OnGUI()
    {
        ScrollPosition = GUILayout.BeginScrollView(ScrollPosition);

        Metadata.Title = EditorGUILayout.TextField("Title", Metadata.Title);
        EditorGUILayout.HelpBox("The name pertaining to this stage.", MessageType.Info);

        GUILayout.Space(20);

        Metadata.Version = EditorGUILayout.TextField("Version", Metadata.Version);
        EditorGUILayout.HelpBox("The version given to this revision of the stage.", MessageType.Info);

        GUILayout.Space(20);

        Metadata.Author = EditorGUILayout.TextField("Author", Metadata.Author);
        EditorGUILayout.HelpBox("The author of this stage.", MessageType.Info);

        GUILayout.Space(20);

        Metadata.Date = EditorGUILayout.TextField("Date", Metadata.Date);
        EditorGUILayout.HelpBox("The date this stage was created on (DD/MM/YYYY).", MessageType.Info);

        GUILayout.Space(20);

        Metadata.Description = EditorGUILayout.TextField("Description", Metadata.Description, GUILayout.Height(45));
        EditorGUILayout.HelpBox("A short description about this stage (120 character limit).", MessageType.Info);

        GUILayout.Space(20);

        Metadata.Mission = EditorGUILayout.TextField("Mission", Metadata.Mission, GUILayout.Height(45));
        EditorGUILayout.HelpBox("A brief goal for this stage to display on the loading screens and results screen (50 character limit).", MessageType.Info);

        GUILayout.Space(20);

        Metadata.Thumbnail = EditorGUILayout.TextField("Thumbnail", Metadata.Thumbnail);
        EditorGUILayout.HelpBox("The file name of an image to display as the background thumbnail for this stage.", MessageType.Info);

        GUILayout.Space(20);

        Metadata.LoadingThumbnail = EditorGUILayout.TextField("Loading Screen", Metadata.LoadingThumbnail);
        EditorGUILayout.HelpBox("The file name of an image to display as the loading screen for this stage.", MessageType.Info);

        GUILayout.Space(20);

        Metadata.AssetBundle = EditorGUILayout.TextField("Asset Bundle", Metadata.AssetBundle);
        EditorGUILayout.HelpBox("The file name of the asset bundle to load for this stage.", MessageType.Info);

        GUILayout.Space(20);

        Metadata.GameBananaItemID = EditorGUILayout.IntField("GameBanana Item ID", Metadata.GameBananaItemID);
        EditorGUILayout.HelpBox
        (
            "The item ID usually located at the end of a URL pertaining to your GameBanana submission.\n\n" +
            "" +
            "If you don't intend to update, leave this as -1, but it is highly recommended that you set this up.\n\n" +
            "" +
            "Example;\n" +
            "https://gamebanana.com/mods/400338\n" +
            "\t\t\t     ^ - Item ID starts here",
            MessageType.Info
        );

        GUILayout.Space(20);

        Metadata.GameBananaFileIndex = EditorGUILayout.IntField("GameBanana File Index", Metadata.GameBananaFileIndex);
        EditorGUILayout.HelpBox
        (
            "The index of the file in the downloads section on GameBanana pertaining to this revision of the mod.\n\n" +
            "" +
            "If you only have one download, this will always be zero.\n\n" +
            "" +
            "If you have multiple downloads, count up from zero starting with the first download and set this to the number you land on that is most relevant to this revision.\n\n" +
            "" +
            "Example;\n" +
            "test_mod_sonic_version.zip - File Index 0\n" +
            "test_mod_shadow_version.zip - File Index 1\n" +
            "test_mod_silver_version.zop - File Index 2",
            
            MessageType.Info
        );

        GUILayout.Space(20);

        Metadata.RankData.TimeBonusBase = EditorGUILayout.IntField("Time Bonus Base", Metadata.RankData.TimeBonusBase);
        EditorGUILayout.HelpBox("The base score before the multiplied time is subtracted from it.", MessageType.Info);

        GUILayout.Space(20);

        Metadata.RankData.TimeBonusRate = EditorGUILayout.IntField("Time Bonus Rate", Metadata.RankData.TimeBonusRate);
        EditorGUILayout.HelpBox("The multiplier for the current time (higher values will subtract more from the base score).", MessageType.Info);

        GUILayout.Space(20);

        Metadata.RankData.RingBonusRate = EditorGUILayout.IntField("Ring Bonus Rate", Metadata.RankData.RingBonusRate);
        EditorGUILayout.HelpBox("The multiplier for the current rings.", MessageType.Info);

        GUILayout.Space(20);

        Metadata.RankData.Rank_S = EditorGUILayout.IntField("S Rank Score Threshold", Metadata.RankData.Rank_S);
        EditorGUILayout.HelpBox("The minimum score required for an S rank.", MessageType.Info);

        GUILayout.Space(20);

        Metadata.RankData.Rank_A = EditorGUILayout.IntField("A Rank Score Threshold", Metadata.RankData.Rank_A);
        EditorGUILayout.HelpBox("The minimum score required for an A rank.", MessageType.Info);

        GUILayout.Space(20);

        Metadata.RankData.Rank_B = EditorGUILayout.IntField("B Rank Score Threshold", Metadata.RankData.Rank_B);
        EditorGUILayout.HelpBox("The minimum score required for a B rank.", MessageType.Info);

        GUILayout.Space(20);

        Metadata.RankData.Rank_C = EditorGUILayout.IntField("C Rank Score Threshold", Metadata.RankData.Rank_C);
        EditorGUILayout.HelpBox("The minimum score required for a C rank.", MessageType.Info);

        GUILayout.Space(20);

        Metadata.RankData.Rank_D = EditorGUILayout.IntField("D Rank Score Threshold", Metadata.RankData.Rank_D);
        EditorGUILayout.HelpBox("The minimum score required for a D rank.", MessageType.Info);

        GUILayout.EndScrollView();

        if (GUILayout.Button("Save"))
        {
            string metadata = AssetBundlePacker.GetMetadataPath();

            if (!string.IsNullOrEmpty(metadata))
            {
                Metadata.Save(Metadata, metadata);

                if (AssetBundlePacker.RequestBuild)
                    AssetBundlePacker.BuildAssetBundles();

                Close();
            }
        }

        if (GUILayout.Button("Restore defaults"))
            Metadata = new Metadata();
    }

    void OnDestroy()
    {
        AssetBundlePacker.RequestBuild = false;
    }
}
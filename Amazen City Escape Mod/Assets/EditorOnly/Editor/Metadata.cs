using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;

public class Metadata
{
    public string Type = "Stage";

    public string Title { get; set; } = "Untitled";

    public string Version { get; set; } = "1.0";

    public string Author { get; set; } = Environment.UserName;

    public string Date { get; set; } = DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

    public string Description { get; set; }

    public string Mission { get; set; }

    public string Thumbnail { get; set; }

    public string LoadingThumbnail { get; set; }

    public string AssetBundle { get; set; }

    public int GameBananaItemID { get; set; } = -1;

    public int GameBananaFileIndex { get; set; } = 0;

    public RankData RankData { get; set; } = new RankData();

    public static Metadata Load(string path)
        => JsonConvert.DeserializeObject<Metadata>(File.ReadAllText(path));

    public static void Save(Metadata metadata, string path)
    {
        // Use title as asset bundle name.
        if (string.IsNullOrEmpty(MetadataEditor.Metadata.AssetBundle))
            MetadataEditor.Metadata.AssetBundle = StringHelper.RemoveInvalidFileNameChars(MetadataEditor.Metadata.Title).Replace(" ", "");

        File.WriteAllText(path, JsonConvert.SerializeObject(metadata, Formatting.Indented));
    }
}
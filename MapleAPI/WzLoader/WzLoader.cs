namespace MapleAPI.WzLoader;

public class WzLoader
{
    private static WzLoader? _instance;
    public static WzLoader Instance
    {
        get
        {
            if (_instance == null) _instance = new WzLoader();
            return _instance;
        }
    }
    
    private WzLoader()
    {
        DataPath = "";
        ExtractTargets = new List<string>();
    }


    private string DataPath
    {
        get => WzExtractor.dataPath == null ? "" : WzExtractor.dataPath;
        set => WzExtractor.dataPath = value;
    }
    private List<string> ExtractTargets;
    
    public WzLoader SetDataPath(string path)
    {
        DataPath = path;
        return this;
    }

    public WzLoader AddExtract(string path)
    {
        ExtractTargets.Add(path);
        return this;
    }

    public void Extract()
    {
        new Thread(() =>
        {
            StringExtractor sEx = new StringExtractor();
            sEx.Extract();

            foreach (string value in ExtractTargets)
            {
                string clazz = value.Split("\\")[0].ToLower();
                string args = value.Split("\\").Length > 1 ? value.Split("\\")[1] : "";
                switch (clazz.ToLower())
                {
                    case "skill":
                        if (File.Exists("./SkillExtractorResult.json")) break;
                        SkillExtractor skEx = new SkillExtractor();
                        skEx.BindStringExtractor(sEx);
                        skEx.Extract();
                        break;
                    case "character":
                        if (File.Exists("./CharacterExtractorResult.json")) break;
                        CharacterExtractor cEx = new CharacterExtractor();
                        cEx.SetArgs(args);
                        cEx.BindStringExtractor(sEx);
                        cEx.Extract();
                        break;
                    case "item":
                        if (File.Exists("./ItemExtractorResult.json")) break;
                        ItemExtractor iEx = new ItemExtractor();
                        iEx.SetArgs(args);
                        iEx.BindStringExtractor(sEx);
                        iEx.Extract();
                        break;
                }
            }
            
            Console.WriteLine("Extract Complete");
        }).Start();
    }
}
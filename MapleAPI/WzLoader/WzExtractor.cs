using System.Drawing;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using WzComparerR2.WzLib;

namespace MapleAPI.WzLoader;

#pragma warning disable CA1416
public abstract class WzExtractor
{
    internal static string? dataPath;

    private Dictionary<string, int> nameDupChecker = new();
    private string arguments = "";
    private protected StringExtractor? sEx;
    
    private Wz_Structure OpenWzFile(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"File not exists. {path}");
        }
        
        Wz_Structure wz = new Wz_Structure();
        if (wz.IsKMST1125WzFormat(path))
        {
            wz.LoadKMST1125DataWz(path);
        }
        else
        {
            wz.Load(path);
        }

        return wz;
    }
    
    private protected string ExportPng(Wz_Png png, string dirName, string fileName)
    {
        fileName = fileName
            .Replace("<", "[")
            .Replace(">", "]")
            .Replace(":", "-")
            .Replace("\\", " ")
            .Replace("\"", "")
            .Replace("?", "")
            .Replace("/", " ");
        
        string outDir = $@".\ImageOut\{dirName}";
        if (!Directory.Exists(outDir)) Directory.CreateDirectory(outDir);
            
        string exportPth = $"{outDir}/{fileName}";
        while (nameDupChecker.ContainsKey(exportPth)) exportPth = $"{outDir}/{fileName}_{++nameDupChecker[exportPth]:00}";
        nameDupChecker.Add(exportPth, 0);

        exportPth += ".png";
        Bitmap bitmap = png.ExtractPng();
        try
        {
            bitmap.Save(exportPth);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{ex.GetType()} occurs from {exportPth}.");
        } 
        bitmap.Dispose();
        return exportPth;
    }

    private protected void MergeJsonObject(JsonObject baseObject, JsonObject target)
    {
        foreach (KeyValuePair<string, JsonNode?> pairs in target)
        {
            if (baseObject.ContainsKey(pairs.Key))
            {
                if (baseObject[pairs.Key] is JsonObject jo && pairs.Value is JsonObject)
                    MergeJsonObject(jo, JsonNode.Parse(pairs.Value.AsObject().ToJsonString())!.AsObject());
                else if(!baseObject[pairs.Key]!.ToString().StartsWith("./ImageOut/"))
                    baseObject[pairs.Key] = JsonNode.Parse(pairs.Value!.ToJsonString());
            }
            else baseObject.Add(pairs.Key, JsonNode.Parse(pairs.Value!.ToJsonString()));
        }
    }

    private protected string ClearString(string str)
    {
        str = str.Replace("\\ ", "\\")
            .Replace("\r", "")
            .Replace("\n", "")
            .Replace("\\#", "#")
            .Replace("\\c", "")
            .Replace("\"", "\'")
            .Trim('\t', ' ', (char) 9, '\r', '\n')
            .TrimEnd('\\');
        return Regex.Replace(str, @"\\(?!n|r)", "");
    } 
    
    private void RecursiveFind(Wz_Node node, string parent)
    {
        string self = $"{parent}\\{node.Text}";

        if (node.Value is Wz_Image image && image.TryExtract())
            ProcessImage(self, image);
        else
            foreach (Wz_Node sNode in node.Nodes)
                RecursiveFind(sNode, self);
    }
    
    private void RecursiveImageFind(string path, Wz_Node node)
    {
        if (IsTargetImage(node.Text))
            TryAddData($"{path}\\{node.Text}", node);
        else
            foreach(Wz_Node child in node.Nodes)
                RecursiveImageFind($"{path}\\{child.Text}", child);
    }
    
    private protected virtual void ProcessImage(string path, Wz_Image image)
    {
        foreach (Wz_Node node in image.Node.Nodes)
            RecursiveImageFind($"{path}\\{node.Text}", node);
    }

    protected abstract string GetWzPath();
    protected abstract void TryAddData(string path, Wz_Node node);
    protected virtual bool IsTargetImage(string name)
    {
        return true;
    }
    protected virtual void PreExtract()
    {
    }
    protected virtual void PostExtract()
    {
    }

    public void Extract()
    {
        PreExtract();
        
        if (arguments.Equals(""))
        {
            Wz_Structure wz = OpenWzFile(GetWzPath());
            foreach (Wz_Node sNode in wz.WzNode.Nodes)
                RecursiveFind(sNode, wz.WzNode.Text);
        }
        else
        {
            string[] split = arguments.Split(",");
            string wzPath = GetWzPath();
            int lastSepIdx = wzPath.LastIndexOf('\\');
            foreach (string arg in split)
            {
                string trim = arg.Trim('[', ']');
                string newWzPath = $@"{wzPath.Substring(0, lastSepIdx)}\{trim}\{trim}.wz";
                Wz_Structure wz = OpenWzFile(newWzPath);
                foreach (Wz_Node sNode in wz.WzNode.Nodes)
                    RecursiveFind(sNode, wz.WzNode.Text);
            }
        }

        PostExtract();
    }

    public void SetArgs(string args)
    {
        arguments = args;
    }


    public void BindStringExtractor(StringExtractor stringExtractor)
    {
        sEx = stringExtractor;
    }

}
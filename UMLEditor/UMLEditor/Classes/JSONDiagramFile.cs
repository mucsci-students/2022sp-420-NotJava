namespace UMLEditor.Classes;

using System.IO;
using UMLEditor.Interfaces;
using Newtonsoft.Json;

public class JSONDiagramFile : IDiagramFile
{

    public Diagram LoadDiagram(string fromFile)
    {
        return JsonConvert.DeserializeObject<Diagram>(File.ReadAllText(fromFile));
    }
    
    public void SaveDiagram(ref Diagram toSave, string fileName)
    {
        File.WriteAllText(fileName, JsonConvert.SerializeObject(toSave));
    }

}
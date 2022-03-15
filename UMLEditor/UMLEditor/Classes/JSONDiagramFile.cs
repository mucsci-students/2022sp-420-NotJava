namespace UMLEditor.Classes;

using System.IO;
using Interfaces;
using Newtonsoft.Json;

/// <summary>
/// Ctor for JSONDiagramFile
/// </summary>
public class JSONDiagramFile : IDiagramFile
{
    /// <inheritdoc />
    public Diagram? LoadDiagram(string fromFile)
    {
        return JsonConvert.DeserializeObject<Diagram>(File.ReadAllText(fromFile));
    }

    /// <inheritdoc />
    public void SaveDiagram(ref Diagram toSave, string fileName)
    {
        File.WriteAllText(fileName, JsonConvert.SerializeObject(toSave));
    }

}
namespace UMLEditor.Interfaces;

using Classes;

/// <summary>
/// IDiagramFile
/// </summary>
public interface IDiagramFile
{

    /// <summary>
    /// Loads a diagram from the provided class
    /// </summary>
    /// <param name="fromFile">The file name to load from</param>
    /// <returns>The diagram loaded from the provided file</returns>
    public Diagram? LoadDiagram(string fromFile);

    /// <summary>
    /// Saves the provided diagram to the provided file.
    /// Creates new file if a file by that name does not already exist
    /// </summary>
    /// <param name="toSave">The diagram to save</param>
    /// <param name="fileName">The file to save the diagram to</param>
    public void SaveDiagram(ref Diagram toSave, string fileName);

}
using System.Drawing;

if (args.Length == 0)
{
    Console.WriteLine("No file path provided.");
    Console.WriteLine("Usage: pngtoico path/to/png.png");
    return;
}

string filePath = args[0];

// Check if the file exists
if (!File.Exists(filePath))
{
    Console.WriteLine($"The file '{filePath}' does not exist.");
    return;
}

// Check if the file is a PNG file
if (Path.GetExtension(filePath).ToLower() != ".png")
{
    Console.WriteLine("The file is not a PNG file. Do you still want to attempt conversion? (Y):");
    int res = Console.Read();
    if (Char.ToLower(Convert.ToChar(res)) != 'y') return;
}

// Determine the output file path
string outputFilePath = args.Length > 1 ? args[1] : Path.ChangeExtension(filePath, ".ico");

// Check if the output path is a directory
if (Directory.Exists(outputFilePath))
{
    string fileName = Path.GetFileNameWithoutExtension(filePath) + ".ico";
    outputFilePath = Path.Combine(outputFilePath, fileName);
}

try
{
    using FileStream stream = File.OpenRead(filePath);
    var image = new Bitmap(stream);

    using FileStream output = File.OpenWrite(outputFilePath);
    Icon.FromHandle(image.GetHicon()).Save(output);

    Console.WriteLine($"The ICO file has been saved as: {outputFilePath}");
}
catch (Exception e)
{
    Console.WriteLine("An error occurred: " + e.Message);
}